using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public bool isGameStarted, isGameOver;

    #region UI Elements
    public GameObject WinPanel, LosePanel;
    #endregion

    public GameObject Confetti1, Confetti2;

    public List<GameObject> players;
    public List<GameObject> PowerUpBoxes;
    float powerUpCount;

    private void Update()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        if (PowerUpBoxes.Count > 0)
        {
            powerUpCount -= Time.deltaTime;
            if (powerUpCount <= 0)
            {
                int rand = Random.Range(0, PowerUpBoxes.Count);
                PowerUpBoxes[rand].SetActive(true);
                PowerUpBoxes[rand].GetComponent<Rigidbody>().isKinematic = false;
                PowerUpBoxes.RemoveAt(rand);
                powerUpCount = Random.Range(4f, 6f);
            }
        }
    }

    public void CheckPlayerCountForWin()
    {
        if (players.Count <= 1)
        {
            StartCoroutine(WaitAndGameWin());
        }
    }

    public IEnumerator WaitAndGameWin()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Confetti1.SetActive(true);
            Confetti2.SetActive(true);

            yield return new WaitForSeconds(1f);
            WinPanel.SetActive(true);
        }
    }

    public IEnumerator WaitAndGameLose()
    {
        if (!isGameOver)
        {
            isGameOver = true;

            yield return new WaitForSeconds(1f);
            LosePanel.SetActive(true);
        }
    }

    public void TapToStartButtonClick()
    {
        isGameStarted = true;
    }

    public void TapToPlayAgainButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
