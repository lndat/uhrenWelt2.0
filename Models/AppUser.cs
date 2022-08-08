using Microsoft.AspNetCore.Identity;

namespace uhrenWelt.Models
{
    public class AppUser : IdentityUser
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}