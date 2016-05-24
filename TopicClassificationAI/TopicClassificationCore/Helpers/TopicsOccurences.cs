using DataAccessLayer.Contexts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Helpers
{
	public static class TopicsOccurences
	{
		public static TopicsRanklist GenerateTermFrequencyForWord(TopicClassificationContext context, Article dbArticle, Word word, List<WordOccurence> allWordOccurences)
		{
			var ranklist = new TopicsRanklist();

			var articleIds = allWordOccurences.Select(wo => wo.ArticleId).Distinct().ToList();
			foreach(var articleId in articleIds)
			{
				var article = context.Articles.FirstOrDefault(a => a.Id == articleId);
				if (article != null)
				{
					allWordOccurences.Where(wo => wo.ArticleId == articleId).ToList().ForEach(wo => wo.Article = article);
				}
			}
			var articles = allWordOccurences.Select(wo => wo.Article).Distinct().ToList();

			var score = 0.0;

			foreach (var article in articles)
			{
				var wordOccurencesForCurrentArticle = allWordOccurences.Count(wo => wo.ArticleId == article.Id);
				var occurencesCount = allWordOccurences.Where(wo => wo.Article.Topic == article.Topic).Count();
				score = ((double)wordOccurencesForCurrentArticle) / occurencesCount;
				ranklist.AddScore((ClassificationTopics)article.Topic, score);
			}

			return ranklist;
		}

		public static double GenerateInverseDocumentFrequencyForWord(TopicClassificationContext context, Article dbArticle, Word word)
		{
			var allWordOccurences = context.WordOccurences.Sum(wo => wo.TimesOccured);

			var currentWordOccurences = context.WordOccurences.Where(wo => wo.WordId == word.Id).Sum(wo => wo.TimesOccured);

			var division = (double)allWordOccurences / currentWordOccurences;

			return Math.Log10(division);
		}

		public static TopicsRanklist GenerateScoreForWord(TopicsRanklist tfScore, double idfScore)
		{
			var multipliedRanklist = tfScore.MultiplyScores(idfScore);

			return multipliedRanklist;
		}

		public static ClassificationTopics CalculateTopicByScore(TopicsRanklist score)
		{
			var topic = score.GetHighestRankedTopic();

			return topic;
		}
	}
}
