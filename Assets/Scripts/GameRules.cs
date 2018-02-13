using UnityEngine;

// This class contains static fields that are set in the MainMenu scene and accessed in the MainScene
public class GameRules {

    public static int NumCols = 0, NumRows = 0, NumMines = 0; // The number of rows, columns and mines in the grid
    public static string Difficulty; // "Easy", "Medium" or "Hard"

    // This method sets the values for the static fields
    public static void SetRules(int cols, int rows, int mines, string difficulty) {

        NumCols = cols;
        NumRows = rows;
        NumMines = mines;
        Difficulty = difficulty;
    }
}
