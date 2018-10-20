# CustomCache
*A Shared Xamarin component for Caching custom data on to disk*

# Usage:

        using Subsystems.Cache.External;
        ....
        ....

## General:
        
        var cachedFolderPathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var cacheProxy = new CMPCacheProxy(cachedFolderPathString, <sizeLimitInKB>); // e.g. 5 * 1024
        cacheProxy.ExpiryDays(<Expiry_Days>); // Cache will invalidate after expiry from Creation or Last Update

        string cacheString = null;				
        byte[] retrievedBytes = cacheProxy.RetieveItem("testfile81");
        if (retrievedBytes != null)
        {
            cacheString = System.Text.Encoding.UTF8.GetString(retrievedBytes);
            Console.WriteLine("cacheString:" + cacheString);
        }
        else
        {

            cacheString = "Test String38";
            byte[] cacheBytes = System.Text.Encoding.UTF8.GetBytes(cacheString);
            cacheProxy.CacheItem(cacheBytes, "testfile81");

        }

## Various ways to define Cache Expiry:

        // Cache will invalidate after expiry from Creation or Last Update

        var cacheProxy = new CMPCacheProxy(cachedFolderPathString, <sizeLimitInKB>);

        cacheProxy.ExpiryDays(<Expiry_Days>);
      OR,
        cacheProxy.ExpiryMonths(<Expiry_Months>);      
      OR,
        cacheProxy.ExpiryYears(<Expiry_Years>);
      OR,
        cacheProxy.ExpiryHours(<Expiry_Years>);
      OR,
        cacheProxy.ExpiryMinutes(<Expiry_Minutes>);
      OR,
        cacheProxy.ExpirySeconds(<Expiry_Seconds>);

