using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn;
using RxUI.Analysis.Roslyn.Tests.Verifiers;
using Xunit;

namespace RxUI.Analysis.Roslyn.Tests.rxui0004
{
    public class WhenAnyValueClosureAnalyzerTests : CSharpAnalyzerTest<WhenAnyValueClosureAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(WhenAnyValueTestData.IncorrectSingleProperty)]
        public async Task GivenSingleClosure_WhenVerified_ThenDiagnosticsReported(string code)
        {
            // Given
            var diagnosticResult =
                AnalyzerVerifier<WhenAnyValueClosureAnalyzer>
                    .Diagnostic(UnsupportedExpressionAnalyzer.RXUI0004.Id)
                    .WithSeverity(DiagnosticSeverity.Error)
                    .WithSpan(10, 71, 10, 76)
                    .WithMessage(UnsupportedExpressionAnalyzer.RXUI0004.MessageFormat.ToString());

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
        public async Task GivenMultiClosure_WhenVerified_ThenDiagnosticsReported(string code)
        {
            // Given
            var diagnosticResults =
                new[]
                {
                    AnalyzerVerifier<WhenAnyValueClosureAnalyzer>
                        .Diagnostic(UnsupportedExpressionAnalyzer.RXUI0004)
                        .WithSeverity(DiagnosticSeverity.Error)
                        .WithSpan(10, 71, 10, 76)
                        .WithMessage(UnsupportedExpressionAnalyzer.RXUI0004.MessageFormat.ToString())
                };

            // When, Then
            await AnalyzerVerifier<WhenAnyValueClosureAnalyzer>
                .VerifyAnalyzerAsync(code, diagnosticResults);
        }

        [Theory]
        [InlineData(WhenAnyValueTestData.CorrectMultipleProperty)]
        public Task GivenMultiClosure_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            AnalyzerVerifier<WhenAnyValueClosureAnalyzer>.VerifyAnalyzerAsync(code);
    }
}