using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ResourceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var userInfo = HttpContext.User.Identity;
            var claimInfo = HttpContext.User.Claims;
            var isInAdministrator = User.IsInRole("Administrator");
            var isInOverriden = User.IsInRole("Overridden");
            var result = new {
                a = userInfo,
                b = claimInfo,
                isInAdministrator,
                isInOverriden
            };
            return Ok(result);
        }

        // GET api/values/MoraMocOboje
        [HttpGet("MoraMocOboje")]
        [Authorize("MoraMocOboje")]
        public ActionResult<string> GetMoraMocOboje(int id)
        {
            return "Mora moc oboje! Ti to mozes!";
        }

        // GET api/values/MozeBrisat
        [HttpGet("MozeBrisat")]
        [Authorize("MozeBrisat")]
        public ActionResult<string> GetMozeBrisat(int id) {
            return "Sorry nema brisanja za tebe...";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
