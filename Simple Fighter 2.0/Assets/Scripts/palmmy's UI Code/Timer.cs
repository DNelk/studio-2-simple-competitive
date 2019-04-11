using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text LeftOne;
    public Text RightOne;
    
    public float TimerRaw = 60f;
    public string TimeString;

    public Image RoundSprite;
    public int RoundNumber = 1;
    public Sprite[] RoundList;

    private Canvas canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindWithTag("UI").GetComponent<Canvas>();
        GameObject newRoundAnnouncer = Instantiate(Resources.Load("Prefabs/PalmmyEffect/RoundAnnouncer"), canvas.transform) as GameObject;
        newRoundAnnouncer.GetComponent<RoundAnnouncer>().roundNumber = RoundNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.CurrentManagerState == ManagerState.Fighting)
            TimerUpdate();
    }

    private void TimerUpdate()
    {
        if (TimerRaw > 0)
            TimerRaw -= Time.deltaTime;

        TimeString = Mathf.Ceil(TimerRaw).ToString();

        LeftOne.text = TimeString[0].ToString();
        RightOne.text = TimeString[1].ToString();
    }

    public void RoundUpdate()
    {
        TimerRaw = 60;
        RoundNumber++;
        GameObject newRoundAnnouncer = Instantiate(Resources.Load("Prefabs/PalmmyEffect/RoundAnnouncer"), canvas.transform) as GameObject;
        newRoundAnnouncer.GetComponent<RoundAnnouncer>().roundNumber = RoundNumber;
        RoundSprite.sprite = RoundList[RoundNumber - 1];
    }
    
}
