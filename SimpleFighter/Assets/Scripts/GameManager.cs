using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Text fpsText;
    public float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        //cap the frame rate at 60fps
        Application.targetFrameRate = 60;
        
    }

    // Update is called once per frame
    void Update()
    {


		//DEBUG TOOLS

        //fps display here
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "fps " + Mathf.Floor(fps).ToString();

        //PAUSE & UNPAUSE
        if (Input.GetButtonDown("Fire2")) // temporarily set this to right click. Need to fix it to use Rewired.
        {
            if (Time.timeScale == 1.0f)
                Time.timeScale = 0.0f;
            else
                Time.timeScale = 1.0f;
            // Adjust fixed delta time according to timescale
            // The fixed delta time will now be 0.02 frames per real-time second
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }



    }
}
