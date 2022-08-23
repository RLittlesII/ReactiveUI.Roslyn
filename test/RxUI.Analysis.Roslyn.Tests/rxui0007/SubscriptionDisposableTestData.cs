using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using ReactiveUI.Analysis.Roslyn.Tests.Verifiers;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Analysis.Roslyn.Tests.rxui0007
{
    public class SubscriptionDisposableTestData : IEnumerable<object[]>
    {
        private readonly List<DiagnosticResult> _diagnostics = new List<DiagnosticResult>()
        {
            AnalyzerVerifier<SubscriptionDisposalAnalyzer>.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
               .WithSeverity(DiagnosticSeverity.Warning)
               .WithSpan(15, 17, 15, 23)
               .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString()),
            AnalyzerVerifier<SubscriptionDisposalAnalyzer>.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
               .WithSeverity(DiagnosticSeverity.Warning)
               .WithSpan(19, 17, 19, 30)
               .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString()),
            AnalyzerVerifier<SubscriptionDisposalAnalyzer>.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
               .WithSeverity(DiagnosticSeverity.Warning)
               .WithSpan(23, 17, 23, 26)
               .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString()),
            AnalyzerVerifier<SubscriptionDisposalAnalyzer>.Diagnostic(SubscriptionDisposalAnalyzer.Rule.Id)
               .WithSeverity(DiagnosticSeverity.Warning)
               .WithSpan(27, 17, 27, 27)
               .WithMessage(SubscriptionDisposalAnalyzer.Rule.MessageFormat.ToString())
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { _diagnostics, SubscriptionDisposalTestData.Incorrect, SubscriptionDisposalTestData.Incorrect };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}