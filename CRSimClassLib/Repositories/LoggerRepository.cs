using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CRSimClassLib.Repositories
{
    public class LoggerRepository
    {
        private static string _file;
        private static LoggerRepository instance;

        /// <summary>
        /// specifies upto which priorty types of logs are logged
        /// </summary>
        public static int LogLevel { get; set; }

        public enum LogTypes
        {
            SimulationStart = 0, CellCreate = 2,
            GapJunctionCreation = 3, CellAdjacency = 4, CellStimulation = 5,
            MoleculeMove = 6, CellMoleculeAbsorb = 7, CellFunctionCalculate = 8
        };

        private LoggerRepository()
        {
            var directory = Directory.GetCurrentDirectory();
            if (!Directory.Exists(directory + "\\Logs"))
            {
                Directory.CreateDirectory(directory + "\\Logs");
            }
            _file = directory + "\\Logs\\" + DateTime.Now.ToString().Replace('.', '-').Replace(':', '_').Replace('/', '_') + ".txt";
        }

        public static LoggerRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoggerRepository();
                }
                return instance;
            }
            set
            {
                instance = null;
            }
        }

        public void AppendLog(string logString)
        {
            lock (_file)
            {
                var tw = new StreamWriter(_file, true);
                tw.WriteLine(logString);
                tw.Close();
            }
        }

        public void LogAction(LogTypes logType, string action)
        {
            //only log important log types
            if (LogLevel < (int)logType)
            {
                return;
            }
            lock (_file)
            {
                var tw = new StreamWriter(_file, true);
                tw.WriteLine(string.Format("{0} , {1} , {2}", DateTime.Now, logType, action));
                tw.Close();
            }
        }
    }
}
