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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopicClassificationAI.Pages
{
	class LearningTopic
	{
		public ClassificationTopics Topic;
		public int Value;

		public override string ToString()
		{
			return Topic.ToString().SeparateCamelCase();
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
				var learningTopic = new LearningTopic() { Topic = (ClassificationTopics)enumValue, Value = (int)enumValue };

				learningTopics.Add(learningTopic);
			}

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

		private void learnButton_Click(object sender, RoutedEventArgs e)
		{
			var topicSelected = (LearningTopic)topicsComboBox.SelectedValue;

			Storage.StoreArticle(articleBox.Text, topicSelected.Topic);
		}
	}
}
