using ConsoleTables;
using System;
using System.Collections.Generic;
using System.IO;
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
		//   public string search;
		static readonly char[] delimiterChars = new[] { ',', '.', ':' };

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
				var s = xmlDoc.InnerText;

				string[] words = s.Split ( delimiterChars );

				var keys = new List<ExtractedKey> ();
				//			var cleanList = new List<ExtractedKey> ();

				int i = 0;
				ExtractedKey key = new ExtractedKey ();
				foreach ( var word in words )
				{
					i++;

					if ( word == "\"localKey\"" )
					{
						var replace = words[i].Replace ( "\"", "" );
						key.LocalKey = replace;
					}

					if ( word == "\"devId\"" )
					{
						var replace = words[i].Replace ( "\"", "" );
						key.DeviceID = replace;
					}

					if ( word == "\"name\"" )
					{
						var replace = words[i].Replace ( "\"", "" );
						key.DeviceName = replace;

						if ( key.LocalKey != null && key.DeviceName != null )
						{
							keys.Add ( key );
							key = new ExtractedKey ();
						}
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
