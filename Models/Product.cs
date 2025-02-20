using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TuketAppAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required] // Ürün ismi boş bırakılamaz
        public string Name { get; set; } = string.Empty;

        [Required] // Açıklama boş olamaz
        public string Description { get; set; } = string.Empty;

        [Required] // Fiyat zorunlu
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat negatif olamaz.")]
        public double Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        //  Ürünün bağlı olduğu işletme
        [ForeignKey("Business")]
        public int BusinessId { get; set; }
        public Business Business { get; set; } = null!;
    }
}