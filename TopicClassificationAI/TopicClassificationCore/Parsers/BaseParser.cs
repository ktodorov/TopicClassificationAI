using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopicClassificationCore.Topics;

namespace TopicClassificationCore.Parsers
{
	public abstract class BaseParser
	{
		public List<BaseTopic> Topics = new List<BaseTopic>();

		public abstract Task Parse(string text);
	}
}
