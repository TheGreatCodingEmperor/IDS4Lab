using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

namespace NgClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(ILogger<IdentityController> logger)
        {
            _logger = logger;
        }

        [HttpGet("token")]
        public async Task<IActionResult> Get()
        {
            var claims = User.Claims;
            var items = (await HttpContext.AuthenticateAsync())?.Properties.Items;
            if(items == null){
                items = new Dictionary<string,string>();
            }
            var token = new Dictionary<string,object>();
            foreach(var claim in claims){
                token[claim.Type] = claim.Value;
            }
            foreach(var item in items){
                token[item.Key] = item.Value;
            }
            return Ok(token);
        }
    }
}
