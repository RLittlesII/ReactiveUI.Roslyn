using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Sample
{
    public class InvokeCommandExample
    {
        public InvokeCommandExample()
        {
            Command = ReactiveCommand.Create(() => { });

            Observable
               .Return(Unit.Default)
               .InvokeCommand(this, x => x.Command)
               .DisposeWith(Garbage);
        }

        public ReactiveCommand<Unit, Unit> Command { get; set; }
        private readonly CompositeDisposable Garbage = new CompositeDisposable();
    }
}