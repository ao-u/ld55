using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Director : MonoBehaviour
{
    public static List<GameObject> allCreatures = new List<GameObject>();
    public static List<GameObject> playerCreatures = new List<GameObject>();
    public static string state = "shop";
    GameObject endtext;
    bool endtextActive = false;
    public static int gold = 0;
    void Start()
    {
        gold = 10;
        GameObject.Find("Money").GetComponent<TextMeshProUGUI>().text = gold + " G";
        endtext = GameObject.Find("endtext");
        Application.targetFrameRate = 144;

       
        
        


        StartCoroutine(GameFlow());
    }
    void SpawnCreatures()
    {
        for (int i = 0; i < playerCreatures.Count; i++)
        {
            playerCreatures[i].transform.position = new Vector3(-7f, 1f, i * 1.5f);
            allCreatures.Add(playerCreatures[i]);
            playerCreatures[i].GetComponent<Creature>().state = "fight";
        }


        for (int i = 0; i < 4; i++)
        {
            GameObject g = Instantiate(Resources.Load<GameObject>("prefabs/Creature"), new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Random.rotation);
            g.transform.position = new Vector3(7f, 1f, i * 1.5f);
            g.GetComponent<Creature>().team = 1;
            g.GetComponent<Creature>().state = "fight";
            allCreatures.Add(g);
        }
    }
    public static int choice = 0;
    IEnumerator GameFlow()
    {
        int summonprice = 1;
        while (true)
        {
            if (state == "fight")
            {
                if (allCreatures.Count == 0) SpawnCreatures();
                List<int> teams = new List<int>();
                foreach (GameObject g in allCreatures)
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
                    
                    
                    
                    for (int i = 0; i < playerCreatures.Count; i++) {
                        playerCreatures[i].transform.position = GameObject.Find("place" + i).transform.position;
                        playerCreatures[i].transform.position += new Vector3(0f, 1f, 0f);
                        playerCreatures[i].GetComponent<Creature>().state = "standstill";
                    }
                    for (int i = 0; i < allCreatures.Count; i++)
                    {
                        if (allCreatures[i].GetComponent<Creature>().team != 0)
                        {
                            allCreatures[i].GetComponent<Creature>().KillThis("", true);
                            Debug.Log("killed enemy!");
                        }
                    }

                    yield return new WaitForSeconds(2f);
                    choice = 0;
                    summonprice = 1;
                    state = "shop";
                }
            }
            else if (state == "shop")
            {
                
                
                GameObject.Find("summonbutton").transform.Find("Canvas").Find("goldtext").GetComponent<TextMeshProUGUI>().text = summonprice + " G";



                endtextActive = false;
                yield return new WaitUntil(() => choice != 0);
                if (choice == 1 && playerCreatures.Count < 4 && gold >= summonprice)
                {
                    Director.Log("-" + summonprice + " G", -summonprice);
                    summonprice++;
                    GameObject g = Instantiate(Resources.Load<GameObject>("prefabs/Creature"), new Vector3(0f, -100f, 0f), Random.rotation);
                    playerCreatures.Add(g);
                    g.transform.position = GameObject.Find("place" + (playerCreatures.Count - 1)).transform.position;
                    g.transform.position += new Vector3(0f, 1f, 0f);
                    g.GetComponent<Creature>().team = 0;
                    g.GetComponent<Creature>().state = "standstill";
                }
                else if (choice == 2)
                {
                    state = "fight";
                    yield return new WaitForSeconds(1f);
                }
                choice = 0;
                
            }
            yield return new WaitForFixedUpdate();

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
        //for (int j = 0; j < middles; j++)
            //name += middle[Random.Range(0, middle.Count)];
        name += suffix[Random.Range(0, suffix.Count)];
        return name;
    }
    public static List<GameObject> logs = new List<GameObject>();
    public static List<float> logtimers = new List<float>();
    public static List<GameObject> moneylogs = new List<GameObject>();
    public static List<float> moneylogtimers = new List<float>();
    public static void Log(string message, int golddiff)
    {
        if (golddiff != 0)
        {
            GameObject c = GameObject.Find("MoneyLog");
            GameObject l = Instantiate(Resources.Load<GameObject>("prefabs/LogText"), c.transform.position, Quaternion.identity, c.transform);
            l.GetComponent<TextMeshProUGUI>().text = message;
            l.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, .5f);
            moneylogs.Add(l);
            moneylogtimers.Add(1f);
            for (int i = 0; i < moneylogs.Count; i++)
            {
                moneylogs[moneylogs.Count - i - 1].transform.position = new Vector3(c.transform.position.x, c.transform.position.y - 40f * i, c.transform.position.z);
            }
            gold += golddiff;
            GameObject.Find("Money").GetComponent<TextMeshProUGUI>().text = gold + " G";
        }
        else
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
        for (int i = 0; i < moneylogs.Count; i++)
        {
            moneylogtimers[i] -= .03f;
            moneylogs[i].GetComponent<TextMeshProUGUI>().color = new Color(moneylogs[i].GetComponent<TextMeshProUGUI>().color.r, moneylogs[i].GetComponent<TextMeshProUGUI>().color.g, moneylogs[i].GetComponent<TextMeshProUGUI>().color.b, moneylogtimers[i]);
            if (moneylogtimers[i] < 0f)
            {
                Destroy(moneylogs[i]);
                moneylogs.RemoveAt(i);
                moneylogtimers.RemoveAt(i);
                i--;
            }
        }
        if (state == "fight")
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(45f, 0f, 0f), .1f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0f, 15f, -15f), .1f);
            transform.parent.Rotate(new Vector3(0f, .2f, 0f));
        }
        else if (state == "shop")
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(15f, 45f, 0f), .1f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-20f, 25f, 10f), .1f);
            transform.parent.localEulerAngles = Vector3.Lerp(transform.parent.localEulerAngles, new Vector3(0f, 0f, 0f), .1f);
        }
       
        Color c = endtext.GetComponent<TextMeshProUGUI>().color;
        endtext.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, Mathf.Lerp(c.a, endtextActive ? 1f : 0f, .2f));
    }
}
