using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniFootball.Maze{

public class MazeSolver : MonoBehaviour
{
    public Maze mazeGenerator;
    public GameObject pathPrefab; 
    public float pathHeight = 0.5f; 
    public Color pathColor = Color.green;
    public Color visitedColor = Color.yellow;
    public float solveSpeed = 0.1f; 
    
    private List<Vector2Int> _solution;
    private List<GameObject> _pathMarkers = new List<GameObject>();
    
    [ContextMenu("Solve Maze")]
    public async void SolveMaze()
    {
        ClearExistingPath();
        
        Vector2Int start = new Vector2Int(mazeGenerator.widthMaze / 2, 0);
        Vector2Int end = new Vector2Int(mazeGenerator.widthMaze / 2, mazeGenerator.heightMaze - 1);
        
        // Create a grid to track visited cells and their previous positions
        bool[,] visited = new bool[mazeGenerator.widthMaze, mazeGenerator.heightMaze];
        Dictionary<Vector2Int, Vector2Int> previousCell = new Dictionary<Vector2Int, Vector2Int>();
        
        // Use A* algorithm to find the path
        bool pathFound = await AStarSearch(start, end, visited, previousCell);
        
        if (pathFound)
        {
            // Reconstruct and visualize the path
            _solution = ReconstructPath(start, end, previousCell);
            VisualizePath(_solution);
        }
        else
        {
            Debug.LogWarning("No path found!");
        }
    }
    
    private async System.Threading.Tasks.Task<bool> AStarSearch(
        Vector2Int start, 
        Vector2Int end, 
        bool[,] visited,
        Dictionary<Vector2Int, Vector2Int> previousCell)
    {
        // Priority queue with estimated total cost as priority
        var openSet = new SortedDictionary<float, HashSet<Vector2Int>>();
        var cellCosts = new Dictionary<Vector2Int, float>();
        
        // Add start position
        AddToOpenSet(openSet, 0, start);
        cellCosts[start] = 0;
        
        while (openSet.Count > 0)
        {
            // Get the cell with lowest estimated cost
            Vector2Int current = openSet.First().Value.First();
            float currentCost = cellCosts[current];
            
            openSet.First().Value.Remove(current);
            if (openSet.First().Value.Count == 0)
                openSet.Remove(openSet.First().Key);
            
            // Check if we reached the end
            if (current == end)
                return true;
            
            visited[current.x, current.y] = true;
            
            // Visualize visited cell
            VisualizeVisitedCell(current);
            await System.Threading.Tasks.Task.Delay((int)(solveSpeed * 1000));
            
            // Check all possible neighbors
            foreach (Vector2Int neighbor in GetValidNeighbors(current))
            {
                if (visited[neighbor.x, neighbor.y])
                    continue;
                
                float newCost = currentCost + 1;
                
                if (!cellCosts.ContainsKey(neighbor) || newCost < cellCosts[neighbor])
                {
                    cellCosts[neighbor] = newCost;
                    float priority = newCost + ManhattanDistance(neighbor, end);
                    AddToOpenSet(openSet, priority, neighbor);
                    previousCell[neighbor] = current;
                }
            }
        }
        
        return false;
    }
    
    private void AddToOpenSet(SortedDictionary<float, HashSet<Vector2Int>> openSet, float priority, Vector2Int cell)
    {
        if (!openSet.ContainsKey(priority))
            openSet[priority] = new HashSet<Vector2Int>();
        openSet[priority].Add(cell);
    }
    
    private float ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    
    private List<Vector2Int> GetValidNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Cell currentCell = mazeGenerator.GetCell(pos.x, pos.y);
        
        // Check each direction based on wall presence
        if (!currentCell.NorthWall && pos.y < mazeGenerator.heightMaze - 1)
            neighbors.Add(new Vector2Int(pos.x, pos.y + 1));
        if (!currentCell.SouthWall && pos.y > 0)
            neighbors.Add(new Vector2Int(pos.x, pos.y - 1));
        if (!currentCell.EastWall && pos.x < mazeGenerator.widthMaze - 1)
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y));
        if (!currentCell.WestWall && pos.x > 0)
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y));
        
        return neighbors;
    }
    
    private List<Vector2Int> ReconstructPath(Vector2Int start, Vector2Int end, Dictionary<Vector2Int, Vector2Int> previousCell)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;
        
        while (current != start)
        {
            path.Add(current);
            current = previousCell[current];
        }
        path.Add(start);
        path.Reverse();
        
        return path;
    }
    
    private void VisualizePath(List<Vector2Int> path)
    {
        foreach (GameObject marker in path.Select(pos => new Vector3(
                     pos.x * mazeGenerator.cellSize,
                     pathHeight,
                     pos.y * mazeGenerator.cellSize
                 ) + mazeGenerator.transform.position).Select(worldPos => Instantiate(pathPrefab, worldPos, Quaternion.identity)))
        {
            marker.transform.parent = transform;
            
            // Set the color of the path marker
            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = pathColor;
            }
            
            _pathMarkers.Add(marker);
        }
    }
    
    private void VisualizeVisitedCell(Vector2Int pos)
    {
        Vector3 worldPos = new Vector3(
            pos.x * mazeGenerator.cellSize,
            pathHeight,
            pos.y * mazeGenerator.cellSize
        ) + mazeGenerator.transform.position;
        
        GameObject marker = Instantiate(pathPrefab, worldPos, Quaternion.identity);
        marker.transform.parent = transform;
        
        Renderer localRenderer = marker.GetComponent<Renderer>();
        if (localRenderer)
        {
            localRenderer.material.color = visitedColor;
        }
        
        _pathMarkers.Add(marker);
    }
    
    private void ClearExistingPath()
    {
        foreach (GameObject marker in _pathMarkers.Where(marker => marker))
        {
            Destroy(marker);
        }

        _pathMarkers.Clear();
    }
}

}