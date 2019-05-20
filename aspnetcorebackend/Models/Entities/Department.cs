using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetcorebackend.Models.Entities
{

    public class Department
    {
        public Guid Id { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(4)]
        public string Name { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(55)]
        public string Description { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(55)]
        public string Head { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(12)]
        public string Code { get; set; }
    }
}