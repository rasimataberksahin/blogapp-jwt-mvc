using BlogApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data
{
    public static class DataExtensions
    {
        // Bu extension method Program.cs içinde çağrılır.
        // Amaç: DbContext kayıt işlemini Program.cs içinde kalabalık yapmamak.
        //program.cs kalabalık olursa bakım(maintainability) zorlaiır ve başka biri girdiğinde anlamayabilir. Modülerlik(single responsibility(bir dosya tek iş yapmalı)) kaybolur.
        //test zorlaşır. reusability azalır.
        //yani aslında program.cs in tek temel görevi programı başlatmaktır.

        public static IServiceCollection AddBlogData(this IServiceCollection services, string connectionString)
        {
            // DbContext dependency injection'a eklenir.
            // Uygulama içinde DbContext istendiğinde BlogAppDbContext kullanılır.

            services.AddDbContext<DbContext, BlogAppDbContext>(options =>
            {
                // SQL Server kullanacağımız belirtilir
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        // Uygulama ilk çalıştığında veritabanı yoksa oluşturulur
        // ve içine örnek veriler (seed data) eklenir.

        public static async Task EnsureCreatedAndSeedAsync(this DbContext context)
        {

            if (await context.Database.EnsureCreatedAsync())//veritabanı yoksa oluştur. eğer yeni oluşturulduysa true dönecek.
            {
                // seed
                // Veritabanına başlangıç verileri ekleniyor.

                var user1 = new UserEntity
                {
                    Email = "user1@gmail.com",
                    Password = "1234"
                };

                var user2 = new UserEntity
                {
                    Email = "user2@gmail.com",
                    Password = "1234"
                };

                var user3 = new UserEntity
                {
                    Email = "user3@gmail.com",
                    Password = "1234"
                };

                // Users tablosuna kullanıcılar ekleniyor

                context.Set<UserEntity>().AddRange(user1, user2, user3);

                await context.SaveChangesAsync();

                // İlk blog postu ekleniyor

                var post1 = new BlogPostEntity
                {
                    // user1'in Id değeri kullanılıyor
                    UserId = user1.Id,

                    Title = "First Post",

                    // Html içerik olduğu için daha sonra view içinde Html.Raw ile gösterilebilir
                    Content = "<h1>This is the first post</h1>",

                    CreatedAt = DateTime.Now
                };

                // BlogPosts tablosuna ekleme

                context.Set<BlogPostEntity>().AddRange(post1);

                await context.SaveChangesAsync();
            }
        }
    }
}