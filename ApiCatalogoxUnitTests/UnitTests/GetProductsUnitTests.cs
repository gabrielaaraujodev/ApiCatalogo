using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiCatalogo.Controllers;
using ApiCatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class GetProductsUnitTests:IClassFixture<ProductsUnitTestController>
    {
        private readonly ProductsController _controller;

        public GetProductsUnitTests(ProductsUnitTestController controller)
        {
            _controller = new ProductsController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task GetProductById_OkResult()
        {
            //Arrange
            var prodId = 2;
            //Act
            var data = await _controller.Get(prodId);
            //Assert (com FluentAssertions)
            data.Result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetProductById_Return_ListOfProductsDTO()
        {
            //Act
            var data = await _controller.Get();
            //Assert (com FluentAssertions)
            data.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<IEnumerable<ProductDTO>>()
                .And.NotBeNull();
        }

        [Fact]
        public async Task GetProductById_Return_NotFound()
        {
            //Arrange
            var prodId = 999;
            //Act
            var data = await _controller.Get(prodId);
            //Assert (com FluentAssertions)
            data.Result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetProductById_Return_BadRequest()
        {
            //Arrange
            var prodId = -1;
            //Act
            var data = await _controller.Get(prodId);
            //Assert (com FluentAssertions)
            data.Result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetProductById_Return_BadRequestListOfProducts()
        {
            //Act
            var data = await _controller.Get();
            //Assert (com FluentAssertions)
            data.Result.Should().BeOfType<BadRequestResult>();
        }
    }
}
