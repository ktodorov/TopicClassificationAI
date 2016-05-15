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
				return mainProgress.IsActive;
			}
			set
			{
				mainProgress.IsActive = value;
				progressText.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				if (!value)
				{
					ProgressText = string.Empty;
				}
			}
		}

		public string ProgressText
		{
			get
			{
				return progressText.Text;
			}
			set
			{
				progressText.Text = value;
			}
		}

		public TextProgressUserControl()
		{
			this.InitializeComponent();
		}
	}
}
