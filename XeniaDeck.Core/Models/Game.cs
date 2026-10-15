namespace XeniaDeck.Core.Models
{
    public record Game(uint TitleId, bool IsInGamesFolder, string FilePath, SimpleGameInfo SimpleGameInfo);
}
