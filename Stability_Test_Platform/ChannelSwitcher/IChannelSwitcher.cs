using Stability_Test_Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.ChannelSwitcher
{
    public interface IChannelSwitcher
    {
        /// <summary>
        /// 获取当前通道号
        /// </summary>
        public ChannelInfo CurrentChannel { get; }
        /// <summary>
        /// 建立连接，启动切换器
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> Start();
        /// <summary>
        /// 断开连接，清理缓存
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> Close();
        /// <summary>
        /// 切换为指定通道
        /// </summary>
        /// <returns></returns>
        public MethodResult<bool> ChannelSwitch(ChannelInfo channelInfo);
        /// <summary>
        /// 获取当前通道信息
        /// </summary>
        /// <returns></returns>
        public MethodResult<ChannelInfo> GetChannelAsync();
    }
}
