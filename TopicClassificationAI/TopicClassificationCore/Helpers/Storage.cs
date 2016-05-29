using DataAccessLayer.Contexts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopicClassificationCore.Extensions;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace TopicClassificationCore.Helpers
{
	public static class Storage
	{
		public static async Task StoreArticle(string text, List<ClassificationTopics> topics, IProgress<double> progress = null)
		{
			using (var context = new TopicClassificationContext())
			{
				var article = new Article();
				article.Text = text;

				context.Articles.Add(article);
				await context.SaveChangesAsync();

				foreach (var topic in topics)
				{
					var articleTopic = new ArticleTopic();
					articleTopic.ArticleId = article.Id;
					articleTopic.Topic = (int)topic;
					context.ArticleTopics.Add(articleTopic);
				}

				await context.SaveChangesAsync();

				var words = SplitTextByWords(text);

				var i = 0.0;
				var wordsCount = (double)words.Count;

				foreach (var word in words)
				{
					await Task.Delay(1);
					await AddWordOccurance(context, article.Id, word);
					i++;
					progress?.Report((i / wordsCount) * 100.0);
				}
			}
		}

		public static async Task AddWordOccurance(TopicClassificationContext context, int articleId, string word)
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

		public static async Task<List<ClassificationTopics>> FindArticleTopic(TopicClassificationContext context, Article article, IProgress<double> progress = null)
		{
			var words = SplitTextByWords(article.Text);

			var articleScore = new TopicsRanklist();

			var wordsCount = words.Count;
			var i = 0.0;

			var dictionaryCache = new Dictionary<string, TopicsRanklist>();

			foreach (var word in words)
			{
				await Task.Delay(1);

				var score = await CalculateWordScore(context, article, word, dictionaryCache);
				articleScore.AddScores(score);

				i++;
				progress?.Report((i / wordsCount) * 100);
			}

			var topics = TopicsOccurences.CalculateTopicsByScore(articleScore);
			return topics;
		}

		public static async Task<TopicsRanklist> CalculateWordScore(TopicClassificationContext context, Article article, string word, Dictionary<string, TopicsRanklist> dictionaryCache = null)
		{
			// Save the word in the database if it's not already there
			var wordEntity = context.Words.FirstOrDefault(w => w.Text == word);
			if (wordEntity == null)
			{
				wordEntity = new Word();
				wordEntity.Text = word;
				context.Words.Add(wordEntity);
				await context.SaveChangesAsync();
			}

			// Add word occurence for the current article
			var wordOccurenceInCurrentArticle = context.WordOccurences.Where(wo => wo.WordId == wordEntity.Id && wo.ArticleId == article.Id).FirstOrDefault();
			if (wordOccurenceInCurrentArticle == null)
			{
				var wordOccurence = new WordOccurence();
				wordOccurence.ArticleId = article.Id;
				wordOccurence.WordId = wordEntity.Id;
				wordOccurence.TimesOccured = 1;
				context.WordOccurences.Add(wordOccurence);
			}
			else
			{
				wordOccurenceInCurrentArticle.TimesOccured++;
			}
			await context.SaveChangesAsync();

			// If the word score is already in the dictionary cache, we take it from there
			// Otherwise we calculate it now and store it in the dictionary if it's available
			if (dictionaryCache == null || !dictionaryCache.ContainsKey(word))
			{
				var allWordOccurences = context.WordOccurences.Where(wo => wo.WordId == wordEntity.Id && wo.ArticleId != article.Id).ToList();
				if (!allWordOccurences.Any())
				{
					return null;
				}

				var tfScore = await Task.Run(() => TopicsOccurences.GenerateTermFrequencyForWord(context, article, wordEntity, allWordOccurences));
				var idfScore = TopicsOccurences.GenerateInverseDocumentFrequencyForWord(context, article, wordEntity);

				var score = TopicsOccurences.GenerateScoreForWord(tfScore, idfScore);

				if (dictionaryCache != null)
				{
					dictionaryCache.Add(word, score);
				}

				return score;
			}
			else
			{
				return dictionaryCache[word];
			}
		}

		public static ClassificationTopics ConvertTopicFromDatabase(int articleTopic)
		{
			var topic = (ClassificationTopics)articleTopic;

			return topic;
		}

		public static List<string> SplitTextByWords(string text)
		{
			var filteredText = text.RemoveSpecialSymbols();
			var splitText = filteredText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

			return splitText;
		}
	}
}
