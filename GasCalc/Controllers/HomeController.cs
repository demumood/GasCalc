// <copyright file="HomeController.cs" company="MAudu Productions">
// Copyright (c) MAudu Productions. All rights reserved.
// </copyright>

namespace GasCalc.Controllers
{
    using System.Diagnostics;
    using System.Diagnostics.Metrics;
    using System.Reflection;
    using AngleSharp;
    using AngleSharp.Dom;
    using GasCalc.Models;
    using GasCalc.Models.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using PuppeteerSharp;

    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// the logger.
        /// </summary>
        private readonly ILogger<HomeController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">the logger.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// The index page.
        /// </summary>
        /// <returns>the view of the index page.</returns>
        public IActionResult Index()
        {
            this.ViewBag.tankInformation = this.GetTankInformation();
            this.ViewBag.GasCalc = new GasCalcResult();
            return this.View();
        }

        /// <summary>
        /// The post of the index page.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>navigates to the result page with the data.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("TankSize,CurrentGasAmount,AmountToSpend,Zipcode,GasCalcResults")] GasClacViewModel data)
        {
            // check if the data sent over is valid.
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.tankInformation = this.GetTankInformation();
                return this.View();
            }

            // set up a headless browser.
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions { Headless = true });

            // create a new page, set it to gas buddy and run and fetc the html response.
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync($"https://www.gasbuddy.com/home?search={data.Zipcode}&fuel=1&method=all&maxAge=0");
            var content = await page.GetContentAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));

            // parse the html tags until you find the tag that has the stations informtation
            var stations = document.QuerySelectorAll("*").Where(e => e.LocalName == "div" && e.ClassName == "GenericStationListItem-module__stationListItem___3Jmn4").ToList();

            // loop through the result
            foreach (var s in stations)
            {
                // fetch the price first
                var price = s.QuerySelectorAll("*").FirstOrDefault(e => e.LocalName == "span" && e.ClassName == "text__xl___2MXGo text__left___1iOw3 StationDisplayPrice-module__price___3rARL");

                // if a price does not exist  skip it, else get the station name, address and when it was last updated.
                if (price != null && price.Text() != "- - -")
                {
                    var stationName = s.QuerySelectorAll("*").FirstOrDefault(e => e.LocalName == "h3" && e.ClassName == "header__header3___1b1oq header__header___1zII0 header__midnight___1tdCQ header__snug___lRSNK StationDisplay-module__stationNameHeader___1A2q8");

                    var stationAddress = s.QuerySelectorAll("*").FirstOrDefault(e => e.LocalName == "div" && e.ClassName == "StationDisplay-module__address___2_c7v");

                    var lastUpdated = s.QuerySelectorAll("*").FirstOrDefault(e => e.LocalName == "span" && e.ClassName == "ReportedBy-module__postedTime___J5H9Z");

                    // convert number for calculations
                    double priceAsDouble = double.Parse(price.Text().Substring(1));
                    var gasAdded = 0.0;

                    var galofGasBought = Math.Round(data.AmountToSpend / priceAsDouble, 1);
                    var approxCurrentGalofGas = data.TankSize * (data.CurrentGasAmount / 100.0);
                    gasAdded = ((galofGasBought + approxCurrentGalofGas) / (double)data.TankSize) * 100.0;

                    GasCalcResult gasCalcResult = new GasCalcResult()
                    {
                        StationName = stationName != null ? stationName.Text() : string.Empty,
                        StationAddress = stationAddress != null ? stationAddress.Text() : string.Empty,
                        GasPrice = price != null ? price.Text() : string.Empty,
                        AfterGasAmount = gasAdded.ToString(),
                    };

                    data.GasCalcResults.Add(gasCalcResult);
                }
            }

            this.TempData["data"] = data;

            return this.RedirectToAction("ResultPage");
        }

        public IActionResult ResultPage()
        {

            GasClacViewModel data = (GasClacViewModel)this.TempData["data"];

                return this.View(data);
        }

        /// <summary>
        /// The error class.
        /// </summary>
        /// <returns>the error page.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }


        private List<SelectListItem> GetTankInformation()
        {
            List<SelectListItem> progessBarData = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "100", Text = "Full Tank" },
                new SelectListItem() { Value = "75", Text = "Three Quarter Tank" },
                new SelectListItem() { Value = "50", Text = "Half Tank" },
                new SelectListItem() { Value = "25", Text = "Quarter Tank" },
                new SelectListItem() { Value = "0", Text = "Empty Tank" },
            };

            return progessBarData;
        }
    }
}