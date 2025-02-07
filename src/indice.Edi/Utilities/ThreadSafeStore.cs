﻿#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace indice.Edi.Utilities
{
    internal class ThreadSafeStore<TKey, TValue>
    {
        private readonly object _lock = new object();
        private Dictionary<TKey, TValue> _store;
        private readonly Func<TKey, TValue> _creator;

        public ThreadSafeStore(Func<TKey, TValue> creator) {
            if (creator == null) {
                throw new ArgumentNullException(nameof(creator));
            }

            _creator = creator;
            _store = new Dictionary<TKey, TValue>();
        }

        public TValue Get(TKey key) {
            if (!_store.TryGetValue(key, out var value)) {
                return AddValue(key);
            }

            return value;
        }

        private TValue AddValue(TKey key) {
            var value = _creator(key);

            lock (_lock) {
                if (_store == null) {
                    _store = new Dictionary<TKey, TValue> {
                        [key] = value
                    };
                } else {
                    // double check locking
                    if (_store.TryGetValue(key, out var checkValue)) {
                        return checkValue;
                    }

                    var newStore = new Dictionary<TKey, TValue>(_store) {
                        [key] = value
                    };

#if !(PORTABLE || NETSTANDARD10 || NETSTANDARD13)
                    Thread.MemoryBarrier();
#endif
                    _store = newStore;
                }

                return value;
            }
        }
    }
}