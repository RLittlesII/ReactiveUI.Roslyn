using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace ReactiveUI.Analysis.Roslyn
{
    public class SubscriptionDisposalCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Add DisposeWith to subscription.";

        public override Task RegisterCodeFixesAsync(CodeFixContext context) => Task.CompletedTask;

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(SubscriptionDisposalAnalyzer.Rule.Id);
    }
}
