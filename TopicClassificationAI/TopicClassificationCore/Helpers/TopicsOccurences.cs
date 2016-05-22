using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Helpers
{
	public class TopicsOccurences
	{
		Dictionary<ClassificationTopics, int> Occurences;

		public TopicsOccurences()
		{
			Occurences = InitializeEmptyDictionary();
		}

		public string Serialize()
		{
			var serializedBuilder = new StringBuilder();
			foreach (var occurence in Occurences)
			{
				serializedBuilder.Append($"<%{(int)occurence.Key}&{occurence.Value}%>");
			}

			return serializedBuilder.ToString();
		}

		public void Deserialize(string serializedValue)
		{
			var values = serializedValue.Split(new string[] { "<%", "%>" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var value in values)
			{
				var keyValuePair = value.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

				var topicString = keyValuePair[0];
				var topic = 0;
				if (!int.TryParse(topicString, out topic))
				{
					continue;
				}

				var occurencesString = keyValuePair[1];
				var occurences = 0;
				if (!int.TryParse(occurencesString, out occurences))
				{
					continue;
				}

				SetOccurencesToDictionary((ClassificationTopics)topic, occurences);
			}
		}

		public Dictionary<ClassificationTopics, int> InitializeEmptyDictionary()
		{
			var newDictionary = new Dictionary<ClassificationTopics, int>();

			var enumValues = Enum.GetValues(typeof(ClassificationTopics));

			foreach (var enumValue in enumValues)
			{
				newDictionary.Add((ClassificationTopics)enumValue, 0);
			}

			return newDictionary;
		}

		public void AddOccurenceToDictionary(ClassificationTopics topic)
		{
			if (Occurences.ContainsKey(topic))
			{
				Occurences[topic] = Occurences[topic] + 1;
			}
			else
			{
				Occurences[topic] = 1;
			}
		}

		public void SetOccurencesToDictionary(ClassificationTopics topic, int occurences)
		{
			Occurences[topic] = occurences;
		}

		public ClassificationTopics GetWordTopic()
		{
			var maxTopic = Occurences.FirstOrDefault(t => t.Value == Occurences.Max(m => m.Value));

			return maxTopic.Key;
		}
	}
}
