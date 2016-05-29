using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Exceptions
{
	public class TopicValidationException : Exception
	{
		public TopicValidationException()
			: base()
		{ }

		public TopicValidationException(string message)
			: base(message)
		{ }
	}
}
