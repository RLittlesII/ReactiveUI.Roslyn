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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                    .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var ancestorsAndSelf = (root?.FindToken(diagnosticSpan.Start)
                                        .Parent?.AncestorsAndSelf()
                                 ?? Array.Empty<SyntaxNode>()).ToList();
            var invocation = ancestorsAndSelf.OfType<InvocationExpressionSyntax>()
                                             .First();
            var declaration =
                ancestorsAndSelf.First(x => x.IsKind(SyntaxKind.InvocationExpression)) as InvocationExpressionSyntax;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => Fixup(context, invocation, declaration, c),
                    equivalenceKey: SubscriptionDisposalAnalyzer.RXUI0007.Id + SubscriptionDisposalAnalyzer.RXUI0007.Title
                ),
                diagnostic
            );
        }

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private static async Task<Document> Fixup(
            CodeFixContext context,
            InvocationExpressionSyntax invocation,
            InvocationExpressionSyntax declarationSyntax,
            CancellationToken cancellationToken)
        {
            var rootAsync = await context.Document.GetSyntaxRootAsync(cancellationToken);
            if (rootAsync == null)
            {
                return context.Document;
            }

            var diagnosticParent = rootAsync.FindNode(context.Span)
                                            .Parent;
            var invocationExpressionSyntax = diagnosticParent.Parent as InvocationExpressionSyntax;

            var withCloseParenToken =
                ArgumentList(Token(SyntaxKind.OpenParenToken),
                             invocationExpressionSyntax.ArgumentList.Arguments,
                             Token(TriviaList(),
                                   SyntaxKind.CloseParenToken,
                                   TriviaList(LineFeed)));
            var modifiedInvocationExpressionSyntax =
                invocationExpressionSyntax.ReplaceNode(invocationExpressionSyntax.ArgumentList, withCloseParenToken);

            var newNode =
                ExpressionStatement(
                        InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                       modifiedInvocationExpressionSyntax
                                                           .WithArgumentList(withCloseParenToken),
                                                       IdentifierName("DisposeWith"))
                                    .WithOperatorToken(
                                        Token(
                                            TriviaList(
                                                Whitespace("               ")),
                                            SyntaxKind.DotToken,
                                            TriviaList())))
                            .WithArgumentList(
                                ArgumentList(SingletonSeparatedList(Argument(IdentifierName("Garbage"))))
                                    .WithOpenParenToken(
                                        Token(SyntaxKind.OpenParenToken))
                                    .WithCloseParenToken(
                                        Token(SyntaxKind.CloseParenToken))))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            var changed = rootAsync.ReplaceNode(invocationExpressionSyntax, newNode.Expression);
            return context.Document.WithSyntaxRoot(changed);
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SubscriptionDisposalAnalyzer.RXUI0007.Id);
    }
}