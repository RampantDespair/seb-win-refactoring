﻿/*
 * Copyright (c) 2018 ETH Zürich, Educational Development and Technology (LET)
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace SafeExamBrowser.Contracts.Behaviour
{
	public interface IRuntimeController : IStartupController
	{
		/// <summary>
		/// Reverts any changes performed during the startup or runtime and releases all used resources.
		/// </summary>
		void FinalizeApplication();

		/// <summary>
		/// Initializes a new session and starts performing the runtime logic / event handling.
		/// </summary>
		void StartSession();
	}
}
