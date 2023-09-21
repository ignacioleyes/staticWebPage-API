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

        [HttpGet("test")]
        public async Task<JObject> CouchbaseTest()
        {
            string endpoint = "couchbases://cb.0myvjtyjyia7zdmf.cloud.couchbase.com";
            string username = "ArcherCouchbase";
            string password = "Tonic3Archer#";
            string bucketName = "ConfigService";
            string scopeName = "public";
            string collectionName = "property";
            string key = "ArcherWebProperties";

            var options = new ClusterOptions()
              .WithConnectionString(endpoint)
              .WithCredentials(username, password);

            var cluster = await Couchbase.Cluster.ConnectAsync(options);
            var bucket = await cluster.BucketAsync(bucketName);
            var scope = bucket.Scope(scopeName);
            var collection = scope.Collection(collectionName);

            var getResult = await collection.GetAsync(key);
            var resultContent = getResult.ContentAs<JObject>();

            string authURL = resultContent["AuthenticationUrl"].ToString();
            string baseURL = resultContent["BaseUrl"].ToString();

            return resultContent;
        }

        [HttpGet("etcdGET")]
        public ActionResult<PutResponse> GetKV()
        {
            //Name of the key to look for
            var key = "keyName";

            //Create ETCD connection
            var etcdClient = new EtcdClient("https://ec2-54-213-22-124.us-west-2.compute.amazonaws.com:2379");

            var authentication = new AuthenticateRequest()
            {
                Name = "root",
                Password = "Ho!bjaCAbh52"
            };

            var authResponse = etcdClient.Authenticate(authentication);

            //Create de GET request
            var getRequest = new RangeRequest
            {
                Key = ByteString.CopyFromUtf8(key), //Key Name
                RangeEnd = ByteString.Empty, //Range if needed
            };

            //Do the GET request and take the response
            var response = etcdClient.Get(getRequest, new Grpc.Core.Metadata()
            {
                new Grpc.Core.Metadata.Entry("token", authResponse.Token)
            });

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
            var key = "Archer/BaseURL";
            var keyValue = "http://localhost/Archer";

            var etcdClient = new EtcdClient("https://ec2-54-213-22-124.us-west-2.compute.amazonaws.com:2379");

            var authentication = new AuthenticateRequest()
            {
                Name = "root",
                Password = "Ho!bjaCAbh52"
            };

            var authResponse = etcdClient.Authenticate(authentication);
            
            Console.WriteLine(authResponse);

            var request = new PutRequest
            {
                Key = ByteString.CopyFromUtf8(key),
                Value = ByteString.CopyFromUtf8(keyValue),
            };

            var response = await etcdClient.PutAsync(request, new Grpc.Core.Metadata()
            {
                new Grpc.Core.Metadata.Entry("token", authResponse.Token)
            });

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
