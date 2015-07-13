using System.ComponentModel.DataAnnotations;

namespace fIT.WebApi.Models
{
    public class EntryModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
    }
}
