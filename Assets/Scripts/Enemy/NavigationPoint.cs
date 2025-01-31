using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationPoint : MonoBehaviour
{
    [SerializeField] private float maxDistanceToNeighbor;
    [SerializeField] private LayerMask terrain, player;

    public Dictionary<NavigationPoint, float> pointsWithCost = new Dictionary<NavigationPoint, float>();

    public static List<NavigationPoint> ActiveNavigationPoints;
    
    private void OnEnable() => ActiveNavigationPoints.Add(this);
    private void OnDisable() => ActiveNavigationPoints.Remove(this);
    

    //Add all visible neighbors within the max distance.
    public void FindNeighbors(NavigationPoint[] points)
    {
        foreach (var point in points)
        {
            var distance = Vector3.Distance(transform.position, point.transform.position);
            
            if (distance > maxDistanceToNeighbor) continue;
            if (Physics.Raycast(transform.position, Vector3.Normalize(point.transform.position - transform.position), float.PositiveInfinity, terrain)) continue;
            
            pointsWithCost.Add(point, distance);
        }
        
        if (pointsWithCost.Count == 0) ObjectPoolController.DeactivateInstance(gameObject);
    }

    private List<NavigationPoint> ReconstructPath(Dictionary<NavigationPoint, NavigationPoint> cameFrom, NavigationPoint current)
    {
        var totalPath = new List<NavigationPoint>();
        totalPath.Add(current);
        
        while (cameFrom.Keys.Contains(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        return totalPath;
    }


    public List<NavigationPoint> FindShortestPathToPoint(NavigationPoint targetPoint, float playerCheckRadius = -1f)
    {
        var openSet = new List<NavigationPoint>();
        openSet.Add(this);

        var cameFrom = new Dictionary<NavigationPoint, NavigationPoint>();

        var distanceFromStart = new Dictionary<NavigationPoint, float>();
        distanceFromStart.Add(this, 0f);

        var estimatedRemainingDistance = new Dictionary<NavigationPoint, float>();
        estimatedRemainingDistance.Add(this, Vector3.Distance(transform.position, targetPoint.transform.position));

        while (openSet.Count > 0)
        {
            var current = openSet[0];
            if (current == targetPoint) return ReconstructPath(cameFrom, current);

            openSet.RemoveAt(0);

            foreach (var (neighbor, cost) in current.pointsWithCost)
            {
                if (playerCheckRadius > 0f && 
                    Physics.SphereCast(current.transform.position, playerCheckRadius, 
                        (neighbor.transform.position - current.transform.position).normalized, 
                        out var hitInfo, cost, player)) continue;
                
                
                var newDistanceFromStart = distanceFromStart[current] + cost;
                if (newDistanceFromStart < distanceFromStart[neighbor])
                {
                    cameFrom[current] = current;
                    distanceFromStart[current] = newDistanceFromStart;
                    estimatedRemainingDistance[current] = newDistanceFromStart +
                                                          Vector3.Distance(neighbor.transform.position,
                                                              targetPoint.transform.position);
                    if (!openSet.Contains(neighbor))
                    {
                        var targetIndex = openSet.FindLastIndex(point =>
                            estimatedRemainingDistance[point] < estimatedRemainingDistance[current]);

                        if (targetIndex == -1) openSet.Add(neighbor);
                        openSet.Insert(targetIndex, neighbor);
                    }
                }
            }
        }

        return null;
    }

    public static NavigationPoint FindPointClosestToPosition(Vector3 position)
    {
        return ActiveNavigationPoints.Aggregate((closest, next) =>
                Vector3.Distance(position, next.transform.position) <
                Vector3.Distance(position, closest.transform.position) ? next : closest);
    }
    
    public static NavigationPoint FindPointFurthestFromPosition(Vector3 position)
    {
        return ActiveNavigationPoints.Aggregate((furthest, next) =>
            Vector3.Distance(position, next.transform.position) >
            Vector3.Distance(position, furthest.transform.position) ? next : furthest);
    }
}
