using UnityEngine;

// This class controls the camera in the main scene. It allows the user to move the camera around (within limits)
public class CameraController : MonoBehaviour{

    public float panSpeed; // The speed at which the camera pans
    public float panBorderThickness; // How many pixels in from the edges of the screen denote the pan border
    public float minZ, maxZ; // The limits to zooming in/out
    public float scrollSpeed; // The speed at which the camera zooms in/out

    private float screenHeight, screenWidth; // The height and width of the screen (in pixels)
    private float minX, maxX, minY, maxY; // The minimum/maximum positions of the camera's x and y co-ordinates
    private Vector3 pos; // The position of the camera

    void Start() {

        // Get the height and width of the screen
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        // Get the position of the camera
        pos = transform.position;

        // Set the co-ordinate limits
        minX = pos.x - 80;
        maxX = pos.x + 80;
        minY = pos.y - 80;
        maxY = pos.y + 150; // Extra space at the top so that the UI panels don't interfere with gameplay
    }

    void Update(){

        // Update the position
        pos = transform.position;

        // Move the camera up
        if (pos.y <= maxY && (Input.GetKey("w") || Input.mousePosition.y >= screenHeight - panBorderThickness)) {
            transform.Translate(Vector2.up * panSpeed * Time.deltaTime, Space.World);
        }
        // Move the camera left
        if (pos.x >= minX && (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)){
            transform.Translate(Vector2.left * panSpeed * Time.deltaTime, Space.World);
        }
        // Move the camera down
        if (pos.y >= minY && (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)) {
            transform.Translate(Vector2.down * panSpeed * Time.deltaTime, Space.World);
        }
        // Move the camera right
        if (pos.x <= maxX && (Input.GetKey("d") || Input.mousePosition.x >= screenWidth - panBorderThickness)){
            transform.Translate(Vector2.right * panSpeed * Time.deltaTime, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f){
            // If the user scrolled in or out, update the camera's z co-ordinate
            pos.z += scroll * 1000 * scrollSpeed * Time.deltaTime;
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            transform.position = pos;
        }
    }
}