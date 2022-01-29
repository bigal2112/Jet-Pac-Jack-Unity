using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
	public void RestartGame()
	{
		GameMaster.InitialiseScoresAndLives();
		SceneManager.LoadScene(0);
	}

	public void QuitGame()
	{
		Debug.Log("QUIT GAME"); ;
		Application.Quit();
	}

}
