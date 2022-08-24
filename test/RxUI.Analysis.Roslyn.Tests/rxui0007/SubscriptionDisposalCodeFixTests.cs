using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.SubscriptionDisposalAnalyzer, ReactiveUI.Analysis.Roslyn.SubscriptionDisposalCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests.rxui0007
{
    public class SubscriptionDisposalCodeFixTests : CSharpCodeFixTest<SubscriptionDisposalAnalyzer, SubscriptionDisposalCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [ClassData(typeof(SubscriptionDisposableTestData))]
        public async Task GivenSubscriptionNotDisposed_WhenAnalyzed_ThenCodeFixed(IEnumerable<DiagnosticResult> results, string incorrect, string correct) =>
            // Given, When, Then
            await VerifyCS.VerifyAnalyzerAsync(incorrect, correct, results.ToArray());
    }
}