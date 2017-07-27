using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Services
{
    /// <summary>
    ///     Class AuthMessageSender.
    /// </summary>
    /// <seealso cref="IEmailSender" />
    /// <seealso cref="ISmsSender" />
    /// TODO Edit XML Comment Template for AuthMessageSender
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="EmailSender" /> class.
        /// </summary>
        /// <param name="optionsAccessor">The options accessor.</param>
        /// TODO Edit XML Comment Template for #ctor
        public AuthMessageSender(
            IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        /// <summary>
        ///     Gets the options.
        /// </summary>
        /// <value>The options.</value>
        /// TODO Edit XML Comment Template for Options
        public AuthMessageSenderOptions Options
        {
            get;
        }

        /// <summary>
        ///     Sends the email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        /// TODO Edit XML Comment Template for SendEmailAsync
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        /// <summary>
        ///     Sends the SMS asynchronous.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        /// TODO Edit XML Comment Template for SendSmsAsync
        public Task SendSmsAsync(string number, string message)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Executes the specified API key.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="email">The email.</param>
        /// <returns>Task.</returns>
        /// TODO Edit XML Comment Template for Execute
        public Task Execute(
            string apiKey,
            string subject,
            string message,
            string email)
        {
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(
                    "noreply@kingsportmill.com",
                    "Kingsport Mill Evacuation Logger"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}