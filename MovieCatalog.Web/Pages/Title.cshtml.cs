using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;

namespace MovieCatalog.Web.Pages
{
    public class TitleModel : PageModel
    {
        private readonly IMovieCatalogDataService _dataService;

        public TitleModel(IMovieCatalogDataService dataService)
        {
            _dataService = dataService;
        }
        [BindProperty]
        public Title Title { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        public List<TitleGenre> SelectedGenres { get; set; }
        public List<SelectListItem>? Genres { get; set; }
        public int? Id { get; set; }

        public async Task OnGetAsync(int? id)
        {
            Genres = await GetGenresAsync();

           
            if (id.HasValue)
            {
                Title = await _dataService.GetTitleByIdAsync(id.Value);
                var genreIDs = await _dataService.GetGenreById(id.Value);
               foreach (var genre in genreIDs)
                {
                   for (int i = 0; i < Genres.Count; i++)
                    {
                        if (Genres[i].Value == genre.ToString())
                        {
                            Genres[i].Selected = true;
                        }
                    }
             
                }
                Id = id.Value;
            }
            else
            {
                Title = new Title("", "");

                SelectedGenres = new List<TitleGenre>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {   
                Genres = await GetGenresAsync();
                return Page();  
            }
            
            try
            {
                Id = Convert.ToInt32(RouteData.Values["id"]);
                if (Id == 0)
                {
                    Id = null;
                }

                var a  = Request.Form["Genres"].Select(int.Parse);
                foreach (var b in a)
                {
                    for (int i = 0; i < Genres.Count; i++)
                    {
                        if (b.ToString() == Genres[i].Value)
                        {
                            Genres[i].Selected = true;
                        }
                    }
                }
                var title = await _dataService.InsertOrUpdateTitleAsync(
                         Id, Title.PrimaryTitle, Title.OriginalTitle, Title.TitleType,
                         Title.StartYear, Title.EndYear, Title.RuntimeMinutes, null);

                SuccessMessage = "Title saved successfully";
                return RedirectToPage($"/Title", new { title.Id });

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                Genres = await GetGenresAsync();
                return Page();
            }
        }

        public async Task<List<SelectListItem>> GetGenresAsync()
        {
            var genreOptions = new List<SelectListItem>();

            var genres = await _dataService.GetGenresAsync();

            foreach (var genre in genres)
            {
                genreOptions.Add(new SelectListItem
                {
                    Value = genre.Id.ToString(),
                    Text = genre.Name
                });
            }

            return genreOptions;
        }
    }
}
