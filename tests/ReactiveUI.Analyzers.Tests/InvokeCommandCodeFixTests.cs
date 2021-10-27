using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    ReactiveUI.Analyzers.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analyzers.InvokeCommandAnalyzer, ReactiveUI.Analyzers.InvokeCommandCodeFixProvider>;

namespace ReactiveUI.Analyzers.Tests
{
    public class
        InvokeCommandCodeFixTests : CSharpCodeFixTest<InvokeCommandAnalyzer, InvokeCommandCodeFixProvider, XUnitVerifier>
    {
        [Theory]
        [InlineData(InvokeCommandTestData.Incorrect, InvokeCommandTestData.Correct)]
        public async Task Given_When_Then(string incorrect, string correct)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(InvokeCommandAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Warning)
                    .WithSpan(13, 32, 13, 39)
                    .WithMessage("Use expression lambda overload for property Command");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(incorrect, correct, diagnosticResult);
        }
    }
}