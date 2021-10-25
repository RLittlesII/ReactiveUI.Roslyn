using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    ReactiveUI.Analyzers.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analyzers.InvokeCommandAnalyzer>;

namespace ReactiveUI.Analyzers.Tests
{
    public class InvokeCommandAnalyzerTests : CSharpAnalyzerTest<InvokeCommandAnalyzer, XUnitVerifier>
    {
        private static string _testCode = @"
    using ReactiveUI;
    using System.Reactive;
    using System.Reactive.Linq;
    public class InvokeCommandExample
    {
        public InvokeCommandExample()
        {
            Command = ReactiveCommand.Create(() => { });

            Observable
                .Return(Unit.Default)
                .InvokeCommand(Command);
        }

        public ReactiveCommand<Unit, Unit> Command { get; }
    }";

        [Fact]
        public async Task Given_When_Then()
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(InvokeCommandAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Warning)
                    .WithSpan(13, 32, 13, 39)
                    .WithMessage("Use expression lambda overload for property Command");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(_testCode, diagnosticResult);
        }
    }
}