using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace weather_service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWeatherService" in both code and config file together.
    [ServiceContract]
    public interface IWeatherService
    {

        // TODO: Add your service operations here

        // returns a tuple holding latitude and longitude values from a given zipcode
        [OperationContract]
        Tuple<double, double> getLatAndLong(string zipcode);

        [OperationContract]
        List<string> getForcast(string zipcode, string units);

        [OperationContract]
        string getLocation(string zipcode);
    }
}
