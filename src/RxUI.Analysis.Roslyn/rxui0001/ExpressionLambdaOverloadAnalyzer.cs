using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ReactiveUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class ExpressionLambdaOverloadAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Rule =
            new("RXUI0001",
                "Use expression lambda overload",
                "Use expression lambda overload for property {0}",
                "Usage",
                DiagnosticSeverity.Warning,
                true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

        public sealed override void Initialize(AnalysisContext context)
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
        /// This analyzer should find any operator that uses the non expression format if a property exists for the value.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void Analyze(SyntaxNodeAnalysisContext context) { }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context) => Analyze(context);
    }
}