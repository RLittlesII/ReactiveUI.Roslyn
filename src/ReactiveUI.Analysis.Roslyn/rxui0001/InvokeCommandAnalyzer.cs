using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReactiveUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvokeCommandAnalyzer : ExpressionLambdaAnalyzer
    {
        /// <inheritdoc />
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            // This analyser should only pick up public values.
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
                foreach (var diagnostic in syntaxTokens.Where(token => !token.IsKind(SyntaxKind.ThisExpression) &&
                        !token.IsKind(SyntaxKind.SimpleLambdaExpression))
                   .Select(token => Diagnostic.Create(Rule, token.GetLocation(), token)))
                {
                    context.ReportDiagnostic(diagnostic);
                }

            }
        }
    }
}