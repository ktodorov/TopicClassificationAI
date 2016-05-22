using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TopicClassificationCore.Helpers
{
	public static class Storage
	{
		public static string GetValue(string value)
		{
			if (ApplicationData.Current.LocalSettings.Values.ContainsKey(value))
			{
				return ApplicationData.Current.LocalSettings.Values[value].ToString();
			}
			else
			{
				return null;
			}
		}

		public static void SetValue(string key, string value)
		{
			ApplicationData.Current.LocalSettings.Values[key] = value;
		}

		public static void StoreArticle(string text, ClassificationTopics topic)
		{
			var words = text.Split(' ');

			foreach (var word in words)
			{
				AddWordOccurance(word, topic);
			}

			//switch (topic)
			//{
			//	case ClassificationTopics.Arts:

			//		break;
			//	case ClassificationTopics.Business:
			//		break;
			//	case ClassificationTopics.Environment:
			//		break;
			//	case ClassificationTopics.Politics:
			//		break;
			//	case ClassificationTopics.ScienceAndTechnology:
			//		break;
			//	case ClassificationTopics.Sports:
			//		break;
			//}
		}

		public static void AddWordOccurance(string word, ClassificationTopics topic)
		{
			var storedWord = GetValue(word);

			var occurences = new TopicsOccurences();
			if (!string.IsNullOrEmpty(storedWord))
			{
				occurences.Deserialize(storedWord);
			}

			occurences.AddOccurenceToDictionary(topic);

			var serializedWord = occurences.Serialize();

			SetValue(word, serializedWord);
		}

		public static ClassificationTopics FindWordTopic(string word)
		{
			var storedWord = GetValue(word);

			var occurences = new TopicsOccurences();
			if (!string.IsNullOrEmpty(storedWord))
			{
				occurences.Deserialize(storedWord);
			}
			else
			{
				throw new Exception("not recognized word");
			}

			return occurences.GetWordTopic();
		}
	}
}
