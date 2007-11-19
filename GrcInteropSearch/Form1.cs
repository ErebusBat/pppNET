using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mono.Math;
using grc;
using PerfectPaperPasswords;
using PerfectPaperPasswords.Math;

namespace GrcInteropSearch
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			BigInteger passcodeIndex = BigInteger.Parse("0");
			byte[] seqKeyBytes = pppHelper.GetSequenceKeyFromPassword("zombie");
			seqKeyBytes = new byte[] { 0x49, 0x46, 0x0b, 0x7b, 0xbb, 0xd3, 0xaa, 0xd3, 0xf2, 0xcb, 0xa0, 0x98, 0x64, 0xf5, 0xe8, 0xb0, 0x1a, 0x22, 0x0e, 0xa8, 0xc0, 0x77, 0xe9, 0xfa, 0x99, 0x6d, 0xe3, 0x67, 0xe7, 0x98, 0x4a, 0xf0 };
			seqKeyBytes = PppConvert.SwapBits(seqKeyBytes);
			pppNet ppp = new pppNet();
			ppp.SequenceKey = seqKeyBytes;
			ppp.RetrievePasscodes(passcodeIndex.ToString());

			StringBuilder codes = new StringBuilder();
			for (int i = 0; i < 70; i++)
			{
				if (i % 7 == 0)
					codes.AppendLine();
				else
					codes.Append(" ");
				codes.Append(ppp.GetPasscode(i));
			}
			textBox1.Text = codes.ToString();
		}
	}
}