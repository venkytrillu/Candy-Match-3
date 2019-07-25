using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public GameObject homePanel,levelSelect;
    public GameObject PlayBtn,musicBtn;
    public Image ImageSprite;

    private void Start()
    {
        //AudioController.instacne.sprite = musicBtn.GetComponent<Image>();
        PlayBtn_Ani();
    }
    private void CloseLevelSelect()
    {
        levelSelect.SetActive(false);
       
    }
    private void OpenLevelSelect()
    {
        levelSelect.SetActive(true);
    }

    private void CloseHomePanel()
    {
        homePanel.SetActive(false);
    }
    private void OpenHomePanel()
    {
        homePanel.SetActive(true);
    }

    public void LevelSelect()
    {
        CloseHomePanel();
        OpenLevelSelect();
    }

    public void HomePanel()
    {
        OpenHomePanel();
        CloseLevelSelect();
    }

    public void PlayBtn_Ani()
    {
        iTween.ScaleFrom(PlayBtn, iTween.Hash("scale",
            new Vector3(0.9f, 0.9f, 0.9f), "islocal", true, "delay", 0f, "time", 0.5f,
                       "easetype", iTween.EaseType.easeInSine, "looptype", iTween.LoopType.pingPong));
    }

    public void MusicOnOff()
    {
        AudioController.instacne.MusicOnOff(ImageSprite);
    }

}//class



























