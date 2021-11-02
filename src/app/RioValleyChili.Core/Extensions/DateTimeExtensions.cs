using System;

namespace RioValleyChili.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public const string SQLDateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

        /// <summary>
        /// Returns a new DateTime with milliseconds rounded to end in 0, 3, or 7 to avoid exceptions when inserting into SQL datetime columns.
        /// </summary>
        public static DateTime RoundMillisecondsForSQL(this DateTime datetime)
        {
            var milliseconds = datetime.Millisecond;
            var lastMillisecond = milliseconds % 10;
            switch(lastMillisecond)
            {
                case 1:
                case 4:
                case 8:
                    milliseconds -= 1;
                    break;

                case 2:
                case 6:
                case 9:
                    milliseconds += 1;
                    break;

                case 5:
                    milliseconds += 2;
                    break;
            }

            return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second).AddMilliseconds(milliseconds);
        }

        public static DateTime? RoundMillisecondsForSQL(this DateTime? datetime)
        {
            return datetime != null ? datetime.Value.RoundMillisecondsForSQL() : (DateTime?)null;
        }

        public static DateTime ConvertLocalToUTC(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).UtcDateTime;
        }

        public static DateTime? ConvertLocalToUTC(this DateTime? dateTime)
        {
            return dateTime != null ? dateTime.Value.ConvertLocalToUTC() : (DateTime?) null;
        }

        public static DateTime ConvertUTCToLocal(this DateTime utcDateTime)
        {
            return new DateTimeOffset(utcDateTime).LocalDateTime;
        }

        public static DateTime? ConvertUTCToLocal(this DateTime? dateTime)
        {
            return dateTime != null ? dateTime.Value.ConvertUTCToLocal() : (DateTime?) null;
        }

        public static DateTime? GetDate(this DateTime? dateTime)
        {
            return dateTime != null ? dateTime.Value.Date : (DateTime?)null;
        }

        public static DateTime ToSimpleDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}