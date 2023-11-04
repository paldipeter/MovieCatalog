using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCatalog.Web.Utils;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;

namespace MovieCatalog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private IMovieCatalogDataService _movieCatalogDataService;
        private readonly ILogger<IndexModel> _logger;
        public Dictionary<Genre, int> GenresWithCount;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public TitleSort TitleSort { get; set; } = TitleSort.ReleaseYear;
        [BindProperty(SupportsGet = true)]
        public bool SortDescending { get; set; } = true;

        public PagedResult<Title> TitleList { get; set; }

        [BindProperty(SupportsGet = true)]
        public TitleFilter Filter { get; set; } = TitleFilter.Empty;


        public SelectListItem[] PageSizeOptions { get; } = {
        new SelectListItem { Text = "10", Value = "10" },
        new SelectListItem { Text = "20", Value = "20" },
        new SelectListItem { Text = "30", Value = "30" },
        new SelectListItem { Text = "60", Value = "60" },
        new SelectListItem { Text = "120", Value = "120" },
    };

        public SelectListItem[] TitleSortOptions { get; } = {
        new SelectListItem { Text = "Release Year", Value = TitleSort.ReleaseYear.ToString()},
        new SelectListItem { Text = "Title", Value = TitleSort.PrimaryTitle.ToString() },
        new SelectListItem { Text = "Runtime", Value = TitleSort.Runtime.ToString()},
    };

        public SelectListItem[] SortDirectionOptions { get; } = {
        new SelectListItem { Text = "Ascending", Value = "true"},
        new SelectListItem { Text = "Descending", Value = "false" },
    };

        public IReadOnlyList<int> PageNumberOptions =>
            new[]{
            1, 2, 3, PageNumber - 1, PageNumber,    PageNumber + 1, TitleList.LastPageNumber -    1,
            TitleList.LastPageNumber, TitleList.LastPageNumber + 1
        }
        .Where(i => i > 0 && i <= TitleList.LastPageNumber + 1)
        .Distinct()
        .OrderBy(i => i)
        .ToArray();


        public List<Title> Titles { get; set; }
        

        public IndexModel(ILogger<IndexModel> logger, IMovieCatalogDataService movieservice)
        {
            _logger = logger;
            _movieCatalogDataService = movieservice;
        }

        public async Task<ActionResult> OnGetAsync()
        {

            if (string.IsNullOrWhiteSpace(Request.QueryString.Value))
            {
                return RedirectToPage("/Index", new
                {
                    PageSize = 20,
                    PageNumber = 1,
                    TitleSort = TitleSort.ReleaseYear,
                    SortDescending = true
                });
            }
            GenresWithCount = await _movieCatalogDataService.GetGenresWithTitleCountsAsync();

            TitleList = await _movieCatalogDataService.GetTitlesAsync(PageSize, PageNumber,
               Filter,
               TitleSort, SortDescending);
            Titles = TitleList.Results.ToList();
            return Page();
        }
    }
}