using System.ComponentModel.DataAnnotations;

namespace Authn.Models
{
    public class AppUser
    {

        public int Id { get; set; }
        public string Provider { get; set; }
        public string NameIdentifier { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Mobile { get; set; }
        public string Roles { get; set; }
        public List<string> RoleList
        {
            get
            {
                return Roles?.Split(',').ToList() ?? new List<string>();
            }
        }
    }
}
