using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = RxUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.SubscriptionDisposalAnalyzer>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0007
{
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    public class SubscriptionDisposalAnalyzerTests : CSharpAnalyzerTest<SubscriptionDisposalAnalyzer, XUnitVerifier>
    {
        [Theory]
        [ClassData(typeof(SubscriptionDisposalTestData))]
        public async Task GivenSubscription_WhenVerified_ThenDiagnosticsReported(string incorrect, string correct, IEnumerable<DiagnosticResult> results) =>
            // Given, When, Then
            await VerifyCS.VerifyAnalyzerAsync(incorrect, results.ToArray());

        [Theory]
        [InlineData(SubscriptionTestData.Correct)]
        public Task GivenSubscriptionDisposed_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            VerifyCS.VerifyAnalyzerAsync(code);
    }
}