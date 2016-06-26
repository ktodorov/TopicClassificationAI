using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopicClassificationCore.Helpers;

namespace TopicClassificationCore.Parsers
{
	public abstract class BaseParser
	{
		public List<ClassificationTopics> Topics = new List<ClassificationTopics>();

		public abstract Task Parse(string text, IProgress<double> progress = null);
	}
}
