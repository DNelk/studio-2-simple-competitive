using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    private int capNum = 0;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot("capture" + capNum);
            capNum++;
        }
    }
}
