namespace Protogame
{
    using System;
    
    public class UberEffectAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is UberEffectAsset;
        }
        
        public IRawAsset Handle(IAsset asset, AssetTarget target)
        {
            var effectAsset = asset as EffectAsset;

            if (effectAsset.SourcedFromRaw && target != AssetTarget.CompiledFile)
            {
                // We were sourced from a raw FX; we don't want to save
                // an ".asset" file back to disk.
                return null;
            }

            if (target == AssetTarget.CompiledFile)
            {
                if (effectAsset.PlatformData == null)
                {
                    throw new InvalidOperationException(
                        "Attempted save of effect asset as a compiled file, but the effect wasn't compiled.  This usually " +
                        "indicates that you are compiling on Linux, but no remote effect compiler on a Windows machine " +
                        "could be located to perform the compilation.");
                }

                return new CompiledAsset
                {
                    Loader = typeof(UberEffectAssetLoader).FullName, 
                    PlatformData = effectAsset.PlatformData
                };
            }

            return
                new AnonymousObjectBasedRawAsset(
                    new
                    {
                        Loader = typeof(UberEffectAssetLoader).FullName,
                        effectAsset.Code,
                        PlatformData = target == AssetTarget.SourceFile ? null : effectAsset.PlatformData
                    });
        }
    }
}