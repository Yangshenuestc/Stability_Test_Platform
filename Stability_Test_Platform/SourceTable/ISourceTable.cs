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
        /// 以ISOS-LC-1/2模式进行测试（光暗1sun1dark，不加热）
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> TestMode1();
        /// <summary>
        /// 以ISOS-L-1模式进行测试（持续光照）
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> TestMode2();
        /// <summary>
        /// 以ISOS-L-2模式进行测试（光热，持续光照并且以一定温度稳定）
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> TestMode3();
        /// <summary>
        /// 停止测试，并且复位相应源表
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> StopTest();


    }
}
