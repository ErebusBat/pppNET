using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using Mono.Math;
using System.IO;

namespace PerfectPaperPasswords.Core
{
	public class CryptoRandom
	{
		#region *** ctors
		public CryptoRandom(SymmetricAlgorithm crypto, byte[] sequenceKey)
			: this(crypto, sequenceKey, BigInteger.Parse("0"))
		{
		}

		public CryptoRandom(SymmetricAlgorithm crypto, byte[] sequenceKey, BigInteger counter)
		{
			Crypto = crypto;
			Counter = counter;
			SequenceKey = sequenceKey;
			InvalidateBuffer(true);
		}
		#endregion //*** ctors

		#region *** Properties
		private SymmetricAlgorithm _crypto;
		protected SymmetricAlgorithm Crypto
		{
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "Crypto must be a valid symmetric cryptographic instance!");
				_crypto = value;
				_crypto.Padding = PaddingMode.Zeros;
				_crypto.Mode = CipherMode.ECB;
			}
		}

		private BigInteger _counter;
		protected BigInteger Counter
		{
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "Counter must be a valid value!");
				_counter = value;
			}
		}

		private byte[] _buffer;

		private byte[] _sequenceKey;
		protected byte[] SequenceKey
		{
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "SequenceKey can not be null!");
				if (_crypto == null)
					throw new InvalidOperationException("You must set your crypto algorithm before setting the sequence key!");
				if (value.Length * 8 != _crypto.KeySize)
					throw new ArgumentOutOfRangeException("value", String.Format("SequenceKey must match the key size for the give crypto algorithm (Currently set at {0} bits)!", _crypto.KeySize));
				_sequenceKey = value;
				_crypto.Key = _sequenceKey;
			}
		}

		private int _bufferPosition;

		protected int BufferBytesAvailable
		{
			get
			{
				if (_bufferPosition < 0)
					return 0;
				if (_bufferPosition >= _buffer.Length)
					return 0;
				return _buffer.Length - _bufferPosition;
			}
		}
		#endregion //*** Properties

		#region *** Internal Methods
		/// <summary>
		/// Resets the buffer and all related pointers, optionaly zeroing out
		/// the data currently in the buffer.
		/// </summary>
		/// <param name="zeroOut"></param>
		protected void InvalidateBuffer(bool zeroOut)
		{
			//Create the buffer if needed
			if (_buffer == null)
			{
				_buffer = new byte[16]; //128bits
				zeroOut = false; //no need to, they will already be zero
			}
			_bufferPosition = -1;

			if (zeroOut)
			{
				for (int i = 0; i < _buffer.Length; i++)
					_buffer[i] = 0;
			}
		}

		/// <summary>
		/// Builds a byte array based on the current counter value.
		/// The byte array will be padded if needed to match the
		/// current buffer length
		/// </summary>
		/// <returns></returns>
		protected byte[] GetCounterHexValue()
		{
			byte[] counterBytes = _counter.GetBytes();
			counterBytes = SwapByteOrder(counterBytes);
			int padNeeded = _buffer.Length - counterBytes.Length;
			List<byte> padedBytes;

			if (counterBytes.Length < _buffer.Length)
			{
				padedBytes = new List<byte>(_buffer.Length);
				padedBytes.AddRange(counterBytes);
				padedBytes.AddRange(new byte[padNeeded]);
			}
			else
			{
				padedBytes = new List<byte>(counterBytes);
			}
			return padedBytes.ToArray();
		}

		protected static byte[] SwapByteOrder(byte[] bits)
		{
			byte[] newBits = new byte[bits.Length];
			int adjust = bits.Length - 1;
			for (int i = 0; i < bits.Length; i++)
			{
				newBits[i] = bits[adjust - i];
			}
			return newBits;
		}

		protected void FillNextBuffer()
		{
			InvalidateBuffer(false); //No need to zero, we will overwrite all of it anyway

			byte[] counterBytes = GetCounterHexValue();

			using (ICryptoTransform transform = _crypto.CreateEncryptor())
			using (MemoryStream memStream = new MemoryStream(_buffer))
			using (CryptoStream cryptStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write))
			{
				cryptStream.Write(counterBytes, 0, counterBytes.Length);
				cryptStream.FlushFinalBlock();
			}

			//Increase the counter
			_counter += 1;
			_bufferPosition = 0; //now valid
		}
		#endregion //*** Internal Methods

		#region *** Methods
		public virtual void GetBytes(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");

			//Make sure we have bytes and how many we have
			int bytesCurrentlyAvailable = BufferBytesAvailable;
			if (bytesCurrentlyAvailable <= 0)
			{
				FillNextBuffer();
				bytesCurrentlyAvailable = BufferBytesAvailable;
			}

			//Fill the buffer appropriatley
			int bytesUsed = buffer.Length;
			if (bytesCurrentlyAvailable >= buffer.Length)
			{
				//Have enough now to satisfy the request
				Array.Copy(_buffer, _bufferPosition, buffer, 0, buffer.Length);

				//Increment our counter (buffer will be refilled on next call if needed)
				_bufferPosition += bytesUsed;
			}
			else
			{
				//Have to do it over multiple calls
				int destIndex = 0;
				while (destIndex < buffer.Length)
				{
					if (BufferBytesAvailable <= 0)
						FillNextBuffer(); //May not be empty on last call of loop

					int bytesToWrite = buffer.Length - destIndex;
					if (bytesToWrite > BufferBytesAvailable)
						bytesToWrite = BufferBytesAvailable;
					
					Array.Copy(_buffer, _bufferPosition, buffer, destIndex, bytesToWrite);

					//Buffer should be 'empty' now
					_bufferPosition += bytesToWrite;
					destIndex += bytesToWrite;
				}
			}
		}
		#endregion //*** Methods
		
	}
}
