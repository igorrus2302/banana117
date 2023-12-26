using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scenes
{
    MainMenu, 
    Game
}
public class LevelManager : MonoBehaviour
{
    private static float FadeSpeed = 0.02f;
    private static Color Fadetransparency = new Color(0, 0, 0, 0.04f);
    private static AsyncOperation _async;
    public static LevelManager Instance;
    public GameObject _faderObj;
    public Image _faderImg;
    public void Start()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        //SceneManager.LoadScene(1);    
        PlayScene(Scenes.MainMenu);
    }

    public static void PlayScene(Scenes sceneEnum)
    {
        Instance.LoadScene(sceneEnum.ToString());
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Instance.StartCoroutine(FadeIn(Instance._faderObj, Instance._faderImg));
    }

    private void LoadScene(string sceneName)
    {
        Instance.StartCoroutine(Load(sceneName));
        Instance.StartCoroutine(FadeOut(Instance._faderObj, Instance._faderImg));
    }

    private static IEnumerator FadeOut(GameObject faderObject, Image fader)
    {
        faderObject.SetActive(true);
        while (fader.color.a < 1)
        {
            fader.color += Fadetransparency;
            yield return new WaitForSeconds(FadeSpeed);
        }
        ActivateScene();
    }

    private static IEnumerator FadeIn(GameObject faderObject, Image fader)
    {
        faderObject.SetActive(true);
        while (fader.color.a > 0)
        {
            fader.color -= Fadetransparency;
            yield return new WaitForSeconds(FadeSpeed);
        }
        faderObject.SetActive(false);
    }

    private static IEnumerator Load(string sceneName)
    {
        _async = SceneManager.LoadSceneAsync(sceneName);
        _async.allowSceneActivation = false;
        yield return _async;
    }

    private static void ActivateScene()
    {
        _async.allowSceneActivation = true;
    }
}