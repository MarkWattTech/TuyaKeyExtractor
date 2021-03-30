using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TuyaKeyExtractor
{
    /// <summary>
    /// This class is where the parsing happens. <inheritdoc'm using a lazy string split to just brake down the inner text from the malformed xml.
    /// The XML appears to be malformed and I did this at 2:30am :P Feel free to improve it and make it more efficient if you wish./>
    /// </summary>
    public class KeyExtract
    {
        public string search;

        /// <summary>
        /// This method splits the strings and decides how the result should be printed.
        /// </summary>
        /// <param name="path"> The path to the XML file</param>
        /// <param name="option"> The option for which print to be used</param>
        public void ReadXml(string path, int option)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            var s = xmlDoc.InnerText;

            char[] delimiterChars = { ',', '.', ':' };
            string[] words = s.Split(delimiterChars);

            List<ExtractedKey> keys = new List<ExtractedKey>();

            int i = 0;
            ExtractedKey key = new ExtractedKey();
            foreach (var word in words)
            {
                i++;


                if (word == "\"localKey\"")
                {
                    var replace = words[i].Replace("\"", "");
                    key.LocalKey = replace;
                }

                if (word == "\"devId\"")
                {
                    var replace = words[i].Replace("\"", "");
                    key.DeviceID = replace;
                }

                if (word == "\"name\"")
                {
                    var replace = words[i].Replace("\"", "");
                    key.DeviceName = replace;

                    if (key.LocalKey != null && key.DeviceName != null)
                    {
                        keys.Add(key);
                        key = new ExtractedKey();
                    }
                }


            }
            // Standard Print
            if (option == 1)
            {
                foreach (ExtractedKey k in keys)
                {
                    if (k.DeviceID != null && k.DeviceName != null && k.LocalKey != null)
                    {
                        Console.WriteLine("\nDevice Name is : " + k.DeviceName);
                        Console.WriteLine("Local Key is   : " + k.LocalKey);
                        Console.WriteLine("Device ID is   : " + k.DeviceID);
                        Console.WriteLine("");
                       
                    }
                }
            }
            // Fancy Print (Using Tables)
            if (option == 2)
            {
                var table = new ConsoleTable("Name", "Local Key", "Device ID");
                foreach (ExtractedKey k in keys)
                {
                    if (k.DeviceID != null && k.DeviceName != null && k.LocalKey != null)
                    {
                        table.AddRow(k.DeviceName, k.LocalKey, k.DeviceID);
                    }
                }
                table.Write();

            }
            // Print searched ID
            if (option == 3)
            {
                foreach (ExtractedKey k in keys)
                {
                    if (k.DeviceID == search)
                    {
                        Console.WriteLine("Device Name is : " + k.DeviceName);
                        Console.WriteLine("Local Key is   : " + k.LocalKey);
                        Console.WriteLine("Device ID is   : " + k.DeviceID);
                        Console.WriteLine("");
                        break;
                    }
                }
            }
        }
    }
}
