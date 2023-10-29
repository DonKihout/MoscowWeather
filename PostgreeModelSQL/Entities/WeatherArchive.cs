using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreeModelSQL.Entities
{
    public class WeatherArchive
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly Time { get; set; }

        public double? Temperature { get; set; }

        public double? Relative_Humidity { get; set; }

        public double? Dew_Point { get; set; }

        public int? Atmospheric_Pressure { get; set; }

        public string? Wind_Direction { get; set; }

        public int? Wind_Speed { get; set; }

        public int? Cloud_Cover { get; set; }

        public int? Cloud_Height { get; set; }

        public int? Horizontal_Visibility { get; set; }

        public string? Weather_State { get; set; }
    }
}
