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
using System.Configuration;
using Be.Stateless.Runtime.Configuration.Validators;

namespace Be.Stateless.Runtime.Configuration
{
	public class StartupServiceConfigurationElement : ConfigurationElement
	{
		static StartupServiceConfigurationElement()
		{
			_properties.Add(_typeProperty);
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		/// <returns>
		/// The <see cref="ConfigurationPropertyCollection"/> collection of properties for the element.
		/// </returns>
		protected override ConfigurationPropertyCollection Properties => _properties;

		#endregion

		[ConfigurationProperty(TYPE_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		public Type Type => Type.GetType((string) base[_typeProperty], true);

		private const string TYPE_PROPERTY_NAME = "type";

		private static readonly ConfigurationPropertyCollection _properties = new();

		private static readonly ConfigurationProperty _typeProperty = new(
			TYPE_PROPERTY_NAME,
			typeof(string),
			null,
			null,
			new StartupServiceTypeValidator(),
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
	}
}
