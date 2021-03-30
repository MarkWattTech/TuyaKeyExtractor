using System;
using System.Collections.Generic;
using System.Text;


// Class to hold information on an extracted key. Currently only holding three bits of information.
namespace TuyaKeyExtractor
{

    /// <summary>
    /// Object/Model to hold the Extracted Key
    /// </summary>
    /// 
    public class ExtractedKey
    {
        /// <summary>
        /// The name of the Device as listed in the Smart Life App.
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// The Localy Key which will be used to control the device locally.
        /// </summary>
        public string LocalKey { get; set; }
        /// <summary>
        /// The ID of the device as listed in the Smart Life App.
        /// </summary>
        public string DeviceID { get; set; }
    }
}
