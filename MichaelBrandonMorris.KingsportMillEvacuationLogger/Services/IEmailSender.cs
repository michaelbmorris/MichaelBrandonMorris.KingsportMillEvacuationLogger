using System.Threading.Tasks;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Services
{
    /// <summary>
    ///     Interface IEmailSender
    /// </summary>
    /// TODO Edit XML Comment Template for IEmailSender
    public interface IEmailSender
    {
        /// <summary>
        ///     Sends the email asynchronous.
        /// </summary>
        /// <param name="toName">To name.</param>
        /// <param name="toAddress">To address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="htmlBody">The HTML body.</param>
        /// <param name="textBody">The text body.</param>
        /// <returns>Task.</returns>
        /// TODO Edit XML Comment Template for SendEmailAsync
        Task SendEmailAsync(
            string toName,
            string toAddress,
            string subject,
            string htmlBody,
            string textBody);
    }
}