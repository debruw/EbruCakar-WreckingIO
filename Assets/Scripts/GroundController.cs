using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundController : MonoBehaviour
{
    [SerializeField]
    GameObject[] GroundCircles;
    float counter, counterMax = 20f;
    int currentCircle;
    [SerializeField]
    GameObject WarningCanvas;
    [SerializeField]
    float[] canvasSizes;
    [SerializeField]
    float[] maincolliderSizes;
    public GameObject MainColliderObject;
    Image warningFillImage;
    bool isBlinking;

    // Start is called before the first frame update
    void Start()
    {
        counter = counterMax;
        warningFillImage = WarningCanvas.GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        counter -= Time.deltaTime;
        warningFillImage.fillAmount = (counterMax - counter) / counterMax;
        if (counter <= 0)
        {
            StartCoroutine(WaitAndLoseChunks());
            counter = counterMax;
            currentCircle++;
            WarningCanvas.transform.localScale = new Vector3(canvasSizes[currentCircle], canvasSizes[currentCircle], 1);
            warningFillImage.fillAmount = 0;
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
            warningFillImage.enabled = !warningFillImage.enabled;
        }
        isBlinking = false;
    }

    IEnumerator WaitAndLoseChunks()
    {
        Transform currentCircleTr = GroundCircles[currentCircle].transform;
        currentCircleTr.gameObject.SetActive(true);
        int forCount = GroundCircles[currentCircle].transform.childCount;
        int nextColliderSize = currentCircle;
        MainColliderObject.transform.localScale = new Vector3(maincolliderSizes[nextColliderSize], maincolliderSizes[nextColliderSize], 1);
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
