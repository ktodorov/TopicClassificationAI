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
			var score = 0.0;

			foreach (var wordOccurence in allWordOccurences)
			{
				var wordOccurencesCount = wordOccurence.TimesOccured;

				var articleTopics = wordOccurence.Article.GetTopics(context).Select(at => at.Topic).ToList();

				foreach (var topic in articleTopics)
				{
					var occurencesInArticlesWithSameTopic = allWordOccurences.Where(wo => wo.Article.Topics.Select(at => at.Topic).Contains(topic));
					var occurencesCount = occurencesInArticlesWithSameTopic.Sum(wo => wo.TimesOccured);
					score = ((double)wordOccurencesCount) / occurencesCount;
					ranklist.AddScore((ClassificationTopics)topic, score);
				}
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

		public static List<ClassificationTopics> CalculateTopicsByScore(TopicsRanklist score)
		{
			var topic = score.GetHighestRankedTopic();

			var calculatedTopics = new List<ClassificationTopics>();
			calculatedTopics.Add(topic);

			var subtopics = score.GetSubtopics();
			if (subtopics.Any())
			{
				calculatedTopics.AddRange(subtopics);
			}

			return calculatedTopics;
		}
	}
}
