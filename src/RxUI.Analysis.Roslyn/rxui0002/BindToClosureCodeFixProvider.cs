using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Analysis.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BindToClosureCodeFixProvider)), Shared]
    public sealed class BindToClosureCodeFixProvider : UnsupportedExpressionCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(UnsupportedExpressionAnalyzer.RXUI0002.Id);

        protected override void RegisterCodeFix(CodeFixContext context, InvocationExpressionSyntax invocation, SimpleLambdaExpressionSyntax declaration, Diagnostic diagnostic) =>
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => Fixup(context, invocation, declaration, c),
                    equivalenceKey: UnsupportedExpressionAnalyzer.RXUI0002.Id + UnsupportedExpressionAnalyzer.RXUI0002.Title + "BindTo"
                ),
                diagnostic);

        protected override async Task<Document> Fix(CodeFixContext context,
                                                    InvocationExpressionSyntax invocation,
                                                    SimpleLambdaExpressionSyntax declaration,
                                                    CancellationToken cancellationToken)
        {
            var rootAsync = await context.Document.GetSyntaxRootAsync(cancellationToken);
            if (rootAsync == null)
            {
                return context.Document;
            }

            var arguments = ArgumentList(
                SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        Argument(
                            ThisExpression()
                        ),
                        Token(
                            TriviaList(),
                            SyntaxKind.CommaToken,
                            TriviaList(
                                Space
                            )),
                        Argument(
                            SimpleLambdaExpression(
                                    Parameter(
                                        Identifier(
                                            TriviaList(),
                                            declaration.Parameter.Identifier.Text,
                                            TriviaList(
                                                Space
                                            ))))
                               .WithArrowToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.EqualsGreaterThanToken,
                                        TriviaList(
                                            Space
                                        )))
                               .WithExpressionBody(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(declaration.Parameter.Identifier),
                                        IdentifierName(declaration.Body.GetLastToken())
                                    )))
                    }));

            var changed = rootAsync.ReplaceNode(invocation.ArgumentList, arguments);
            return context.Document.WithSyntaxRoot(changed);
        }
    }
}