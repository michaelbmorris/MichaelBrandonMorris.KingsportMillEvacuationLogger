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
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        /// TODO Edit XML Comment Template for SendEmailAsync
        Task SendEmailAsync(string email, string subject, string message);
    }
}