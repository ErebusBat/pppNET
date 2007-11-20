using System;
using System.Collections.Generic;
using System.Text;
using CommandLine.OptParse;
using System.ComponentModel;

namespace ppp
{
	class CmdLineOptions
	{
		#region Flags
		private bool _showHelp;
		[OptDef(OptValType.Flag)]
		[ShortOptionName('?')]
		[LongOptionName("-help")]
		[UseNameAsLongOption(false)]
		[Description("Show usage instructions")]
		public bool ShowHelp
		{
			get
			{
				return _showHelp;
			}
			set
			{
				_showHelp = value;
			}
		}
		#endregion //Flags

		#region Values

		private int _dumpRandomSize = 4096*256*10;	//4096*256==1MB=1048576
		[OptDef(OptValType.ValueOpt)]
		[LongOptionName("dump-random-size")]
		[UseNameAsLongOption(false)]
		[Description("Size of the file (in KB) to generate when --dump-random is specified")]
		public int DumpRandomSize
		{
			get
			{
				return _dumpRandomSize;
			}
			set
			{
				_dumpRandomSize = value;
			}
		}

		private string _dumpRandomFile;
		[OptDef(OptValType.ValueOpt)]
		[LongOptionName("dump-rand")]
		[LongOptionName("dump-random")]
		[UseNameAsLongOption(false)]
		[Description("Dumps raw random data from the internal RNG to the specified file for analysis.")]
		public string DumpRandomFile
		{
			get
			{
				return _dumpRandomFile;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("dump-random", "You must specify a file name!");
				_dumpRandomFile = value;
			}
		}
		#endregion //Values
	}
}
