using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    /// <summary>
    /// Singleton class to represent time
    /// </summary>
    public class Time
    {
        private int _miliSeconds;
        private static Time instance;

        public static Time Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Time();
                    return instance;
                }
                else
                {
                    return instance;
                }
            }
        }

        private Time()
        {
            _miliSeconds = 0;    //initial time
        }

        public void SetTimeToZero()
        {
            _miliSeconds = 0;
        }

        public void AdvanceTime(int to)
        {
            instance._miliSeconds = to;
        }

        public int Now { get { return _miliSeconds; } }

        public int GetTimeAfterMiliSeconds(int passingMiliSeconds)
        {
            return _miliSeconds + passingMiliSeconds;
        }

        public double GetTimeInSeconds()
        {
            return (double)Now / 1000;
        }

        internal void PrintTime()
        {
            Console.WriteLine(_miliSeconds);
        }
    }
}
