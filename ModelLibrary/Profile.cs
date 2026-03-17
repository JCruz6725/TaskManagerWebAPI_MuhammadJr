using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public partial class Profile
    {
        public Guid Id { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Education { get; set; } = null!;

        public string Employer { get; set; } = null!;

        public string JobTitle { get; set; } = null!;

        public Guid PurposeId { get; set; }

        public Guid LicenseId { get; set; }

        public Guid CreatedUserId { get; set; }

        public virtual User CreatedUser { get; set; } = null!;

        public virtual LicenseType License { get; set; } = null!;

        public virtual PurposeType Purpose { get; set; } = null!;
    }
}
