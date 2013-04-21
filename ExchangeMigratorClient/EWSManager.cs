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

using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeMigratorClient
{
    class EWSManager
    {
        public ExchangeService Service { get; set; }

        string exchangeUrl;

        public EWSManager(string url)
        {
            exchangeUrl = url;
        }


        public void Connect( )
        {

            // connect to the Exchange server.
            //service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);

            Service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);


            // Use the AutodiscoverUrl method to locate the service endpoint.
            try
            {
                Service.UseDefaultCredentials = true;

                // stick to something 6I know of at least. :)
                Service.Url = new Uri( exchangeUrl);

            }
            catch (AutodiscoverRemoteException ex)
            {

                // log a screw up.
            }
        }

        public void Connect( string domain, string username, string password)
        {
            // check version.
            Service = new ExchangeService(ExchangeVersion.Exchange2010);
            Service.Credentials = new WebCredentials(username, password, domain);

            try
            {

                Service.Url = new Uri(exchangeUrl);
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidationCallback);

            }
            catch (AutodiscoverRemoteException ex)
            {
                // log properly.
                Console.WriteLine("Exception thrown: " + ex.Error.Message);
            }

        }

        private static bool OnValidationCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public void ImpersonateEmailAccount(string email)
        {
            // impersonate the email address passed.
            Service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, email);

        }


    }
}
