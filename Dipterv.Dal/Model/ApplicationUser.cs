using Dipterv.Shared.Enum;
using Microsoft.AspNetCore.Identity;

namespace Dipterv.Dal.Model
{
    public class ApplicationUser : IdentityUser<long>
    {
        public UserType UserType { get; set; }

        public int? CustomerId { get; set; }

        public Customer Customer { get; set; }

        public int? PersonId { get; set; }

        public Person Person { get; set; }
    }
}
