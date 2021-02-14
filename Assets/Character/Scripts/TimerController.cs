using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public static TimerController instance;

    public Text timeCounter;

    public TimeSpan timePlaying;
    private bool timerGoing = true;

    private float elapsedTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        timeCounter.text = "Time: 00:00.00";
    }

    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = 0f;
        

        StartCoroutine(UpdateTimer());
        
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    public IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            Debug.Log(timePlaying);
           
            timeCounter.text = timePlayingStr;


            yield return null;
            Debug.Log(timerGoing);
        }
        
    }
}
