using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class OutParameterAssignmentExample : ReactiveObject
    {
        public OutParameterAssignmentExample() => Observable
           .Return(Unit.Default)
           .ToProperty(this, nameof(Value), out _value);

        public Unit Value => _value.Value;

        private readonly ObservableAsPropertyHelper<Unit> _value;
    }
}