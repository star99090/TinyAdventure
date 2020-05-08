using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhp;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int hp;

    //점수는 Update()
    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        if(stageIndex < Stages.Length -1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            //일시정지
            Time.timeScale = 0;
            //버튼 내용 수정
            Text btnText = RestartButton.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            //버튼 불러오기
            RestartButton.SetActive(true);

        }

        totalPoint += stagePoint;
        stagePoint = 0;

    }
    public void hpDown()
    {
        if (hp > 1)
        {
            hp--;
            UIhp[hp].color = new Color(1, 1, 1, 0.2f);
            player.PlaySound("Damaged");
        }

        else
        {
            UIhp[0].color = new Color(1, 1, 1, 0.2f);
            player.OnDie();
            RestartButton.SetActive(true);
            player.PlaySound("Die");
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (hp > 1)
                PlayerReposition();
            hpDown();
            
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-2, 0, 0);
        player.velocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
