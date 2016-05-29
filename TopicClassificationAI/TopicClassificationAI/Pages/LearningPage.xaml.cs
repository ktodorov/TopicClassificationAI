using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TopicClassificationCore.Helpers;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TopicClassificationCore.Extensions;
using Windows.UI.Popups;
using System.Threading.Tasks;
using TopicClassificationCore.Exceptions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopicClassificationAI.Pages
{
	class LearningTopic
	{
		public ClassificationTopics Topic;
		public int Value;
		public string Name;

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LearningPage : Page
	{
		public LearningPage()
		{
			this.InitializeComponent();

			var enumValues = Enum.GetValues(typeof(ClassificationTopics));

			foreach(var enumValue in enumValues)
			{
				var learningTopic = new LearningTopic() { Topic = (ClassificationTopics)enumValue, Value = (int)enumValue, Name = ((ClassificationTopics)enumValue).ToString().SeparateCamelCase() };

				learningTopics.Add(learningTopic);
			}

			var orderedTopics = learningTopics.OrderBy(lt => lt.Name);
			learningTopics = new ObservableCollection<LearningTopic>(orderedTopics);

			SystemNavigationManager.GetForCurrentView().BackRequested += LearningPage_BackRequested;

			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
		}

		private void LearningPage_BackRequested(object sender, BackRequestedEventArgs e)
		{
			if (this.Frame.CanGoBack)
			{
				Frame.GoBack();
				e.Handled = true;
			}
		}

		ObservableCollection<LearningTopic> learningTopics = new ObservableCollection<LearningTopic>();

		private async void learnButton_Click(object sender, RoutedEventArgs e)
		{
			var topicsSelected = topicsListView.SelectedItems.ToList().Cast<LearningTopic>().ToList();

			var articleText = articleBox.Text;

			if (string.IsNullOrEmpty(articleText))
			{
				throw new TopicValidationException("Please enter text for the article");
			}

			if (topicsSelected == null || !topicsSelected.Any())
			{
				throw new TopicValidationException("Please select one or more topics for the article");
			}

			progressBar.Visibility = Visibility.Visible;
			progressTextBlock.Visibility = Visibility.Visible;
			learnButton.IsEnabled = false;
			topicsListView.IsEnabled = false;
			articleBox.IsEnabled = false;
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

			var progress = new Progress<double>(percent => progressBar.Value = percent);

			await Task.Delay(100);
			await Storage.StoreArticle(articleText, topicsSelected.Select(t => t.Topic).ToList(), progress);

			learnButton.IsEnabled = true;
			progressBar.Visibility = Visibility.Collapsed;
			progressTextBlock.Visibility = Visibility.Collapsed;
			topicsListView.IsEnabled = true;
			articleBox.IsEnabled = true;
			articleBox.Text = string.Empty;
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
		}
	}
}
