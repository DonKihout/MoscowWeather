using PostgreeBusinessSQL;
using PostgreeModelSQL;
namespace ExcelToDBConverter
{
    public class Program
    {
        static void Main(string[] args)
        {
            Converter converter = new Converter();
            EF_WeatherRow ef = new EF_WeatherRow(new ApplicationContext());
            var a = converter.ExportExcelDataToList(@"C:\\Users\\Vadim\\Desktop\\Тестовое задание DS\\Weather.Moscow.2010-2014\\Weather.Moscow.2010-2014\\moskva_2013.xlsx", ef);
            ef.AddWeatherArchiveToDB(a);
        }
    }
}