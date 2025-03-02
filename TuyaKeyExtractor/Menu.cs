﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TuyaKeyExtractor
{
	/// <summary>
	/// This class houses the main logic that is used in the menu (switch statement).
	/// </summary>
	public class Menu
	{
		private MenuPrint _print = new MenuPrint ();
		private KeyExtract _extract = new KeyExtract ();

		/// <summary>
		/// Initialise and start the menu.
		/// </summary>
		public void InitialiseMenu ()
		{
			MenuEntry ( null, _print.MenuOptions, 1 );
		}

		/// <summary>
		/// This displays the menu for a chosen List. It also adds the headers and any optional menu text.
		/// </summary>
		/// <param name="menutext"></param>
		/// <param name="menuOptions"></param>
		/// <param name="menuToDisplay"></param>
		public void MenuEntry ( string menutext, List<string> menuOptions, int menuToDisplay )
		{
			ClearConsole ();
			Console.Title = $"Mark Watt Tech - Tuya Key Extractor (v0.05)";

			_print.DisplayMenu ( menuToDisplay );
			if ( string.IsNullOrEmpty ( menutext ) )
			{
				Console.Write ( "\n Enter a Menu Option : " );
			}
			else
			{
				Console.Write ( "\n" + menutext + " : " );
			}
			string input = Console.ReadLine ();
			ProcessInput ( input );
		}

		/// <summary>
		/// This is the main switch statement that contols all of the inputted menu logic.
		/// </summary>
		/// <param name="input"></param>
		public void ProcessInput ( string input )
		{
			bool skipContinue = false;
			if ( input != null )
			{
				Console.WriteLine ( "Entered : " + input );
				switch ( input.ToUpper () )
				{
				case "A":
					AboutTool ();
					break;

				case "E":
					MenuEntry ( null, _print.AllCommands, 2 );
					break;

				case "1":
					Console.WriteLine ( "Enter the file path for the Preference.xml file   (e.g. C:\\Users\\MarkWattTech\\Desktop\\Preference.xml)" );
					string readIn = Console.ReadLine ();
					if ( _print.SetConfigFilePath ( readIn.Replace ( "\"", string.Empty ).Replace ( "'", string.Empty ) ) )
					{
						Console.WriteLine ( "PATH has been set" );
					}
					else
					{
						Console.WriteLine ( "ERROR - Enter a valid file path" );
					}
					break;

				case "2":
					// Print All Keys
					if ( _print.IsPathSet )
					{
						_extract.ReadXml ( _print.ConfigFilePath, 1 );
					}
					else
					{
						ShowPathError ();
					}
					break;

				case "3":
					// Print All Keys FANCY
					if ( _print.IsPathSet )
					{
						_extract.ReadXml ( _print.ConfigFilePath, 2 );
					}
					else
					{
						ShowPathError ();
					}
					break;

				case "4":
					// Search by ID
					if ( _print.IsPathSet )
					{
						Console.Write ( "Enter the Device ID you would like the local key for : " );
						string deviceSearch = Console.ReadLine ();

						if ( deviceSearch != null && deviceSearch != string.Empty )
						{
							_extract.SearchKeyInXml ( _print.ConfigFilePath, deviceSearch );
						}
						else
						{
							Console.WriteLine ( "Invalid Search... Please try again." );
						}
					}
					else
					{
						ShowPathError ();
					}
					break;

				// Generate .CSV File
				case "5":
					if ( _print.IsPathSet )
					{
						_extract.ReadXml ( _print.ConfigFilePath, 4 );
					}
					else
					{
						ShowPathError ();
					}
					break;

				// Generate .TXT file
				case "6":
					if ( _print.IsPathSet )
					{
						_extract.ReadXml ( _print.ConfigFilePath, 5 );
					}
					else
					{
						ShowPathError ();
					}
					break;

				case "T":
					_extract.ReadXml ( _print.ConfigFilePath, 4 );
					break;

				// Subscribe to Mark Watt Tech
				case "S":
					UrlOpener ( "http://www.youtube.com/channel/UCQRm_z7seHnGsBiWDNEWr6A?sub_confirmation=1" );
					break;
				// Instagram
				case "I":
					UrlOpener ( "https://www.instagram.com/markwatttech/" );
					break;
				// Facebook
				case "F":
					UrlOpener ( "https://www.facebook.com/MarkWattTech" );
					break;
				// Facebook Group
				case "FF":
					UrlOpener ( "hhttps://www.facebook.com/groups/2963936147172102/" );
					break;
				// Reddit
				case "R":
					UrlOpener ( "https://www.reddit.com/r/MarkWattTech/" );
					break;
				// GitHub
				case "G":
					UrlOpener ( "https://github.com/MarkWattTech/TuyaKeyExtractor" );
					break;
				// Patreon
				case "P":
					UrlOpener ( "https://www.patreon.com/markwatttech" );
					break;
				// BuyMeACoffee
				case "B":
					UrlOpener ( "https://www.buymeacoffee.com/MarkWattTech" );
					break;
				// Video related to the tool
				case "V":
					UrlOpener ( "https://youtu.be/YKvGYXw-_cE" );
					break;
				// How to use the tool
				case "H":
					UrlOpener ( "https://youtu.be/F00_4jDk06g" );
					break;

				case "Q":
					Console.WriteLine ( "Thanks for checking out the tool!" );
					System.Environment.Exit ( 0 );
					break;

				default:
					ClearConsole ();
					Console.WriteLine ();
					MenuEntry ( " Unknown Try again", _print.MenuOptions, 1 );
					break;
				}

				if ( skipContinue == false )
				{
					Console.WriteLine ( "\n Press Enter to Continue." );
					Console.ReadLine ();
				}
			}
			MenuEntry ( null, _print.MenuOptions, 1 );
		}

		private void AboutTool ()
		{
			Console.WriteLine ( "\n===================================================================================================" );
			Console.WriteLine ( "I (Mark Watt) created this tool as a simple way for me to read my local keys from my\n" +
				"extracted SmartLife config. I decided to share this tool with others as it may help to simplify the process\n" +
				"for others. This is just something I quickly put together to fit my need. If anybody wants to add and\n" +
				"contribute to it, then please feel free to put in pull requests. Or if theres a small change you would\n" +
				"like me to make then let me know and I can try and get it added!" +
				"\nAs it's something I built at 2:30am it's not the most efficient or well written but it does the job as expected!\n" +
				"I may try and tidy it up and standardise it. Who knows :P" +
				"\n\nThe tool is designed to be used with my Local Tuya Key Extraction video.\n\nCheers." +
				"" +
				"\n\nIf you have found this tool useful and aren't subscribed, then please HIT that SUBSCRIBE button!" );
			Console.WriteLine ( "===================================================================================================\n" );
		}

		private void ShowPathError ()
		{
			Console.WriteLine ( "ERROR - You need to specify the filepath for the preference.xml file first." );
		}

		public void ClearConsole ()
		{
			try {
				Console.Clear ();
			} catch ( Exception e ) {
				Console.WriteLine ( "Failed to clear console - " + e.Message );
			}
		}

		public void UrlOpener ( string url )
		{
			Process myProcess = new Process ();

			try
			{
				// true is the default, but it is important not to set it to false
				myProcess.StartInfo.UseShellExecute = true;
				myProcess.StartInfo.FileName = url;
				;
				myProcess.Start ();
			}
			catch ( Exception e )
			{
				Console.WriteLine ( "FAILED TO OPEN URL - " + e.Message );
			}
		}
	}
}
