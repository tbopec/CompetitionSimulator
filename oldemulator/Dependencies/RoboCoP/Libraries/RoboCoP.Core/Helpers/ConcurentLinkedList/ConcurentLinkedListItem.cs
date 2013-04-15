using System;
using System.Threading;

namespace RoboCoP.Helpers
{
    public partial class ConcurentLinkedList<T>
    {
        #region Nested type: ConcurentLinkedListItem

        /// <summary>
        /// Item of the <see cref="ConcurentLinkedList{T}"/>.
        /// It tracks number of the <see cref="ConcurentLinkedListEnumerator"/> which observes it using methods <see cref="Enter"/> and <see cref="Leave"/>.
        /// If the item should be deleted (method <see cref="Delete"/> is called) from the list
        /// but there are still some observing <see cref="ConcurentLinkedListEnumerator"/> then such item is marked as 'Deleted' (<see cref="Deleted"/> is set to true).
        /// When all observing <see cref="ConcurentLinkedListEnumerator"/> leaves 'Deleted' item or if that item was not observed when it was deleted then such item
        /// is removed (method <see cref="Remove"/> is called): item marked as 'removed' (flag <see cref="removed"/>), then all reference
        /// from the <see cref="ConcurentLinkedList{T}"/> and from the <see cref="Next"/> and <see cref="Prev"/> items to that item are removed.
        /// 
        /// When you use this object you should kip in mind that special 'removing' stage - you may get an reference to the 'removing' item.
        /// Such reference should be dropt and retaken.
        /// If method <see cref="Enter"/> returns false it means that item is in 'removing' stage.
        /// 
        /// Also you should track you usage of the item with methods <see cref="Enter"/> and <see cref="Leave"/>.
        /// </summary>
        protected class ConcurentLinkedListItem
        {
            private readonly object wideLock;
            private uint observerCount;
            private bool removed;
//todo: maybe use common lock ?
            private SpinLock spinLock;

            public ConcurentLinkedListItem(T value, object wideLock)
            {
                this.wideLock = wideLock;
                Value = value;
            }

            /// <summary>
            /// Previous item in <see cref="ConcurentLinkedList{T}"/>.
            /// </summary>
            public ConcurentLinkedListItem Prev { get; private set; }

            /// <summary>
            /// Next item in <see cref="ConcurentLinkedList{T}"/>.
            /// </summary>
            public ConcurentLinkedListItem Next { get; private set; }

            /// <summary>
            /// Value of the item.
            /// </summary>
            public T Value { get; private set; }

            /// <summary>
            /// Flag which indicates was this item <see cref="Delete"/>d or not.
            /// </summary>
            public bool Deleted { get; private set; }

            /// <summary>
            /// Mark this item as observed by one more enumerator.
            /// Return false if the item is in 'removing' stage - then item is not marked as observed.
            /// Return true otherwise.
            /// </summary>
            public bool Enter()
            {
                bool lockTaken = false;
                spinLock.Enter(ref lockTaken);
                if(removed) {
                    spinLock.Exit();
                    return false;
                }
                observerCount++;
                spinLock.Exit();
                return true;
            }

            private void Remove()
            {
                removed = true;
                // here we should use lock to avoid situation when two serial items are removing simultaneously
                // this lock should be 'wideLock' not 'spinLock' to avoid deadlocks between two serial items
                lock(wideLock) {
                    // we should inform list that item it removing - list should update it's first and last to avoid dead links
                    Removing(this);

                    if(Prev != null)
                        Prev.Next = Next;
                    if(Next != null)
                        Next.Prev = Prev;
                    Next = Prev = null;
                }
            }

            /// <summary>
            /// Invokes before <see cref="ConcurentLinkedListItem"/> is removed from <see cref="ConcurentLinkedList{T}"/>
            ///  - <see cref="ConcurentLinkedList{T}"/> should update it's <see cref="ConcurentLinkedList{T}.first"/> and <see cref="ConcurentLinkedList{T}.last"/>
            /// references to avoid dead links.
            /// </summary>
            public event Action<ConcurentLinkedListItem> Removing;

            /// <summary>
            /// Informs the item that it is not more observed.
            /// </summary>
            public void Leave()
            {
                bool lockTaken = false;
                spinLock.Enter(ref lockTaken);
                observerCount--;
                if(Deleted && observerCount == 0)
                    Remove();
                spinLock.Exit();
            }

            /// <summary>
            /// Marks the item as deleted.
            /// </summary>
            public void Delete()
            {
                Deleted = true;
                bool lockTaken = false;
                spinLock.Enter(ref lockTaken);
                if(observerCount == 0)
                    Remove();
                spinLock.Exit();
            }

            /// <summary>
            /// Insert one more item after this.
            /// </summary>
            public ConcurentLinkedListItem InsertAfter(ConcurentLinkedListItem next)
            {
                if(next == null)
                    throw new ArgumentNullException("next");
                bool lockTaken = false;
                spinLock.Enter(ref lockTaken);
                bool invalidOperation = false;
                if(removed)
                    invalidOperation = true;
                else {
                    Next = next;
                    next.Prev = this;
                }
                spinLock.Exit();
                if(invalidOperation)
                    throw new InvalidOperationException();
                return next;
            }
        }

        #endregion
    }
}