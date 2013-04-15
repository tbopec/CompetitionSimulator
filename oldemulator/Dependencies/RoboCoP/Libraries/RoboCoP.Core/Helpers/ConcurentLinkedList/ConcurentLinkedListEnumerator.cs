using System;
using System.Collections;
using System.Collections.Generic;

namespace RoboCoP.Helpers
{
    public partial class ConcurentLinkedList<T>
    {
        #region Nested type: ConcurentLinkedListEnumerator

        protected class ConcurentLinkedListEnumerator: IEnumerator<ConcurentLinkedListItem>
        {
            private readonly ConcurentLinkedList<T> list;
            private bool finished;

            public ConcurentLinkedListEnumerator(ConcurentLinkedList<T> list)
            {
                this.list = list;
            }

            #region IEnumerator<ConcurentLinkedList<T>.ConcurentLinkedListItem> Members

            /// <inheritdoc/>
            public ConcurentLinkedListItem Current { get; private set; }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                if(finished)
                    return false;

                ConcurentLinkedListItem current;
                if (Current == null)
                {
                    // we are at the begin of the collection
                    // try get first element
                    ConcurentLinkedListItem first;
                    do
                    {
                        first = list.first;
                        // this do-while is required for avoiding 'removing' items
                    } while (first != null && !first.Enter());

                    if (first == null)
                    {
                        // list is empty
                        finished = true;
                        return false;
                    }
                    if (!first.Deleted)
                    {
                        // we've got the first item and it is non-'Deleted'
                        Current = first;
                        return true;
                    }
                    // if first item is in 'Deleted' stage then we should move forward from it = works with it as we'd work with 'Current' item
                    current = first;
                }
                else
                    current = Current;

                ConcurentLinkedListItem next;
                do {
                    do {
                        next = current.Next;
                        // this do-while is required for avoiding 'removing' items
                    } while(next != null && !next.Enter());
                    // now we've 'Entered' next item so previous is not needed anymore:
                    current.Leave();
                    current = next;
                    //do-while is required for acquiring non-'Deleted' items
                } while(next != null && next.Deleted);

                Current = next;
                if (next == null)
                {
                    // end of the list reached
                    finished = true;
                    return false;
                }
                return true;
            }

            /// <inheritdoc/>
            public void Reset()
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc/>
            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                CleanupResourse();
                GC.SuppressFinalize(this);
            }

            #endregion

            /// <summary>
            /// Method which is definitely call after enumerator disposes or garbage collected.
            /// It ensures that <see cref="ConcurentLinkedListItem.Leave"/> was called on the <see cref="Current"/> item.
            /// </summary>
            private void CleanupResourse()
            {
                finished = true;
                if(Current != null) {
                    Current.Leave();
                    Current = null;
                }
            }

            ~ConcurentLinkedListEnumerator()
            {
                CleanupResourse();
            }
        }

        #endregion
    }
}