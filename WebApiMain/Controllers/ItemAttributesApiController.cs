using Domain.ItemAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.ItemAttributes;
using WebApiMain.Filters.ItemAttributes;

namespace WebApiMain.Controllers
{
    [ApiController]
    [Route("item_attributes")]    

    public class ItemAttributesApiController : ControllerBase
    {
        private readonly IItemAttributeResponse iattr;

        public ItemAttributesApiController(IItemAttributeResponse iattr)
        {
            this.iattr = iattr;
        } // constructor...

        [HttpGet("")]
        [Authorize(Policy = "ATTRMASTER-VIEW_POLICY")]
        public async Task<IActionResult> GetAllAttributes()
        {
            List<ItemAttrHeadResponse> heads = await iattr.GetAllAttributes();
            return Ok(heads);
        } // GetAllAttributes...

        [HttpGet("{id?}")]
        [ItemAttrsActionFilter]
        [Authorize(Policy = "ATTRMASTER-ALL_POLICY")]
        public async Task<IActionResult> GetAttributeById(int? id)
        {
            var attr = await iattr.GetAttributeById(id);
            if(attr.IdNo == 0)
            {
                ModelState.AddModelError("NotFound", $"Item Attribute Head Id. {id} not found.");
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status404NotFound
                };
                return new NotFoundObjectResult(problemDetails);
            } // end if...

            return Ok(attr);
        } // GetAttributeById...

        [HttpPost("")]
        [ItemAttrsSaveActionFilter]
        [Authorize(Policy = "ATTRMASTER-ALL_POLICY")]
        public async Task<IActionResult> SaveAttr(ItemAttrHeadResponse head)
        {
            string str = await iattr.Save(head);
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
        } // SaveAttr...

        [HttpGet("details/{headId}")]
        public async Task<IActionResult> GetAttrDtlsByAttrHeadId(int headId)
        {
            var dtls = await iattr.GetAttrDtlsByAttrHeadId(headId);
            return Ok(dtls);
        } // GetAttrDtlsByAttrHeadId...
    } // class...
}
