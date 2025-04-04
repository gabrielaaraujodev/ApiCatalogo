using ApiCatalogo.Models;
using ApiCatalogo.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiCatalogo.DTOs
{
    public class ProductDtoUpdateRequest : IValidatableObject
    {
        [Range(1, 9999, ErrorMessage = "Estoqeu deve estar entre 1 e 9999")]
        public float Stock { get; set; }
        public DateTime RegistrationDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(RegistrationDate.Date <= DateTime.Now.Date)
            {
                yield return new ValidationResult("A data deve ser maior que a data atual",
                        new[] { nameof(this.RegistrationDate) });
            }
        }
    }
}
