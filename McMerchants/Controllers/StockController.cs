using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace McMerchants.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        // GET api/<StockController>/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }
    }
}
