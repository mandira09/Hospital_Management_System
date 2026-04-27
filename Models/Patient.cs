using System.ComponentModel.DataAnnotations;
namespace Hospital_Management.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        [Required]
        public string Contact { get; set; }
    }
}
