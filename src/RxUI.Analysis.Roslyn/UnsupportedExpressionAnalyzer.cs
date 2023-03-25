using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RxUI.Analysis.Roslyn
{
    public abstract class UnsupportedExpressionAnalyzer : DiagnosticAnalyzerBase
    {
        protected UnsupportedExpressionAnalyzer() => SupportedDiagnostics = ImmutableArray.Create(Descriptor());

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        /// <summary>
        /// Provides the <see cref="DiagnosticDescriptor"/> that defines the rule for the analyzer.
        /// </summary>
        /// <returns>The rule descriptor.</returns>
        protected abstract DiagnosticDescriptor Rule();

        internal static DiagnosticDescriptor RXUI0002 { get; } = new(
            "RXUI0002",
            "Unsupported expression type.",
            "Provide a well-formed lambda expression",
            "Usage",
            DiagnosticSeverity.Error,
            true);

        internal static DiagnosticDescriptor RXUI0004 { get; } = new(
            "RXUI0004",
            "Unsupported expression missing member access prefix.",
            "Provide a well-formed lambda expression",
            "Usage",
            DiagnosticSeverity.Error,
            true);

        private DiagnosticDescriptor Descriptor() => Rule();
    }
}