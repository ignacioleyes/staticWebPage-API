using AutoMapper;
using conduflex_api.DTOs;
using conduflex_api.Entities;
using conduflex_api.Extensions;
using conduflex_api.Utils;
using iText.IO.Image;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter pw = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(pw);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(75, 35, 70, 35);

                string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "logoConduflex.png");
                Image img = new(ImageDataFactory.Create(imagePath));

                // Set up the header and footer
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderHandler(img));

                doc.Add(new Paragraph($"{product.Name}"));

                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    string[] parts = product.ProductImage.Split(',');
                    if (parts.Length == 2) 
                    {
                        string mimeTypePart = parts[0]; 
                        string imageBase64Part = parts[1];

                        // Check for supported MIME types
                        if (mimeTypePart == "data:image/png;base64" ||
                            mimeTypePart == "data:image/jpeg;base64" ||
                            mimeTypePart == "data:image/jpg;base64")
                        {
                            byte[] imageBytes = Convert.FromBase64String(imageBase64Part);
                            ImageData data = ImageDataFactory.Create(imageBytes);
                            Image image = new(data);
                            doc.Add(image);
                        }
                    }
                }

                doc.Add(new Paragraph("Descripción:"));

                doc.Add(new Paragraph($"{product.Description}"));

                doc.Add(new Paragraph("Características:"));

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

                if (product.CharacteristicsImages.Length > 0)
                {
                    foreach( var characteristicImage in product.CharacteristicsImages)
                    {
                    string[] parts = characteristicImage.Split(',');
                        if (parts.Length == 2) 
                        {
                            string mimeTypePart = parts[0]; 
                            string imageBase64Part = parts[1];

                            // Check for supported MIME types
                            if (mimeTypePart == "data:image/png;base64" ||
                                mimeTypePart == "data:image/jpeg;base64" ||
                                mimeTypePart == "data:image/jpg;base64")
                            {
                                byte[] imageBytes = Convert.FromBase64String(imageBase64Part);
                                ImageData data = ImageDataFactory.Create(imageBytes);
                                Image image = new(data);
                                doc.Add(image);
                            }
                        }

                    }

                }

                doc.Add(new Paragraph("Tabla:"));

                if (!string.IsNullOrEmpty(product.TablesImage))
                {
                    string[] parts = product.TablesImage.Split(',');
                    if (parts.Length == 2)
                    {
                        string mimeTypePart = parts[0];
                        string imageBase64Part = parts[1];

                        // Check for supported MIME types
                        if (mimeTypePart == "data:image/png;base64" ||
                            mimeTypePart == "data:image/jpeg;base64" ||
                            mimeTypePart == "data:image/jpg;base64")
                        {
                            byte[] imageBytes = Convert.FromBase64String(imageBase64Part);
                            ImageData data = ImageDataFactory.Create(imageBytes);
                            Image image = new(data);
                            doc.Add(image);
                        }
                    }
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

    public class HeaderHandler : IEventHandler
    {
        Image Img;
        public HeaderHandler(Image img)
        {
            Img = img;
        }

        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();

            Rectangle rootArea = new Rectangle(35, page.GetPageSize().GetTop() - 70, page.GetPageSize().GetRight() - 70, 50);
            Canvas canvas = new Canvas(page, rootArea);
            canvas
                .Add(getTable(docEvent))
                .Close();

        }

            public Table getTable(Event docEvent)
            {
                float[] cellWidth = { 20f, 80f };
                Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                Style styleCell = new Style()
                    .SetBorder(Border.NO_BORDER);

                Style styleText = new Style()
                    .SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10f);

                Cell cell = new Cell().Add(Img.SetAutoScale(true));

                tableEvent.AddCell(cell
                    .AddStyle(styleCell)
                    .SetTextAlignment(TextAlignment.LEFT));

                return tableEvent;
            }
    }
    
}
