using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

using Mono.Math;

using PerfectPaperPasswords.Math;

namespace PerfectPaperPasswords.Core
{
	/// <summary>
	/// Class used by PppGenerator to calculate byte offsets, etc.
	/// </summary>
	public class PppEngine
	{
		#region *** Properties
		private byte[] _passcodeMaterial = new byte[128];
		//public byte[] PasscodeMaterial
		//{
		//   get
		//   {
		//      return _passcodeMaterial;
		//   }
		//}

		private BigInteger _firstGeneratedPasscode = null;
		/// <summary>
		/// The first passcode that can be generated using the material in PasscodeMaterial. NULL if no passcode can be generated
		/// </summary>
		public BigInteger FirstGeneratedPasscode
		{
			get
			{
				return _firstGeneratedPasscode;
			}
		}

		private BigInteger _lastGeneratedPasscode = null;
		/// <summary>
		/// The last passcode that can be generated using the material in PasscodeMaterial. NULL if no passcode can be generated
		/// </summary>
		public BigInteger LastGeneratedPasscode
		{
			get
			{
				return _lastGeneratedPasscode;
			}
		}

		/// <summary>
		/// TRUE if there is currently material in CryptoBytes that can be used to generate passcodes
		/// </summary>
		public bool HavePasscodes
		{
			get
			{
				return (_firstGeneratedPasscode != null && _lastGeneratedPasscode != null);
			}
		}
		#endregion //*** Properties

		#region *** Methods
      
		#region ExpandBits
		static char[] ppp_alphabet = "23456789!@#%+=:?abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPRSTUVWXYZ".ToCharArray();
		public string ExpandBitsToPassCode(byte[] material)
		{
			//List<char> alphabet = new List<char>("23456789!@#%+=:?abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPRSTUVWXYZ");
			System.Diagnostics.Debug.Assert(ppp_alphabet.Length == 64);
			if (material.Length != 3)
				throw new ArgumentOutOfRangeException();
			material = PppConvert.SwapBits(material);
			StringBuilder passcode = new StringBuilder();
			int tempReg = 0;
			int data = 0;
			byte maskLast6 = (byte)63;			//00111111
			byte maskFirst2 = (byte)192;		//11000000
			byte maskLast4 = (byte)15;			//00001111
			byte maskFirst4 = (byte)240;		//11110000
			byte maskLast2 = (byte)3;			//00000011
			byte maskFirst6 = (byte)252;		//11111100


			//Get the first passcode
			data = material[0] & maskFirst6;
			data = data >> 2;
			passcode.Insert(0, ppp_alphabet[data]);

			//Second passcode
			tempReg = material[0] & maskLast2;
			data = tempReg << 4;
			tempReg = material[1] & maskFirst4;
			tempReg = tempReg >> 4;
			data += tempReg;
			passcode.Insert(0, ppp_alphabet[data]);

			//Third passcode
			tempReg = material[1] & maskLast4;
			data = tempReg << 2;
			tempReg = material[2] & maskFirst2;
			tempReg = tempReg >> 6;
			data += tempReg;
			passcode.Insert(0, ppp_alphabet[data]);

			//Fourth passcode
			data = material[2] & maskLast6;
			passcode.Insert(0, ppp_alphabet[data]);

			return passcode.ToString();
		}
		#endregion //ExpandBits

		#region GetPasscodeBytes
		/// <summary>
		/// Returns the three byte (24bit) sequence that makes up a specific passcode
		/// </summary>
		/// <param name="passcodeIndex"></param>
		/// <returns></returns>
		public byte[] GetPasscodeBytes(BigInteger passcodeIndex)
		{
			//GenerateCryptoMaterialPasscode(sequenceKeyHex, passcodeIndex);
			byte[] passcode = new byte[3];
			int cryptoOffset = GetPasscodeOffsetInCryptoBlocks(passcodeIndex);

			Array.Copy(_passcodeMaterial, cryptoOffset, passcode, 0, 3);

			return passcode;
		}
		#endregion //GetPasscodeBytes

