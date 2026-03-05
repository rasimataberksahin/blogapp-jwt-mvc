using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogApp.Data.Entities;
using BlogApp.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BlogApp.Mvc.Controllers
{
    [Authorize]
    // Bu attribute þunu yapar:
    // "Bu controller'a gelen her istekte kullanýcý authenticated olmalý."
    // Token yoksa/yanlýþsa Program.cs'teki JWT middleware 401 yerine /Auth/Login'e yönlendirir (OnChallenge).
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, DbContext dbContext)
        {
            _logger = logger;       // log basmak için (ders projesinde çok kullanýlmýyor ama gerçek projede önemli)
            _dbContext = dbContext; // Data katmanýndaki BlogAppDbContext burada DbContext olarak gelir
        }

        public async Task<IActionResult> Index()
        {
            // JWT'nin pratik kýsmý: kullanýcýyý "session'dan" deðil token'dan tanýyoruz.
            // AuthController token üretirken sub claim içine user.Id koymuþtu.
            // Burada o sub claim'i okuyup userId elde ediyoruz.
            var userId = GetUserId();

            // Son 10 blogu listele (herkese açýk liste gibi düþün)
            // Include(s => s.User): BlogPostEntity içindeki navigation property User'ý da getirir (JOIN gibi)
            // Select: Entity yerine ViewModel'e map ediyoruz (View'a sadece lazým olan gider)
            var blogs = await _dbContext.Set<BlogPostEntity>()
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .Take(10)
                .Select(s => new BlogTableViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Size = s.Content.Length, // içerik boyutu (örnek alan)
                    CreatedAt = s.CreatedAt
                }).ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> MyBlogs()
        {
            // Amaç: "sadece benim bloglarým" ekraný
            // Bunun için token'dan kendi userId'ni çekiyoruz.
            var userId = GetUserId();

            var blogs = await _dbContext.Set<BlogPostEntity>()
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .Take(10)
                .Where(s => s.Id == userId)
                // ÖNEMLÝ NOT:
                // Buradaki filtre büyük ihtimalle yanlýþ.
                // "Benim bloglarým" demek: BlogPostEntity.UserId == userId olmalý.
                // Þu anki hali: BlogPostEntity.Id == userId (blogun id'si user id'ye eþit mi?) gibi çalýþýr.
                // (Kod bozmuyorum, sadece mantýðý anlaman için not düþüyorum.)

                .Select(s => new BlogTableViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Size = s.Content.Length,
                    CreatedAt = s.CreatedAt
                }).ToListAsync();

            return View(blogs);
        }

        [HttpGet]
        public IActionResult AddBlog()
        {
            // Formu açar
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBlog([FromForm] NewBlogViewModel newBlogViewModel)
        {
            // Form validation (Required vs)
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Kritik güvenlik alýþkanlýðý:
            // UserId formdan alýnmaz. Çünkü kullanýcý html'i deðiþtirip baþka userId gönderebilir.
            // UserId her zaman token'dan okunmalý.
            var userId = GetUserId();

            var blogPost = new BlogPostEntity
            {
                Title = newBlogViewModel.Title,
                Content = newBlogViewModel.Content,
                UserId = userId,           // token'dan aldýðýmýz gerçek userId
                CreatedAt = DateTime.Now
            };

            _dbContext.Set<BlogPostEntity>().Add(blogPost);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail([FromRoute] long id)
        {
            // Bu action route'dan id alýr: /Home/Detail/5 gibi
            // [FromRoute] uzun id parametresi URL'den geldiðini netleþtirir.

            // --------ilk hali (hatalý)---------//
            // Bu yaklaþým þunu yapýyordu: "Sadece kendi blogumu görebileyim."
            // Güvenlik için mantýklý olabilir ama burada ders örneðinde "her blog detayý açýlsýn" istenmiþ olabilir.

            //var userId = GetUserId();

            //var blog = await _dbContext.Set<BlogPostEntity>()
            //    .Include(s => s.User)
            //    .SingleOrDefaultAsync(s => s.UserId == userId && s.Id == id);


            // -------- yeni hali -------- //
            // Sadece id ile blogu getiriyor. Yani blog kimde olursa olsun detay açýlýr.
            // Eðer ileride "sadece kendi blogumun detayýný göreyim" istersen eski hal mantýklýydý.

            var blog = await _dbContext.Set<BlogPostEntity>()
                .Include(s => s.User)
                .SingleOrDefaultAsync(s => s.Id == id);

            return View(blog);
        }

        public long GetUserId()
        {
            // Bu projenin JWT açýsýndan en kritik satýrý burasý.
            // Authentication middleware token'ý doðrularsa HttpContext.User dolar.
            // Buradan claim okuyabiliriz.

            // JwtRegisteredClaimNames.Sub = "sub"
            // AuthController token üretirken sub içine user.Id koymuþtu.
            // O yüzden burada userId çekiyoruz.

            return long.Parse(
                User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? throw new InvalidOperationException()
            );
        }
    }
}