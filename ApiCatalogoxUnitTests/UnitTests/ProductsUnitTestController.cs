using ApiCatalogo.Context;
using ApiCatalogo.DTOs.AutomaticMapper;
using ApiCatalogo.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class ProductsUnitTestController
    {
        public IUnitOfWork repository;
        public IMapper mapper;
        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string connectionString = 
            "Server=127.0.0.1;Port=3306;Database=ApiCatalogo;User=root;Password=120905lG*";

        static ProductsUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;
        }

        public ProductsUnitTestController() 
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductDtoMappingProfile());
            });

            mapper = config.CreateMapper();
            var context = new AppDbContext(dbContextOptions);
            repository = new UnitOfWork(context);
    }
    }
}
