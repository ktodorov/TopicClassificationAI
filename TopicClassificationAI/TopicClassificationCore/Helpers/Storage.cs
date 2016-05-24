using DataAccessLayer.Contexts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace TopicClassificationCore.Helpers
{
	public static class Storage
	{
		public static async Task StoreArticle(string text, ClassificationTopics topic, IProgress<double> progress)
		{
			using (var context = new TopicClassificationContext())
			{
				var article = new Article();
				article.Text = text;
				article.Topic = (int)topic;

				context.Articles.Add(article);
				await context.SaveChangesAsync();

				var words = SplitTextByWords(text);

				var i = 0.0;
				var wordsCount = (double)words.Count;

				foreach (var word in words)
				{
					await Task.Delay(1);
					await AddWordOccurance(context, article.Id, word, topic);
					i++;
					progress.Report((i / wordsCount) * 100.0);
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

		public static async Task<ClassificationTopics> FindArticleTopic(TopicClassificationContext context, Article article)
		{
			var words = SplitTextByWords(article.Text);

			var articleScore = new TopicsRanklist();

			foreach (var word in words)
			{
				var wordEntity = context.Words.FirstOrDefault(w => w.Text == word);

				if (wordEntity == null)
				{
					wordEntity = new Word();
					wordEntity.Text = word;
					context.Words.Add(wordEntity);
					await context.SaveChangesAsync();
				}

				var allWordOccurences = context.WordOccurences.Where(wo => wo.WordId == wordEntity.Id).ToList();

				if (!allWordOccurences.Any())
				{
					var wordOccurence = new WordOccurence();
					wordOccurence.ArticleId = article.Id;
					wordOccurence.WordId = wordEntity.Id;
					wordOccurence.TimesOccured = 1;
					context.WordOccurences.Add(wordOccurence);
					await context.SaveChangesAsync();
					continue;
				}

				var tfScore = await Task.Run(() => TopicsOccurences.GenerateTermFrequencyForWord(context, article, wordEntity, allWordOccurences));
				var idfScore = TopicsOccurences.GenerateInverseDocumentFrequencyForWord(context, article, wordEntity);

				var score = TopicsOccurences.GenerateScoreForWord(tfScore, idfScore);

				articleScore.AddScores(score);
			}

			var topic = TopicsOccurences.CalculateTopicByScore(articleScore);
			return topic;
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

		public static List<string> SplitTextByWords(string text)
		{
			return text.Split(' ').ToList();
		}
	}
}
