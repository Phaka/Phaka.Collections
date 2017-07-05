// Copyright (c) Werner Strydom. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Phaka.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class ConcurrentSet<T> : ISet<T>, IReadOnlyCollection<T>
    {
        private readonly ReaderWriterLockSlim _lock;
        private readonly HashSet<T> _set;

        public ConcurrentSet()
        {
            _set = new HashSet<T>();
            _lock = new ReaderWriterLockSlim();
        }

        public ConcurrentSet(IEqualityComparer<T> comparer)
        {
            _set = new HashSet<T>(comparer);
            _lock = new ReaderWriterLockSlim();
        }

        public ConcurrentSet(IEnumerable<T> collection)
        {
            _set = new HashSet<T>(collection);
            _lock = new ReaderWriterLockSlim();
        }

        public ConcurrentSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _set = new HashSet<T>(collection, comparer);
            _lock = new ReaderWriterLockSlim();
        }

        public IEnumerator<T> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                var array = new T[_set.Count];
                if (array.Length > 0)
                    _set.CopyTo(array, 0);
                return ((IEnumerable<T>) array).GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                _set.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _lock.EnterWriteLock();
            try
            {
                _set.UnionWith(other);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _lock.EnterWriteLock();
            try
            {
                _set.IntersectWith(other);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            _lock.EnterWriteLock();
            try
            {
                _set.ExceptWith(other);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _lock.EnterWriteLock();
            try
            {
                _set.SymmetricExceptWith(other);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.IsSubsetOf(other);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.IsSupersetOf(other);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.IsProperSupersetOf(other);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.IsProperSubsetOf(other);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.Overlaps(other);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.SetEquals(other);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        bool ISet<T>.Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _set.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _set.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.Contains(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.EnterReadLock();
            try
            {
                _set.CopyTo(array, arrayIndex);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _set.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _set.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    ICollection<T> collection = _set;
                    return collection.IsReadOnly;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public T Find(Func<T, bool> item)
        {
            _lock.EnterReadLock();
            try
            {
                return _set.FirstOrDefault(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}