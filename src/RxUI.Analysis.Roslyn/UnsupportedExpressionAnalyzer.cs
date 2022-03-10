using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ReactiveUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class UnsupportedExpressionAnalyzer : DiagnosticAnalyzer
    {
        public UnsupportedExpressionAnalyzer() => SupportedDiagnostics = ImmutableArray.Create(Descriptor());

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // this is where the coding starts,
            // in this case we register a handler (AnalyzeNamedType method defined below)
            // to be invoked analyzing NamedType (class, interface, delegate etc) symbols
            context.RegisterSyntaxNodeAction(action: AnalyzeNode, syntaxKinds: SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// This analyzer find any unsupported constants in expression syntax.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void Analyze(SyntaxNodeAnalysisContext context) { }

        /// <summary>
        /// Provides the <see cref="DiagnosticDescriptor"/> that defines the rule for the analyzer.
        /// </summary>
        /// <returns>The rule descriptor.</returns>
        protected abstract DiagnosticDescriptor Rule();

        private void AnalyzeNode(SyntaxNodeAnalysisContext context) => Analyze(context);

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