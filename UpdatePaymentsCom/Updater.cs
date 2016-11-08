using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace UpdatePaymentsCom
{
    [Guid("EFC77FE9-0719-40AE-991C-9431993277CA")]
    public interface IUpdateDLL
    {
        [DispId(1)]
        string CheckDLL(string version);

        [DispId(2)]
        string Check418Version(string version);

        [DispId(3)]
        string GetSiteAdress();
    }
}
