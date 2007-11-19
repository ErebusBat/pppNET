using System;
using System.Collections.Generic;
using System.Text;

namespace ppp.tests
{

	[global::System.Serializable]
	public class TestFailedException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public TestFailedException() { }
		public TestFailedException(string message) : base(message) { }
		public TestFailedException(string message, Exception inner) : base(message, inner) { }
		protected TestFailedException(
		 System.Runtime.Serialization.SerializationInfo info,
		 System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }


		private string _message;
		public string Message
		{
			get
			{
				return _message;
			}
		}
	}
}
