using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RxUI.Analysis.Roslyn.rxui0008
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StaticLambdaCodeFixProvider)), Shared]
    public class StaticLambdaCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Make lambda expression static";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(StaticLambdaAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null)
                return;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the lambda expression identified by the diagnostic
            var token = root.FindToken(diagnosticSpan.Start);
            var lambda = token.Parent?.AncestorsAndSelf().OfType<LambdaExpressionSyntax>().FirstOrDefault();
            if (lambda == null)
                return;

            // Register the code fix
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => MakeLambdaStaticAsync(context.Document, lambda, c),
                    equivalenceKey: Title
                ),
                diagnostic
            );
        }

        private static async Task<Document> MakeLambdaStaticAsync(Document document, LambdaExpressionSyntax lambda, CancellationToken cancellationToken)
        {
            // Create a static keyword with appropriate spacing
            var staticKeyword = SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space);

            // Create new lambda with static keyword added
            var newModifiers = lambda.Modifiers.Add(staticKeyword);
            var newLambda = lambda.WithModifiers(newModifiers)
               .WithAdditionalAnnotations(Formatter.Annotation);

            // Apply the changes
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null)
            {
                return document;
            }

            var newRoot = root.ReplaceNode(lambda, newLambda);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}