namespace XeniaDeck.Core.Models
{
    public record SimpleGameInfo
    (
        string Id = "",
        string[] AlternativeId = null!,
        string Title = "",
        string Boxart = "",
        string[] MediaId = null!,
        string[] Genre = null!,
        string UserRating = "",
        string Developer = "",
        string Publisher = "",
        string ReleaseDate = ""
    )
    {
        public string[] AlternativeId { get; init; } = AlternativeId ?? [];
        public string[] MediaId { get; init; } = MediaId ?? [];
        public string[] Genre { get; init; } = Genre ?? [];
    };
}