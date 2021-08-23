using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.ComponentModel;

namespace SimpleDigitalClock
{
    public class SimpleClockViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string CurrentDate { get; private set; }
        public string CurrentTime { get; private set; }

        private DateTime PreviousDateTime = DateTime.Now;
        private Timer timer = new Timer(1000.0);


        private void Update(object sender, ElapsedEventArgs args)
        {
            var current = args.SignalTime;
            if(PreviousDateTime.Year != current.Year || PreviousDateTime.Month != current.Month || PreviousDateTime.Day != current.Day)
            {
                CurrentDate = current.ToString("d");
                RaisePropertyChanged(nameof(CurrentDate));
            }

            CurrentTime = current.ToString("T");
            RaisePropertyChanged(nameof(CurrentTime));
            PreviousDateTime = current;
        }

        public SimpleClockViewModel()
        {
            timer.Elapsed += Update;
            timer.Interval = 1000.0;
            timer.Start();
        }

    }
}
