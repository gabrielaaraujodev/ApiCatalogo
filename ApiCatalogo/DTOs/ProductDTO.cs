using ApiCatalogo.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(80)]
        [PrimeiraLetraMaiuscula]
        public string? Name { get; set; }

        [Required]
        [StringLength(300)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        [Required]
        [StringLength(300)]
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

    }
}
