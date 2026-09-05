using Domain.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.PurchaseOrder
{
    public interface IPurchaseOrder
    {
        Task<List<PurchaseOrderHeadResponse>> GetPOHeads();
        Task<PurchaseOrderHeadResponse> GetPOHeadByOrderId(int? orderId);
        Task<List<PurchaseOrderDtlResponse>> GetPODtlsByOrderId(int? orderId);
        Task<List<PurchaseOrderHeadResponse>> GetPOHeadsByPartyCode(int? partyCode);
        Task<PurchaseOrderDtlResponse> GetPODtlByPODtlRecId(int? recId);
        Task<string> Save(PurchaseOrderHeadResponse response);
    } // interface...
}
