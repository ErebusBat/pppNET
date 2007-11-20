using System;
using System.Collections.Generic;
using System.Text;
using CommandLine.OptParse;
using System.IO;
using PerfectPaperPasswords.Core;
using System.Security.Cryptography;

namespace ppp
{
	static class Program
	{
		internal static CmdLineOptions Options = null;

		static void Main(string[] args)
		{
			Options = new CmdLineOptions();
			Parser p = ParserFactory.BuildParser(Options);
			p.OptStyle = OptStyle.Unix;
			p.UnixShortOption = UnixShortOption.CollapseShort;
			p.UnknownOptHandleType = UnknownOptHandleType.Warning;
			p.DupOptHandleType = DupOptHandleType.Warning;
			p.CaseSensitive = false;
			p.OptionWarning += new WarningEventHandler(p_OptionWarning);
			args = p.Parse(args);


			if (Options.ShowHelp)
			{
				p.PrintUsage(OptStyle.Unix, null, Console.Out, Console.WindowWidth);
			}
			else if (!string.IsNullOrEmpty(Options.DumpRandomFile))
			{
				DumpRandom(Options.DumpRandomFile, Options.DumpRandomSize);
			}

			if (System.Diagnostics.Debugger.IsAttached)
			{
				OutLine(string.Empty);
				OutLine("Press any key...");
				Console.ReadKey();
			}
		}

		static void p_OptionWarning(Parser sender, OptionWarningEventArgs e)
		{
			Console.WriteLine(e.WarningMessage);
		}

		#region Interface Methods
		private static void OutLine(string value)
		{
			Console.WriteLine(value);
		}

		private static void OutLine(string format, params object[] args)
		{
			Console.WriteLine(String.Format(format, args));
		}
		#endregion //Interface Methods

		#region Operation Methods
		private static void displayHelp()
		{
			OutLine("--test	Run internal integrity tests.");
		}

		static void DumpRandom(string file, int bytes)
		{
			Console.WriteLine("Dumping random data to {0}", file);
			using (FileStream fstream = new FileStream(file, FileMode.Create))
			using (BinaryWriter writer = new BinaryWriter(fstream))
			{
				//use the built in CryptoRNG to grab our sequence key
				byte[] sequenceKey = new byte[32];
				RandomNumberGenerator rng = RandomNumberGenerator.Create();
				rng.GetBytes(sequenceKey);
				CryptoRandom crand = new CryptoRandom(Rijndael.Create(), sequenceKey);

				//Now we have our crand, dump the output
				int bytesWritten = 0;
				while(bytesWritten < bytes)
				{
					byte[] buffer = new byte[4096];
					//crand.GetBytes(buffer);
					rng.GetBytes(buffer);
					writer.Write(buffer);

					bytesWritten += buffer.Length;
					if (bytesWritten % 245 == 0)
						Console.Write(".");
				}
				Console.WriteLine("Done, wrote {0:0,0} bytes", bytesWritten);
			}
		}
		#endregion //Operation Methods
	}
}
