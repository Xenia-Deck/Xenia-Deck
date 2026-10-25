namespace XeniaManagerDatabase.Models
{
    public record GameInfo
    (
        string Id,
        Title Title,
        string[] Genre,
        string Developer,
        string Publisher,
        string ReleaseDate,
        string UserRating,
        Description Description,
        string[] Media,
        Artwork Artwork,
        Products Products
    );

    public record Title(string Full, string Reduced);
    public record Description(string Full, string Short);
    public record Artwork
    (
        string Background,
        string Banner,
        string Boxart,
        string Icon,
        string[] Gallery
    );

    public record Products(string[] Parent, string[] Related);
}