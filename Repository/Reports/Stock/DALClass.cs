using Dapper;
using Domain.ItemMaster;
using Domain.Reports.Stock;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Reports.Stock
{
    namespace NStock
    {
        internal sealed class DALClass : IStockReports
        {
            private readonly IDbConnection db;
            private readonly IMemoryCache cache;

            public DALClass(IDbConnection db, IMemoryCache cache)
            {
                this.db = db;
                this.cache = cache;
            } // constructor...

            public async Task<List<StockVsReOrderClass>> GetStockVsReOrderLevel(int? itemId)
            {
                string key = "GetStockVsReOrderLevel_Key";

                if (!cache.TryGetValue(key, out List<StockVsReOrderClass>? stocks))
                {
                    string str = @"select i.ItemId, i.ItemName, 0 StockQty, i.ReOrderLevel, (i.ReOrderLevel - 0) Diff, SRate MRP
                                  from ItemHead i
                                 where i.ItemId not in (select ItemId from StockDtl where ItemId = i.ItemId)
                                   and (
		                                    @id = 0
			                                    or
		                                    (@id <> 0 and i.ItemId = @id)
	                                    ) 
                                union all
                                select i.ItemId, i.ItemName, sum(Inqty) - sum(OutQty), i.ReOrderLevel, 
                                        (i.ReOrderLevel - (sum(Inqty) - sum(OutQty))) Diff, i.SRate
                                  from ItemHead i inner join StockDtl d on (d.ItemId = i.ItemId)
                                    where (
		                                    @id = 0
			                                    or
		                                    (@id <> 0 and i.ItemId = @id)
	                                    ) 
                                  group by i.ItemId, i.ItemName, i.ReOrderLevel, i.SRate
                                  having sum(Inqty) - sum(OutQty) <= i.ReOrderLevel";

                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@id", itemId);

                    stocks = (await db.QueryAsync<StockVsReOrderClass>(str, dp)).ToList();

                    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                    options.SlidingExpiration = TimeSpan.FromMinutes(10);
                    cache.Set(key, stocks, options);
                } // end if...

                return stocks ?? new List<StockVsReOrderClass>();
            } // GetStockVsReOrderLevel...
        } // class...
    } // namespace NStock...
}
