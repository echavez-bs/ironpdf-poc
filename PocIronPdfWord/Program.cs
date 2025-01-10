var builder = WebApplication.CreateBuilder(args);

IronPdf.License.LicenseKey = builder.Configuration["IronPdf:LicenseKey"];
IronPdf.Installation.ChromeGpuMode = IronPdf.Engines.Chrome.ChromeGpuModes.Disabled;
IronPdf.Installation.LinuxAndDockerDependenciesAutoConfig = true;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/html-to-pdf-base64", async (IWebHostEnvironment env) =>
    {
        var htmlPath = Path.Combine(env.ContentRootPath, "Resources", "documento.html");
        var imagePath = Path.Combine(env.ContentRootPath, "Resources", "logo.png");

        var html = await File.ReadAllTextAsync(htmlPath);

        byte[] oxxoImageBinaryData = File.ReadAllBytes(imagePath);
        string imgDataUri = @"data:image/png;base64," + Convert.ToBase64String(oxxoImageBinaryData);
        string imgHtml = $"<img src='{imgDataUri}'>";

        var header = new HtmlHeaderFooter()
        {
            HtmlFragment = imgHtml
        };

        var footer = new HtmlHeaderFooter()
        {
            HtmlFragment = @"
<div style=""text-align:center"">
<div>OXXO | Uso interno</div>
<div>CONTRATO DE ARRENDAMIENTO (OXXO – Nombre de la tienda – nombre región)</div>
<div>{page}</div>
</div>"
        };

        // Instantiates Chrome Renderer
        var renderer = new ChromePdfRenderer()
        {
            RenderingOptions =
            {
                MarginTop = 25D,
                MarginBottom = 25D,
                MarginLeft = 0D,
                MarginRight = 0D
            }
        };

        using var pdf = renderer.RenderHtmlAsPdf(html);

        var pdfBytes = pdf.BinaryData;

        pdf.AddHtmlHeaders(header);
        pdf.AddHtmlFooters(footer);

        return Convert.ToBase64String(pdfBytes);
    })
    .WithName("HtmlToPdfBase64");

app.MapGet("/html-to-pdf", async (IWebHostEnvironment env) =>
    {
        var htmlPath = Path.Combine(env.ContentRootPath, "Resources", "documento.html");
        var imagePath = Path.Combine(env.ContentRootPath, "Resources", "logo.png");

        var html = await File.ReadAllTextAsync(htmlPath);

        byte[] oxxoImageBinaryData = File.ReadAllBytes(imagePath);
        string imgDataUri = @"data:image/png;base64," + Convert.ToBase64String(oxxoImageBinaryData);
        string imgHtml = $"<img src='{imgDataUri}'>";

        var header = new HtmlHeaderFooter()
        {
            HtmlFragment = imgHtml
        };

        var footer = new HtmlHeaderFooter()
        {
            HtmlFragment = @"
<div style=""text-align:center"">
<div>OXXO | Uso interno</div>
<div>CONTRATO DE ARRENDAMIENTO (OXXO – Nombre de la tienda – nombre región)</div>
<div>{page}</div>
</div>"
        };

        // Instantiates Chrome Renderer
        var renderer = new ChromePdfRenderer()
        {
            RenderingOptions =
            {
                MarginTop = 25D,
                MarginBottom = 25D,
                MarginLeft = 0D,
                MarginRight = 0D
            }
        };

        using var pdf = renderer.RenderHtmlAsPdf(html);

        pdf.AddHtmlHeaders(header);
        pdf.AddHtmlFooters(footer);

        var pdfBytes = pdf.BinaryData;

        return Results.File(pdfBytes, "application/pdf", $"file.pdf");
    })
    .WithName("HtmlToPdf");

app.Run();