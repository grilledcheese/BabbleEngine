using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabbleEngine
{
    /// <summary>
    /// A subclass of the list that supports add and remove buffers, so changes
    /// can be scheduled to the list while it is being processed, and applied
    /// afterwards.
    /// </summary>
    public class BufferedList<T> : List<T>
    {
        private List<T> AddBuffer = new List<T>();
        private List<T> RemoveBuffer = new List<T>();

        public void BufferAdd(T obj)
        {
            AddBuffer.Add(obj);
        }

        public void BufferRemove(T obj)
        {
            RemoveBuffer.Add(obj);
        }

        public void ApplyBuffers()
        {
            AddRange(AddBuffer);
            RemoveAll(InRemoveBuffer);

            AddBuffer.Clear();
            RemoveBuffer.Clear();
        }

        private bool InRemoveBuffer(T obj)
        {
            return RemoveBuffer.Contains(obj);
        }
    }
}
