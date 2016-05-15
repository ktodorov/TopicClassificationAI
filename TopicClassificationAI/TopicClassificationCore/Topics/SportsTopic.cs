using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Topics
{
	class SportsTopic : BaseTopic
	{
		public override string DisplayName
		{
			get
			{
				return "Sports";
			}
		}

		public override bool MatchesArticle(string article)
		{
			return true;
		}
	}
}
