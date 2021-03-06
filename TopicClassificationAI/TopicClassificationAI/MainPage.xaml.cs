﻿using DataAccessLayer.Contexts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TopicClassificationAI.Pages;
using TopicClassificationCore.Exceptions;
using TopicClassificationCore.Extensions;
using TopicClassificationCore.Helpers;
using TopicClassificationCore.Parsers;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TopicClassificationAI
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

			var backgroundBrushName = "ApplicationPageBackgroundThemeBrush";
			var backgroundBrush = new SolidColorBrush();
			if (Application.Current.Resources.ContainsKey(backgroundBrushName))
			{
				backgroundBrush = new SolidColorBrush((Application.Current.Resources[backgroundBrushName] as SolidColorBrush).Color);
			}

			var foregroundBrushName = "SystemControlForegroundAccentBrush";
			var foregroundBrush = new SolidColorBrush();
			if (Application.Current.Resources.ContainsKey(foregroundBrushName))
			{
				foregroundBrush = new SolidColorBrush((Application.Current.Resources[foregroundBrushName] as SolidColorBrush).Color);
			}

			var appTitleBar = ApplicationView.GetForCurrentView().TitleBar;
			appTitleBar.BackgroundColor = backgroundBrush.Color;
			appTitleBar.ForegroundColor = foregroundBrush.Color;
			appTitleBar.ButtonBackgroundColor = backgroundBrush.Color;
			appTitleBar.ButtonForegroundColor = foregroundBrush.Color;
		}

		private async void submitArticle_Click(object sender, RoutedEventArgs e)
		{
			topicsMatches.Text = string.Empty;
			topicsMatchesHeader.Visibility = Visibility.Collapsed;

			var articleText = articleBox.Text;

			if (string.IsNullOrEmpty(articleText))
			{
				throw new TopicValidationException("Please enter text for the article");
			}

			using (var context = new TopicClassificationContext())
			{
				var enumValues = Enum.GetValues(typeof(ClassificationTopics));

				foreach (var enumValue in enumValues)
				{
					if (!context.ArticleTopics.Any(at => at.Topic == (int)((ClassificationTopics)enumValue)))
					{
						throw new TopicValidationException($"You must enter at least one article of '{((ClassificationTopics)enumValue).ToString().SeparateCamelCase()}' topic in order to start calculating");
					}
				}
			}

			textProgress.IsActive = true;

			articleBox.IsEnabled = false;
			submitArticle.IsEnabled = false;
			submitArticle.Visibility = Visibility.Collapsed;
			learnTopics.IsEnabled = false;

			var progress = new Progress<double>(percent => textProgress.UpdateProgress(percent));

			var parser = new ArticleParser();
			await Task.Delay(100);
			await parser.Parse(articleText, progress);

			foreach (var topic in parser.Topics)
			{
				topicsMatches.Text += $" - {topic.ToString().SeparateCamelCase()}\n";
			}

			topicsMatchesHeader.Visibility = Visibility.Visible;
			textProgress.IsActive = false;

			submitArticle.IsEnabled = true;
			submitArticle.Visibility = Visibility.Visible;
			articleBox.IsEnabled = true;
			learnTopics.IsEnabled = true;
		}

		private void learnTopics_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(LearningPage));
		}
	}
}
