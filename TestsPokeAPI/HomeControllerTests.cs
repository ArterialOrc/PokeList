using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Moq;
using PokeAPI.Controllers;
using PokeAPI.Models;
using PokeAPI.Services.PokeAPI;
using Xunit;

namespace TestsPokeAPI;


public class HomeControllerTests
{
    [Fact]
    public async Task PokeWithId()
    {
        // Arrange
        var mockFileProvider = new Mock<IFileProvider>();
        var mockService = new Mock<IPokeApi>();
        mockService.Setup(repo => repo.GetPokeInf(1)).ReturnsAsync(GetTestPokemon);
        HomeController controller = new HomeController(mockFileProvider.Object);
 
        // Act
        var result = await controller.PokemonWithId(1,mockService.Object);

        var jsonResult = Assert.IsType<JsonResult>(result);
        var pokemon = Assert.IsAssignableFrom<Pokemon>(jsonResult.Value);
        
        Assert.Equal("pikachu",pokemon.Name);
    }
    
    [Fact]
    public async Task RandomPokemon()
    {
        // Arrange
        var mockFileProvider = new Mock<IFileProvider>();
        var mockService = new Mock<IPokeApi>();
        mockService.Setup(repo => repo.GetPokeRandom()).ReturnsAsync(5);
        HomeController controller = new HomeController(mockFileProvider.Object);
 
        // Act
        var result = await controller.PokemonRandom(mockService.Object);

        var jsonResult = Assert.IsType<JsonResult>(result);
        var id = Assert.IsAssignableFrom<int>(jsonResult.Value);
        
        Assert.Equal(5,id);
    }
    
    [Fact]
    public async Task PokemonList()
    {
        // Arrange
        var mockFileProvider = new Mock<IFileProvider>();
        var mockService = new Mock<IPokeApi>();
        mockService.Setup(repo => repo.GetPokeNames()).ReturnsAsync(GetTestListPokemon);
        HomeController controller = new HomeController(mockFileProvider.Object);
 
        // Act
        var result = await controller.Index(mockService.Object);

        var jsonResult = Assert.IsType<JsonResult>(result);
        var pokemons = Assert.IsAssignableFrom<List<Pokemon>>(jsonResult.Value);
        
        Assert.Equal(1,pokemons.ToList().Count);
    }
    
    [Fact]
    public async Task PokemonFight()
    {
        // Arrange
        var mockFileProvider = new Mock<IFileProvider>();
        var mockService = new Mock<IPokeApi>();
        mockService.Setup(repo => repo.GetPokeInf(1)).ReturnsAsync(GetTestPokemon);
        HomeController controller = new HomeController(mockFileProvider.Object);
 
        // Act
        var result = await controller.PokemonFight(mockService.Object,new Enemies(){myPoke = 1,enemyPoke = 1});

        var jsonResult = Assert.IsType<JsonResult>(result);
        var pokemons = Assert.IsAssignableFrom<List<Pokemon>>(jsonResult.Value);
        
        Assert.Equal(2,pokemons.ToList().Count);
    }
 
    [Fact]
    public async Task PokemonAttack()
    {
        // Arrange
        var mockFileProvider = new Mock<IFileProvider>();
        var mockService = new Mock<IPokeApi>();
        mockService.Setup(repo => repo.PokemonAttack(GetTestPokemon(),35)).Returns(GetTestPokemon);
        HomeController controller = new HomeController(mockFileProvider.Object);
 
        // Act
        var result = await controller.PokemonAttack(mockService.Object,6,new EnemiesPokes(){myPoke = GetTestPokemon(),enemyPoke = GetTestPokemon()});

        var jsonResult = Assert.IsType<JsonResult>(result);
        var pokemons = Assert.IsAssignableFrom<List<Pokemon>>(jsonResult.Value);
        
        Assert.Equal(2,pokemons.ToList().Count);
    }
    
    private Pokemon GetTestPokemon()
    {
        var pokemon = new Pokemon
        {
            Name = "pikachu",
            Id=3,
            AttackPower = 35,
            Height = 2,
            Hp=55,
            Image="",
            Weight=3
        };
        return pokemon;
    }

    private List<Pokemon> GetTestListPokemon()
    {
        return new List<Pokemon>()
        {
            new()
            {
                Id=1,
                Name = "pikachu",
                AttackPower = 35,
                Height = 2,
                Hp=55,
                Image = "",
                Weight = 3
            }
        };
    }
    
}