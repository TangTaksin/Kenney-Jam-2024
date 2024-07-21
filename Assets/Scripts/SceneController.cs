using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadNextScene(String nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

}
