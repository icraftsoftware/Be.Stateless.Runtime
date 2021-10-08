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
using System.Runtime.Caching;

namespace Be.Stateless.Runtime.Caching
{
	/// <summary>
	/// Simple runtime memory cache base class.
	/// </summary>
	/// <typeparam name="TKey">
	/// The type of the objects to be used as key.
	/// </typeparam>
	/// <typeparam name="TItem">
	/// The type of the objects to be cached.
	/// </typeparam>
	/// <remarks>
	/// For each derived class, <see cref="Cache{TKey,TItem}"/> creates behind the scene a named memory cache instance, i.e. a
	/// <see cref="System.Runtime.Caching.MemoryCache"/>, named after the most-derived class name.
	/// </remarks>
	public abstract class Cache<TKey, TItem>
	{
		/// <summary>
		/// Create the <see cref="Cache{TKey,TItem}"/>-derived instance.
		/// </summary>
		/// <param name="keyFactory">
		/// Converts a <typeparamref name="TKey"/> item key to its string representation.
		/// </param>
		/// <param name="itemFactory">
		/// Returns the item to be added to the cache.
		/// </param>
		protected Cache(Func<TKey, string> keyFactory, Func<TKey, TItem> itemFactory)
		{
			_keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
			_itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
			_cache = new(GetType().Name);
		}

		/// <summary>
		/// Gets, and sets if not already in cache, the <typeparamref name="TItem"/> instance associated to the <typeparamref
		/// name="TKey"/> instance.
		/// </summary>
		/// <param name="key">
		/// The <typeparamref name="TKey"/> key object instance to get or set in cache.
		/// </param>
		/// <returns>
		/// The <typeparamref name="TKey"/> object instance associated to the <typeparamref name="TKey"/> instance.
		/// </returns>
		/// <remarks>
		/// When the cache does not already contain the <typeparamref name="TItem"/> instance, it is inserted into the cache with
		/// <see cref="CacheItemPolicy"/> provided by the derived class.
		/// </remarks>
		[SuppressMessage("ReSharper", "InvertIf")]
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public TItem this[TKey key]
		{
			get
			{
				var keyString = _keyFactory(key);
				if (_cache.Contains(keyString)) return (TItem) _cache[keyString];
				lock (_cache)
				{
					if (!_cache.Contains(keyString))
					{
						var cacheItem = new CacheItem(keyString, _itemFactory(key));
						if (!_cache.Add(cacheItem, CacheItemPolicy)) throw new InvalidOperationException($"{GetType().Name} already contains an entry for '{keyString}'.");
						return (TItem) cacheItem.Value;
					}
				}
				return (TItem) _cache[keyString];
			}
		}

		/// <summary>
		/// <see cref="CacheItemPolicy"/> to be used for any new item that will be added to the cache.
		/// </summary>
		protected abstract CacheItemPolicy CacheItemPolicy { get; }

		/// <summary>
		/// Determines whether a cache entry exists in the cache for the <typeparamref name="TKey"/> instance.
		/// </summary>
		/// <param name="key">
		/// The <typeparamref name="TKey"/> instance to search for in cache.
		/// </param>
		/// <returns>
		/// <c>true</c> if the cache contains an entry for the <typeparamref name="TKey"/> instance; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public bool Contains(TKey key)
		{
			var keyString = _keyFactory(key);
			return _cache.Contains(keyString);
		}

		private readonly MemoryCache _cache;
		private readonly Func<TKey, TItem> _itemFactory;
		private readonly Func<TKey, string> _keyFactory;
	}
}
