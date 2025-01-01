using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public string sceneToLoad = "";
    public void LoadDashboardAfter() => sceneToLoad = "Dashboard";
    public void LoadRegisterAfter() => sceneToLoad = "Register";
    public void LoadSignInAfter() => sceneToLoad = "SignIn";
    public void LoadTutorialAfter() => sceneToLoad = "Tutorial";
    
    public void Update()
    {
        if (sceneToLoad.Length > 0)
        {
            SceneManager.LoadScene(sceneToLoad);
            sceneToLoad = "";
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
