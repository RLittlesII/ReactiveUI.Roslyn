using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReactiveUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvokeCommandAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule =
            new("RXUI0001",
                "Use expression lambda overload",
                "Use expression lambda overload for property {0}",
                "Usage",
                DiagnosticSeverity.Warning,
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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);
    }
}