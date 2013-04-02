using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    public class Singleton<T> where T : class
    {
        private static T _instance;
        public static T Instance
        {
            get 
            {
                if (_instance == null)
                {
                    Type t = typeof(T);
                    _instance = (T)Activator.CreateInstance(t, true);
                }
                return _instance;
            }
        }

        private Singleton(){ }
    }
}
