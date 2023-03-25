using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RxUI.Analysis.Roslyn
{
    public abstract class ExpressionLambdaOverloadCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Use lambda expression syntax";

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var ancestorsAndSelf = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? Array.Empty<SyntaxNode>();
            var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => Fixup(context.Document, invocation, c),
                    equivalenceKey: ExpressionLambdaOverloadAnalyzer.RXUI0001.Id + ExpressionLambdaOverloadAnalyzer.RXUI0001.Title),
                diagnostic);
        }

        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(ExpressionLambdaOverloadAnalyzer.RXUI0001.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        protected virtual Task<Document> Fix(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken
        ) => Task.FromResult(document);

        private Task<Document> Fixup(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken
        ) => Fix(document, invocation, cancellationToken);
    }
}