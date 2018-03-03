using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviorHelper
{
	private bool canJump;
	private bool isGameOver;
	public float ConstantForceX;
	public float ConstantForceY;
	private Rigidbody2D _rigidbody;
    float exspeed = 0.08f;

    void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D> ();
		_rigidbody.isKinematic = true;
		transform.position = Vector2.zero;
		canJump = false;

	
		#if UNITY_TVOS
		ConstantForceX *= 3;
		#endif
	}

	/// <summary>
	/// 事件委托加载
	/// </summary>
	void OnEnable()
	{
		InputTouch.OnTouchScreen += OnTouchScreen;

		GameManager.OnGameStarted += OnStarted;

		GameManager.OnGameEnded += OnFinished;
	}

	/// <summary>
	/// 事件处理卸载
	/// </summary>
	void OnDisable()
	{
		InputTouch.OnTouchScreen -= OnTouchScreen;

		GameManager.OnGameStarted -= OnStarted;

		GameManager.OnGameEnded -= OnFinished;
	}

	void OnTouchScreen()
	{
		Jump(transform.position.x >= 0);
	}

	void Start()

	{
		ConstantForceY = PlayerPrefs.GetFloat ("speedy");
		isGameOver = false;
	}

	void OnFinished()
	{
		_rigidbody.isKinematic = true;
	}
	private void OnStarted()
	{
		if (_rigidbody.isKinematic) {
			_rigidbody.isKinematic = false;
		}

		_rigidbody.velocity = new Vector2 (-ConstantForceX , ConstantForceY);

		StartCoroutine (OnStartDelay ());
	}
	IEnumerator OnStartDelay()
	{
		yield return new WaitForSeconds (0.3f);
		gameObject.SetActive (true);
		isGameOver = false;
		canJump = true;

		StartCoroutine (CoUpdate ());
	}
	void OnCollisionEnter2D(Collision2D coll) 
	{
		OnCollision (coll.gameObject, coll);
	}

	void OnCollision(GameObject obj, Collision2D coll)
	{
		if (isGameOver)
			return;

		_rigidbody.velocity = new Vector2 (0, ConstantForceY);
		if (coll.gameObject.name.Contains("WallLeft"))
		{
			gameManager.Add1Point ();
			gameManager.SpawnParticleWallLeft (coll.contacts [0].point);
		}
		else if (coll.gameObject.name.Contains("WallRight")) 
		{
			gameManager.Add1Point ();
			gameManager.SpawnParticleWallRight (coll.contacts [0].point);
		} 
		else if (coll.transform.tag.Equals("Obstacles")) 
		{
			transform.position = coll.contacts [0].point;
			LaunchGameOver ();
		}
	}

	void LaunchGameOver()
	{
		if (isGameOver)
			return;

		isGameOver = true;

		StartCoroutine (CoroutLaunchGameOver ());
	}
		

	IEnumerator CoroutLaunchGameOver()
	{
		_rigidbody.velocity = Vector2.zero;

		_rigidbody.isKinematic = true;

		GetComponent<Collider2D> ().enabled = false;

		soundManager.PlayMusicGameOver ();

		StartCoroutine(CameraShake.Shake(Camera.main.transform,0.1f));

		#if UNITY_TVOS
		#else

			#if APPADVISORY_ADS
			yield return new WaitForSeconds (2);
			gameManager.ShowAds();
			#endif

		#endif
			yield return new WaitForSeconds (1);

			gameManager.GameOver ();
		#if UNITY_TVOS
		#else
		#if APPADVISORY_ADS
		}
		#endif
		#endif
	}

	IEnumerator CoUpdate()
	{
		while (true) 
		{
            
            ConstantForceY += exspeed * Time.deltaTime;
            //Debug.Log(ConstantForceY);
            //Debug.Log(Time.deltaTime);
            if (!canJump || isGameOver)
				break;

			PlayerMovement ();

			yield return 0;
		}
	}

	/// <summary>
	/// 给玩家纵向的力，使使用
	/// </summary>
	void PlayerMovement()
	{
		var v = _rigidbody.velocity;
		v.y = ConstantForceY;
		_rigidbody.velocity = v;

		mainCameraManager.UpdatePos ();
	}

	/// <summary>
	/// 横向施加力，实现左右跳动
	/// </summary>
	void Jump(bool isLeft)
	{
		if (!canJump || isGameOver)
			return;
		
		int direction = isLeft ? -1 : 1;

		soundManager.PlayJumpFX ();

		_rigidbody.velocity = new Vector2 (direction*ConstantForceX , ConstantForceY);

        gameObject.GetComponent<SpriteRenderer>().flipX = isLeft == true;

	}
}