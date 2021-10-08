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

namespace Be.Stateless.Runtime.Configuration
{
	/// <summary>
	/// The collection of services to be initialized at start up.
	/// </summary>
	[ConfigurationCollection(typeof(StartupServiceConfigurationElement), AddItemName = SERVICE_COLLECTION_ITEM_NAME)]
	public class StartupServiceConfigurationElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StartupServiceConfigurationElementCollection"/> class.
		/// </summary>
		public StartupServiceConfigurationElementCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
			AddElementName = SERVICE_COLLECTION_ITEM_NAME;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Creates a new <see cref="StartupServiceConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="StartupServiceConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new StartupServiceConfigurationElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">
		/// The <see cref="ConfigurationElement"/>-derived <see cref="StartupServiceConfigurationElement"/> to return the key for.
		/// </param>
		/// <returns>
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			if (element == null) throw new ArgumentNullException(nameof(element));
			return ((StartupServiceConfigurationElement) element).Type.AssemblyQualifiedName!;
		}

		#endregion

		private const string SERVICE_COLLECTION_ITEM_NAME = "service";
	}
}
