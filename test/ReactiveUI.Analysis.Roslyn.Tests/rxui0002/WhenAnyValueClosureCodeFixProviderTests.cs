using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.WhenAnyValueClosureAnalyzer, ReactiveUI.Analysis.Roslyn.WhenAnyValueClosureCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests.RXUI0002
{
    public class WhenAnyValueClosureCodeFixTests : CSharpCodeFixTest<WhenAnyValueClosureAnalyzer, WhenAnyValueClosureCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [InlineData(WhenAnyValueTestData.IncorrectSingleProperty, WhenAnyValueTestData.CorrectSingleProperty)]
        public async Task GivenBindToClosure_WhenAnalyzed_ThenCodeFixed(string source, string fixedSource)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(UnsupportedExpressionAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(10, 66, 10, 76)
                   .WithMessage(UnsupportedExpressionAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyCodeFixAsync(source, fixedSource, diagnosticResult);
        }
    }
}