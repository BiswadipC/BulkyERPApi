using Domain.PartyMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.PartyMaster
{
    public interface IPartyMaster
    {
        Task<List<PartyResponse>> GetParties();
        Task<PartyResponse> GetPartyByCode(int code);
        Task<string> Save(PartyResponse response);
    } // interface...
}
