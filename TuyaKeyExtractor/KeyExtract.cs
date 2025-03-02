using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Newtonsoft.Json;

namespace TuyaKeyExtractor
{
	/// <summary>
	/// This class is where the parsing happens. <inheritdoc'm using a lazy string split to just brake down the inner text from the malformed xml.
	/// The XML appears to be malformed and I did this at 2:30am :P Feel free to improve it and make it more efficient if you wish./>
	/// </summary>
	public class KeyExtract
	{

		/// <summary>
		/// Parse the given data file and extract keys from it.
		/// </summary>
		/// <param name="path"> The path to the XML file</param>
		/// <returns></returns>
		private IList<ExtractedKey> ParseXmlFile ( string path )
		{
			XmlDocument xmlDoc = new XmlDocument ();

			try
			{
				xmlDoc.Load ( path );
				var keys = new List<ExtractedKey> ();

				// this file looks like:
				// <?xml stuff>
				// <map>
				//  <otherstuff>
				//  <string ...> possibly irrelevant </string>
				//  <string ...> HTML-encoded JSON (relevant) </string>
				var map = xmlDoc.GetElementsByTagName ( "map" );
				
				// parse the 'string' subnodes of the parent node
				foreach ( XmlNode node in map[0].ChildNodes) {

					// Make a description of the node for error messages.
					string description = null;

					var attr = node.Attributes["name"];
					if ( attr != null ){
						description=$"Node {node.Name} \"{attr.Value}\"";
					} else {
						description=$"Node {node.Name} (no name attribute)";
					}

					if (node.Name != "string") {
						// several nodes, but we only care about the "string" nodes.
						// Console.WriteLine( $"{description} is not a string.  Skipped");
						continue;
					}
					// otherwise...
					// the string inside appears to be HTML-encoded JSON.

					string innertext = node.InnerText;
					
					try {
						innertext = HttpUtility.HtmlDecode( node.InnerText);
					} catch (Exception) {
						// is this even possible?
						Console.WriteLine( $"Invalid HTML at {description}.  Using it anyway." );
					}

					dynamic j;
					try {
						j = JsonConvert.DeserializeObject<dynamic>(innertext);
					} catch( Exception ) {
						// there are other "string" nodes that aren't relevant to us,
						// and which are not JSON.  Skip those.
						// this seems pretty expected, so don't emit an error.
						// Console.WriteLine( $"Invalid JSON at {node.Name}.  Skipped.");
						continue;
					}

					// otherwise, the interior should look like:
					// {
					//  "deviceRespBeen": [
					//     { "localKey": "key", "devId": "id", "name": "name", ... },
					//     { "localKey": "key", "devId": "id", "name": "name", ... },
					//    ...
					//   ]

					try {
						var devices = j["deviceRespBeen"];
						foreach (var device in devices) {
							var key = new ExtractedKey();
							key.LocalKey = device["localKey"];
							key.DeviceID = device["devId"];
							key.DeviceName = device["name"];
							// Console.WriteLine( $"Found key in {description} for {key.DeviceName} ({key.DeviceID})");
							keys.Add( key );
						}
					} catch (Exception) {
						// if "name" is not an attribute, it'll return null, which is fine.
						// There are objects in the JSON with different structures (or none),
						// so just skip it.
						// Console.WriteLine( $"Bad JSON structure at {description}.  Skipped.");
						continue;
					}


				}
				
				return keys;
			}
			catch ( System.IO.FileNotFoundException )
			{
				Console.WriteLine ( $"ERROR - It looks like your entered Path: \"{path}\" isn't valid. Try setting it again.\n\n" );
			}
			return null;
		}

		/// <summary>
		/// This method splits the strings and decides how the result should be printed.
		/// </summary>
		/// <param name="path"> The path to the XML file</param>
		/// <param name="option"> The option for which print to be used</param>
		public void ReadXml ( string path, int option )
		{
			var keys = ParseXmlFile ( path );
			if ( keys == null )
				return;

			switch ( option )
			{
			case 1: // Standard Print
				foreach ( ExtractedKey k in CleanKeys ( keys ) )
				{
					Console.WriteLine ( "\nDevice Name is : " + k.DeviceName );
					Console.WriteLine ( "Local Key is   : " + k.LocalKey );
					Console.WriteLine ( "Device ID is   : " + k.DeviceID );
					Console.WriteLine ( "" );
				}
				break;

			case 2: // Fancy Print (Using Tables)
				var table = new ConsoleTable ( "Name", "Local Key", "Device ID" );
				foreach ( ExtractedKey k in CleanKeys ( keys ) )
				{
					table.AddRow ( k.DeviceName, k.LocalKey, k.DeviceID );
				}
				table.Write ();
				break;

			case 4: // Generate .CSV List
				try
				{
					File.WriteAllLines ( "Tuya Local Keys.csv", MakeCollectionToWrite ( keys, ',' ) );
					Console.WriteLine ( "Successfully Generated Tuya Local Keys.csv" );
				}
				catch ( System.IO.IOException ie )
				{
					Console.WriteLine ( "ERROR - Can't update the File as it is in use. Have you got it open?\n\n" + ie.Message );
				}
				catch ( Exception e )
				{
					Console.WriteLine ( e.Message );
				}
				break;

			case 5: // Generate .txt 
				try
				{
					File.WriteAllLines ( "Tuya Local Keys.txt", MakeCollectionToWrite ( keys, '\t' ) );
					Console.WriteLine ( "Successfully Generated Tuya Local Keys.txt" );
				}
				catch ( System.IO.IOException ie )
				{
					Console.WriteLine ( "ERROR - Can't update the File as it is in use. Have you got it open?\n\n" + ie.Message );
				}
				catch ( Exception e )
				{
					Console.WriteLine ( e.Message );
				}
				break;
			}
		}

		public void SearchKeyInXml ( string path, string search )
		{
			var keys = ParseXmlFile ( path );
			if ( keys == null )
				return;

			var k = keys.FirstOrDefault ( k => 0 == string.Compare ( k.DeviceID, search, ignoreCase: true ) );
			if ( k == null )
			{
				// Not found...
				return;
			}

			Console.WriteLine ( "Device Name is : " + k.DeviceName );
			Console.WriteLine ( "Local Key is   : " + k.LocalKey );
			Console.WriteLine ( "Device ID is   : " + k.DeviceID );
			Console.WriteLine ( "" );
		}

		private static IEnumerable<ExtractedKey> CleanKeys ( IEnumerable<ExtractedKey> inputList )
		{
			return inputList.Where ( k => ( k.DeviceID != null && k.DeviceName != null && k.LocalKey != null ) )
				.Select ( k => new ExtractedKey ( k.DeviceName, k.LocalKey, k.DeviceID ) );
		}

		private static IEnumerable<string> MakeCollectionToWrite ( IEnumerable<ExtractedKey> inputList, char separator )
		{
			return ( new string[] { string.Join ( separator, new[] { "Name", "Local Key", "Device ID" } ) } )
				.Concat ( CleanKeys ( inputList )
					.Select ( k => string.Join ( separator, k.DeviceName, k.LocalKey, k.DeviceID ) ) );
		}
	}
}
