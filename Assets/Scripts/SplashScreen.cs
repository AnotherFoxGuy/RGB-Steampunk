using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float fadeTime = 1.0f;

    public Material[] Logos;
    public int Nextscene = 1;

    private IEnumerator Start()
    {
        foreach (var m in Logos)
            m.color = new Color(1, 1, 1, 0);

        foreach (var m in Logos)
        {
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(Fademat(m, fadeTime, Fade.In));
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Fademat(m, fadeTime, Fade.Out));
            yield return new WaitForSeconds(0.5f);
        }
#if UNITY_EDITOR
        foreach (var m in Logos)
            m.color = new Color(1, 1, 1, 1);
#endif //UNITY_EDITOR
        SceneManager.LoadScene(Nextscene);
    }

    private IEnumerator Fademat(Material curentmat, float timer, Fade fadeType)
    {
        var start = fadeType == Fade.In ? 0.0f : 1.0f;
        var end = fadeType == Fade.In ? 1.0f : 0.0f;
        var i = 0.0f;
        var step = 1.0f / timer;
        while (i < 1.0f)
        {
            i += step * Time.deltaTime;
            //curentmat.color.a = new Mathf.Lerp(start, end, i) * 1;
            curentmat.color = new Color(1, 1, 1, Mathf.Lerp(start, end, i));
            //print("" + Mathf.Lerp(start, end, i));
            yield return null;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.anyKeyDown)
            SceneManager.LoadScene(Nextscene);
    }

    private enum Fade
    {
        In,
        Out
    }
}