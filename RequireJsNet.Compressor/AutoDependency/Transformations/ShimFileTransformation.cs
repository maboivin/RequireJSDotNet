﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequireJsNet.Compressor.AutoDependency.Transformations
{
    using System.IO;

    using RequireJsNet.Compressor.Parsing;
    using RequireJsNet.Compressor.Transformations;
    using RequireJsNet.Helpers;

    internal class ShimFileTransformation : IRequireTransformation
    {
        private List<string> dependencies;

        private string moduleName;

        public ShimFileTransformation(string moduleName, List<string> dependencies)
        {
            this.moduleName = moduleName;
            this.dependencies = dependencies;
        }

        public RequireCall RequireCall { get; set; }

        public static ShimFileTransformation Create(string moduleName, List<string> dependencies)
        {
            return new ShimFileTransformation(moduleName, dependencies);
        }

        public void Execute(ref string script)
        {
            var depString = string.Format("[{0}]", string.Join(",", dependencies.Select(r => "'" + r + "'")));

            script = string.Format("define('{0}', {1}, function () {{{2}{3}{2}}});", moduleName.ToModuleName(), depString, Environment.NewLine, script);
        }

        public int[] GetAffectedRange()
        {
            // this range is only here to position this as the first transformation that should be executed
            // it's likely that no other transformations will be run on this script since it doesn't have a req call,
            // so it doesn't really matter anyway
            return new[] { 0, 1 };
        }
    }
}
