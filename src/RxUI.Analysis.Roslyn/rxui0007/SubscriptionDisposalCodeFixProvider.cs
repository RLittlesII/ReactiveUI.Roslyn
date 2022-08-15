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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Analysis.Roslyn
{
    public class SubscriptionDisposalCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Add DisposeWith to subscription.";

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var ancestorsAndSelf = ( root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf() ?? Array.Empty<SyntaxNode>() ).ToList();
            var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>().First();
            var declaration = ancestorsAndSelf.First(x => x.IsKind(SyntaxKind.InvocationExpression)) as InvocationExpressionSyntax;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => Fixup(context.Document, invocation, declaration, c),
                    equivalenceKey: SubscriptionDisposalAnalyzer.Rule.Id + SubscriptionDisposalAnalyzer.Rule.Title
                ),
                diagnostic
            );
        }

        private static async Task<Document> Fixup(
            Document document,
            InvocationExpressionSyntax invocation,
            InvocationExpressionSyntax declarationSyntax,
            CancellationToken cancellationToken
        )
        {
            var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
            if (rootAsync == null)
            {
                return document;
            }

            var expressionSyntax = (MemberAccessExpressionSyntax)declarationSyntax.Expression;

            var disposeWithExpression = ExpressionStatement(
                    InvocationExpression(IdentifierName(Identifier(TriviaList(Trivia(SkippedTokensTrivia().WithTokens(TokenList(Token(SyntaxKind.DotToken))))),
                                    "DisposeWith",
                                    TriviaList())))
                       .WithArgumentList(ArgumentList(SingletonSeparatedList<ArgumentSyntax>(Argument(IdentifierName("Gargabe"))))
                               .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                               .WithCloseParenToken(Token(SyntaxKind.CloseParenToken))))
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            var invocationExpressionSyntax = ExpressionStatement(invocation.ReplaceNode(invocation.ArgumentList, disposeWithExpression)).AncestorsAndSelf();
            var changed = rootAsync.ReplaceNode(declarationSyntax, invocationExpressionSyntax);
            return document.WithSyntaxRoot(changed);
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(SubscriptionDisposalAnalyzer.Rule.Id);
    }
}
