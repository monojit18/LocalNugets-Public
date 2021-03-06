﻿/*
 * 
 * Copyright 2018 Monojit Datta

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 *
 */

using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Subsystems.AzureStorage.External
{
    public class CMPStorageOptions
    {

        public TimeSpan TimeToLive { get; set; }
        public TimeSpan VisibilityTimeoutOrDelay { get; set; }
        public OperationContext OperationContext { get; set; }

    }

    public class CMPTableStorageOptions : CMPStorageOptions
    {

        public TableRequestOptions TableRequestOptions { get; set; }

    }

    public class CMPBlobStorageOptions : CMPStorageOptions
    {

        public BlobContainerPublicAccessType BlobContainerPublicAccessType { get; set; }
        public AccessCondition AccessCondition { get; set; }
        public DeleteSnapshotsOption DeleteSnapshotsOption { get; set; }
        public BlobRequestOptions BlobRequestOptions { get; set; }

    }

    public class CMPQueueStorageOptions : CMPStorageOptions
    {

        public QueueRequestOptions QueueRequestOptions { get; set; }
        public MessageUpdateFields MessageUpdateFields { get; set; }

    }

}
