using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptionsPattern.Models.Items;
using OptionsPattern.Services.Items;
namespace OptionsPattern.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]

    public class ItemController : ControllerBase
    {

        private readonly IItemServices _item;
        public ItemController(IItemServices item)
        {
            _item = item;
        }
        [Authorize(Policy = "JWTOrCookies")]
        [HttpGet("Display")]
        public IActionResult Display()
        {
            var item = _item.GetItems();
            return Ok(item);
        }
        [Authorize(Policy = "SecondJwt")]
        [HttpPost("Create")] 
        public IActionResult Create(Item obj)
        {
            if (ModelState.IsValid)
            {
               _item.AddItem(obj); 
            }
            return Ok(obj);
        }
        [Authorize(Policy = "Jwt")]
        [HttpPost("Update")]
        public IActionResult Update( int id, Item obj)
        {           
           var asd =  _item.UpdateItem(id,obj); 
           return Ok(obj);
        }
        [Authorize(Policy = "Cookies")]
        [HttpPost("Delete")]
        public IActionResult Delete(int Id)
        {
           var asd= _item.DeleteItem(Id);
            return Ok(Id);
        }
    }
}
