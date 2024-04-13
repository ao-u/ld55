using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Director : MonoBehaviour
{
    public static List<GameObject> creatures = new List<GameObject>();
    public static string state = "shop";
    GameObject endtext;
    bool endtextActive = false;
    Vector3 TargetCameraRot;
    void Start()
    {
        TargetCameraRot = new Vector3(90f, 0f, 0f);
        endtext = GameObject.Find("endtext");
        //endtext.GetComponent<TextMeshProUGUI>().text = "ltiearly";
        Application.targetFrameRate = 144;

        for (int i = 0; i < 100; i++)
        {
            Debug.Log(GetRandomName());
        }
        
        


        StartCoroutine(GameFlow());
    }
    void SpawnCreatures()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject g = Instantiate(Resources.Load<GameObject>("prefabs/Creature"), new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Random.rotation);
            g.GetComponent<Creature>().team = Random.Range(0, 3);
            creatures.Add(g);
        }
    }
    IEnumerator GameFlow()
    {
        while (true)
        {
            if (state == "fight")
            {
                if (creatures.Count == 0) SpawnCreatures();
                TargetCameraRot = new Vector3(90f, 0f, 0f);
                List<int> teams = new List<int>();
                foreach (GameObject g in creatures)
                {
                    if (!teams.Contains(g.GetComponent<Creature>().team))
                    {
                        teams.Add(g.GetComponent<Creature>().team);
                    }
                }
                if (teams.Count <= 1)
                {
                    endtextActive = true;
                    if (teams.Count == 0)
                    {
                        endtext.GetComponent<TextMeshProUGUI>().text = "Draw";
                    }
                    else
                    {
                        if (teams[0] == 0)
                        {
                            endtext.GetComponent<TextMeshProUGUI>().text = "Victory!";
                        }
                        else
                        {
                            endtext.GetComponent<TextMeshProUGUI>().text = "Defeated!";
                        }
                    }
                    state = "shop";
                    for (int i = 0; i < creatures.Count; i++)
                    {
                        Destroy(creatures[i]);
                        creatures.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (state == "shop")
            {
                endtextActive = false;
                TargetCameraRot = new Vector3(0f, 0f, 0f);
                yield return new WaitForSeconds(10f);
                state = "fight";
            }
            yield return new WaitForSeconds(1f);
        }
        
    }
    void Update()
    {
        
    }





    static List<string> prefix = new List<string>()
    {
        "Glor", "Glin", "Gla", "Gor", "Gno", "Gle", "Gloso", "Schpin", "Bin", "Kla", "Tro", "Tre"
    };
    static List<string> middle = new List<string>()
    {
        "ba", "bo", "ga", "go", "ba", "bo", "mo", "ma"
    };
    static List<string> suffix = new List<string>()
    {
        "bach", "dorf", "gle", "gak", "bud", "ble", "bulk", "dex", "trol", "reng", "lerd", "zle", "tle", "bok", "nok", "zork", "lerg", "lek", "wronk"
    };

    public static string GetRandomName()
    {
        string name = "";
        name += prefix[Random.Range(0, prefix.Count)];
        int middles = Random.Range(0, 2);
        for (int j = 0; j < middles; j++)
            name += middle[Random.Range(0, middle.Count)];
        name += suffix[Random.Range(0, suffix.Count)];
        return name;
    }
    public static List<GameObject> logs = new List<GameObject>();
    public static List<float> logtimers = new List<float>();
    public static void Log(string message)
    {
        GameObject c = GameObject.Find("Log");
        GameObject l = Instantiate(Resources.Load<GameObject>("prefabs/LogText"), c.transform.position, Quaternion.identity, c.transform);
        l.GetComponent<TextMeshProUGUI>().text = message;
        logs.Add(l);
        logtimers.Add(1f);
        for (int i = 0; i < logs.Count; i++)
        {
            logs[logs.Count - i - 1].transform.position = new Vector3(c.transform.position.x, c.transform.position.y - 40f * i, c.transform.position.z);
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < logs.Count; i++)
        {
            logtimers[i] -= .005f;
            logs[i].GetComponent<TextMeshProUGUI>().color = new Color(logs[i].GetComponent<TextMeshProUGUI>().color.r, logs[i].GetComponent<TextMeshProUGUI>().color.g, logs[i].GetComponent<TextMeshProUGUI>().color.b, logtimers[i]);
            if (logtimers[i] < 0f)
            {
                Destroy(logs[i]);
                logs.RemoveAt(i);
                logtimers.RemoveAt(i);
                i--;
            }
        }
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, TargetCameraRot, .2f);
        Color c = endtext.GetComponent<TextMeshProUGUI>().color;
        endtext.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, Mathf.Lerp(c.a, endtextActive ? 1f : 0f, .2f));
    }
}
