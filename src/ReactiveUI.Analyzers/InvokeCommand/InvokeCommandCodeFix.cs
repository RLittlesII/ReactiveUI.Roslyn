using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReactiveUI.Analyzers
{
    public class InvokeCommandCodeFix : CodeFixProvider
    {
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            SyntaxNode node = root.FindNode(context.Span);

            if (node is not ArgumentSyntax argumentSyntax)
            {
                return;
            }

            SemanticModel documentSemanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            ISymbol invokeCommand = documentSemanticModel.GetEnclosingSymbol(argumentSyntax.SpanStart);
        }
// should be overriden and should return immutable array of ids that could be fixed by provider
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(InvokeCommandAnalyzer.Rule.Id);
    }
}