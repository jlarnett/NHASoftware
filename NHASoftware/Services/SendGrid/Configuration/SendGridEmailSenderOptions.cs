﻿namespace NHA.Website.Software.Services.SendGrid.Configuration
{
    public class SendGridEmailSenderOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }
}
