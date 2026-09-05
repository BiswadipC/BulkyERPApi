using Domain.PurchaseBill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.PurchaseBill
{
    public interface IPurchaseBill
    {
        Task<List<PurchaseBillHeadResponse>> GetPurchaseBills();
        Task<List<PurchaseBillDtlResponse>> GetPurchaseBillDtlsByBillId(int  billId);
        Task<PurchaseBillHeadResponse> GetPurchaseBillHeadByBillId(int billId);
        Task<List<PurchaseBillDtlResponse>> PopulatePBByOrderId(int[] orderIds);
        Task<PurchaseBillDtlResponse> GetPBDtlByPBDtlRecid(int recId);
        Task<string> SaveBill(PurchaseBillHeadResponse response);
    } // interface...
}
