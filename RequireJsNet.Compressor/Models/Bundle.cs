﻿using System.Collections.Generic;

namespace RequireJsNet.Compressor
{
    internal class Bundle
    {
        public string Output { get; set; }

        public List<FileSpec> Files { get; set; }

        public string ContainingConfig { get; set; }

        public string BundleId { get; set; }
    }
}
