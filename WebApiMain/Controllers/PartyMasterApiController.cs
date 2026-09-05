using Domain.PartyMaster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.PartyMaster;
using WebApiMain.Filters.PartyMaster;

namespace WebApiMain.Controllers
{
    [Route("party")]
    [ApiController]

    public class PartyMasterApiController : ControllerBase
    {
        private readonly IPartyMaster iparty;

        public PartyMasterApiController(IPartyMaster iparty)
        {
            this.iparty = iparty;
        } // constructor...

        [HttpGet("")]
        public async Task<IActionResult> GetAllParties()
        {
            var parties = await iparty.GetParties();
            return Ok(parties);
        } // GetAllParties...

        [HttpGet("{partyCode}")]
        public async Task<IActionResult> GetPartyByCode(int partyCode)
        {
            var party = await iparty.GetPartyByCode(partyCode);
            return Ok(party);
        } // GetPartyByCode...

        [HttpPost("")]
        [SavePartyMasterActionFilter]
        public async Task<IActionResult> Save(PartyResponse party)
        {
            string str = await iparty.Save(party);
            return Ok(new {message = str});
        } // Save...
    } // class...
}
