using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Authentication.Request
{
    public class VerifyDeviceRequest
    {
        public bool IsNewUser { get; set; }
        public string ActivationCode { get; set; }
        public string DeviceToken { get; set; }
    }
}
