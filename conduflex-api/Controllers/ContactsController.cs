using conduflex_api.DTOs;
using conduflex_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static conduflex_api.Utils.Constants;

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
        public async Task<ActionResult> CreateContact(ContactCreationDTO contactCreation)
        {
            return await contactsServices.CreateContact(contactCreation);
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> DeleteContact (int id)
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
