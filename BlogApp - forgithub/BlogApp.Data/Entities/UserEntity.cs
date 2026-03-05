using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Entities
{
    // UserEntity → veritabanındaki Users tablosunu temsil eder
    public class UserEntity : BaseEntity
    {
        // Email alanı zorunludur.
        // StringLength(100) → maksimum 100 karakter olabilir.
        // MinimumLength = 4 → en az 4 karakter olmalı.

        [Required, StringLength(100, MinimumLength = 4)]
        public string Email { get; set; } = null!;


        // Password alanı zorunludur.
        // MinimumLength = 4 → en az 4 karakter.

        // NOT:
        // Bu proje bir öğrenim projesi olduğu için
        // password hashlenmeden tutuluyor.
        // Gerçek uygulamalarda mutlaka HASH kullanılır.

        [Required, MinLength(4)]
        public string Password { get; set; } = null!;
    }
}