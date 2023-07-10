using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

PrintSkia();

//SkiaSharp
//Is bitmap, with SVG support
void PrintSkia()
{
    try
    {
        //300dpi
        var info = new SKImageInfo(2480, 3508);
        //120dpi
        info = new SKImageInfo(992, 1403);
        using (var surface = SKSurface.Create(info))
        {
            // the the canvas and properties
            var canvas = surface.Canvas;

            // make sure the canvas is blank
            canvas.Clear(SKColors.White);

            // draw some text
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 24,
                Typeface = SKTypeface.FromFile("NotoSerifJP-Light.otf")
            };
            //Background SVG
            canvas.DrawPicture(new SKSvg().Load("BG.svg"), paint);

            //Draw text with coord
            var coord = new SKPoint(info.Width / 2, (info.Height + paint.TextSize) / 2);
            canvas.DrawText("���{�ꂧ����������ABCabc�A�C�E", coord, paint);

            // save the file
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite("output.png"))
            {
                data.SaveTo(stream);
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        throw;
    }
}



//PDFSharp
//Is PDF, No SVG support
void PrintPDF()
{
    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    string filename = "tempfile.pdf";
    var s_document = new PdfDocument();
    s_document.Info.Title = "PDFsharp XGraphic Sample";
    XGraphics gfx = XGraphics.FromPdfPage(s_document.AddPage());
    var pdf_ja_font_options = new XPdfFontOptions(PdfFontEncoding.Unicode);
    var pdf_ja_font = new XFont("������", 12, XFontStyle.Regular, pdf_ja_font_options);
    gfx.DrawString("asdfABC", pdf_ja_font, XBrushes.Black, new XRect(0, 0, s_document.Pages[0].Width, s_document.Pages[0].Height), XStringFormats.Center);
    s_document.Save(filename);
}

//var builder = WebApplication.CreateBuilder(args);
//// Add services to the container.
//builder.Services.AddRazorPages();
//var app = builder.Build();
//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthorization();
//app.MapRazorPages();
//app.Run();