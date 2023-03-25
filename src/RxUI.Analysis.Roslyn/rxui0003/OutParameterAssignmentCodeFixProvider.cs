using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI.Analysis.Roslyn
{
    public abstract class OutParameterAssignmentCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Fix out parameter assignment";

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var ancestorsAndSelf = (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? Array.Empty<SyntaxNode>()).ToList();
            var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();
            var declaration = ancestorsAndSelf.OfType<SimpleLambdaExpressionSyntax>().First();
            var variableDeclaration = ancestorsAndSelf.OfType<LocalDeclarationStatementSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => Fixup(context.Document,variableDeclaration, invocation, declaration, c),
                    equivalenceKey: OutParameterAssignmentAnalyzer.Rule.Id + OutParameterAssignmentAnalyzer.Rule.Title),
                diagnostic);
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(OutParameterAssignmentAnalyzer.Rule.Id);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        protected virtual Task<Document> Fix(Document document, LocalDeclarationStatementSyntax declarationSyntax, InvocationExpressionSyntax invocation, SimpleLambdaExpressionSyntax declaration, CancellationToken cancellationToken) => Task.FromResult(document);

        private Task<Document> Fixup(
            Document document,
            LocalDeclarationStatementSyntax declarationSyntax,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            CancellationToken cancellationToken
        ) => Fix(document,declarationSyntax, invocation, declaration, cancellationToken);
    }
}