using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using PerfectPaperPasswords.Math;
using Mono.Math;
using System.Diagnostics;
using PerfectPaperPasswords.Core;

namespace PerfectPaperPasswords.Tests
{
	[TestFixture]
	public class PppEngineV3Tests
	{
		PppV3Engine ppp;
		byte[] sequenceKey = pppHelper.GetSequenceKeyFromPassword("zombie");
		string alphabet = "23456789!@#%+=:?abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPRSTUVWXYZ";

		#region helper
		private void testCodeBlock(char[] actualCodeBlock, string[] knownGood)
		{
			int index = 0;
			int codeLength = knownGood[0].Length;
			for (int i = 0; i < knownGood.Length; i++)
			{
				StringBuilder code = new StringBuilder();
				for (int offset = 0; offset < codeLength; offset++)
				{
					code.Append(actualCodeBlock[index + offset]);
				}
				Assert.AreEqual(knownGood[i], code.ToString(), "Code mismatch @ {0:0,0}", i);
				index += codeLength;
			}
		}
		#endregion //helper

		[SetUp]
		public void Setup()
		{
			ppp = new PppV3Engine(sequenceKey, alphabet);
		}

		[Test]
		public void CalculatePasscodesPerBlock_V2Params()
		{
			//Tests based on the PPPv2 parameters of 4 and 64
			int actual = ppp.CalculatePasscodesPerBlock(4, 64);
			Assert.AreEqual(5, actual);
		}

		[Test]
		public void CalculatePasscodesPerBlock_OtherParams()
		{
			int actual = ppp.CalculatePasscodesPerBlock(3, 64);
			Assert.AreEqual(7, actual);
		}

		[Test]
		public void Newgroup5518_First5Test()
		{
			BigInteger counter = BigInteger.Parse("0");
			string[] knownGood = { "4p=#", "3WCg", "AcG#", "bvX8", "a!hM" };
			char[] rawCodeData = ppp.GetCounterCodeBytes(counter);
			testCodeBlock(rawCodeData, knownGood);
		}

		[Test]
		public void Newsgroup5518_Second5Test()
		{
			BigInteger counter = BigInteger.Parse("1");
			string[] knownGood = { "eXk?", "oEWm", "yTaz", "%cFN", "vNYL" };
			char[] rawCodeData = ppp.GetCounterCodeBytes(counter);
			testCodeBlock(rawCodeData, knownGood);
		}

		[Test]
		public void Newsgroup5518_BlockSpanningFirst10()
		{
			BigInteger counter = BigInteger.Parse("0");
			string[] knownGood = { "4p=#", "3WCg", "AcG#", "bvX8", "a!hM", "eXk?", "oEWm", "yTaz", "%cFN", "vNYL" };
			char[] rawCodeData = ppp.GetCounterCodeBytes(counter);
			testCodeBlock(rawCodeData, knownGood);
		}
	}
}
