using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analysis.Roslyn.ToPropertyAssignmentAnalyzer, ReactiveUI.Analysis.Roslyn.ToPropertyAssignmentCodeFixProvider>;

namespace ReactiveUI.Analysis.Roslyn.Tests.rxui0003
{
    public class ToPropertyAssignmentCodeFixTests : CSharpCodeFixTest<ToPropertyAssignmentAnalyzer, ToPropertyAssignmentCodeFixProvider, XUnitVerifier>
    {
        [Fact]
        public async Task GivenToPropertyAssignment_WhenAnalyzed_ThenCodeFixed()
        {
            // Given
            var diagnosticResult =
                VerifyCS
                   .Diagnostic(OutParameterAssignmentAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Error)
                    .WithSpan(15, 38, 15, 50)
                    .WithMessage(OutParameterAssignmentAnalyzer.Rule.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(ToPropertyTestData.Incorrect, ToPropertyTestData.Correct, diagnosticResult);
        }
    }
}