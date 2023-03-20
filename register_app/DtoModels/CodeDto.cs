using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace register_app.DtoModels
{
    public class CodeDto
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
