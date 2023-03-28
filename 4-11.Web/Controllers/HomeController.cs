using _4_11.Data;
using _4_11.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _4_11.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            IndexViewModel vm = new();
            var repo = new QARepository(_connectionString);
            vm.Questions = repo.GetQuestions();
            return View(vm);
        }
        public IActionResult ViewQuestion(int id)
        {
            ViewQuestionViewModel vm = new();
            var repo = new QARepository(_connectionString);
            vm.Users = repo.GetUsers();
            vm.Question = repo.GetQuestionById(id);
            vm.IsAuthenticated = User.Identity.IsAuthenticated;
            string email = User.Identity.Name;
            User currentUser = repo.GetByEmail(email);
            vm.CurrentUser = currentUser;
            vm.Likes = repo.GetLikesByQuestion(id);
            return View(vm);
        }
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public void AddLike(int id)
        {

            var repo = new QARepository(_connectionString);
            Question question = repo.GetQuestionById(id);
            string email = User.Identity.Name;
            User currentUser = repo.GetByEmail(email);
            if (question.Likes.Select(l => l.UserId).Contains(currentUser.Id))
            {
                return;
            }
            Like like = new();
            like.UserId = currentUser.Id;
            like.QuestionId = id;
            repo.AddLike(like);
        }
       
        public string GetLikes(int id)
        {
            var repo = new QARepository(_connectionString);
            return repo.GetQuestionById(id).Likes.Count.ToString();

        }
        [Authorize]
        public IActionResult AddQuestion()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult Add(Question question, List<Tag> tags)
        //{
        //    var repo = new QARepository(_connectionString);
        //    question.DatePosted = DateTime.Now;
        //    question.UserId = 1;
        //    repo.AddQuestion(question);
        //    repo.AddTagsForQuestion(question, tags);
        //    return Redirect("/");
        //}
        [Authorize]
        [HttpPost]
        public IActionResult Add(Question question, List<string> tags)
        {
            var repo = new QARepository(_connectionString);
            question.DatePosted = DateTime.Now;
            string email = User.Identity.Name;
            User currentUser = repo.GetByEmail(email);
            question.UserId = currentUser.Id;
            repo.AddQuestion(question, tags);
            return Redirect("/");
        }
        [Authorize]
        public IActionResult AddAnswer(Answer answer)
        {
            var repo = new QARepository(_connectionString);
            string email = User.Identity.Name;
            User currentUser = repo.GetByEmail(email);
            answer.UserId = currentUser.Id;
            answer.Date = DateTime.Now;
            repo.AddAnswer(answer);
            return Redirect($"/Home/ViewQuestion?id={answer.QuestionId}");
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new QARepository(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login!";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {

                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/index");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
        public IActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddUser(User user, string password)
        {
            var repo = new QARepository(_connectionString);
            repo.AddUser(user, password);
            var claims = new List<Claim>
            {
                new Claim("user", user.Email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/home/index");
        }

    }
}
