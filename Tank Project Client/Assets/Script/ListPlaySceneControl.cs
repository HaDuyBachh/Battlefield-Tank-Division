using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPlaySceneControl : MonoBehaviour
{
    public void ChangePlayScene()
    {
        FindAnyObjectByType<SceneControl>().ChangeScene(2);
    }    
}
