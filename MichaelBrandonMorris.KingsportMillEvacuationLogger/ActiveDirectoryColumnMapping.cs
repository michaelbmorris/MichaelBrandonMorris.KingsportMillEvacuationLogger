using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger
{
    public class ActiveDirectoryColumnMapping
    {
        public string FirstName
        { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string IsActive { get; set; }
    }
}
