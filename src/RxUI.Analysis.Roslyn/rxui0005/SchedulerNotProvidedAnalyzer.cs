using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RxUI.Analysis.Roslyn
{
    /// <summary>
    /// https://stackoverflow.com/a/15396488/2088094
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SchedulerNotProvidedAnalyzer : ImproperUsageAnalyzer
    {
        /// <inheritdoc />
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var thing = context.SemanticModel.GetSymbolInfo(invocationExpression)
                               .Symbol as IMethodSymbol;
            var methodDeclarations =
                context
                   .Node
                   .SyntaxTree
                   .GetRoot()
                   .DescendantNodes()
                   .OfType<InvocationExpressionSyntax>()
                   .Where(
                        x => x.Expression is MemberAccessExpressionSyntax
                        {
                            Name: { Identifier: { Text: "Throttle" } }
                        }
                    )
                   .ToList();

            foreach (var methodDeclaration in methodDeclarations)
            {
                var stuff = context.SemanticModel.GetSymbolInfo(methodDeclaration);
            }

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return;
            }

            if (memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
            {
                return;
            }

            var tokens =
                invocationExpression
                   .ArgumentList
                   .Arguments
                   .Select(argument => argument.DescendantNodesAndTokens())
                   .SelectMany(token => token.Where(x => x.IsKind(SyntaxKind.SimpleLambdaExpression)))
                   .ToList();

            if (!tokens.Any(x => x.IsKind(SyntaxKind.SimpleLambdaExpression)))
            {
                return;
            }

            foreach (var diagnostic in tokens.Select(token => Diagnostic.Create(RXUI0005, token.GetLocation(), token)))
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    public class SchedulerSymbolVisitor : SymbolVisitor
    {
        public override void Visit(ISymbol symbol) => base.Visit(symbol);
    }
    public class SchedulerSyntaxWalker : SyntaxWalker
    {
        public override void Visit(SyntaxNode node)
        {
            switch (node)
            {
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    break;
                case MemberAccessExpressionSyntax:
                    break;
            }

            base.Visit(node);
        }
    }
}