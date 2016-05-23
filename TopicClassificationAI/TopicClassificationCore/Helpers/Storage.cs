using DataAccessLayer.Contexts;
using DataAccessLayer.Models;
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
		//public static string GetValue(string value)
		//{
		//	if (ApplicationData.Current.LocalSettings.Values.ContainsKey(value))
		//	{
		//		return ApplicationData.Current.LocalSettings.Values[value].ToString();
		//	}
		//	else
		//	{
		//		return null;
		//	}
		//}

		//public static void SetValue(string key, string value)
		//{
		//	ApplicationData.Current.LocalSettings.Values[key] = value;
		//}

		public static async Task StoreArticle(string text, ClassificationTopics topic)
		{
			using (var context = new TopicClassificationContext())
			{
				var article = new Article();
				article.Text = text;
				article.Topic = (int)topic;

				context.Articles.Add(article);
				await context.SaveChangesAsync();

				var words = text.Split(' ');

				foreach (var word in words)
				{
					await AddWordOccurance(context, article.Id, word, topic);
				}

			}
		}

		public static async Task AddWordOccurance(TopicClassificationContext context, int articleId, string word, ClassificationTopics topic)
		{
			var wordEntity = context.Words.FirstOrDefault(w => w.Text == word);

			if (wordEntity == null)
			{
				wordEntity = new Word();
				wordEntity.Text = word;

				context.Words.Add(wordEntity);
				await context.SaveChangesAsync();
			}

			var wordOccurance = context.WordOccurences.FirstOrDefault(wo => wo.ArticleId == articleId && wo.WordId == wordEntity.Id);

			if (wordOccurance == null)
			{
				wordOccurance = new WordOccurence();
				wordOccurance.WordId = wordEntity.Id;
				wordOccurance.ArticleId = articleId;
				wordOccurance.TimesOccured = 1;
				context.WordOccurences.Add(wordOccurance);
			}
			else
			{
				wordOccurance.TimesOccured++;
			}

			await context.SaveChangesAsync();
		}

		public static ClassificationTopics FindWordTopic(string word)
		{
			//var storedWord = GetValue(word);

			//var occurences = new TopicsOccurences();
			//if (!string.IsNullOrEmpty(storedWord))
			//{
			//	occurences.Deserialize(storedWord);
			//}
			//else
			//{
			//	throw new Exception("not recognized word");
			//}

			//return occurences.GetWordTopic();

			using (var context = new TopicClassificationContext())
			{
				var words = context.Words.ToList();

				var wordEntity = context.Words.FirstOrDefault(w => w.Text == word);

				if (wordEntity == null)
				{
					throw new Exception("Not recognized word");
				}

				var allWordOccurences = context.WordOccurences.Where(wo => wo.WordId == wordEntity.Id).ToList();

				if (!allWordOccurences.Any())
				{
					throw new Exception("Not recognized word");
				}

				var firstArticle = context.Articles.FirstOrDefault(a => a.Id == allWordOccurences.FirstOrDefault().ArticleId);

				return (ClassificationTopics)firstArticle.Topic;

				return ClassificationTopics.Arts;
			}
		}

		public static async Task ClearData()
		{
			await ApplicationData.Current.ClearAsync();
		}

		public static ClassificationTopics ConvertTopicFromDatabase(int articleTopic)
		{
			var topic = (ClassificationTopics)articleTopic;

			return topic;
		}
	}
}
