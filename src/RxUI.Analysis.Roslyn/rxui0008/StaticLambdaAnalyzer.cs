using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace RxUI.Analysis.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StaticLambdaAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RXUI0008);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // Register for syntax nodes rather than symbols to improve performance
            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression);
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            // Check language version early to avoid unnecessary work
            if (( (CSharpParseOptions)context.Node.SyntaxTree.Options ).LanguageVersion < LanguageVersion.CSharp9)
                return;

            var lambdaExpression = (LambdaExpressionSyntax)context.Node;

            // Skip if already static
            if (lambdaExpression.Modifiers.Any(SyntaxKind.StaticKeyword))
                return;

            // Skip if lambda has attributes (probably decorator patterns)
            if (lambdaExpression.AttributeLists.Count > 0)
                return;

            // Use lazy semantic analysis - only get the semantic model when needed
            var semanticModel = context.SemanticModel;

            // Use cheaper operation analysis first before diving into data flow analysis
            var operation = semanticModel.GetOperation(lambdaExpression);
            if (operation == null)
                return;

            // Quick check for 'this' keyword or base calls which would make the lambda non-static
            if (ContainsThisOrBase(lambdaExpression))
                return;

            // Now do the more expensive data flow analysis
            var dataFlowAnalysis = ModelExtensions.AnalyzeDataFlow(semanticModel, lambdaExpression);
            if (dataFlowAnalysis == null || !dataFlowAnalysis.Succeeded)
                return;

            // If it doesn't capture any variables, it can be static
            if (!dataFlowAnalysis.CapturedInside.Any() && !dataFlowAnalysis.CapturedOutside.Any())
            {
                var diagnostic = Diagnostic.Create(RXUI0008, lambdaExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool ContainsThisOrBase(SyntaxNode node) => node.DescendantNodes().Any(n => n.IsKind(SyntaxKind.ThisExpression) || n.IsKind(SyntaxKind.BaseExpression));

        internal static readonly DiagnosticDescriptor RXUI0008 = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description
        );

        public const string DiagnosticId = "RXUI0008";
        private const string Title = "Lambda expression can be made static";
        private const string MessageFormat = "Lambda expression can be made static to prevent accidental variable capture";
        private const string Description = "Lambda expressions that don't capture local variables or instance state can be marked as static.";
        private const string Category = "Performance";
    }
}