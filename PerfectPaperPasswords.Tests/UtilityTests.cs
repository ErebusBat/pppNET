using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace PerfectPaperPasswords.Tests
{
	[TestFixture]
	public class UtilityTests
		: TestFixture
	{
		[Test]
		public void BytesToHexString()
		{
			//We ToLower() because the case does not matter and we don't want the
			//test failing if the expected or function gets one changes as that will
			//NOT affect the result.

			byte[] zombie = { 0x49, 0x46, 0x0b, 0x7b, 0xbb, 0xd3, 0xaa, 0xd3, 0xf2, 0xcb, 0xa0, 0x98, 0x64, 0xf5, 0xe8, 0xb0, 0x1a, 0x22, 0x0e, 0xa8, 0xc0, 0x77, 0xe9, 0xfa, 0x99, 0x6d, 0xe3, 0x67, 0xe7, 0x98, 0x4a, 0xf0 };
			string expected = "49460b7bbbd3aad3f2cba09864f5e8b01a220ea8c077e9fa996de367e7984af0".ToLower();
			string actual = pppHelper.BytesToHexString(zombie).ToLower();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void HexStringToBytes()
		{
			string hexString = "49460b7bbbd3aad3f2cba09864f5e8b01a220ea8c077e9fa996de367e7984af0".ToLower();
			byte[] expected = { 0x49, 0x46, 0x0b, 0x7b, 0xbb, 0xd3, 0xaa, 0xd3, 0xf2, 0xcb, 0xa0, 0x98, 0x64, 0xf5, 0xe8, 0xb0, 0x1a, 0x22, 0x0e, 0xa8, 0xc0, 0x77, 0xe9, 0xfa, 0x99, 0x6d, 0xe3, 0x67, 0xe7, 0x98, 0x4a, 0xf0 };
			byte[] actual = pppHelper.HexStringToBytes(hexString);

			AssertEqual(expected, actual);
		}
	}
}
