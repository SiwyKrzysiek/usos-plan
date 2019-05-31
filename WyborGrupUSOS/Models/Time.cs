using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WyborGrupUSOS.Models
{
    public class Time
    {
        private int _hour;
        private int _minute;

        public int Hour
        {
            get => _hour;
            set
            {
                if (value < 0 || value > 24)
                    throw new ArgumentOutOfRangeException(nameof(value), "Hour must be between 0 and 24");
                _hour = value;
            }
        }

        public int Minute
        {
            get => _minute;
            set
            {
                if (value < 0 || value > 60)
                    throw new ArgumentOutOfRangeException(nameof(value), "Minute must be between 0 and 60");
                _minute = value;
            }
        }

        public Time()
        {

        }

        public Time(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        /// <summary>
        /// Parses string in hh:mm format to Time object
        /// </summary>
        /// <param name="time"></param>
        public Time(string time)
        {
            var pattern = @"\d{1,2}:\d{2}";
            if (!Regex.IsMatch(time, pattern))
                throw new ArgumentException("Time must be in hh:mm format");

            var parts = time.Split(':');

            Hour = int.Parse(parts[0]);
            Minute = int.Parse(parts[1]);
        }

        public override string ToString()
        {
            return $"{_hour}:{_minute:2D}";
        }
    }
}
