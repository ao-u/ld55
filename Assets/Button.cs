using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public string state = "none";
    Vector3 baseSize;
    void Start()
    {
        baseSize = transform.localScale;
    }

    void FixedUpdate()
    {
        state = "none";
        Ray r = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
        {
            if (hit.transform.gameObject == gameObject)
            {
                state = "hovered";
            }
        }




        Vector3 targetRotation = Vector3.zero;
        Vector3 targetSize = baseSize;
        if (state == "none")
        {
            targetRotation = Vector3.zero;
        }
        else if (state == "hovered")
        {
            targetSize = baseSize * 1.1f;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, .3f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotation), .1f);
        float offset = (Mathf.Sin(Time.fixedTime * 2f) / 10f);
        float offset1 = (Mathf.Sin(Time.fixedTime * 3f) / 15f);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + offset1, transform.localEulerAngles.y, transform.localEulerAngles.z + offset);
    }
}