using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.BindToClosureAnalyzer>;

namespace ReactiveUI.Analysis.Roslyn.Tests.rxui0002
{
    public class BindToClosureAnalyzerTests : CSharpAnalyzerTest<BindToClosureAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(BindToTestData.Incorrect)]
        public async Task GivenBindToClosure_WhenVerified_ThenDiagnosticsReported(string  code)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(UnsupportedExpressionAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(12, 26, 12, 36)
                   .WithMessage(UnsupportedExpressionAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(BindToTestData.Correct)]
        public Task GivenBindToExpression_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            VerifyCS.VerifyAnalyzerAsync(code);
    }
}