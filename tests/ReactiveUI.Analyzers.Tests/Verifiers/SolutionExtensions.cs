using System.IO;
using Microsoft.CodeAnalysis;

namespace ReactiveUI.Analyzers.Tests.Verifiers
{
    public static class SolutionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="projectId"></param>
        /// <param name="fileName"></param>
        /// <remarks>https://davidwalschots.com/fixing-roslyn-compilation-errors-in-unit-tests-of-a-code-analyzer/</remarks>
        /// <returns></returns>
        internal static Solution AddRuntimeLibrary(this Solution solution, ProjectId projectId, string fileName)
        {
            var runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment
                .GetRuntimeDirectory();
            var dll = Path.Combine(runtimeDirectory, fileName);

            return solution.AddMetadataReference(projectId, MetadataReference.CreateFromFile(dll));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="projectId"></param>
        /// <param name="runtimes"></param>
        /// <remarks>https://davidwalschots.com/fixing-roslyn-compilation-errors-in-unit-tests-of-a-code-analyzer/</remarks>
        /// <returns></returns>
        internal static Solution AddRuntimeLibrary(this Solution solution, ProjectId projectId, params string[] runtimes)
        {
            foreach (var runtime in runtimes)
            {
                AddRuntimeLibrary(solution, projectId, runtime);
            }

            return solution;
        }
    }
}