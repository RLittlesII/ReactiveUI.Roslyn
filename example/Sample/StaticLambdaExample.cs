using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Sample
{
    public class StaticLambdaExample : ReactiveObject
    {
        public StaticLambdaExample()
        {
            Observable
               .Return(Unit.Default)
               .Select(static _ => _staticValue)
               .Subscribe();

            // Using static property - should warn
            Observable
               .Return(Unit.Default)
               .Select(static _ => StaticValue)
               .Subscribe();

            // Using static method - should warn
            Observable
               .Return(Unit.Default)
               .Select(static _ => GetStaticValue())
               .Subscribe();

            this.WhenAnyValue(static x => x.Life)
               .Subscribe();
            Command = ReactiveCommand.Create(static () => { });
        }

        public int Life
        {
            get => _life;
            set => this.RaiseAndSetIfChanged(ref _life, value);
        }

        public ReactiveCommand<Unit, Unit> Command { get; }

        // Static property
        public static int StaticValue { get; } = 100;

        // Static method
        private static int GetStaticValue() => 200;
        private static readonly int _staticValue = 42;
        private int _life;
    }
}