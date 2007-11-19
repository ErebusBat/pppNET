using System;
using System.Collections.Generic;
using System.Text;
using Mono.Math;

namespace PerfectPaperPasswords.Math
{
	public static class PppConvert
	{
		public static byte[] HexStringToByteArray(string hexString)
		{
			List<byte> bytes = new List<byte>(hexString.Length / 2);
			for (int i = 0; i < hexString.Length; i+=2)
			{
				byte thisByte = byte.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
				bytes.Add(thisByte);
			}
			bytes.TrimExcess();
			return bytes.ToArray();
		}

		public static string ByteArrayToString(byte[] buffer)
		{
			StringBuilder hexString = new StringBuilder();
			for (int i = 0; i < buffer.Length; i++)
			{
				hexString.AppendFormat("{0:x2}", buffer[i]);
			}
			return hexString.ToString();
		}

		public static byte[] SwapBits(byte[] bits)
		{
			byte[] newBits = new byte[bits.Length];
			int adjust = bits.Length - 1;
			for (int i = 0; i < bits.Length; i++)
			{
				newBits[i] = bits[adjust - i];
			}
			return newBits;
		}

		#region Helper Methods
		//public static BigInteger Pow(BigInteger value, int exponent)
		//{
		//   if(exponent == 0)
		//      return BigInteger.Parse("1");

		//   BigInteger returnValue = new BigInteger(value);

		//   for (int i = 1; i < exponent; i++)
		//   {
		//      returnValue = returnValue * value;
		//   }

		//   return returnValue;
		//}

		//public static BigInteger Pow(BigInteger value, BigInteger exponent)
		//{
		//   if (exponent == 0)
		//      return BigInteger.Parse("1");

		//   BigInteger returnValue = new BigInteger(value);

		//   for (int i = 1; i < exponent; i++)
		//   {
		//      returnValue = returnValue * value;
		//   }

		//   return returnValue;
		//}
		#endregion //Helper Methods
	}
}
