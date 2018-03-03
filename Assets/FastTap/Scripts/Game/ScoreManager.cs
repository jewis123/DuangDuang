using UnityEngine;
using System.Collections;

/// <summary>
/// 集中处理分数和最高分
/// </summary>
public class ScoreManager 
{

	/// <summary>
	/// 保存分数
	/// </summary>
	public static void SaveScore(int lastScore,int i)
	{
		PlayerPrefs.SetInt ("LAST_SCORE".ToString(), lastScore);

		int bestScore = PlayerPrefs.GetInt ("BEST_SCORE"+i.ToString());

		if (lastScore > bestScore) {
			Debug.Log ("NEW BEST SCORE : " + lastScore);
			PlayerPrefs.SetInt ("BEST_SCORE"+i.ToString(), lastScore);
			PlayerPrefs.SetInt ("LAST_SCORE_IS_NEW_BEST", 1);
			return;
		}

		PlayerPrefs.SetInt ("LAST_SCORE_IS_NEW_BEST", 0);

	}

	/// <summary>
	/// Get the last score
	/// </summary>
	public static int GetLastScore()
	{
		return PlayerPrefs.GetInt ("LAST_SCORE");
	}

	/// <summary>
	///如果新纪录则返回真
	/// </summary>
	public static bool GetLastScoreIsBest(){
		int temp = PlayerPrefs.GetInt ("LAST_SCORE_IS_NEW_BEST");
		if (temp == 1) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// 获取最高分
	/// </summary>
	public static int GetBestScore(int i)
	{
		return PlayerPrefs.GetInt ("BEST_SCORE"+i.ToString());
	}
}
