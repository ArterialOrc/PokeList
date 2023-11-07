namespace PokeAPI.Models;

public class Email
{
    public class Statistic
    {
        public string winPoke { get; set; }
        public string losPoke { get; set; }
    }
    public string email { get; set; }
    public Statistic statistic { get; set; }
}