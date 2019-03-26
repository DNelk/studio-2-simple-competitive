using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    
    #region Private Variables
    //Current hitpoints
    private int currentHitPoints;
    
    #endregion
    
    #region Public Variables
    
    //Character Statistics
    [Range(1, 5)] public int MaxHitPoints;
    [Range(1, 50)] public float MoveSpeed;
    [Range(1, 10)] public int StrikeStartupFrames;
    [Range(1, 10)] public int StrikeActiveFrames;
    [Range(1, 10)] public int StrikeRecoveryFrames;
    [Range(1, 10)] public int GrabStartupFrames;
    [Range(1, 10)] public int GrabActiveFrames;
    [Range(1, 10)] public int GrabRecoveryFrames;
    [Range(1, 10)] public int BlockStartupFrames;
    [Range(1, 10)] public int BlockRecoveryFrames;
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
