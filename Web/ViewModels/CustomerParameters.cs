using System.ComponentModel.DataAnnotations;

namespace MMT_Test
{
    public class CustomerParameters
    {
        [Required]
        public string User { get; set; }

        [Required]
        public string CustomerId { get; set; }
    }
}