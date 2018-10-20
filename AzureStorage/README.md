# AzureStorage
*A Xamarin wrapper binding for Azure Storage APIs - Blob, Queue, Table*

## Usage

### General

        using Subsystems.AzureStorage.External;
        ....
        ....       
		CMPAzureStorageFactory.CreateBlobStorage("<Connection_String>", "<Container_Name>");
		
### Data Models
        
        public class CMPTableStorageModel : TableEntity {...}        
        public class CMPStorageOptions
        {

            public TimeSpan TimeToLive
            public TimeSpan VisibilityTimeoutOrDelay
            public OperationContext OperationContext
        }
        public class CMPTableStorageOptions : CMPStorageOptions
        {
            public TableRequestOptions TableRequestOptions
        }
        public class CMPBlobStorageOptions : CMPStorageOptions
        {
            public BlobContainerPublicAccessType BlobContainerPublicAccessType
            public AccessCondition AccessCondition
            public DeleteSnapshotsOption DeleteSnapshotsOption
            public BlobRequestOptions BlobRequestOptions
        }
        public class CMPQueueStorageOptions : CMPStorageOptions
        {
            public QueueRequestOptions QueueRequestOptions
            public MessageUpdateFields MessageUpdateFields
        }		
                 
### Create Storage Proxy

**Blob**

	CMPAzureStorageFactory.CreateBlobStorage("<Connection_String>", "<Container_Name>");

**Queue**

        CMPAzureStorageFactory.CreateQueueStorage("<Connection_String>", "<Queue_Name>");

**Table**

        CMPAzureStorageFactory.CreateTableStorage("<Connection_String>");

### Blob APIs

         public async Task<Tuple<bool, CMPStorageError>> CreateBlobContainerAsync()
        
         public async Task<Tuple<bool, CMPStorageError>> AddBytesToBlobAsync(byte[] blobBytesArray,
                                                                            string blobNameString)
                                                                                    
         public async Task<Tuple<bool, CMPStorageError>> AddStreamToBlobAsync(Stream blobStream, string blobNameString)
        
       	 public async Task<Tuple<bool, CMPStorageError>> DeleteBlobAsync(string blobNameString)
        
         public async Task<Tuple<bool, CMPStorageError>> DeleteBlobContainerAsync()
        
   	     public async Task<Tuple<byte[], CMPStorageError>> DownloadFromBlobAsync(string blobNameString)
        
### Queue APIs

        public async Task<Tuple<bool, CMPStorageError>> AddMessageAsync(string messageString,
                                                                        string popupTokenString,
                                                                        CMPQueueStorageOptions queueStorageOptions =
                                                                        null)        
        public async Task<int> GetMessagesCountAsync(CMPQueueStorageOptions queueStorageOptions)
        
        public async Task<Tuple<string, string, CMPStorageError>> PeekMessageAsync(CMPQueueStorageOptions
                                                                                   queueStorageOptions = null)
        
        public async Task<Tuple<bool, CMPStorageError>> DeQueueMessageAsync(TimeSpan visibilityTimeOut,
                                                                            CMPQueueStorageOptions queueStorageOptions)
        
        public async Task<Tuple<bool, CMPStorageError>> DeQueueBatchMessagesAsync(TimeSpan visibilityTimeOut,
                                                                                  int deQueueCount,
                                                                                  CMPQueueStorageOptions
                                                                                  queueStorageOptions = null)
        
        public async Task<Tuple<bool, CMPStorageError>> UpdateMessageAsync(string updateMessageString,
                                                                           TimeSpan visibilityTimeOut,
                                                                           CMPQueueStorageOptions
                                                                           queueStorageOptions = null)
         await _azureQueueStorageProxy.UpdateMessageAsync(<UpdateMessage>, <TimeOut>, <QueueOptons>);

        public async Task<Tuple<bool, CMPStorageError>> DeleteMessageAsync(CloudQueueMessage queueMessage,
                                                                           CMPQueueStorageOptions
                                                                           queueStorageOptions = null)
        
        public async Task<Tuple<bool, CMPStorageError>> DeleteBatchMessagesAsync(List<CloudQueueMessage>
                                                                                 queueMessagesList,
                                                                                 CMPQueueStorageOptions
                                                                                 queueStorageOptions = null)
        
        public async Task<Tuple<bool, CMPStorageError>> DeleteQueueAsync(CMPQueueStorageOptions queueStorageOptions =
                                                                         null)
### Table APIs

**Callbacks**
        
        public delegate void FetchProgressCallback<T>(List<T> fetchedResults)
        
**Methods**        

        public async Task<Tuple<bool, CMPStorageError>> CreateTableAsync(string tableNameString)	

        public async Task<Tuple<bool, CMPStorageError>> DeleteTableAsync(string tableNameString)
        
        public async Task<Tuple<T, CMPStorageError>> InsertRowAsync<T>(T insertModel, string tableNameString)            
        
        public async Task<Tuple<T, CMPStorageError>> InsertRowsInBatchAsync<T>(T insertModel, string tableNameString)
        
        public async Task<Tuple<T, CMPStorageError>> UpdateRowAsync<T>(string tableNameString, T updateModel)

        public async Task<Tuple<T, CMPStorageError>> UpdateRowsAsync<T>(string tableNameString, T updateModel)
                    
        public async Task<List<Tuple<T, CMPStorageError>>> UpdateRowsAsync<T>(string tableNameString,
                                                                              string partitionKeyString)
                                                                              
        public async Task<Tuple<T, CMPStorageError>> DeleteRowAsync<T>(string tableNameString, T deleteModel)
        
        public async Task<Tuple<T, CMPStorageError>> DeleteRowsAsync<T>(string tableNameString, T deleteModel)
        
        public async Task<Tuple<List<T>, CMPStorageError>> FetchAllAsync<T>(string tableNameString,
                                                                            FetchProgressCallback<T>
                                                                            fetchProgressCallback)

        public async Task<Tuple<List<T>, CMPStorageError>> FetchAsync<T>(string tableNameString,
                                                                         string partitionKeyString,
                                                                         FetchProgressCallback<T> fetchProgressCallback)            
        public async Task<Tuple<T, CMPStorageError>> FetchAsync<T>(string tableNameString,
                                                                         string partitionKeyString,
                                                                         string rowKeyString)
        
