using UnityEngine;
using System.Collections;

/// <summary>
/// Class in charge to follow the player and to place the left and right walls on the screen
/// 
/// This script is attached to the Main Camera. This script is in charge to follow the Player vertically.
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
        ///默认球的颜色为白色
        if (Application.loadedLevelName== "menu1")
        {
            PlayerPrefs.SetString("Skin", "Player");
        }
        ///根据上一次选择的player的颜色来加载player
        if (Application.loadedLevelName== "GravityBall") {
           
           GameObject go= Instantiate(Resources.Load(PlayerPrefs.GetString("Skin"), typeof(GameObject))) as GameObject;
            player = go.transform;

        }
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


	



	}

	private void OnStarted(){
		stopFollow = false;


	}


	#if UNITY_TVOS
	
	public void StartTVOS()
	{

		StartCoroutine(DoRotate());

	}



	/// <summary>
	/// Smoothly change the rotation on TV
	/// </summary>
	public IEnumerator DoRotate()
	{
		float timer = 0;
		float time = 0.3f;

		while (timer <= time)
		{
			timer += Time.deltaTime;

			transform.eulerAngles = Vector3.forward * Mathf.Lerp(0,90,timer/time);

			left.parent.localEulerAngles = Vector3.forward * Mathf.Lerp(0,90,timer/time);
			yield return null;
		}
		left.parent.localEulerAngles = Vector3.forward * 90;
	}

	#endif

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

   
        if (player==null)
        {
         
      
          player = GameObject.FindGameObjectWithTag("Player").transform;
        }
      


        if (Input.GetKeyDown(KeyCode.Escape))//按手机：返回键
        {
            ClickTime += 1;//按一次就加一次
            if (ClickTime == 1) { timeFirst = Time.time; }//记录第一次按下返回键的时间

            if (ClickTime == 2)
            {
                timeSecond = Time.time;//记录第二次按下返回键的时间

                if (timeSecond - timeFirst <= 2f)
                {//第二次 减 第一次的时间在2秒内，就执行
                    Application.Quit();//退出
                }
                else//否则次数归还为0
                {
                    ClickTime = 0;
                }
            }


        }

    }
}
