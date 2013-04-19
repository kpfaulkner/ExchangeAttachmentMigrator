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
                    FileAttachment fa = message.Attachments[0] as FileAttachment;
                    if (fa != null)
                    {
                        /*
                        fa.Load();
                        message.Attachments.AddItemAttachment<EmailMessage>();
                        message.Attachments.RemoveAt(0);
                        message.Update(ConflictResolutionMode.AlwaysOverwrite);
                         * */
                    }

                }

            }
        }

        public void InspectExchange()
        {

        }

        public void CopyAttachmentToAzure()
        {

        }

        public void RemoveAttachmentFromExchange()
        {

        }
    }
}
