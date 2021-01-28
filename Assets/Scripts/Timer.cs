#pragma warning disable 649

using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 65;
    public bool timerIsRunning = false;
    public TextMeshPro timeText;
    
    [SerializeField] float timeBetweenGames = 600;
    [SerializeField] float timeOfGame = 90;
    [SerializeField] GameController gameController;

    public bool wasGameTime;
    
    void Start()
    {
        timerIsRunning = true;
        wasGameTime = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
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
