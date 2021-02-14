using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;               // Included to modify Unity UI components
using UnityEngine.SceneManagement;  // Included to load into different scenes

public class GameController : MonoBehaviour
{
    public GameController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TimerController.instance.timeCounter.text = "Time: 00:00.00";
        TimerController.instance.BeginTimer();
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}
