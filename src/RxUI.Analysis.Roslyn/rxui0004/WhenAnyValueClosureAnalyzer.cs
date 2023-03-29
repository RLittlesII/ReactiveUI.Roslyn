using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RxUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhenAnyValueClosureAnalyzer : UnsupportedExpressionAnalyzer
    {
        /// <inheritdoc />
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return;
            }

            if (memberAccessExpressionSyntax.Name.Identifier.Text != "WhenAnyValue"
             || memberAccessExpressionSyntax.Expression is not ThisExpressionSyntax)
            {
                return;
            }

            var diagnostics =
                invocationExpression
                    .ArgumentList
                    .Arguments
                    .Select(argumentSyntax => argumentSyntax.Expression)
                    .OfType<SimpleLambdaExpressionSyntax>()
                    .Where(simpleLambdaExpressionSyntax => !simpleLambdaExpressionSyntax.ExpressionBody.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    .Select(simpleLambdaExpressionSyntax => simpleLambdaExpressionSyntax.GetLastToken())
                    .Select(syntaxToken => Diagnostic.Create(RXUI0004, syntaxToken.GetLocation(), syntaxToken))
                    .ToList();

            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule() => RXUI0004;
    }
}