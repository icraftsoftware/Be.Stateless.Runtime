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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.Diagnostics;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Runtime.Configuration;

namespace Be.Stateless.Runtime
{
	/// <summary>
	/// This class participates in the creation of new application domains in a process in order to allow any custom
	/// infrastructure code to be executed before other managed code runs.
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.appdomainmanager">AppDomainManager Class</seealso>
	/// <seealso href="https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/appdomainmanagerassembly-element">appDomainManagerAssembly Element</seealso>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Loaded by CLR at startup.")]
	public class StartupServiceManager : AppDomainManager
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		public override void InitializeNewDomain(AppDomainSetup appDomainInfo)
		{
			try
			{
				EventLog.WriteInformation($"{typeof(StartupServiceManager).FullName} is loading startup services...");
				var startupServiceConfigurationElements = StartupServiceManagerConfigurationSection.Current.StartupServices.Cast<StartupServiceConfigurationElement>();
				if (startupServiceConfigurationElements.Any())
				{
					startupServiceConfigurationElements.ForEach(RunStartupService);
					EventLog.WriteInformation($"{typeof(StartupServiceManager).FullName} has loaded startup services.");
				}
				else
				{
					EventLog.WriteInformation($"{typeof(StartupServiceManager).FullName} has not found any startup service to load.");
				}
			}
			catch (Exception exception)
			{
				EventLog.WriteError($"{typeof(StartupServiceManager).FullName} failed to load startup services.\r\n{exception}");
				throw;
			}
		}

		#endregion

		private void RunStartupService(StartupServiceConfigurationElement serviceConfigurationElement)
		{
			try
			{
				var service = (IStartupService) Activator.CreateInstance(serviceConfigurationElement.Type);
				service.Execute();
			}
			catch (Exception exception)
			{
				EventLog.WriteWarning($"{typeof(StartupServiceManager).FullName} failed to run startup service {serviceConfigurationElement.Type.FullName}.\r\n{exception}");
			}
		}
	}
}
