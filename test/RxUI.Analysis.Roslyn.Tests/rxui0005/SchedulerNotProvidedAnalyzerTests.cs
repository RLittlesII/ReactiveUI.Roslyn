using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn;
using Xunit;
using VerifyCS =
    RxUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.SchedulerNotProvidedAnalyzer>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0005
{
    public class SchedulerNotProvidedAnalyzerTests : CSharpAnalyzerTest<SchedulerNotProvidedAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(Incorrect)]
        public async Task GivenToPropertyAssignment_WhenVerified_ThenDiagnosticsReported(string code)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(ImproperUsageAnalyzer.RXUI0005)
                        .WithSeverity(DiagnosticSeverity.Warning);

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(Correct)]
        public Task GivenToPropertyAssignment_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            VerifyCS.VerifyAnalyzerAsync(code);

        private const string Incorrect = @"
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Sample
{
    public class SchedulerExample
    {
        public SchedulerExample() => Observable.Start(() => Unit.Default).Throttle(TimeSpan.FromMilliseconds(100)).Subscribe();
    }
}";

        private const string Correct = @"
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Sample
{
    public class SchedulerExample
    {
        public SchedulerExample() => Observable.Start(() => Unit.Default).Throttle(TimeSpan.FromMilliseconds(100), TaskPoolScheduler.Default).Subscribe();
    }
}";
    }
}