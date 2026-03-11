using Stability_Test_Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.Hotplate
{
    public interface IHotplate
    {
        /// <summary>
        /// 获取当前温度
        /// </summary>
        public TemperatureInfo CurrentTemperature { get; }
        /// <summary>
        /// 获取当前温度信息
        /// </summary>
        /// <returns></returns>
        public MethodResult<TemperatureInfo> GetTemperatureAsync();
        /// <summary>
        /// 建立连接，启动设备
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> Start();
        /// <summary>
        /// 断开连接，清理缓存
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> Close();
        /// <summary>
        /// 设置热台加热温度
        /// </summary>
        /// <param name="temperatureInfo"></param>
        /// <returns></returns>
        public MethodResult<bool> SetTemperature(TemperatureInfo temperatureInfo);
        /// <summary>
        /// 按设定温度开始加热
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> StartHeating();
        /// <summary>
        /// 停止加热
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> StopHeating();
    }
}
