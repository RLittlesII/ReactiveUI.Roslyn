using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RxUI.Analysis.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BindToClosureCodeFixProvider)), Shared]
    public class ToPropertyAssignmentCodeFixProvider : OutParameterAssignmentCodeFixProvider
    {
        protected override async Task<Document> Fix(
            Document document,
            LocalDeclarationStatementSyntax declarationSyntax,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            CancellationToken cancellationToken
        )
        {
            var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
            if (rootAsync == null)
            {
                return document;
            }

            var equalSyntax = declarationSyntax.Declaration;

            var expressionSyntax = (MemberAccessExpressionSyntax)declaration.Body;

            var arguments =
                ArgumentList(
                    SeparatedList<ArgumentSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            Argument(ThisExpression().WithToken(Token(SyntaxKind.ThisKeyword))),
                            Token(TriviaList(), SyntaxKind.CommaToken, TriviaList(Space)),
                            Argument(
                                InvocationExpression(IdentifierName(Identifier(TriviaList(), SyntaxKind.NameOfKeyword, "nameof", "nameof", TriviaList())))
                                   .WithArgumentList(
                                        ArgumentList(SingletonSeparatedList(Argument(IdentifierName(expressionSyntax.Name.Identifier))))
                                           .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                                           .WithCloseParenToken(Token(SyntaxKind.CloseParenToken)))),
                            Token(TriviaList(),
                                SyntaxKind.CommaToken,
                                TriviaList(Space)),
                            Argument(IdentifierName("_" + IdentifierName(expressionSyntax.Name.Identifier.Text.ToLowerInvariant())))
                               .WithRefOrOutKeyword(Token(TriviaList(), SyntaxKind.OutKeyword, TriviaList(Space)))
                               .WithRefKindKeyword(Token(TriviaList(), SyntaxKind.OutKeyword, TriviaList(Space)))
                        }));

            var invocationExpressionSyntax = ExpressionStatement(invocation.ReplaceNode(invocation.ArgumentList, arguments)).AncestorsAndSelf();
            var changed =  rootAsync.ReplaceNode(declarationSyntax, invocationExpressionSyntax);
            return document.WithSyntaxRoot(changed);
        }
    }
}