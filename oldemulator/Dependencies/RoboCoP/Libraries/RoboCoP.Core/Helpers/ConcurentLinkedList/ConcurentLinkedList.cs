using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RoboCoP.Helpers
{
    public partial class ConcurentLinkedList<T>: ICollection<T>
    {
        private readonly object lockObj = new object();
        protected ConcurentLinkedListItem first;
        protected ConcurentLinkedListItem last;

        #region ICollection<T> Members

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return ItemsEnumerator().Select(x => x.Value).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            //lock is required because few threads may simultaneously change 'Count', 'first' and 'last'
            lock(lockObj) {
                var newItem = new ConcurentLinkedListItem(item, lockObj);
                newItem.Removing += OnListItemRemoving;
                if(last == null)
                    first = last = newItem;
                else
                    last = last.InsertAfter(newItem);
                Count++;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            //lock is required because few threads may simultaneously change 'Count', 'first' and 'last'
            lock(lockObj) {
                ItemsEnumerator().ForEach(x => x.Delete());
                last = first = null;
                Count = 0;
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            //see comment in 'CopyTo'
            return ItemsEnumerator().Any(x => x.Value.Equals(item));
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            //lock is not required - the worst thing we'll get without lock is some state
            //which probably hasn't ever appeared:
            // - elements which had been removed before operation was started will absence in result
            // - elements which were removed during operation might presence in result or might not
            // - elements which weren't changed or which were added during operation will definitely presence

            T[] me = this.ToArray();
            Array.Copy(me, 0, array, arrayIndex, me.Length);
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            //if it wont be a lock here then two simultaneous 'Remove'
            //can remove single item instead of removing two - this is an error
            lock(lockObj) {
                ConcurentLinkedListItem victim = ItemsEnumerator().FirstOrDefault(x => x.Value.Equals(item));
                bool result = false;
                if(victim != null) {
                    result = true;
                    victim.Delete();
                    Count--;
                }
                return result;
            }
        }

        /// <inheritdoc/>
        public int Count { get; private set; }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        private void OnListItemRemoving(ConcurentLinkedListItem removingItem)
        {
            //lock is required because few threads may simultaneously change 'first' and 'last'
            lock(lockObj) {
                if(last == removingItem)
                    last = removingItem.Prev;
                if(first == removingItem)
                    first = removingItem.Next;
            }
        }

        protected IEnumerable<ConcurentLinkedListItem> ItemsEnumerator()
        {
            using(IEnumerator<ConcurentLinkedListItem> item = new ConcurentLinkedListEnumerator(this))
                while(item.MoveNext())
                    yield return item.Current;
        }
    }
}