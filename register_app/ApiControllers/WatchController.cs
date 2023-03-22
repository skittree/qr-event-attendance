using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using register_app.DtoModels;
using register_app.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace register_app.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchController : ControllerBase
    {
        private IAttendeeService AttendeeService { get; }

        public WatchController(IAttendeeService attendeeService)
        {
            AttendeeService = attendeeService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] JObject payload)
        {
            if (payload == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                string formId = payload["formId"].ToString();
                //add function here to update list of attendees for an event (connected to formid) and send emails to all new participants
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
