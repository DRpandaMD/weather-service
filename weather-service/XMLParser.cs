using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;

namespace weather_service
{
    /*
     * Author Michael Zarate
     * We are going to make an xml parser.  This is a good idea as most of our data objects are comming back as xml
     */
    public class XMLParser
    {
        // make an XML document object
        XmlDocument XMLDoc;


        //Create a new XML document based on the byte stream we are feeding it.
        public XMLParser(Stream xml)
        {
            XMLDoc = new XmlDocument();
            XMLDoc.Load(xml);
        }

        //return a latitude based on the zipcode we had passed google geocode api
        public double getLatitude()
        {
            XmlNodeList nodes = XMLDoc.DocumentElement.SelectNodes("//GeocodeResponse/result/geometry/location");
            double latitude = -2; //our error checking number
            foreach(XmlNode node in nodes)
            {
                latitude = Convert.ToDouble(node.SelectSingleNode("lat").InnerText);
            }
            return latitude;
        }

        //do the same thing for longitude
        public double getLongitude()
        {
            XmlNodeList nodes = XMLDoc.DocumentElement.SelectNodes("//GeocodeResponse/result/geometry/location");
            double longitude = -2; //error checking number
            foreach(XmlNode node in nodes)
            {
                longitude = Convert.ToDouble(node.SelectSingleNode("lng").InnerText);
            }
            return longitude;
        }

        // We need to make a list of our forcast data from the xml we got from the api call
        public List<dailyForcastInfo> getForcast()
        {
            XmlNodeList nodes = XMLDoc.DocumentElement.SelectNodes("//weatherdata/forecast/time");
            List<dailyForcastInfo> forecast = new List<dailyForcastInfo>();
            foreach(XmlNode node in nodes)
            {
                dailyForcastInfo oneDay = new dailyForcastInfo(); //create a new struct
                if(node.Attributes != null) //if we still have attributes in our node
                {
                    oneDay.date = "<b>" + node.Attributes["day"].Value + "</b>";
                    oneDay.maxTemp = node.SelectSingleNode("temperature").Attributes["max"].Value;
                    oneDay.minTemp = node.SelectSingleNode("temperature").Attributes["min"].Value;
                    oneDay.humidityLevel = node.SelectSingleNode("humidity").Attributes["value"].Value;
                    oneDay.clouds = node.SelectSingleNode("clouds").Attributes["value"].Value;
                }
                forecast.Add(oneDay);
            }
            return forecast;
        }

        //lastly we also want to give the location name for a zipcode that got entered in
        public string getLocationName()
        {
            XmlNodeList nodes = XMLDoc.DocumentElement.SelectNodes("//GeocodeResponse/result");
            string location = "";
            foreach(XmlNode node in nodes)
            {
                location = node.SelectSingleNode("formatted_address").InnerText;
            }
            return location;
        }

        //We need to make our own custom data structure to hold daily Forcast info
        public struct dailyForcastInfo
        {
            public string date;
            public string maxTemp;
            public string minTemp;
            public string humidityLevel;
            public string clouds;

            public string toString()
            {
                string weatherStringOut = string.Format("Date: {0} \nLow Temperature: {1} \nHigh Temperature: {2} \nHumidity: {3} \nClouds: {4} \n",
                                                    date, minTemp, maxTemp, humidityLevel, clouds);
                return weatherStringOut;
            }
        }
    }
}