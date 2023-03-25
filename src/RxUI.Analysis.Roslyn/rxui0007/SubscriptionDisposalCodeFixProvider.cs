using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RxUI.Analysis.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SubscriptionDisposalCodeFixProvider)), Shared]
    public class SubscriptionDisposalCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SubscriptionDisposalAnalyzer.RXUI0007.Id);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => Fixup(root, context, c),
                    equivalenceKey: SubscriptionDisposalAnalyzer.RXUI0007.Id + SubscriptionDisposalAnalyzer.RXUI0007.Title),
                diagnostic);
        }

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private static async Task<Document> Fixup(
            SyntaxNode root,
            CodeFixContext context,
            CancellationToken cancellationToken)
        {
            if (root == null)
            {
                return context.Document;
            }

            var diagnosticParent =
                root.FindNode(context.Span)
                    .Parent;

            var originalInvocationExpression = diagnosticParent.Parent as InvocationExpressionSyntax;

            var closeParenToken =
                ArgumentList(Token(SyntaxKind.OpenParenToken),
                             originalInvocationExpression.ArgumentList.Arguments,
                             Token(TriviaList(),
                                   SyntaxKind.CloseParenToken,
                                   TriviaList(LineFeed)));

            var modifiedInvocationExpression =
                originalInvocationExpression
                    .ReplaceNode(originalInvocationExpression.ArgumentList, closeParenToken);

            var whitespaceTrivia =
                modifiedInvocationExpression
                    .GetLeadingTrivia()
                    .Last(syntaxTrivia => syntaxTrivia.IsKind(SyntaxKind.WhitespaceTrivia));

            var expressionStatementSyntax =
                ExpressionStatement(
                        InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                       modifiedInvocationExpression
                                                           .WithArgumentList(closeParenToken),
                                                       IdentifierName("DisposeWith"))
                                    .WithOperatorToken(
                                        Token(TriviaList(
                                                  whitespaceTrivia,
                                                  // HACK: [rlittlesii: March 25, 2023] Whitespace alignment is hard!
                                                  Whitespace("   ")),
                                              SyntaxKind.DotToken,
                                              TriviaList())))
                            .WithArgumentList(
                                ArgumentList(
                                        SingletonSeparatedList(
                                            // TODO: [rlittlesii: March 25, 2023] Look for a CompositeDisposable use it's identifier name
                                            Argument(IdentifierName("Garbage"))))
                                    .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                                    .WithCloseParenToken(Token(SyntaxKind.CloseParenToken))))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return context.Document.WithSyntaxRoot(root.ReplaceNode(originalInvocationExpression, expressionStatementSyntax.Expression));
        }

        private const string Title = "Add DisposeWith to subscription";
    }
}