using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 处理菜单中皮肤、难度选这的点击事件
/// </summary>
public class UIManger : MonoBehaviour
{
    public GameObject panel;
    public GameObject buttons;
    private bool panelOpen = false;
    [SerializeField]
    Transform Wall;
    public Vector3 SpawnPosistion;
    public List<GameObject> ballPreb;
    public GameObject[] tables;
    public Text currency;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.SetInt("coinNum", 35000);
        if (!PlayerPrefs.HasKey("First"))
        {
            PlayerPrefs.SetInt("coinNum", 350);
            PlayerPrefs.SetFloat("First", 1);
            PlayerPrefs.SetInt("Skin", 0);
        }

        currency.text = "Currency: " + PlayerPrefs.GetInt("coinNum").ToString();

        if (PlayerPrefs.GetInt("Skin") != 0)
        {
            if (SceneManager.GetActiveScene().name.Equals("menu1"))
                ChangeTheme();
        }
    }
    public void ChangeWallColor(string name)
    {
        Color color = new Color();
        char temp = name[0];
        switch (temp)
        {
            case '0':
                color = Color.white;
                break;
            case '1':
                color = new Color(1, 0.8313f, 0.6196f);
                break;
            case '2':
                color = new Color(0.7725f, 1, 0.6196f);
                break;
            case '3':
                color = new Color(0.6196f, 0.8431f, 1);
                break;
            case '4':
                color = new Color(1, 0.8901f, 0);
                break;
            case '5':
                color = new Color(0.0823f, 0.2509f, 0.4196f);
                break;
            default:
                break;
        }
        for (int i = 0; i < 2; i++)
        {
            Wall.GetChild(i).GetComponent<SpriteRenderer>().color = color;
        }
    }
    public void ChangeTheme()
    {
        //Change Ball
        GameObject ball = GameObject.FindGameObjectWithTag("Player");
        SpawnPosistion = new Vector3(ball.transform.position.x, 202.5f, 0);
        Destroy(ball);
        GameObject a = Instantiate(ballPreb[PlayerPrefs.GetInt("Skin")], SpawnPosistion, Quaternion.identity) as GameObject;
        a.AddComponent<BallJumpAnimation>();

        //Change Theme
        GameObject table = GameObject.FindGameObjectWithTag("Table");
        Vector3 TablePosition = table.transform.position;
        Destroy(table);
        Instantiate(tables[PlayerPrefs.GetInt("Skin")], TablePosition, Quaternion.identity);
        //b.AddComponent<Rigidbody2D> ();

        //Change Wall
        ChangeWallColor(a.name);

    }
    public void OnSlowBtnClik()
    {
        PlayerPrefs.SetFloat("speedy", 10f);
        PlayerPrefs.SetFloat("level", 1);
        SceneManager.LoadScene("GravityBall");
    }
    public void OnMediumwBtnClik()
    {
        PlayerPrefs.SetFloat("speedy", 15f);
        PlayerPrefs.SetFloat("level", 1.4f);
        SceneManager.LoadScene("GravityBall");
    }
    public void OnFastBtnClik()
    {
        PlayerPrefs.SetFloat("speedy", 20f);
        PlayerPrefs.SetFloat("level", 1.95f);
        SceneManager.LoadScene("GravityBall");
    }
    public void ChooseBall()
    {
        //获取点击对象的index值
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        int index = int.Parse(buttonName);

        //记录选择
        PlayerPrefs.SetInt("Skin", index);

        //替换当前球
        if (SceneManager.GetActiveScene().name.Equals("menu1"))
        {
            ChangeTheme();
            SoundManager sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            AudioClip clip = sm.musicGame[PlayerPrefs.GetInt("Skin")];
            sm.music.clip = clip;
            sm.music.Play();
        }

        panel.SetActive(false);
        panelOpen = !panelOpen;
        buttons.SetActive(true);
    }
    public void BackToStart()
    {
        SceneManager.LoadScene("Start");
    }
    public void LeaveGame()
    {
        Application.Quit();
    }
    public void ShowMarket()
    {
        panelOpen = !panelOpen;
        panel.SetActive(panelOpen);
        buttons.SetActive(false);
        currency.color = Color.black;
        if (!panelOpen)
            buttons.SetActive(true);
    }
    public void UnLock()
    {
        //解锁前,先比较钱够不够
        int cost = int.Parse(EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);

        if (PlayerPrefs.GetInt("coinNum") < cost)
        {
            currency.color = Color.red;
        }
        else
        {
            currency.color = Color.black;

            //给购买的球加上解锁标志
            int unlockedBall = int.Parse(EventSystem.current.currentSelectedGameObject.name) - 1;
            //记录解锁标志
            PlayerPrefs.SetInt(unlockedBall.ToString(), unlockedBall);
            Destroy(EventSystem.current.currentSelectedGameObject);

            //扣钱
            PlayerPrefs.SetInt("coinNum", PlayerPrefs.GetInt("coinNum") - cost);
            currency.text = "Currency: " + PlayerPrefs.GetInt("coinNum").ToString();
        }
    }
}
