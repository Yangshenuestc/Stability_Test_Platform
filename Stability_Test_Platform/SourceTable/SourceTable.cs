using Stability_Test_Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stability_Test_Platform.SourceTable
{
    public class SourceTable : ISourceTable
    {
        public MethodResult<bool> IVMode(ElectricalInfo electricalInfo)
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> Start()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> Close()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> StopTest()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> TestMode_Jsc()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> TestMode_Vmpp()
        {
            throw new NotImplementedException();
        }

        public MethodResult<bool> TestMode_Voc()
        {
            throw new NotImplementedException();
        }
    }
}
