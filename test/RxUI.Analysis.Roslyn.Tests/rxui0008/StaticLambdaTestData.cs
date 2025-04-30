
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using RxUI.Analysis.Roslyn.Tests.Verifiers;
using System.Collections;
using System.Collections.Generic;

namespace RxUI.Analysis.Roslyn.Tests.rxui0008
{
    public class StaticLambdaTestData : IEnumerable<object[]>
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
    public class StaticLambdaExample : ReactiveObject
    {
        public StaticLambdaExample()
        {
            // Using instance properties - good
            Observable
                .Return(Unit.Default)
                .Select(static _ => Value)
                .Subscribe();

            // Using local variables - good
            var localValue = 42;
            Observable
                .Return(Unit.Default)
                .Select(static _ => localValue)
                .Subscribe();

            // Using parameters - good
            void Process(int param) =>
                Observable
                    .Return(Unit.Default)
                    .Select(static _ => param)
                    .Subscribe();
                    
            Command = ReactiveCommand.Create(static () => { });
        }

        public ReactiveCommand<Unit, Unit> Command { get; }

        public int Value { get; set; } = 42;
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
    public class StaticLambdaExample : ReactiveObject
    {
        // Static field
        private static readonly int _staticValue = 42;
        
        public StaticLambdaExample()
        {
            // Using static field - should warn
            Observable
                .Return(Unit.Default)
                .Select(_ => _staticValue)
                .Subscribe();

            // Using static property - should warn
            Observable
                .Return(Unit.Default)
                .Select(_ => StaticValue)
                .Subscribe();
                
            // Using static method - should warn
            Observable
                .Return(Unit.Default)
                .Select(_ => GetStaticValue())
                .Subscribe();
                
            Command = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Command { get; }
        
        // Static property
        public static int StaticValue { get; } = 100;
        
        // Static method
        private static int GetStaticValue() => 200;
    }
}";

        private readonly List<DiagnosticResult> _diagnostics = new List<DiagnosticResult>()
                                                               {
                                                                   AnalyzerVerifier<StaticLambdaAnalyzer>
                                                                       .Diagnostic(
                                                                           StaticLambdaAnalyzer.RXUI0008.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(18, 27, 18, 38)
                                                                       .WithMessage(
                                                                           StaticLambdaAnalyzer.RXUI0008
                                                                               .MessageFormat.ToString()),
                                                                   AnalyzerVerifier<StaticLambdaAnalyzer>
                                                                       .Diagnostic(
                                                                           StaticLambdaAnalyzer.RXUI0008.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(24, 27, 24, 37)
                                                                       .WithMessage(
                                                                           StaticLambdaAnalyzer.RXUI0008
                                                                               .MessageFormat.ToString()),
                                                                   AnalyzerVerifier<StaticLambdaAnalyzer>
                                                                       .Diagnostic(
                                                                           StaticLambdaAnalyzer.RXUI0008.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(30, 27, 30, 42)
                                                                       .WithMessage(
                                                                           StaticLambdaAnalyzer.RXUI0008
                                                                               .MessageFormat.ToString()),
                                                                   AnalyzerVerifier<StaticLambdaAnalyzer>
                                                                       .Diagnostic(
                                                                           StaticLambdaAnalyzer.RXUI0008.Id)
                                                                       .WithSeverity(DiagnosticSeverity.Warning)
                                                                       .WithSpan(35, 46, 35, 55)
                                                                       .WithMessage(
                                                                           StaticLambdaAnalyzer.RXUI0008
                                                                               .MessageFormat.ToString())
                                                               };
    }
}