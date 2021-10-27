using JetBrains.Annotations;
using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using Rocket.Surgery.Nuke;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[PublicAPI]
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
// [PackageIcon("https://raw.githubusercontent.com/RocketSurgeonsGuild/graphics/master/png/social-square-thrust-rounded.png")]
[EnsureGitHooks(GitHook.PreCommit)]
[EnsureReadmeIsUpdated("Readme.md")]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
[ShutdownDotNetAfterServerBuild]
[GitHubActionsSteps(
    "ci",
    GitHubActionsImage.MacOsLatest,
    AutoGenerate = false,
    On = new[] { GitHubActionsTrigger.Push },
    OnPushTags = new[] { "v*" },
    OnPushBranches = new[] { "main", "next" },
    OnPullRequestBranches = new[] { "main", "next" },
    InvokedTargets = new[] { nameof(Deliver) },
    NonEntryTargets = new[]
    {
        nameof(ICIEnvironment.CIEnvironment),
        nameof(Deliver)
    },
    ExcludedTargets = new[] { nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore) }
)]
[PrintBuildVersion]
[PrintCIEnvironment]
class ContinuousDelivery : NukeBuild,
                                  ICanRestoreWithDotNetCore,
                                  ICanBuildWithDotNetCore,
                                  ICanTestWithDotNetCore,
                                  ICanPackWithDotNetCore,
                                  IHaveDataCollector,
                                  ICanClean,
                                  ICanUpdateReadme,
                                  IGenerateCodeCoverageReport,
                                  IGenerateCodeCoverageSummary,
                                  IGenerateCodeCoverageBadges,
                                  IHaveConfiguration<Configuration>
{
    public static int Main() => Execute<ContinuousDelivery>(x => x.Deliver);

    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    public Target Build => _ => _.Inherit<ICanBuildWithDotNetCore>(x => x.CoreBuild);

    public Target Pack => _ => _.Inherit<ICanPackWithDotNetCore>(x => x.CorePack)
       .DependsOn(Clean);

    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;

    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);
    public Target Restore => _ => _.Inherit<ICanRestoreWithDotNetCore>(x => x.CoreRestore);
    public Target Test => _ => _.Inherit<ICanTestWithDotNetCore>(x => x.CoreTest);

    public Target BuildVersion => _ => _.Inherit<IHaveBuildVersion>(x => x.BuildVersion)
       .Before(Deliver)
       .Before(Clean);

    Target Deliver => _ => _
       .DependsOn(Restore)
       .DependsOn(Build)
       .DependsOn(Test)
       .DependsOn(Pack);

    [Parameter("Configuration to build")] public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;
}
