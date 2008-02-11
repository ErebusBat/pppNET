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
	public class CustomAlphabetTests
		: TestFixture
	{
		PppV3Engine ppp;
		//zombie = 49460b7bbbd3aad3f2cba09864f5e8b01a220ea8c077e9fa996de367e7984af0
		byte[] sequenceKey = pppHelper.GetSequenceKeyFromPassword("zombie");
		string alphabet = "~!@#$%^&*()_+-=<>?ABCDEFGHJKLMNPRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

		[SetUp]
		public void Setup()
		{
			ppp = new PppV3Engine(sequenceKey, alphabet);
		}

		[Test]
		public void FirstThreeCards()
		{
			//int.maxValue = 2147483647
			BigInteger cardNumber = BigInteger.Parse("1");

			string[] knownGood;
			string[] actual;

			//Card 1
			knownGood = new[] { "<gxV", "(bJ_", "Ea?N", "mNeB", ")CYf", "(A(s", "MA_o", "LSo+", "btH^", "v?r>", "^Z<?", "iz>y", "AK=h", ">S+A", "Jo=v", "uyZt", "ccf(", "XsLL", "Cstv", "A!^Z", "-w%f", "(yCk", "ViJK", "if*r", "kAz_", "wAmo", "*ugf", "&HHt", "zhy!", "qoo<", "FB*C", "rDmc", ">&i!", "euvX", "-TcU", "wav>", "S^oP", "K%sy", "MrkD", "XPh?", "*RU_", "(h-)", "*<us", "F<&w", "y~%T", "J&wW", "?tMo", "tLK@", "q$ep", "h<~H", "Cg?F", "<Ku*", "gtTc", "nbWc", "p$wp", "MPxH", "_hEa", "Xo^v", "Amyc", "y+^R", "&umx", "<Z!^", "<gHw", "eqKR", "-b@(", "nT&>", "WJC_", "yFbo", "GA*!", "RY<e" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//Card 2
			cardNumber += 1;
			knownGood = new[] { "hvRG", "*sXy", "NS)X", "nkT$", "*mu(", "=NSz", "JisZ", "JBW%", "!g=X", "_TJS", "~Eyh", "whp@", "^)jr", "uuE=", "RiaP", "uy*K", "?L>&", "Tua~", "Pttw", "(KUc", "xhP&", "&=e_", "*ah&", "P*Hh", "MDD>", "YC_A", "UrTh", "UhAh", "W!=h", "tdqH", "hfJH", "s$GE", "a$FU", "LtsG", "Ae&+", "FcJk", "nF_k", "^ZrC", "VefW", "<noq", "@t!i", "b+Bc", "*FHw", "u@Ct", ">wq-", "nNFx", "Nkzd", "a*Sv", "r=mG", "pV%P", "_j$d", "G_<L", "rNNC", ")<HH", "RtRg", "V%tB", "bq<?", "oP=%", "wunA", "*j)p", "LwJ&", "+htR", "Kq*$", "%^fK", "JcFg", "hMxT", "%LVh", "CLpT", "Li?(", "!wkV" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));

			//Card 3
			cardNumber += 1;
			knownGood = new[] { "R#^i", "qhBc", "a@*a", "WiTG", "hLnB", "N>m>", "dDV~", "T)gG", "PW&x", "*g^_", "&o!~", "bs(a", "PwF)", "cyDk", "N@Va", "M&_F", "ALRp", "qSc+", "K>Ff", "#=?o", "Fb!Y", "BF~$", "ReCG", "cLgu", "#sC#", ">)jP", "#VHi", "rv=E", "#$Ta", "%LHe", "SDDL", "zRY+", "ZMa+", "?TAK", "(~uh", "RSVS", ">j%N", "AvLw", "(hwr", "xo~j", "STok", "j#r&", "CZse", "Gg~e", "eJjY", "prqx", "iDj?", "goPv", "?A%S", "N=VC", "&s!c", "+yTg", "Mnu#", "vAVr", "s*KA", "D%gY", "Hrwg", "dJyx", "sk(=", "Sxxf", ")Vp^", "PX*m", "qJsn", "ht)@", "&-%i", "fc~W", "tZcj", "S?Pq", "Mo*s", "b(wp" };
			Assert.AreEqual(70, knownGood.Length, "Was expecting the knownGood to be 70 long!");
			actual = ppp.GetPasscodesForCard(cardNumber);
			AssertEqual(knownGood, actual, String.Format("Mismatch on card #{0:#,0}", cardNumber));
		}
	}
}
