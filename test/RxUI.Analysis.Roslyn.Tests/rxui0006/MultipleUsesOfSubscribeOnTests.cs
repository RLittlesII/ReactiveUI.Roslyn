using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    RxUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<RxUI.Analysis.Roslyn.MultipleUsesOfSubscribeOnAnalyzer>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0006
{
    public class MultipleUsesOfSubscribeOnTests : CSharpAnalyzerTest<MultipleUsesOfSubscribeOnAnalyzer, XUnitVerifier>
    {
        [Theory]
        [MemberData(nameof(MultipleUsesOfSubscribeOnTestData.IncorrectTestData),
                    MemberType = typeof(MultipleUsesOfSubscribeOnTestData))]
        public async Task GivenSubscription_WhenVerified_ThenDiagnosticsReported(
            string incorrect,
            IEnumerable<DiagnosticResult> results) =>
            // Given, When, Then
            await VerifyCS.VerifyAnalyzerAsync(incorrect, results.ToArray());
    }

    public class MultipleUsesOfSubscribeOnTestData
    {
        public static IEnumerable<object[]> IncorrectTestData()
        {
            yield return new object[]
                         {
                             Incorrect,
                             new[]
                             {
                                 CreateDiagnosticResult()
                                     .WithSpan(15, 55, 15, 66)
                             }
                         };
        }

        private static DiagnosticResult CreateDiagnosticResult() =>
            AnalyzerVerifier<MultipleUsesOfSubscribeOnAnalyzer>
                .Diagnostic(MultipleUsesOfSubscribeOnAnalyzer.RXUI0006)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithMessage(MultipleUsesOfSubscribeOnAnalyzer.RXUI0006
                                                              .MessageFormat.ToString());

        internal const string Incorrect = @"
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;

namespace Sample
{
    public class MultipleUsesOfSubscribeOnExample : ReactiveObject
    {

        public MultipleUsesOfSubscribeOnExample() => Observable
                                                     .Return(Unit.Default)
                                                     .SubscribeOn(TaskPoolScheduler.Default)
                                                     .SubscribeOn(ImmediateScheduler.Instance);
    }
}";
    }
}