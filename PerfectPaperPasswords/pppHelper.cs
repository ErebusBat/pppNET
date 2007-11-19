using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Mono.Math;
using PerfectPaperPasswords.Math;

namespace PerfectPaperPasswords
{
	public class pppHelper
	{
		public static byte[] GetSequenceKeyFromPassword(string password)
		{
			HashAlgorithm hash = HashAlgorithm.Create("SHA256");
			byte[] buffer = System.Text.Encoding.ASCII.GetBytes(password);
			byte[] hashedBuffer = hash.ComputeHash(buffer);
			return hashedBuffer;
		}

		public static byte[] GetCounterHexValue(BigInteger counter)
		{
			byte[] counterBytes = counter.GetBytes();
			counterBytes = PppConvert.SwapBits(counterBytes);
			int padNeeded = 16 - counterBytes.Length;
			List<byte> padedBytes;

			if (counterBytes.Length != 16)
			{
				padedBytes = new List<byte>(16);
				padedBytes.AddRange(counterBytes);
				padedBytes.AddRange(new byte[padNeeded]);
			}
			else
			{
				padedBytes = new List<byte>(counterBytes);
			}
			return padedBytes.ToArray();
		}
	}
}
