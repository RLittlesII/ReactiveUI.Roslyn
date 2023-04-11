using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RxUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MultipleUsesOfSubscribeOnAnalyzer : DiagnosticAnalyzerBase
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(RXUI0006);

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return;
            }

            foreach (var identifierName in invocationExpressionSyntax
                                           .DescendantNodes()
                                           .OfType<IdentifierNameSyntax>()
                                           .Where(identifierNameSyntax =>
                                                      identifierNameSyntax.Identifier.Text == "SubscribeOn")
                                           .Skip(1))
            {
                context.ReportDiagnostic(Diagnostic.Create(RXUI0006, identifierName.GetLocation()));
            }
        }

        internal static readonly DiagnosticDescriptor RXUI0006 =
            new("RXUI0006",
                "Multiple attempts to subscribe on a thread scheduler",
                "SubscribeOn only supports a single use per expression",
                "Usage",
                DiagnosticSeverity.Info,
                true);
    }
}