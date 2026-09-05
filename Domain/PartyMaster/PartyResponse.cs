using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.PartyMaster
{
    public class PartyResponse
    {
        public int PartyCode { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string? Add1 { get; set; } = string.Empty;
        public string? Add2 { get; set;} = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        public string? Pin { get; set; } = string.Empty;
        public string? Mobile { get; set; } = string.Empty;
        public string? GSTNo { get; set; } = string.Empty;
        public string? DrugLicenceNo { get; set; } = string.Empty;
    } // class...
}
