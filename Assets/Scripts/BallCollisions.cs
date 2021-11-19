using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisions : MonoBehaviour
{
    public GameObject AngryEmoji, LaughEmoji;
    public GameObject myController;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider != myController.GetComponent<Collider>())
            {
                Debug.Log("2154648");
                GameObject collisionObject = collision.gameObject;
                collisionObject.GetComponent<Rigidbody>().AddForce((collisionObject.transform.position - transform.position).normalized * 8 + new Vector3(0, Random.Range(5, 10), 0), ForceMode.Impulse);

                Instantiate(AngryEmoji, collisionObject.transform.position + new Vector3(0, 2, 0), Quaternion.identity);

                if (myController.GetComponentInChildren<PlayerController>())
                {
                    PlayerController pc = myController.GetComponentInChildren<PlayerController>();
                    Instantiate(LaughEmoji, pc.HeadCanvas.transform.position, Quaternion.identity);
                    GameManager.Instance.ShakeCamera(5f, .5f);
                }
                else if (myController.GetComponentInChildren<AIController>())
                {
                    AIController ai = myController.GetComponentInChildren<AIController>();
                    Instantiate(LaughEmoji, ai.HeadCanvas.transform.position, Quaternion.identity);
                }
            }            
        }
    }
}
