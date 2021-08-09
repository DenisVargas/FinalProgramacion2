using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class AStar<T>
{
    public event Action<IEnumerable<T>> OnPathCreation;
    public event Action OnFailPathCreation;

    public IEnumerator Run(
        T start,
        Func<T, bool> isGoal,
        Func<T, IEnumerable<WeightedNode<T>>> explode,
        Func<T, float> getHeuristic)
    {
        var queue = new PriorityQueue<T>();
        var distances = new Dictionary<T, float>();
        var parents = new Dictionary<T, T>();
        var visited = new HashSet<T>();

        distances[start] = 0;
        queue.Enqueue(new WeightedNode<T>(start, 0));

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        while (!queue.IsEmpty)
        {
            if (stopwatch.ElapsedMilliseconds > 1 / 60)
            {
                yield return null;
                stopwatch.Restart();
            }

            var current = queue.Dequeue();
            visited.Add(current.Element);

            if (isGoal(current.Element))
            {
                OnPathCreation?.Invoke(CommonUtils.CreatePath(parents, current.Element));
                yield break;
            }

            IEnumerable<WeightedNode<T>> toEnqueue = explode(current.Element);

            foreach (WeightedNode<T> transition in toEnqueue)
            {
                T neighbour = transition.Element;
                var neighbourToDequeuedDistance = transition.Weight;

                var startToNeighbourDistance =
                    distances.ContainsKey(neighbour) ? distances[neighbour] : float.MaxValue;
                var startToDequeuedDistance = distances[current.Element];

                var newDistance = startToDequeuedDistance + neighbourToDequeuedDistance;

                if (!visited.Contains(neighbour) && startToNeighbourDistance > newDistance)
                {
                    distances[neighbour] = newDistance;
                    parents[neighbour] = current.Element;


                    queue.Enqueue(new WeightedNode<T>(neighbour, newDistance + getHeuristic(neighbour)));
                }
            }
            yield return null;
        }

        OnFailPathCreation?.Invoke();
    }
}