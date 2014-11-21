using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Dialect;
using TCPServer.Data.Model;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;
using NHibernate.Linq;
using System.IO;
using NHibernate.UserTypes;
using NHibernate.SqlTypes;
using System.Data;
using NHibernate;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;

namespace TCPServer.Data
{
    public class ServerDB
    {
        ServerDB()
        {
        }

        public static ServerDB Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested() { }
            internal static readonly ServerDB instance = new ServerDB();
        }

        private NHibernate.ISessionFactory _sessionFactory;
        public NHibernate.ISessionFactory SessionFactory 
        {
            get
            {
                if (_sessionFactory == null)
                {
                    InitSessionFactory();
                }
                return _sessionFactory;
            }
            private set
            {
                _sessionFactory = value;
            }
        }

        public void InitSessionFactory()
        {
            SessionFactory = CommonConfiguration(SqlExpress(), null, CommonConventions)
                    .BuildSessionFactory();
        }

        public void InitSessionFactory(Action<MsSqlConnectionStringBuilder> connectionStringBuilder)
        {
            SessionFactory = CommonConfiguration(SqlServer(connectionStringBuilder), null, CommonConventions)
                    .BuildSessionFactory();
        }

        private static FluentConfiguration CommonConfiguration(FluentConfiguration configuration, TextWriter mappingExport = null, Action<SetupConventionFinder<AutoPersistenceModel>> conventions = null)
        {
            var autoMapping = AutoMap.AssemblyOf<User>(new CustomAutoConfiguration());
            //var postanswer = AutoMap.AssemblyOf<GroupPostAnswer>(new CustomAutoConfiguration()).Conventions.Add<StringColumnLengthConvention>();
            //var post = AutoMap.AssemblyOf<GroupPost>(new CustomAutoConfiguration()).Conventions.Add<StringColumnLengthConvention>();
            //var conversation = AutoMap.AssemblyOf<ConversationMessage>(new CustomAutoConfiguration()).Conventions.Add<StringColumnLengthConvention>();

            OverrideAutoMapping(autoMapping, conventions); 
            //OverrideAutoMapping(post, conventions);
            //OverrideAutoMapping(postanswer, conventions);
            //OverrideAutoMapping(conversation, conventions);

            configuration = configuration
                .Mappings(m => m.AutoMappings.Add(autoMapping));
                //.Mappings(m =>m.AutoMappings.Add(post))
                //.Mappings(m => m.AutoMappings.Add(postanswer))
                //.Mappings(m => m.AutoMappings.Add(conversation));
            ExportMappings(configuration, mappingExport);
            return configuration;
        }

        private static FluentConfiguration SqlServer(Action<MsSqlConnectionStringBuilder> connectionStringBuilder)
        {
            var configuration = Fluently.Configure()
                                    .Database(MsSqlConfiguration.MsSql2008
                                    .ConnectionString(connectionStringBuilder));
            return configuration;
        }

        private static FluentConfiguration SqlExpress()
        {
            var configuration = Fluently.Configure()
                                    .Database(MsSqlConfiguration.MsSql2008
                                    .ConnectionString(m => m.Server(@".\SqlExpress")
                                    .Database("CRSim")
                                    .TrustedConnection()));
            return configuration;
        }

        private static FluentConfiguration SqLiteInFile()
        {
        	var filename = "test.db3";
        	if (File.Exists(filename)) File.Delete(filename);
            var configuration = Fluently.Configure()
                                    .Database(SQLiteConfiguration.Standard.UsingFile(filename));
            return configuration;
        }

		private static FluentConfiguration SqlCeInFile(Action<string> dbCreate)
		{
			var dbPath = "Data Source=test.sdf";
			if (File.Exists("test.sdf")) File.Delete("test.sdf");

			var configuration = Fluently.Configure()
									.Database(MsSqlCeConfiguration.Standard.Dialect<MsSqlCe40Dialect>().ConnectionString(dbPath));
			dbCreate(dbPath);
			return configuration;
		}

