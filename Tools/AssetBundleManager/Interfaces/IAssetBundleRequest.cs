﻿using System;
using Tools.AsyncOperations;

namespace AssetBundlesModule {

    public interface IAssetBundleRequest : IAsyncOperation {

        IAssetBundleResource BundleResource { get; }

        string Resource { get; }

        string BundleName { get; }

        void Initialize(string assetBundleName, string resource);

    }

}