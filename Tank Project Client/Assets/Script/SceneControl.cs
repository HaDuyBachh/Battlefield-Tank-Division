using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }    
    public void Quit()
    {
        Application.Quit();
    }
}
