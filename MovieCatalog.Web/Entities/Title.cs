using System.ComponentModel.DataAnnotations;

namespace MovieCatalogApi.Entities;

public enum TitleType
{
    Unknown, Short, Movie, TvMovie, TvSeries, TvEpisode, TvShort, TvMiniSeries, TvSpecial, Video, VideoGame, TvPilot
}

public class Title
{
    public int Id { get; set; }

    [StringLength(500, ErrorMessage = "PrimaryTitle cannot exceed 500 characters.")]
    public string PrimaryTitle { get; set; }

    [StringLength(500, ErrorMessage = "PrimaryTitle cannot exceed 500 characters.")]
    public string OriginalTitle { get; set; }
    [Range(1900, 2100, ErrorMessage = "StartYear must be between 1900 and 2100.")]
    public int? StartYear { get; set; }
    [Range(1900, 2100, ErrorMessage = "EndYear must be between 1900 and 2100.")]
    public int? EndYear { get; set; }
    
    [Range(1, 9999, ErrorMessage = "RuntimeMinutes must be between 1 and 9999.")]
    public int? RuntimeMinutes { get; set; }

    public TitleType TitleType { get; set; }
    [MaxLength(3, ErrorMessage = "TitleGenres must contain at most 3 elements.")]
    public ICollection<TitleGenre> TitleGenres { get; set; }
        = new List<TitleGenre>();
    public override string ToString() => $"Title {Id}: {TitleType} - {PrimaryTitle} ({OriginalTitle}, [{StartYear?.ToString() ?? "?"}{(EndYear != null ? $"-{EndYear}" : "")}]{($"<{RuntimeMinutes} min>" )}{(TitleGenres.Any() ? $" - {string.Join(", ", TitleGenres.Select(g => $"{g.Genre.Name}"))}" : string.Empty)}";
    public Title(string primaryTitle, string originalTitle)
    {
        PrimaryTitle = primaryTitle;
        OriginalTitle = originalTitle;
    }
    public Title()
    {

    }
}