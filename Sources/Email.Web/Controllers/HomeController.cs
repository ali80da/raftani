using Microsoft.AspNetCore.Mvc;

namespace Email.Web.Controllers;

public class HomeController : SharedController
{



    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }











}