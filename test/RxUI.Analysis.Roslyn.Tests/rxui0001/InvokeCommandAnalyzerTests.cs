using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = RxUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.InvokeCommandAnalyzer>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0001
{
    public class InvokeCommandAnalyzerTests : CSharpAnalyzerTest<InvokeCommandAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(InvokeCommandTestData.Incorrect)]
        public async Task GivenInvokeCommand_WhenAnalyzed_ThenDiagnosticReported(string code)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(ExpressionLambdaOverloadAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Warning)
                   .WithSpan(13, 32, 13, 39)
                   .WithMessage("Use expression lambda overload for property Command");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(InvokeCommandTestData.Correct)]
        public async Task GivenInvokeCommandExpression_WhenAnalyzed_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            await VerifyCS.VerifyAnalyzerAsync(code);
    }
}