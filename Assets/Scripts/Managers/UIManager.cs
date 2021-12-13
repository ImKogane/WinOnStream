using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : SingletonMonobehaviour<UIManager>
{
    [Header("UI Canvas references")]
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject pauseScreen;
    
    [Header("Other")]
    [SerializeField] private Slider timerBar;

    [SerializeField] private TMP_Text phaseTitle;
    [SerializeField] private TMP_Text phaseDescription;

    [SerializeField] private float turnDescriptionDuration;

    [SerializeField] private TMP_Text turnCount;

    [SerializeField] private float textSpeed;
    [SerializeField] private float waitingDurationAfterPhaseDescription;

    public RectTransform choiceScreen;
    public List<Image> choiceImagesList;

    public UI_Lobby uiLobby;
    

    public override bool DestroyOnLoad => true;

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
        endScreen.SetActive(value); 
    }

    public void DisplayGameScreen(bool value)
    {
        gameScreen.SetActive(value);
    }
    
    public void DisplayPauseScreen(bool value)
    {
        pauseScreen.SetActive(value);
    }

    public void DisplayAllPlayersUI(List<Player> playerList, bool value)
    {
        foreach (var player in playerList)
        {
            player.playerCanvas.enabled = value;
        }
    }

    public void DisplayPlayerUI(Player player, bool value)
    {
        player.playerCanvas.enabled = value;
    }
    
    public void UpdateTurnCount(int newCount)
    {
        turnCount.enabled = true;
        turnCount.text = newCount.ToString();
    }
    
    public void DisplayChoiceScreen(bool value)
    {
        choiceScreen.gameObject.SetActive(value);
    }
    

    void DestroyChoicesImages()
    {
        foreach (var choiceImage in choiceImagesList)
        {
            Destroy(choiceImage.gameObject);
        }
        
        choiceImagesList.Clear();
        
    }
    
    public void UpdateChoiceCardsImage()
    {
        DestroyChoicesImages();

        int currentIndexChoice = ScriptableManager.Instance.GetChoiceIndexCompteur();

        if (ScriptableManager.Instance._turnChoiceList.Count <= currentIndexChoice)
        {
            return;
        }
        
        for(int i = 0; i < ScriptableManager.Instance._turnChoiceList[currentIndexChoice].choiceList.Count; i++)
        {
            Image newChoiceImage = new GameObject().AddComponent<Image>();

            newChoiceImage.transform.parent = choiceScreen;
            newChoiceImage.sprite = ScriptableManager.Instance._turnChoiceList[currentIndexChoice].choiceList[i]._cardSprite;
            //newChoiceImage.SetNativeSize();
            newChoiceImage.preserveAspect = true;

            choiceImagesList.Add(newChoiceImage);
        }

    }

    public void EnablePlayButton()
    {
        uiLobby.EnablePlayButton();
    }
    
    public void EndGameUI()
    {
        GetComponent<UI_WinScreen>().MainCameraEnabled(false);
        GetComponent<UI_WinScreen>().WinCameraEnabled(true);
        DisplayEndScreen(true);
        DisplayGameScreen(false);
        GetComponent<UI_WinScreen>().SetPlayerNameText(PlayerManager.Instance.GetLastPlayer().GetPlayerName());
    }
    
}
