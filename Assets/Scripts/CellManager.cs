using UnityEngine;
using System;
using System.Collections;

// This class manages all Cells in the game
public class CellManager : MonoBehaviour {

    public Cell cellPrefab; // Reference to the Cell prefab
    public GameManager gameManager; // Reference to the GameManager
    public Sprite clearCell, flag, mine, unknownCell, cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8; // Images
    public Sprite[] numberedCells; // List of all cell images that contain numbers

    private int clearedCells; // The number of cells that have been cleared so far
    private int totalNonMines; // The total number of cells minus the number of mines
    private Cell[] mines; // Array of all Cells that hold mines
    private Cell[][] cells; // Matrix of Cells that make up the grid

    void Start(){

        // Initialise data
        numberedCells = new Sprite[] { cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8 };
        clearedCells = 0;
        totalNonMines = GameRules.NumRows * GameRules.NumCols - GameRules.NumMines;
        cells = new Cell[GameRules.NumRows][];

        // Build the grid
        BuildGrid();
    }

    // This is called whenever the player left-clicks a cell
	public void ClearCell(Cell cell){

        if (cell.image == flag)
            // Take no action if the cell is flagged
			return;
		else if (cell.isMine) {
            // If the cells is a mine, the player loses
			gameManager.EndGame (false);
		} else if(cell.image == unknownCell) {
			
			if (!gameManager.timerStarted) {
                // If this is the first cell, start the game
				GenerateMines (cell);
				gameManager.timerStarted = true;
			}
            // Set the image of the cell being cleared
			SetClearedCellImage (cell);
		} else {
            // If a numbered was clicked, sweep the surrounding Cells
			Cell[] adjacentCells = GetAdjacentCells (cell);
			if(FlagsPlaced(adjacentCells, GetNumFromImage(cell.image)))
				SweepCells (adjacentCells);
		}
	}
    
    // This method displays all mines that weren't flagged
    public void DisplayAllMines() {

        foreach (Cell cell in mines)
            if (cell.image != flag)
                cell.SetImage(mine);
    }

    // Toggles the flagged state of the cell and returns whether it was set as flagged (true) or not-flagged (false)
    public bool FlagCell(Cell cell){

		if (cell.image == unknownCell) {
			cell.SetImage (flag);
            return true;
		} else{
			cell.SetImage (unknownCell);
            return false;
        }
	}

    // Tests whether this cell has been cleared (i.e. it isn't an unknown or flagged cell)
    public bool IsCleared(Cell cell) {
        return (cell.image != unknownCell && cell.image != flag);
    }

    // This method builds the grid on the CellCanvas
	private void BuildGrid(){
        // Build each row iteratively
		for (int i = 0; i < GameRules.NumRows; i++) {

			cells [i] = new Cell[GameRules.NumCols];
			for (int j = 0; j < GameRules.NumCols; j++) {
				
				Cell cell = Instantiate (cellPrefab) as Cell; // Instantiate the Cell
                // Add the Cell to the Canvas
				cell.transform.SetParent (GameObject.FindGameObjectWithTag ("CellCanvas").transform, false);
				cell.Init(i, j, unknownCell); // Initialise the Cell's data

				cells [i] [j] = cell; // Add the cell to the matrix
			}
		}
	}

    // Helper method to check whether there have been numFlags flags marked in the given cells
	private bool FlagsPlaced(Cell[] cells, int numFlags){
	
		int count = 0;
		foreach (Cell c in cells)
			if (c.image == flag)
				count++;

		return count == numFlags;
	}

    // Generates the random locations of all mines and sets each cell at that location as a mine
	private void GenerateMines(Cell firstCell){

		Cell[] adjacentCells = GetAdjacentCells (firstCell); // Get the cells surrounding the first cell clicked
        mines = new Cell[GameRules.NumMines];
		
		for (int i = 0; i < GameRules.NumMines;) {
            // Generate random x and y coordinates within valid range
			int x = UnityEngine.Random.Range (0, GameRules.NumRows);
			int y = UnityEngine.Random.Range (0, GameRules.NumCols);
            Cell candidate = cells[x][y]; // Get the cell at this location in the matrix

            /* Only set the mine and increment if:
             *  - This cell isn't already a mine
             *  - This cell is not connected to the first cell clicked
             *  - This is not the first cell clicked
             */
			if (!candidate.isMine && Array.IndexOf(adjacentCells, candidate) < 0
                && candidate != firstCell) {

				candidate.SetMine ();
                mines[i] = candidate; // Add this cell to mines
				i++; // Increment i
			}
		}
	}