		private static FluentConfiguration SqLiteInMemory()
		{
			var configuration = Fluently.Configure()
									.Database(SQLiteConfiguration.Standard.InMemory());
			return configuration;
		}

        private static void ExportMappings(FluentConfiguration configuration, TextWriter mappingExport)
        {
            if (mappingExport == null) return;
            configuration.Mappings(m =>
            {
                m.AutoMappings.ExportTo(mappingExport);
            });
        }

        private static void OverrideAutoMapping(AutoPersistenceModel autoMapping, Action<SetupConventionFinder<AutoPersistenceModel>> conventions)
        {
			if(conventions != null)
				conventions(autoMapping.Conventions);

            const int nvarcharMax = 4096; // anything over 4000 should map to nvarchar(max)
            
        }

        public void Create(Action<MsSqlConnectionStringBuilder> connectionStringBuilder)
        {
            SessionFactory = CommonConfiguration(SqlServer(connectionStringBuilder), null, CommonConventions)
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                .BuildSessionFactory();
        }

        public void DropCreate(Action<MsSqlConnectionStringBuilder> connectionStringBuilder)
        {
            CommonConfiguration(SqlServer(connectionStringBuilder), null, CommonConventions)
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Drop(false, true))
                .BuildSessionFactory().OpenSession().Close();
            Create(connectionStringBuilder);
        }

        public SessionFactoryExports Update(Action<MsSqlConnectionStringBuilder> connectionStringBuilder)
        {
            var mappings = new StringWriter();
            var script = new StringWriter();
            SessionFactory = CommonConfiguration(SqlServer(connectionStringBuilder), mappings, CommonConventions)
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(s => script.WriteLine(s), true))
                .BuildSessionFactory();
            return new SessionFactoryExports
            {
                Mappings = mappings.ToString(),
				Script = script.ToString(),
            };

        }

		public void CommonConventions(SetupConventionFinder<AutoPersistenceModel> conventions)
		{
			conventions.Add(new AssignedIdConvention());
            //conventions.Add(new StringColumnLengthConvention());
		}


        //public NHibernate.ISessionFactory TestInMemory()
        //{
        //    var schema = new StringWriter();
        //    var cfg =  CommonConfiguration(SqLiteInMemory(), null, conventions =>
        //        {
        //            conventions.Add(
        //                new NormalizedDateTimeUserTypeConvention(),
        //                new NormalizedNullableDateTimeUserTypeConvention());
        //            CommonConventions(conventions);
        //        });
        //    cfg.ExposeConfiguration(c => c.Properties["connection.release_mode"] = "on_close");
        //    var sf = cfg.BuildSessionFactory();
        //    var session = sf.OpenSession();
        //    cfg.ExposeConfiguration(c => new SchemaExport(c).Execute(false, true, false, session.Connection, schema)).BuildConfiguration();
        //    var export = schema.ToString();
        //    return new TestSessionFactory(session);
        //}

		
        //public NHibernate.ISessionFactory TestInFile(Action<string> dbCreate)
        //{
        //    var cfg = CommonConfiguration(SqlCeInFile(dbCreate), null, conventions =>
        //    {
        //        conventions.Add(
        //            new NormalizedDateTimeUserTypeConvention(),
        //            new NormalizedNullableDateTimeUserTypeConvention());
        //        CommonConventions(conventions);
        //    });
        //    cfg.ExposeConfiguration(c => new SchemaExport(c).Execute(false, true, false)).BuildConfiguration();
        //    return cfg.BuildSessionFactory();
        //}
    }

	public class AssignedIdConvention : IIdConvention
	{
		public void Apply(IIdentityInstance instance)
		{
			instance.GeneratedBy.Assigned();
		}
	}

    //public class StringColumnLengthConvention : IPropertyConvention, IPropertyConventionAcceptance
    //{
    //    public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
    //    {
    //        criteria.Expect(x => x.Type == typeof(string)).Expect(x => x.Length == 0);
    //    }
    //    public void Apply(IPropertyInstance instance)
    //    {
    //        instance.Length(10000);
    //    }
    //}
}