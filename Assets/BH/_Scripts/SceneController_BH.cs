using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneController_BH : MonoBehaviour
{

    public Image fadeImage;

    private static SceneController_BH _instance;
    public static SceneController_BH Instance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        fadeImage.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // �� �Ŵ����� sceneLoaded�� ü���� �Ǵ�.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ü���� �ɾ �� �Լ��� �� ������ ȣ��ȴ�.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeOut();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //videoPlayer.loopPointReached -= CheckIsVideoEnd;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(IFadeIn());
    }

    IEnumerator IFadeIn()//(float startAlpha, float endAlpha, float deltaTime)
    {
        Color alPhaColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        fadeImage.gameObject.SetActive(true);

        while (alPhaColor.a < 1)
        {
            alPhaColor.a += Time.deltaTime;
            fadeImage.color = alPhaColor;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(IFadeOut());
    }

    IEnumerator IFadeOut()//(float startAlpha, float endAlpha, float deltaTime)
    {
        Color alPhaColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        while (alPhaColor.a > 0)
        {
            alPhaColor.a -= Time.deltaTime;
            fadeImage.color = alPhaColor;
            yield return null;
        }

    }
}
