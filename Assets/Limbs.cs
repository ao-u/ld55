using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limbs : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Death());
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(1f);
        GetComponentInChildren<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = false;
        for (int i =0; i < 100; i++)
        {
            transform.position -= new Vector3(0f, .05f, 0f);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
    void Update()
    {
        
    }
}
