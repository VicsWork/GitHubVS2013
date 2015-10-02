using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Description;

namespace powercal
{
    [ServiceContract]
    public interface ICalibrationService
    {
        [OperationContract]
        void Calibrate(string boardtype);
    }

}
