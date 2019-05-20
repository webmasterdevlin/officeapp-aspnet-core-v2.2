using System.ComponentModel.DataAnnotations;

namespace aspnetcorebackend.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Head { get; set; }
        public string Code { get; set; }
    }
}