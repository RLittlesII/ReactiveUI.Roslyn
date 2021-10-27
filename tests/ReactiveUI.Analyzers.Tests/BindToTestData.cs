namespace ReactiveUI.Analyzers.Tests
{
    internal static class BindToTestData
    {
        internal const string Correct = @"
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class BindToClosureExample : ReactiveObject
    {
        public BindToClosureExample() => Observable
           .Return(Unit.Default)
           .BindTo(this, x => x.Value);

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";
        internal const string Incorrect = @"
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class BindToClosureExample : ReactiveObject
    {
        public BindToClosureExample() => Observable
           .Return(Unit.Default)
           .BindTo(this, x => Value);

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";

    }
}