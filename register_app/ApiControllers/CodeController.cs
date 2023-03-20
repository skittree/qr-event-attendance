using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using register_app.DtoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace register_app.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(CodeDto model)
        {
            if (model == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                Console.WriteLine("received");
                return Ok();
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
