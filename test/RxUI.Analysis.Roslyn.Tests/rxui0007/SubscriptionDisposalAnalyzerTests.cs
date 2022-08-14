using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.SubscriptionDisposalAnalyzer>;

namespace ReactiveUI.Analysis.Roslyn.Tests.rxui0007
{
    public class SubscriptionDisposalAnalyzerTests : CSharpAnalyzerTest<SubscriptionDisposalAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(SubscriptionDisposalTestData.Incorrect)]
        public async Task GivenSubscription_WhenVerified_ThenDiagnosticsReported(string code)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(15, 38, 15, 50)
                   .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString())
                   .WithArguments("x => x.Value");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(SubscriptionDisposalTestData.Correct)]
        public Task GivenSubscriptionDisposed_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            VerifyCS.VerifyAnalyzerAsync(code);
    }
}