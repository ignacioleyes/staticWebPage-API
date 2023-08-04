using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using conduflex_api.Utils;
using static conduflex_api.Utils.Constants;
using conduflex_api.Entities;
using conduflex_api.DTOs;
using conduflex_api.Extensions;

namespace conduflex_api.Services
{
    public class UsersServices : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration configuration;
        private readonly IActionContextAccessor actionContextAccessor;

        public UsersServices(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IActionContextAccessor actionContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.actionContextAccessor = actionContextAccessor;
        }

        public async Task<ActionResult<AuthResponseDTO>> CreateUser(ApplicationUserCreationDTO dto)
        {
            var user = mapper.Map<ApplicationUser>(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {     
                return await BuildToken(user);
            }
            
            return BadRequest(result.Errors);
        }

        public async Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal principal)
        {
            var id = principal.Claims.FirstOrDefault(c => c.Type == CustomClaims.ID_CLAIM_TYPE)?.Value;
            if (id == null) throw new ArgumentNullException(nameof(id));
            var user = await context.ApplicationUsers.FirstOrDefaultAsync(au => au.Id == id);
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user;
        }

        public async Task<ActionResult<ListResponse<ApplicationUserDTO>>> GetUsers(UsersFilter filter)
        {
            var queryable = context.ApplicationUsers.AsQueryable();
            if (!string.IsNullOrEmpty(filter.ClientsGeneralSearch))
            {
                queryable = queryable
                    .Where(q =>
                        q.FullName.Contains(filter.ClientsGeneralSearch) ||
                        q.Email.Contains(filter.ClientsGeneralSearch));
            }
            return await queryable.FilterSortPaginate<ApplicationUser, ApplicationUserDTO>(filter, mapper, actionContextAccessor);
        }

        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null) return BadRequest(BuildErrorResponse("EMAIL_NOT_REGISTERED", "Email not registered"));

            var result = await _signInManager.CheckPasswordSignInAsync(user,
                dto.Password, false);

            if (result.Succeeded) return await BuildToken(user);
            
            return BadRequest(BuildErrorResponse("PASSWORD_INVALID", "Invalid password"));
        }

        private async Task<AuthResponseDTO> BuildToken(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new (CustomClaims.ID_CLAIM_TYPE, user.Id),
                new (CustomClaims.EMAIL_CLAIM_TYPE, user.Email)
            };

            if(user.UserType == UserTypeEnum.Admin)
            {
                claims.Add(new Claim(ClaimTypes.Role, Roles.ADMIN));
            }

            var claimsDB = await _userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("CONDUFLEX_JWT_KEY")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var validDays = 7;
            var expiration = DateTime.UtcNow.AddDays(validDays);

            var authToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration.ToLocalTime(), signingCredentials: credentials);

            var response = new AuthResponseDTO
            {
                AuthToken =
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(authToken),
                    ExpiresIn = (int)(expiration - DateTime.UtcNow).TotalMinutes
                },
                TokenType = "Bearer",
                AuthState = mapper.Map<ApplicationUserDTO>(user)
            };
            return response;
        }
        
        private AuthResponseDTO BuildErrorResponse(string errorCode, string errorDescription)
        {
            return new AuthResponseDTO
            {
                Error = new ApiError
                {
                    Code = errorCode,
                    Description = errorDescription
                }
            };
        }
    }
}
