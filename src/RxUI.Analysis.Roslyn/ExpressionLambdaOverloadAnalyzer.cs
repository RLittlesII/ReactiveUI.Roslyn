using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RxUI.Analysis.Roslyn
{
    public abstract class ExpressionLambdaOverloadAnalyzer : DiagnosticAnalyzerBase
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(RXUI0001);

        internal static readonly DiagnosticDescriptor RXUI0001 =
            new("RXUI0001",
                "Use expression lambda overload",
                "Use expression lambda overload for property {0}",
                "Usage",
                DiagnosticSeverity.Warning,
                true);
    }
}