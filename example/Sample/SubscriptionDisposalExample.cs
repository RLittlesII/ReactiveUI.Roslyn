using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Sample
{
    public class SubscriptionDisposalExample : ReactiveObject
    {
        public SubscriptionDisposalExample()
        {
            Observable
               .Return(Unit.Default)
               .BindTo(this, x => x.Unit)
               .DisposeWith(Garbage);

            Observable
               .Return(Unit.Default)
               .InvokeCommand(this, x => x.Command);

            Observable
               .Return(Unit.Default)
               .Subscribe();

            Observable
               .Return(Unit.Default)
               .ToProperty(this, nameof(Value), out _value);

            Command = ReactiveCommand.Create(() => { });
        }


        public ReactiveCommand<Unit, Unit> Command { get; }

        public Unit Value => _value.Value;

        public Unit Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        private readonly CompositeDisposable Garbage = new CompositeDisposable();

        private readonly ObservableAsPropertyHelper<Unit> _value;
        private Unit _unit;
    }
}