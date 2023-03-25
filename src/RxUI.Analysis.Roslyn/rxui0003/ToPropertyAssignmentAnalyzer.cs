using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RxUI.Analysis.Roslyn
{
    public class ToPropertyAssignmentAnalyzer : OutParameterAssignmentAnalyzer
    {
        /// <inheritdoc />
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (!context.Node.Parent.IsKind(SyntaxKind.EqualsValueClause))
            {
                return;
            }

            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return;
            }

            if (memberAccessExpressionSyntax.Name.Identifier.Text != "ToProperty" || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
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

            foreach (var diagnostic in tokens.Select(token => Diagnostic.Create(RXUI0003, token.GetLocation(), token)))
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}