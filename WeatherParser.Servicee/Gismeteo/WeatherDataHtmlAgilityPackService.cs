﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WeatherParser.Entities;
using WeatherParser.Repository.Contract;
using WeatherParser.Service.Contract;

namespace WeatherParser.Service
{
    class WeatherDataHtmlAgilityPackService : IWeatherParserService
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        public WeatherDataHtmlAgilityPackService(IWeatherParserRepository weatherParserRepository)
        {
            _weatherParserRepository = weatherParserRepository;
        }

        public Dictionary<DateTime, List<WeatherData>> GetAllWeatherData(DateTime targetDate)
        {
            return _weatherParserRepository.GetAllWeatherData(targetDate);
        }

        public DateTime GetFirstDate()
        {
            return _weatherParserRepository.GetFirstDate();
        }

        public DateTime GetLastDate()
        {
            return _weatherParserRepository.GetLastDate();
        }

        public void SaveWeatherData(string url, int dayNum)
        {
            List<WeatherData> listOfWeatherData = new List<WeatherData>(8);

            for (int i = 0; i < 8; ++i)
            {
                listOfWeatherData.Add(new WeatherData());
                listOfWeatherData[i].CollectionDate = DateTime.Now;
                listOfWeatherData[i].Date = listOfWeatherData[i].CollectionDate.AddDays(dayNum);
            }

            string pageContent = LoadPage(url);
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            if (pageContent != "" && pageContent != null)
            {
                document.LoadHtml(pageContent);

                //temperature
                HtmlNodeCollection linkTemp = document.DocumentNode.SelectNodes("/html/body/section[2]/div[1]/section[3]/div/div/div/div/div[3]/div/div/div/span[1]");
                if (linkTemp != null)
                {
                    string[] temperature = new string[8];

                    var k = 0;

                    foreach (HtmlNode link in linkTemp)
                    {
                        if (k < 8)
                        {
                            temperature[k] = link.InnerText;
                            ++k;
                        }
                    }

                    for (int j = 0; j < 8; ++j)
                    {
                        int minus = temperature[j].IndexOf('-'); //if temp < 0

                        while (!char.IsDigit(temperature[j][0]))
                        {
                            temperature[j] = temperature[j].Remove(0, 1);
                        }

                        listOfWeatherData[j].Temperature = int.Parse(temperature[j]);

                        if (minus != -1)
                        {
                            listOfWeatherData[j].Temperature *= -1;
                        }

                    }

                }

                //humidity
                HtmlNodeCollection linkHum = document.DocumentNode.SelectNodes("/html/body/section[2]/div[1]/section[15]/div/div[3]/div/div/div[2]/div");
                if (linkHum != null)
                {
                    string[] humidity = new string[8];

                    var k = 0;

                    foreach (HtmlNode link in linkHum)
                    {
                        if (k < 8)
                        {
                            humidity[k] = link.InnerText;
                            ++k;
                        }
                    }

                    for (int j = 0; j < 8; ++j)
                    {
                        while (!char.IsDigit(humidity[j][0]))
                        {
                            humidity[j] = humidity[j].Remove(0, 1);
                        }

                        listOfWeatherData[j].Humidity = int.Parse(humidity[j]);
                    }
                }

                //pressure
                HtmlNodeCollection linkPres = document.DocumentNode.SelectNodes("/html/body/section[2]/div[1]/section[14]/div/div[3]/div/div/div[2]/div/div/div/span[1]");
                if (linkPres != null)
                {
                    string[] pressure = new string[8];

                    var k = 0;

                    foreach (HtmlNode link in linkPres)
                    {
                        if (k < 8)
                        {
                            pressure[k] = link.InnerText;
                            ++k;
                        }
                    }

                    for (int j = 0; j < 8; ++j)
                    {
                        while (!char.IsDigit(pressure[j][0]))
                        {
                            pressure[j] = pressure[j].Remove(0, 1);
                        }

                        listOfWeatherData[j].Pressure = int.Parse(pressure[j]);
                    }
                }

                //wind-speed
                HtmlNodeCollection linkWindSpeed = document.DocumentNode.SelectNodes("/html/body/section[2]/div[1]/section[10]/div/div[3]/div/div/div[2]/div/span[1]");
                if (linkWindSpeed != null)
                {
                    string[] windSpeed = new string[8];

                    var k = 0;

                    foreach (HtmlNode link in linkWindSpeed)
                    {
                        if (k < 8)
                        {
                            windSpeed[k] = link.InnerText;
                            ++k;
                        }
                    }

                    for (int j = 0; j < 8; ++j)
                    {
                        if (windSpeed[j].Any(c => c == '-'))
                        {
                            listOfWeatherData[j].WindSpeedFirst = int.Parse(windSpeed[j].Split('-')[0]);
                            listOfWeatherData[j].WindSpeedSecond = int.Parse(windSpeed[j].Split('-')[1]);
                        }
                        else
                        {
                            listOfWeatherData[j].WindSpeedFirst = int.Parse(windSpeed[j]);
                            listOfWeatherData[j].WindSpeedSecond = int.MaxValue;
                        }
                    }
                }

                //wind-direction
                HtmlNodeCollection linkWindDirect = document.DocumentNode.SelectNodes("/html/body/section[2]/div[1]/section[10]/div/div[3]/div/div/div[3]/div/div[2]");
                if (linkWindDirect != null)
                {
                    string[] windDir = new string[8];

                    var k = 0;

                    foreach (HtmlNode link in linkWindDirect)
                    {
                        if (k < 8)
                        {
                            windDir[k] = link.InnerText;
                            ++k;
                        }
                    }

                    for (int j = 0; j < 8; ++j)
                    {
                        listOfWeatherData[j].WindDirection = windDir[j];
                    }
                }

                _weatherParserRepository.SaveWeatherData(listOfWeatherData);
            }
        }

    public string LoadPage(string url) //загрузка страницы
    {
        HttpWebResponse response = null;
        string result = "";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
            Stream receiveStream = response.GetResponseStream();
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
}
