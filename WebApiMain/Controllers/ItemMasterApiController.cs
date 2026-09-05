using Domain.ItemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ItemMaster;
using WebApiMain.Filters.ItemMaster;

namespace WebApiMain.Controllers
{
    [Route("items")]
    [ApiController]

    public class ItemMasterApiController : ControllerBase
    {
        private readonly IItemMasterResponse iitem;

        public ItemMasterApiController(IItemMasterResponse iitem)
        {
            this.iitem = iitem;
        } // constructor...

        [HttpGet("")]
        [Authorize(Policy = "ITEMMASTER-VIEW_POLICY")]
        public async Task<IActionResult> GetItems()
        {
            var items = await iitem.GetItemHeads();
            return Ok(items);
        } // GetItems...

        [HttpGet("{id?}")]
        [GetItemHeadByIdActionFilter]
        [Authorize(Policy = "ITEMMASTER-ALL_POLICY")]
        public async Task<IActionResult> GetItemById(int? id)
        {
            var item = await iitem.GetItemHeadByItemId((int)id!);
            if(item.ItemId == -1)
            {
                ModelState.AddModelError("NotFound", $"Item Id. {id} does not exists. Please check your entry.");
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status404NotFound
                };
                return new NotFoundObjectResult(problemDetails);
            }

            return Ok(item);
        } // GetItemById...

        [HttpGet("ItemGST/{itemId}")]
        public async Task<IActionResult> GetItemGSTByItemId(int itemId)
        {
            var value = await iitem.GetItemGSTByItemId(itemId);
            return Ok(value);
        } // GetItemGSTByItemId...

        [HttpPost("")]
        [Authorize(Policy = "ITEMMASTER-ALL_POLICY")]
        [SaveItemActionFilter]
        public async Task<IActionResult> Save(ItemHeadResponse head)
        {
            string str = await iitem.Save(head);
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
        } // Save...
    } // class...
}
