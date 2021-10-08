#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics;

namespace Be.Stateless.Diagnostics
{
	internal static class EventLog
	{
		internal static void WriteError(string message)
		{
			WriteEntry(message, EventLogEntryType.Error);
		}

		internal static void WriteInformation(string message)
		{
			WriteEntry(message, EventLogEntryType.Information);
		}

		internal static void WriteWarning(string message)
		{
			WriteEntry(message, EventLogEntryType.Warning);
		}

		private static void WriteEntry(string message, EventLogEntryType entryType)
		{
			var currentProcess = Process.GetCurrentProcess();
			System.Diagnostics.EventLog.WriteEntry(
				"BizTalk.Factory",
				$@"{message}
Process Name: {currentProcess.MainModule!.ModuleName}
Process Id: {currentProcess.Id}
AppDomain: {AppDomain.CurrentDomain.FriendlyName}",
				entryType);
		}
	}
}
