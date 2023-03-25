namespace ReactiveUI.Analysis.Roslyn.Tests.RXUI0002
{
    internal static class WhenAnyValueTestData
    {
        internal const string CorrectSingleProperty = @"
using ReactiveUI;
using System;
using System.Reactive;

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
using ReactiveUI;
using System;
using System.Reactive;

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
using ReactiveUI;
using System;
using System.Reactive;

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
using ReactiveUI;
using System;
using System.Reactive;

namespace Sample
{
    public class WhenAnyValueClosureExample : ReactiveObject
    {
        public WhenAnyValueClosureExample() => this.WhenAnyValue(x => x.Value, y => Value).Subscribe();

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