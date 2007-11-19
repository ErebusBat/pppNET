using System;
using System.Collections.Generic;
using System.Text;
using Mono.Math;
using PerfectPaperPasswords.Math;
using PerfectPaperPasswords;

namespace ppp.tests
{
	class IntegrityTests
	{
		public void RunTests()
		{
			//testCounter();
			testPasswordHashGeneration();
			testBigIntegerExponent();
			testBinaryStringParsing();
			testHexStringParsing();
			testHexStringToArray();
			//testBitExpansion();
			testBinaryToBigIntToHex();
			testZombieSequenceRaw();
			testZombieSequence();
			testZombieCryptoSequence();
			testGrcVectorSequence();
		}

		#region Helper Functions
		private void Assert(BigInteger expected, BigInteger actual, string message)
		{
			if (expected != actual)
				throw new TestFailedException(String.Format("Expected {0}, got {1}: {2}", expected, actual, message));
		}
		private void Assert(string expected, string actual, string message)
		{
			if (expected != actual)
				throw new TestFailedException(String.Format("Expected {0}, got {1}: {2}", expected, actual, message));
		}
		private void Assert(byte[] expected, byte[] actual, string message)
		{
			if (expected.Length != actual.Length)
				throw new TestFailedException(String.Format("Expected.Length={0}, Actual.Length={1}: {2}", expected.Length, actual.Length, message));

			for (int i = 0; i < expected.Length; i++)
			{
				if (expected[i] != actual[i])
					throw new TestFailedException(String.Format("Arrays differ at position {0}: Expected={1}, Got={2}:  {3}", i, expected[i], actual[i], message));
			}
		}
		#endregion //Helper Functions

		#region Tests

