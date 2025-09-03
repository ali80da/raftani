using System.ComponentModel.DataAnnotations;

namespace Email.Core.Models.Email;


public sealed class EmailAttachmentInput
{
    [Required] public string FileName { get; set; } = "";          // e.g. "offer.pdf"
    [Required] public string ContentBase64 { get; set; } = "";      // base64 of the file bytes
    public string? ContentType { get; set; } = "application/octet-stream";
}

public sealed class SmtpOverrides
{
    public string? Host { get; set; }        // required if Provider == "custom"
    public int? Port { get; set; }           // e.g. 587 or 465
    public bool? EnableSsl { get; set; }     // true: STARTTLS/SSL; false: plain (not recommended)
}

public sealed class EmailSendRequest
{
    /// <summary>"auto" | "gmail" | "outlook" | "custom"</summary>
    public string Provider { get; set; } = "auto";

    // auth (basic only in this step: Gmail app password or Outlook password)
    [Required] public string SenderEmail { get; set; } = "";
    public string? SenderPassword { get; set; } // for Gmail (app password) or Outlook basic SMTP

    // addressing
    [Required] public List<string> To { get; set; } = new();
    public List<string>? Cc { get; set; }
    public List<string>? Bcc { get; set; }
    public string? ReplyTo { get; set; }

    // content
    [Required] public string Subject { get; set; } = "(no subject)";
    /// <summary>Optional plain text body</summary>
    public string? TextBody { get; set; }
    /// <summary>HTML body (preferred). If HtmlBodyBase64 is present, it takes precedence.</summary>
    public string? HtmlBody { get; set; }
    public string? HtmlBodyBase64 { get; set; }

    // attachments (optional)
    public List<EmailAttachmentInput>? Attachments { get; set; }

    // optional SMTP custom overrides (when Provider == "custom")
    public SmtpOverrides? Smtp { get; set; }
}
