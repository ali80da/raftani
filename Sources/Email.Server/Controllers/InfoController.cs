using Email.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Email.Server.Controllers;

public class InfoController : SharedController
{



    [HttpGet("info")]
    public IActionResult Info()
    {
        return Ok(new
        {
            Author = "Ali Darehshori",
            Github = "ali80da",
            ProName = "Raftani"
        });
    }





}