using Stability_Test_Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.ChannelSwitcher
{
    public class ChannelSwitcher : IChannelSwitcher
    {

        private ChannelInfo _currentChannel=new ChannelInfo();
        public ChannelInfo CurrentChannel
        {
            get
            {
                GetChannelAsync();
                return _currentChannel;
            }
        }

        public MethodResult<bool> ChannelSwitch(ChannelInfo channelInfo)
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> Close()
        {
            throw new NotImplementedException();
        }

        public MethodResult<ChannelInfo> GetChannelAsync()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> Start()
        {
            throw new NotImplementedException();
        }
    }
}
