using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class SecurePackage
    {
        public DateTime ClientSendingTime { get; set; }
        public DateTime ClientReceivingTime { get; set; }
        public DateTime ServerReceivingTime { get; set; }
        public string Msg { get; set; }
        public string FaceTemplate { get; set; }
        public string PalmTemplate { get; set; }
    }
}
