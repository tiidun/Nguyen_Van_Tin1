using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using URLShortener.Infrastructure;
using URLShortener.Models;
using URLShortener.Models.Home;
using URLShortener.Models.URLAddresses;

using URLShortenerData.Data;
using URLShortenerData.Data.Entities;

namespace URLShortenerData.Controllers
{
    public class HomeController : Controller
    {
        private readonly URLShortenerDbContext data; // Dependency injection for the database context

        public HomeController(URLShortenerDbContext data) // Constructor to initialize the database context
        {
            this.data = data;
        }

        public IActionResult Index(int? pageNumber) // Action method for the Index page
        {
            // Create a model to store all the data required for the Index page
            IndexViewModel allIndexInfo = new IndexViewModel()
            {
                TotalShortURLsCount = this.data.URLAddresses.Count(), // Total number of short URLs
                TotalURLVisitorsCount = this.GetAllUrlsVisitsCount(), // Total number of URL visits
                URLAddresses = this.data.URLAddresses // Fetch the latest 3 URLs for display
                    .Select(x => new URLAddressViewModel
                    {
                        Id = x.Id,
                        OriginalUrl = x.OriginalUrl,
                        ShortUrl = x.ShortUrl,
                        DateCreated = x.DateCreated,
                        Visits = x.Visits
                    })
                    .OrderByDescending(x => x.DateCreated)
                    .Take(3)
                    .ToList()
            };

            // Check if the user is authenticated
            if (this.User.Identity is not null && this.User.Identity.IsAuthenticated)
            {
                // Fetch URLs associated with the logged-in user
                allIndexInfo.URLAddresses = this.data.URLAddresses
                    .Where(x => x.UserId == this.User.UserId())
                    .Select(x => new URLAddressViewModel
                    {
                        Id = x.Id,
                        OriginalUrl = x.OriginalUrl,
                        ShortUrl = x.ShortUrl,
                        DateCreated = x.DateCreated,
                        Visits = x.Visits
                    })
                    .ToList();

                // Set additional user-specific statistics
                allIndexInfo.MyShortURLsCount = allIndexInfo.URLAddresses.Count(); // Number of URLs created by the user
                allIndexInfo.MyTotalURLVisitorsCount = this.GetMyUrlsVisistsCount(allIndexInfo); // Total visits for the user's URLs
            };

            // Pagination setup
            allIndexInfo.PageIndex = pageNumber ?? 1; // Default to page 1 if no page number is provided
            allIndexInfo.TotalPages = (int)Math.Ceiling((decimal)allIndexInfo.MyShortURLsCount / allIndexInfo.URLsPerPage); // Calculate total pages

            // Fetch URLs for the current page
            allIndexInfo.URLAddresses = allIndexInfo.URLAddresses
                .Skip((allIndexInfo.PageIndex - 1) * allIndexInfo.URLsPerPage)
                .Take(allIndexInfo.URLsPerPage)
                .ToList();

            return this.View(allIndexInfo); // Pass the data to the view
        }

        private int GetMyUrlsVisistsCount(IndexViewModel allIndexInfo) // Calculate total visits for user's URLs
        {
            int myTotalVisits = 0;
            foreach (URLAddressViewModel urlAddress in allIndexInfo.URLAddresses!)
            {
                myTotalVisits += urlAddress.Visits; // Sum up visits for each URL
            }

            return myTotalVisits;
        }

        private int GetAllUrlsVisitsCount() // Calculate total visits for all URLs
        {
            int countAllVisits = 0;
            foreach (URLAddress address in this.data.URLAddresses)
            {
                countAllVisits += address.Visits; // Sum up visits for each URL
            }

            return countAllVisits;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Prevent caching of the error page
        public IActionResult Error() => this.View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier // Display the current activity ID or trace identifier
        });
    }
}
