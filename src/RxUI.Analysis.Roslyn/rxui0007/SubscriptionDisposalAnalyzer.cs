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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(RXUI0007);

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

            if (!_subscriptionAccess.Any(methodName => methodName.Contains(memberAccessExpressionSyntax.Name.Identifier.Text)) || memberAccessExpressionSyntax.Expression is not InvocationExpressionSyntax)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(RXUI0007, memberAccessExpressionSyntax.Name.Identifier.GetLocation()));
        }

        internal static readonly DiagnosticDescriptor RXUI0007 =
            new("RXUI0007",
                "Subscription not disposed",
                "Consider use of DisposeWith to clean up subscriptions",
                "Usage",
                DiagnosticSeverity.Warning,
                true);

        private readonly List<string> _subscriptionAccess = new List<string> { "InvokeCommand", "HandledSubscribe", "SafeSubscribe", "SubscribeSafe", "ToProperty", "BindTo" };
    }
}
