using AutoMapper;
using conduflex_api.DTOs;
using conduflex_api.Entities;
using conduflex_api.Extensions;
using conduflex_api.Utils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace conduflex_api.Services
{
    public class ContactsServices : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IActionContextAccessor actionContextAccessor;

        public ContactsServices(ApplicationDbContext context, IMapper mapper, IActionContextAccessor actionContextAccessor)
        {
            this.context = context;
            this.mapper = mapper;
            this.actionContextAccessor = actionContextAccessor;
        }

        public async Task<ActionResult<ListResponse<ContactDTO>>> GetContacts(BaseFilter filter)
        {
            var queryable = context.Contacts.AsQueryable();
            return await queryable.FilterSortPaginate<Contact, ContactDTO>(filter, mapper, actionContextAccessor);
        }

        public async Task<ActionResult<ContactDTO>> GetContactById(int id)
        {
            var contact = await context.Contacts.FirstOrDefaultAsync(r => r.Id == id);
            if (contact == null) return NotFound();

            return mapper.Map<ContactDTO>(contact);
        }

        public async Task<ActionResult> CreateContact(ContactCreationDTO contactCreation)
        {
            var contact = mapper.Map<Contact>(contactCreation);
            context.Add(contact);
            await context.SaveChangesAsync();

            return Ok(contact);
        }

        public async Task<ActionResult> DeleteContact(int id)
        {
            var contact = await context.Contacts.FirstOrDefaultAsync(r => r.Id == id);
            if (contact == null) return NotFound("Contact not found");

            context.Remove(contact);
            await context.SaveChangesAsync();
            return NoContent();
        }
        public async Task<ActionResult> DeleteAllContacts()
        {
            var contacts = await context.Contacts.ToListAsync();
            if (contacts == null) return NotFound("There is no Contacts in database");

            context.RemoveRange(contacts);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
