namespace ReactiveUI.Analyzers.Tests
{
    using ReactiveUI;
    using System.Reactive;
    using System.Reactive.Linq;
    public class InvokeCommandTestData
    {
        public InvokeCommandTestData()
        {
            Command = ReactiveCommand.Create(() => { });

            Observable
                .Return(Unit.Default)
                .InvokeCommand(Command);
        }

        public ReactiveCommand<Unit, Unit> Command { get; }
    }
}