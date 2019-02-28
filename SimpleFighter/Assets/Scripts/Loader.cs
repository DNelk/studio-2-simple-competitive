using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    //Loads the Game Manager when the project/scene starts to ensure that there is always a game manager

    public GameManager gameManager; //prefab to instantiate
    
    // Start is called before the first frame update
    void Awake()
    {
        //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
        if (GameManager.instance == null)
                
            //Instantiate gameManager prefab
            Instantiate(gameManager);
    }
}
