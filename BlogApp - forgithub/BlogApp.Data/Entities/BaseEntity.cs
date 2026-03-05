using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Data.Entities
{
    //BaseEntity: veritabanındaki tüm tabloların ortak alanlarını içerir.
    //Bu projede tüm entity sınıfları bundan kalıtım alır.
    public abstract class BaseEntity
    {
        // [Key] → Bu alanın Primary Key olduğunu belirtir.
        // EF Core bu alanı tablonun ana anahtarı yapar.

        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // Identity → SQL Server bu değeri otomatik üretir (Auto Increment).
        // Yani kayıt eklenirken Id'yi biz vermeyiz, veritabanı üretir.
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
}
