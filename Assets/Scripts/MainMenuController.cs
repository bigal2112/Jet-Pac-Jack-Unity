using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    Debug.Log("Loading level 1"); ;
    SceneManager.LoadScene(1);
  }


}