		#region GetCryptoBlocksForPasscode
		/// <summary>
		/// Generates the three crypto cycles (128 bytes) that contain the specified passcode
		/// </summary>
		/// <param name="passcodeIndex"></param>
		/// <returns></returns>
		public void GenerateCryptoMaterialPasscode(byte[] sequenceKey, BigInteger passcodeIndex)
		{
			//See if it is in our cache so we don't regen needlessly
			//if (HavePasscodes 
			//   && passcodeIndex >= _firstGeneratedPasscode && passcodeIndex <= _lastGeneratedPasscode)
			//   return _passcodeMaterial;

			//Wasn't in the cache so generate new ones
			BigInteger counter = GetFirstCounterForPasscode(passcodeIndex);
			using (MemoryStream buffer = new MemoryStream(_passcodeMaterial))
			{
				buffer.Write(GetCryptoBlockForCounter(sequenceKey, counter), 0, 16);
				buffer.Write(GetCryptoBlockForCounter(sequenceKey, counter + 1), 0, 16);
				buffer.Write(GetCryptoBlockForCounter(sequenceKey, counter + 2), 0, 16);

				_firstGeneratedPasscode = GetFirstPasscodeForCounter(counter);
				_lastGeneratedPasscode = GetLastPasscodeForCounter(counter);
			}

			//return _passcodeMaterial;
		}
		#endregion //GetCryptoBlocksForPasscode

		#region GetCryptoBlockForCounter
		/// <summary>
		/// Generates the cryptographic output for the specified counter value
		/// </summary>
		/// <param name="counter"></param>
		/// <returns></returns>
		public byte[] GetCryptoBlockForCounter(byte[] sequenceKey, BigInteger counter)
		{
			if (sequenceKey == null)
				throw new ArgumentNullException("sequenceKey");
			if (counter == null)
				throw new ArgumentNullException("counter");

			byte[] counterBytes = pppHelper.GetCounterHexValue(counter);
			byte[] cryptoBlock;
			Rijndael crypto = Rijndael.Create();
			//crypto.Key = PppConvert.HexStringToByteArray(sequenceKeyHex);
			crypto.Key = sequenceKey;
			crypto.Padding = PaddingMode.Zeros;
			crypto.Mode = CipherMode.ECB;
			using (ICryptoTransform trans = crypto.CreateEncryptor())
			using (MemoryStream memStream = new MemoryStream())
			using (CryptoStream cryptStream = new CryptoStream(memStream, trans, CryptoStreamMode.Write))
			{
				cryptStream.Write(counterBytes, 0, counterBytes.Length);
				cryptStream.FlushFinalBlock();
				cryptoBlock = memStream.ToArray();
			}
			return cryptoBlock;
		}
		#endregion //GetCryptoBlockForCounter

		#region GetPasscodeOffsetInCryptoBlocks
		/// <summary>
		/// Returns the passcode offset (0-15) in the three crypto blocks that would contain
		/// the passcode
		/// </summary>
		/// <param name="passcodeIndex"></param>
		/// <returns></returns>
		public int GetPasscodeOffsetInCryptoBlocks(BigInteger passcodeIndex)
		{
			//Because we are dealing with integers we don't need to specificly call floor
			//the decimal is dropped automagicly which is good in this instance :)
			BigInteger floor = passcodeIndex / 16;
			//BigInteger floor = passcodeIndex / 16 * 3;

			//We lost our decimal part on the divide so this is NOT redunant
			floor = floor * 16;

			//This should put the value in the 0-15 range, thus fitting into int
			floor = passcodeIndex - floor;

			//Now convert it to the byte offset
			floor *= 3;

			System.Diagnostics.Debug.Assert(floor < int.MaxValue);

			return int.Parse(floor.ToString());
		}		
		#endregion //GetPasscodeOffsetInCryptoBlocks

		#region *** Counter Calculations
		#region GetFirstCounterForPasscode
		/// <summary>
		/// Returns the counter value for the first crypto block (of the three) that will contain
		/// the specified passcode
		/// </summary>
		/// <param name="passcodeIndex"></param>
		/// <returns></returns>
		public BigInteger GetFirstCounterForPasscode(BigInteger passcodeIndex)
		{
			//Because we are dealing with integers we don't need to specificly call floor
			//the decimal is dropped automagicly which is good in this instance :)
			BigInteger floor = passcodeIndex / 16;
			floor = floor * 3;
			return floor;
		}
		#endregion //GetFirstCounterForPasscode

		#region GetFirstPasscodeForCounter
		public BigInteger GetFirstPasscodeForCounter(BigInteger counter)
		{
			BigInteger val = (counter / 3) * 16;
			return val;
		}
		#endregion //GetFirstPasscodeForCounter

		#region GetLastPasscodeForCounter
		public BigInteger GetLastPasscodeForCounter(BigInteger counter)
		{
			return GetFirstPasscodeForCounter(counter) + 15;
		}
		#endregion //GetLastPasscodeForCounter		
		#endregion //*** Counter Calculations


		#endregion //*** Methods
	}
}
