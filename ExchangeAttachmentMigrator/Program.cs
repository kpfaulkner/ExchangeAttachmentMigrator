using ExchangeMigratorClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeAttachmentMigrator
{
    class Program
    {

        static void Main(string[] args)
        {
            var client = new EMClient( ConfigHelper.ExchangeUrl, ConfigHelper.Domain, ConfigHelper.Account, ConfigHelper.Password);

            client.SearchEmailOfAccount("");
        }
    }
}
