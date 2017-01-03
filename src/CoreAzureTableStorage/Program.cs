using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CoreAzureTableStorage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tommywk;AccountKey=dir8BNExdrma04J2xJbxh7oogdWEmnURiDVvfN6djVaaU9oP0oi2HL4j6fbNQPIaI87OX81iOT+WiLfw8+rk6w==");
            var tableClient = storageAccount.CreateCloudTableClient();

            //we could replace this with .net's standard configuration manager
            //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //var tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            var table = tableClient.GetTableReference("people");

            //currently .net core does not have synchronous methods

            // Create the table if it doesn't exist.
            var wasCreated = table.CreateIfNotExistsAsync().Result;
            if (wasCreated)
            {
                // Create a new customer entity.
                var customer1 = new CustomerEntity("Harp", "Walter");
                customer1.Email = "Walter@contoso.com";
                customer1.PhoneNumber = "425-555-0101";

                // Create the TableOperation object that inserts the customer entity.
                var insertOperation = TableOperation.Insert(customer1);

                // Execute the insert operation.
                var result = table.ExecuteAsync(insertOperation).Result;
                Console.WriteLine(result);

                // Create the batch operation.
                var batchOperation = new TableBatchOperation();

                // Create a customer entity and add it to the table.
                var customer2 = new CustomerEntity("Smith", "Jeff");
                customer2.Email = "Jeff@contoso.com";
                customer2.PhoneNumber = "425-555-0104";

                // Create another customer entity and add it to the table.
                var customer3 = new CustomerEntity("Smith", "Ben");
                customer3.Email = "Ben@contoso.com";
                customer3.PhoneNumber = "425-555-0102";

                // Add both customer entities to the batch insert operation.
                batchOperation.Insert(customer2);
                batchOperation.Insert(customer3);

                // Execute the batch operation.
                var results = table.ExecuteBatchAsync(batchOperation).Result;
                Console.WriteLine(results);
            }

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));
            var list = table.ExecuteQuerySegmentedAsync(query, null).Result;

            // Print the fields for each customer.
            foreach (var entity in list)
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }

            Console.ReadLine();
        }
    }
}
