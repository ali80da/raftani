using Microsoft.AspNetCore.Mvc;

namespace Email.Server.Areas.Raftani.V01.Controllers;

[Area("Raftani")]
[ApiVersion("1.0")]
[Route("raft/{Version}/[controller]")]
[ApiController]
public class SharedV01Controller : ControllerBase {}