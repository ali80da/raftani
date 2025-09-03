using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Email.Web.Controllers;

[Route("raft/[controller]")]
[ApiController]
public class SharedController : ControllerBase {}