using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WebFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            

            var qInitial = CreateQueueIfNotExists(OrderInitialQueueName);
            var qDiscounted = CreateQueueIfNotExists(OrderDiscountedQueueName);
            qInitial.FetchAttributes();
            qDiscounted.FetchAttributes();
            ViewData["Message"] = $" {qInitial.ApproximateMessageCount} in InitialQueue |||  {qDiscounted.ApproximateMessageCount} in DiscountedQueue";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        const string OrderInitialQueueName = "initialorders";
        const string OrderDiscountedQueueName = "discountedorders";
        const string AccountName = "**REPLACE WITH YOUR STORAGE ACCOUNT NAME**";
        const string AccountKey = "**REPLACE WITH YOUR STORAGE ACCOUNT KEY**";

        public CloudStorageAccount GetStorageAccount()
        {
            StorageCredentials credentials = new StorageCredentials(AccountName, AccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, true);
            return storageAccount;
        }

        private  CloudQueue CreateQueueIfNotExists(string queueName)
        {
            var account = GetStorageAccount();
            CloudQueueClient queueClient = account.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            bool created = queue.CreateIfNotExists();
            return queue;
        }
    }
}
