﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace ME3Explorer
{
    public class PeriodicUpdater : DependencyObject, INotifyPropertyChanged
    {
        DispatcherTimer timer;

        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(nameof(Interval), typeof(double), typeof(PeriodicUpdater), new PropertyMetadata(0.0, IntervalChanged));

        private static void IntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PeriodicUpdater p = d as PeriodicUpdater;
            p?.Start((double)e.NewValue);
        }

        public DateTime Now => DateTime.Now;

        public void Start(double interval)
        {
            Stop();
            timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(interval)};
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Stop()
        {
            timer?.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Now)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
