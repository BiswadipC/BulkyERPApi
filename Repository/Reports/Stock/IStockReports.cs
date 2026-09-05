using Domain.Reports.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Reports.Stock
{
    public interface IStockReports
    {
        Task<List<StockVsReOrderClass>> GetStockVsReOrderLevel(int? itemId);
    } // interface...
}
