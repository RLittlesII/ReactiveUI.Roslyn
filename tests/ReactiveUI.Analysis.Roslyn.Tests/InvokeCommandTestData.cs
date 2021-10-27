namespace ReactiveUI.Analysis.Roslyn.Tests
{
    public static class InvokeCommandTestData
    {
        internal const string Correct = @"
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
                .InvokeCommand(this, x => x.Command);
        }

        public ReactiveCommand<Unit, Unit> Command { get; }
    }";

        internal const string Incorrect = @"
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
    }";

    }
}