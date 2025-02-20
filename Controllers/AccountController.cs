using BlogWebApp.Data;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======== LOGIN ========
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // 승인 여부 확인
            if (!user.IsApproved)
            {
                ModelState.AddModelError("", "Your account is not approved yet. Please wait for admin approval.");
                return View(model);
            }

            // 로그인 성공
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
        }


        // ======== REGISTER ========
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            Console.WriteLine("Register attempt received.");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(model);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Email is already in use.");
                return View(model);
            }

            // 비밀번호 해시 자동 생성
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            Console.WriteLine($"Generated Hashed Password: {hashedPassword}");

            var user = new User
            {
                Username = model.Username,
                Password = hashedPassword, // 해시된 비밀번호 저장
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Console.WriteLine($"User registered successfully: {user.Username}");
            return RedirectToAction("Login");
        }

        // ======== Logout ========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Console.WriteLine("User logged out.");

            // 세션 삭제
            HttpContext.Session.Clear();

            // 로그인 페이지로 리다이렉트
            return RedirectToAction("Login", "Account");
        }

    }
}
