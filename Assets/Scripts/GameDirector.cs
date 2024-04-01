using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int hp;
    public PlayerControl player;
    public GameObject[] Stages;

    public Image[] UIhp;
    public Text UIpoint;
    public Text UIstage;
    public GameObject Retry;

    
    
    public void NextStage()
    {
        if (stageIndex < Stages.Length -1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);

            PlayerSetPosition();

            UIstage.text = "STAGE" + (stageIndex + 1);
        }
        else
        {
            Time.timeScale = 0;

            Text Rtry = Retry.GetComponentInChildren<Text>();
            Rtry.text = "Game Clear!!";
            Retry.SetActive(true);
        }
        totalPoint = stagePoint + totalPoint;
        stagePoint = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            hpDown();
            hpDown();
            hpDown();
        }
    }

    public void hpDown()
    {
        if (hp > 1)
        {
            hp--;
            UIhp[hp].color = new Color(0, 0, 0, 0.0f);
        }
        else
        {
            UIhp[0].color = new Color(0, 0, 0, 0.0f);
            player.Die();
            
            Retry.SetActive(true);
        }
    }

    void PlayerSetPosition()
    {
        player.transform.position = new Vector3(-4.6f, -3.5f, 0.0f);
    }

    void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
    }

    public void Restart()
    {
        Retry.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
