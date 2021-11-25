using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Canvas endScreen;
    [SerializeField] private Canvas gameScreen;
    
    [SerializeField] private Slider timerBar;

    [SerializeField] private TMP_Text phaseTitle;
    [SerializeField] private TMP_Text phaseDescription;

    [SerializeField] private float turnDescriptionDuration;

    [SerializeField] private TMP_Text turnCount;

    [SerializeField] private float textSpeed;
    [SerializeField] private float waitingDurationAfterPhaseDescription;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void UpdateTimerBar(float value)
    {
        timerBar.value = Mathf.Clamp(value, timerBar.minValue, timerBar.maxValue);
    }

    public void ActivateTimerBar(bool value)
    {
        timerBar.enabled = value;
    }

    public void DisplayPhaseTitle(string turnName)
    {
        phaseTitle.text = turnName;
    }

    public void DisplayPhaseDescription(string newTurnDescription)
    {
        StartCoroutine(DisplayPhaseDescriptionCoroutine(newTurnDescription));
    }

    IEnumerator DisplayPhaseDescriptionCoroutine(string newTurnDescription)
    {
        phaseDescription.enabled = true;
        phaseDescription.text = newTurnDescription;

        for (int i = 0; i <= phaseDescription.text.Length; i++)
        {
            phaseDescription.maxVisibleCharacters = i;
            yield return new WaitForSeconds(1 / textSpeed);
        }

        yield return new WaitForSeconds(waitingDurationAfterPhaseDescription);
        
        phaseDescription.enabled = false;
    }

    public void DisplayEndScreen(bool value)
    {
        endScreen.enabled = value;
    }

    public void DisplayGameScreen(bool value)
    {
        gameScreen.enabled = value;
    }
    
    public void UpdateTurnCount(int newCount)
    {
        turnCount.enabled = true;
        turnCount.text = "Tour " + newCount.ToString();
    }
    
}
