using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarUIText : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public CarAI carScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timerText.text = "Elapsed: "+carScript.timeElapsed.ToString("0.00");
    }
}
