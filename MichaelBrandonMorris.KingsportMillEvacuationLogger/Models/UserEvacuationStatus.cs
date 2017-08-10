using System.Collections.Generic;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger.Models
{
    /// <summary>
    ///     Enum UserEvacuationStatus
    /// </summary>
    /// TODO Edit XML Comment Template for UserEvacuationStatus
    public enum UserEvacuationStatus
    {
        /// <summary>
        ///     The unknown
        /// </summary>
        /// TODO Edit XML Comment Template for Unknown
        Unknown,

        /// <summary>
        ///     The missing
        /// </summary>
        /// TODO Edit XML Comment Template for Missing
        Missing,

        /// <summary>
        ///     The here
        /// </summary>
        /// TODO Edit XML Comment Template for Here
        Here,

        /// <summary>
        ///     The away
        /// </summary>
        /// TODO Edit XML Comment Template for Away
        Away
    }

    /// <summary>
    ///     Class UserEvacuationStatusMap.
    /// </summary>
    /// TODO Edit XML Comment Template for UserEvacuationStatusMap
    public static class UserEvacuationStatusMap
    {
        /// <summary>
        ///     Gets the map.
        /// </summary>
        /// <value>The map.</value>
        /// TODO Edit XML Comment Template for Map
        public static Dictionary<UserEvacuationStatus, int> Map => new
            Dictionary<UserEvacuationStatus, int>
            {
                {
                    UserEvacuationStatus.Missing, 0
                },
                {
                    UserEvacuationStatus.Unknown, 1
                },
                {
                    UserEvacuationStatus.Here, 2
                },
                {
                    UserEvacuationStatus.Away, 3
                }
            };
    }
}