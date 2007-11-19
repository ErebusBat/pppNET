using System;
using System.Collections.Generic;
using System.Text;
using ppp.tests;

namespace ppp
{
	static class Program
	{
		static void Main(string[] args)
		{
			if(args.Length == 0)
			{
				displayHelp();
			}
			else if(string.Compare(args[0], "-test", true) != -1)
			{
				runTests();
			}

			if(System.Diagnostics.Debugger.IsAttached)
			{
				OutLine(string.Empty);
				OutLine("Press any key...");
				Console.ReadKey();
			}
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

		private static void runTests()
		{
			IntegrityTests tests = new IntegrityTests();
			
			//If we don't get an exception then they all passed.
			tests.RunTests();

			OutLine(string.Empty);
			OutLine("All tests passed!");
		}
		#endregion //Operation Methods
	}
}
