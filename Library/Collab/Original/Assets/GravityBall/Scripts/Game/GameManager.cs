﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif


/// <summary>
/// Class in charge of the pooling system (to spawn obstacle prefabs), trigger start and finished event, and display points / best score / button start when nedded
/// 
/// This script is attached to the GameObject "GameManager".
/// This script in in charge of the game logic. And to spawn the obstacles.
/// </summary>
public class GameManager : MonoBehaviorHelper
{


    public int numberOfPlayToShowInterstitial = 5;


    public GameObject popUpContinuePrefab;

    public delegate void GameStart();
    /// <summary>
    /// Delegate stored function subscribe by script who want to know when the game is started
    /// </summary>
    public static event GameStart OnGameStarted;

    public delegate void GameEnd();
    /// <summary>
    /// Delegate stored function subscribe by script who want to know when the game is ended
    /// </summary>
    public static event GameEnd OnGameEnded;

    /// <summary>
    /// Obstacle "rectangle" prefabs
    /// </summary>
    public Transform obstacleRectanglePrefab;
    /// <summary>
    /// Obstacle "square" prefabs
    /// </summary>
    public Transform obstacleCarrePrefab;

    // Coin
    public Transform coinPrefab;

    /// <summary>
    /// List of the obstacle rectangle
    /// </summary>
    public List<Transform> obstacleRectanglePrefabList = new List<Transform>();
    /// <summary>
    /// List of the obstacle square
    /// </summary>
    public List<Transform> obstacleCarrePrefabList = new List<Transform>();

    public List<Transform> coinsList = new List<Transform>();

    /// <summary>
    /// Particle to emit when the player starts
    /// </summary>
    public ParticleEmitter particleExplosionStart;
    /// <summary>
    /// Particle to emit when the player touches the left wall
    /// </summary>
    public ParticleEmitter particleExplosionWallLeftPrefab;
    /// <summary>
    /// Particle to emit when the player touches the right wall
    /// </summary>
    public ParticleEmitter particleExplosionWallRightPrefab;


    /// <summary>
    /// Reference to the point UI Text 
    /// </summary>
    public Text pointText;
    /// <summary>
    /// Reference to the best score UI Text 
    /// </summary>
    public Text bestScoreText;
    public Text coinsText;
    /// <summary>
    /// Reference to the title UI Text
    /// </summary>
    public Text title;
    /// <summary>
    /// Reference to the start UI Button
    /// </summary>
    public Button buttonStart;
    /// <summary>
    /// The current player score = number of jumps
    /// </summary>
    private int point;
    /// <summary>
    /// To get the current score of the player
    /// </summary>
    public int GetPoint()
    {
        return point;
    }
    /// <summary>
    /// The total life the player have
    /// </summary>
    private int coinNum;
    /// <summary>
    /// To get the total coin the player have. 
    /// </summary>
    public int GetCoinNum
    {
        get
        {
            return PlayerPrefs.GetInt("Coins");
        }
        set {
            coinNum = value;
        }
    }

    private void Awake()
    {
        if (Time.realtimeSinceStartup < 5)
        {
            LeaderboardManager.Init();
        }

        if (!PlayerPrefs.HasKey("Coins"))
        {
            coinNum = 0;
            PlayerPrefs.SetInt("Coins", 0);
        }

        pointText.gameObject.SetActive(false);

        title.gameObject.SetActive(true);

        int best = ScoreManager.GetBestScore();

        bestScoreText.text = "Best: " + best;
        coinsText.text = "Coins: " + coinNum;


        pointText.text = "0";

        ActivateButtonStart();

        CreateListRectangle(20);
        CreateListCarre(20);
        CreateCoinsList(5);
    }


    /// <summary>
    /// To activate button start
    /// </summary>
    void ActivateButtonStart()
    {

        buttonStart.onClick.RemoveAllListeners();
        buttonStart.onClick.AddListener(OnStart);
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        GC.Collect();
    }

    void CreateListRectangle(int i)
    {
        int count = 0;

        while (count < i)
        {
            _CreateListRectangle();
            count++;
        }
    }

