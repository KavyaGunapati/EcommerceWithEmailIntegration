using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Models.DTOs;
using SecureTaskAPI.Models.Constants;

namespace ECommerceWithEmail.IntegrationTests
{
    [TestFixture]
    public class CategoryControllerIntegrationTests
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUpPerTest()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
            // Ensure DB is clean for this test
            _client.PostAsync("/test/clear-categories", null).GetAwaiter().GetResult();
        }

        [TearDown]
        public void TearDownPerTest()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }
        private async Task ClearAllCategoriesAsync()
        {
            var get = await _client.GetAsync("/api/Category/all");
            if (get.StatusCode != System.Net.HttpStatusCode.OK) return;
            var wrapper = await get.Content.ReadFromJsonAsync<Result<List<CategoryDto>>>();
            var all = wrapper?.Data;
            if (all == null) return;
            foreach (var c in all)
            {
                await _client.DeleteAsync($"/api/Category/delete/{c.Id}");
            }
        }
        // No longer clearing via shared DB; each test gets a fresh factory/client instance
        [Test]
        public async Task AddCategory_ShouldReturnOk()
        {
            var category=new CategoryDto
            {
                Name="Test Category"
            };
            var res=await _client.PostAsJsonAsync("/api/Category/add",category);
            res.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var result=await res.Content.ReadFromJsonAsync<Result>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be(ErrorConstants.Added);
        }
        [Test]
        public async Task GetAllCategories_ShouldReturnOk()
        {
            var category=new CategoryDto
            {
                Name="Test Category"
            };
            await _client.PostAsJsonAsync("/api/Category/add",category);
            var res=await _client.GetAsync("/api/Category/all");
            res.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var result=await res.Content.ReadFromJsonAsync<Result<List<CategoryDto>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result!.Data!.Count.Should().BeGreaterThanOrEqualTo(1);
        }
        [Test]
        public async Task GetAll_ShouldReturnEmptyList()
        {
            await _client.PostAsJsonAsync("/api/Category/add",new CategoryDto{Name="Test Category"});
            await _client.PostAsJsonAsync("/api/Category/add",new CategoryDto{Name="Test Category 2"});
            var res=await _client.GetAsync("/api/Category/all");
            res.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var result=await res.Content.ReadFromJsonAsync<Result<List<CategoryDto>>>();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result!.Data!.Count.Should().BeGreaterThanOrEqualTo(2);
        }
        [Test]
        public async Task GetById_ShouldReturnOK()
        {
            await _client.PostAsJsonAsync("/api/Category/add",new CategoryDto{Name="Test Category"});
            var allRes=await _client.GetAsync("/api/Category/all");
            var all=await allRes.Content.ReadFromJsonAsync<Result<List<CategoryDto>>>();
            var first=all.Data.First();
            var res=await _client.GetAsync($"/api/Category/{first.Id}");
            res.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var body=await res.Content.ReadFromJsonAsync<Result<CategoryDto>>();
            body.Should().NotBeNull();
            body.Success.Should().BeTrue();
            body.Data.Id.Should().Be(first.Id);
            body.Data.Name.Should().Be(first.Name);
        }
    }
}