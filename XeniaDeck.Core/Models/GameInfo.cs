namespace XeniaDeck.Core.Models
{
    public record GameInfo
    (
        string Id = "",
        Title Title = null!,
        string[] Genre = null!,
        string Developer = "",
        string Publisher = "",
        string ReleaseDate = "",
        string UserRating = "",
        Description Description = null!,
        string[] Media = null!,
        Artwork Artwork = null!,
        Products Products = null!
    )
    {
        public Title Title { get; init; } = Title ?? new();
        public string[] Genre { get; init; } = Genre ?? [];
        public Description Description { get; init; } = Description ?? new();
        public string[] Media { get; init; } = Media ?? [];
        public Artwork Artwork { get; init; } = Artwork ?? new();
        public Products Products { get; init; } = Products ?? new();
    }

    public record Title(string Full = "", string Reduced = "");
    public record Description(string Full = "", string Short = "");
    public record Artwork
    (
        string Background = "",
        string Banner = "",
        string Boxart = "",
        string Icon = "",
        string[] Gallery = null!)
    {
        public string[] Gallery { get; init; } = Gallery ?? [];
    };

    public record Products(string[] Parent = null!, string[] Related = null!)
    {
        public string[] Parent { get; init; } = Parent ?? [];
        public string[] Related { get; init; } = Related ?? [];
    };
}