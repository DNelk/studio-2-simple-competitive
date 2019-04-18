using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Moves and resizes as players move
 Credit to https://www.youtube.com/watch?v=oj2_O_ycSfo
 */

public class CameraManager : MonoBehaviour
{
    #region Private Variables
    
    private List<GameObject> playerViews = new List<GameObject>();
    private Camera cam;
    private float orthoDefault;
    private Vector3 defaultPos;
    #endregion

    #region Public Variables

    public static CameraManager Instance = null;
    public float minDistance = 4.0f;
    public float maxDistance = 5.6f;
    public float xMin, xMax, yMin, yMax;
    
    #endregion

    private void Awake()
    {
        //Set up the singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        cam = transform.GetComponent<Camera>();
        orthoDefault = cam.orthographicSize;
        defaultPos = transform.position;
    }

    private void LateUpdate()
    {
        //Don't Track if we're not fighting
        if (GameManager.Instance.CurrentManagerState != ManagerState.Fighting)
        {
            cam.orthographicSize = orthoDefault;
            transform.position = defaultPos;
            return;
        }

        if (playerViews.Count == 0) //No Players Detected
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
        transform.position = new Vector3(xMid, defaultPos.y, defaultPos.z);
        cam.orthographicSize = orthoDefault * (dist / maxDistance);
    }

    public void AddView(GameObject playerView)
    {
        playerViews.Add(playerView);
    }

    public void ClearViews()
    {
        playerViews.Clear();
    }
    
}
