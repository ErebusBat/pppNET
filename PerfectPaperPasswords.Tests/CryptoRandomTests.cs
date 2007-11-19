using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using MbUnit.Framework;
using PerfectPaperPasswords.Core;
using Mono.Math;

namespace PerfectPaperPasswords.Tests
{
	[TestFixture]
	public class CryptoRandomTests
	{
		byte[] zerobytes = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
		byte[] block0 = { 0x46, 0xdd, 0x04, 0xc5, 0x28, 0xb5, 0x50, 0x6a, 0x05, 0xe8, 0x4e, 0x2a, 0x27, 0xe0, 0x6e, 0xdc };
		byte[] block1 = { 0x2b, 0x19, 0x3b, 0x34, 0x35, 0xca, 0x3e, 0x78, 0xfe, 0x42, 0x5a, 0x71, 0x3b, 0x57, 0x6a, 0x66 };
		byte[] block2 = { 0x7f, 0x73, 0xd2, 0xce, 0x12, 0xd1, 0xcd, 0xc5, 0x05, 0x7a, 0x01, 0x66, 0xd7, 0xa5, 0xa1, 0xb8 };

		CryptoRandom cryptrnd;

		[SetUp]
		public void Setup()
		{
			SymmetricAlgorithm cryptoAlgorithm = Rijndael.Create();
			byte[] sequenceKey = pppHelper.GetSequenceKeyFromPassword("zombie");
			BigInteger counter = BigInteger.Parse("0");
			cryptrnd = new CryptoRandom(cryptoAlgorithm, sequenceKey, counter);

			Assert.AreEqual(128 / 8, zerobytes.Length, "Zero byte array mismatch!");
		}

		[TearDown]
		public void TearDown()
		{
			cryptrnd = null;
		}

		[Test]
		public void GetBytes_NonZero()
		{
			byte[] actual = new byte[16];

			cryptrnd.GetBytes(actual);
			Assert.AreNotEqual(zerobytes, actual, "GetBytes() returned all zeros!");
		}

		[Test]
		public void GetBytes_FirstThreeBlocks()
		{
			byte[] actual;

			actual = new byte[16];

			cryptrnd.GetBytes(actual);
			AreEqual(block0, actual, "GetBytes() did not match expected value for counter=0!");

			cryptrnd.GetBytes(actual);
			AreEqual(block1, actual, "GetBytes() did not match expected value for counter=1!");

			cryptrnd.GetBytes(actual);
			AreEqual(block2, actual, "GetBytes() did not match expected value for counter=2!");
		}

		[Test]
		public void GetBytes_MultipleReads()
		{
			byte[] expected;
			byte[] actual;
			actual = new byte[8];
			expected = new byte[actual.Length];

			//First Part - 0
			Array.Copy(block0, 0, expected, 0, 8);
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting first 8 bytes of first block 0");

			//Second Part - 0
			Array.Copy(block0, 8, expected, 0, 8);
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting last 8 bytes of first block 0");

			//First Part - 1
			Array.Copy(block1, 0, expected, 0, 8);
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting first 8 bytes of second block 1");

			//Second Part - 1
			Array.Copy(block1, 8, expected, 0, 8);
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting last 8 bytes of block 1");
		}

		[Test]
		public void GetBytes_BlockSpanning()
		{
			byte[] expected;
			byte[] actual;

			//0(0-13)=14 bytes
			actual = new byte[14];
			expected = new byte[actual.Length];
			Array.Copy(block0, 0, expected, 0, expected.Length);
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting bytes 0-13 from block 0");

			//0(14,15)+1(0,1)=4 bytes
			actual = new byte[4];
			expected = new byte[actual.Length];
			Array.Copy(block0, 14, expected, 0, 2);	//0(14,15)
			Array.Copy(block1, 0, expected, 2, 2);		//1(0,1)
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting 0(14,15)+1(0,1)=4 bytes");

			//1(2-5)=4 bytes
			actual = new byte[4];
			expected = new byte[actual.Length];
			Array.Copy(block1, 2, expected, 0, expected.Length);
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting 1(2-5)=4 bytes");

			//1(6-9)=4 bytes
			actual = new byte[4];
			expected = new byte[actual.Length];
			Array.Copy(block1, 6, expected, 0, expected.Length);
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting 1(6-9)=4 bytes");

			//1(10-15)+2(0,1)=8 bytes
			actual = new byte[8];
			expected = new byte[actual.Length];
			Array.Copy(block1, 10, expected, 0, 6);	//1(10-15)
			Array.Copy(block2, 0, expected, 6, 2);		//2(0,1)
			cryptrnd.GetBytes(actual);
			AreEqual(expected, actual, "GetBytes() was expecting 1(10-15)+2(0,1)=8 bytes");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ctor_NullSeqKeyNotAllowed()
		{
			//Have to use alternate instance because we are testing
			//the ctor.
			CryptoRandom crnd = new CryptoRandom(Rijndael.Create(), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ctor_NullSeqLength()
		{
			//Have to use alternate instance because we are testing
			//the ctor.
			CryptoRandom crnd;
			crnd = new CryptoRandom(Rijndael.Create(), new byte[16]); //Will succeed
			crnd = new CryptoRandom(Rijndael.Create(), new byte[15]); //Will succeed
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ctor_NullCryptoNotAllowed()
		{
			//Have to use alternate instance because we are testing
			//the ctor.
			CryptoRandom crnd = new CryptoRandom(null, new byte[16]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ctor_NullCounterNotAllowed()
		{
			//Have to use alternate instance because we are testing
			//the ctor.
			CryptoRandom crnd = new CryptoRandom(Rijndael.Create(), new byte[16], null);
		}

		#region helpers
		string DumpBytes(byte[] data)
		{
			StringBuilder str = new StringBuilder("byte[] block = {");
			int initalLen = str.Length;
			for (int i = 0; i < data.Length; i++)
			{
				if (str.Length > initalLen)
					str.Append(", ");
				str.AppendFormat("0x{0:x2}", data[i]);
			}
			str.Append("};");
			return str.ToString();
		}

		void AreEqual(byte[] expected, byte[] actual, string message)
		{
			if (expected.Length != actual.Length)
				Assert.Fail("expected.Length != actual.Length ({0}!={1})", expected.Length, actual.Length);
			for (int i = 0; i < actual.Length; i++)
			{
				if (expected[i] != actual[i])
					Assert.Fail("Arrays differed at ordinal {0} ({1}!={2})", i, expected[i], actual[i]);
			}
		}
		#endregion //helpers
	}
}
