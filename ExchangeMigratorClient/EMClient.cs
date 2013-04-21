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

using CommonDataTypes;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeMigratorClient
{
    public class EMClient
    {
        EWSManager ewsManager;

        public EMClient(string url)
        {
            ewsManager = new EWSManager(url);
            ewsManager.Connect();
        }

        public EMClient(string url, string domain, string account, string password)
        {
            ewsManager = new EWSManager(url);
            ewsManager.Connect(domain, account, password);
        }



        public void SearchEmailOfAccount(string emailAddr)
        {
            //ewsManager.ImpersonateEmailAccount( emailAddr);

            // Get emails in batches
            ItemView itemView = new ItemView(50);

            // only get new emails...  need to remove this filter.
            SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));

            // just want unread items.
            var items =  ewsManager.Service.FindItems(WellKnownFolderName.Inbox, sf, itemView);

            foreach (var item in items)
            {
                EmailMessage message = EmailMessage.Bind( ewsManager.Service, item.Id, new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Attachments));

                if (message.HasAttachments)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        FileAttachment fa = attachment as FileAttachment;
                        if (fa != null)
                        {
                            fa.Load();
                            var url = CopyAttachmentToAzure(fa);
                            DeleteAttachmentException(message, fa);
                            var newAttachment = CreateAttachment(url, fa.FileName);
                            message.Attachments.AddFileAttachment(newAttachment.Name, newAttachment.Bytes);
                            message.Update(ConflictResolutionMode.AlwaysOverwrite);

                        }
                    }
                }

            }
        }


        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        // url is cloud url.
        public UrlAttachment CreateAttachment(string url, string name)
        {
            var html = "<html><a href=\""+ url + "\">Attachment: "+name+"</a></html>";
            var attachment = new UrlAttachment();
            attachment.Name = name;
            attachment.Bytes = GetBytes( html);
            return attachment;
        }

        public string CopyAttachmentToAzure(FileAttachment fa)
        {
            throw new NotImplementedException();
        }

        public void DeleteAttachmentException(EmailMessage message, FileAttachment fa)
        {
            message.Attachments.Remove( fa );
        }
    }
}
