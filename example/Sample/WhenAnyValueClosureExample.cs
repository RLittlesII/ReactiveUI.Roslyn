using ReactiveUI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;

namespace Sample
{
    [SuppressMessage("Usage", "RXUI0007:Subscription is not disposed")]
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample()
        {
            this.WhenAnyValue(x => x.Value).Subscribe();
            this.WhenAnyValue(y => y.Value).Subscribe();
            this.WhenAnyValue(x => x.Value, y => y.Value).Subscribe();
            this.WhenAnyValue(x => x.Value, y => y.Value).Subscribe();
        }

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}