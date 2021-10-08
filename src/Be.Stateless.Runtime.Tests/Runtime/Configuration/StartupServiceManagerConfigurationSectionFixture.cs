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

using System.Configuration;
using System.Linq;
using Be.Stateless.Dummy;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Runtime.Configuration
{
	public class StartupServiceManagerConfigurationSectionFixture
	{
		[Fact]
		public void EmptyStartup()
		{
			Invoking(() => ConfigurationManager.GetSection("be.stateless.test/runtime/emptyStartup"))
				.Should().NotThrow()
				.And.Subject().Should().NotBeNull();
		}

		[Fact]
		public void Startup()
		{
			var startUpManagerConfigurationSection = (StartupServiceManagerConfigurationSection) ConfigurationManager
				.GetSection("be.stateless.test/runtime/startup");
			startUpManagerConfigurationSection.Should().NotBeNull();
			startUpManagerConfigurationSection!.StartupServices.Cast<StartupServiceConfigurationElement>().Select(s => s.Type)
				.Should().BeEquivalentTo(
					new[] {
						typeof(StartupServiceOne),
						typeof(StartupServiceTwo)
					});
		}

		[Fact]
		public void StartupWithIncompleteService()
		{
			Invoking(() => ConfigurationManager.GetSection("be.stateless.test/runtime/startupWithIncompleteService"))
				.Should().Throw<ConfigurationErrorsException>()
				.WithMessage("Required attribute 'type' not found.*");
		}

		[Fact]
		public void StartupWithInvalidService()
		{
			Invoking(() => ConfigurationManager.GetSection("be.stateless.test/runtime/startupWithInvalidService"))
				.Should().Throw<ConfigurationErrorsException>()
				.WithMessage($"The value for the property 'type' is not valid. The error is: '{typeof(int).FullName}' does not implement {nameof(IStartupService)}.*");
		}

		[Fact]
		public void StartupWithoutServices()
		{
			var startUpManagerConfigurationSection = (StartupServiceManagerConfigurationSection) ConfigurationManager
				.GetSection("be.stateless.test/runtime/startupWithoutServices");
			startUpManagerConfigurationSection.Should().NotBeNull();
			startUpManagerConfigurationSection!.StartupServices.Cast<object>().Should().BeEmpty();
		}

		[Fact]
		public void UndeclaredStartup()
		{
			// App.Config does not declare be.stateless/runtime/startup
			StartupServiceManagerConfigurationSection.Current.Should().NotBeNull();
			StartupServiceManagerConfigurationSection.Current.StartupServices.Should().NotBeNull();
			StartupServiceManagerConfigurationSection.Current.StartupServices.Cast<object>().Should().BeEmpty();
		}
	}
}