    Transform _CreateListRectangle()
    {
        var ob = Instantiate(obstacleRectanglePrefab) as Transform;
        ob.SetParent(transform, false);
        ob.gameObject.SetActive(false);
        obstacleRectanglePrefabList.Add(ob);

        return ob;
    }

    Transform GetRectangle()
    {
        var l = obstacleRectanglePrefabList.Find(o => o.gameObject.activeInHierarchy == false);

        if (l == null)
        {
            l = _CreateListRectangle();
        }

        return l;
    }
   

    void CreateListCarre(int i)
    {
        int count = 0;

        while (count < i)
        {
            _CreateListCarre();
            count++;
        }
    }

    Transform _CreateListCarre()
    {
        var ob = Instantiate(obstacleCarrePrefab) as Transform;
        ob.SetParent(transform, false);
        ob.gameObject.SetActive(false);
        obstacleCarrePrefabList.Add(ob);

        return ob;
    }

    Transform GetCarre()
    {
        var l = obstacleCarrePrefabList.Find(o => o.gameObject.activeInHierarchy == false);

        if (l == null)
        {
            l = _CreateListCarre();
        }
        return l;
    }

    void CreateCoinsList(int i)
    {
        int count = 0;
        while (count < i)
        {
            _CreateListCoins();
            count++;
        }
    }
    Transform _CreateListCoins()
    {
        var ob = Instantiate(coinPrefab) as Transform;
        ob.SetParent(transform, false);
        ob.gameObject.SetActive(false);
        coinsList.Add(ob);

        return ob;
    }
    Transform GetCoin()
    {
        var l = coinsList.Find(o => o.gameObject.activeInHierarchy == false);

        if (l == null)
        {
            l = _CreateListCoins();
        }
        return l;
    }
    public void Add1Point()
    {

        point++;

        pointText.text = point.ToString();

        int best = ScoreManager.GetBestScore();

        if (point > best)
        {
            bestScoreText.text = "best: " + point;
        }
        else
        {
            bestScoreText.text = "best: " + best;
        }
    }

    /// <summary>
    /// Game Over function, who called the OnFinished event
    /// </summary>
    public void GameOver()
    {
        if (OnGameEnded != null)
            OnGameEnded();

        ScoreManager.SaveScore(point);

        LeaderboardManager.ReportScore(point);

        Utils.ReloadScene();
    }

    /// <summary>
    /// Desactivate start button (to avoid double click) and start the game
    /// </summary>
    public void OnStart()
    {
        buttonStart.onClick.RemoveAllListeners();

#if !UNITY_TVOS
        if (OnGameStarted != null)
            OnGameStarted();
#endif

        point = 0;

        countSpawn = 0;

        soundManager.PlayMusicGame();

        pointText.gameObject.SetActive(true);

        StartCoroutine(Spawner());

        SpawnParticleStart();

        Invoke("ActivateButtonStart", 2);


#if UNITY_TVOS
		mainCameraManager.StartTVOS();
		Invoke("TVOSStart",0.31f);
#endif
    }

#if UNITY_TVOS
	void TVOSStart()
	{
		if(OnGameStarted!=null)
			OnGameStarted ();
	}
#endif

    /// <summary>
    /// 将所有所有产生的障碍物对象放回对象池
    /// </summary>
    public void DespawnAll()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 要产生的障碍数
    /// </summary>
    int countSpawn = 0;

