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
	public class RandSeqKeyTests
		: TestFixture
	{
		PppV3Engine ppp;
		//Random Key from GRC's server
		string hexSequenceKey = "D66789295E79B9E35A9A239C751599C64E818ADACB4E0005D00C99AC106DE15C";

		[SetUp]
		public void Setup()
		{
			byte[] sequenceKey = pppHelper.HexStringToBytes(hexSequenceKey);
			ppp = PppV3Engine.BuildStandard(sequenceKey);
		}

		[Test]
		public void FirstThreeCards()
		{
			//int.maxValue = 2147483647
			BigInteger cardNumber = BigInteger.Parse("1");

			string[] knownGood;
			string[] actual;

			//Card 1
			knownGood = new[] { "X2ST", "qtje", "q=nr", "iX5a", "WJTo", "XHbV", "fNjs", "Evnz", ":y#m", ":4sY", "s7hB", "WvgE", "So3a", "nhvN", "B4!7", "uPSJ", "my5V", "VmyT", "au@g", "gpN@", "%#9r", "nAX@", "9G7?", "+:aa", "ouWA", "A?x?", "xWZn", "JhA7", "JAeB", "Kz7j", "Fxwy", "8Zzw", "4YMW", "#nR3", ":nG3", "a:PB", "5Zpx", "hB@e", "jryA", ":8i+", "hys+", "xP:y", "z5m3", "kmAg", "TEmB", "zfzK", "UqLt", "yF+h", "F%3j", "WFYj", "8+ex", "3kS2", "oNL=", "M4jE", "77AM", "Tf%w", ":Hut", "D@CT", "cLDa", "?Gxw", "b2Cv", "HSPU", "aAuA", "A3xs", "@V3S", "npH!", "Zs8D", "K@BF", "8#U6", "m%?b" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//Card 2
			cardNumber += 1;
			knownGood = new[] { "czd@", "i3pB", "rqzY", "NC6R", "Gf29", "GMma", "=cRK", "q+hq", "Y9dZ", "r6e6", "=Rw?", "MSxe", "p=bn", "bYmz", "!DUo", "vHPC", "VHdU", "CK+d", "z=@K", "eGFD", "@8=a", "ySh#", "nJW8", "PMCm", "rk4k", "zzEM", "JnW?", "Nw8q", "kTTN", "hUhY", "@Xn7", "pH!u", "xfi!", "RsNV", "#@sa", "2JN6", "fiX=", "b68d", "wpRA", "@?k9", "#Ts?", "cDKG", "pu+=", "qYGP", "T%:@", "#ipE", "Da3g", "8#:5", "=jLn", "HDLv", "MTSH", "g9hW", "e8CP", "sK7!", "V%Yq", "=7@@", "JXJs", "5mK9", "2Ydn", "5F4x", "WHjz", "Mmx3", "wM!9", "Duy3", "Umnj", "iX9i", "m=5L", "e27E", "kk62", "wJaN" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//Card 3
			cardNumber += 1;
			knownGood = new[] { "UkXD", "!c96", "Kfd#", "jMog", "nzKV", "GJgv", "Zh!?", "9JSi", "sEmH", "Bwe9", "c#HN", "aru@", "eLDi", "Po6U", "?cCS", "Hmb5", "3wrd", "WB:G", "A3#p", "Y33U", "i++?", "GBM?", "ZqZj", "sLBU", "mi3J", "%?Nz", "9Jga", "+g3J", "nebx", "y2Vs", "W5iq", "h35?", "esGw", "cG2L", "K6V?", "MAR#", "wNmx", "W29h", "dN=d", "qkEB", "3Ds@", "7roz", "F!:t", "jY7w", "kahL", "Z9y2", "Ucnn", "VcHG", "pGvJ", "GFzD", "7n?A", "nAvx", "5d4v", "=Biq", "G4iV", "u+ed", "@v8x", "kJEZ", "Ahha", "Z?BM", "+mty", "EECu", "gB8o", "zxq:", "kZSS", "UCii", "=hUZ", "YSX:", "j9g+", "kp5h" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));
		}
	}
}
