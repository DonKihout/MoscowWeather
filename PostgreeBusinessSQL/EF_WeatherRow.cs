using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PostgreeModelSQL;
using PostgreeModelSQL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PostgreeBusinessSQL
{
    public class EF_WeatherRow
    {
        private readonly ApplicationContext _context;

        public EF_WeatherRow(ApplicationContext context)
        {
            this._context = context;
        }

        public void AddWeatherRowsToDB(List<List<WeatherArchive>> weatherArchive)
        {
            foreach (var archive in weatherArchive)
            {
                _context.WeatherArchive.AddRange(archive);
            }         
            _context.SaveChanges();
        }

        public int GetNewID()
        {
            return _context.WeatherArchive.Max(m => m.Id) + 1;
        }

        public string? GetWeatherStateByDay(DateOnly date)
        {
            var weatherState = _context.WeatherArchive
                                    .Where(d => d.Date == date && d.Weather_State != "" && d.Weather_State != null)
                                    .Select(w => w.Weather_State).
                                    FirstOrDefault();
            return weatherState;
        }

        public List<WeatherArchive> GetWeatherWritesByDay(DateOnly date)
        {
            var weatherWrites = _context.WeatherArchive
                                    .Where(d => d.Date == date)
                                    .OrderBy(d => d.Time)
                                    .ToList();
            return weatherWrites;
        }

        public Dictionary<string, List<int>>? GetAllYearsByMonth()
        {
            var existingYears = _context.WeatherArchive
                                    .Select(d => d.Date.Year)
                                    .Distinct()
                                    .OrderBy(d => d)
                                    .ToList();

            if (!(existingYears.Equals(null) || existingYears.Count == 0))
            {
                Dictionary<string, List<int>> existingMonthsByYears = new Dictionary<string, List<int>> { };
                foreach (var year in existingYears)
                {
                    var mounth = _context.WeatherArchive
                                    .Where(y => y.Date.Year == year)
                                    .Select(m => m.Date.Month)
                                    .Distinct()
                                    .OrderBy(m => m)
                                    .ToList();

                    if (mounth.Any())
                    {
                        existingMonthsByYears.Add(year.ToString(), mounth);
                    }
                }

                if (existingMonthsByYears.Any())
                {
                    return existingMonthsByYears;
                }
            }
            return null;
        }

        public List<List<WeatherArchiveDaily>>? GetDailyArchiveByMonth(int monthNumber, int year)
        {
            if (monthNumber <= 12 && monthNumber >= 1 && year > 0)
            {
                List<WeatherArchiveDaily> monthlyArchive = _context.WeatherArchiveDaily
                                                        .Where(m => m.Date.Month == monthNumber
                                                                && m.Date.Year == year)
                                                        .OrderBy(d => d.Date).ToList();
                if (monthlyArchive.Count != 0)
                {
                    var WeatherArchiveDailyGrouped = new List<List<WeatherArchiveDaily>> { };
                    int dayArchiveInGroup = 0;
                    foreach (var dayArchive in monthlyArchive)
                    {
                        if (dayArchiveInGroup % 6 == 0)
                        {
                            WeatherArchiveDailyGrouped.Add(new List<WeatherArchiveDaily> {dayArchive});
                        }
                        else 
                        {
                            WeatherArchiveDailyGrouped.Last().Add(dayArchive);
                        }
                        dayArchiveInGroup++;
                    }
                    return WeatherArchiveDailyGrouped;
                }
            }

            return null;
        }

    }
}
