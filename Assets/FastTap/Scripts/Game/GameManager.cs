using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using LOS;

/// <summary>

/// 逻辑脚本添加障碍物
/// </summary>
public class GameManager : MonoBehaviorHelper
{
    public int numberOfPlayToShowInterstitial = 5;
    public Transform spawnPos;
    public GameObject popUpContinuePrefab;

    public delegate void GameStart();
    public static event GameStart OnGameStarted;

    public delegate void GameEnd();
    public static event GameEnd OnGameEnded;

    //预制体
    public List<Transform> obstacleRectanglePrefab;
    public List<Transform> obstacleCarrePrefab;
    public Transform[] coinPrefab = new Transform[2];
    public List<Transform> ballPrefabs;
    public List<Transform> podiumPrefabs;
    public List<Transform> buffsPre = new List<Transform>();

    //将要显示的物体集
    List<Transform> obstacleRectanglePrefabList = new List<Transform>();
    List<Transform> obstacleCarrePrefabList = new List<Transform>();
    List<Transform> coinPrefabList = new List<Transform>();
    List<Transform> buffs = new List<Transform>();
    //buffs


    //粒子特效
    public ParticleEmitter particleExplosionStart;
    public ParticleEmitter particleExplosionWallLeftPrefab;
    public ParticleEmitter particleExplosionWallRightPrefab;



    public Text pointText;
    public Text bestScoreText;
    public Text title;
    public Button buttonStart;

    public Text coinsText;
    private int coins;
    public int Coins
    {
        get
        {
            return PlayerPrefs.GetInt("coinNum");
        }

        set
        {
            coins = value;
            coinsText.text = "Coins: " + coins.ToString();
            PlayerPrefs.SetInt("coinNum", coins);
        }
    }
    public int GetCurrentCoins()
    {
        return PlayerPrefs.GetInt("coinNum");
    }

    private int point;
    public int GetPoint()
    {
        return point;
    }


    Transform Wall;
    Wave effects;

    private void Awake()
    {
        effects = Camera.main.GetComponent<Wave>();
        Wall = GameObject.Find("Walls").transform;
        if (Time.realtimeSinceStartup < 5)
            LeaderboardManager.Init();

        //设置Text内容
        pointText.gameObject.SetActive(false);
        title.gameObject.SetActive(true);

        int best;
        switch (PlayerPrefs.GetFloat("speedy").ToString())
        {
            case "10":
                best = ScoreManager.GetBestScore(1);
                bestScoreText.text = "Best: " + best;
                break;
            case "15":
                best = ScoreManager.GetBestScore(2);
                bestScoreText.text = "Best: " + best;
                break;
            case "20":
                best = ScoreManager.GetBestScore(3);
                bestScoreText.text = "Best: " + best;
                break;
        }

        coinsText.text = "Coins: " + GetCurrentCoins();
        pointText.text = "0";

        ActivateButtonStart();

        ThemeSelect();
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        GC.Collect();

    }

    void ThemeSelect()
    {
        if (PlayerPrefs.HasKey("Skin"))
        {
            int index = PlayerPrefs.GetInt("Skin");
            //根据换肤记录，更换主题

            //替换障碍物
            CreateListRectangle(20, PlayerPrefs.GetInt("Skin"));
            CreateListCarre(20, PlayerPrefs.GetInt("Skin"));

            //替换球
            Transform ball = Instantiate(ballPrefabs[index]) as Transform;
            ball.position = spawnPos.position;
            ball.GetChild(0).GetComponent<LOSRadialLight>().enabled = true;

            //替换起点柱
            Transform pos = GameObject.Find("PodiumStart").transform;
            Transform podiumStart = Instantiate(podiumPrefabs[index]) as Transform;
            Destroy(GameObject.Find("PodiumStart"));

            //Change Walls
            Color color = new Color();
            switch (PlayerPrefs.GetInt("Skin"))
            {
                case 0:
                    color = Color.white;
                    break;
                case 1:
                    color = new Color(1, 0.8313f, 0.6196f);
                    break;
                case 2:
                    color = new Color(0.7725f, 1, 0.6196f);
                    break;
                case 3:
                    color = new Color(0.6196f, 0.8431f, 1);
                    break;
                case 4:
                    color = new Color(0.9843f, 0.9254f, 0.4352f);
                    break;
                case 5:
                    color = new Color(0.0823f, 0.2509f, 0.4196f);
                    break;
                case 6:
                    color = new Color(1, 0.9372f, 0.8588f);
                    break;
                default:
                    break;
            }
            for (int i = 0; i < 2; i++)
            {
                Wall.GetChild(i).GetComponent<SpriteRenderer>().color = color;
            }
        }
        else
        {
            PlayerPrefs.SetInt("Skin", 0);
            //使用原始主题
            CreateListRectangle(20, 0);
            CreateListCarre(20, 0);
            Transform ball = Instantiate(ballPrefabs[0]) as Transform;
            ball.position = spawnPos.position;
        }
        CreateListCoin(5);
    }

