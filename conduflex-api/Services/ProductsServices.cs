using AutoMapper;
using conduflex_api.DTOs;
using conduflex_api.Entities;
using conduflex_api.Extensions;
using conduflex_api.Utils;
using iText.Commons.Actions;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;


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

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter pw = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(pw);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(35, 35, 35, 35);

                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images", "logoConduflex.png");
                iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(imagePath));

                // Set the desired width and height for resizing
                float maxWidth = 200; // Set your desired max width
                float maxHeight = 150; // Set your desired max height

                // Calculate scaling factors
                float widthScale = maxWidth / img.GetImageWidth();
                float heightScale = maxHeight / img.GetImageHeight();
                float scale = Math.Min(widthScale, heightScale);

                // Apply the scaling
                img.ScaleToFit(maxWidth, maxHeight);

                doc.Add(img);

                doc.Add(new Paragraph($"{product.Name}"));

                //if (!string.IsNullOrEmpty(product.ProductImage))
                //{
                //    byte[] imageBytes = Convert.FromBase64String(product.ProductImage);

                //    ImageData data = ImageDataFactory.Create(imageBytes);

                //    Image image = new(data);
                //    doc.Add(image);
                //}

                doc.Add(new Paragraph($"{product.Description}"));

                if (product.Characteristics != null)
                {
                    var characteristics = product.Characteristics
                        .Split('*')
                        .Select(part => part.Trim())
                        .Where(part => !string.IsNullOrEmpty(part))
                        .Select((filteredPart, index) => $"- {filteredPart}");

                    var formattedCharacteristics = string.Join(Environment.NewLine, characteristics);

                    foreach (var characteristic in characteristics)
                    {
                        Console.WriteLine(characteristic);
                    }

                    doc.Add(new Paragraph(formattedCharacteristics));
                }

                doc.Close();

                byte[] byteStream = ms.ToArray();

                return new FileContentResult(byteStream, "application/pdf");
            }
        }

        public async Task<ActionResult> CreateProduct(ProductCreationDTO productCreation)
        {
            var product = mapper.Map<Product>(productCreation);
            context.Add(product);
            await context.SaveChangesAsync();

            return Ok(product);
        }

        public async Task<ActionResult> UpdateProduct(int id, ProductPatchDTO productPatchDTO)
        {
            var product = await context.Products.FirstOrDefaultAsync(r => r.Id == id);
            if (product == null) return NotFound();

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
