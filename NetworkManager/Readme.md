# NetworkManager
*A Cross platform wrapper for NetworkManager on iOS and Android*

# Usage:
```` 
    using Subsystems.NetworkManagerShared.External;

````
...
...
```` 
    private CMPNetworkManagerSharedProxy _networkManagerProxy;

````

## Initialize
````
    _networkManagerProxy = new CMPNetworkManagerSharedProxy(this);

````
## NetworkStatusChanged       
````
_networkManagerProxy.NetworkStatusChanged += (object sender,
                                                          CMPNetworkEventArgs e) =>
{

    Console.WriteLine(e.State);

};

_networkManagerProxy.StartMonitoring();

````
    
        
## IsNetworkReachable
````
_networkManagerProxy.IsNetworkReachable();
````
    
## StartMonitoring
````
_networkManagerProxy.StartMonitoring();
````

## StopMonitoring
````
_networkManagerProxy.StopMonitoring();
````
