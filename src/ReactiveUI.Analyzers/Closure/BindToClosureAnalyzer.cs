using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace ReactiveUI.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BindToClosureAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule =
            new("RXUI0002",
                "Unsupported expression type",
                "Provide a well-formed lambda function .BindTo(this, x => x.Property)",
                "Usage",
                DiagnosticSeverity.Error,
                true);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                                   GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // this is where the coding starts,
            // in this case we register a handler (AnalyzeNamedType method defined below)
            // to be invoked analyzing NamedType (class, interface, delegate etc) symbols
            context.RegisterSyntaxNodeAction(action: AnalyzeNode, syntaxKinds: SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// This analyzer should pickup the following.
        /// Any InvokeCommand that used the non expression format if a property exists for the value.
        /// </summary>
        /// <param name="context">The context.</param>
        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // This analyser should only pick up public values.
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

            if (tokens.Any(x => x.IsKind(SyntaxKind.SimpleLambdaExpression)) && tokens.Any(x => x.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
            {
                return;
            }

            foreach (var diagnostic in tokens.Select(token => Diagnostic.Create(Rule, token.GetLocation(), token)))
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);
    }
}