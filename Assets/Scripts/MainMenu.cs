using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

// This class manages the display of the main menu
public class MainMenu : MonoBehaviour {

    public GameObject mainPanel, playPanel, highScorePanel;
    public AudioClip mainClip, secondaryClip; // Two audio clips; one is for yellow buttons, the other for red
    public Text easyScore, mediumScore, hardScore;

    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    void Awake() {
        // This object stays alive when starting a game so the button click audio doesn't get cut
        DontDestroyOnLoad(this.gameObject);
        gameObject.AddComponent<AudioSource>();
    }

    // This method is called from either of the sub-menus. The panel from which it is called is passed as an argument
    public void BackToMainMenu(GameObject panel) {

        // Disable the current panel, enable the main panel
        panel.SetActive(false);
        mainPanel.SetActive(true);

        source.PlayOneShot(secondaryClip);
    }

    // Displays the "Play" panel, where the user selects a difficulty
    public void DisplayPlayPanel() {

        // Disable the main panel, show the play panel
        mainPanel.SetActive(false);
        playPanel.SetActive(true);

        source.PlayOneShot(mainClip);
    }

    // This method displays all high scores on the high score panel
    public void DisplayHighScorePanel() {

        // Disable the main panel, enable the high score panel
        mainPanel.SetActive(false);
        highScorePanel.SetActive(true);

        // Display all high scores
        DisplayHighScore("Easy", easyScore);
        DisplayHighScore("Medium", mediumScore);
        DisplayHighScore("Hard", hardScore);

        source.PlayOneShot(mainClip);
    }

    // Exits the game
    public void ExitGame() {
        
        source.PlayOneShot(secondaryClip);
        Application.Quit();
    }

    // This method loads the main scene with the easy game settings
    public void LoadEasyMode() {
        LoadSceneWithRules(9, 9, 10, "Easy");
    }

    // This method loads the main scene with the hard game settings
    public void LoadHardMode() {
        LoadSceneWithRules(30, 16, 99, "Hard");
    }

    // This method loads the main scene with the medium game settings
    public void LoadMediumMode() {
        LoadSceneWithRules(16, 16, 40, "Medium");
    }

    // Helper method to display the high score of a given difficulty on the given Text object
    private void DisplayHighScore(string difficulty, Text text) {

        // If there is an entry for the high score, display it
        if (PlayerPrefs.HasKey(difficulty + "HighScore"))
            text.text = string.Format("{0:00.00}", PlayerPrefs.GetFloat(difficulty + "HighScore"));
        // otherwise, substitute the score with N/A
        else
            text.text = "N/A";
    }

    // Helper method to load the main scene with the given game rules
    private void LoadSceneWithRules(int cols, int rows, int mines, string difficulty) {

        GameRules.SetRules(cols, rows, mines, difficulty); // Store the rules and difficulty in the GameRules class
        SceneManager.LoadScene("MainScene"); // Load the main scene

        source.PlayOneShot(mainClip);
        StartCoroutine(WaitForAudio()); // Wait for the audio to finish playing
    }

    // Helper method to wait for the audio to finish playing
    IEnumerator WaitForAudio(){
        // After 1 second, destroy this object
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}