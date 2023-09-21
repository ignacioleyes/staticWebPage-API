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
            var from = new EmailAddress("ventas@conduflex.com.ar", "Nuevo contacto");
            var to = new EmailAddress("consultasweb@conduflex.com.ar", "Nuevo Contacto");
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicTemplateData);
            return await client.SendEmailAsync(msg);
        }


    }
}
