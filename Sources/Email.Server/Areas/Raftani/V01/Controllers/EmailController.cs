using Email.Core.Models.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Email.Core.Abstractions.Email;
using System.Net.Mail;

namespace Email.Server.Areas.Raftani.V01.Controllers;


public sealed record ApiEnvelope(bool ok, string? code = null, string? error = null)
{
    public static ApiEnvelope Ok() => new(true);
    public static ApiEnvelope Fail(string code, string error) => new(false, code, error);
}

public sealed class EmailController : SharedV01Controller
{

    private readonly IEmailSender _sender;
    private readonly ILogger<EmailController> _log;

    public EmailController(IEmailSender sender, ILogger<EmailController> log)
    {
        _sender = sender;
        _log = log;
    }



    [HttpPost("send")]
    [ProducesResponseType(typeof(ApiEnvelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiEnvelope), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiEnvelope>> Send([FromBody] EmailSendRequest req, CancellationToken ct)
    {
        try
        {
            await _sender.SendAsync(req, ct);
            return Ok(ApiEnvelope.Ok());                // ✅ بدنهٔ JSON
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiEnvelope.Fail("VALIDATION", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiEnvelope.Fail("POLICY_OR_CONFIG", ex.Message));
        }
        catch (SmtpException ex)
        {
            _log.LogWarning(ex, "SMTP failed");
            return BadRequest(ApiEnvelope.Fail("SMTP", ex.Message));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected");
            return StatusCode(500, ApiEnvelope.Fail("UNKNOWN", ex.Message));
        }
    }

}