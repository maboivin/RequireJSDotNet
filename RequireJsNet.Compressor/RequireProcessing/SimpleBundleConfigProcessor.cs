﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RequireJsNet.Compressor
{
    using RequireJsNet.Compressor.RequireProcessing;
    using RequireJsNet.Configuration;
    using RequireJsNet.Models;

    internal class SimpleBundleConfigProcessor : ConfigProcessor
    {
        public SimpleBundleConfigProcessor(string projectPath, string packagePath, string entryPointOverride, List<string> filePaths)
        {
            ProjectPath = projectPath;
            FilePaths = filePaths;
            OutputPath = projectPath;
            EntryOverride = entryPointOverride;
            if (!string.IsNullOrWhiteSpace(packagePath))
            {
                OutputPath = packagePath;
            }

            EntryPoint = this.GetEntryPointPath();
        }

        public override List<Bundle> ParseConfigs()
        {
            if (!Directory.Exists(ProjectPath))
            {
                throw new DirectoryNotFoundException("Could not find project directory.");
            }

            FindConfigs();

            var loader = new ConfigLoader(
                FilePaths,
                new ExceptionThrowingLogger(),
                new ConfigLoaderOptions { ProcessBundles = true });

            Configuration = loader.Get();

            var bundles = new List<Bundle>();
            foreach (var bundleDefinition in Configuration.Bundles.BundleEntries.Where(r => !r.IsVirtual))
            {
                var bundle = new Bundle();
                bundle.Output = GetOutputPath(bundleDefinition.OutputPath, bundleDefinition.Name);
                bundle.Files = bundleDefinition.BundleItems
                                                .Select(r => new FileSpec(this.ResolvePhysicalPath(r.RelativePath), r.CompressionType))
                                                .ToList();
                bundles.Add(bundle);
            }

            return bundles;
        }
    }
}
