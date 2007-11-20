using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace PerfectPaperPasswords.Core
{
	internal class PppAlphabet
		: IEnumerable<string>
	{
		#region *** ctors
		public PppAlphabet(string alphabet)
		{
			if (string.IsNullOrEmpty(alphabet))
				throw new ArgumentNullException("alphabet");

			_characters = new List<string>(alphabet.Length);
			for(int i =0; i < alphabet.Length; i++)
			{
				string charString = alphabet[i].ToString();
            if(_characters.Contains(charString))
					throw new ArgumentOutOfRangeException("alphabet", String.Format("Alphabet contains a duplicate character ({0}) at ordinal {1}", charString, i));
				else
					_characters.Add(charString);
			}
			_characters.Sort(PppAlphabetSorter);
			_charactersReadOnly = _characters.AsReadOnly();
		}
		#endregion //*** ctors
      
		#region *** Properties
		private List<string> _characters;
		private ReadOnlyCollection<string> _charactersReadOnly;
		protected ReadOnlyCollection<string> Characters
		{
			get
			{
				return _charactersReadOnly;
			}
		}

		public int Count
		{
			get
			{
				return _characters.Count;
			}
		}

		public char this[int index]
		{
			get
			{
				return _characters[index][0];
			}
		}
		#endregion //*** Properties

		#region *** Methods
		int PppAlphabetSorter(string x, string y)
		{
			byte xbyte = ((byte)x[0]);
			byte ybyte = ((byte)y[0]);
			return xbyte.CompareTo(ybyte);
		}
		#endregion //*** Methods


		
		#region IEnumerable Members
		public IEnumerator<string> GetEnumerator()
		{
			return _characters.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
