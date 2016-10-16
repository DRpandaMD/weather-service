using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using static weather_service.XMLParser;

namespace weather_service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "_weatherservice" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select _weatherservice.svc or _weatherservice.svc.cs at the Solution Explorer and start debugging.
    public class _weatherservice : IWeatherService
    {

        /*
         * Author: Michael Zarate
         * Creating a weather service 
         */

        //First we in order to properly use a weather service to get us the weather 
        //We need to get latitude and longitude from google.

        public Tuple<double,double> getLatAndLong(string zipcode)
        {
            //We need to set up the web request from google's geocode api
            //The reference material can be found here: https://developers.google.com/maps/documentation/geocoding/start
            string URL = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}", zipcode);
            //Now we can make a web request and pass it in the URL
            WebRequest latandlongRequest = WebRequest.Create(URL);
            //Make a response stream to hold the information comming back from the web request (since its comming back in bytes)
            Stream latandlongStream = latandlongRequest.GetResponse().GetResponseStream();
            XMLParser latandlongParser = new XMLParser(latandlongStream);
            //Set up a new parser and return the parsed values we found.
            return new Tuple<double, double>(latandlongParser.getLatitude(), latandlongParser.getLongitude());
        }

        //Now that we have a way to get zipcode converted lat and long values we can use those to get the weather forcast for that area!
        public List<string> getForcast(string zipcode, string units)
        {
            Tuple<double, double> latAndLong = getLatAndLong(zipcode);
            //check to see if we have a valid lat and long values from the zipcode
            if (latAndLong.Item1 == -2 || latAndLong.Item2 == -2)
            {
                throw new Exception(string.Format("Invalid latitude and longitued return from zipcode: {0}.  Check your zipcode and try again.", zipcode));
            }

            // this is an api key from http://openweathermap.org/api
            string apiKey = "dd0a0e2438b0e704fcdfcfd7aa49e6a5";
            string weatherURL = string.Format("http://api.openweathermap.org/data/2.5/forecast/daily?lat={0}&lon={1}&cnt=5&units={2}&APPID={3}&mode=xml",
                                            latAndLong.Item1, latAndLong.Item2, units, apiKey);
            WebRequest weatherRequest = WebRequest.Create(weatherURL);
            Stream weatherStream = weatherRequest.GetResponse().GetResponseStream();
            // now we need to parse the xml data
            XMLParser weatherParser = new XMLParser(weatherStream);
            List<dailyForcastInfo> weatherDays = weatherParser.getForcast();
            List<string> forcast = new List<string>();
            //now that we have the parsed out data in a list of structs we can use to toString method to create the output for it
            for (int i = 0; i != weatherDays.Count; i++)
            {
                forcast.Add(weatherDays[i].toString()); //since we formated the output in the struct, it will make sense
            }
            return forcast;
        }


        //This one is super easy use the methods demonstrated above we can get the location name from a zipcode  Thanks to google.
        // A zipcode of 85282 should give a location of tempe
        //You can try it by opening up a web broswer and formulating the URL yourself
        public string getLocation(string zipcode)
        {
            string locationURL = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}", zipcode);
            WebRequest locationRequest = WebRequest.Create(locationURL);
            Stream locationStream = locationRequest.GetResponse().GetResponseStream();
            XMLParser locationParser = new XMLParser(locationStream);
            return locationParser.getLocationName();
        }
    }
}
