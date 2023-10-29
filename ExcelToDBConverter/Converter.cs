using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PostgreeBusinessSQL;
using PostgreeModelSQL.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToDBConverter
{
    public class Converter
    {
        public List<WeatherArchive> ExportExcelDataToList(string path, EF_WeatherRow ef) 
        {
            IWorkbook workbook;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            workbook.MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK;
            List<WeatherArchive> archives = new List<WeatherArchive>();
            int rowID = ef.GetNewID();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);
                var enumeratorRow = sheet.GetRowEnumerator();

                while (enumeratorRow.MoveNext())
                {
                    var row = (IRow) enumeratorRow.Current;
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
                        archives.Add(archiveRow);
                        rowID++;

                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    
                }              
            }

            return archives;
        }     
    }
}
