using System.ComponentModel.DataAnnotations;
namespace Hospital_Management.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Specialization { get; set; }

        public string Availability { get; set; }
    }
}
