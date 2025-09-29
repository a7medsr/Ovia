using Ovia.DTO;
using Ovia.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Charting;
using DocumentFormat.OpenXml.Drawing;
using System.Drawing;
using System.Drawing.Imaging;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MomEntity momDb;

        public OrdersController(MomEntity _momDb)
        {
            momDb = _momDb;
        }

        [HttpGet("GetCustomerOrders")]
        public async Task<IActionResult> GetCustomerOrders(int customerId)
        {
            var orders = await momDb.CustomerPackageSelect
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            if (orders.Count != 0)
            {
                List<OrderDTO> OrdersList = new List<OrderDTO>();
                foreach (var order in orders)
                {
                    var package = GetPackageDetails((int)order.PackagesId);
                    if (package != null)
                    {
                        var orderDto = new OrderDTO
                        {
                            OrderId = order.Id,
                            CustomerId = order.CustomerId,
                            PackagesId = order.PackagesId,
                            PackageName = package.Name,
                            Cost = order.Cost,
                            OrignalCost = order.OrignalCost,
                            InvoiceSerial = order.InvoiceSerial,
                            IsCompleted = order.IsCompleted,
                            CreationDate = order.CreationDate
                        };
                        OrdersList.Add(orderDto);

                    }

                }
                return Ok(OrdersList);
            }
            return NotFound("You dont have any orders");

        }


        [HttpGet("GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var order = await momDb.CustomerPackageSelect
                .FirstOrDefaultAsync(c => c.Id == orderId);
            if (order != null)
            {
                var package = GetPackageDetails((int)order.PackagesId);
                if (package != null)
                {
                    var orderDto = new OrderDTO
                    {
                        OrderId = order.Id,
                        CustomerId = order.CustomerId,
                        PackagesId = order.PackagesId,
                        PackageName = package.Name,
                        Cost = order.Cost,
                        OrignalCost = order.OrignalCost,
                        InvoiceSerial = order.InvoiceSerial,
                        IsCompleted = order.IsCompleted,
                        CreationDate = order.CreationDate
                    };
                    return Ok(orderDto);

                }


                return NotFound("This package not found");
            }
            return NotFound("This order not found");

        }


        [HttpGet("GetOrderRecieptAS_PDF")]
        public async Task<IActionResult> GetOrderRecieptAS_PDF(int orderId)
        {
            var order = await (from customerPackage in momDb.CustomerPackageSelect
                               where customerPackage.Id == orderId
                               join custattribute in momDb.CustomerAttributes
                                   on customerPackage.CustomerId equals custattribute.Id
                               join customer in momDb.CustomerInfo
                                   on custattribute.CustomerInfoId equals customer.Id
                               join package in momDb.Packages
                                   on customerPackage.PackagesId equals package.Id
                               select new
                               {
                                   customerPackage,
                                   custattribute,
                                   customer,
                                   package
                               }).FirstOrDefaultAsync();

            if (order != null)
            {
                // Extracting data from the result
                var PackageCost = order.customerPackage.OrignalCost;
                var PackageOriginalCost = order.customerPackage.Cost;
                var InoviceSerial = order.customerPackage.InvoiceSerial;
                var ReferId = order.custattribute.ReferId;
                var customerName = order.customer.NameEn;
                var packageName = order.package.Name;

                // Create PDF document
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 12);

                // Drawing text on PDF
                int x = 40;
                int y = 40;
                gfx.DrawString("Order Details", font, XBrushes.Black, x, y);
                y += 20;
                gfx.DrawString($"Reciept Serial: {InoviceSerial}", font, XBrushes.Black, x, y);
                y += 20;
                gfx.DrawString($"Customer Name: {customerName}", font, XBrushes.Black, x, y);
                y += 20;
                gfx.DrawString($"Back Office Id : {ReferId}", font, XBrushes.Black, x, y);
                y += 20;
                gfx.DrawString($"Package Name: {packageName}", font, XBrushes.Black, x, y);
                y += 20;
                gfx.DrawString($"Package Original Cost: {PackageOriginalCost}", font, XBrushes.Black, x, y);
                y += 20;
                gfx.DrawString($"Package Cost: {PackageCost}", font, XBrushes.Black, x, y);
                y += 20;
                // Add more details as needed...

                // Save PDF to a memory stream
                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                stream.Seek(0, SeekOrigin.Begin);

                // Return the PDF file
                return File(stream, "application/pdf", "receipt.pdf");
            }

            return NotFound("Order not found");
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        private Package GetPackageDetails(int packageId)
        {
            var package = momDb.Packages
                .FirstOrDefault(c => c.Id == packageId);
            if (package != null)
                return package;
            return null;
        }
       
        private void DrawSection(Graphics gfx, string sectionTitle, ref int y, System.Drawing.Font labelFont, System.Drawing.Font valueFont, params (string label, string value)[] values)
    {
        gfx.DrawString(sectionTitle, labelFont, Brushes.Black, 20, y);
        y += 20;

        foreach (var (label, value) in values)
        {
            gfx.DrawString(label, labelFont, Brushes.Black, 20, y);
            gfx.DrawString(value, valueFont, Brushes.Black, 200, y);
            y += 20;
        }
    }





}
}
