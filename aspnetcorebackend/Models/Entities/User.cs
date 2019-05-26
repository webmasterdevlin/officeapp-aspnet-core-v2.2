using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetcorebackend.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [MinLength(12)]
        [MaxLength(55)]
        [Required(ErrorMessage = "An email is required")]
        public string Email { get; set; }
        
//        [Required(ErrorMessage = "A password is required")]
//        [MinLength(6)]
//        [MaxLength(12)]
//        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}