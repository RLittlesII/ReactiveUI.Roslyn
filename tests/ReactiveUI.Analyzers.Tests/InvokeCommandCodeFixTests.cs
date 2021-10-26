using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    ReactiveUI.Analyzers.Tests.Verifiers.CodeFixVerifier<ReactiveUI.Analyzers.InvokeCommandAnalyzer, ReactiveUI.Analyzers.InvokeCommandCodeFix>;

namespace ReactiveUI.Analyzers.Tests
{
    public class
        InvokeCommandCodeFixTests : CSharpCodeFixTest<InvokeCommandAnalyzer, InvokeCommandCodeFix, XUnitVerifier>
    {
        [Fact]
        public async Task Given_When_Then()
        {
            // Given
            var testCode = @"
    using ReactiveUI;
    using System.Reactive;
    using System.Reactive.Linq;
    public class InvokeCommandTestData
    {
        public InvokeCommandTestData()
        {
            Command = ReactiveCommand.Create(() => { });

            Observable
                .Return(Unit.Default)
                .InvokeCommand(Command);
        }

        public ReactiveCommand<Unit, Unit> Command { get; }
    }";
            var fixedCode = @"
    using ReactiveUI;
    using System.Reactive;
    using System.Reactive.Linq;
    public class InvokeCommandTestData
    {
        public InvokeCommandTestData()
        {
            Command = ReactiveCommand.Create(() => { });

            Observable
                .Return(Unit.Default)
                .InvokeCommand(this, x => x.Command);
        }

        public ReactiveCommand<Unit, Unit> Command { get; }
    }";

            var diagnosticResult =
                VerifyCS.Diagnostic(InvokeCommandAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Warning)
                    .WithSpan(13, 32, 13, 39)
                    .WithMessage("Use expression lambda overload for property Command");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(testCode, fixedCode, diagnosticResult);
        }
    }
}