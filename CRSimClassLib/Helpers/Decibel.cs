using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRSimClassLib.Repositories;

namespace CRSimClassLib.Helpers
{
    public class Decibel
    {
        private double _value;
        /// <summary>
        /// use with caution
        /// </summary>
        /// <param name="value"></param>
        public Decibel(double value)
        {
            _value = value;    
        }

        public static implicit operator Decibel(double value)
        {
            return new Decibel(value);
        }

        public static implicit operator Decibel(int value)
        {
            return new Decibel((double)value);
        }

        public static Decibel operator +(Decibel left, Decibel rigth)
        {
            return (left._value.ToLinearScale() + rigth._value.ToLinearScale()).ToDecibels();
        }

        public static Decibel operator -(Decibel left, Decibel rigth)
        {
            return (left._value.ToLinearScale() - rigth._value.ToLinearScale()).ToDecibels();            
        }
    }
}
