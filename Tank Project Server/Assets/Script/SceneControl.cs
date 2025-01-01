using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public string currentScene = "";
    public string sceneToLoad = "";
    public void LoadTutorialdAfter() => sceneToLoad = "Tutorial";
    public void LoadMainAfter() => sceneToLoad = "Main";

    public void Update()
    {
        if (sceneToLoad.Length > 0)
        {
            SceneManager.LoadScene(sceneToLoad);
            currentScene = sceneToLoad;
            sceneToLoad = "";
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
