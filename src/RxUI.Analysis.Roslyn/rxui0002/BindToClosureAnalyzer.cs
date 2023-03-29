using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RxUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BindToClosureAnalyzer : UnsupportedExpressionAnalyzer
    {
        /// <inheritdoc />
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return;
            }

            if (memberAccessExpressionSyntax.Name.Identifier.Text != "BindTo" || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
            {
                return;
            }

            var tokens =
                invocationExpression
                   .ArgumentList
                   .Arguments
                   .Select(argument => argument.DescendantNodesAndTokens())
                   .SelectMany(token => token.Where(x => x.IsKind(SyntaxKind.SimpleLambdaExpression) || x.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
                   .ToList();

            if (tokens.Any(syntaxNodeOrToken => syntaxNodeOrToken.IsKind(SyntaxKind.SimpleLambdaExpression)) && tokens.Any(syntaxNodeOrToken => syntaxNodeOrToken.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
            {
                return;
            }

            foreach (var diagnostic in tokens.Select(token => Diagnostic.Create(RXUI0002, token.GetLocation(), token)))
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule() => RXUI0002;
    }
}