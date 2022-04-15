using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
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
            var diagnosticResult = new[]
            {
                VerifyCS
                   .Diagnostic(UnsupportedExpressionAnalyzer.RXUI0004)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(10, 85, 10, 90)
                   .WithMessage(UnsupportedExpressionAnalyzer.RXUI0004.MessageFormat.ToString()),

                VerifyCS
                   .Diagnostic(new DiagnosticDescriptor("CS0121", "", "The call is ambiguous between the following methods or properties: 'WhenAnyMixin.WhenAnyValue<TSender, TRet, T1>(TSender?, Expression<Func<TSender, T1>>, Func<T1, TRet>)' and 'WhenAnyMixin.WhenAnyValue<TSender, T1, T2>(TSender?, Expression<Func<TSender, T1>>, Expression<Func<TSender, T2>>)'", "", DiagnosticSeverity.Error, true))
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(10, 53, 10, 65)
            };

            // When, Then
            await VerifyCS.VerifyCodeFixAsync(source, fixedSource, diagnosticResult);
        }
    }
}