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
	public class ExtendedAlphabetTests
		: TestFixture
	{
		PppV3Engine ppp;
		//zombie = 49460b7bbbd3aad3f2cba09864f5e8b01a220ea8c077e9fa996de367e7984af0
		byte[] sequenceKey = pppHelper.GetSequenceKeyFromPassword("zombie");

		[SetUp]
		public void Setup()
		{
			ppp = PppV3Engine.BuildExtendedAlphabet(sequenceKey);
		}

		[Test]
		public void FirstThreeCards()
		{
			//int.maxValue = 2147483647
			BigInteger cardNumber = BigInteger.Parse("1");

			string[] knownGood;
			string[] actual;

			//Card 1
			knownGood = new[] { "[uDy", ">/*9", "e;?W", "p7PD", "o\"Au", "nVuK", "%K\"T", "<?bF", "|s:q", "J[Z,", "3's=", "UHPC", "aiLn", "]tq[", "RE},", "HZ56", "Nm{f", "^2F\\", "z*9h", "2ht&", "B(7a", "nA@$", "tYO'", "mo}!", "o!+k", "K4qn", "@!_Z", "U>2s", "Nqv&", "tRn:", "fK&j", "]A]a", "-<vM", "h,|A", "Bu:A", "cAr5", "AEnh", "#|3^", "=r_+", "v=<P", "pCt]", "nL3D", "(.L_", "fLK%", "|[Tb", "iYZ+", "^8Yv", "Ge^G", ",#,w", "Tv9z", "4{2e", "[RU+", "SJ\"O", "A}S2", "[@uS", "=sH\"", "4),~", ".@n!", "xs.A", "eWwx", "my7L", "s>!2", "[]AM", "hY-x", "rYBd", "qRS)", "-u^|", "6s[9", "~n?f", "@y&," };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//Card 2
			cardNumber += 1;
			knownGood = new[] { "$5@(", "@m(/", "nhdt", "Y[*w", "p~-v", "DrM/", "iYGA", ":-U$", "!dgh", "45S<", "O7:C", "zSht", "JCAU", "Ho77", "@wPm", "HN2+", "v{cO", "ZD#K", "o~cu", "&5X'", "{vp!", "=|oY", "XjaS", "Wn^7", "mS@+", "_L-<", "sk#f", "+|i$", "u>+y", "GVzq", "k'&A", "^5vO", "{iV9", "<37*", "2,M&", "7mLz", ")CY5", "3cJ=", "ty!n", "[\\}x", "_8$v", "M?cj", "(HYy", "abZk", "EpgR", "q7_3", "nU(w", "dyyz", "E/)z", "se2R", "42h%", "O(~P", "EUA{", "WLu#", "@Vei", "\\FFJ", "eexW", "*&&<", "z\\$q", "pTzm", "TLSj", "AF-A", "#/Lq", "$u,-", "iB/{", "$f\\@", "<^_p", "zLmT", "k)7k", "h\"6v"};
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//Card 3
			cardNumber += 1;
			knownGood = new[] { "(EON", "t.ix", "L>:?", "u|[&", "<e=T", "nTZE", "O)fY", "r%Y\"", "o<bB", "XE7]", "m?{,", "e=_)", "'#BT", "f/&>", ">J[i", "m|sk", "H#hm", ",u:F", ";NUP", "iSD}", "}Y6&", "bN'c", "pVMk", "7e^{", ":HP8", "-CMu", "ivN:", "-(Mr", "\"L?G", "kpru", "q.Kk", "NP7j", "x6?%", "vK!<", ">[5}", "@WCX", "E}qG", "az=W", "nLHa", "{-6G", ")Xjg", ">iW6", "K\\pU", "85E8", "9:D!", "+2-_", "UYR5", ";LvV", "v&[O", "&TwS", "mNPO", "q6%E", "m9gf", "Jc@{", "vb?R", "L.]*", "h<@2", "8-ef", "^Cdy", "qjGr", "o^Aa", "'[3r", "DRdv", "$\\?p", "=:(<", "RHBR", "G'o>", ")~Mx", "U:D,", "ero9" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));
		}
	}
}
