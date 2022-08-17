using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = RxUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.BindToClosureAnalyzer>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0002
{
    public class BindToClosureAnalyzerTests : CSharpAnalyzerTest<BindToClosureAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(BindToTestData.Incorrect)]
        public async Task GivenBindToClosure_WhenVerified_ThenDiagnosticsReported(string  code)
        {
            // Given
            var diagnosticResult =
                VerifyCS
                   .Diagnostic(UnsupportedExpressionAnalyzer.RXUI0002.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(12, 26, 12, 36)
                   .WithMessage(UnsupportedExpressionAnalyzer.RXUI0002.MessageFormat.ToString());

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