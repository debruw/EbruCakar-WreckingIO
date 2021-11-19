using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundController : MonoBehaviour
{
    [SerializeField]
    GameObject[] GroundCircles;
    public MeshRenderer[] Cylinders;
    float counter, counterMax = 20f;
    int currentCircle;
    bool isBlinking;

    // Start is called before the first frame update
    void Start()
    {
        counter = counterMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        counter -= Time.deltaTime;
        Cylinders[currentCircle].material.SetFloat("_Arc2", 360 - ((counterMax - counter) / counterMax * 360));
        if (counter <= 0)
        {
            StartCoroutine(WaitAndLoseChunks());
            counter = counterMax;
            Cylinders[currentCircle].gameObject.SetActive(false);
            currentCircle++;
            Cylinders[currentCircle].gameObject.SetActive(true);
            Cylinders[currentCircle].material.SetFloat("_Arc2", 0);
        }
        if (counter < 3 && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkWarning());
        }
    }

    IEnumerator BlinkWarning()
    {
        while (counter > 0 && counter < 3)
        {
            yield return new WaitForSeconds(.5f);
            Cylinders[currentCircle].enabled = !Cylinders[currentCircle].enabled;
            if (GameManager.Instance.isGameOver)
            {
                Cylinders[currentCircle].enabled = false;
                break;
            }
        }
        isBlinking = false;
    }

    IEnumerator WaitAndLoseChunks()
    {
        Transform currentCircleTr = GroundCircles[currentCircle].transform;
        GroundCircles[currentCircle].GetComponent<MeshRenderer>().enabled = false;
        int forCount = GroundCircles[currentCircle].transform.childCount;

        for (int i = 0; i < forCount; i++)
        {
            currentCircleTr.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = 0; i < forCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(.1f, .2f));
            //convex colliders with rigidbody not supported
            currentCircleTr.GetChild(i).GetComponent<MeshCollider>().enabled = true;
            currentCircleTr.GetChild(i).GetComponent<MeshCollider>().convex = true;
            currentCircleTr.GetChild(i).gameObject.AddComponent<Rigidbody>();
        }
    }
}
