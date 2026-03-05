using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Entities
{
    //BlogPostEntity: verştabanındaki BlogPosts tablosunu temsil eder.
    public class BlogPostEntity : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Title { get; set; } = null!;//title alanı
        [Required]
        public string Content { get; set; }//content--> blog içeriğini tutar.

        public DateTime CreatedAt { get; set; }//blog ne zaman oluşturuldu.controller içerisinde genelde dateTime.Now atanır.


        public long UserId { get; set; }//bolgu yazan kullanıcının ıd değeri. bu alan User property sinin foreign keyi görevini görür.

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; } = null!;
    }
}
