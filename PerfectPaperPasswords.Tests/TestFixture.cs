using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace PerfectPaperPasswords.Tests
{
	public class TestFixture
	{
		#region string[]
		protected void AssertEqual(string[] expected, string[] actual)
		{
			AssertEqual(expected, actual, null);
		}

		protected void AssertEqual(string[] expected, string[] actual, string message)
		{
			string msg = string.Empty;
			if (!string.IsNullOrEmpty(message))
				msg = String.Format(": {0}", message);

			//Check Lengths First
			Assert.AreEqual(expected.Length, actual.Length, "Array Length Mismatch" + msg);

			for (int i = 0; i < actual.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], string.Format("Array Mismatch at ordinal {0:#,0}{1}", i, msg));
			}
		}
		#endregion //string[]		

		#region byte[]
		protected void AssertEqual(byte[] expected, byte[] actual)
		{
			AssertEqual(expected, actual, null);
		}

		protected void AssertEqual(byte[] expected, byte[] actual, string message)
		{
			string msg = string.Empty;
			if (!string.IsNullOrEmpty(message))
				msg = String.Format(": {0}", message);

			//Check Lengths First
			Assert.AreEqual(expected.Length, actual.Length, "Array Length Mismatch" + msg);

			for (int i = 0; i < actual.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], string.Format("Array Mismatch at ordinal {0:#,0}{1}", i, msg));
			}
		}
		#endregion //byte[]
	}
}
