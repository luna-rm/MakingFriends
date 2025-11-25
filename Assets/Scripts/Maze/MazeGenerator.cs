using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    [SerializeField] private MazeCell flowerCell;
    [SerializeField] private MazeCell oceanCell;
    [SerializeField] private MazeCell farmCell;
        
    [SerializeField] private MazeTypeEnum MazeType = MazeTypeEnum.FLOWER;
    
    private MazeCell cellUse;

    [SerializeField] private int mazeWidth; 
    [SerializeField] private int mazeDepth; 
    private MazeCell[,] mazeGrid;

    [SerializeField] private Vector2Int startCell;
    [SerializeField] private Vector2Int endCell;

    [SerializeField] Vector3 position = Vector3.zero; 

    [SerializeField] private int noWalls = 0;

    private void Awake() {
        if(MazeType == MazeTypeEnum.FLOWER) {
            cellUse = flowerCell;
        } else if(MazeType == MazeTypeEnum.OCEAN) {
            cellUse = oceanCell;
        } else if(MazeType == MazeTypeEnum.FARM) {
            cellUse = farmCell;
        }

        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for(int i = 0; i < mazeWidth; i++) {
            for(int j = 0; j < mazeDepth; j++) {
                Vector3 cellLocalPos = new Vector3(i * cellUse.size.x, 0, j * cellUse.size.y);
                
                MazeCell newCell = Instantiate(cellUse, transform);
                newCell.transform.localPosition = cellLocalPos;
                newCell.transform.localRotation = Quaternion.identity; 
                
                mazeGrid[i, j] = newCell;
            }
        }

        GenerateMaze(null, mazeGrid[startCell.x, startCell.y], startCell.x, startCell.y);

        mazeGrid[endCell.x, endCell.y].SetAsExit();
        mazeGrid[startCell.x, startCell.y].ClearLeftWall();

        disableMoreWalls();

        transform.position = position;
    }

    public void GenerateNewMaze() {
        if (mazeGrid != null) {
            foreach (MazeCell cell in mazeGrid) {
                if (cell != null) {
                    Destroy(cell.gameObject);
                }
            }
        }

        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for(int i = 0; i < mazeWidth; i++) {
            for(int j = 0; j < mazeDepth; j++) {
                Vector3 cellLocalPos = new Vector3(i * 4, 0, j * 4);
                
                MazeCell newCell = Instantiate(cellUse, transform);
                newCell.transform.localPosition = cellLocalPos;
                newCell.transform.localRotation = Quaternion.identity;
                
                mazeGrid[i, j] = newCell;
            }
        }

        GenerateMaze(null, mazeGrid[startCell.x, startCell.y], startCell.x, startCell.y);

        mazeGrid[endCell.x, endCell.y].SetAsExit();
        mazeGrid[endCell.x, endCell.y].ClearFrontWall();
        mazeGrid[startCell.x, startCell.y].ClearBackWall();

        disableMoreWalls();
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell, int x, int z) {
        currentCell.Visit();

        if (previousCell != null) {
             ClearWalls(previousCell, currentCell);
        }

        MazeCell nextCell;
        do {
            nextCell = GetNextUnvisitedCell(x, z); 

            if (nextCell != null) {
                Vector2Int nextCoords = GetCellCoords(nextCell);
                GenerateMaze(currentCell, nextCell, nextCoords.x, nextCoords.y);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(int x, int z) {
        var unvisitedCells = GetUnvisitedCells(x, z);
        
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(int x, int z) {
        if (x + 1 < mazeWidth) {
            if (mazeGrid[x + 1, z].IsVisited == false) yield return mazeGrid[x + 1, z];
        }

        if (x - 1 >= 0) {
            if (mazeGrid[x - 1, z].IsVisited == false) yield return mazeGrid[x - 1, z];
        }

        if (z + 1 < mazeDepth) {
            if (mazeGrid[x, z + 1].IsVisited == false) yield return mazeGrid[x, z + 1];
        }

        if (z - 1 >= 0) {
            if (mazeGrid[x, z - 1].IsVisited == false) yield return mazeGrid[x, z - 1];
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell) {
        if (previousCell == null) return;

        float prevX = previousCell.transform.localPosition.x;
        float currX = currentCell.transform.localPosition.x;
        float prevZ = previousCell.transform.localPosition.z;
        float currZ = currentCell.transform.localPosition.z;

        if (prevX < currX) { 
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if (prevX > currX) { 
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }
        if (prevZ < currZ) { 
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (prevZ > currZ) { 
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    private void disableMoreWalls() {
        int num = (int)((float)((mazeDepth*mazeWidth/2.0f)/100)*noWalls);
        while(num > 0) {
            Vector2Int cell = new Vector2Int(Random.Range(1, mazeWidth-1), Random.Range(1, mazeDepth-1));
            int wall = Random.Range(0, 4);

            if(wall == 0) {
                mazeGrid[cell.x-1, cell.y].ClearRightWall();
                mazeGrid[cell.x, cell.y].ClearLeftWall();
            } else if(wall == 1) {
                mazeGrid[cell.x+1, cell.y].ClearLeftWall();
                mazeGrid[cell.x, cell.y].ClearRightWall();
            } else if(wall == 2) {
                mazeGrid[cell.x, cell.y-1].ClearFrontWall();
                mazeGrid[cell.x, cell.y].ClearBackWall();
            } else if(wall == 3) {
                mazeGrid[cell.x, cell.y+1].ClearBackWall();
                mazeGrid[cell.x, cell.y].ClearFrontWall();
            }

            num--;
        }
    }

    private Vector2Int GetCellCoords(MazeCell cell) {
        for(int i=0; i<mazeWidth; i++) {
            for(int j=0; j<mazeDepth; j++) {
                if(mazeGrid[i,j] == cell) return new Vector2Int(i, j);
            }
        }
        return Vector2Int.zero;
    }
}