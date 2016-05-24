using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Helpers
{
	public class TopicsRanklist
	{
		private Dictionary<ClassificationTopics, double> rankList = new Dictionary<ClassificationTopics, double>();

		public Dictionary<ClassificationTopics, double> Ranklist
		{
			get
			{
				return rankList;
			}
		}

		public TopicsRanklist()
		{
			var enumValues = Enum.GetValues(typeof(ClassificationTopics));

			foreach (var enumValue in enumValues)
			{
				rankList.Add((ClassificationTopics)enumValue, 0.0);
			}
		}

		public void AddScore(ClassificationTopics topic, double score)
		{
			rankList[topic] += score;
		}

		public TopicsRanklist MultiplyScores(double value)
		{
			var newRanklist = new TopicsRanklist();

			foreach (var rank in rankList)
			{
				newRanklist.AddScore(rank.Key, rank.Value * value);
			}

			return newRanklist;
		}

		public void AddScores(TopicsRanklist ranklistToAdd)
		{
			foreach(var element in ranklistToAdd.Ranklist)
			{
				AddScore(element.Key, element.Value);
			}
		}

		public ClassificationTopics GetHighestRankedTopic()
		{
			return rankList.FirstOrDefault(r => r.Value == rankList.Max(rm => rm.Value)).Key;
		}
	}
}
