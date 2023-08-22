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
    public class ProductsController : ControllerBase
    {
        private readonly ProductsServices productsServices;

        public ProductsController(ProductsServices productsServices)
        {
            this.productsServices = productsServices;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<ProductDTO>>> GetProducts([FromQuery] BaseFilter filter)
        {
            return await productsServices.GetProducts(filter);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProductById([FromRoute] int id)
        {
            return await productsServices.GetProductById(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> CreateProduct([FromBody] ProductCreationDTO productCreation)
        {
            return await productsServices.CreateProduct(productCreation);
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductPatchDTO productPatchDTO)
        {
            return await productsServices.UpdateProduct(id, productPatchDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
        {
            return await productsServices.DeleteProduct(id);
        }
    }
}
