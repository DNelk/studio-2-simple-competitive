using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundAnnouncer : MonoBehaviour
{
    public Image roundText;
    public Sprite[] roundSprite;

    public int roundNumber = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RoundUpdate();
    }

    void RoundUpdate()
    {
        roundText.sprite = roundSprite[roundNumber - 1];
    }
    
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
