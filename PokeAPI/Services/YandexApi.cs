namespace PokeAPI.Services
{
    public class YandexApi
    {
        public string ClientId { get; init; }
        public string RedirectUri { get; init; }
        public string TokenPageOrigin { get; init; }

        public string ResponseType { get; private set; } = "token";
    }
}