    /// <summary>
    /// To activate button start
    /// </summary>
    void ActivateButtonStart()
    {
        if (SceneManager.GetActiveScene().name.Contains("menu"))
            return;
        buttonStart.onClick.RemoveAllListeners();
        buttonStart.onClick.AddListener(OnStart);
    }

    void CreateListCoin(int i)
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
        int index = PlayerPrefs.GetInt("Skin")==6?0:1;
        var ob = Instantiate(coinPrefab[index]) as Transform;
        ob.SetParent(transform, false);
        ob.gameObject.SetActive(false);
        coinPrefabList.Add(ob);

        return ob;
    }
    Transform GetCoin()
    {
        var l = coinPrefabList.Find(o => o.gameObject.activeInHierarchy == false);

        if (l == null)
        {
            l = _CreateListCoins();
        }

        return l;
    }

    //创建对象池的过程
    void CreateListRectangle(int i, int prbIndex)
    {
        int count = 0;

        while (count < i)
        {
            _CreateListRectangle(prbIndex);
            count++;
        }
    }
    //创建对象
    Transform _CreateListRectangle(int prbIndex)
    {
        var ob = Instantiate(obstacleRectanglePrefab[prbIndex]) as Transform;
        ob.SetParent(transform, false);
        ob.gameObject.SetActive(false);
        obstacleRectanglePrefabList.Add(ob);

        return ob;
    }
    //从池中取操作
    Transform GetRectangle(int prbIndex)
    {
        //找池中没显示的物体
        var l = obstacleRectanglePrefabList.Find(o => o.gameObject.activeInHierarchy == false);

        //如果没有就再创建一个
        if (l == null)
            l = _CreateListRectangle(prbIndex);

        return l;
    }

    void CreateListCarre(int i, int prbIndex)
    {
        int count = 0;

        while (count < i)
        {
            _CreateListCarre(prbIndex);
            count++;
        }
    }
    Transform _CreateListCarre(int prbIndex)
    {
        var ob = Instantiate(obstacleCarrePrefab[prbIndex]) as Transform;
        ob.SetParent(transform, false);
        ob.gameObject.SetActive(false);
        obstacleCarrePrefabList.Add(ob);

        return ob;
    }
    Transform GetCarre(int prbIndex)
    {
        var l = obstacleCarrePrefabList.Find(o => o.gameObject.activeInHierarchy == false);

        if (l == null)
        {
            l = _CreateListCarre(prbIndex);
        }
        return l;
    }

    void InitiateBuff(int nums)
    {
        int count = 0;

        int buffIndex = UnityEngine.Random.Range(0, 30);
        buffIndex = buffIndex <= 10 ? 0 : (buffIndex <= 20 ? 1 : 2);

        while (count < nums)
        {
            _InitiateBuff(buffIndex);
            count++;
        }
    }
    Transform _InitiateBuff(int prbIndex)
    {
        var ob = Instantiate(buffsPre[prbIndex]) as Transform;
        ob.SetParent(transform, false);
        ob.gameObject.SetActive(false);
        obstacleCarrePrefabList.Add(ob);

        return ob;
    }
    Transform GetBuffs(int prbIndex)
    {
        var l = obstacleCarrePrefabList.Find(o => o.gameObject.activeInHierarchy == false);

        if (l == null)
        {
            l = _InitiateBuff(prbIndex);
        }
        return l;
    }

    // 随机产生障碍物 如果现实的障碍物数量大于10则暂停，小于10则重新启动
    IEnumerator Spawner()
    {
        while (true)
        {
            //查找已经显示的障碍数，小于10则
            if (GetCount() < 10)
            {
                countSpawn++;
                bool hasSkinKey = PlayerPrefs.HasKey("Skin");
                float posY = (5 + countSpawn) * 5;

                //给长形障碍调整位置
                if (Utils.RandomRange(0, 3) == 0)
                {
                    bool isRectangleLeft = Utils.RandomRange(0, 2) == 0;

                    Vector3 posRectangle = new Vector3(0, 0, 1f);

                    if (isRectangleLeft)
                    {
                        posRectangle = new Vector3(-12 + Utils.RandomRange(-1f, 5f), posY, 1f);
                    }
                    else
                    {
                        posRectangle = new Vector3(12 + Utils.RandomRange(-5f, 1f), posY, 1f);
                    }

                    if (hasSkinKey)
                    {
                        var ob = GetRectangle(PlayerPrefs.GetInt("Skin"));
                        ob.position = posRectangle;
                        ob.gameObject.SetActive(true);
                    }
                    else
                    {
                        var ob = GetRectangle(0);
                        ob.position = posRectangle;
                        ob.gameObject.SetActive(true);
                    }
                }
                //Coin位置赋值
                else if (Utils.RandomRange(0, 5) == 0)
                {
                    Vector2 posCoin = new Vector2(Utils.RandomRange(-3f, 3f), posY);

                    var ob = GetCoin();
                    ob.position = posCoin;
                    ob.gameObject.SetActive(true);
                }
                //buff位置赋值
                else if (Utils.RandomRange(0, 12) == 0)
                {
                    Vector2 posBuff = new Vector2(Utils.RandomRange(-5f, 5f), posY);

                    int buffIndex = UnityEngine.Random.Range(0, 30);
                    buffIndex = buffIndex <= 10 ? 0 : (buffIndex <= 20 ? 1 : 2);

                    var ob = GetBuffs(buffIndex);
                    ob.position = posBuff;
                    ob.gameObject.SetActive(true);
                }
                //给方形障碍调整位置
                else
                {
                    Vector2 posCarre = new Vector2(Utils.RandomRange(-3f, 3f), posY);

                    if (hasSkinKey)
                    {
                        var ob = GetCarre(PlayerPrefs.GetInt("Skin"));
                        ob.position = posCarre;
                        ob.gameObject.SetActive(true);
                    }
                    else
                    {
                        var ob = GetRectangle(0);
                        ob.position = posCarre;
                        ob.gameObject.SetActive(true);
                    }
                }
            }
            yield return null;
        }

    }
    public void Add1Point()
    {

        point++;

        pointText.text = point.ToString();

        int best;
        switch (PlayerPrefs.GetFloat("speedy").ToString())
        {
            case "10":
                {
                    best = ScoreManager.GetBestScore(1);
                    if (point > best)
                        bestScoreText.text = "best: " + point;
                    else
                        bestScoreText.text = "best: " + best;
                }
                break;
            case "15":
                {
                    best = ScoreManager.GetBestScore(2);
                    if (point > best)
                        bestScoreText.text = "best: " + point;
                    else
                        bestScoreText.text = "best: " + best;
                }
                break;
            case "20":
                {
                    best = ScoreManager.GetBestScore(3);
                    if (point > best)
                        bestScoreText.text = "best: " + point;
                    else
                        bestScoreText.text = "best: " + best;
                }
                break;
        }


    }

    /// <summary>
    /// 游戏结束时调用该方法
    /// </summary>
    public void GameOver()
    {
        if (OnGameEnded != null)
            OnGameEnded();
        switch (PlayerPrefs.GetFloat("speedy").ToString())
        {
            case "10":
                ScoreManager.SaveScore(point, 1);
                break;
            case "15":
                ScoreManager.SaveScore(point, 2);
                break;
            case "20":
                ScoreManager.SaveScore(point, 3);
                break;
        }

        Utils.ReloadScene();

    }

    /// <summary>
    /// 游戏开始后接触开始游戏按钮
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

        //开始计时
        StartCoroutine(Timing());


