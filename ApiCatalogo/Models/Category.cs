namespace ApiCatalogo.Models;

public class Category
{
    // Para o EF reconhecer a propriedade sendo uma PK, deve-se criar sendo do tipo INT e nomeDaClasse + Id ou, somente, Id.
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
}
