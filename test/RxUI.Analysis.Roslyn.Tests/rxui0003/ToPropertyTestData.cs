namespace RxUI.Analysis.Roslyn.Tests.rxui0003
{
    internal static class ToPropertyTestData
    {
        internal const string Correct = @"
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class OutParameterAssignmentExample : ReactiveObject
    {
        public OutParameterAssignmentExample()
        {
                Observable
                   .Return(Unit.Default)
                   .ToProperty(this, nameof(Value), out _value);
        }

        public Unit Value => _value.Value;

        private readonly ObservableAsPropertyHelper<Unit> _value = ObservableAsPropertyHelper<Unit>.Default();
    }
}";
        internal const string Incorrect = @"
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class OutParameterAssignmentExample : ReactiveObject
    {
        public OutParameterAssignmentExample()
        {
            var _ =
                Observable
                   .Return(Unit.Default)
                   .ToProperty(this, x => x.Value);
        }

        public Unit Value => _value.Value;

        private readonly ObservableAsPropertyHelper<Unit> _value = ObservableAsPropertyHelper<Unit>.Default();
    }
}";
    }
}