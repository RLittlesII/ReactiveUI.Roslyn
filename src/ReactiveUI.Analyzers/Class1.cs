using Microsoft.CodeAnalysis;

namespace ReactiveUI.Analyzers
{
    static class Descriptors
    {
        internal static readonly DiagnosticDescriptor RXUI0001UseExpressionLambdaOverload =
            new("RXUI0001",
                "Use expression Lambda overload",
                "{0} Use expression Lambda overload",
                "Naming",
                DiagnosticSeverity.Error,
                true);
    }
}