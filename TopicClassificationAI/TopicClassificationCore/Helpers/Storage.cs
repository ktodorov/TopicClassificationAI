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

				var groupedWords = GroupTextByWordsAndOccurences(text);

				var i = 0.0;
				var wordsCount = (double)groupedWords.Sum(gw => gw.Value);

				foreach (var groupedWord in groupedWords)
				{
					await Task.Delay(1);
					await AddWordOccurance(context, article.Id, groupedWord);
					i += groupedWord.Value;
					progress?.Report((i / wordsCount) * 100.0);
				}
			}
		}

		public static async Task AddWordOccurance(TopicClassificationContext context, int articleId, KeyValuePair<string, int> groupedWord)
		{
			var wordEntity = context.Words.FirstOrDefault(w => w.Text == groupedWord.Key);

			if (wordEntity == null)
			{
				wordEntity = new Word();
				wordEntity.Text = groupedWord.Key;

				context.Words.Add(wordEntity);
				await context.SaveChangesAsync();
			}

			var wordOccurance = context.WordOccurences.FirstOrDefault(wo => wo.ArticleId == articleId && wo.WordId == wordEntity.Id);

			if (wordOccurance == null)
			{
				wordOccurance = new WordOccurence();
				wordOccurance.WordId = wordEntity.Id;
				wordOccurance.ArticleId = articleId;
				wordOccurance.TimesOccured = groupedWord.Value;
				context.WordOccurences.Add(wordOccurance);
			}
			else
			{
				wordOccurance.TimesOccured += groupedWord.Value;
			}

			await context.SaveChangesAsync();
		}

		public static async Task<List<ClassificationTopics>> FindArticleTopic(TopicClassificationContext context, Article article, IProgress<double> progress = null)
		{
			var groupedWords = GroupTextByWordsAndOccurences(article.Text);

			var articleScore = new TopicsRanklist();

			var wordsCount = groupedWords.Sum(gw => gw.Value);
			var i = 0.0;

			var wordOccurences = GetAllWordOccurences(context, article);

			foreach (var groupedWord in groupedWords)
			{
				await Task.Delay(1);

				var score = await CalculateWordScore(context, article, groupedWord, wordOccurences);
				articleScore.AddScores(score);

				i += groupedWord.Value;
				progress?.Report((i / wordsCount) * 100);
			}

			var topics = TopicsOccurences.CalculateTopicsByScore(articleScore);
			return topics;
		}

		/// <summary>
		/// Calculate the word score based on the current article appearances.
		/// </summary>
		/// <param name="context">The context from which we should get the data</param>
		/// <param name="article">Current article for which we calculate the score</param>
		/// <param name="groupedWord">Grouped word key-value pair. The key is the word and the value is the number of appearances in the current article</param>
		public static async Task<TopicsRanklist> CalculateWordScore(TopicClassificationContext context, Article article, KeyValuePair<string, int> groupedWord, List<WordOccurence> allWordOccurences)
		{
			// Save the word in the database if it's not already there
			var wordEntity = context.Words.FirstOrDefault(w => w.Text == groupedWord.Key);
			if (wordEntity == null)
			{
				wordEntity = new Word();
				wordEntity.Text = groupedWord.Key;
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
				wordOccurence.TimesOccured = groupedWord.Value;
				context.WordOccurences.Add(wordOccurence);
			}
			else
			{
				wordOccurenceInCurrentArticle.TimesOccured += groupedWord.Value;
			}
			await context.SaveChangesAsync();

			var wordOccurences = allWordOccurences.Where(wo => wo.WordId == wordEntity.Id).ToList();
			if (!wordOccurences.Any())
			{
				return null;
			}

			var tfScore = await Task.Run(() => TopicsOccurences.GenerateTermFrequencyForWord(context, article, wordEntity, wordOccurences));
			var idfScore = TopicsOccurences.GenerateInverseDocumentFrequencyForWord(context, article, wordEntity);

			var score = TopicsOccurences.GenerateScoreForWord(tfScore, idfScore);

			wordOccurences = null;

			return score;
		}

		public static ClassificationTopics ConvertTopicFromDatabase(int articleTopic)
		{
			var topic = (ClassificationTopics)articleTopic;
			return topic;
		}

		/// <summary>
		/// Group the given text in a dictionary with words as keys and the number of their occurences as values
		/// </summary>
		public static Dictionary<string, int> GroupTextByWordsAndOccurences(string text)
		{
			var filteredText = text.RemoveSpecialSymbols();
			var splitText = filteredText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());

			return splitText;
		}

		/// <summary>
		/// Take all word occurences in the database which are not for the current article and
		/// populate their Article objects and the ArticleTopics objects for the articles
		/// </summary>
		public static List<WordOccurence> GetAllWordOccurences(TopicClassificationContext context, Article article)
		{
			var wordOccurences = context.WordOccurences.Where(wo => wo.ArticleId != article.Id).ToList();

			var articleIds = wordOccurences.Where(wo => wo.Article == null).Select(wo => wo.ArticleId).Distinct().ToList();

			foreach (var articleId in articleIds)
			{
				var tempArticle = context.Articles.FirstOrDefault(a => a.Id == articleId);
				if (tempArticle != null)
				{
					if (tempArticle.Topics == null)
					{
						var articleTopics = context.ArticleTopics.Where(at => at.ArticleId == tempArticle.Id).ToList();
						tempArticle.Topics = articleTopics;
					}

					wordOccurences.Where(wo => wo.ArticleId == tempArticle.Id).ToList().ForEach(wo => wo.Article = tempArticle);
				}
			}

			wordOccurences.RemoveAll(a => a.Article == null);

			return wordOccurences;
		}
	}
}
