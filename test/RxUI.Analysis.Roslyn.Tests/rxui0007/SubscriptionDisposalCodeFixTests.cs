using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.SubscriptionDisposalAnalyzer, ReactiveUI.Analysis.Roslyn.SubscriptionDisposalCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests.rxui0007
{
    public class SubscriptionDisposalCodeFixTests : CSharpCodeFixTest<SubscriptionDisposalAnalyzer, SubscriptionDisposalCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [InlineData(SubscriptionDisposalTestData.Incorrect, SubscriptionDisposalTestData.Correct)]
        public async Task GivenToPropertyAssignment_WhenAnalyzed_ThenCodeFixed(string incorrect, string correct)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Warning)
                    .WithSpan(15, 38, 15, 50)
                    .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(incorrect, correct, diagnosticResult);
        }
    }
}