using BlogWebApp.Data;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles.ToListAsync();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin" || role == "Contributor")
            {
                return View();
            }
            return Unauthorized();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article)
        {
            if (!ModelState.IsValid)
            {
                return View(article);
            }

            article.ContributorUsername = HttpContext.Session.GetString("Username");
            article.CreateDate = DateTime.Now;

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("Role") != "Admin" && HttpContext.Session.GetString("Username") != article.ContributorUsername)
            {
                return Unauthorized();
            }

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Article updatedArticle)
        {
            if (id != updatedArticle.ArticleId)
            {
                return BadRequest();
            }

            var existingArticle = await _context.Articles.FindAsync(id);
            if (existingArticle == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("Role") != "Admin" && HttpContext.Session.GetString("Username") != existingArticle.ContributorUsername)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                existingArticle.Title = updatedArticle.Title;
                existingArticle.Body = updatedArticle.Body;
                existingArticle.StartDate = updatedArticle.StartDate;
                existingArticle.EndDate = updatedArticle.EndDate;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(updatedArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("Role") != "Admin" && HttpContext.Session.GetString("Username") != article.ContributorUsername)
            {
                return Unauthorized();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
