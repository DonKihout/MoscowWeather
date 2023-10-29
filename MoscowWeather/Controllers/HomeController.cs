using Microsoft.AspNetCore.Mvc;
using MoscowWeather.Models;
using PostgreeBusinessSQL;
using PostgreeModelSQL.Entities;
using System.IO;
using System.IO.Pipes;
using System.Web;

namespace MoscowWeather.Controllers
{
    public class HomeController : Controller
    {
        private EF_WeatherRow _EF;
        public HomeController(EF_WeatherRow _EF)
        {
            this._EF = _EF;
        }
        public IActionResult Index()
        {
            var defaultWeatherArchiveDaily = _EF.GetDailyArchiveByMonth(2, 2012);
            return View(defaultWeatherArchiveDaily);
        }

        public IActionResult SelectedMonth(int monthNumber, int year)
        {
            var selectedWeatherArchiveDaily = _EF.GetDailyArchiveByMonth(monthNumber, year);
            return View("index",selectedWeatherArchiveDaily);
        }

        [HttpPost]
        public IActionResult DetailedDescriptionDay(string date)
        {   
            DateOnly clickedDate = DateOnly.FromDateTime(Convert.ToDateTime(date));
            List<WeatherArchive> weatherWritesList = _EF.GetWeatherWritesByDay(clickedDate);
            return View(weatherWritesList);
        }

        
        public IActionResult WeatherArchive()
        {
            var existingMonthsByYears = Business.Business.GetYearsMonthsByNames(_EF.GetAllYearsByMonth());
            
            return View("ModalArchive", existingMonthsByYears);
        }

        public IActionResult UploadWeatherArchive()
        {
            return View("UploadFileModal");
        }

        [HttpPost]
        public IActionResult UploadFile(List<IFormFile> files)
        {
            if (files.Count > 0)
            {
                var excelByteList = new List<KeyValuePair<string, byte[]>> { };
                foreach (var file in files)
                {
                    var fileExtansion = Path.GetExtension(file.FileName);
                    if (fileExtansion == ".xls" || fileExtansion == ".xlsx" || file.Length > 0)
                    {
                        byte[] bytes = new byte[file.Length];
                        using (Stream fl = file.OpenReadStream())
                        {
                            fl.Read(bytes, 0, (int)file.Length);
                        }
                        excelByteList.Add(new KeyValuePair<string, byte[]>(fileExtansion, bytes));
                    }
                }
                if (excelByteList.Count > 0)
                {
                    try {
                        Business.Business.CreateWorkbookByBytes(excelByteList);
                        TempData["Message"] = "Файл(-ы) архива погодных условий был(-и) успешно загружен(-ы)!";
                    }
                    catch {
                        TempData["Message"] = "Возникла проблема при загрузке файла(-ов) архива погодных условий!";
                    }
                    
                }
            }
            
            return RedirectToAction("Index");
        }

    }
}