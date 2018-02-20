﻿/*
 * Copyright (c) 2018 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Reflection;
using SafeExamBrowser.Contracts.Configuration;
using SafeExamBrowser.Contracts.Configuration.Settings;

namespace SafeExamBrowser.Configuration
{
	public class ConfigurationRepository : IConfigurationRepository
	{
		private RuntimeInfo runtimeInfo;

		public ISession CurrentSession { get; private set; }
		public Settings CurrentSettings { get; private set; }

		public RuntimeInfo RuntimeInfo
		{
			get
			{
				if (runtimeInfo == null)
				{
					InitializeRuntimeInfo();
				}

				return runtimeInfo;
			}
		}

		public ISession InitializeSession()
		{
			var session = new Session
			{
				Id = Guid.NewGuid(),
				StartupToken = Guid.NewGuid()
			};

			CurrentSession = session;

			return session;
		}

		public ClientConfiguration BuildClientConfiguration()
		{
			return new ClientConfiguration
			{
				RuntimeInfo = RuntimeInfo,
				SessionId = CurrentSession.Id,
				Settings = CurrentSettings
			};
		}

		public Settings LoadSettings(Uri path)
		{
			// TODO

			return LoadDefaultSettings();
		}

		public Settings LoadDefaultSettings()
		{
			var settings = new Settings()
			{
				// TODO
				ServicePolicy = ServicePolicy.Optional
			};

			settings.Browser.StartUrl = "https://www.duckduckgo.com";
			settings.Browser.AllowAddressBar = true;
			settings.Browser.AllowBackwardNavigation = true;
			settings.Browser.AllowDeveloperConsole = true;
			settings.Browser.AllowForwardNavigation = true;
			settings.Browser.AllowReloading = true;

			settings.Taskbar.AllowApplicationLog = true;
			settings.Taskbar.AllowKeyboardLayout = true;
			settings.Taskbar.AllowWirelessNetwork = true;

			CurrentSettings = settings;

			return settings;
		}

		private void InitializeRuntimeInfo()
		{
			var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(SafeExamBrowser));
			var baseAddress = "net.pipe://localhost/safeexambrowser";
			var clientId = Guid.NewGuid();
			var executable = Assembly.GetEntryAssembly();
			var runtimeId = Guid.NewGuid();
			var startTime = DateTime.Now;
			var logFolder = Path.Combine(appDataFolder, "Logs");
			var logFilePrefix = startTime.ToString("yyyy-MM-dd\\_HH\\hmm\\mss\\s");

			runtimeInfo = new RuntimeInfo
			{
				ApplicationStartTime = startTime,
				AppDataFolder = appDataFolder,
				BrowserCachePath = Path.Combine(appDataFolder, "Cache"),
				BrowserLogFile = Path.Combine(logFolder, $"{logFilePrefix}_Browser.txt"),
				ClientId = Guid.NewGuid(),
				ClientAddress = $"{baseAddress}/client/{clientId}",
				ClientExecutablePath = Path.Combine(Path.GetDirectoryName(executable.Location), $"{nameof(SafeExamBrowser)}.Client.exe"),
				ClientLogFile = Path.Combine(logFolder, $"{logFilePrefix}_Client.txt"),
				DefaultSettingsFileName = "SebClientSettings.seb",
				ProgramCopyright = executable.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright,
				ProgramDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), nameof(SafeExamBrowser)),
				ProgramTitle = executable.GetCustomAttribute<AssemblyTitleAttribute>().Title,
				ProgramVersion = executable.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
				RuntimeId = Guid.NewGuid(),
				RuntimeAddress = $"{baseAddress}/runtime/{runtimeId}",
				RuntimeLogFile = Path.Combine(logFolder, $"{logFilePrefix}_Runtime.txt"),
				ServiceAddress = $"{baseAddress}/service"
			};
		}
	}
}
