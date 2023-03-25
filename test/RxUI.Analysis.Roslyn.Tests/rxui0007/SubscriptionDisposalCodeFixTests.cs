using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = RxUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.SubscriptionDisposalAnalyzer, ReactiveUI.Analysis.Roslyn.SubscriptionDisposalCodeFixProvider>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0007
{
    public class SubscriptionDisposalCodeFixTests : CSharpCodeFixTest<SubscriptionDisposalAnalyzer, SubscriptionDisposalCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [ClassData(typeof(SubscriptionDisposalTestData))]
        public async Task GivenSubscriptionNotDisposed_WhenAnalyzed_ThenCodeFixed(string incorrect,
            string correct,
            IEnumerable<DiagnosticResult> results) =>
            // Given, When, Then
            await VerifyCS.VerifyAnalyzerAsync(incorrect, correct, results.ToArray());
    }
}