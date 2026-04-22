using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxScroller : MonoBehaviour
{
    Transform cam; // store the camera's transform (position, rotation, scale).
    Vector3 camStartPos; // store the camera's initial position when the game starts.
    float distance; // store the distance the camera has moved horizontally from its starting position

    GameObject[] backgrounds; // array of GameObjects that will store all the background objects that need to have the parallax effect.
    Material[] mat; // store the materials associated with each background object. 
    // Materials are used to control the appearance of objects in Unity
    
    float[] backSpeed; // array of floats that will hold the relative speeds at which each background should move

    float farthestBack; // store the distance of the farthest background object from the camera. 
    // This is used to determine how fast each background should move relative to the camera

    // controls the overall speed of the parallax effect.
    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        // assign the main camera's transform (Camera.main.transform) to the cam variable.
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount; // stores the number of child objects (background elements) under the current game object
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        // iterates through each child object
        for (int i = 0; i < backCount; i++) 
        {
            backgrounds[i] = transform.GetChild(i).gameObject; // assigns each child object to the backgrounds array.
            mat[i] = backgrounds[i].GetComponent<Renderer>().material; // gets the Material component from the Renderer of each background object and stores it in the mat array
        }

        BackSpeedCalculate(backCount);
    }

    // Calculates and assigns speeds (backSpeed) to each background based on their distance from the camera.
    void BackSpeedCalculate(int backCount) 
    {
        /*This loop iterates through all background objects 
         * to find the one that is farthest from the camera (farthestBack). 
         * It calculates the distance between the camera and each background and updates farthestBack 
         * if a greater distance is found.*/
        for (int i = 0; i < backCount; i++) //find the farthest background
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        /* This second loop calculates the speed for each background relative to the farthest background. 
         * The speed is normalized between 0 and 1, where closer backgrounds will move faster, 
         * and farther ones will move slower.*/
        for (int i = 0; i < backCount; i++) //set the speed of background
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    // Updates the position of the parent object (transform.position) to follow the camera horizontally.
    private void LateUpdate()
    {
        distance = cam.position.x - camStartPos.x; // calculated as the horizontal difference between the camera's current position and its starting position.
        transform.position = new Vector3(cam.position.x, transform.position.y, 0); // updated to follow the camera's horizontal movement, but keeps its original vertical position

        // This loop iterates through each background object.
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            Vector2 offset = new Vector2(distance, 0) * speed; // Apply parallax effect horizontally

            // For the back two backgrounds, also adjust their vertical position to match the camera's y position
            if (i < backgrounds.Length - 2) // Assuming the back two backgrounds are the last two in the array
            {
                backgrounds[i].transform.position = new Vector3(backgrounds[i].transform.position.x, cam.position.y, backgrounds[i].transform.position.z);
            }

            mat[i].SetTextureOffset("_MainTex", offset);
            // shifts the texture of each background along the x-axis
            // based on the calculated speed and the camera's movement (distance).
            // This is what creates the parallax scrolling effect.
        }
    }

}