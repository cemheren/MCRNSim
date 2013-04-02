using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    public class Event : IComparable
    {
        public int EventTime { get; set; }
        private Action _eventMethod;

        public Event(int time)
        {
            EventTime = time;
        }

        public Event(int time, Action eventMethod)
        {
            EventTime = time;
            _eventMethod = eventMethod;
        }

        public void ExecuteEvent()
        {
            Time.Instance.AdvanceTime(this.EventTime);

            if (_eventMethod != null)
            {
                _eventMethod.Invoke();
            }
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Event);
        }

        public int CompareTo(Event other)
        {
            if (other == null)
            {
                return 1;
            }

            var referenceEquals = ReferenceEquals(this, other);
            if (referenceEquals)
            {
                return 0;
            }

            var compare = this.EventTime.CompareTo(other.EventTime);
            if (compare == 0)
            {
                compare = 1;
            }

            return compare;
        }
    }
}
