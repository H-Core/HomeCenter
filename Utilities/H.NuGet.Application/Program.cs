using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;

namespace H.NuGet
{
    internal static class Program
    {
        [MTAThread]
        private static async Task Main(string[] arguments)
        {
            const string rootPath = @"D:\test\";

            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);

            var settings = Settings.LoadDefaultSettings(rootPath);
            var packageSourceProvider = new PackageSourceProvider(settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);

            var project = new FolderNuGetProject(rootPath);
            var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, rootPath)
            {
                PackagesFolderNuGetProject = project
            };

            var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            var supportedFramework = new[] { ".NETFramework,Version=v4.6" };
            var searchFilter = new SearchFilter(true)
            {
                SupportedFrameworks = supportedFramework,
                IncludeDelisted = false,
            };

            var packages = await searchResource.SearchAsync("H.Pipes", searchFilter, 0, 10, null, CancellationToken.None);
            
            INuGetProjectContext projectContext = new EmptyNuGetProjectContext();
            var resolutionContext = new ResolutionContext(
                DependencyBehavior.Ignore,
                true,
                true,
                VersionConstraints.None);

            var jsonPackage = packages.First();
            var versions = await jsonPackage.GetVersionsAsync();
            var identity = new PackageIdentity(jsonPackage.Identity.Id, jsonPackage.Identity.Version);
            await packageManager.InstallPackageAsync(
                project,
                identity,
                resolutionContext,
                projectContext,
                sourceRepository,
                new List<SourceRepository>(),
                CancellationToken.None);

            var infos = await packageManager.GetInstalledPackagesDependencyInfo(project, CancellationToken.None, true);
            var test = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
            var www = await test.ResolvePackage(jsonPackage.Identity, NuGetFramework.AnyFramework,
                new SourceCacheContext(), new ProjectContextLogger(projectContext), CancellationToken.None);

            var tes2t = test;
        }
    }
}
