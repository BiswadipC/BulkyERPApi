using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports.Stock;

namespace WebApiMain.Controllers
{
    [Route("stock")]
    [ApiController]

    public class StockreportsController : ControllerBase
    {
        private readonly IStockReports istock;

        public StockreportsController(IStockReports istock)
        {
            this.istock = istock;
        } // constructor...

        [HttpGet("GetStockVsReOrderLevelDownload/{itemId?}")]
        public async Task<IActionResult> GetStockVsReOrderLevelDownload(int? itemId = null)
        {
            var stocks = await istock.GetStockVsReOrderLevel(itemId);

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Stock Report");

            // Header
            worksheet.Cell(1, 1).Value = "Item Id";
            worksheet.Cell(1, 2).Value = "Item Name";
            worksheet.Cell(1, 3).Value = "Stock Qty";
            worksheet.Cell(1, 4).Value = "Re Order Level";
            worksheet.Cell(1, 5).Value = "Difference";
            worksheet.Cell(1, 6).Value = "MRP";

            // Data
            int row = 2;

            foreach (var item in stocks)
            {
                worksheet.Cell(row, 1).Value = item.ItemId;
                worksheet.Cell(row, 2).Value = item.ItemName;
                worksheet.Cell(row, 3).Value = item.StockQty;
                worksheet.Cell(row, 4).Value = item.ReOrderLevel;
                worksheet.Cell(row, 5).Value = item.Diff;
                worksheet.Cell(row, 6).Value = item.MRP;

                row++;
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"StockVsReorderLevel_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        } // GetStockVsReOrderLevelDownload...

        [HttpGet("GetStockVsReOrderLevelData/{itemId?}")]
        public async Task<IActionResult> GetStockVsReOrderLevelData(int? itemId = null)
        {
            var stocks = await istock.GetStockVsReOrderLevel(itemId);
            return Ok(stocks);
        } // GetStockVsReOrderLevelData...
    } // class...
}
