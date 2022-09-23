using System.Net;
using System.Reflection;
using System.Text;
using Helpers;
using HtmlAgilityPack;
using WeatherParser.Service.Entities;
using WeatherParser.Service.Entities.Urls;
using WeatherParser.Service.GismeteoService.Contract;

namespace WeatherParser.Service.GismeteoService;

public class WeatherDataHtmlAgilityPackServiceGismeteo : IWeatherParserServiceGismeteo
{
    public List<WeatherDataService> SaveWeatherData()
    {
        var resultListWeatherDataService = new List<WeatherDataService>();

        var urls = new List<string>();

        //find all static fields
        var fields = typeof(GismeteoSaratovCollection).GetFields(BindingFlags.Static);

        //bring in collection
        foreach (var field in fields) urls.Add((string)field.GetValue(null));

        foreach (var url in urls)
        {
            var listOfWeatherData = new WeatherService
            {
                Temperature = new List<double>(),
                Humidity = new List<int>(),
                Pressure = new List<int>(),
                WindDirection = new List<string>(),
                WindSpeed = new List<int>()
            };

            //TODO PARSE DATE
            listOfWeatherData.Date = DateTime.UtcNow.AddDays(1);

            var pageContent = LoadPage(url);
            var document = new HtmlDocument();

            if (pageContent != "" && pageContent != null)
            {
                document.LoadHtml(pageContent);

                //temperature
                var linkTemp =
                    document.DocumentNode.SelectNodes(
                        "/html/body/section[2]/div[1]/section[3]/div/div/div/div/div[3]/div/div/div/span[1]");
                if (linkTemp != null)
                {
                    var temperature = new string[8];

                    var k = 0;

                    foreach (var link in linkTemp)
                        if (k < 8)
                        {
                            temperature[k] = link.InnerText;
                            ++k;
                        }

                    for (var j = 0; j < 8; ++j)
                    {
                        var minus = temperature[j].IndexOf('-'); //if temp < 0

                        while (!char.IsDigit(temperature[j][0])) temperature[j] = temperature[j].Remove(0, 1);

                        if (minus != -1)
                            listOfWeatherData.Temperature.Add(int.Parse(temperature[j]) * -1);
                        else
                            listOfWeatherData.Temperature.Add(int.Parse(temperature[j]));
                    }
                }

                //humidity
                var linkHum =
                    document.DocumentNode.SelectNodes(
                        "/html/body/section[2]/div[1]/section[15]/div/div[3]/div/div/div[2]/div");
                if (linkHum != null)
                {
                    var humidity = new string[8];

                    var k = 0;

                    foreach (var link in linkHum)
                        if (k < 8)
                        {
                            humidity[k] = link.InnerText;
                            ++k;
                        }

                    for (var j = 0; j < 8; ++j)
                    {
                        while (!char.IsDigit(humidity[j][0])) humidity[j] = humidity[j].Remove(0, 1);

                        listOfWeatherData.Humidity.Add(int.Parse(humidity[j]));
                    }
                }

                //pressure
                var linkPres = document.DocumentNode.SelectNodes(
                    "/html/body/section[2]/div[1]/section[14]/div/div[3]/div/div/div[2]/div/div/div/span[1]");
                if (linkPres != null)
                {
                    var pressure = new string[8];

                    var k = 0;

                    foreach (var link in linkPres)
                        if (k < 8)
                        {
                            pressure[k] = link.InnerText;
                            ++k;
                        }

                    for (var j = 0; j < 8; ++j)
                    {
                        while (!char.IsDigit(pressure[j][0])) pressure[j] = pressure[j].Remove(0, 1);

                        listOfWeatherData.Pressure.Add(int.Parse(pressure[j]));
                    }
                }

                //wind-speed
                var linkWindSpeed =
                    document.DocumentNode.SelectNodes(
                        "/html/body/section[2]/div[1]/section[10]/div/div[3]/div/div/div[2]/div/span[1]");
                if (linkWindSpeed != null)
                {
                    var windSpeed = new string[8];

                    var k = 0;

                    foreach (var link in linkWindSpeed)
                        if (k < 8)
                        {
                            windSpeed[k] = link.InnerText;
                            ++k;
                        }

                    for (var j = 0; j < 8; ++j)
                        if (windSpeed[j].Any(c => c == '-'))
                            listOfWeatherData.WindSpeed.Add(int.Parse(windSpeed[j].Split('-')[0]));
                        else
                            listOfWeatherData.WindSpeed.Add(int.Parse(windSpeed[j]));
                }

                //wind-direction
                var linkWindDirect =
                    document.DocumentNode.SelectNodes(
                        "/html/body/section[2]/div[1]/section[10]/div/div[3]/div/div/div[3]/div/div[2]");
                if (linkWindDirect != null)
                {
                    var windDir = new string[8];

                    var k = 0;

                    foreach (var link in linkWindDirect)
                        if (k < 8)
                        {
                            windDir[k] = link.InnerText;
                            ++k;
                        }

                    for (var j = 0; j < 8; ++j) listOfWeatherData.WindDirection.Add(windDir[j]);
                }
            }

            //map service entity to repository entity
            var newWeatherDataRepository = new WeatherDataService
            {
                TargetDate = DateTime.UtcNow,
                Weather = new List<WeatherService> { listOfWeatherData },
                SiteId = SitesHelperCollection.GismeteoSaratovCollection
            };

            resultListWeatherDataService.Add(newWeatherDataRepository);
        }

        return resultListWeatherDataService;
    }

    public string LoadPage(string url) //загрузка страницы
    {
        HttpWebResponse response = null;
        var result = "";
        var request = (HttpWebRequest)WebRequest.Create(url);
        try
        {
            response = (HttpWebResponse)request.GetResponse();
        }
        catch
        {
            return null;
        }

        if (response != null && response.StatusCode == HttpStatusCode.OK)
        {
            var receiveStream = response.GetResponseStream();
            if (receiveStream != null)
            {
                StreamReader readStream;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                result = readStream.ReadToEnd();
                readStream.Close();
            }

            response.Close();
        }

        return result;
    }
}