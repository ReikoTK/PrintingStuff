using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SkiaSharp;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;


internal class Program
{
	private static void Main(string[] args)
	{
		PrintSkia();

		//SkiaSharp
		//Is bitmap, with SVG support
		void PrintSkia()
		{
			try
			{
				//300dpi
				var info = new SKImageInfo(2480, 3508);
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

					//Draw text with coord
					var coord = new SKPoint(info.Width / 2, (info.Height + paint.TextSize) / 2);
					canvas.DrawPicture(new SKSvg().Load("BG.svg"), paint);

					PrintInvoice(canvas,info);

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

		void PrintInvoice(SKCanvas canvas, SKImageInfo info)
		{
			InvoiceItem? item;

			try {
				string text = File.ReadAllText("sample.json");
				if(text != null)
				{
					item = JsonSerializer.Deserialize<InvoiceItem>(text);
				}
				else
				{
					throw new Exception("Invoice Read Text Error");
				}
			} catch(Exception e)
			{
				Console.WriteLine($"Error: {e.Message}");
				throw;
			}

			if ( item != null )
			{
				var paintLeft = new SKPaint
				{
					Color = SKColors.RoyalBlue,
					IsAntialias = true,
					Style = SKPaintStyle.Fill,
					TextAlign = SKTextAlign.Left,
					TextSize = 34,
					Typeface = SKTypeface.FromFile("NotoSerifJP-Light.otf")
				};
				var paintRight = new SKPaint
				{
					Color = SKColors.RoyalBlue,
					IsAntialias = true,
					Style = SKPaintStyle.Fill,
					TextAlign = SKTextAlign.Right,
					TextSize = 34,
					Typeface = SKTypeface.FromFile("NotoSerifJP-Light.otf")
				};
				for (int r = 0; r < 2; r++)
				{
					var VOffset = (info.Height-5) / 2 * r;
					canvas.DrawText(item.CustomerNumber, new SKPoint(465, 235+VOffset), paintLeft);
					canvas.DrawText(item.TicketNumber, new SKPoint(2365, 211+VOffset), paintRight);
					var time = DateTime.Now;
					canvas.DrawText(time.Year.ToString(), new SKPoint(1615, 262+VOffset), paintRight);
					canvas.DrawText(time.Month.ToString(), new SKPoint(1765, 262+VOffset), paintRight);
					canvas.DrawText(time.Day.ToString(), new SKPoint(1920, 262+VOffset), paintRight);
					var ADrow1 = item.Address.Split(' ')[0];
					var ADrow2 = item.Address.Split(' ')[1];
					canvas.DrawText(ADrow1, new SKPoint(364, 317+VOffset), paintLeft);
					canvas.DrawText(ADrow2, new SKPoint(364, 362+VOffset), paintLeft);
					canvas.DrawText(item.CustomerName, new SKPoint(364, 476+VOffset), paintLeft);
					canvas.DrawText(item.Telephone, new SKPoint(436, 584+VOffset), paintLeft);
					canvas.DrawText(item.FAX, new SKPoint(909, 584+VOffset), paintLeft);
					canvas.DrawText(item.Manager, new SKPoint(1666, 584+VOffset), paintLeft);
					float ProductStart = 737;
					float RowDistance = 96;
					float LineDistance = 40;
					for (int i = 0; i < item.Products.Length; i++)
					{
						float row1 = ProductStart + (RowDistance * i);
						float row2 = row1 + LineDistance;
						Product thisitem = item.Products[i];
						canvas.DrawText(thisitem.Code, new SKPoint(207, row1+VOffset), paintLeft);
						canvas.DrawText(thisitem.ProductName, new SKPoint(207, row2+VOffset), paintLeft);
						canvas.DrawText(thisitem.Count.ToString(), new SKPoint(1339, row2+VOffset), paintRight);
						canvas.DrawText(thisitem.Unit, new SKPoint(1414, row2+VOffset), paintLeft);
						canvas.DrawText(thisitem.PricePer.ToString(), new SKPoint(1693, row2+VOffset), paintRight);
						canvas.DrawText(thisitem.TotalPrice.ToString(), new SKPoint(2038, row2+VOffset), paintRight);
						if (thisitem.Addition.Length >= 12)
						{
							canvas.DrawText(thisitem.Addition[..12], new SKPoint(2120, row1+VOffset), paintLeft);
							canvas.DrawText(thisitem.Addition[^12..], new SKPoint(2120, row2+VOffset), paintLeft);
						}
					}
					canvas.DrawText(item.NonTaxTotal.ToString(), new SKPoint(867, 1607+VOffset), paintRight);
					canvas.DrawText(item.Tax.ToString(), new SKPoint(1427, 1607+VOffset), paintRight);
					canvas.DrawText(item.Total.ToString(), new SKPoint(2080, 1627+VOffset), paintRight);
					canvas.DrawText(item.Note, new SKPoint(384, 1645+VOffset), paintLeft);
				}
			}
		}
	}
}

public class InvoiceItem
{
	public string CustomerNumber { get; set; }
	public string TicketNumber { get; set; }
	public string Date { get; set; }
	public string Address { get; set; }
	public string CustomerName { get; set; }
	public string Telephone { get; set; }
	public string FAX { get; set; }
	public string Manager { get; set; }
	public Product[] Products { get; set; }
	public float NonTaxTotal { get; set; }
	public float Tax { get; set; }
	public float Total { get; set; }
	public string Note { get; set; }
	public InvoiceItem(
		string customerNumber, string ticketNumber, 
		string date,string address,string customerName,
		string telephone,string fax,string manager, Product[] products,
		float nonTaxTotal,float tax,float total,string note) {
		CustomerNumber = customerNumber;
		TicketNumber = ticketNumber;
		Date = date;
		Address = address;
		CustomerName = customerName;
		Telephone = telephone;
		FAX = fax;
		Manager = manager;
		Products = products;
		NonTaxTotal = nonTaxTotal;
		Tax = tax;
		Total = total;
		Note = note;
	}
}

public class Product
{
	public string Code { get; set; }
	public string ProductName { get; set; }
	public int Count { get; set; }
	public string Unit { get; set; }
	public float PricePer { get; set; }
	public float TotalPrice { get; set; }
	public string Addition { get; set; }

	public Product(
		string code,string productName,int count,
		string unit,float pricePer,float totalPrice,string addition) { 
		Code = code;
		ProductName = productName;
		Count = count;
		Unit = unit;
		PricePer = pricePer;
		TotalPrice = totalPrice;
		Addition = addition;
	}
}
