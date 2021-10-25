using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Sample
{
    public class InvokeCommandExample
    {
        public InvokeCommandExample()
        {
            Command = ReactiveCommand.Create(() => { });

            Observable
                .Return(Unit.Default)
                .InvokeCommand(this, x => x.Command);
        }

        public ReactiveCommand<Unit, Unit> Command { get; set; }
    }
}