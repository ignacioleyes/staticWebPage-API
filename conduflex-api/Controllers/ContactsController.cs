using conduflex_api.DTOs;
using conduflex_api.Services;
using Couchbase;
using Couchbase.Core.Exceptions;
using Couchbase.Core.Exceptions.KeyValue;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static conduflex_api.Utils.Constants;
using static Etcdserverpb.KV;

namespace conduflex_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ContactsServices contactsServices;

        public ContactsController(ContactsServices contactsServices)
        {
            this.contactsServices = contactsServices;
        }

        [HttpPost("test")]
        public async Task<JToken> CouchbaseTest([FromBody] KeyFinderDTO keyFinderDTO)
        {
            // Start of user input
            // Update these variables to point to your Couchbase Capella instance and credentials.
            string endpoint = "couchbases://cb.oggu01wh0igkecjj.cloud.couchbase.com"; // Replace this with Connection String
            string username = "couchbaseDB2"; // Replace this with username from database access credentials
            string password = "Gianmarco12@"; // Replace this with password from database access credentials
            string bucketName = "travel-sample";
            string scopeName = "Archer";
            string collectionName = "InstanceProperties";
            // Sample airline document
            JObject sampleAirline = new JObject
            {
                ["type"] = "AIRLINETEST",
                ["id"] = 1,
                ["callsign"] = "TEST",
                ["iata"] = null,
                ["icao"] = null,
                ["name"] = "TEST"
            };
            // Key will equal: "airline_8091"
            string key = keyFinderDTO.Key;
            // End of user input variables

            try
            {
                // Connect to cluster with specified credentials
                var options = new ClusterOptions()
                    .WithConnectionString(endpoint)
                    .WithCredentials(username, password);

                using var cluster = await Couchbase.Cluster.ConnectAsync(options);
                await cluster.WaitUntilReadyAsync(TimeSpan.FromSeconds(30));

                var bucket = await cluster.BucketAsync(bucketName);
                var scope = bucket.Scope(scopeName);
                var collection = scope.Collection(collectionName);

                // Simple K-V operation - to retrieve a document by ID
                try
                {
                    var getResult = await collection.GetAsync(key);
                    var resultContent = getResult.ContentAs<JObject>();

                    
                    if (resultContent["instanceName"].ToString() == keyFinderDTO.InstanceName &&
                        JArray.DeepEquals(resultContent["tags"], JArray.FromObject(keyFinderDTO.Tags)))
                    {
                        Console.WriteLine("Document fetched successfully");
                        return resultContent["enableFieldEncryption"];
                    }
                    else
                    {
                        Console.WriteLine("Conditions not met for the document");
                    }
                }
                catch (DocumentNotFoundException)
                {
                    Console.WriteLine("Document not found!");
                }
            }

            catch (AmbiguousTimeoutException ex)
            {
                // Simplest approach is to look at the exception string
                bool authFailure = ex.ToString().Contains("Authentication Failure");
                if (authFailure)
                {
                    Console.WriteLine("Authentication Failure Detected");
                }
                else
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(ex.Message);
                }
            }

            return sampleAirline;
        }

        [HttpGet("etcdGET")]
        public async Task<ActionResult<PutResponse>> GetKV()
        {
            //Name of the key to look for
            var key = "keyName";

            //Create ETCD connection
            var etcdClient = new EtcdClient("localhost:2379");

            //Create de GET request
            var getRequest = new RangeRequest
            {
                Key = ByteString.CopyFromUtf8(key), //Key Name
                RangeEnd = ByteString.Empty, //Range if needed
            };

            //Do the GET request and take the response
            var response = await etcdClient.GetAsync(getRequest);

            //If there is one or more keys in the response, return the following text
            if (response.Count > 0)
            {
                var keyValue = response.Kvs[0].Value.ToStringUtf8();
                return Ok("Key name: " + key + " with value: " + keyValue + " was found. " + " And this is the expected response: " + response);
            }
            else
            {
                return NotFound($"Key '{key}' not found.");
            }
        }

        [HttpGet("etcdPOST")]
        public async Task<ActionResult<PutResponse>> CreateKV()
        {
            var key = "keyName";
            var keyValue = "keyValue";

            var etcdClient = new EtcdClient("localhost:2379");

            var request = new PutRequest
            {
                Key = ByteString.CopyFromUtf8(key),
                Value = ByteString.CopyFromUtf8(keyValue),
            };

            var response = await etcdClient.PutAsync(request);

            Console.WriteLine($"Key '{key}' created with value '{keyValue}' successfully.");

            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<ListResponse<ContactDTO>>> GetContacts([FromQuery] BaseFilter filter)
        {
            return await contactsServices.GetContacts(filter);
        }

        [HttpGet("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<ContactDTO>> GetContact([FromRoute] int id)
        {
            return await contactsServices.GetContactById(id);
        }

        [HttpPost]
        public async Task<ActionResult> CreateContact([FromBody] ContactCreationDTO contactCreation)
        {
            return await contactsServices.CreateContact(contactCreation);
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> DeleteContact ([FromRoute] int id)
        {
            return await contactsServices.DeleteContact(id);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> DeleteAllContacts()
        {
            return await contactsServices.DeleteAllContacts();
        }
    }
}
