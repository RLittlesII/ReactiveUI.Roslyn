using ReactiveUI;
using System;
using System.Reactive;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample() => this.WhenAnyValue(x => x.Value, x => Value).Subscribe();

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}