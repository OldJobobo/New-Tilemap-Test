using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadSceneCoroutine()
    {
        SceneManager.LoadSceneAsync(2);
        yield return null;
    }
}
