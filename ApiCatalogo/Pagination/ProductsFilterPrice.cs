namespace ApiCatalogo.Pagination
{
    public class ProductsFilterPrice : QueryStringParameters
    {
        public decimal? Price { get; set; }
        public string? PriceStandart { get; set; }
    }
}
