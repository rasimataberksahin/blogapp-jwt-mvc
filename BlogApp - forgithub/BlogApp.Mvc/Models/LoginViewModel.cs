using System.ComponentModel.DataAnnotations;

namespace BlogApp.Mvc.Models
{
    // LoginViewModel sadece login formundan gelen veriyi taşır.
    // Buradaki validation attribute'ları ModelState.IsValid kontrolünde devreye girer.
    // Amaç: boş/yanlış formatta veri gelirse daha DB'ye gitmeden yakalamak.

    public class LoginViewModel
    {
        [Required, MinLength(4), MaxLength(100)]
        // Required: boş geçilemez
        // MinLength/MaxLength: form validation + mantıksal sınır
        public string Email { get; set; } = null!;

        [Required, MinLength(4), DataType(DataType.Password)]
        // DataType.Password: view tarafında input type="password" gibi davranır (gizli yazar)
        public string Password { get; set; } = null!;
    }
}