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
      private static readonly byte[] bits128 = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
		private static readonly BigInteger Int128Max;

		public static string DefaultAlphabet = "!#%+23456789:=?@ABCDEFGHJKLMNPRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
		public static string ExtendedAlphabet = "!\"#$%&'()*+,-./23456789:;<=>?@ABCDEFGHJKLMNOPRSTUVWXYZ[\\]^_abcdefghijkmnopqrstuvwxyz{|}~";

		#region *** ctors
		static PppV3Engine()
		{
			Int128Max = new BigInteger(bits128);
		}

		#region Static Factories
		//Standard
		public static PppV3Engine BuildStandard(byte[] sequenceKey)
		{
			return BuildStandardAlphabet(sequenceKey, 4);
		}

		public static PppV3Engine BuildStandardAlphabet(byte[] sequenceKey, int passcodeLength)
		{
			return new PppV3Engine(sequenceKey, DefaultAlphabet, passcodeLength);
		}

		//Extended
		public static PppV3Engine BuildExtendedAlphabet(byte[] sequenceKey)
		{
			return BuildExtendedAlphabet(sequenceKey, 4);
		}

		public static PppV3Engine BuildExtendedAlphabet(byte[] sequenceKey, int passcodeLength)
		{
			return new PppV3Engine(sequenceKey, ExtendedAlphabet, passcodeLength);
		}

		//Custom
		public static PppV3Engine BuildCustom(byte[] sequenceKey, string alphabet)
		{
			return BuildCustom(sequenceKey, alphabet, 4); ;
		}

		public static PppV3Engine BuildCustom(byte[] sequenceKey, string alphabet, int passcodeLength)
		{
			return new PppV3Engine(sequenceKey, alphabet, passcodeLength);
		}
		#endregion //Static Factories

		public PppV3Engine(byte[] sequenceKey, string alphabet)
			: this(sequenceKey, alphabet, 4)
		{
		}

		public PppV3Engine(byte[] sequenceKey, string alphabet, int passcodeLength)
		{
			SequenceKey = sequenceKey;
			_alphabet = new PppAlphabet(alphabet);
			PasscodeLength = passcodeLength;
			CodesPerCard = 70;
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

		private int _codesPerCard;
		public int CodesPerCard
		{
			get
			{
				return _codesPerCard;
			}
			set
         {
				if (value < 1)
					throw new ArgumentOutOfRangeException();
         	_codesPerCard = value;
         }
		}

		//private int _windowSize = -1;
		//public int WindowSize
		//{
		//   get
		//   {
		//      if (_windowSize == -1)
		//         _windowSize = _alphabet.Count;
		//      return _windowSize;
		//   }
		//}

		private int _passcodeLength;
		public int PasscodeLength
		{
			get
			{
				return _passcodeLength;
			}
			internal set
			{
				int maxLength = CalculateMaxPasscodeLength(_alphabet.Count);
				if (value > maxLength)
					throw new ArgumentOutOfRangeException(String.Format("The current alphabet is {0} characters, which can generate passcodes of only {1} max.", _alphabet.Count, maxLength));
				_passcodeLength = value;
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

		/// <summary>
		/// Calculates the passcode for the given index
		/// </summary>
		/// <param name="passcodeOrdinal"></param>
		/// <returns></returns>
		public string GetPasscode(BigInteger passcodeOrdinal)
		{
			if (passcodeOrdinal > Int128Max)
				throw new ArgumentOutOfRangeException("passcodeOrdinal", String.Format("Passcode Ordinals can not exceed 128 bits!", string.Empty));

			byte[] crypto = GetCryptoBlockForCounter(_sequenceKey, passcodeOrdinal);
			StringBuilder passcode = new StringBuilder(_passcodeLength);
			BigInteger working = new BigInteger(crypto);
			while (passcode.Length < _passcodeLength)
			{
				int index = working.ModDivInPlace(_alphabet.Count);
				passcode.Append(_alphabet[index]);
			}
			return passcode.ToString();
		}

		public string[] GetPasscodesForOrdinal(int startOrdinal, int count)
		{
			return GetPasscodesForOrdinal(BigInteger.Parse(startOrdinal.ToString()), count);
		}

		public string[] GetPasscodesForOrdinal(BigInteger startOrdinal, int count)
		{
			BigInteger currentIndex = new BigInteger(startOrdinal);
			List<string> codes = new List<string>(count);
			while (codes.Count < count)
			{
				string code = GetPasscode(currentIndex);
				codes.Add(code);
				currentIndex += 1;
			}
			return codes.ToArray();
		}

		public string[] GetPasscodesForCard(BigInteger cardNumber)
		{
			if (cardNumber < 1)
				throw new ArgumentOutOfRangeException("cardNumber");
			BigInteger startOrdinal = CalculateCardStartOffset(70, cardNumber);
			List<string> codes = new List<string>(_codesPerCard);

			for (int i = 0; i < _codesPerCard; i++)
			{
				string code = GetPasscode(startOrdinal + i);
				codes.Add(code);
			}

			return codes.ToArray();
		}

		public string[] GetPasscodesForCard(int cardNumber)
		{
			return GetPasscodesForCard(BigInteger.Parse(cardNumber.ToString()));
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


		/// <summary>
		/// Each passcode is generated from a seperate 128 bit crypto operation, meaning that
		/// passcodes can not exceed a certain length for a given alphabet.
		/// </summary>
		/// <param name="alphabetLength"></param>
		/// <returns></returns>
		public static int CalculateMaxPasscodeLength(int alphabetLength)
		{			
			BigInteger allBits = new BigInteger(bits128);
			int blocks = 0;
			while (allBits > alphabetLength)
			{
				allBits.ModDivInPlace(alphabetLength);
				blocks++;
			}
			return blocks;
		}

		/// <summary>
		/// Calculates the starting passcode offset based on a card number
		/// </summary>
		/// <param name="codesPerCard"></param>
		/// <param name="card"></param>
		/// <returns></returns>
		public BigInteger CalculateCardStartOffset(int codesPerCard, BigInteger card)
		{
			BigInteger offset = new BigInteger();
			offset = (card - 1) * codesPerCard;
			return offset;
		}

		public int CalculateCardStartOffset(int codesPerCard, int card)
		{
			BigInteger offset = CalculateCardStartOffset(codesPerCard, BigInteger.Parse(card.ToString()));
			if(offset > int.MaxValue)
				throw new OverflowException(string.Format("Use CalculateCardStartOffset with a BigInteger to retrive this result: {0}", offset));
			int offsetInt = int.Parse(offset.ToString());
			return offsetInt;
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

		//protected char[] GetCounterCodeBytes(BigInteger counter)
		//{
		//   byte[] crypto = GetCryptoBlockForCounter(_sequenceKey, counter);
		//   List<char> bytes = new List<char>();
		//   BigInteger working = new BigInteger(crypto);
		//   while (working >= WindowSize)
		//   {
		//      int index = working.ModDivInPlace(WindowSize);
		//      bytes.Add(_alphabet[index]);
		//   }
		//   return bytes.ToArray();
		//}
		#endregion //*** Internal Methods

	}
}
