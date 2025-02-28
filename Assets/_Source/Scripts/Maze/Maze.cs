using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniFootball.Maze
{
    public class Maze : MonoBehaviour
    {
        public int widthMaze, heightMaze;
        public float cellSize = 1f;
        public GameObject wallPrefab;
        public Vector3 offsets;
        public GameObject balls;

        private bool[,] _visited;
        private Cell[,] _generatedCells;
        private List<Vector3> _position;
        private Vector2Int _startPoint, _endPoint;
        private bool _hasPlaceBall = false;

        private void Start()
        {
            _generatedCells = new Cell[widthMaze, heightMaze];
            _visited = new bool[widthMaze, heightMaze];
            _position = new List<Vector3>(widthMaze * heightMaze);
            GenerateMaze();
        }

        [ContextMenu("Generate Maze")]
        private void GenerateMaze()
        {
            // Debug.Log($"{_startPoint} + {_endPoint}");
            int middleOfMaze = Mathf.CeilToInt(widthMaze/2);
            _startPoint = new Vector2Int(middleOfMaze, 0);
            _endPoint = new Vector2Int(middleOfMaze, heightMaze - 1);
            
            for (int x = 0; x < widthMaze; x++)
            {
                for (int y = 0; y < heightMaze; y++)
                {
                    _generatedCells[x, y].NorthWall = true;
                    _generatedCells[x, y].SouthWall = true;
                    _generatedCells[x, y].EastWall = true;
                    _generatedCells[x, y].WestWall = true;
                }
            }
            
            RecursiveBacktracker(_startPoint.x, _endPoint.y);
            
            // Remove Wall in the start and end for entry 
            _generatedCells[_startPoint.x, _startPoint.y].SouthWall = false;
            _generatedCells[_endPoint.x, _endPoint.y].NorthWall = false;
            
            CreateMaze();
            
            PlaceBallRandom();
        }

        private void PlaceBallRandom()
        {
            balls.transform.position = _position[Random.Range(0, _position.Count)];
        }

        private void RecursiveBacktracker(int x, int y)
        {
            _visited[x, y] = true;
            
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(x, y);
            
            neighbors.Sort((a, b) => 
                Vector2Int.Distance(b, _endPoint).CompareTo(Vector2Int.Distance(a, _endPoint)));
            
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!(Random.value > 0.7f)) continue; // 70% chance to keep priority, 30% to shuffle
                int swapIndex = Random.Range(i, neighbors.Count);
                (neighbors[i], neighbors[swapIndex]) = (neighbors[swapIndex], neighbors[i]);
            }
            
            foreach (Vector2Int neighbor in neighbors.Where(neighbor => !_visited[neighbor.x, neighbor.y]))
            {
                RemoveWalls(x, y, neighbor.x, neighbor.y);
                RecursiveBacktracker(neighbor.x, neighbor.y);
            }
        }

        private void RemoveWalls(int currentX, int currentY, int neighborX, int neighborY)
        {
            if (currentX == neighborX) //Horizontal Walls
            {
                if (currentY < neighborY)
                {
                    _generatedCells[currentX, currentY].NorthWall = false;
                    _generatedCells[neighborX, neighborY].SouthWall = false;
                }
                else
                {
                    _generatedCells[currentX, currentY].SouthWall = false;
                    _generatedCells[neighborX, neighborY].NorthWall = false;
                }
            }
            else //Vertical Walls
            {
                if (currentX < neighborX)
                {
                    _generatedCells[currentX, currentY].EastWall = false;
                    _generatedCells[neighborX, neighborY].WestWall = false;
                }
                else
                {
                    _generatedCells[currentX, currentY].WestWall = false;
                    _generatedCells[neighborX, neighborY].EastWall = false;
                }
            }
        }

        private List<Vector2Int> GetUnvisitedNeighbors(int x, int y)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            
            if (x > 0 && !_visited[x - 1, y]) neighbors.Add(new Vector2Int(x - 1, y));  // West
            if (x < widthMaze - 1 && !_visited[x + 1, y]) neighbors.Add(new Vector2Int(x + 1, y));  // East
            if (y > 0 && !_visited[x, y - 1]) neighbors.Add(new Vector2Int(x, y - 1));  // South
            if (y < heightMaze - 1 && !_visited[x, y + 1]) neighbors.Add(new Vector2Int(x, y + 1));  // North
            
            return neighbors;
        }
        
        private void CreateMaze()
        {
            for (int i = 0; i < widthMaze; i++)
            {
                for (int j = 0; j < heightMaze; j++)
                {
                    Cell cell = _generatedCells[i, j];
                    Vector3 position = new Vector3((i - widthMaze/2) * cellSize, 0, (j - heightMaze/2) * cellSize) + offsets;
                    _position.Add(position);
                    
                    if (cell.NorthWall)
                        CreateWall(position + new Vector3(0, 0, cellSize/2), Quaternion.identity);
                    if (cell.SouthWall)
                        CreateWall(position + new Vector3(0, 0, -cellSize/2), Quaternion.identity);
                    if (cell.EastWall)
                        CreateWall(position + new Vector3(cellSize/2, 0, 0), Quaternion.Euler(0, 90, 0));
                    if (cell.WestWall)
                        CreateWall(position + new Vector3(-cellSize/2, 0, 0), Quaternion.Euler(0, 90, 0));
                }
            }
        }
        
        private void CreateWall(Vector3 position, Quaternion rotation)
        {
            GameObject wall = Instantiate(wallPrefab, position, rotation);
            wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, wall.transform.localScale.z);
            wall.transform.parent = transform;
        }

        public Cell GetCell(int posX, int posY)
        {
            return _generatedCells[posX, posY];
        }
    }
    
    public struct Cell
    {
        public bool NorthWall, SouthWall, EastWall, WestWall;
    }
}