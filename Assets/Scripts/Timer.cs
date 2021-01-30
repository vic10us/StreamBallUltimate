#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] public float timeBetweenGames = 600;
    [SerializeField] public float timeOfGame = 90;
    [SerializeField] public GameController gameController;
    public float timeRemaining = 65;
    public bool timerIsRunning;
    public TextMeshPro timeText;
    public bool wasGameTime;
    
    void Start()
    {
        timerIsRunning = true;
        wasGameTime = true;
    }

    void Update()
    {
        if (!timerIsRunning) return;
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            timerIsRunning = false;
            timeRemaining = 0;
            if (wasGameTime)
            {
                gameController.TriggerCutscene();
                wasGameTime = false;
            }
            else
            {
                gameController.TriggerDowntime();
                wasGameTime = true;
            }
        }
    }

    public void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        var x = TimeSpan.FromSeconds(timeToDisplay);
        timeText.text = Math.Floor(x.TotalSeconds) > 0 ? 
            x.Minutes > 0 ? 
                $"{x:%m'm '%s's'}" : 
                $"{x:%s}s" : 
                "";
    }

    public void ResetGameTimer()
    {
        timeRemaining = timeOfGame;
        timerIsRunning = true;
    }

    public void ResetDowntimeTimer()
    {
        timeRemaining = timeBetweenGames;
        timerIsRunning = true;
    }
}
