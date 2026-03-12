using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.SemiconductorCooling
{
    public class TemperatureInfo
    {
        //设定温度
        public double TargetT { get; set; }

        //当前温度
        public double CurrentT { get; set; }
    }
}
