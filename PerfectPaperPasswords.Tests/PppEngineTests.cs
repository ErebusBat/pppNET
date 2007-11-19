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
	public class PppEngineTests
	{
		PppEngine ppp;
		byte[] sequenceKey = pppHelper.GetSequenceKeyFromPassword("zombie");

		[SetUp]
		public void Setup()
		{
			ppp = new PppEngine();
		}

		[Test]
		public void GetFirstCounterForPasscode()
		{
			//Build a dictionary of known valid combinations
			//  we use int in the test because we are not testing beyond 32bit boundry
			//We only test 0-79 which would include the entire first card and start of second
			Dictionary<int, int> knowGood = new Dictionary<int, int>();

			//0-15 belong to 0 index
			for (int i = 0; i <= 15; i++) knowGood.Add(i, 0);
			//16-31 belong to 3 index
			for (int i = 16; i <= 31; i++) knowGood.Add(i, 3);
			//32-47 belong to 6 index
			for (int i = 32; i <= 47; i++) knowGood.Add(i, 6);
			//48-63 belong to 9 index
			for (int i = 48; i <= 63; i++) knowGood.Add(i, 9);
			//64-79 belong to 12 index
			for (int i = 64; i <= 79; i++) knowGood.Add(i, 12);

			/* 
			 * End setup, start tests
			 */
			foreach (KeyValuePair<int, int> kvp in knowGood)
			{
				//kvp.Key = passcode index
				//kvp.Value = known good crypto block start index
				BigInteger expected = BigInteger.Parse(kvp.Value.ToString());
				BigInteger actual = ppp.GetFirstCounterForPasscode(kvp.Key);
				Assert.AreEqual(expected, actual, String.Format("GetCryptoBlocksContainingPasscode returned invalid crypto block index ({0}) for passcode {1}", actual, kvp.Key));
			}

		}

		[Test]
		public void GetFirstPasscodeForCounter()
		{
			//Build a dictionary of known valid combinations
			//  we use int in the test because we are not testing beyond 32bit boundry
			//We only test 0-79 which would include the entire first card and start of second
			Dictionary<int, int> knowGood = new Dictionary<int, int>();

			knowGood.Add(0, 0);
			knowGood.Add(3, 16);
			knowGood.Add(6, 32);
			knowGood.Add(9, 48);
			knowGood.Add(12, 64);

			/* 
			 * End setup, start tests
			 */
			foreach (KeyValuePair<int, int> kvp in knowGood)
			{
				//kvp.Key = passcode index
				//kvp.Value = known good crypto block start index
				BigInteger expected = BigInteger.Parse(kvp.Value.ToString());
				BigInteger actual = ppp.GetFirstPasscodeForCounter(kvp.Key);
				Assert.AreEqual(expected, actual, String.Format("GetFirstPasscodeForCounter returned invalid passcode index ({0}) for counter {1}", actual, kvp.Key));
			}

		}

		[Test]
		public void GetLastPasscodeForCounter()
		{
			//Build a dictionary of known valid combinations
			//  we use int in the test because we are not testing beyond 32bit boundry
			//We only test 0-79 which would include the entire first card and start of second
			Dictionary<int, int> knowGood = new Dictionary<int, int>();

			knowGood.Add(0, 15);
			knowGood.Add(3, 31);
			knowGood.Add(6, 47);
			knowGood.Add(9, 63);
			knowGood.Add(12, 79);

			/* 
			 * End setup, start tests
			 */
			foreach (KeyValuePair<int, int> kvp in knowGood)
			{
				//kvp.Key = passcode index
				//kvp.Value = known good crypto block start index
				BigInteger expected = BigInteger.Parse(kvp.Value.ToString());
				BigInteger actual = ppp.GetLastPasscodeForCounter(kvp.Key);
				Assert.AreEqual(expected, actual, String.Format("GetLastPasscodeForCounter returned invalid passcode index ({0}) for counter {1}", actual, kvp.Key));
			}

		}

		[Test]
		public void GetPasscodeOffsetInCryptoBlocks()
		{
			//Build a dictionary of known valid combinations
			//  we use int in the test because we are not testing beyond 32bit boundry
			//We only test 0-79 which would include the entire first card and start of second
			Dictionary<int, int> knowGood = new Dictionary<int, int>();

			for (int i = 0; i <= 15; i++) knowGood.Add(i, i);
			for (int i = 16; i <= 31; i++) knowGood.Add(i, i - 16);
			for (int i = 32; i <= 47; i++) knowGood.Add(i, i - 32);
			for (int i = 48; i <= 63; i++) knowGood.Add(i, i - 48);
			for (int i = 64; i <= 79; i++) knowGood.Add(i, i - 64);

			/* 
			 * End setup, start tests
			 */
			foreach (KeyValuePair<int, int> kvp in knowGood)
			{
				//kvp.Key = passcode index
				//kvp.Value = known good offset in crypto block
				int actual = ppp.GetPasscodeOffsetInCryptoBlocks(kvp.Key);
				int expected = kvp.Value * 3;
				Assert.AreEqual(expected, actual, String.Format("_GetPasscodeOffsetInCryptoBlocks returned invalid crypto block offset ({0}) for passcode {1}", actual, kvp.Key));
			}
		}

		[Test]
		public void RawCryptoBits()
		{
			BigInteger counter;
			byte[] crypto;
			byte[] expected;

			counter = BigInteger.Parse("0");
			crypto = ppp.GetCryptoBlockForCounter(sequenceKey, counter);
			expected = new byte[] { 0x46, 0xDD, 0x04, 0xC5, 0x28, 0xB5, 0x50, 0x6A, 0x05, 0xE8, 0x4E, 0x2A, 0x27, 0xE0, 0x6E, 0xDC };
			Assert.AreEqual(expected, crypto, string.Format("Crypto block for counter {0} failed!", counter));

			counter = BigInteger.Parse("1");
			crypto = ppp.GetCryptoBlockForCounter(sequenceKey, counter);
			expected = new byte[] { 0x2B, 0x19, 0x3B, 0x34, 0x35, 0xCA, 0x3E, 0x78, 0xFE, 0x42, 0x5A, 0x71, 0x3B, 0x57, 0x6A, 0x66 };
			Assert.AreEqual(expected, crypto, string.Format("Crypto block for counter {0} failed!", counter));
		}

		#region Exception Tests
		#region ExpandBits
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ExpandBitsThrowsOnFourBytes()
		{
			byte[] material = new byte[4];
			ppp.ExpandBitsToPassCode(material);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ExpandBitsThrowsOnTwoBytes()
		{
			byte[] material = new byte[2];
			ppp.ExpandBitsToPassCode(material);
		}
		#endregion //ExpandBits

		#region GetCryptoBlockForCounter
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetCryptoThrowsOnNullKey()
		{
			BigInteger counter = BigInteger.Parse("0");
			byte[] material = ppp.GetCryptoBlockForCounter(null, counter);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetCryptoThrowsOnNullCounter()
		{
			byte[] key = new byte[32];
			byte[] material = ppp.GetCryptoBlockForCounter(key, null);
		}
		#endregion //GetCryptoBlockForCounter
		#endregion //Exception Tests
	}
}
