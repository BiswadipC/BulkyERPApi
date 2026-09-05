using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ItemAttributes
{
    public class ItemAttrHeadResponse
    {
        public int IdNo { get; set; }
        public string? AttrName {get; set; } = string.Empty;
        public List<ItemAttrDtlresponse> ListAttrDtls {  get; set; } = new List<ItemAttrDtlresponse>();
    } // class...

    public class ItemAttrDtlresponse
    {
        public int IdNo { get; set; }
        public string? AttrValue { get; set; } = string.Empty;
    } // class...
}
