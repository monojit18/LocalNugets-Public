# ImageLoader
*A Reusable component to Download Image directly from URL. This also supportes Caching the downloaded images efficiently*

# Usage:

        using Subsystems.Cache.External;
        using Subsystems.ImageLoader.External;
        ....
        ....
        
        private CMPCacheProxy _cacheProxy;
        private CMPImageLoaderProxy _imageLoaderProxy;
        ....
        ....
        
        var baseFolderPathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _cacheProxy = new CMPCacheProxy(baseFolderPathString, 5 * 1024);
        _cacheProxy.ExpiryDays(1);
        
## Explicit Cache        
**Inject Cache explicitly** <br>

        _imageLoaderProxy = new CMPImageLoaderProxy("<Image_URL>", _cacheProxy);
        await _imageLoaderProxy.LoadImageAsync();
        
## Implicit Cache
**This will create a default Cache with 7 days Expiry** <br>

        _imageLoaderProxy = new CMPImageLoaderProxy("<Image_URL>", <sieLimitInKB>);        
        await _imageLoaderProxy.LoadImageAsync();
