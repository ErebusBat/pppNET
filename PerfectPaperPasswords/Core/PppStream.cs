using System;
using System.Collections.Generic;
using System.Text;
using Mono.Math;

namespace PerfectPaperPasswords.Core
{
	public class PppStream
	{
		#region *** ctors
		public PppStream(byte[] sequenceKey, bool allowReverse)
		{
			if (sequenceKey == null)
				throw new ArgumentNullException("sequenceKey");
			SequenceKey = sequenceKey;
			_allowReverse = allowReverse;
			_position = BigInteger.Parse("0");
		}
		#endregion //*** ctors
      
		#region *** Properties
		private bool _allowReverse;
		public bool AllowReverse
		{
			get
			{
				return _allowReverse;
			}
		}

		private byte[] _sequenceKey;
		protected byte[] SequenceKey
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

		private BigInteger _position;
		public BigInteger Position
		{
			get
			{
				return _position;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				_position = value;
			}
		}
		#endregion //*** Properties
      
	}
}
