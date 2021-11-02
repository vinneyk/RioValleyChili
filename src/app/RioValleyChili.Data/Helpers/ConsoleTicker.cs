using System;

namespace RioValleyChili.Data.Helpers
{
    public class ConsoleTicker
    {
        public int UpdateMilliseconds = 100;

        private int _currentTick = 0;
        private DateTime? _lastTime;

        public ConsoleTicker(int updateMilliseconds = 100)
        {
            UpdateMilliseconds = updateMilliseconds;
        }

        public void ResetTicker()
        {
            _lastTime = null;
            _currentTick = 0;
        }

        public void TickConsole(string line)
        {
            var currentTime = DateTime.Now;
            if(_lastTime != null)
            {
                if((currentTime - _lastTime).Value.Milliseconds < UpdateMilliseconds)
                {
                    return;
                }
            }

            string currentCursor;
            switch(_currentTick % 4)
            {
                case 0: currentCursor = "/"; break;
                case 1: currentCursor = "-"; break;
                case 2: currentCursor = "\\"; break;
                default: currentCursor = "|"; break;
            }

            if((++_currentTick) == 4)
            {
                _currentTick = 0;
            }

            Console.Write("\r{0} {1}", line, currentCursor);

            _lastTime = currentTime;
        }

        public void ReplaceCurrentLine(string format, params object[] arg)
        {
            Console.Write("\r");
            for(int i = 0; i < Console.WindowWidth - 1; ++i)
            {
                Console.Write(" ");
            }
            Console.Write("\r" + format + "\n", arg);
        }

        public void WriteTimeElapsed(TimeSpan timeElapsed)
        {
            Console.WriteLine("Time Elapsed: {0}", timeElapsed.ToString(@"hh\:mm\:ss\.ff"));
        }
    }
}