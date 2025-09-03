using Microsoft.AspNetCore.Mvc;

namespace Email.Client.Controllers;

public class HomeController : SharedController
{



    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }











}