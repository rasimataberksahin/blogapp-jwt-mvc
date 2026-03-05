using BlogApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();//mvc çalýþsýn. controllwe+view pipeline'ý açar.

var connectionString = builder.Configuration.GetConnectionString("Default");

// DB Servisi ekleme iþi;
// AddBlogData -> Data katmanýndaki extension. Program.cs kalabalýk olmasýn diye oraya taþýndý.
builder.Services.AddBlogData(connectionString);


// Jwt servisi ekleme iþi;
//authentication sistemini açýyoruz. varsayýlan þema jwt bearer olacak.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // birisi token ile istekte bulunduðu zaman, token'ýn kontrol edilme kurallarý (validation)
        //yani (jwt doðrulama kurallarý:"gelen token kabul edilecek mi?")
        // Buradaki kurallar yanlýþsa: tokenlar ya gereksiz reddedilir ya da güvenlik açýðý olur.

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            // issuer: token'ý kim üretti?
            ValidateIssuer = true,
            ValidIssuer = "BlogApp",

            // audience: token hangi uygulama için üretildi?
            ValidateAudience = true,
            ValidAudience = "BlogApp",

            // exp/nbf: token süresi geçerli mi?
            ValidateLifetime = true,

            // signature: token ile oynandý mý? imza doðru mu?
            ValidateIssuerSigningKey = true,

            // Secret: token üretirken kullanýlan anahtarýn aynýsý burada olmalý.
            // AuthController token üretirken de ayný Jwt:Secret kullanýyor.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };

        // istek yapýldýðýnda Token'ýn nerde bulunacaðý söylenmeli.
        // Normalde JWT header'dan okunur: Authorization: Bearer xxx
        // Ama bu projede token Cookie'de saklanýyor: access_token
        // O yüzden "token nerede?" sorusunun cevabýný burada veriyoruz.

        options.Events = new JwtBearerEvents
        {
            // OnMessageReceived : Her istek geldiðinde Token'ý cookie'den çekip middleware'e veriyoruz.
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["access_token"];

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    context.Token = accessToken;//context.token set edilince doðrulama otomatik baþlar.(issuer/audience/exp/signature)
                }

                return Task.CompletedTask;
            },

            // LoginPath için;
            // Normal JWT davranýþý: token yoksa 401 döner.
            // MVC'de daha iyi UX: 401 yerine login sayfasýna gönder.

            OnChallenge = async context =>
            {
                // varsayýlan zorlama davranýþýný engelle
                context.HandleResponse();//default 401 davranýþýný iptal et.

                context.Response.Redirect("/Auth/Login");//login sayfasýna yönlendir.

                await Task.CompletedTask;
            }

        };


        //Claim isimleri otomatik maplenmesin diye(sub, email gibi claim'ler aynen kalsýn.)
        options.MapInboundClaims = false;

    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())//prod ortamda hata sayfaasý + HSFS
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();//wwwroot içeriði içeriðini yayýnlar.(css/js/bootstrap vs)

app.UseRouting();


// Sýra önemli:
// UseAuthentication -> token doðrulanýr, User.Identity dolar
app.UseAuthentication();

// UseAuthorization -> [Authorize] kontrol edilir
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//Uygulama ilk kez ayaða kalkýnca DB yoksa oluþtur + seed bas.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DbContext>();

    await context.EnsureCreatedAndSeedAsync();
}


app.Run();
