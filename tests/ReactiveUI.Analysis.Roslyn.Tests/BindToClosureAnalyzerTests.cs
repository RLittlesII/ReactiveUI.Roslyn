using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.BindToClosureAnalyzer>;

namespace ReactiveUI.Analysis.Roslyn.Tests
{
    public class BindToClosureAnalyzerTests : CSharpAnalyzerTest<BindToClosureAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(BindToTestData.Incorrect)]
        public async Task GivenMalformedLambda_WhenVerified_ThenDiagnosticsReported(string  code)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(BindToClosureAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(12, 26, 12, 36)
                   .WithMessage("Provide a well-formed lambda function .BindTo(this, x => x.Property)");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(BindToTestData.Correct)]
        public Task GivenWelformedLambda_WhenVerified_ThenDiagnosticsReported(string code) =>
            // Given, When, Then
            VerifyCS.VerifyAnalyzerAsync(code);
    }
}