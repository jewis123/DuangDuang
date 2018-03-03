﻿using UnityEngine;
using System.Collections;
#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif

/// <summary>
/// Class who managed the player
/// 
/// This script is attached go the GameObject "Player".
/// In charge to detect the input, and to jump the player from one side to the other side, and detect collisions. 
/// You can change the speed of the jump in this GameObject ("Constant force y") and the speed of the player ("Constant force x").
/// </summary>
public class PlayerManager : MonoBehaviorHelper
{



	/// <summary>
	/// True if the player can jump
	/// </summary>
	private bool canJump;

	/// <summary>
	/// True if game over
	/// </summary>
	private bool isGameOver;


	/// <summary>
	/// The force apply to the player when is jumping
	/// </summary>
	public float ConstantForceX;

	/// <summary>
	/// The force apply to the player to move up continuously
	/// </summary>
	public float ConstantForceY;

	/// <summary>
	/// reference to the player rigidbody2D
	/// </summary>
	private Rigidbody2D _rigidbody;

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
	/// Subscribe to OnTouchScreen from InputTouch
	/// </summary>
	void OnEnable()
	{
		InputTouch.OnTouchScreen += OnTouchScreen;

		GameManager.OnGameStarted += OnStarted;

		GameManager.OnGameEnded += OnFinished;
	}

	/// <summary>
	/// Unsubscribe to OnTouchScreen from InputTouch
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


	/// <summary>
	/// When game over, ridbody2d is kinematic so the player doesn't move anymore
	/// </summary>
	void OnFinished()
	{
		_rigidbody.isKinematic = true;
	}

	/// <summary>
	/// When the game is started, the ridbody2d is not kinematic (to apply force to it) and we start the coroutine to continuously move up the player
	/// </summary>
	private void OnStarted()
	{
		if (_rigidbody.isKinematic) {
			_rigidbody.isKinematic = false;
		}

		_rigidbody.velocity = new Vector2 (-ConstantForceX , ConstantForceY);

		StartCoroutine (OnStartDelay ());
	}

	/// <summary>
	/// A little delay to start the game, just to have the tiome to emit the particles and make some stuff like isGameOver = false and canJump = true with a delay
	/// </summary>
	IEnumerator OnStartDelay()
	{
		yield return new WaitForSeconds (0.3f);
		gameObject.SetActive (true);
		isGameOver = false;
		canJump = true;

		StartCoroutine (CoUpdate ());
	}

	/// <summary>
	/// Call OnCollision if collision with player and obstacles or walls 
	/// </summary>
	void OnCollisionEnter2D(Collision2D coll) 
	{

		OnCollision (coll.gameObject, coll);

	}

	/// <summary>
	/// Check who is collide with the player: if walls: emit particles, if obstacles: game over
	/// </summary>
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
		else if (coll.gameObject.name.Contains("Rectangle") || coll.gameObject.name.Contains("Carre")) 
		{
			transform.position = coll.contacts [0].point;
			LaunchGameOver ();
		}
	}

	/// <summary>
	/// Turn isGameOver to true and lauch the coroutine CoroutLaunchGameOver
	/// </summary>
	void LaunchGameOver()
	{
		if (isGameOver)
			return;

		isGameOver = true;

		StartCoroutine (CoroutLaunchGameOver ());
	}
		

	/// <summary>
	/// Coroutine to turn rigidbody2d to kinematic = true (player can't move anymore), emit particles game over, and show popup continue if a rewarded video is available. And if game over potentially show interstitial (please have a look to AdsManager)
	/// </summary>
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


		if (AdsManager.instance.IsReadyRewardedVideo ()) 
		{
			yield return new WaitForSeconds (1.5f);

			var go = Instantiate(gameManager.popUpContinuePrefab) as GameObject;
			var popUp = go.GetComponent<PopupContinue>();

			popUp.OpenPopupContinue ((bool success) => {
				if (success) 
				{
					gameManager.DespawnAll();

					_rigidbody.velocity = Vector2.zero;

					_rigidbody.isKinematic = false;

					isGameOver = false;

					canJump = true;

					GetComponent<Collider2D> ().enabled = true;

					int direction = (transform.position.x >= 0) ? 1 : -1;


					_rigidbody.velocity = new Vector2 (direction*ConstantForceX, ConstantForceY);

					StartCoroutine(CoUpdate());

				} 
				else 
				{
					gameManager.ShowAds();

					gameManager.GameOver ();
				}
			});
		} 
		else 
		{
		#else
		gameManager.ShowAds();
		#endif
		#endif
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

	/// <summary>
	/// Like the classic Update() function. I use coroutine to avoir useless Update if the game is not started or if is game over
	/// </summary>
	IEnumerator CoUpdate()
	{
		while (true) 
		{
			if (!canJump || isGameOver)
				break;

			PlayerMovement ();

			yield return 0;
		}
	}

	/// <summary>
	/// To move the player on the Y axis with a constant force
	/// </summary>
	void PlayerMovement()
	{
		var v = _rigidbody.velocity;
		v.y = ConstantForceY;
		_rigidbody.velocity = v;

		mainCameraManager.UpdatePos ();
	}

	/// <summary>
	/// Do a player jump, ie. a move on the X axis
	/// </summary>
	void Jump(bool isLeft)
	{
		if (!canJump || isGameOver)
			return;
		
		int direction = isLeft ? -1 : 1;

		soundManager.PlayJumpFX ();

		_rigidbody.velocity = new Vector2 (direction*ConstantForceX , ConstantForceY);

	}
}