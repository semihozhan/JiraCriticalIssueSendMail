using Attlassian.Core.Domain;
using System.Net;
using System.Net.Mail;

namespace Attlassian.MailService
{
    public static class MailManager
    {


        public static IConfiguration _configuration
        {
            get
            {
                return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            }
        }

        public static async Task<string> SendAsync(List<IssueModel> issueModels)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetSection("MailDetail:mail").Value);
            message.To.Add(_configuration.GetSection("MailDetail:To").Value);
            message.Subject = "Kritik SD tespit edildi";
            message.IsBodyHtml = true;
            


            string content = "<table border='1'><tr><th>Value</th><th>Summary</th><th>Description</th><tr/>";
            foreach (var item in issueModels)
            {
                content +=$"<tr><td><a href =\"https://jira.brighteventure.com/browse/{item.Value}\"  target=\"_blank\">{item.Value}</a></td><td>{item.Summary}</td><td>{item.Description}</td><tr/>";
            }
            content += "</table>";
           message.Body = content;


            try
            {
                var _host = _configuration.GetSection("MailDetail:Host").Value;
                var _port = 587;
                var _defaultCredentials = false;
                var _enableSsl = true;
                var _emailfrom = _configuration.GetSection("MailDetail:mail").Value;//Your yandex email adress
                var _password = _configuration.GetSection("MailDetail:password").Value;//Your yandex app password
                using (var smtpClient = new SmtpClient(_host, _port))
                {
                    smtpClient.EnableSsl = _enableSsl;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = _defaultCredentials;
                   
                    if (_defaultCredentials == false)
                    {
                        smtpClient.Credentials = new NetworkCredential(_emailfrom, _password);
                    }
                    
                    smtpClient.Send(message);
                }
                return "ok";
            }
            catch (System.Exception ex)
            {

                return ex.Message;
            }
        }

    }
}
