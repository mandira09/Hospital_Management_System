using System.Net;
using System.Net.Mail;
namespace Hospital_Management.Services;
public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail(string to, string subject, string body)
    {
        var smtp = _config.GetSection("SmtpSettings");

        var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
        {
            Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress("test@mail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        client.Send(mail);
    }
}