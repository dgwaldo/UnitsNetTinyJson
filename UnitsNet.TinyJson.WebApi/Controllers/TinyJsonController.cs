using Microsoft.AspNetCore.Mvc;
using TinyJson.WebApi.Models;
using UnitsNet.Units;

namespace TinyJson.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TinyJsonController : ControllerBase
    {
        private static readonly IEnumerable<Length> units = new[]
        {
            new Length(10, LengthUnit.Foot), new Length(100, LengthUnit.Mile), new Length(500, LengthUnit.Fathom)
        };

        [HttpGet(Name = "GetUnits")]
        public IActionResult Get()
        {
            var unitsData = units;
            return Ok(unitsData);
        }

        [HttpPut(Name = "SendUnits")]
        public IActionResult Put(TestDataModel model)
        {
            var test = model;
            return Ok(test);
        }

    }
}