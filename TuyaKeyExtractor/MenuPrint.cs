
using System;
using System.Collections.Generic;

namespace TuyaKeyExtractor
{
	/// <summary>
	/// A class just to hold bulky menu prints for the console.
	/// </summary>
	public class MenuPrint
	{
		public List<string> MenuOptions = new List<string> ();
		public List<string> AllCommands = new List<string> ();

		public MenuPrint ()
		{
			BuildMenu ();
			BuildAllCommands ();
		}

		public bool IsPathSet { get { return !string.IsNullOrWhiteSpace ( ConfigFilePath ); } }
		public string PathStr { get { return IsPathSet ? ConfigFilePath : "Not Set"; } }
		public string ConfigFilePath { get; private set; }

		public bool SetConfigFilePath ( string path )
		{
			ConfigFilePath = path;
			return IsPathSet;
		}

		private void BuildMenu ()
		{
			MenuOptions.Add ( $"  1   -   Set Config Filepath        |    [ PATH : {PathStr} ]" );
			MenuOptions.Add ( "  2   -   Print All Keys             | " );
			MenuOptions.Add ( "  3   -   Print All Keys Fancy       | " );
			MenuOptions.Add ( "  4   -   Print Key by ID            | " );
			MenuOptions.Add ( "  5   -   Generate .CSV              | " );
			MenuOptions.Add ( "  6   -   Generate .TXT              | " );
			MenuOptions.Add ( "  E   -   Extras                     | " );
			MenuOptions.Add ( "  S   -   SUBSCRIBE TO MarkWattTech  | " );
			MenuOptions.Add ( "  Q   -   Quit                       |" );
		}

		private void ReBuildMenu ()
		{
			DisplayMenu ( 1 );
		}

		private void BuildAllCommands ()
		{
			foreach ( string str in MenuOptions )
			{
				AllCommands.Add ( str );
			}

			AllCommands.Add ( "                                     | " );
			AllCommands.Add ( "  M   -   Main Menu                  |" );
			AllCommands.Add ( "  A   -   About this tool            |" );
			AllCommands.Add ( "                                     | " );
			AllCommands.Add ( "  Links for all My Socials           |" );
			AllCommands.Add ( "  I   -   Instagram                  | " );
			AllCommands.Add ( "  F   -   Facebook                   | " );
			AllCommands.Add ( "  FF   -   Facebook Group            | " );
			AllCommands.Add ( "  R   -   Reddit                     | " );
			AllCommands.Add ( "  G   -   GitHub for this Tool       | " );
			AllCommands.Add ( "                                     | " );
			AllCommands.Add ( "  Want to help me out?               |" );
			AllCommands.Add ( "  P   -   Patreon                    |" );
			AllCommands.Add ( "  B   -   BuyMeACoffee               |" );
			AllCommands.Add ( "                                     | " );
			AllCommands.Add ( "  RELATED VIDEOS                     |" );
			AllCommands.Add ( "  V   -   Video on Local Tuya        |" );
			AllCommands.Add ( "  H   -   How to use this tool       |" );
			AllCommands.Add ( "                                     | " );
			AllCommands.Add ( "  Press 'M' to return to Basic Menu  |" );
		}

		public void DisplayMenu ( int menu )
		{
			if ( menu == 1 )
			{
				MenuOptions.Clear ();
				BuildMenu ();
				DisplayHeading ();
				foreach ( string str in this.MenuOptions )
				{
					Console.WriteLine ( str );
				}
			}

			if ( menu == 2 )
			{
				AllCommands.Clear ();
				BuildAllCommands ();
				DisplayHeading ();
				foreach ( string str in this.AllCommands )
				{
					Console.WriteLine ( str );
				}
			}
		}

		public void DisplayHeading ()
		{
			Console.WriteLine ( "===================================================================================\n" );

			Console.WriteLine ( "   ███╗   ███╗    ██╗    ██╗    ████████╗" );
			Console.WriteLine ( "   ████╗ ████║    ██║    ██║    ╚══██╔══╝" );
			Console.WriteLine ( "   ██╔████╔██║    ██║ █╗ ██║       ██║   " );
			Console.WriteLine ( "   ██║╚██╔╝██║    ██║███╗██║       ██║   " );
			Console.WriteLine ( "   ██║ ╚═╝ ██║    ╚███╔███╔╝       ██║   -  Tuya Key Extractor " );
			Console.WriteLine ( "   ╚═╝     ╚═╝     ╚══╝╚══╝        ╚═╝   \n" );

			Console.WriteLine ( "===================================================================================\n" );
		}
	}
}
