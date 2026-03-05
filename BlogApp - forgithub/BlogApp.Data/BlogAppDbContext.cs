using BlogApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data
{
    // DbContext: EF Core'un veritabanı ile iletişim kurduğu ana sınıftır.
    // Veritabanındaki tablolar burada DbSet olarak tanımlanır.

    internal class BlogAppDbContext : DbContext
    {
        // Constructor: yapıcı metod
        // Program.cs tarafında AddDbContext ile verilen ayarlar (connection string vb.)
        // DbContext'e bu constructor üzerinden gelir.

        public BlogAppDbContext(DbContextOptions<BlogAppDbContext> options) : base(options)
        {
        }

        // DbSet<UserEntity>
        // Veritabanındaki Users tablosunu temsil eder.
        // EF Core bu property üzerinden Users tablosu ile işlem yapar.

        public DbSet<UserEntity> Users { get; set; }


        // DbSet<BlogPostEntity>
        // Veritabanındaki BlogPosts tablosunu temsil eder.

        public DbSet<BlogPostEntity> BlogPosts { get; set; }
    }
}