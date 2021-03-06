using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Miru.Core;
using Miru.Queuing;

namespace Miru.Mailing
{
    public class FluentEmailMailer : IMailer
    {
        private readonly ISender _sender;
        private readonly MailingOptions _options;
        private readonly Jobs _jobs;
        private readonly ILogger<IMailer> _logger;
        private readonly RazorViewToStringRenderer _razorRenderer;

        public FluentEmailMailer(
            ISender sender, 
            MailingOptions options, 
            Jobs jobs, 
            ILogger<IMailer> logger, 
            RazorViewToStringRenderer razorRenderer)
        {
            _sender = sender;
            _options = options;
            _jobs = jobs;
            _logger = logger;
            _razorRenderer = razorRenderer;
        }
        
        public async Task SendNow<TMailable>(TMailable mailable, Action<Email> emailBuilder = null) where TMailable : Mailable
        {
            var deliverableEmail = await BuildEmailFrom(mailable, emailBuilder);

            await _sender.SendAsync(new FluentEmail.Core.Email { Data = deliverableEmail });
            
            // TODO: if too many email, show: 'email1, email2 and more 300'
            _logger.LogDebug($"Sent email '{deliverableEmail.Subject}' to {deliverableEmail.ToAddresses.Select(m => m.EmailAddress).Join(",")}");
        }
        
        /// <summary>
        /// It builds the email using all the passed parameters/services, then queue the only email message object 
        /// </summary>
        public async Task SendLater<TMailable>(TMailable mailable, Action<Email> emailBuilder = null) where TMailable : Mailable
        {
            var fluentMail = await BuildEmailFrom(mailable, emailBuilder);

            Enqueue(fluentMail);
        }

        public async Task<Email> BuildEmailFrom<TMailable>(TMailable mailable, Action<Email> emailBuilder) where TMailable : Mailable
        {
            mailable.MailingOptions = _options;

            var email = new Email();

            _options.SetDefaultEmail(email);

            mailable.Build(email);
            
            emailBuilder?.Invoke(email);

            if (email.Template != null)
            {
                var fullFile = GetFullFile(mailable, email.Template);
                
                var body = await _razorRenderer.RenderViewToStringAsync(fullFile, email.Template.Model);

                email.Body = body;
                email.IsHtml = true;
            }

            return email;
        }

        private string GetFullFile(Mailable mailable, EmailTemplate emailTemplate)
        {
            if (emailTemplate.File.IsEmpty())
                emailTemplate.File = mailable.GetType().Name + ".cshtml";
            
            else if (emailTemplate.File.EndsWith(".cshtml") == false)
                emailTemplate.File += ".cshtml";

            var type = mailable.GetType();

            var dirs = type.Namespace!.Replace(type.Assembly.GetName().Name!, string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);

            var path = string.Join(Path.DirectorySeparatorChar, dirs);

            if (_options.TemplatePath.IsNotEmpty())
                return A.Path(_options.TemplatePath) / path / emailTemplate.File;
            
            return A.Path(path) / emailTemplate.File;
        }
        
        private void Enqueue(Email fluentMail)
        {
            _jobs.PerformLater(new EmailJob(fluentMail));

            _logger.LogDebug(
                $"Enqueued email '{fluentMail.Subject}' to {fluentMail.ToAddresses.Select(m => m.EmailAddress).Join(",")}");
        }
    }
}