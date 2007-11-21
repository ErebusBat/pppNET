using System;
using System.Collections.Generic;
using System.Text;
using Mono.Math;
using PerfectPaperPasswords.Math;
using System.IO;
using System.Security.Cryptography;

namespace PerfectPaperPasswords.Core
{
	public class PppV3Engine
	{
		#region *** ctors

		public PppV3Engine(byte[] sequenceKey, string alphabet)
		{
			SequenceKey = sequenceKey;
			_alphabet = new PppAlphabet(alphabet);
		}
		#endregion //*** ctors

		#region *** Properties
		private string _sequenceKeyHex;
		public string SequenceKeyHex
		{
			get
			{
				if (_sequenceKeyHex == null)
					_sequenceKeyHex = PppConvert.ByteArrayToString(_sequenceKey);
				return _sequenceKeyHex;
			}
		}

		private byte[] _sequenceKey;
		public byte[] SequenceKey
		{
			get
			{
				return _sequenceKey;
			}
			set
			{
				if (value.Length != 32)
					throw new ArgumentOutOfRangeException("value", "Sequence key must be 256 bits (32 bytes)!");
				_sequenceKey = value;
			}
		}

		private PppAlphabet _alphabet;

		private int _windowSize = -1;
		public int WindowSize
		{
			get
			{
				if (_windowSize == -1)
					_windowSize = _alphabet.Count;
				return _windowSize;
			}
		}

		//private BigInteger _counter;
		//public BigInteger Counter
		//{
		//   get
		//   {
		//      return _counter;
		//   }
		//   set
		//   {
		//      if (value == null)
		//         throw new ArgumentNullException("value");
		//      _counter = value;
		//   }
		//}
		#endregion //*** Properties

		#region *** Methods
		public char[] GetCounterCodeBytes(BigInteger counter)
		{
			byte[] crypto = GetCryptoBlockForCounter(_sequenceKey, counter);
			List<char> bytes = new List<char>();
			BigInteger working = new BigInteger(crypto);
			while (working >= WindowSize)
			{
				int index = working.ModDivInPlace(WindowSize);
				bytes.Add(_alphabet[index]);
			}
			return bytes.ToArray();
		}

		/// <summary>
		/// Because the PPPv3 spec says that individual passcodes can not
		/// cross crypto/counter boundaries we must calculate how many
		/// passcodes we can generate out of a given block of 128 bits
		/// </summary>
		/// <returns></returns>
		public int CalculatePasscodesPerBlock(int passcodeLength, int alphabetLength)
		{
			byte[] bits128 = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
			BigInteger allBits = new BigInteger(bits128);
			int charactersPerBlock = 0;
			while (allBits > alphabetLength)
			{
				allBits.ModDivInPlace(alphabetLength);
				charactersPerBlock++;
			}
			int codesPerBlock = charactersPerBlock / passcodeLength;
			return codesPerBlock;
		}
		#endregion //*** Methods

		#region *** Internal Methods
		protected byte[] GetCryptoBlockForCounter(byte[] sequenceKey, BigInteger counter)
		{
			if (sequenceKey == null)
				throw new ArgumentNullException("sequenceKey");
			if (counter == null)
				throw new ArgumentNullException("counter");

			byte[] counterBytes = pppHelper.GetCounterHexValue(counter);
			byte[] cryptoBlock;
			Rijndael crypto = Rijndael.Create();
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
			return PppConvert.SwapBits(cryptoBlock);
		}
		#endregion //*** Internal Methods

	}
}
