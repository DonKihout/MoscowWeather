using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PostgreeModelSQL.Entities
{
    [Keyless]
    public class WeatherArchiveDaily
    {

        public DateOnly Date { get; set; }

        public double? Average_Temperature { get; set; }

        public int? Average_Atmospheric_Pressure { get; set; }

        public int? Average_Wind_Speed { get; set; }
    }
}
