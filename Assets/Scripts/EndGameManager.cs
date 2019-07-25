using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Timer
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour {

    public static EndGameManager instance;
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private Board board;
    private float timerSeconds;
    [HideInInspector]
    public bool startTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start () {
        board = FindObjectOfType<Board>();
       SetGameType();
        SetupGame();
	}

    void SetGameType()
    {
        if (board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
        }
    }

    void SetupGame()
    {
        
        currentCounterValue = requirements.counterValue;
        if(requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }else{
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.playState != PlayState.Pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                StartCoroutine(LossCo());
            }
        }
    }

    IEnumerator LossCo()
    {
        yield return new WaitForSeconds(1f);
        LoseGame();
        //print("You Lose!");
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.playState = PlayState.Win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        AnimationController.instance.PannelWin_In();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.playState = PlayState.Lose;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        AnimationController.instance.PannelTryAgain_In();
    }

    //// Update is called once per frame
    void Update()
    {
        if(startTime)
        StartTimer();
    }


    void StartTimer()
    {
        if (requirements.gameType == GameType.Timer && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

    
}// class
































