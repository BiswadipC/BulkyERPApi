using Domain.ItemAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ItemAttributes
{
    public interface IItemAttributeResponse
    {
        Task<List<ItemAttrHeadResponse>> GetAllAttributes();
        Task<ItemAttrHeadResponse> GetAttributeById(int? id);
        Task<List<ItemAttrDtlresponse>> GetAttrDtlsByAttrHeadId(int headId);
        Task<string> Save(ItemAttrHeadResponse head);
    } // interface...
}
