using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PostgreeBusinessSQL;
using PostgreeModelSQL.Entities;


namespace MoscowWeather.Business
{
    public static class Business
    {
        private static EF_WeatherRow _EF = new EF_WeatherRow(new PostgreeModelSQL.ApplicationContext());
        private enum MonthNames 
        {
            Январь = 1, Февраль, Март, Апрель, Май, Июнь, Июль, Август, Сентябрь, Октябрь, Ноябрь, Декабрь
        }
        public static string GetDayOfWeekName(string name)
        {
            switch (name)
            {
                case "Monday": return "Пн";
                case "Tuesday": return "Вт";
                case "Wednesday": return "Ср";
                case "Thursday": return "Чт";
                case "Friday": return "Пт";
                case "Saturday": return "Сб";
                case "Sunday": return "Вс";
                default: return "";
            }
        }

        public static List<string> GetImgByDay(DateOnly date)
        {
            string weatherState = _EF.GetWeatherStateByDay(date);

            if (weatherState != null && weatherState != "")
            {
                string imgPath = "";
                using (StreamReader r = new StreamReader("WeatherState.json"))
                {
                    string json = r.ReadToEnd();
                    var jsonObject = JObject.Parse(json);
                    imgPath = (string)jsonObject[weatherState];
                }
                return new List<string> { imgPath, weatherState};
            }
            else 
            {
                return new List<string> { "/img/sun.png", ""};
            }

        }

        public static Dictionary<string, List<string>> GetYearsMonthsByNames(Dictionary<string, List<int>> keyValues)
        {
            Dictionary<string, List<string>> existingMonthsByYears = new Dictionary<string, List<string>> { };
            foreach (var pair in keyValues)
            {
                List<string> months = new List<string>();
                foreach (var month in pair.Value)
                {
                    months.Add(((MonthNames)month).ToString());
                }
                existingMonthsByYears.Add(pair.Key, months);
            }
            return existingMonthsByYears;
        }

        public static void CreateWorkbookByBytes(List<KeyValuePair<string, byte[]>> excelByteList)
        {
            List<List<WeatherArchive>> archives = new List<List<WeatherArchive>>();
            var yearArchive = new List<WeatherArchive>();
            foreach (var excelBytes in excelByteList)
            {
                try 
                {
                    using (MemoryStream ms = new MemoryStream(excelBytes.Value))
                    {
                        switch (excelBytes.Key)
                        {
                            case ".xls":
                                yearArchive = ExportWorkbookDataToEFRow(new HSSFWorkbook(ms));
                                if(yearArchive.Count != 0) archives.Add(yearArchive);
                                break;
                            case ".xlsx":
                                yearArchive = ExportWorkbookDataToEFRow(new XSSFWorkbook(ms));
                                if (yearArchive.Count != 0) archives.Add(yearArchive);
                                break;
                        }

                    }
                }
                catch
                {
                    continue;
                }                   
            }
            if (archives.Count != 0)
            {
                _EF.AddWeatherRowsToDB(archives);
            }
            else throw new Exception();                   
        }

        public static List<WeatherArchive> ExportWorkbookDataToEFRow(IWorkbook workbook)
        {
            List<WeatherArchive> yearArchive = new List<WeatherArchive>();
            workbook.MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK;           
            int rowID = _EF.GetNewID();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);
                var enumeratorRow = sheet.GetRowEnumerator();
                
                while (enumeratorRow.MoveNext())
                {
                    var row = (IRow)enumeratorRow.Current;
                    WeatherArchive archiveRow = new WeatherArchive();
                    try
                    {
                        archiveRow.Id = rowID;
                        archiveRow.Date = DateOnly.Parse(row.GetCell(0).ToString());
                        archiveRow.Time = TimeOnly.Parse(row.GetCell(1).ToString());
                        archiveRow.Temperature = row.GetCell(2).CellType != CellType.Numeric ? null : Convert.ToDouble(row.GetCell(2).ToString());
                        archiveRow.Relative_Humidity = row.GetCell(3).CellType != CellType.Numeric ? null : Convert.ToDouble(row.GetCell(3).ToString());
                        archiveRow.Dew_Point = row.GetCell(4).CellType != CellType.Numeric ? null : Convert.ToDouble(row.GetCell(4).ToString());
                        archiveRow.Atmospheric_Pressure = row.GetCell(5).CellType != CellType.Numeric ? null : Convert.ToInt32(row.GetCell(5).ToString());
                        archiveRow.Wind_Direction = row.GetCell(10) == null || row.GetCell(6).CellType != CellType.String ? "" : row.GetCell(6).ToString();
                        archiveRow.Wind_Speed = row.GetCell(7).CellType != CellType.Numeric ? null : Convert.ToInt32(row.GetCell(7).ToString());
                        archiveRow.Cloud_Cover = row.GetCell(8).CellType != CellType.Numeric ? null : Convert.ToInt32(row.GetCell(8).ToString());
                        archiveRow.Cloud_Height = row.GetCell(9).CellType != CellType.Numeric ? null : Convert.ToInt32(row.GetCell(9).ToString());
                        archiveRow.Horizontal_Visibility = row.GetCell(10).CellType != CellType.Numeric ? null : Convert.ToInt32(row.GetCell(10).ToString());
                        archiveRow.Weather_State = row.GetCell(11) == null || row.GetCell(11).CellType != CellType.String ? "" : row.GetCell(11).ToString();
                        yearArchive.Add(archiveRow);
                        rowID++;

                    }
                    catch (NullReferenceException)
                    {
                        continue;                   
                    }
                    catch (InvalidCastException)
                    {
                        continue;
                    }

                }
            }
            return yearArchive;
        }
    }
}
