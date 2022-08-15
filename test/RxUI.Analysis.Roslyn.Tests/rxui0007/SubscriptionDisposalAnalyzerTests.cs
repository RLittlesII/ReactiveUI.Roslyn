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
            var bindToDiagnostic =
                VerifyCS.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(15, 17, 15, 23)
                   .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString());

            var invokeCommandDiagnostic =
                VerifyCS.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(19, 17, 19, 30)
                   .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString());

            var subscribeDiagnostic =
                VerifyCS.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(23, 17, 23, 26)
                   .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString());

            var toPropertyDiagnostic =
                VerifyCS.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(27, 17, 27, 27)
                   .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, bindToDiagnostic, invokeCommandDiagnostic, subscribeDiagnostic, toPropertyDiagnostic);
        }

        [Theory]
        [InlineData(SubscriptionDisposalTestData.Correct)]
        public Task GivenSubscriptionDisposed_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            VerifyCS.VerifyAnalyzerAsync(code);
    }
}