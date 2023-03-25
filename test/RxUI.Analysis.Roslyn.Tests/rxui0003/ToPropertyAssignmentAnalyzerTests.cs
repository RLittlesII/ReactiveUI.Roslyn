using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using ReactiveUI.Analysis.Roslyn;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = RxUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.ToPropertyAssignmentAnalyzer>;

namespace RxUI.Analysis.Roslyn.Tests.rxui0003
{
    public class ToPropertyAssignmentAnalyzerTests : CSharpAnalyzerTest<ToPropertyAssignmentAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(ToPropertyTestData.Incorrect)]
        public async Task GivenToPropertyAssignment_WhenVerified_ThenDiagnosticsReported(string code)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(OutParameterAssignmentAnalyzer.Rule.Id)
                   .WithSeverity(DiagnosticSeverity.Error)
                   .WithSpan(15, 38, 15, 50)
                   .WithMessage(OutParameterAssignmentAnalyzer.Rule.MessageFormat.ToString())
                   .WithArguments("x => x.Value");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, diagnosticResult);
        }

        [Theory]
        [InlineData(ToPropertyTestData.Correct)]
        public Task GivenToPropertyAssignment_WhenVerified_ThenNoDiagnosticsReported(string code) =>
            // Given, When, Then
            VerifyCS.VerifyAnalyzerAsync(code);
    }
}