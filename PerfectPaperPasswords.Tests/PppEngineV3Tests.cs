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
		BigInteger counter;
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
			counter = BigInteger.Parse("0");
		}

		[Test]
		public void Dump()
		{
			char[] bytes = ppp.GetCounterCodeBytes(counter);
		}

		[Test]
		public void Newgroup5518_First5Test()
		{
			string[] knownGood = { "4p=#", "3WCg", "AcG#", "bvX8", "a!hM" };
			char[] rawCodeData = ppp.GetCounterCodeBytes(counter);
			testCodeBlock(rawCodeData, knownGood);
		}
	}
}
