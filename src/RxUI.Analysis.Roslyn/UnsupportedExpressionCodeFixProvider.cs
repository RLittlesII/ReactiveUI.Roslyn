using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI.Analysis.Roslyn
{
    public abstract class UnsupportedExpressionCodeFixProvider : CodeFixProvider
    {
        protected const string Title = "Add required member access prefix on expression lambda";

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var ancestorsAndSelf = (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? Array.Empty<SyntaxNode>()).ToList();
            var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();
            var declaration = ancestorsAndSelf.OfType<SimpleLambdaExpressionSyntax>().First();

            RegisterCodeFix(context, invocation, declaration, diagnostic);
        }

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        protected abstract void RegisterCodeFix(
            CodeFixContext context,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            Diagnostic diagnostic
        );

        protected Task<Document> Fixup(
            CodeFixContext context,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            CancellationToken cancellationToken
        ) => Fix(context, invocation, declaration, cancellationToken);

        protected virtual Task<Document> Fix(CodeFixContext context,
                                             InvocationExpressionSyntax invocation,
                                             SimpleLambdaExpressionSyntax declaration,
                                             CancellationToken cancellationToken) => Task.FromResult(context.Document);
    }
}