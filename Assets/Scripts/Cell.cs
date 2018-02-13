using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// This object represents a single cell in the game
// Implements IPointerClickHandler in order to override the OnPointerClick method
public class Cell : MonoBehaviour, IPointerClickHandler {

	public bool isMine; // Does this cell contain a mine?
	public int rowIndex, colIndex; // These indices denote the cell's location in the grid
    public GameManager gameManager; // Reference to the GameManager object
    public Sprite image; // The image displayed on this cell

	void Start(){

		isMine = false; // Each cell is initially not a mine

        // Set the reference to GameManager
        gameManager = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameManager>();
	}

    // Used to set the initial values for the cell's data
    public void Init(int rowIndex, int colIndex, Sprite image){

		this.rowIndex = rowIndex;
		this.colIndex = colIndex;
        this.image = image;
	}

    // This method is run whenever the cell is clicked
	public void OnPointerClick(PointerEventData eventData){

        // All action is disabled when the game is over
		if (gameManager.gameEnded)
			return;
        
		if (eventData.button == PointerEventData.InputButton.Left)
            // If left-clicked, clear the cell
			gameManager.ClearCell (this);
		else if (eventData.button == PointerEventData.InputButton.Right)
            // If right-clicked, flag the cell
			gameManager.FlagCell(this);
	}

    // Sets the Cell's image
    public void SetImage(Sprite sprite) {

        image = sprite;
        this.GetComponent<Image>().sprite = sprite;
    }

    // Sets this cell as a mine
    public void SetMine() { this.isMine = true; }
}
