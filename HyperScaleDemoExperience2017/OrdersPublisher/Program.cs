using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersPublisher
{
    class Program
    {

        const string QueueName = "initialorders";

        // TODO : UPDATE ACCOUNT NAME & KEY FOR YOUR STORAGE
        const string AccountName = "**REPLACE WITH YOUR STORAGE ACCOUNT NAME**";
        const string AccountKey = "**REPLACE WITH YOUR STORAGE ACCOUNT KEY**";

        static void Main(string[] args)
        {
            Console.WriteLine("Push 'order' to queue ");

            Random rnd = new Random((int)DateTime.Now.Ticks);

            CloudQueue queue = CreateQueueIfNotExists(QueueName);
            while (true)
            {
                double orderTotal = rnd.NextDouble() * 1000; // entre 0.0 et 1000.0
                QueueMessage(queue, orderTotal.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine($"Message queued : {orderTotal}");
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


        private static void QueueMessage(CloudQueue queue, string message)
        {
            TimeSpan timeToLive = TimeSpan.FromDays(1);
            queue.AddMessage(new CloudQueueMessage(message), timeToLive);
        }
    }
}
