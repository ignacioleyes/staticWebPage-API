using conduflex_api.DTOs;
using conduflex_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using static conduflex_api.Utils.Constants;

namespace conduflex_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly HomeServices homeServices;

        public HomeController(HomeServices homeServices)
        {
            this.homeServices = homeServices;
        }

        [HttpGet]
        public async Task<ActionResult<HomeDTO>> GetHomeInfo()
        {
            return await homeServices.GetHomeInfo();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> CreateHomeInfo(HomeCreationDTO homeCreation)
        {
            return await homeServices.CreateHomeInfo(homeCreation);
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> PatchHomeInfo([FromRoute] int id, JsonPatchDocument<HomePatchDTO> patchDocument)
        {
            return await homeServices.PatchHomeInfo(id, patchDocument);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> DeleteHomeInfo()
        {
            return await homeServices.DeleteHomeInfo();
        }
    }
}
