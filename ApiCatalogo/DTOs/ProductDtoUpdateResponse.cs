﻿namespace ApiCatalogo.DTOs
{
    public class ProductDtoUpdateResponse
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public float Stock { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int CategoryId { get; set; }
    }
}
