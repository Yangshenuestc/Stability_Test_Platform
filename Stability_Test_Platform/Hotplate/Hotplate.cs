using Stability_Test_Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.Hotplate
{
    public class Hotplate : IHotplate
    {
        public TemperatureInfo CurrentTemperature => throw new NotImplementedException();

        public MethodResult<bool> Close()
        {
            throw new NotImplementedException();
        }

        public MethodResult<TemperatureInfo> GetTemperatureAsync()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> SetTemperature(TemperatureInfo temperatureInfo)
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> Start()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> StartHeating()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> StopHeating()
        {
            throw new NotImplementedException();
        }
    }
}
