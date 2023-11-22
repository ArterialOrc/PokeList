using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TestsPokeAPI;

public class UITests : IDisposable
{
    private readonly IWebDriver _driver;
    public UITests()
    {
        _driver = new ChromeDriver("C:/ChromeDriver");
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
    
    [Fact]
    public void LoadingCards()
    {
        _driver.Navigate()
            .GoToUrl("http://localhost/index");
        try
        {
            var wait = new WebDriverWait(_driver,  TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementExists(By.ClassName("card-title")));
        }
        catch (Exception e)
        {
            Console.WriteLine("Element with locator: card-title was not found in current context page.");
            throw;
        }
        var text = _driver.FindElement(By.ClassName("card-title")).Text;

        Assert.Contains("saur", text);
    }
    
    [Fact]
    public void Search()
    {
        _driver.Navigate()
            .GoToUrl("http://localhost/index");
        try
        {
            var wait = new WebDriverWait(_driver,  TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementExists(By.ClassName("card-title")));
        }
        catch (Exception e)
        {
            Console.WriteLine("Element with locator: card-title was not found in current context page.");
            throw;
        }
        _driver.FindElement(By.ClassName("search__btn")).SendKeys("pikachu");
        try
        {
            var wait = new WebDriverWait(_driver,  TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementExists(By.ClassName("card-title")));
        }
        catch (Exception e)
        {
            Console.WriteLine("Element with locator: card-title was not found in current context page.");
            throw;
        }
        var text = _driver.FindElement(By.ClassName("card-title")).Text;

        Assert.Contains("pikachu", text);
    }
    
    [Fact]
    public void CardPokemon()
    {
        _driver.Navigate()
            .GoToUrl("http://localhost/info?poke=1");
        try
        {
            var wait = new WebDriverWait(_driver,  TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementExists(By.ClassName("card-title")));
        }
        catch (Exception e)
        {
            Console.WriteLine("Element with locator: card-title was not found in current context page.");
            throw;
        }
        var text = _driver.FindElement(By.ClassName("card-title")).Text;

        Assert.Equal("bulbasaur", text);
    }
    
    [Fact]
    public void TurnInFight()
    {
        _driver.Navigate()
            .GoToUrl("http://localhost/fight?myPoke=2&enemyPoke=413");
        try
        {
            var wait = new WebDriverWait(_driver,  TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementExists(By.ClassName("card-title")));
        }
        catch (Exception e)
        {
            Console.WriteLine("Element with locator: card-title was not found in current context page.");
            throw;
        }
        
        _driver.FindElement(By.ClassName("attack__btn")).Click();
        
        var text = _driver.FindElement(By.ClassName("fight__history")).FindElement(By.TagName("div")).Text;

        Assert.Contains("Удар нанёс", text);
    }
}