// <copyright file="HomeController.cs" company="MAudu Productions">
// Copyright (c) MAudu Productions. All rights reserved.
// </copyright>

namespace GasCalc.Controllers
{
    using System.Diagnostics;
    using GasCalc.Models;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}