using Domain.ItemMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ItemMaster
{
    public interface IItemMasterResponse
    {
        Task<List<ItemHeadResponse>> GetItemHeads();
        Task<ItemHeadResponse> GetItemHeadByItemId(int itemId);
        Task<ItemGSTResponse> GetItemGSTByItemId(int itemId);
        Task<string> Save(ItemHeadResponse head);
    } // interface...
}
