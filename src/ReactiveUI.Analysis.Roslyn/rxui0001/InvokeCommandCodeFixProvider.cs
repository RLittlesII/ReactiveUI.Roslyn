using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Composition;
using System.Threading;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Analysis.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InvokeCommandCodeFixProvider)), Shared]
    public class InvokeCommandCodeFixProvider : ExpressionLambdaOverloadCodeFixProvider
    {
        protected override async Task<Document> Fix(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
            if (rootAsync == null)
            {
                return document;
            }

            var argumentSyntax = invocation.ArgumentList.Arguments.First();
            var commandName = argumentSyntax.Expression.GetText();
            var arguments = ArgumentList(
                SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]{
                        Argument(
                            ThisExpression()),
                        Token(
                            TriviaList(),
                            SyntaxKind.CommaToken,
                            TriviaList(
                                Space)),
                        Argument(
                            SimpleLambdaExpression(
                                    Parameter(
                                        Identifier(
                                            TriviaList(),
                                            "x",
                                            TriviaList(
                                                Space))))
                               .WithArrowToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.EqualsGreaterThanToken,
                                        TriviaList(
                                            Space)))
                               .WithExpressionBody(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("x"),
                                        IdentifierName(commandName.ToString()))))}));

            var changed = rootAsync.ReplaceNode(invocation.ArgumentList, arguments);

            return document.WithSyntaxRoot(changed);
        }
    }
}