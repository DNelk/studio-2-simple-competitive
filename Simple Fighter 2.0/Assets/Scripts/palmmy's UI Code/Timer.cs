using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text LeftOne;
    public Text RightOne;
    
    public float timerRaw = 60f;
    public string timeString;

    public Image roundSprite;
    public int roundNumber = 1;
    public Sprite[] roundList;

    public GameObject UIcanvas;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimerUpdate();
        RoundUpdate();

    }

    void TimerUpdate()
    {
        if (timerRaw > 0)
            timerRaw -= Time.deltaTime;

        timeString = Mathf.Ceil(timerRaw).ToString();

        LeftOne.text = timeString[0].ToString();
        RightOne.text = timeString[1].ToString();
    }

    void RoundUpdate()
    {
        if (roundNumber < 5 && Input.GetKeyDown(KeyCode.A))
        {
            timerRaw = 60;
            roundNumber++;
            GameObject newRoundAnnouncer = Instantiate(Resources.Load("Prefabs/PalmmyEffect/RoundAnnouncer"), UIcanvas.transform) as GameObject;
            newRoundAnnouncer.GetComponent<RoundAnnouncer>().roundNumber = roundNumber;
        }

        roundSprite.sprite = roundList[roundNumber - 1];
    }
}