#if UNITY_TVOS
		mainCameraManager.StartTVOS();
		Invoke("TVOSStart",0.31f);
#endif
    }

    private IEnumerator Timing()
    {
        while (true)
        {
            //if (time % 15 == 0 && time !=0)
            //{
            //    Debug.Log(111);
            //    //StopCoroutine(effects.SpecialEffect());
            //    StartCoroutine(effects.SpecialEffect());
            //}
            time++;
            yield return new WaitForSeconds(1);
        }

    }

    public void DespawnAll()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    int countSpawn = 0;
    public int time = 0;

    int GetCount()
    {
        int count = 0;

        count += obstacleRectanglePrefabList.FindAll(o => o.gameObject.activeInHierarchy == true).Count;

        count += obstacleCarrePrefabList.FindAll(o => o.gameObject.activeInHierarchy == true).Count;

        count += coinPrefabList.FindAll(o => o.gameObject.activeInHierarchy == true).Count;

        return count;
    }

    public void SpawnParticleStart()
    {
        var pe = Instantiate(particleExplosionStart) as ParticleEmitter;
        pe.transform.position = new Vector3(0, 0, -1);
        pe.transform.rotation = Quaternion.identity;

    }

    public void SpawnParticleWallLeft(Vector3 v)
    {
        var pe = Instantiate(particleExplosionWallLeftPrefab) as ParticleEmitter;
        pe.transform.position = v;
        pe.transform.rotation = Quaternion.identity;

    }


    public void SpawnParticleWallRight(Vector3 v)
    {
        var pe = Instantiate(particleExplosionWallRightPrefab) as ParticleEmitter;
        pe.transform.position = v;
        pe.transform.rotation = Quaternion.identity;

    }

}
