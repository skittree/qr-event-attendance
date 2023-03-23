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
        private IFormService FormService { get; }

        public WatchController(IFormService formService)
        {
            FormService = formService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JObject payload)
        {
            if (payload == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                string formId = payload["message"]["attributes"]["formId"].ToString();
                //add function here to update list of attendees for an event (connected to formid) and send emails to all new participants
                await FormService.GetFormResponsesAsync(formId);
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
