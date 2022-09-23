using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;

namespace WeatherParser.Repository;

public class WeatherDataFileRepository : IWeatherParserRepository
{
    public List<WeatherDataRepository> GetAllWeatherData(DateTime targetDate, Guid siteId)
    {
        var dataInFiles = new Dictionary<DateTime, List<WeatherRepository>>();
        //когда был составлен прогноз + список, где каждый список это weatherData на каждый из 8 часов
        //проще и быстрее проводить работу с поиском и сравнением данных с помощью словаря, где ключ - дата сбора данных

        var weatherData = new WeatherRepository();

        using (var fileTemperature = new StreamReader("../WeatherParser.Repository/SaveFiles/Temperature.txt"))
        {
            while (!fileTemperature.EndOfStream)
            {
                var tempStr = fileTemperature.ReadLine().Trim().Split(' ');

                if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                {
                    if (!dataInFiles.ContainsKey(DateTime.Parse(tempStr[0])))
                        dataInFiles.Add(DateTime.Parse(tempStr[0]), new List<WeatherRepository>());

                    weatherData = new WeatherRepository
                    {
                        Temperature = new List<double>(),
                        Humidity = new List<int>(),
                        Pressure = new List<int>(),
                        WindDirection = new List<string>(),
                        WindSpeed = new List<int>()
                    };

                    weatherData.Date = targetDate;

                    for (var i = 2; i < tempStr.Length; ++i)
                        if (tempStr[i].Any(c => c == '−'))
                            weatherData.Temperature.Add(double.Parse(tempStr[i].Replace('−', ' ').Trim()) * -1);
                        else
                            weatherData.Temperature.Add(double.Parse(tempStr[i]));
                    dataInFiles[DateTime.Parse(tempStr[0])].Add(weatherData);
                }
            }
        }

        var indexWeatherData = 0;

        using (var filePressure = new StreamReader("../WeatherParser.Repository/SaveFiles/Pressure.txt"))
        {
            while (!filePressure.EndOfStream)
            {
                var tempStr = filePressure.ReadLine().Trim().Split(' ');

                if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                {
                    var pressure = new List<int>();

                    for (var i = 2; i < tempStr.Length; ++i) pressure.Add(int.Parse(tempStr[i]));
                    dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].Pressure = pressure;
                    indexWeatherData += 1;
                }
            }
        }

        using (var fileWindSpeed = new StreamReader("../WeatherParser.Repository/SaveFiles/WindSpeed.txt"))
        {
            indexWeatherData = 0;

            while (!fileWindSpeed.EndOfStream)
            {
                var tempStr = fileWindSpeed.ReadLine().Trim().Split(' ');

                if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                {
                    var windSpeed = new List<int>();

                    for (var i = 2; i < tempStr.Length; ++i)
                        if (!tempStr[i].Any(c => c == '-'))
                            windSpeed.Add(int.Parse(tempStr[i]));
                        else
                            windSpeed.Add(int.Parse(tempStr[i].Split('-')[0]));
                    dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindSpeed = windSpeed;
                    indexWeatherData += 1;
                }
            }
        }

        using (var fileWindDirection = new StreamReader("../WeatherParser.Repository/SaveFiles/WindDirection.txt"))
        {
            indexWeatherData = 0;

            while (!fileWindDirection.EndOfStream)
            {
                var tempStr = fileWindDirection.ReadLine().Trim().Split(' ');

                if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                {
                    var windDir = new List<string>();

                    for (var i = 2; i < tempStr.Length; ++i) windDir.Add(tempStr[i]);
                    dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindDirection = windDir;
                    indexWeatherData += 1;
                }
            }
        }

