using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Globalization;

namespace DiscountCalculatorSvc
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class DiscountCalculatorSvc : StatelessService
    {
        public DiscountCalculatorSvc(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        const string OrderInitialQueueName = "initialorders";
        const string OrderDiscountedQueueName = "discountedorders";
        const string AccountName = "**REPLACE WITH YOUR STORAGE ACCOUNT NAME**";
        const string AccountKey = "**REPLACE WITH YOUR STORAGE ACCOUNT KEY**";

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var csa = GetStorageAccount();
            var orderInitialQueue = CreateQueueIfNotExists(OrderInitialQueueName);
            var orderDiscountedQueue = CreateQueueIfNotExists(OrderDiscountedQueueName);

            var visibilityTimeout = TimeSpan.FromSeconds(10);

          

            long iterations = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                try
                {

                   // read message
                    var cqMsg = await orderInitialQueue.GetMessageAsync();
                    if (cqMsg == null)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);
                        continue;
                    }
                    var amount = double.Parse(cqMsg.AsString, CultureInfo.InvariantCulture);

                    // calcuel discount
                    double discountedAmount = Math.Ceiling(amount * 0.75);

                    // write message
                    await orderDiscountedQueue.AddMessageAsync(new CloudQueueMessage(discountedAmount.ToString(CultureInfo.InvariantCulture)));

                    // confirmation du traitement du message
                    await orderInitialQueue.DeleteMessageAsync(cqMsg);
                }
                catch
                {
                    // NE PAS FAIRE CA ... SI SI ... JE PLAISANTE PAS !! C'est BEURK !
                }
                await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
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
       
    }
}
