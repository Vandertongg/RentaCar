using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentaCar.RealEstateManager.Database.Data.Entities
{
    [Table("RentNews", Schema = "blg")]
    public class RentNews
    {
        [Key]
        public int Pk { get; set; }

        public string? NewsPicture { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}
