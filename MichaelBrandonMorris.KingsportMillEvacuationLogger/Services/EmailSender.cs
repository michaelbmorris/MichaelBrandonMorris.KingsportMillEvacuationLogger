using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MichaelBrandonMorris.Core.Extensions.String;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Services
{
    /// <summary>
    ///     Class EmailSender.
    /// </summary>
    /// <seealso cref="IEmailSender" />
    /// TODO Edit XML Comment Template for EmailSender
    public class EmailSender : IEmailSender
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="EmailSender" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// TODO Edit XML Comment Template for #ctor
        public EmailSender(IOptions<SmtpOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        ///     Gets the options.
        /// </summary>
        /// <value>The options.</value>
        /// TODO Edit XML Comment Template for Options
        public SmtpOptions Options
        {
            get;
        }

        /// <summary>
        /// send email as an asynchronous operation.
        /// </summary>
        /// <param name="toName">To name.</param>
        /// <param name="toAddress">To address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <returns>Task.</returns>
        /// TODO Edit XML Comment Template for SendEmailAsync
        public async Task SendEmailAsync(
            string toName,
            string toAddress,
            string subject,
            string htmlBody,
            string textBody)
        {
            var msg = new MimeMessage();
            msg.From.Add(
                new MailboxAddress(Options.FromName, Options.FromAddress));

            msg.ReplyTo.Add(
                new MailboxAddress(
                    Options.ReplyToName,
                    Options.ReplyToAddress));

            msg.To.Add(new MailboxAddress(toName, toAddress));
            msg.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody.NullOrWhiteSpaceConditional(),
                TextBody = textBody.NullOrWhiteSpaceConditional()
            };

            msg.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client
                    .ConnectAsync(Options.Server, Options.Port, Options.UseSsl)
                    .ConfigureAwait(false);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                if (Options.RequiresAuthentication)
                {
                    await client
                        .AuthenticateAsync(Options.User, Options.Password)
                        .ConfigureAwait(false);
                }

                await client.SendAsync(msg).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}