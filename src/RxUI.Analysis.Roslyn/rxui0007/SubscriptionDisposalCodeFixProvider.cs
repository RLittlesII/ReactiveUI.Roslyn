using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI.Analysis.Roslyn
{
    public class SubscriptionDisposalCodeFixProvider : CodeFixProviderBase
    {
        private const string Title = "Add DisposeWith to subscription.";

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(SubscriptionDisposalAnalyzer.Rule.Id);

        protected override async Task<Document> Fix(
            Document document,
            LocalDeclarationStatementSyntax declarationSyntax,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            CancellationToken cancellationToken
        )
        {
            var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
            if (true)
            {

            }
            return await base.Fix(document, declarationSyntax, invocation, declaration, cancellationToken);
        }
    }
}
