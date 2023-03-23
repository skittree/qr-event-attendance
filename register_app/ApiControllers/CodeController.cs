using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using register_app.DtoModels;
using register_app.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace register_app.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeController : ControllerBase
    {
        private IAttendeeService AttendeeService { get; }

        public CodeController(IAttendeeService attendeeService)
        {
            AttendeeService = attendeeService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CodeDto model)
        {
            if (model == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                var attendee = await AttendeeService.AuthenticateAttendeeAsync(model.Key);
                return Ok();
            }
            catch (ArgumentNullException ae)
            {
                return BadRequest(ae.Message);
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
