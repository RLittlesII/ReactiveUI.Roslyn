using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.WhenAnyValueClosureAnalyzer,
        ReactiveUI.Analysis.Roslyn.WhenAnyValueClosureCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests.rxui0004
{
    public class WhenAnyValueClosureCodeFixTests : CSharpCodeFixTest<WhenAnyValueClosureAnalyzer, WhenAnyValueClosureCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [InlineData(WhenAnyValueTestData.IncorrectSingleProperty, WhenAnyValueTestData.CorrectSingleProperty)]
        public async Task GivenSinglePropertyClosure_WhenAnalyzed_ThenCodeFixed(string source, string fixedSource)
        {
            // Given
            var diagnosticResult =
                VerifyCS
                   .Diagnostic(UnsupportedExpressionAnalyzer.RXUI0004.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(10, 71, 10, 76)
                   .WithMessage(UnsupportedExpressionAnalyzer.RXUI0004.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyCodeFixAsync(source, fixedSource, diagnosticResult);
        }

        [Theory]
        [InlineData(WhenAnyValueTestData.IncorrectMultipleProperty, WhenAnyValueTestData.CorrectMultipleProperty)]
        public async Task GivenMultiplePropertiesWithSinglePropertyClosure_WhenAnalyzed_ThenCodeFixed(string source, string fixedSource)
        {
            // Given
            var diagnosticResult = new DiagnosticResult[]
                                   {
                                       VerifyCS
                                           .Diagnostic(UnsupportedExpressionAnalyzer.RXUI0004)
                                           .WithSpan(10, 71, 10, 76)
                                   };

            // When, Then
            await VerifyCS.VerifyCodeFixAsync(source, fixedSource, diagnosticResult);
        }
    }
}