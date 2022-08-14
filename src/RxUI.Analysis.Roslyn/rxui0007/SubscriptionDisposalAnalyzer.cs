using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReactiveUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SubscriptionDisposalAnalyzer : DiagnosticAnalyzerBase
    {
        internal static readonly DiagnosticDescriptor Rule =
            new("RXUI0007",
                "Subscription is not disposed",
                "Consider use of DisposeWith to clean up subscriptions",
                "Usage",
                DiagnosticSeverity.Warning,
                true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.Parent is not ExpressionStatementSyntax expressionStatementSyntax)
            {
                return;
            }

            if (expressionStatementSyntax.Expression is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return;
            }

            if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                return;
            }
            //
            // if (memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax { Expression: InvocationExpressionSyntax invocationExpressionSyntax })
            // {
            //     return;
            // }

            if (!_subscriptionAccess.Any(methodName => methodName.Contains(memberAccessExpressionSyntax.Name.Identifier.Text)) || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccessExpressionSyntax.Name.Identifier.GetLocation()));
        }

        private readonly List<string> _subscriptionAccess = new List<string> { "InvokeCommand", "Subscribe", "ToProperty", "BindTo" };
    }
}
