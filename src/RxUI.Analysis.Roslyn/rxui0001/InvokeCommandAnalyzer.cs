using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReactiveUI.Analysis.Roslyn
{
    public class InvokeCommandAnalyzer : ExpressionLambdaOverloadAnalyzer
    {
        /// <inheritdoc />
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return;
            }

            if (memberAccessExpressionSyntax.Name.Identifier.Text != "InvokeCommand" || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
            {
                return;
            }

            foreach (var syntaxTokens in invocationExpression.ArgumentList.Arguments.Select(argument => argument.ChildNodesAndTokens()))
            {
                var diagnostics =
                    syntaxTokens
                       .Where(token => !token.IsKind(SyntaxKind.ThisExpression) && !token.IsKind(SyntaxKind.SimpleLambdaExpression))
                       .Select(token => Diagnostic.Create(Rule, token.GetLocation(), token));

                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}