    // Returns an array of all Cells connected to this one
	private Cell[] GetAdjacentCells(Cell cell){
	
		Cell[] adjCells;
        // row, col, endRow and endCol are shorthand ways of writing their alternative
		int r = cell.rowIndex, c = cell.colIndex;
		int endRow = GameRules.NumRows - 1, endCol = GameRules.NumCols - 1;

		if (r == 0)
			if (c == 0)
				adjCells = new Cell[] {
					cells [r+1] [c], cells [r+1] [c+1], cells [r] [c+1]
				};
			else if (c == endCol)
				adjCells = new Cell[] {
					cells [r] [c-1], cells [r+1] [c-1], cells [r+1] [c]
				};
			else
				adjCells = new Cell[] {
					cells [r] [c-1], cells [r+1] [c-1], cells [r+1] [c], cells [r+1] [c+1], cells [r] [c+1]
				};
		else if (r == endRow)
			if (c == 0)
				adjCells = new Cell[] {
					cells [r-1] [c], cells [r-1] [c+1], cells [r] [c+1]
				};
			else if (c == endCol)
				adjCells = new Cell[] {
					cells [r] [c-1], cells [r-1] [c-1], cells [r-1] [c]
				};
			else
				adjCells = new Cell[] {
					cells [r] [c-1], cells [r-1] [c-1], cells [r-1] [c], cells [r-1] [c+1], cells [r] [c+1]
				};
		else if (c == 0)
			adjCells = new Cell[] {
				cells [r-1] [c], cells [r-1] [c+1], cells [r] [c+1], cells [r+1] [c+1], cells [r+1] [c]
			};
		else if (c == endCol)
			adjCells = new Cell[] {
				cells [r-1] [c], cells [r-1] [c-1], cells [r] [c-1], cells [r+1] [c-1], cells [r+1] [c]
			};
		else
			adjCells = new Cell[] {
			    cells[r-1][c], cells[r-1][c-1], cells[r][c-1], cells[r+1][c-1], cells[r+1][c],
			    cells[r+1][c+1], cells[r][c+1], cells[r-1][c+1]
			};

		return adjCells;
	}

    // Counts the number of mines surrounding the given cell
	private int GetMineCount(Cell[] cells){

        int mineCount = 0;
		for (int i = 0; i < cells.Length; i++)
			if (cells[i].isMine)
				mineCount++;

		return mineCount;
	}

    // Helper method to extract the number from given image
	private int GetNumFromImage(Sprite image){
		return Array.IndexOf (numberedCells, image) + 1;
	}

    // Helper method to set the image of a cell that is being cleared and also to sweep the adjacent cells if possible
	private void SetClearedCellImage(Cell cell){

		Cell[] adjacentCells = GetAdjacentCells (cell); // The cells connected to this one
		int mineCount = GetMineCount (adjacentCells); // The number of mines surrounding this cell

		if (mineCount != 0)
			cell.SetImage (numberedCells[mineCount - 1]); // Set the image to the relevant number
		else {
            // If no surrounding mines, set the image to a clear cell and sweep surrounding cells
			cell.SetImage (clearCell);
			SweepCells (adjacentCells);
		}

        gameManager.PlayClearAudio();

        // Increment clearedCells, and if it matches totalNonMines tell the GameManager to end the game
		if (++clearedCells == totalNonMines)
			gameManager.EndGame (true);
	}

    // Clears any of the given cells that do not border a mine
	private void SweepCells(Cell[] cells){

		bool mineFound = false;
		foreach(Cell c in cells){
			if (c.image == unknownCell) {
			
				if (c.isMine) {
					mineFound = true;
					c.SetImage (mine);
				} else
					SetClearedCellImage (c);
			}
		}

        // After sweeping all cells, check if a mine was found and if so, tell the GameManager to end the game
		if (mineFound)
			gameManager.EndGame (false);
	}
}
