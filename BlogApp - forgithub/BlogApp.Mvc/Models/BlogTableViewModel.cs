/*
 * VİEWMODEL Uİ KATMANI İÇİN HAZIRLANMIŞ MODELDİR.
 * AMAÇ: VIEW A SADECE GEREKLİ VERİYİ GÖNDEREYİM
  
  
 *Neden Entity’i direkt View’a göndermiyoruz?
  Bu çok kritik bir yazılım prensibi.
   Eğer Entity gönderirsek:
     Güvenlik riski

MEELA USERENTITY DE EMAIL E PASSWORD VIEW A GÖNDERİLİRSE PASSWORD DA GİDER BUNU İSTEMEYİZ.
GEREKSİZ VERİ TAŞIMIŞ OLABİLİRİZ.
UI BAĞIMLILIĞI OLUŞUR. EĞER UI DEĞİŞİRSE DB MODELİNİ DEĞİŞTİRMEK ZORUNDA KALIRSIN.
 */

namespace BlogApp.Mvc.Models
{
    // Bu model sadece "liste ekranı" için var.
    // Entity (BlogPostEntity) yerine bunu kullanıyoruz çünkü:
    // - View'a gereksiz alan göndermeyelim (Content'in tamamı gibi)
    // - UI için gereken kadar veri gitsin
    // - Entity'yi view'a bağımlı hale getirmeyelim

    public class BlogTableViewModel
    {
        // Blog'un Id'si
        // Detail sayfasına giderken asp-route-id ile bu gönderiliyor
        public long Id { get; set; }

        // Blog başlığı (listede gösteriyoruz)
        public string Title { get; set; }

        // Blog içeriğinin uzunluğu (örnek alan)
        // Controller'da Size = s.Content.Length şeklinde hesaplanıyor
        public long Size { get; set; }

        // Oluşturulma tarihi (listede gösteriyoruz)
        public DateTime CreatedAt { get; set; }
    }
}
