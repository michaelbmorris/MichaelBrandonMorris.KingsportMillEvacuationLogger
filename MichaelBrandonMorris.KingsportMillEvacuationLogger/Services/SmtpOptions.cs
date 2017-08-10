namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Services
{
    /// <summary>
    ///     Class SmtpOptions.
    /// </summary>
    /// TODO Edit XML Comment Template for SmtpOptions
    public class SmtpOptions
    {
        /// <summary>
        ///     Gets or sets from address.
        /// </summary>
        /// <value>From address.</value>
        /// TODO Edit XML Comment Template for FromAddress
        public string FromAddress
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets from name.
        /// </summary>
        /// <value>From.</value>
        /// TODO Edit XML Comment Template for From
        public string FromName
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        /// TODO Edit XML Comment Template for Password
        public string Password
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        /// TODO Edit XML Comment Template for Port
        public int Port
        {
            get;
            set;
        } = 25;

        /// <summary>
        ///     Gets or sets the preferred encoding.
        /// </summary>
        /// <value>The preferred encoding.</value>
        /// TODO Edit XML Comment Template for PreferredEncoding
        public string PreferredEncoding
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets the reply to address.
        /// </summary>
        /// <value>The reply to address.</value>
        /// TODO Edit XML Comment Template for ReplyToAddress
        public string ReplyToAddress
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets the reply to name.
        /// </summary>
        /// <value>The reply to.</value>
        /// TODO Edit XML Comment Template for ReplyTo
        public string ReplyToName
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether [requires
        ///     authentication].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [requires authentication]; otherwise,
        ///     <c>false</c>.
        /// </value>
        /// TODO Edit XML Comment Template for RequiresAuthentication
        public bool RequiresAuthentication
        {
            get;
            set;
        } = false;

        /// <summary>
        ///     Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        /// TODO Edit XML Comment Template for Server
        public string Server
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        /// TODO Edit XML Comment Template for User
        public string User
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether [use SSL].
        /// </summary>
        /// <value><c>true</c> if [use SSL]; otherwise, <c>false</c>.</value>
        /// TODO Edit XML Comment Template for UseSsl
        public bool UseSsl
        {
            get;
            set;
        } = false;
    }
}