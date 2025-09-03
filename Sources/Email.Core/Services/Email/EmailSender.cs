using global::Email.Core.Abstractions.Email;
using global::Email.Core.Models.Email;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Email.Core.Services.Email;

public sealed class EmailSender : IEmailSender
{
    private sealed record SmtpPreset(string Host, int Port, bool EnableSsl);

    private static SmtpPreset ResolvePreset(string provider, string senderEmail, SmtpOverrides? ovr)
    {
        var p = (provider ?? "auto").Trim().ToLowerInvariant();
        if (p == "custom")
        {
            if (string.IsNullOrWhiteSpace(ovr?.Host) || ovr!.Port is null || ovr.EnableSsl is null)
                throw new InvalidOperationException("Custom provider requires Smtp.Host, Smtp.Port, Smtp.EnableSsl.");
            return new(ovr.Host!, ovr.Port!.Value, ovr.EnableSsl!.Value);
        }

        if (p == "gmail") return new("smtp.gmail.com", 587, true);
        if (p is "outlook" or "office365" or "m365") return new("smtp.office365.com", 587, true);

        // auto: detect by domain
        var dom = senderEmail.Split('@').LastOrDefault()?.ToLowerInvariant() ?? "";
        if (dom is "gmail.com" or "googlemail.com") return new("smtp.gmail.com", 587, true);
        // default to Outlook/M365 for non-gmail business domains
        return new("smtp.office365.com", 587, true);
    }

    public async Task SendAsync(EmailSendRequest req, CancellationToken ct = default)
    {
        // minimal validation
        if (string.IsNullOrWhiteSpace(req.SenderEmail)) throw new ArgumentException("SenderEmail is required.");
        if (req.To is null || req.To.Count == 0) throw new ArgumentException("At least one recipient in 'To' is required.");
        if (string.IsNullOrWhiteSpace(req.HtmlBody) && string.IsNullOrWhiteSpace(req.HtmlBodyBase64) && string.IsNullOrWhiteSpace(req.TextBody))
            throw new ArgumentException("Provide HtmlBody/HtmlBodyBase64 or TextBody.");

        var preset = ResolvePreset(req.Provider, req.SenderEmail, req.Smtp);

        // build MailMessage
        using var msg = new MailMessage
        {
            From = new MailAddress(req.SenderEmail, req.SenderEmail),
            Subject = req.Subject ?? "(no subject)",
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        foreach (var t in req.To) msg.To.Add(MailAddressParser(t));
        if (req.Cc != null) foreach (var c in req.Cc) msg.CC.Add(MailAddressParser(c));
        if (req.Bcc != null) foreach (var b in req.Bcc) msg.Bcc.Add(MailAddressParser(b));
        if (!string.IsNullOrWhiteSpace(req.ReplyTo)) msg.ReplyToList.Add(MailAddressParser(req.ReplyTo!));

        // content: prefer base64 HTML > HTML > text
        var html = !string.IsNullOrWhiteSpace(req.HtmlBodyBase64)
            ? Encoding.UTF8.GetString(Convert.FromBase64String(req.HtmlBodyBase64!))
            : req.HtmlBody;

        if (!string.IsNullOrWhiteSpace(html) && !string.IsNullOrWhiteSpace(req.TextBody))
        {
            // both plain + html using alternate views
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(req.TextBody!, Encoding.UTF8, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html!, Encoding.UTF8, MediaTypeNames.Text.Html));
            msg.Body = req.TextBody;
            msg.IsBodyHtml = false;
        }
        else if (!string.IsNullOrWhiteSpace(html))
        {
            msg.Body = html!;
            msg.IsBodyHtml = true;
        }
        else
        {
            msg.Body = req.TextBody!;
            msg.IsBodyHtml = false;
        }

        // attachments
        if (req.Attachments != null)
        {
            foreach (var a in req.Attachments)
            {
                var bytes = Convert.FromBase64String(a.ContentBase64);
                var stream = new MemoryStream(bytes);
                var attachment = new Attachment(stream, a.FileName, a.ContentType ?? "application/octet-stream");
                // stream disposed with MailMessage
                msg.Attachments.Add(attachment);
            }
        }

        using var smtp = new SmtpClient(preset.Host, preset.Port)
        {
            EnableSsl = preset.EnableSsl,             // STARTTLS on 587 when true
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Timeout = 30_000
        };

        // basic auth only in this step
        if (string.IsNullOrWhiteSpace(req.SenderPassword))
            throw new InvalidOperationException("SenderPassword is required for SMTP basic authentication.");
        smtp.Credentials = new NetworkCredential(req.SenderEmail, req.SenderPassword);

        // .NET SmtpClient has async send
        await smtp.SendMailAsync(msg, ct);
    }

    private static MailAddress MailAddressParser(string raw)
    {
        // supports "Name <email@host>" or "email@host"
        try
        {
            return new MailAddress(raw.Trim());
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid email address: '{raw}'. {ex.Message}");
        }
    }
}
