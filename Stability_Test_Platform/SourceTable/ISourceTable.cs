using Stability_Test_Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.SourceTable
{
    public interface ISourceTable
    {
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
        /// 切换为Jsc测试通道:电压源，电流测量（V_target = 0 V）
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> TestMode_Jsc();
        /// <summary>
        /// 切换为Voc测试通道:电流源，电压测量（I_target = 0 A）
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> TestMode_Voc();
        /// <summary>
        /// 切换为Vmpp测试通道:电压源，电流测量（V_target = V_mpp）
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> TestMode_Vmpp();
        /// <summary>
        /// IV曲线的测量
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> IVMode(ElectricalInfo electricalInfo);
        /// <summary>
        /// 停止测试，并且复位相应源表
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> StopTest();


    }
}
