using System.Collections.Generic;
using UnityEngine;
using System;

namespace Team.Gameplay.TurnSystem
{
    public class TurnQueue<T>
    {
        private Queue<T> turnQueue;

        /// <summary>
        /// Invoked when the queue is empty after completing all turns.
        /// </summary>
        public event Action OnQueueFinished;

        public TurnQueue()
        {
            turnQueue = new Queue<T>();
        }

        // Add an entity to the turn queue
        public void AddToQueue(T entity)
        {
            if (!turnQueue.Contains(entity))
            {
                turnQueue.Enqueue(entity);
            }
        }

        // Remove an entity from the queue
        public void RemoveFromQueue(T entity)
        {
            if (!turnQueue.Contains(entity)) return;

            Queue<T> newQueue = new Queue<T>();

            while (turnQueue.Count > 0)
            {
                T current = turnQueue.Dequeue();
                if (!EqualityComparer<T>.Default.Equals(current, entity))
                {
                    newQueue.Enqueue(current);
                }
            }

            turnQueue = newQueue;
        }

        // Get the current turn entity without dequeuing
        public T PeekCurrentTurn()
        {
            return turnQueue.Peek();
        }

        /// <summary>
        /// Proceed to the next turn. When the queue becomes empty, the OnQueueFinished event is invoked.
        /// </summary>
        public T NextTurn()
        {
            if (IsQueueEmpty())
            {
                Debug.LogWarning("NextTurn called on an empty queue.");
                return default;
            }

            T entity = turnQueue.Dequeue();

            return entity;
        }

        // Check if queue is empty
        public bool IsQueueEmpty()
        {
            return turnQueue.Count == 0;
        }

        // Clear the queue
        public void ClearQueue()
        {
            turnQueue.Clear();
        }

        // Get current queue (e.g. for UI)
        public IEnumerable<T> GetQueue()
        {
            return turnQueue;
        }
    }
}
