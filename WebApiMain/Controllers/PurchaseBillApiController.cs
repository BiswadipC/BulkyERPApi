using DocumentFormat.OpenXml.InkML;
using Domain.PurchaseBill;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.PurchaseBill;
using WebApiMain.Filters.PurchaseBill;

namespace WebApiMain.Controllers
{
    [Route("purchase-bill")]
    [ApiController]

    public class PurchaseBillApiController : ControllerBase
    {
        private readonly IPurchaseBill ibill;

        public PurchaseBillApiController(IPurchaseBill ibill)
        {
            this.ibill = ibill;
        } // constructor...

        [HttpGet("")]
        public async Task<IActionResult> GetPurchaseBills()
        {
            var bills = await ibill.GetPurchaseBills();
            return Ok(bills);
        } // GetPurchaseBills...

        [HttpGet("details/{billId}")]
        public async Task<IActionResult> GetPurchaseBillDtlsByBillId(int billId)
        {
            var details = await ibill.GetPurchaseBillDtlsByBillId(billId);
            return Ok(details);
        } // GetPurchaseBillDtlsByBillId...

        [HttpGet("{billId}")]
        public async Task<IActionResult> GetPurchaseBillHeadByBillId(int billId)
        {
            var head = await ibill.GetPurchaseBillHeadByBillId(billId);
            return Ok(head);
        } // GetPurchaseBillHeadByBillId...

        [HttpPost("pbdtsByOrderIds")]
        public async Task<IActionResult> PopulatePBByOrderId([FromBody]int[] orderIds)
        {
            var dtls = await ibill.PopulatePBByOrderId(orderIds);
            return Ok(dtls);
        } // PopulatePBByOrderId...

        [HttpGet("GetPBDtlByPBDtlRecid/{recId}")]
        public async Task<IActionResult> GetPBDtlByPBDtlRecid(int recId)
        {
            var dtl = await ibill.GetPBDtlByPBDtlRecid(recId);
            return Ok(dtl);
        } // GetPBDtlByPBDtlRecid...

        [HttpPost("")]
        [SavePBActionFilter]
        public async Task<IActionResult> SaveBill(PurchaseBillHeadResponse head)
        {
            string str = await ibill.SaveBill(head);
            if(str == "Success")
            {
                return Ok(new { Message = str });
            }

            ModelState.AddModelError("BadRequest", str);
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        } // SaveBill...
    } // class...
}
