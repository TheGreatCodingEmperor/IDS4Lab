using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using IdentityServer4.AccessTokenValidation;

namespace Api.Controllers {
    [Route ("identity")]
    [Authorize (AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    public class IdentityController : ControllerBase {
        private readonly ILogger<IdentityController> _logger;

        public IdentityController (ILogger<IdentityController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get () {
            return new JsonResult (from c in User.Claims select new { c.Type, c.Value });
        }
    }
}