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

        // 📌 1. 글 목록 (Articles Index)
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles.ToListAsync();
            return View(articles);
        }

        // 📌 2. 글 작성 (Create GET)
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin" || role == "Contributor")
            {
                return View();
            }
            return Unauthorized();  // 🚨 권한 없으면 Unauthorized 반환
        }

        // 📌 3. 글 작성 (Create POST)
        [HttpPost]
        [ValidateAntiForgeryToken]  // 🚨 CSRF 공격 방지
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

        // 📌 4. 글 수정 (Edit GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Admin은 모든 글 수정 가능, Contributor는 본인 글만 가능
            if (HttpContext.Session.GetString("Role") != "Admin" && HttpContext.Session.GetString("Username") != article.ContributorUsername)
            {
                return Unauthorized();
            }

            return View(article);
        }

        // 📌 5. 글 수정 (Edit POST)
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

            // Admin은 모든 글 수정 가능, Contributor는 본인 글만 가능
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

        // 📌 6. 글 삭제 (Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Admin은 모든 글 삭제 가능, Contributor는 본인 글만 가능
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
