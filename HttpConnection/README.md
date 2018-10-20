# HttpConnection
*A shared Xamarin wrapper binding for HttpClient*

# Usage:
        using Subsystems.HttpConnection.External;
        
        ...
        ...

## Initialize
        1.  var httpConnectionProxy = new CMPHttpConnectionProxy();
            httpConnectionProxy.URL("http://www.google.com").Build();
            
        2. var httpClientProxy = new CMPHttpConnectionProxy();
            httpClientProxy.URL($"<URL>")
                           .Headers("<Header_Key1>", "<Header_Value1>")
                           .Headers("<Header_Key2>", "<Header_Value2>")
                           .JsonBody("<Body_Key1>", "<Body_Value1>")
                           .JsonBody("<Body_Key2>", "<Body_Value2>")
                           .Build();
        
## GetAsync       
        var httpResponse = await httpConnectionProxy.GetAsync();
        Console.WriteLine(httpResponse.ResponseString);
        
## PostAsync       
        var httpResponse = await httpConnectionProxy.PostAsync();
        Console.WriteLine(httpResponse.ResponseString);
        
## PutAsync       
        var httpResponse = await httpConnectionProxy.PutAsync();
        Console.WriteLine(httpResponse.ResponseString);
        
## DeleteAsync       
        var httpResponse = await httpConnectionProxy.DeleteAsync();
        Console.WriteLine(httpResponse.ResponseString);
        
## GetBytesWithProgressAsync 
        await cl.GetBytesWithProgressAsync((httpResponse, progressBytes, totalBytes) =>
        {

            responseString = string.Concat(responseString, httpResponse.ResponseString);
            Console.WriteLine("Response:" + responseString);
            Console.WriteLine("progressBytes: " + progressBytes + "\ntotalBytes: " + totalBytes);

        });
