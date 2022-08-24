using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Sample
{
    public class BindToClosureExample : ReactiveObject
    {
        public BindToClosureExample() => Observable
           .Return(Unit.Default)
           .BindTo(this, x => x.Value)
           .DisposeWith(Garbage);

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
        private readonly CompositeDisposable Garbage = new CompositeDisposable();
    }
}