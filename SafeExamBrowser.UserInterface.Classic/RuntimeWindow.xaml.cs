﻿/*
 * Copyright (c) 2018 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using SafeExamBrowser.Contracts.Configuration;
using SafeExamBrowser.Contracts.I18n;
using SafeExamBrowser.Contracts.Logging;
using SafeExamBrowser.Contracts.UserInterface;
using SafeExamBrowser.UserInterface.Classic.ViewModels;

namespace SafeExamBrowser.UserInterface.Classic
{
	public partial class RuntimeWindow : Window, IRuntimeWindow
	{
		private bool allowClose;
		private ILogContentFormatter formatter;
		private IRuntimeInfo runtimeInfo;
		private IText text;
		private RuntimeWindowViewModel model;
		private WindowClosingEventHandler closing;

		event WindowClosingEventHandler IWindow.Closing
		{
			add { closing += value; }
			remove { closing -= value; }
		}

		public RuntimeWindow(ILogContentFormatter formatter, IRuntimeInfo runtimeInfo, IText text)
		{
			this.formatter = formatter;
			this.runtimeInfo = runtimeInfo;
			this.text = text;

			InitializeComponent();
			InitializeRuntimeWindow();
		}

		public void BringToForeground()
		{
			Dispatcher.Invoke(Activate);
		}

		public new void Close()
		{
			Dispatcher.Invoke(() =>
			{
				allowClose = true;
				base.Close();
			});
		}

		public new void Hide()
		{
			Dispatcher.Invoke(base.Hide);
		}

		public void Notify(ILogContent content)
		{
			Dispatcher.Invoke(() =>
			{
				LogTextBlock.Text += formatter.Format(content) + Environment.NewLine;
				LogScrollViewer.ScrollToEnd();
			});
		}

		public new void Show()
		{
			Dispatcher.Invoke(base.Show);
		}

		public void UpdateStatus(TextKey key, bool showBusyIndication = false)
		{
			Dispatcher.Invoke(() =>
			{
				AnimatedBorder.Visibility = showBusyIndication ? Visibility.Hidden : Visibility.Visible;
				ProgressBar.Visibility = showBusyIndication ? Visibility.Visible : Visibility.Hidden;

				model.StopBusyIndication();
				model.Status = text.Get(key);

				if (showBusyIndication)
				{
					model.StartBusyIndication();
				}
			});
		}

		private void InitializeRuntimeWindow()
		{
			Title = $"{runtimeInfo.ProgramTitle} - Version {runtimeInfo.ProgramVersion}";

			InfoTextBlock.Inlines.Add(new Run($"Version {runtimeInfo.ProgramVersion}") { FontStyle = FontStyles.Italic });
			InfoTextBlock.Inlines.Add(new LineBreak());
			InfoTextBlock.Inlines.Add(new LineBreak());
			InfoTextBlock.Inlines.Add(new Run(runtimeInfo.ProgramCopyright) { FontSize = 10 });

			model = new RuntimeWindowViewModel();
			StatusTextBlock.DataContext = model;

			Closing += (o, args) => args.Cancel = !allowClose;
		}
	}
}
