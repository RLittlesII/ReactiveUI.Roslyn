using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = RxUI.Analysis.Roslyn.Tests.Verifiers.CodeFixVerifier<RxUI.Analysis.Roslyn.ToPropertyAssignmentAnalyzer, RxUI.Analysis.Roslyn.ToPropertyAssignmentCodeFixProvider>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0003
{
    public class ToPropertyAssignmentCodeFixTests : CSharpCodeFixTest<ToPropertyAssignmentAnalyzer, ToPropertyAssignmentCodeFixProvider, XUnitVerifier>
    {
        [Fact]
        public async Task GivenToPropertyAssignment_WhenAnalyzed_ThenCodeFixed()
        {
            // Given
            var diagnosticResult =
                VerifyCS
                   .Diagnostic(OutParameterAssignmentAnalyzer.RXUI0003.Id)
                    .WithSeverity(DiagnosticSeverity.Error)
                    .WithSpan(15, 38, 15, 50)
                    .WithMessage(OutParameterAssignmentAnalyzer.RXUI0003.MessageFormat.ToString());

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(ToPropertyTestData.Incorrect, ToPropertyTestData.Correct, diagnosticResult);
        }
    }
}