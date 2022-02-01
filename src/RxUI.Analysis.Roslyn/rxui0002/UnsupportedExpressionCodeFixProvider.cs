using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI.Analysis.Roslyn
{
    public abstract class UnsupportedExpressionCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Fix constant expression";

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var ancestorsAndSelf = (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? Array.Empty<SyntaxNode>()).ToList();
            var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();
            var declaration = ancestorsAndSelf.OfType<SimpleLambdaExpressionSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => Fixup(context.Document, invocation, declaration, c),
                    equivalenceKey: UnsupportedExpressionAnalyzer.Rule.Id + UnsupportedExpressionAnalyzer.Rule.Title),
                diagnostic);
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(UnsupportedExpressionAnalyzer.Rule.Id);
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private Task<Document> Fixup(
            Document document,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            CancellationToken cancellationToken
        ) => Fix(document, invocation, declaration, cancellationToken);

        protected virtual Task<Document> Fix(Document document, InvocationExpressionSyntax invocation, SimpleLambdaExpressionSyntax declaration, CancellationToken cancellationToken) => Task.FromResult(document);
    }
}