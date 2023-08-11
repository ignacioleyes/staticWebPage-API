using AutoMapper;
using conduflex_api.DTOs;
using conduflex_api.Utils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace conduflex_api.Services
{
    public class HomeServices : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public HomeServices(ApplicationDbContext context, IMapper mapper, IActionContextAccessor actionContextAccessor)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ActionResult<HomeDTO>> GetHomeInfo()
        {
            var homeInfo = await context.Home.FirstOrDefaultAsync();
            if (homeInfo == null) return NotFound("No se encontro informacion de la Home");

            return mapper.Map<HomeDTO>(homeInfo);
        }

        //public async Task<ActionResult> CreateHomeInfo(HomeCreationDTO homeCreation)
        //{
        //    var homeDBInfo = await context.Home.FirstOrDefaultAsync();

        //    if (homeDBInfo != null) return BadRequest("Ya existe informacion de la Home. Edite la existente en lugar de crear una nueva");

        //    var homeInfo = mapper.Map<Home>(homeCreation);
        //    context.Add(homeInfo);
        //    await context.SaveChangesAsync();

        //    return Ok(homeInfo);
        //}

        public async Task<ActionResult> PatchHomeInfo(int id, JsonPatchDocument<HomePatchDTO> patchDocument)
        {
            var homeInfo = await context.Home.FirstOrDefaultAsync(r => r.Id == id);
            if (homeInfo == null) return NotFound();

            var homePatchDTO = mapper.Map<HomePatchDTO>(homeInfo);
            patchDocument.ApplyTo(homePatchDTO, ModelState);
            mapper.Map(homePatchDTO, homeInfo);

            await context.SaveChangesAsync();
            return NoContent();
        }

        //public async Task<ActionResult> DeleteHomeInfo()
        //{
        //    var homeInfo = await context.Home.FirstOrDefaultAsync();
        //    if (homeInfo == null) return NotFound("No se encontró info en la Home");

        //    context.Remove(homeInfo);
        //    await context.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}
