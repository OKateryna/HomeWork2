using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Homework2.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Homework2.Controllers
{
    public class TasksController : Controller
    {
        private EntitiesService entitiesService;

        public TasksController()
        {
            entitiesService = new EntitiesService();
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Task1()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Task1([FromForm]int number)
        {
            var postCommentCount = entitiesService.GetCommentCountForUser(number);
            ViewBag.UserId = number;

            return View(postCommentCount);
        }

        public IActionResult Task2()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Task2([FromForm]int number)
        {
            var listcom = entitiesService.GetShortCommentsForUser(number);
            ViewBag.UserId = number;

            return View(listcom);
        }

        public IActionResult Task3()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Task3([FromForm]int number)
        {
            var listidname = entitiesService.GetIncompleteTodosForUser(number);
            ViewBag.UserId = number;

            return View(listidname);
        }
        
        public IActionResult Task4()
        {
            var userList = entitiesService.SortUsersByNameAndTodoNameLenght();

            return View(userList);
        }

        public IActionResult Task5()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Task5([FromForm]int number)
        {
            var currentUser = entitiesService.GetFilteredInfo(number);
            ViewBag.UserId = number;

            return View(currentUser);
        }

        public IActionResult Task6()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Task6([FromForm]int number)
        {
            var post = entitiesService.GetPostInfo(number);
            ViewBag.UserId = number;

            return View(post);
        }
    }
}