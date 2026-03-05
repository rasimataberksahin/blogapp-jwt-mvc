using BlogApp.Data.Entities;
using BlogApp.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogApp.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _config;

        public AuthController(DbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext; // DataExtensions'ta DbContext -> BlogAppDbContext olarak kaydedildiği için burada onu alıyoruz.
            _config = config;       // appsettings.json içindeki Jwt:Secret gibi config değerlerini okumak için.
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(); // sadece login sayfasını açar (form burada)
        }

        [HttpPost]
        public IActionResult Login([FromForm] LoginViewModel loginViewModel)
        {
            // ViewModel validation: Email/Password boş mu, minLength vs.
            if (!ModelState.IsValid)
            {
                return View();
            }

            // kullanıcı var mı kontrolü (ders için basit hali: email+password DB'de eşleşiyor mu)
            var dbSet = _dbContext.Set<UserEntity>();
            var user = dbSet.FirstOrDefault(u => u.Email == loginViewModel.Email && u.Password == loginViewModel.Password);

            if (user is null)
            {
                ViewBag.Error = "Invalid Credentials!";
                return View();
            }



            // BURADAN SONRASI "LOGIN BALARILI" KISMI:
            // kullanıcıyı session'a yazmıyoruz. JWT üretip kullanıcıya veriyoruz.
            // sonraki requestlerde kullanıcı bu token ile kimliğini taşıyacak (stateless mantık)

            var claims = new List<Claim>
            {
                // claim = token içine koyduğumuz bilgi
                // Email claim'i: kullanıcıyı tanımlamak/ekranda göstermek vs için kullanılabilir
                new Claim(JwtRegisteredClaimNames.Email , user.Email),

                // sub (subject) = kullanıcı id'si gibi düşün
                // projede HomeController.GetUserId() buradan userId çekiyor.
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            // Secret key: token imzasını üretmek için kullanılır.
            // Program.cs doğrulama yaparken de aynı secret ile "signature doğru mu?" kontrol ediyor.
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));

            // oluşturulacak token'ın ayarları
            var tokenoptions = new JwtSecurityToken(
                issuer: "BlogApp",    // Program.cs -> ValidIssuer ile aynı olmalı yoksa token geçmez
                audience: "BlogApp",  // Program.cs -> ValidAudience ile aynı olmalı yoksa token geçmez
                claims: claims,       // token payload (kimlik bilgileri)
                expires: DateTime.Now.AddMinutes(5), // token ömrü: süresi dolunca login gerekir (ValidateLifetime bunu kontrol eder)
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
                // signingCredentials: token'ı imzalar (signature üretir). Bu imza token'ın değişmediğini garanti eder.
                );

            // token oluştur!
            // JwtSecurityToken -> string JWT formatına çevrilir: header.payload.signature
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenoptions);

            // oluşturulan bu token Cookie içerisine eklensin.
            // Not: Normalde JWT header ile taşınır (Authorization: Bearer ...)
            // Bu projede tarayıcı uygulaması gibi davransın diye cookie içine koyulmuş.

            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true, // js ile erişilemesi
                // js okuyamasın: XSS riskini azaltır (token çalınmasın diye)

                Secure = true, // https ile kullanılabilsin
                // sadece HTTPS üzerinden gönderilir (prod için iyi)

                SameSite = SameSiteMode.Strict // sadece bu sitede(uygulamada) kullanılabilsin. 
                // başka site üzerinden otomatik gönderilmesini engeller (CSRF riskini azaltır)
            });

            return RedirectToAction("Index", "Home"); // login tamam -> Home'a git
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // JWT stateless: server tarafında session kapatma yok.
            // logout = client'taki token'ı silmek.
            Response.Cookies.Delete("access_token");

            return RedirectToAction("Login", "Auth");
        }
    }
}