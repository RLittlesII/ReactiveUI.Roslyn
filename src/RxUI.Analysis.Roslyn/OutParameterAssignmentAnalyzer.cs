using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RxUI.Analysis.Roslyn
{
    public abstract class OutParameterAssignmentAnalyzer : DiagnosticAnalyzerBase
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(RXUI0003);

        internal static readonly DiagnosticDescriptor RXUI0003 =
            new("RXUI0003",
                "Out parameter assignment",
                "Use the out parameter overload",
                "Usage",
                DiagnosticSeverity.Error,
                true);
    }
}