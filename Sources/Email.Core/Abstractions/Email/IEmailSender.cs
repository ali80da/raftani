using Email.Core.Models.Email;

namespace Email.Core.Abstractions.Email;

public interface IEmailSender
{

    Task SendAsync(EmailSendRequest request, CancellationToken ct = default);

}