using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn.Tests.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.WhenAnyValueClosureAnalyzer>;

namespace ReactiveUI.Analysis.Roslyn.Tests.RXUI0002
{
    public class WhenAnyValueClosureAnalyzerTests : CSharpAnalyzerTest<WhenAnyValueClosureAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(WhenAnyValueTestData.IncorrectSingleProperty)]
        public async Task GivenSingleClosure_WhenVerified_ThenDiagnosticsReported(string  code)
        {
            // Given
            var diagnosticResult =
                AnalyzerVerifier<WhenAnyValueClosureAnalyzer>.Diagnostic(UnsupportedExpressionAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(10, 66, 10, 76)
                   .WithMessage(UnsupportedExpressionAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await AnalyzerVerifier<WhenAnyValueClosureAnalyzer>.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(WhenAnyValueTestData.CorrectSingleProperty)]
        public Task GivenSingleClosure_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            AnalyzerVerifier<WhenAnyValueClosureAnalyzer>.VerifyAnalyzerAsync(code);

        [Theory]
        [InlineData(WhenAnyValueTestData.IncorrectMultipleProperty)]
        public async Task GivenMultiClosure_WhenVerified_ThenDiagnosticsReported(string  code)
        {
            // Given
            var diagnosticResult =
                AnalyzerVerifier<WhenAnyValueClosureAnalyzer>.Diagnostic(UnsupportedExpressionAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(10, 66, 10, 76)
                   .WithMessage(UnsupportedExpressionAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await AnalyzerVerifier<WhenAnyValueClosureAnalyzer>.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(WhenAnyValueTestData.CorrectMultipleProperty)]
        public Task GivenMultiClosure_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            AnalyzerVerifier<WhenAnyValueClosureAnalyzer>.VerifyAnalyzerAsync(code);
    }
}