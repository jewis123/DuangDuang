using UnityEngine;
using System.Collections;

/// <summary>
/// 处理音效播放
/// 
/// 挂载于"SoundManager" GameObject 
/// </summary>
public class SoundManager : MonoBehaviorHelper
{
    //各种音效
	public AudioSource music;
	public AudioSource fx;
	public AudioClip[] musicGame;
	public AudioClip musicMenu;
	public AudioClip musicGameOver;
	public AudioClip jumpFX;
    private int ClickTime  = 0;
    private float timeFirst;
    private float timeSecond;


    void Start()
	{
		PlayMusicMenu ();
	}

	/// <summary>
	///播放音乐
	/// </summary>
	public void PlayMusicGame()
	{
        int index = PlayerPrefs.GetInt("Skin");
        PlayMusic (musicGame[index]);
	}
	/// <summary>
	///游戏结束音效
	/// </summary>
	public void PlayMusicGameOver()
	{
		playFX (musicGameOver);
	}
	/// <summary>
	/// 菜单音效
	/// </summary>
	public void PlayMusicMenu()
	{
		PlayMusic (musicMenu);
	}
	/// <summary>
	/// 跳跃音效
	/// </summary>
	public void PlayJumpFX()
	{
		playFX (jumpFX);
	}


	private void PlayMusic(AudioClip a)
	{
        if (music != null && music.clip == a)
            return;


		music.clip = a;
		music.Play ();
	}

	private void playFX(AudioClip a)
	{
		if (fx != null && fx.clip != null)
			fx.Stop ();

		fx.PlayOneShot (a);
	}


	public void MuteAllMusic()
	{
		music.Pause();
		fx.Pause();
	}

	public void UnmuteAllMusic()
	{
		music.Play();
		fx.Play();
	}


    private void Update()
    {
      

        if (Input.GetKeyDown(KeyCode.Escape))//按手机：返回键
        {
            ClickTime += 1;//按一次就加一次
            if (ClickTime == 1) { timeFirst = Time.time; }//记录第一次按下返回键的时间

            if (ClickTime == 2)
            {
                timeSecond = Time.time;//记录第二次按下返回键的时间

                if (timeSecond - timeFirst <= 2f)
                {
                    //第二次 减 第一次的时间在2秒内，就执行
                    Application.Quit();//退出
                }
                else//否则次数归还为0
                    ClickTime = 0;
            }


        }

    }


}
