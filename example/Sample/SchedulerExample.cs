using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class SchedulerExample
    {
        public SchedulerExample() =>
            Observable.Start(() => Unit.Default)
                      .Throttle(TimeSpan.FromMilliseconds(100))
                      .Subscribe();
    }
}