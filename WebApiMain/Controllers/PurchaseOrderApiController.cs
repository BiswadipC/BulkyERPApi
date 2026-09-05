using Domain.PurchaseOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.PurchaseOrder;
using WebApiMain.Filters.PurchaseOrder;

namespace WebApiMain.Controllers
{
    [Route("purchase-order")]
    [ApiController]

    public class PurchaseOrderApiController : ControllerBase
    {
        private readonly IPurchaseOrder ipo;

        public PurchaseOrderApiController(IPurchaseOrder ipo)
        {
            this.ipo = ipo;
        } // constructor...

        [HttpGet("")]
        public async Task<IActionResult> GetPOHeads()
        {
            var heads = await this.ipo.GetPOHeads();
            return Ok(heads);
        } // GetPOHeads...

        [HttpGet("details/{orderId}")]
        public async Task<IActionResult> GetPODtlsByOrderId(int orderId)
        {
            var dtls = await ipo.GetPODtlsByOrderId(orderId);
            return Ok(dtls ?? new List<PurchaseOrderDtlResponse>());
        } // GetPODtlsByOrderId...

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetPOHeadByOrderId(int orderId)
        {
            var head = await ipo.GetPOHeadByOrderId(orderId);
            return Ok(head);
        } // GetPOHeadByOrderId...

        [HttpGet("party/{partyCode}")]
        public async Task<IActionResult> GetPOHeadsByPartyCode(int? partyCode)
        {
            var heads = await ipo.GetPOHeadsByPartyCode(partyCode);
            return Ok(heads);
        } // GetPOHeadsByPartyCode...

        [HttpGet("GetPODtlByPODtlRecId/{recId}")]
        public async Task<IActionResult> GetPODtlByPODtlRecId(int recId)
        {
            var dtl = await ipo.GetPODtlByPODtlRecId(recId);
            return Ok(dtl);
        } // GetPODtlByPODtlRecId...

        [HttpPost("")]
        [SavePOActionfilter]
        public async Task<IActionResult> SavePO(PurchaseOrderHeadResponse head)
        {
            string str = await ipo.Save(head);
            if(str == "Success")
            {
                return Ok(new { message = str });
            }

            ModelState.AddModelError("BadRequest", str);
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        } // SavePO...
    } // class...
}
