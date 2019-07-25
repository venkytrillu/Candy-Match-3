using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    public static AnimationController instance;
    public GameObject PannelGoaltarget,PanelWin,PanelTryAgain,PausePanel;
    public GameObject outerPannel, outerWinPannel, OuterTryAgainPanel;
    public Image ImageSprite;
    private AudioSource audioSource;
    private Animator anim;

    Board board;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        board = GameObject.FindGameObjectWithTag(Tags.Board).GetComponent<Board>();
        
        PannelGoaltarget_In();
        StartCoroutine(GameStartCo());

    }
    public void PannelGoaltarget_In()
    {
       
        iTween.MoveTo(PannelGoaltarget, iTween.Hash("position",
            new Vector3(0, 0, 0),"islocal",true, "delay", 2f, "time", 0.5f));
    }
    public void PannelGoaltarget_Out()
    {
        iTween.MoveTo(PannelGoaltarget, iTween.Hash("position",
            new Vector3(0, 800f, 0), "islocal", true, "time", 0.5f));
        anim = outerPannel.GetComponent<Animator>();
        anim.SetTrigger("WhiteEffect");
    }
    public void PannelWin_Out()
    {
        outerWinPannel.SetActive(true);
        iTween.MoveTo(PanelWin, iTween.Hash("position",
            new Vector3(0, 800f,0), "islocal", true, "time", 0.5f));
        anim = outerPannel.GetComponent<Animator>();
        anim.SetTrigger("WhiteEffect");
    }

   public void PannelWin_In()
    {
        outerWinPannel.SetActive(true);
        iTween.MoveTo(PanelWin, iTween.Hash("position",
            new Vector3(0, 0, 0), "islocal", true, "delay",2f, "time", 0.5f));
    }


    public void PannelTryAgain_In()
    {
        OuterTryAgainPanel.SetActive(true);
        iTween.MoveTo(PanelTryAgain, iTween.Hash("position",
            new Vector3(0, 0, 0), "islocal", true, "delay", 2f, "time", 0.5f));
    }
    public void PannelTryAgain_Out()
    {
        
        iTween.MoveTo(PanelTryAgain, iTween.Hash("position",
            new Vector3(0, 800f, 0), "islocal", true, "time", 0.5f));
        anim = outerPannel.GetComponent<Animator>();
        anim.SetTrigger("WhiteEffect");
        
    }

    public void PannelPause_In()
    {

        iTween.MoveTo(PausePanel, iTween.Hash("position",
            new Vector3(0, 0, 0), "islocal", true, "delay",0f, "time", 0.5f));
    }

    public void PannelPause_Out()
    {

        iTween.MoveTo(PausePanel, iTween.Hash("position",
            new Vector3(0, 800f, 0), "islocal", true, "time", 0.5f));
       
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);
        board.playState = PlayState.Move;
    }

   public void MusicOnOff()
    {
        AudioController.instacne.MusicOnOff(ImageSprite);
    }


}//class
