    /// <summary>
    /// 在游戏中产生障碍。 如果视野中的障碍数> 10, 暂停产生障碍.如果 <10 继续产生障碍
    /// </summary>
    IEnumerator Spawner()
    {
        while (true)
        {
            countSpawn++;

            float posY = (5 + countSpawn) * 5;   //第一个障碍的Y坐标

            if (Utils.RandomRange(0, 3) == 0)     //0-3取随机数，如果取到0，那么产生长条障碍物
            {
                bool isRectangleLeft = Utils.RandomRange(0, 2) == 0;

                Vector2 posRectangle = Vector2.zero;

                //分左右给障碍物设置坐标
                if (isRectangleLeft)
                {
                    posRectangle = new Vector2(-12 + Utils.RandomRange(-1f, 6f), posY);
                }
                else
                {
                    posRectangle = new Vector2(12 + Utils.RandomRange(-6f, 1f), posY);
                }

                //对象池中取物体，将坐标付给产生的障碍物
                var ob = GetRectangle();   
                ob.position = posRectangle;
                ob.gameObject.SetActive(true);
            }
            else if (Utils.RandomRange(0, 5) == 0) //0-5取随机数，如果取到0，那么产生硬币
            {
                Vector2 posCoin = new Vector2(Utils.RandomRange(-3f, 3f), posY);

                //对象池中取物体，将坐标付给产生的硬币
                var ob = GetCoin();     
                ob.position = posCoin;
                ob.gameObject.SetActive(true);
            }
            else
            {
                Vector2 posCarre = new Vector2(Utils.RandomRange(-3f, 3f), posY);

                //对象池中取物体，将坐标付给产生的障碍物  
                var ob = GetCarre();     
                ob.position = posCarre;
                ob.gameObject.SetActive(true);
            }


#if UNITY_TVOS
			while (GetCount() > 30)
			{
				yield return null;
			}
#else
            while (GetCount() > 10)
            {
                yield return null;
            }
#endif

            yield return null;
        }

    }

    /// <summary>
    /// 获取当前场景中障碍物的个数
    /// </summary>
    int GetCount()
    {
        int count = 0;

        count += obstacleRectanglePrefabList.FindAll(o => o.gameObject.activeInHierarchy == true).Count;

        count += obstacleCarrePrefabList.FindAll(o => o.gameObject.activeInHierarchy == true).Count;

        //count += coinsList.FindAll(o => o.gameObject.activeInHierarchy == true).Count;

        return count;
    }

    /// <summary>
    /// 游戏开始产生粒子特效
    /// </summary>
    public void SpawnParticleStart()
    {
        var pe = Instantiate(particleExplosionStart) as ParticleEmitter;
        pe.transform.position = new Vector3(0, 0, -1);
        pe.transform.rotation = Quaternion.identity;

    }

    /// <summary>
    /// 当接触到左边的墙时，产生特效
    /// </summary>
    public void SpawnParticleWallLeft(Vector3 v)
    {
        var pe = Instantiate(particleExplosionWallLeftPrefab) as ParticleEmitter;
        pe.transform.position = v;
        pe.transform.rotation = Quaternion.identity;

    }

    /// <summary>
    /// 当接触到右边的墙时，产生特效
    /// </summary>
    public void SpawnParticleWallRight(Vector3 v)
    {
        var pe = Instantiate(particleExplosionWallRightPrefab) as ParticleEmitter;
        pe.transform.position = v;
        pe.transform.rotation = Quaternion.identity;

    }

    /*
    public void ShowAds()
    {
        int count = PlayerPrefs.GetInt("GAMEOVER_COUNT", 0);
        count++;

#if APPADVISORY_ADS
		if(count > numberOfPlayToShowInterstitial)
		{
#if UNITY_EDITOR
		print("count = " + count + " > numberOfPlayToShowINterstitial = " + numberOfPlayToShowInterstitial);
#endif
		if(AdsManager.instance.IsReadyInterstitial())
		{
#if UNITY_EDITOR
			print("AdsManager.instance.IsReadyInterstitial() == true ----> SO ====> set count = 0 AND show interstial");
#endif
			PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
			AdsManager.instance.ShowInterstitial();
		}
		else
		{
#if UNITY_EDITOR
			print("AdsManager.instance.IsReadyInterstitial() == false");
#endif
		}

	}
	else
	{
		PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
	}
	PlayerPrefs.Save();
#else
        if (count >= numberOfPlayToShowInterstitial)
        {
            PlayerPrefs.SetInt("GAMEOVER_COUNT", 0);
        }
        else
        {
            PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
        }
        PlayerPrefs.Save();
#endif

    }
    */
}
