using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// This script controls the buttons in the main scene
public class ButtonController : MonoBehaviour {

    public AudioClip clip;

    private string mainMenuSceneName = "MainMenu"; // The name of the main menu scene
    private string mainSceneName = "MainScene"; // The name of the main game scene
    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    void Awake() {
        // This object stays alive when reverting to the main menu so the button click audio doesn't get cut
        DontDestroyOnLoad(this.gameObject);
        gameObject.AddComponent<AudioSource>();
    }

    // Called when the "Menu" button is clicked
    public void ClickMenu() {
        LoadScene(mainMenuSceneName);
    }

    // Called when the "Retry" button is clicked
    public void ClickRetry(){
        LoadScene(mainSceneName);
    }

    // Helper method to load a scene
    private void LoadScene(string scene) {

        source.PlayOneShot(clip);
        StartCoroutine(WaitForAudio()); // Wait for the audio to finish playing

        SceneManager.LoadScene(scene); // Load the scene
    }

    // Helper method to wait for the audio to finish playing
    IEnumerator WaitForAudio(){
        // After 1 second, destroy this object
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
