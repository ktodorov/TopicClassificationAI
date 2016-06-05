using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TopicClassificationAI.UserControls
{
	public sealed partial class TextProgressUserControl : UserControl
	{
		public bool IsActive
		{
			get
			{
				return progressBar.Visibility == Visibility.Visible;
			}
			set
			{
				progressBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				progressTextBlock.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				if (!value)
				{
					progressTextBlock.Text = string.Empty;
					progressBar.Value = progressBar.Minimum;
				}
			}
		}

		private string _progressText = string.Empty;
		public string ProgressText
		{
			get
			{
				if (!string.IsNullOrEmpty(_progressText))
				{
					return _progressText;
				}

				return progressTextBlock.Text;
			}
			set
			{
				_progressText = value;
			}
		}

		public TextProgressUserControl()
		{
			this.InitializeComponent();
		}

		public void UpdateProgress(double value)
		{
			progressBar.Value = value;

			var intValue = Convert.ToInt32(value);
			progressTextBlock.Text = $"{ProgressText} ({intValue}%)";
		}
	}
}
