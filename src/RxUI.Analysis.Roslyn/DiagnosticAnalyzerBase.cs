using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReactiveUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class DiagnosticAnalyzerBase : DiagnosticAnalyzer
    {
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // this is where the coding starts,
            // in this case we register a handler (AnalyzeNamedType method defined below)
            // to be invoked analyzing NamedType (class, interface, delegate etc) symbols
            context.RegisterSyntaxNodeAction(action: AnalyzeNode, syntaxKinds: GetKind());
        }


        protected virtual void Analyze(SyntaxNodeAnalysisContext context){}

        protected virtual SyntaxKind GetSyntaxKind() => SyntaxKind.InvocationExpression;

        /// <summary>
        /// This analyzer find any unsupported constants in expression syntax.
        /// </summary>
        /// <param name="context">The context.</param>
        private void AnalyzeNode(SyntaxNodeAnalysisContext context) => Analyze(context);

        private SyntaxKind GetKind() => GetSyntaxKind();
    }
}