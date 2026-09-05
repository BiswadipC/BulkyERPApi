using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Accounts
{
    public class AccountsResponse
    {
        public int AccountId {  get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Schedule {  get; set; } = string.Empty;
        public string TaxStructure {  get; set; } = string.Empty;
        public string? Add1 { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        public string? Pin { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Mobile { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Website { get; set; } = string.Empty;
        public string? AccountNo { get; set; } = string.Empty;
        public string? IFSCCode { get; set; } = string.Empty;
        public string? BranchCode { get; set; } = string.Empty;
    } // class...
}
