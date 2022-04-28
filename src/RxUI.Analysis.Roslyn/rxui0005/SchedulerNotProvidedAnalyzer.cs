using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReactiveUI.Analysis.Roslyn
{
    public class SchedulerNotProvidedAnalyzer : ImproperUsageAnalyzer
    {
        /// <inheritdoc />
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var thing = context.SemanticModel.GetSymbolInfo(invocationExpression)
                               .Symbol as IMethodSymbol;

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
}