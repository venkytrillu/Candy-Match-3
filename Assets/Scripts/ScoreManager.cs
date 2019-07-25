using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public Image scoreBar;
    private int score;
    private GameData gameData;
    private int numberStars;

    private Board board;
    private void Start()
    {
        gameData = GameData.gameData;
        board = GameObject.FindGameObjectWithTag(Tags.Board).GetComponent<Board>();
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;

        for (int i = 0; i < board.scoreGoals.Length; i++)
        {
            if (score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }

        if (gameData!=null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if(score>highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }

            int currentStars = gameData.saveData.stars[board.level];
            if (numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;
            }
            gameData.Save();
        }
        UpdateScore();
    }


    private void UpdateScore()
    {
        if (board != null && scoreBar != null)
        {
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[board.scoreGoals.Length - 1];
            scoreText.text = "" + score;
        }
    }


    public void StartTime()
    {
        EndGameManager.instance.startTime = true;
        AnimationController.instance.PannelGoaltarget_Out();
    }




}//class










































































