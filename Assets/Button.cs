using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Button : MonoBehaviour
{
    public string state = "none";
    public int choice;
    Vector3 baseSize;
    void Start()
    {
        baseSize = transform.localScale;
    }

    private void Update()
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
        if (state == "hovered" && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Director.choice = choice;
        }
    }
    public bool flipped = false;
    void FixedUpdate()
    {
        Vector3 targetSize = baseSize;
        Vector3 targetRotation = new Vector3(0f, 45f, 0f);
        if (flipped) targetRotation = new Vector3(0f, 225f, 0f);

        if (state == "hovered")
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
