using UnityEngine;
using System.Collections;

/// <summary>
/// 跟随玩家
/// </summary>
public class MainCameraManager : MonoBehaviorHelper
{

 
  
        //玩家
	public  Transform player;
	/// <summary>
	///左墙
	/// </summary>
	public Transform left;
	/// <summary>
	///右墙
	/// </summary>
	public Transform right;

	/// <summary>
	/// 照相机跟随player，游戏结束时为false
	/// </summary>
	public bool stopFollow = false;

	/// <summary>
	/// 左右两堵墙间距是否相同，默认为true
	/// </summary>
	public bool useContantWidth = true;
    /// <summary>
    ///左右两堵墙间距相同时的距离
    /// </summary>
    public float constantWidth = 7f;

	#if UNITY_TVOS
	void Awake()
	{
		constantWidth *= 1.5f;
	}
	#endif


	void OnEnable()
	{
		GameManager.OnGameStarted += OnStarted;

		GameManager.OnGameEnded += OnFinished;
	}
	void OnDisable()
	{
		GameManager.OnGameStarted -= OnStarted;

		GameManager.OnGameEnded -= OnFinished;
	}
    static int a = 1;

	void Start ()
	{
        /////默认球的颜色为白色
        //if (Application.loadedLevelName== "menu1")
        //    PlayerPrefs.SetInt("Skin",0);

        /////根据上一次选择的player的颜色来加载player
        //if (Application.loadedLevelName== "GravityBall")
        //{
        //   GameObject go= Instantiate(Resources.Load(PlayerPrefs.GetInt("Skin").ToString(), typeof(GameObject))) as GameObject;
        //    player = go.transform;
        //}
		stopFollow = false;

		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;


		float camHalfHeight = height/2f;
		float camHalfWidth = width/2f; 

		float size = Mathf.Min(camHalfHeight, camHalfWidth);

		if(useContantWidth)
			size = constantWidth;


		float decal = Mathf.Min(size*0.15f, size*0.15f);

		left.position = new Vector2 (-size + decal, 0);   

		right.position = new Vector2 (+size - decal, 0);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

	private void OnStarted(){
		stopFollow = false;
	}

	private void OnFinished()
	{
		stopFollow = true;
	}

	/// <summary>
	/// 保持相机的Y坐标值和player相同，如果stopfollow为false则直接返回
	/// </summary>
	public void UpdatePos()
	{
		if (stopFollow)
			return;

		Vector3 pos = transform.position;

		if (player == null) {
           
            return; }
			
		pos.y = player.transform.position.y;

		transform.position = pos;
	}
    int ClickTime=0;
    float timeFirst;
    float timeSecond;


    void Update()
    {
       
    }
}
