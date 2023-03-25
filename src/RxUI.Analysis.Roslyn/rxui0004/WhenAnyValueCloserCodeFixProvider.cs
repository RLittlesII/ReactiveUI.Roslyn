using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Analysis.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WhenAnyValueClosureCodeFixProvider)), Shared]
    public class WhenAnyValueClosureCodeFixProvider : UnsupportedExpressionCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(UnsupportedExpressionAnalyzer.RXUI0004.Id);

        protected override void RegisterCodeFix(
            CodeFixContext context,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            Diagnostic diagnostic) => context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => Fixup(context, invocation, declaration, c),
                equivalenceKey: UnsupportedExpressionAnalyzer.RXUI0004.Id + UnsupportedExpressionAnalyzer.RXUI0004.Title + "WhenAnyValue"
            ),
            diagnostic
        );

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

            var diagnostic = context.Diagnostics.First(x => x.Id == UnsupportedExpressionAnalyzer.RXUI0004.Id);
            var simpleMemberAccessExpression = rootAsync.FindNode(context.Span);

            var memberAccessExpression =
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(declaration.Parameter.Identifier),
                    IdentifierName(declaration.Body.GetLastToken()));

            var changed = rootAsync.ReplaceNode(simpleMemberAccessExpression, memberAccessExpression);
            return context.Document.WithSyntaxRoot(changed);
        }
    }
}