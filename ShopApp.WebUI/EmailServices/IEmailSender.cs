using System;
namespace ShopApp.WebUI.EmailServices
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string to, string subject, string body ,bool isBodyHtml=true);
        Task SendEmailAsync(string[] tos, string subject, string body, bool isBodyHtml = true);
    }
}

