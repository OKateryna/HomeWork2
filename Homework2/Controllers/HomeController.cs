using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Homework2.Models;
using Homework2.Services;

namespace Homework2.Controllers
{
    public class HomeController : Controller
    {
        private EntitiesService entitiesService;
        public HomeController()
        {
            this.entitiesService = new EntitiesService();
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            var allUsers = entitiesService.AllUsers;
            return View(allUsers);
        }

        public IActionResult Comments()
        {
            var allComments = entitiesService.AllComments;
            return View(allComments);
        }

        public IActionResult Posts()
        {
            var allPosts = entitiesService.AllPosts;
            return View(allPosts);
        }

        public IActionResult Todos()
        {
            var allTodos = entitiesService.AllTodos;
            return View(allTodos);
        }

        public IActionResult Addresses()
        {
            var allAddresses = entitiesService.AllAddresses;
            return View(allAddresses);
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
