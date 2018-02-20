using System.ComponentModel.DataAnnotations;

namespace Store.Web.ViewsModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Email {get; set;}
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}