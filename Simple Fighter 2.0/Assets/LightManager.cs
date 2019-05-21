using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance = null;

    private Light mainSpot;
    public Light[] PlayerSpots;

    private Color[] lightColorDefaults;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        Init();
    }

    private void Init()
    {
        mainSpot = GameObject.FindWithTag("StatusLight").GetComponent<Light>();
        lightColorDefaults = new Color[PlayerSpots.Length];
        for(int i = 0; i < PlayerSpots.Length; i++)
        {
            lightColorDefaults[i] = PlayerSpots[i].color;
        }   
    }
    
    //Changes the color of the big spotlight
    public void ChangeLightColor(Color color, float time, bool flash = false, int playerIndex = -1 )
    {
        if(GameManager.Instance.CurrentManagerState != ManagerState.Fighting)
            return;

        Light currentLight;
        Color defaultCol;
        
        if (playerIndex == -1)
        {
            currentLight = mainSpot;
            defaultCol = Color.white;
        }
        else
        {
            currentLight = PlayerSpots[playerIndex];
            defaultCol = lightColorDefaults[playerIndex];
        }
        
        Tween colorChange = currentLight.DOColor(color, time);
        if (flash)
        {
            colorChange.OnComplete(() => currentLight.DOColor(defaultCol, time));
        }
    }
    
    public Light MainSpot
    {
        get { return mainSpot; }
    }

    public void Reset()
    {
        mainSpot.color = Color.white;
        for(int i = 0; i < PlayerSpots.Length; i++)
        {
            PlayerSpots[i].color = lightColorDefaults[i];
        }  
    }
    
}
