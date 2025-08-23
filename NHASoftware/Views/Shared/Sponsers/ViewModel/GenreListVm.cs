using AutoMapper.Configuration;

namespace NHA.Website.Software.Views.Shared.Sponsers.ViewModel
{
    public class GenreListVm
    {
        public string Genres { get; set; } = string.Empty;
        public GenreListType Type = GenreListType.Anime;

        public GenreListVm(string genres, GenreListType type)
        {
            Type = type;
            Genres = genres;
        }
    }

    public enum GenreListType
    {
        Anime,
        Game,
    }
}
