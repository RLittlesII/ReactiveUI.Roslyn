using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Sample
{
    public class SubscriptionDisposalExample : ReactiveObject
    {
        public SubscriptionDisposalExample()
        {
            Observable
               .Return(Unit.Default)
               .InvokeCommand(this, x => x.Command)
               .DisposeWith(Gargabe);

            Observable
               .Return(Unit.Default)
               .Subscribe()
               .DisposeWith(Gargabe);

            Observable
               .Return(Unit.Default)
               .ToProperty(this, nameof(Value), out _value)
               .DisposeWith(Gargabe);

            Command = ReactiveCommand.Create(() => { });
        }


        public ReactiveCommand<Unit, Unit> Command { get; }

        public Unit Value => _value.Value;

        private readonly CompositeDisposable Gargabe = new CompositeDisposable();

        private readonly ObservableAsPropertyHelper<Unit> _value;
    }
}