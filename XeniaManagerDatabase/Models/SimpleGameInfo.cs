namespace XeniaManagerDatabase.Models
{
    public record SimpleGameInfo
    (
        string Id,
        string[] AlternativeId,
        string Title,
        string Boxart,
        string[] MediaId,
        string[] Genre,
        string UserRating,
        string Developer,
        string Publisher,
        string ReleaseDate
    );
}