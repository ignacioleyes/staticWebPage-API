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
    public class ProductsServices : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IActionContextAccessor actionContextAccessor;

        public ProductsServices(ApplicationDbContext context, IMapper mapper, IActionContextAccessor actionContextAccessor)
        {
            this.context = context;
            this.mapper = mapper;
            this.actionContextAccessor = actionContextAccessor;
        }

        public async Task<ActionResult<ListResponse<ProductDTO>>> GetProducts(BaseFilter filter)
        {
            var queryable = context.Products.AsQueryable();
            return await queryable.FilterSortPaginate<Product, ProductDTO>(filter, mapper, actionContextAccessor);
        }

        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(r => r.Id == id);
            if (product == null) return NotFound("No se encontró el producto");

            return mapper.Map<ProductDTO>(product);
        }

        public async Task<ActionResult> CreateProduct(ProductCreationDTO productCreation)
        {
            var product = mapper.Map<Product>(productCreation);
            context.Add(product);
            await context.SaveChangesAsync();

            return Ok(product);
        }

        public async Task<ActionResult> UpdateProduct(int id, JsonPatchDocument<ProductPatchDTO> patchDocument)
        {
            var product = await context.Products.FirstOrDefaultAsync(r => r.Id == id);
            if (product == null) return NotFound();

            var productPatchDTO = mapper.Map<ProductPatchDTO>(product);
            patchDocument.ApplyTo(productPatchDTO, ModelState);
            mapper.Map(productPatchDTO, product);

            await context.SaveChangesAsync();
            return NoContent();
        }

        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(r => r.Id == id);
            if (product == null) return NotFound("Product not found");

            context.Remove(product);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
