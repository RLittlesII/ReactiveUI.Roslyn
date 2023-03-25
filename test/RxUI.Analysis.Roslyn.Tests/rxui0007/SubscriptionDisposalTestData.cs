using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using ReactiveUI.Analysis.Roslyn;
using RxUI.Analysis.Roslyn.Tests.Verifiers;
using System.Collections;
using System.Collections.Generic;

namespace RxUI.Analysis.Roslyn.Tests.rxui0007
{
    public class SubscriptionDisposalTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
                         {
                             Incorrect,
                             Correct,
                             _diagnostics
                         };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal const string Correct = @"
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Sample
{
    public class SubscriptionDisposalExample : ReactiveObject
    {
        public SubscriptionDisposalExample()
        {
            Observable
               .Return(Unit.Default)
               .BindTo(this, x => x.Unit)
               .DisposeWith(Garbage);

            Observable
               .Return(Unit.Default)
               .InvokeCommand(this, x => x.Command)
               .DisposeWith(Garbage);

            Observable
               .Return(Unit.Default)
               .Subscribe()
               .DisposeWith(Garbage);

            Observable
               .Return(Unit.Default)
               .ToProperty(this, nameof(Value), out _value)
               .DisposeWith(Garbage);

            Command = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Command { get; }

        public Unit Value => _value.Value;

        public Unit Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        private readonly CompositeDisposable Garbage = new CompositeDisposable();

        private readonly ObservableAsPropertyHelper<Unit> _value;
        private Unit _unit;
    }
}";

        internal const string Incorrect = @"
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Sample
{
    public class SubscriptionDisposalExample : ReactiveObject
    {
        public SubscriptionDisposalExample()
        {
            Observable
               .Return(Unit.Default)
               .BindTo(this, x => x.Unit);

            Observable
               .Return(Unit.Default)
               .InvokeCommand(this, x => x.Command);

            Observable
               .Return(Unit.Default)
               .Subscribe();

            Observable
               .Return(Unit.Default)
               .ToProperty(this, nameof(Value), out _value);

            Command = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Command { get; }

        public Unit Value => _value.Value;

        public Unit Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        private readonly CompositeDisposable Garbage = new CompositeDisposable();

        private readonly ObservableAsPropertyHelper<Unit> _value;
        private Unit _unit;
    }
}";

        private readonly List<DiagnosticResult> _diagnostics = new List<DiagnosticResult>()
                                                               {
                                                                   AnalyzerVerifier<SubscriptionDisposalAnalyzer>
                                                                       .Diagnostic(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(16, 17, 16, 23)
                                                                       .WithMessage(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007
                                                                               .MessageFormat.ToString()),
                                                                   AnalyzerVerifier<SubscriptionDisposalAnalyzer>
                                                                       .Diagnostic(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(20, 17, 20, 30)
                                                                       .WithMessage(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007
                                                                               .MessageFormat.ToString()),
                                                                   AnalyzerVerifier<SubscriptionDisposalAnalyzer>
                                                                       .Diagnostic(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(24, 17, 24, 26)
                                                                       .WithMessage(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007
                                                                               .MessageFormat.ToString()),
                                                                   AnalyzerVerifier<SubscriptionDisposalAnalyzer>
                                                                       .Diagnostic(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(28, 17, 28, 27)
                                                                       .WithMessage(
                                                                           SubscriptionDisposalAnalyzer.RXUI0007
                                                                               .MessageFormat.ToString())
                                                               };
    }
}