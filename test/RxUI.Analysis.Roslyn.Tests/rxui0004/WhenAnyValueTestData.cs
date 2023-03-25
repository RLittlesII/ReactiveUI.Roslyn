namespace RxUI.Analysis.Roslyn.Tests.rxui0004
{
    internal static class WhenAnyValueTestData
    {
        internal const string CorrectSingleProperty = @"
using System;
using System.Reactive;
using ReactiveUI;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample() => this.WhenAnyValue(x => x.Value).Subscribe();

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";
        internal const string IncorrectSingleProperty = @"
using System;
using System.Reactive;
using ReactiveUI;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample() => this.WhenAnyValue(x => Value).Subscribe();

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";

        internal const string CorrectMultipleProperty = @"
using System;
using System.Reactive;
using ReactiveUI;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample() => this.WhenAnyValue(x => x.Value, y => y.Value).Subscribe();

        public Unit Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private Unit _value;
    }
}";
        internal const string IncorrectMultipleProperty = @"
using System;
using System.Reactive;
using ReactiveUI;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample() => this.WhenAnyValue(x => Value, y => y.Value).Subscribe();

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