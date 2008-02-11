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
	public class PppEngineV3Tests
		: TestFixture
	{
		PppV3Engine ppp;
		//zombie = 49460b7bbbd3aad3f2cba09864f5e8b01a220ea8c077e9fa996de367e7984af0
		byte[] sequenceKey = pppHelper.GetSequenceKeyFromPassword("zombie");
		string alphabet = "!#%+23456789:=?@ABCDEFGHJKLMNPRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

		#region helper
		private void testCodeBlock(char[] actualCodeBlock, string[] knownGood)
		{
			int index = 0;
			int codeLength = knownGood[0].Length;
			for (int i = 0; i < knownGood.Length; i++)
			{
				StringBuilder code = new StringBuilder();
				for (int offset = 0; offset < codeLength; offset++)
				{
					code.Append(actualCodeBlock[index + offset]);
				}
				Assert.AreEqual(knownGood[i], code.ToString(), "Code mismatch @ {0:0,0}", i);
				index += codeLength;
			}
		}
		#endregion //helper

		[SetUp]
		public void Setup()
		{
			Assert.AreEqual(64, alphabet.Length, "Alphabet Length Mismatch!");
			ppp = new PppV3Engine(sequenceKey, alphabet);
		}

		#region old

		//[Test]
		//public void CalculatePasscodesPerBlock_V2Params()
		//{
		//   //Tests based on the PPPv2 parameters of 4 and 64
		//   int actual = ppp.CalculatePasscodesPerBlock(4, 64);
		//   Assert.AreEqual(5, actual);
		//}

		//[Test]
		//public void CalculatePasscodesPerBlock_OtherParams()
		//{
		//   int actual = ppp.CalculatePasscodesPerBlock(3, 64);
		//   Assert.AreEqual(7, actual);
		//}

		//[Test]
		//public void Newgroup5518_First5Test()
		//{
		//   BigInteger counter = BigInteger.Parse("0");
		//   string[] knownGood = { "4p=#", "3WCg", "AcG#", "bvX8", "a!hM" };
		//   char[] rawCodeData = ppp.GetCounterCodeBytes(counter);
		//   testCodeBlock(rawCodeData, knownGood);
		//}

		//[Test]
		//public void Newsgroup5518_Second5Test()
		//{
		//   BigInteger counter = BigInteger.Parse("1");
		//   string[] knownGood = { "eXk?", "oEWm", "yTaz", "%cFN", "vNYL" };
		//   char[] rawCodeData = ppp.GetCounterCodeBytes(counter);
		//   testCodeBlock(rawCodeData, knownGood);
		//}

		//[Test]
		//public void Newsgroup5518_BlockSpanningFirst10()
		//{
		//   BigInteger counter = BigInteger.Parse("0");
		//   string[] knownGood = { "4p=#", "3WCg", "AcG#", "bvX8", "a!hM", "eXk?", "oEWm", "yTaz", "%cFN", "vNYL" };
		//   char[] rawCodeData = ppp.GetCounterCodeBytes(counter);
		//   testCodeBlock(rawCodeData, knownGood);
		//}
		#endregion //old

		#region Position Calc
		[Test]
		public void CardOffsetCalculations()
		{
			int codesPerCard = 70;
			int cardNumber;

			cardNumber = 1;
			Assert.AreEqual(0, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));

			cardNumber = 2;
			Assert.AreEqual(70, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));

			cardNumber = 3;
			Assert.AreEqual(140, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));

			cardNumber = 4;
			Assert.AreEqual(210, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));

			cardNumber = 5;
			Assert.AreEqual(280, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));

			cardNumber = 6;
			Assert.AreEqual(350, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));

			cardNumber = 7;
			Assert.AreEqual(420, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));

			cardNumber = 1400;
			Assert.AreEqual(97930, ppp.CalculateCardStartOffset(codesPerCard, cardNumber), String.Format("Offset position mismatch on card #{0:#,0}", cardNumber));
		}
		#endregion //Position Calc

		#region New


		[Test]
		public void Passcode_Std_000_069()
		{
			//Zombie Password, PPP Standard alphabet, first 70 codes
			string[] knownGood = { "4p=#", "eXk?", "z=ao", "c!Zb", "T!Lb", "S8Ex", "fGF5", "t8u+", "ps4p", "qSxM", "TX#Y", "bz!b", "+gR=", "jo2r", "PPEh", "PD#x", "uEox", "U6ZS", "eW42", "zrci", "54yL", "HaiP", "gejs", "48Vd", "6nzo", "ztKn", "PVJ:", "sNnU", "Lp#F", "3qWs", "bnR5", "6dAf", "XBmH", "8=9#", "iLEL", "==nE", "?hHA", "uNd@", "V#G5", "zFz4", "7Cxc", "5TU+", "zceN", "!5Yc", "YVvL", "W3Sa", "kKnw", ":bAT", "vpJ5", "HMS6", "km%G", "hRz6", "ypKb", "hN39", "hG+Z", "uxGJ", "BZWq", "3rEH", "kRfv", "ig?U", "qYif", "E?G5", "qDrL", "V:aw", "ezjM", "%x@n", "NEzg", "nyj7", "zR@9", "=Gf6" };
			string[] actual = ppp.GetPasscodesForOrdinal(0, 70);
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");

			AssertEqual(knownGood, actual);
		}

		[Test]
		public void Passcode_Std_070_139()
		{
			//Zombie Password, PPP Standard alphabet, first 70 codes
			string[] knownGood = { "npkH", "gzyo", "rU8K", "2:g9", "k:?x", "cJA5", "WNUN", "r6D6", "Ayj7", "#i3R", "9vvV", "WdU7", "ZLGt", "Yr7i", "P8ps", "F#78", "iBFJ", "=6Vg", "jP7%", "p+b:", "whWF", "CPyU", "DBM%", "Lnha", "GYLx", "uvzy", "2CWk", "%b2S", "GAL?", "wh?S", "Fqc2", "cs7@", "NG9V", "KVxs", "Hbfj", "XAU6", "6jta", "6ufa", "xKUN", "R83L", "mjTw", "7Nhj", "zqHd", "MJJW", "uFLa", "d473", "5C%E", "hv99", "mWdP", "oZb4", "tZww", "D7rt", "VAc+", "m3YD", "pCT:", "z9LM", "aA+S", "#wE9", "varJ", "k:hB", "iFXm", "Zf!W", "CefM", "Dyyp", "ec+w", "+@SA", "kA2P", "DqHJ", "3si%", "LY+i" };
			string[] actual = ppp.GetPasscodesForOrdinal(70, 70);
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");

			AssertEqual(knownGood, actual);
		}

		[Test]
		public void Passcode_Std_140_209()
		{
			//Zombie Password, PPP Standard alphabet, first 70 codes
			string[] knownGood = { "rkET", "3Gch", "jX%5", "4owk", "#7CM", "iyoq", "eedh", "emde", "Ta?e", "WWNZ", "ZWku", "i?z=", "qB!k", "Atg+", "e%b5", "yKG:", "F3pM", "MCKB", "68Mw", "Dcm+", "qv:J", "f6G=", "UPj3", "waMv", "@ran", ":Pkg", "eiVE", "omXN", "#D2d", "PbNy", "%uGH", "8BUH", "KrK:", "r?EU", "eNM3", "=gHU", "mA3@", "+guS", "5xZy", "ofvm", "A%2=", "W4f=", "@=gy", "3W2d", "ysW%", "mjF8", "Ax?o", "JZTK", "5GBT", "gTq?", "q?g?", "Ci6%", "GJT#", "Rcgg", "i5H7", "TV62", "L!A@", "pHZe", "tdJ4", "dd??", "T3Af", "GVYH", "KozW", "eSG=", "%5oX", "YCCj", "oJyE", "jj?o", "J#mZ", "5!GY" };
			string[] actual = ppp.GetPasscodesForOrdinal(140, 70);
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");

			AssertEqual(knownGood, actual);
		}

		[Test]
		public void Passcode_Std_97930_97999()
		{
			//Zombie Password, PPP Standard alphabet, first 70 codes
			string[] knownGood = { "Vv?u", "zC7u", "cX+N", "Vvtf", "JthW", "?c9v", "@WVL", "jcpw", "icjD", "=wCV", "HZGB", "cV2Z", "V=5G", "Mwau", "2Xm@", "t4g%", "g@5V", "GuDF", "oAVo", "ewPR", "NgtD", "Dbj#", "C6Xm", "i2gT", "3pDV", "8BZ%", "ATJJ", "nKZ8", "?9b%", "7B3k", "fJ6C", "KYyJ", ":s@m", "z2c6", "T577", "Jdnj", "6K4K", "y4u7", "yort", "Wzvw", "9W7j", "LxwC", "8=SF", "%AnU", "%WgA", "uFpm", "Ao7t", "E3?%", "VNyF", "Dz!W", "#D!m", "jYL8", "8uvr", "maR?", "De=K", "ENd2", "iHv3", "%Aae", "fpCM", "6j2d", "ovo#", "cmsP", "9e?M", "zpSm", "@N!m", "@rT7", "4apF", "dMaG", "?f=D", "DRh#" };
			string[] actual = ppp.GetPasscodesForOrdinal(97930, 70);
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");

			AssertEqual(knownGood, actual);
		}
		#endregion //New

		#region Card Based
		[Test]
		public void Card_1()
		{
			string[] knownGood = { "4p=#", "eXk?", "z=ao", "c!Zb", "T!Lb", "S8Ex", "fGF5", "t8u+", "ps4p", "qSxM", "TX#Y", "bz!b", "+gR=", "jo2r", "PPEh", "PD#x", "uEox", "U6ZS", "eW42", "zrci", "54yL", "HaiP", "gejs", "48Vd", "6nzo", "ztKn", "PVJ:", "sNnU", "Lp#F", "3qWs", "bnR5", "6dAf", "XBmH", "8=9#", "iLEL", "==nE", "?hHA", "uNd@", "V#G5", "zFz4", "7Cxc", "5TU+", "zceN", "!5Yc", "YVvL", "W3Sa", "kKnw", ":bAT", "vpJ5", "HMS6", "km%G", "hRz6", "ypKb", "hN39", "hG+Z", "uxGJ", "BZWq", "3rEH", "kRfv", "ig?U", "qYif", "E?G5", "qDrL", "V:aw", "ezjM", "%x@n", "NEzg", "nyj7", "zR@9", "=Gf6" };
			string[] actual = ppp.GetPasscodesForCard(1);
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");

			AssertEqual(knownGood, actual);
		}

		[Test]
		public void Card_1400()
		{
			string[] knownGood = { "Vv?u", "zC7u", "cX+N", "Vvtf", "JthW", "?c9v", "@WVL", "jcpw", "icjD", "=wCV", "HZGB", "cV2Z", "V=5G", "Mwau", "2Xm@", "t4g%", "g@5V", "GuDF", "oAVo", "ewPR", "NgtD", "Dbj#", "C6Xm", "i2gT", "3pDV", "8BZ%", "ATJJ", "nKZ8", "?9b%", "7B3k", "fJ6C", "KYyJ", ":s@m", "z2c6", "T577", "Jdnj", "6K4K", "y4u7", "yort", "Wzvw", "9W7j", "LxwC", "8=SF", "%AnU", "%WgA", "uFpm", "Ao7t", "E3?%", "VNyF", "Dz!W", "#D!m", "jYL8", "8uvr", "maR?", "De=K", "ENd2", "iHv3", "%Aae", "fpCM", "6j2d", "ovo#", "cmsP", "9e?M", "zpSm", "@N!m", "@rT7", "4apF", "dMaG", "?f=D", "DRh#" };
			string[] actual = ppp.GetPasscodesForCard(1400);
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");

			AssertEqual(knownGood, actual);
		}

		[Test]
		public void Card_CrossInt32Boundry()
		{
			//int.maxValue = 2147483647
			BigInteger cardNumber = BigInteger.Parse(int.MaxValue.ToString());

			string[] knownGood;
			string[] actual;

			//2147483647
			knownGood = new[] { "C%Bd", "%N:%", "=c+7", "KCs8", "g#5S", "fFxN", "pquN", "cvyA", "G3ZM", "ttsi", "3vj3", "UTqo", "HvH5", "7ETZ", "vHe?", "hBuv", "rZwS", "!g9U", "q=ZN", "qRRU", "eXZp", "JvGs", "zD77", "Bt:7", "VeHz", "qmcZ", "LGDw", "jikH", "vNDu", "fGx4", "k%VK", "V?@7", "P+od", "saPn", "yqbe", "VUix", "p5uH", "NM#c", "CJ?U", "KDwy", ":!%4", "Tewt", "yC+s", "=A96", "gm?s", "zPT?", "uzgZ", "HEjV", "dChp", "4qqP", "W66S", "XnG5", "J4Zg", "uTeh", "kc5s", "HbSK", "HAnC", "Kx?u", "qFUA", "xb%j", "eG@X", "fk=!", "CdS!", "@4iU", "ThiJ", "%uoi", "=M4@", "J?5e", "Ja4e", "hXap" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//+1
			cardNumber += 1;
			knownGood = new[] { "+Ccr", "%Wts", "8DRs", "?W6S", "yis3", "R?gi", "kN%x", "nv+6", "DjMG", "P9cr", "y=gP", ":uVF", "Bfs9", "svNp", "LHTm", "9gLh", "uFwo", "nx8f", "buiy", "x7X5", "DWT%", "hM8#", "umyo", "YfdX", "U!wH", "oyU4", "Kw%P", "Ey7h", "3i7J", "F@P:", "j2tA", "Ehoy", "EZvc", "ER6k", "XHj=", "2Y#d", "sbr9", "rvi3", "!e?i", "Jn?V", "=jvW", "Expk", "9GSn", "Auy:", "cCqC", "NqEG", "UeP!", "t4x!", "bqTD", "=@42", "Hoos", "o8J@", "9W4H", "HET8", "LJLU", "vfh#", "jTDZ", "kFAB", "+ngd", "ucaq", "LkCp", "7m@4", ":y=t", "6AbB", "abrA", "4RD8", "%!sw", "R:fX", "pn!W", "AsFc" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//+2
			cardNumber += 1;
			knownGood = new[] { "TNWe", "8e@=", "NzqJ", "sV8g", "UdVK", "rmWd", "mBNs", "s:WX", "zmMg", "nkTd", "Ds!Z", "D3%B", "gJhH", "6aPB", "6zzG", "G8Rt", "4Jur", "wKAU", "HYm@", "Ud@w", "FN2E", "LfWa", "Uddd", "!keJ", "xG!F", "fwA#", "JkYx", "9u:A", "8yRR", "iep@", "mzMg", "TZ5!", "dzrG", "iX3t", "7bM?", "XXn2", "UjZy", "vWyN", "7+GH", "WUdZ", "+aA+", "Jez=", "Rf2q", "NPL@", "aYCP", "@Guh", "atcM", "McDD", "#E3x", "EV4a", "7DqX", "EEjt", "gJjT", "nXhb", "sgRs", "hLFR", "ggLn", "MiF#", "mBzE", "dHaj", "LMT2", "A7qi", "b3:2", "oEG@", "L@q=", "w8bv", "ie%Y", "?:zx", "ARMF", "Wsa+" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));
		}
		#endregion //Card Based
	}
}
