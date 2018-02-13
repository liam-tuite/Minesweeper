using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This class manages various elements of the main scene
public class GameManager : MonoBehaviour {

    public bool gameEnded, timerStarted; // Has the game been ended? Has the timer been started?
    public AudioClip clearClip, flagClip, explosionClip, newHighScoreClip;
    public CellManager cellManager;
    public GameObject gameOverPanel, newHighScorePanel; // UI panels
    public Text minesText, timer;
    private float timeCount; // Counts the time since the game began
    private int minesLeft; // The number of mines yet to be flagged
    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    void Start() {

        // Initialise data
        gameEnded = false;
        timerStarted = false;
        minesLeft = GameRules.NumMines;
        timeCount = 0f;

        // Set the constraint count on the Cell Canvas (the number of cells in each row)
        GameObject.FindGameObjectWithTag("CellCanvas").GetComponent<GridLayoutGroup>().constraintCount = GameRules.NumCols;

        gameObject.AddComponent<AudioSource>();
    }

    void Update() {

        if (gameEnded)
            return;

        // Update the UI elements if the game is still going
        if (timerStarted) {
            // If the timer has begun, increment it
            timeCount += Time.deltaTime;
            timeCount = Mathf.Clamp(timeCount, 0f, Mathf.Infinity);
            timer.text = string.Format("{0:00.00}", timeCount); // format the time
        }

        // update the number of mines left
        minesText.text = minesLeft.ToString();
    }
    
    // Called when a Cell is left-clicked
    public void ClearCell(Cell cell) {
        // Delegates the action to the CellManager
        cellManager.ClearCell(cell);
    }

    // This method ends the game. The bool parameter is true if the player won and false otherwise
    public void EndGame(bool win) {

        timerStarted = false; // Stop the timer

        // Enable the GameOver panel
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Animation>().Play("GameOver");
        Text text = gameOverPanel.GetComponentInChildren<Text>();

        // Set the message to display based on whether or not the player won
        if (win) {
            text.text = "You Win!";

            if (PlayerPrefs.HasKey(GameRules.Difficulty + "HighScore")) {
                // If the player has an existing high score for this difficulty and has just beaten it, update the high score
                if (timeCount < PlayerPrefs.GetFloat(GameRules.Difficulty + "HighScore")) {
                    SetNewHighScore();
                }
            }
            // If the player has no existing high score for this difficulty, set the high score
            else {
                SetNewHighScore();
            }
        }
        else {
            text.text = "You Lose!";

            source.PlayOneShot(explosionClip);
            cellManager.DisplayAllMines(); // Show the location of all mines that weren't flagged
        }

        gameEnded = true; // Lets other methods know the game has ended
    }

    // Called when a Cell is right-clicked
    public void FlagCell(Cell cell) {
        // Check if this is the first cell or if the cell has been cleared
        if (!timerStarted || cellManager.IsCleared(cell))
            return;

        // If the cell has been flagged, decrement minesLeft
        if (cellManager.FlagCell(cell))
            minesLeft--;
        // Otherwise increment
        else
            minesLeft++;
        
        PlayAudio(flagClip);
    }

    // Called whenever a Cell is cleared
    public void PlayClearAudio() {
        PlayAudio(clearClip);
    }

    // Helper method to play given audio clip
    private void PlayAudio(AudioClip clip) {
        if (!source.isPlaying) // This check prevents the same audio being played multiple times
            source.PlayOneShot(clip);
    }

    // Helper method to update the player's high score
    private void SetNewHighScore() {

        source.PlayOneShot(newHighScoreClip);

        PlayerPrefs.SetFloat(GameRules.Difficulty + "HighScore", timeCount); // Update the high score
        // Enable the NewHighScorePanel
        newHighScorePanel.SetActive(true);
        newHighScorePanel.GetComponent<Animation>().Play("NewHighScore");
    }
}