		#region testBigIntegerExponent
		private void testBigIntegerExponent()
		{
			BigInteger value;
			BigInteger exponent;
			BigInteger expected;
			BigInteger actual;

			//
			//Test 1: 5^3=125
			value = BigInteger.Parse("5");
			exponent = BigInteger.Parse("3");
			expected = BigInteger.Parse("125");
			actual = PppConvert.Pow(value, exponent);
			Assert(expected, actual, "Exponent test 1: 5^3");

			//
			//Test 2: 10^116=100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
			//
			value = BigInteger.Parse("10");
			exponent = BigInteger.Parse("116");
			expected = BigInteger.Parse("100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
			actual = PppConvert.Pow(value, exponent);
			Assert(expected, actual, "Exponent test 2: 10^116");

			//
			//Test 3: 2^384=39402006196394479212279040100143613805079739270465446667948293404245721771497210611414266254884915640806627990306816
			//
			value = BigInteger.Parse("2");
			exponent = BigInteger.Parse("384");
			expected = BigInteger.Parse("39402006196394479212279040100143613805079739270465446667948293404245721771497210611414266254884915640806627990306816");
			actual = PppConvert.Pow(value, exponent);
			Assert(expected, actual, "Exponent test 3: 2^384");

			//
			//Test 4: 2^0=1
			//
			value = BigInteger.Parse("2");
			exponent = BigInteger.Parse("0");
			expected = BigInteger.Parse("1");
			actual = PppConvert.Pow(value, exponent);
			Assert(expected, actual, "Exponent test 4: 2^0=1");
		}
		#endregion //testBigIntegerExponent

		#region Binary String Parsing
		private void testBinaryStringParsing()
		{
			string binaryString;
			BigInteger expected;
			BigInteger actual;

			//
			//Test 1 - 5 bits: 87=1010111
			//
			binaryString = "1010111";
			expected = BigInteger.Parse("87");
			actual = PppConvert.BinaryToBigInteger(binaryString);
			Assert(expected, actual, "BinaryStringParsing, 5 bits: 87=1010111");

			//
			//Test 2 - 9 bits: 357=101100101
			//
			binaryString = "101100101";
			expected = BigInteger.Parse("357");
			actual = PppConvert.BinaryToBigInteger(binaryString);
			Assert(expected, actual, "BinaryStringParsing, 9 bits: 357=101100101");

			//
			//Test 3 - 31 bits: 2147483647=1111111111111111111111111111111
			//
			binaryString = "1111111111111111111111111111111";
			expected = BigInteger.Parse("2147483647");
			actual = PppConvert.BinaryToBigInteger(binaryString);
			Assert(expected, actual, "BinaryStringParsing, 31 bits: 2147483647=1111111111111111111111111111111");

			//
			//Test 4 - 32 bits: 4294967295=11111111111111111111111111111111
			//
			binaryString = "11111111111111111111111111111111";
			expected = BigInteger.Parse("4294967295");
			actual = PppConvert.BinaryToBigInteger(binaryString);
			Assert(expected, actual, "BinaryStringParsing, 31 bits: 4294967295=11111111111111111111111111111111");
		}
		#endregion //Binary String Parsing

		#region Hex String Parsing
		private void testHexStringParsing()
		{
			string hexString;
			BigInteger expected;
			BigInteger actual;

			//
			//Test 1: 4f=79
			//
			hexString = "4f";
			expected = BigInteger.Parse("79");
			actual = PppConvert.HexToBigInteger(hexString);
			Assert(expected, actual, "HexStringParsing 4f=79");

			//
			//Test 2: ffffffff=4294967295
			//
			hexString = "ffffffff";
			expected = BigInteger.Parse("4294967295");
			actual = PppConvert.HexToBigInteger(hexString);
			Assert(expected, actual, "HexStringParsing ffffffff=4294967295");

			//
			//Test 3: fb2717b3=4213643187
			//
			hexString = "fb2717b3";
			expected = BigInteger.Parse("4213643187");
			actual = PppConvert.HexToBigInteger(hexString);
			Assert(expected, actual, "HexStringParsing fb2717b3=4213643187");

			//
			//Test 4: a15eb4783cf5e5a49009c77fc03cca7e057cbc100655007831acd8d9b5360ec6abe264ab967febd22b30cee90c59733b=24837107128891896545571612031389328828616313616798377523698900413983082671338651505377777096221802570786081204695867
			//
			hexString = "a15eb4783cf5e5a49009c77fc03cca7e057cbc100655007831acd8d9b5360ec6abe264ab967febd22b30cee90c59733b";
			expected = BigInteger.Parse("24837107128891896545571612031389328828616313616798377523698900413983082671338651505377777096221802570786081204695867");
			actual = PppConvert.HexToBigInteger(hexString);
			Assert(expected, actual, "HexStringParsing Actual Sequence key");
		}
		#endregion //Hex String Parsing

		#region BitExpansion
		void testBitExpansion()
		{
			/* 
						1         2
			123456789012345678901234
			101011111010001101101010 = afa36a 
			 * af=175 a3=163 6a=106

			  af=175  a3=163   6a=106
			|----- -|---- ---|-- -----|
			          1           2
			123456 789012 345678 901234			
			101011 111010 001101 101010
			43(2b) 58(3a) 13(0d) 42(2a)   = CU=B
			 */

			byte[] bytes;
			string expected;
			string actual;


			bytes = PppConvert.HexStringToByteArray("afa36a");
			actual = PppGenerator._ExpandBitsToPassCode(bytes);
			expected = "CU=B";
			Assert(expected, actual, "Bit Expansion: afa36a");
		}
		#endregion //BitExpansion

		private void testBinaryToBigIntToHex()
		{
			string bitString = "101011111010001101101010";
			string hexString = "afa36a";
			string actual;
			BigInteger interim;

			//
			interim = PppConvert.HexToBigInteger(hexString);
			actual = PppConvert.BigIntegerToBinary(interim);
			Assert(bitString, actual, "Binary->BigInt->Hex");
		}

		private void testHexStringToArray()
		{
			string hexString;
			byte[] expected;
			byte[] actual;

			hexString = "afa36a";
			expected = new byte[] { 0xaf, 0xa3, 0x6a };
			actual = PppConvert.HexStringToByteArray(hexString);
			Assert(expected, actual, "Hex to Bytes: afa36a");
		}

		private void testPasswordHashGeneration()
		{
			string password;
			string actual;
			string expected;
			byte[] buffer;

			password = "zombie";
			buffer = pppHelper.GetSequenceKeyFromPassword(password);
			actual = PppConvert.ByteArrayToString(buffer);
			expected = "49460b7bbbd3aad3f2cba09864f5e8b01a220ea8c077e9fa996de367e7984af0";
			Assert(expected, actual, "Password Generation");
		}

		private void testCounter()
		{
			BigInteger counter;
			string actual;
			string expected;

			counter = BigInteger.Parse("0");
			expected = "00000000000000000000000000000000";
			actual = pppHelper.GetCounterHexValueString(counter);
			Assert(expected, actual, string.Format("Coutner Value: {0}", counter));

			counter = BigInteger.Parse("1");
			expected = "00000000000000000000000000000001";
			actual = pppHelper.GetCounterHexValueString(counter);
			Assert(expected, actual, string.Format("Coutner Value: {0}", counter));

			counter = BigInteger.Parse("10");
			expected = "0000000000000000000000000000000a";
			actual = pppHelper.GetCounterHexValueString(counter);
			Assert(expected, actual, string.Format("Coutner Value: {0}", counter));

			counter = BigInteger.Parse("98127487621438091");
			expected = "0000000000000000015c9e6e1a609a8b";
			actual = pppHelper.GetCounterHexValueString(counter);
			Assert(expected, actual, string.Format("Coutner Value: {0}", counter));
		}

		private void testZombieCryptoSequence()
		{
			BigInteger counter;
			PppGenerator gen = new PppGenerator(PppConvert.ByteArrayToString(pppHelper.GetSequenceKeyFromPassword("zombie")));
			byte[] expected;
			byte[] actual;

			counter = BigInteger.Parse("0");
			expected = new byte[] { 0x46, 0xDD, 0x04, 0xC5, 0x28, 0xB5, 0x50, 0x6A, 0x05, 0xE8, 0x4E, 0x2A, 0x27, 0xE0, 0x6E, 0xDC };
			actual = gen.GetCryptoBlockForCounter(counter);
			Assert(expected, actual, string.Format("Zombie {0} Crypto", counter));

			counter = BigInteger.Parse("1");
			expected = new byte[] { 0x2B, 0x19, 0x3B, 0x34, 0x35, 0xCA, 0x3E, 0x78, 0xFE, 0x42, 0x5A, 0x71, 0x3B, 0x57, 0x6A, 0x66 };
			actual = gen.GetCryptoBlockForCounter(counter);
			Assert(expected, actual, string.Format("Zombie {0} Crypto", counter));
		}

		private void testZombieSequenceRaw()
		{
			BigInteger counter;
			PppGenerator gen;
			byte[] crypto;
			byte[] codeBytes = new byte[3];
			string sequenceKey = PppConvert.ByteArrayToString(pppHelper.GetSequenceKeyFromPassword("zombie"));
			string actual;
			string expected;

			gen = new PppGenerator(sequenceKey);

			counter = BigInteger.Parse("0");
			crypto = gen.GetCryptoBlockForCounter(counter);
			string[] knownKeys = { "8N=3", "7ucE", "aAg3", "zVv#", "y2Fm" };

			for (int i = 0; i < 5; i++)
			{
				Array.Copy(crypto, i * 3, codeBytes, 0, 3);
				actual = PppGenerator._ExpandBitsToPassCode(codeBytes);
				expected = knownKeys[i];
				Assert(expected, actual, string.Format("Zombie RAW sequence, Counter={0}, {1} Passcode", counter, i + 1));
			}
		}

		private void testZombieSequence()
		{
			BigInteger counter;
			PppGenerator gen;
			byte[] crypto;
			byte[] codeBytes = new byte[3];
			string sequenceKey = PppConvert.ByteArrayToString(pppHelper.GetSequenceKeyFromPassword("zombie"));
			string actual;
			string expected;

			gen = new PppGenerator(sequenceKey);

			counter = BigInteger.Parse("0");
			crypto = gen.GetCryptoBlockForCounter(counter);
			string[] knownKeys = { "8N=3", "7ucE", "aAg3", "zVv#", "y2Fm" };

			for (int i = 0; i < 5; i++)
			{
				Array.Copy(crypto, i * 3, codeBytes, 0, 3);
				actual = PppGenerator._ExpandBitsToPassCode(codeBytes);
				expected = knownKeys[i];
				Assert(expected, actual, string.Format("Zombie sequence, Counter={0}, {1} Passcode", counter, i + 1));
			}
		}

		private void testGrcVectorSequence()
		{
			BigInteger counter;
			PppGenerator gen;
			byte[] crypto;
			byte[] codeBytes = new byte[3];
			string sequenceKey = PppConvert.ByteArrayToString(pppHelper.GetSequenceKeyFromPassword("zombie"));
			string actual;
			string expected;

			gen = new PppGenerator(sequenceKey);

			counter = BigInteger.Parse("34784419729695860707371955026017184241");
			crypto = gen.GetCryptoBlockForCounter(counter);

		}
		#endregion //Tests
	}
}
