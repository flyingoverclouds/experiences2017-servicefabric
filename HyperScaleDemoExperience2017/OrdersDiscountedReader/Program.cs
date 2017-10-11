using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OrdersDiscountedReader
{
    class Program
    {
        const string QueueName = "discountedorders";
        const string AccountName = "**REPLACE WITH YOUR STORAGE ACCOUNT NAME**";
        const string AccountKey = "**REPLACE WITH YOUR STORAGE ACCOUNT KEY**";

        static void Main(string[] args)
        {
            CloudQueue queue = CreateQueueIfNotExists(QueueName);
            while (true)
            {
                RetrieveOrders(queue);
            }
        }
        public static CloudStorageAccount GetStorageAccount()
        {
            StorageCredentials credentials = new StorageCredentials(AccountName, AccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, true);
            return storageAccount;
        }

        private static CloudQueue CreateQueueIfNotExists(string queueName)
        {
            var account = GetStorageAccount();
            CloudQueueClient queueClient = account.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            bool created = queue.CreateIfNotExists();
            return queue;
        }

        private static void RetrieveOrders(CloudQueue queue)
        {
            const int maxMessagesToGet = 10;
            var visibilityTimeout = TimeSpan.FromSeconds(10);

            IEnumerable<CloudQueueMessage> messages = queue.GetMessages(maxMessagesToGet, visibilityTimeout);
            if (messages.Count() > 0)
            {
                Console.WriteLine();
                foreach (CloudQueueMessage message in messages)
                {
                    string messageContent = message.AsString;
                    Console.WriteLine("Order discounted total : " + messageContent);
                    queue.DeleteMessage(message);
                }
                
            }
            else
                Console.Write("\rno order ....");


        }


    }
}
