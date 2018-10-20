# ImageLoader
*A custom ImageView component for iOS with inteligent Caching, Handle Cache expiry, Automatic Download*

# Usage:

        using Subsystems.Cache.External;
        using Subsystems.ImageLoader.External;
        using Subsystems.CustomImageViewiOS.External;
        
        ....
        ....
        
        private CMPImageView _imageView1;
        private CMPImageView _imageView2;
        private CMPCacheProxy _cacheProxy;
        private CMPImageLoaderProxy _imageLoaderProxy1;
        private CMPImageLoaderProxy _imageLoaderProxy2;
            
        ....
        ....
        
        var baseFolderPathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _cacheProxy = new CMPCacheProxy(baseFolderPathString, 5 * 1024);

        _imageLoaderProxy1 = new CMPImageLoaderProxy("https://images-eu.ssl-images-amazon.com/images/I/51L0Rro4T1L._SL100_.jpg",
                                                    _cacheProxy);

        _imageLoaderProxy2 = new CMPImageLoaderProxy("https://images-eu.ssl-images-amazon.com/images/I/51L0Rro4T1L._SL160_.jpg",
                                                    _cacheProxy);
                                                    
        _imageView1 = new CMPImageView(ImageView1, _imageLoaderProxy1);
        _imageView1.ExpirySeconds(10);

        _imageView2 = new CMPImageView(ImageView2, _imageLoaderProxy2);
        _imageView2.ExpirySeconds(10);
        
        ....
        ....
        
         _imageView1.LoadImageAsync();
         _imageView2.LoadImageAsync();
        
        
