using System;
using System.Collections.Generic;
using System.Text;

namespace RepoRanked.LevelGeneration
{
    public static class DollarValueGenerationQueue
    {
        private static readonly Queue<ValuableObject> queue = new();
        private static ValuableObject? current = null;

        public static void Enqueue(ValuableObject obj)
        {
            lock (queue)
            {
                queue.Enqueue(obj);
            }
        }

        public static bool IsMyTurn(ValuableObject obj)
        {
            lock (queue)
            {
                if (current == null && queue.Peek() == obj)
                {
                    current = obj;
                    return true;
                }
                return current == obj;
            }
        }

        public static void Done(ValuableObject obj)
        {
            lock (queue)
            {
                if (current == obj)
                {
                    queue.Dequeue();
                    current = null;
                }
            }
        }
    }
}
