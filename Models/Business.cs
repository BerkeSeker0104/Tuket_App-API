using System.ComponentModel.DataAnnotations;

namespace TuketAppAPI.Models
{
    public class Business
    {
        public int Id { get; set; } //  Otomatik olarak oluşturulmalı (Veritabanında AutoIncrement olacak)

        [Required]  //  Boş olamaz
        public string Name { get; set; } = string.Empty;   

        [Required]  //  Boş olamaz
        public string Address { get; set; } = string.Empty;

        [Required]  //  Boş olamaz
        public double Latitude { get; set; }  

        [Required]  //  Boş olamaz
        public double Longitude { get; set; }

        [Required]  //  İşletmeyi oluşturan kullanıcı ID’si zorunlu
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; //  Varsayılan olarak güncel tarih atanmalı
    }
}