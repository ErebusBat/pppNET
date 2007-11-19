using System;
using System.Collections.Generic;
using System.Text;
using Mono.Math;
using PerfectPaperPasswords.Math;
using System.IO;
using PerfectPaperPasswords.Core;

namespace PerfectPaperPasswords
{
	public class PppGenerator
	{
		PppEngine ppp = new PppEngine();

		#region *** ctors
		public PppGenerator(byte[] sequenceKey)
		{
			SequenceKey = sequenceKey;
		}

		public PppGenerator(string hexSequenceKey)
		{
			SequenceKey = PppConvert.HexStringToByteArray(hexSequenceKey);
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
					throw new ArgumentOutOfRangeException("value", "Sequence key must be exactly 32 bytes!");
				_sequenceKey = value;
			}
		}
		#endregion //*** Properties

		#region *** Calculation Methods
		
		#endregion //*** Calculation Methods

		#region *** Public API Methods
		public string GetPasscode(BigInteger passcodeIndex)
		{
			if(!ppp.HavePasscodes ||
				(ppp.FirstGeneratedPasscode<= passcodeIndex && ppp.LastGeneratedPasscode >= passcodeIndex));
			{
				ppp.GenerateCryptoMaterialPasscode(_sequenceKey, passcodeIndex);
			}
			byte[] codeBytes = ppp.GetPasscodeBytes(passcodeIndex);
			return ppp.ExpandBitsToPassCode(codeBytes);
		}
		#endregion //*** Public API Methods
				
		#region *** Statics
		public static string GenerateNewHexSequenceKey()
		{
			//Sequence Keys are 256 bits (32 bytes)
			System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
			byte[] key = new byte[32];
			rng.GetBytes(key);

			StringBuilder hexKey = new StringBuilder();
			for (int i = 0; i < key.Length; i++)
			{
				hexKey.Append(key[i].ToString("x2"));
			}

			return hexKey.ToString();
		}
		#endregion //*** Statics
	}
}