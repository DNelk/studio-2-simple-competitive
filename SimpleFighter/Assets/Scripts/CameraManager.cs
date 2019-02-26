using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Moves and resizes as players move
 Credit to https://www.youtube.com/watch?v=oj2_O_ycSfo
 */

public class CameraManager : MonoBehaviour
{
    #region Private Variables
    
    private GameObject[] playerViews;
    private Camera cam;
    
    
    #endregion

    #region Public Variables

    public float minDistance = 2.9f;
    public float maxDistance = 5.6f;
    public float xMin, xMax, yMin, yMax;
    
    #endregion
    
    // Start is called before the first frame update
    private void Start()
    {
        playerViews = GameObject.FindGameObjectsWithTag("PlayerView");
        cam = transform.GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (playerViews.Length == 0) //No Players Detected
            return;
        
        //Set Defaults
        xMin = xMax = playerViews[0].transform.position.x;
        yMin = yMax = playerViews[0].transform.position.y;

        foreach (GameObject go in playerViews)
        {
            if (go.transform.position.x < xMin)
                xMin = go.transform.position.x;
            if (go.transform.position.x > xMax)
                xMax = go.transform.position.x;
            if (go.transform.position.y < yMin)
                yMin = go.transform.position.y;
            if (go.transform.position.y < yMax)
                yMax = go.transform.position.y;
        }

        float xMid = (xMin + xMax) / 2;
        float yMid = (yMin + yMax) / 2;
        float dist = xMax - xMin/2;
        dist = Mathf.Clamp(dist, minDistance, maxDistance);
        transform.position = new Vector3(xMid,yMid, -dist);
    }
}
