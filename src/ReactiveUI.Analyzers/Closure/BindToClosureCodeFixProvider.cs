using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BindToClosureCodeFixProvider)), Shared]
    public class BindToClosureCodeFixProvider : CodeFixProvider
    {
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
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
                    equivalenceKey: BindToClosureAnalyzer.Rule.Id + BindToClosureAnalyzer.Rule.Title),
                diagnostic);
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(BindToClosureAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private static async Task<Document> Fixup(Document document, InvocationExpressionSyntax invocation, SimpleLambdaExpressionSyntax declaration, CancellationToken cancellationToken)
        {

            var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
            if (rootAsync == null)
            {
                return document;
            }

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
                                            declaration.Parameter.Identifier.Text,
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
                                        IdentifierName(declaration.Parameter.Identifier),
                                        IdentifierName(declaration.Body.GetLastToken()))))}));

            var changed = rootAsync.ReplaceNode(invocation.ArgumentList, arguments);
            return document.WithSyntaxRoot(changed);
        }

        private const string Title = "Fix constant expression";
    }
}