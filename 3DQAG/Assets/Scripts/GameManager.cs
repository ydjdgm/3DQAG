using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public Player player;
    public Boss boss;
    public GameObject shop;
    public GameObject startZone;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] spawnZone;
    public GameObject[] enemy;
    public List<int> enemyList;

    public GameObject menuPanel;
    public Text maxScoreTxt;
    public GameObject gamePanel;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHp;
    public Text playerAmmo;
    public Text playerCoin;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public GameObject gameOverPanel;
    public Text curScore;
    public Text bestText;

    public RectTransform bossHpGroup;
    public RectTransform bossHpBar;

    private void Awake()
    {
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("maxScore"));
        if (!PlayerPrefs.HasKey("maxScore"))
        {
            PlayerPrefs.SetInt("maxScore", 0);
        }
    }
    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
    public void StageStart()
    {
        shop.SetActive(false);
        startZone.SetActive(false);
        foreach (Transform i in spawnZone)
        {
            i.gameObject.SetActive(true);
        }
        isBattle = true;
        StartCoroutine(InBattle());
    }
    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        curScore.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("maxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("maxScore", player.score);
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void StageEnd()
    {
        shop.SetActive(true);
        startZone.SetActive(true);
        foreach (Transform i in spawnZone)
        {
            i.gameObject.SetActive(false);
        }
        isBattle = false;
        stage++;
        player.transform.position = Vector3.up * 1;
    }
    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemy[3], spawnZone[2].position, spawnZone[2].rotation);
            Enemy enemyScript = instantEnemy.GetComponent<Enemy>();
            enemyScript.target = player.transform;
            enemyScript.gameManager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int i = 0; i < stage; i++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
        }
        while(enemyList.Count > 0)
        {
            int ranZone = Random.Range(0, 4);
            GameObject instantEnemy = Instantiate(enemy[enemyList[0]], spawnZone[ranZone].position, spawnZone[ranZone].rotation);
            Enemy enemyScript = instantEnemy.GetComponent<Enemy>();
            enemyScript.target = player.transform;
            enemyScript.gameManager = this;
            enemyList.RemoveAt(0);
            yield return new WaitForSeconds(4f);
        }
        while(enemyCntA + enemyCntB +  enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        boss = null;
        StageEnd();
    }
    private void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }
    private void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "Stage" + stage;

        //플탐 ui
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        playerHp.text = player.health + " / " + player.maxHealth;
        playerCoin.text = string.Format("{0:n0}", player.coin);

        //탄약 수 ui
        if (player.equipWeapon == null || player.equipWeapon.type == Weapon.Type.Melee)
        {
            playerAmmo.text = "- / " + player.ammo;
        }
        else
        {
            playerAmmo.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        }

        //무기 ui 투명도
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //남은 몬스터 수
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        //보스체력바
        if(boss != null)
        {
            bossHpGroup.anchoredPosition = Vector3.down * 30;
            bossHpBar.localScale = new Vector3((float)boss.curHp / boss.maxHp, 1, 1);
        }
        else
        {
            bossHpGroup.anchoredPosition = Vector3.up * 200;
        }

    }
}