        using (var fileHumidity = new StreamReader("../WeatherParser.Repository/SaveFiles/Humidity.txt"))
        {
            indexWeatherData = 0;

            while (!fileHumidity.EndOfStream)
            {
                var tempStr = fileHumidity.ReadLine().Trim().Split(' ');

                if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                {
                    var hum = new List<int>();

                    for (var i = 2; i < tempStr.Length; ++i) hum.Add(int.Parse(tempStr[i]));
                    dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].Humidity = hum;
                    indexWeatherData += 1;
                }
            }
        }

        //преобразование словаря в возвращаемый тип
        var resultData = new List<WeatherDataRepository>();

        foreach (var weather in dataInFiles)
            resultData.Add(new WeatherDataRepository { TargetDate = weather.Key, Weather = weather.Value });

        return resultData;
    }

    public DateTime GetFirstAndLastDate(Guid siteId)
    {
        throw new NotImplementedException();
    }

    public List<SiteRepository> GetSites()
    {
        throw new NotImplementedException();
    }

    public void SaveWeatherData(WeatherDataRepository weatherData)
    {
        var pathMain = @"../WeatherParser.Repository/SaveFiles";
        if (!Directory.Exists(pathMain)) Directory.CreateDirectory(pathMain);

        if (!File.ReadAllLines("../WeatherParser.Repository/SaveFiles/Temperature.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == weatherData.TargetDate.Date))
            using (var fileTemperature =
                   new StreamWriter("../WeatherParser.Repository/SaveFiles/Temperature.txt", true))
            {
                fileTemperature.Write($"{weatherData.TargetDate.ToShortDateString()} ");
                fileTemperature.Write($"{weatherData.Weather.FirstOrDefault().Date.ToShortDateString()} ");
                foreach (var tempOfWeatherData in weatherData.Weather.FirstOrDefault().Temperature)
                    fileTemperature.Write($"{tempOfWeatherData} ");
                fileTemperature.WriteLine();
            }

        if (!File.ReadAllLines("../WeatherParser.Repository/SaveFiles/Pressure.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == weatherData.TargetDate.Date))
            using (var filePressure = new StreamWriter("../WeatherParser.Repository/SaveFiles/Pressure.txt", true))
            {
                filePressure.Write($"{weatherData.TargetDate.ToShortDateString()} ");
                filePressure.Write($"{weatherData.Weather.FirstOrDefault().Date.ToShortDateString()} ");
                foreach (var presOfWeatherData in weatherData.Weather.FirstOrDefault().Pressure)
                    filePressure.Write($"{presOfWeatherData} ");
                filePressure.WriteLine();
            }

        if (!File.ReadAllLines("../WeatherParser.Repository/SaveFiles/WindSpeed.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == weatherData.TargetDate.Date))
            using (var fileWindSpeed = new StreamWriter("../WeatherParser.Repository/SaveFiles/WindSpeed.txt", true))
            {
                fileWindSpeed.Write($"{weatherData.TargetDate.ToShortDateString()} ");
                fileWindSpeed.Write($"{weatherData.Weather.FirstOrDefault().Date.ToShortDateString()}");
                foreach (var windspOfWeatherData in weatherData.Weather.FirstOrDefault().WindSpeed)
                    fileWindSpeed.Write($" {windspOfWeatherData}");
                fileWindSpeed.WriteLine();
            }

        if (!File.ReadAllLines("../WeatherParser.Repository/SaveFiles/WindDirection.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == weatherData.TargetDate.Date))
            using (var fileWindDirection =
                   new StreamWriter("../WeatherParser.Repository/SaveFiles/WindDirection.txt", true))
            {
                fileWindDirection.Write($"{weatherData.TargetDate.ToShortDateString()} ");
                fileWindDirection.Write($"{weatherData.Weather.FirstOrDefault().Date.ToShortDateString()} ");
                foreach (var winddirOfWeatherData in weatherData.Weather.FirstOrDefault().WindDirection)
                    fileWindDirection.Write($"{winddirOfWeatherData} ");
                fileWindDirection.WriteLine();
            }

        if (!File.ReadAllLines("../WeatherParser.Repository/SaveFiles/Humidity.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == weatherData.TargetDate.Date))
            using (var fileHumidity = new StreamWriter("../WeatherParser.Repository/SaveFiles/Humidity.txt", true))
            {
                fileHumidity.Write($"{weatherData.TargetDate.ToShortDateString()} ");
                fileHumidity.Write($"{weatherData.Weather.FirstOrDefault().Date.ToShortDateString()} ");
                foreach (var humOfWeatherData in weatherData.Weather.FirstOrDefault().Humidity)
                    fileHumidity.Write($"{humOfWeatherData} ");
                fileHumidity.WriteLine();
            }
    }

    public DateTime GetFirstDate()
    {
        return DateTime.Parse(File.ReadLines("../WeatherParser.Repository/SaveFiles/Temperature.txt").FirstOrDefault()
            .Trim().Split(' ')[1]);
    }

    public DateTime GetLastDate()
    {
        return DateTime.Parse(File.ReadLines("../WeatherParser.Repository/SaveFiles/Temperature.txt").LastOrDefault()
            .Trim().Split(' ')[1]);
    }
}