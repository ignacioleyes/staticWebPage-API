using conduflex_api.DTOs;
using conduflex_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static conduflex_api.Utils.Constants;

namespace conduflex_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersServices userServices;

        public UsersController(UsersServices userServices)
        {
            this.userServices = userServices;
        }

        [HttpPost("Create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<AuthResponseDTO>> CreateUser([FromBody] ApplicationUserCreationDTO model)
        {
            return await userServices.CreateUser(model);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<ListResponse<ApplicationUserDTO>>> GetUsers([FromQuery] UsersFilter filter)
        {
            return await userServices.GetUsers(filter);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO model)
        {
            return await userServices.Login(model);
        }
    }
}
