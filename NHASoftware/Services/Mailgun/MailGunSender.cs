using Microsoft.AspNetCore.Identity.UI.Services;
using RestSharp;
using RestSharp.Authenticators;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace NHA.Website.Software.Services.Mailgun
{
    class MailGunSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public MailGunSender(IConfiguration configuration, ILogger<MailGunSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var response = await ExecuteEmailAsync(subject, htmlMessage, email);
            _logger.Log(LogLevel.Information, "Sent request to MailGun API, returned status code - {0}", response.StatusCode);
        }

        private async Task<RestResponse> ExecuteEmailAsync(string subject, string message, string email)
        {
            var options = new RestClientOptions("https://api.mailgun.net")
            {
                Authenticator = new HttpBasicAuthenticator("api", _configuration["Mailgun:ApiKey"] ?? "")
            };

            var client = new RestClient(options);
            var request = new RestRequest($"/v3/{_configuration["Mailgun:Domain"]}/messages", Method.Post);

            request.AlwaysMultipartFormData = true;
            request.AddParameter("from", $"Mailgun Sandbox <postmaster@{_configuration["Mailgun:Domain"]}>");
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("text", message);
            return await client.ExecuteAsync(request);
        }
    }
}