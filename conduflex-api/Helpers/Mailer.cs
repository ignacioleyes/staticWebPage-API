using SendGrid;
using SendGrid.Helpers.Mail;

namespace conduflex_api.Helpers
{
    public static class Mailer
    {
        public static readonly bool isTest = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLowerInvariant().Contains("mvc.testing"));

        public async static Task<Response> SendMail(string templateId, Dictionary<string, string> dynamicTemplateData)
        {
            var apiKey = Environment.GetEnvironmentVariable("CONDUFLEX_SENDGRIDKEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ConduflexSenderMail@conduflex.com", "Conduflex nuevo contacto");
            var to = new EmailAddress("ConduflexReceiverMail@conduflex.com", "ConduflexName");
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicTemplateData);
            return await client.SendEmailAsync(msg);
        }


    }
}
