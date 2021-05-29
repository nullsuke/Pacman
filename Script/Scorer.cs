using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Scorer : MonoBehaviour
{
    private Text scoreText = default;

    public void Set(int score, Vector2 pos, float span)
    {
        scoreText.text = score.ToString();
        scoreText.transform.position = pos;
        scoreText.gameObject.SetActive(true);

        StartCoroutine(CoroutineShow(span));
    }

    private void Awake()
    {
        scoreText = GetComponent<Text>();
        scoreText.gameObject.SetActive(false);
    }

    private IEnumerator CoroutineShow(float span)
    {
        yield return new WaitForSeconds(span);
        scoreText.gameObject.SetActive(false);
    }
}
