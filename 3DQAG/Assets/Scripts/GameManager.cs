using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

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
    public RectTransform bossHpGroup;
    public RectTransform bossHpBar;

    private void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("maxScore"));
    }
    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
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
        bossHpBar.localScale = new Vector3((float)boss.curHp / boss.maxHp, 1, 1);
    }
}
