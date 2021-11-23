using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ReactiveUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class UnsupportedExpressionAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule =
            new("RXUI0002",
                "Unsupported expression type",
                "Provide a well-formed lambda expression",
                "Usage",
                DiagnosticSeverity.Error,
                true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

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
        protected virtual void Analyze(SyntaxNodeAnalysisContext context){}

        private void AnalyzeNode(SyntaxNodeAnalysisContext context) => Analyze(context);
    }
}