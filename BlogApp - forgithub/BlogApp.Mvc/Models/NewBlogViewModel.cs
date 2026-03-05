using System.ComponentModel.DataAnnotations;

namespace BlogApp.Mvc.Models
{
    // NewBlogViewModel blog ekleme formu için var.
    // Dikkat: Burada UserId yok.
    // Çünkü UserId formdan alınırsa kullanıcı HTML'i değiştirip başka userId yollayabilir.
    // Bu projede userId her zaman token'dan okunuyor (HomeController.GetUserId).

    public class NewBlogViewModel
    {
        [Required, MaxLength(50)]
        // BlogPostEntity'de de Title max 50 idi.
        // UI tarafı da aynı kuralla validasyon yapıyor (çift taraflı kontrol).
        public string Title { get; set; } = null!;

        [Required, DataType(DataType.Html)]
        // Bu projede Content içine HTML yazılabiliyor (Detail view'da Html.Raw ile basılıyor).
        // Not: Gerçek projede bu XSS riski doğurur, içerik sanitize edilmelidir.
        public string Content { get; set; } = null!;
    }
}