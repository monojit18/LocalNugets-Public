using System;
using Subsystems.AzureStorage.External;

namespace TestAzureStorage.iOS
{
    public class TestStorageModel : CMPTableStorageModel
    {

        public string TalkMessage { get; set; }

        public TestStorageModel() {}

        public TestStorageModel(string partitionKeyString, string rowKeyString)
            : base(partitionKeyString, rowKeyString) {}

    }
}
