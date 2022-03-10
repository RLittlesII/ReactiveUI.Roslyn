using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace ReactiveUI.Analysis.Roslyn;

public class WhenAnyValueClosureAnalyzer : UnsupportedExpressionAnalyzer
{
    /// <inheritdoc />
    protected override void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return;
        }

        if (memberAccessExpressionSyntax.Name.Identifier.Text != "WhenAnyValue" || memberAccessExpressionSyntax.Expression is not ThisExpressionSyntax)
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

        var node =
            tokens.Where(x => x.IsKind(SyntaxKind.SimpleLambdaExpression) && x.Parent.IsKind(SyntaxKind.Argument))
               .Select(x => x.AsNode())
               .OfType<SimpleLambdaExpressionSyntax>()
               .Where(x => x.ExpressionBody.IsKind(SyntaxKind.IdentifierName))
               .Select(x => x.GetLastToken());

        foreach (var diagnostic in node.Select(token => Diagnostic.Create(RXUI0004, token.GetLocation(), token)))
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <inheritdoc />
    protected override DiagnosticDescriptor Rule() => RXUI0004;
}