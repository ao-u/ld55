using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (!(name.Contains("upgrade") && flipped))
            Director.choice = choice;
            Shake(4f);
        }

        if (name.Contains("upgrade"))
        {
            if (Director.playerCreatures.Count > choice - 10)
            {
                flipped = false;
            }
            else
            {
                flipped = true;
            }
        }

    }
    public bool flipped = false;
    void FixedUpdate()
    {
        Vector3 targetSize = baseSize;
        Vector3 targetRotation = new Vector3(0f, 45f, 0f);
        if (flipped) targetRotation = new Vector3(0f, 225f, 0f);
        if (name.Contains("upgrade"))
        {
            targetRotation += new Vector3(20f, 0f, 0f);
            if (flipped)
            {
                targetRotation -= new Vector3(40f, 0f, 0f);
            }
        }
        if (state == "hovered")
        {
            targetSize = baseSize * 1.1f;
        }
        if (SceneManager.GetActiveScene().name == "menu")
        {
            targetRotation = new Vector3(0f, 0f, 0f);

        }

        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, .3f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotation), .2f);
        float offset = (Mathf.Sin(Time.fixedTime * 2f) / 10f);
        float offset1 = (Mathf.Sin(Time.fixedTime * 3f) / 15f);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + offset1, transform.localEulerAngles.y, transform.localEulerAngles.z + offset);
    }
    public void Shake(float amount)
    {
        float div = 3f;
        transform.localEulerAngles += new Vector3(RandSign() * amount + Random.Range(-amount / div, amount / div), 0f, RandSign() * amount + Random.Range(-amount / div, amount / div));
    }
    public static float RandSign() { return Random.Range(0, 2) * 2 - 1; }
}
