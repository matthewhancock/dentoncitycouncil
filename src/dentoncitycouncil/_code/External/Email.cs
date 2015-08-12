using dentoncitycouncil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace External {
    public class Email {
        private static CloudQueue _queue = null;

        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get { return "denton"; } }

        public async Task Send() {
            if (_queue == null) {
                _queue = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={Application.Queue.Name};AccountKey={Application.Queue.Key}").CreateCloudQueueClient().GetQueueReference(Application.Queue.Name);
            }
            await _queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(this)));
        }
    }
}
