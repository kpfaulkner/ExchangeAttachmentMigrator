//-----------------------------------------------------------------------
// <copyright >
//    Copyright 2013 Ken Faulkner
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
//-----------------------------------------------------------------------
 
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using CommonDataTypes;

namespace AzureManager
{
    public static class AzureHelper
    {

        public static string AzureStorageConnectionString { get; set; }

        const string AzureDetection = "windows.net";
        const string DevAzureDetection = "127.0.0.1";
        static CloudBlobClient BlobClient { get; set; }
        static string accountName;
        static string accountKey;

        static AzureHelper()
        {
            BlobClient = null;
        }


        public static CloudBlobClient GetCloudBlobClient(string accountName, string accountKey)
        {
            if (BlobClient == null)
            {

                var credentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, accountKey);
                CloudStorageAccount azureStorageAccount = new CloudStorageAccount(credentials, true);
                BlobClient = azureStorageAccount.CreateCloudBlobClient();
            }

            return BlobClient;
        }

        public static CloudBlobClient GetCloudBlobClient(string accName, string accKey, bool isDev=false)
        {
            accountName = accName;
            accountKey = accKey;
            
            if (BlobClient == null)
            {
                if (isDev)
                {
                   
                    CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                    BlobClient = storageAccount.CreateCloudBlobClient();
              
                }
                else
                {
                  
                    var credentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, accountKey);
                    CloudStorageAccount azureStorageAccount = new CloudStorageAccount(credentials, true);
                    BlobClient = azureStorageAccount.CreateCloudBlobClient();

                    // retry policy.
                    // could do with a little work.
                    IRetryPolicy linearRetryPolicy = new LinearRetry( TimeSpan.FromSeconds( 5), 10);
                    BlobClient.RetryPolicy = linearRetryPolicy;

                }

            }

            return BlobClient;
        }





        public static string UploadFile(UrlAttachment attachment)
        {
            var container = BlobClient.GetContainerReference("ExchangeAttachments");
            container.CreateIfNotExists();

            var blobName = attachment.Account + ":" + attachment.Name + ":" + attachment.TimeStamp.ToShortTimeString();
            var blobRef = container.GetBlockBlobReference(blobName);

            // do we *REALLY* want to delete?
            blobRef.DeleteIfExists();

            using (var ms = new MemoryStream(attachment.Bytes))
            {
                blobRef.UploadFromStream(ms);
            }

            var url = blobRef.Uri.AbsoluteUri;
            return url;
        }
    }
}

