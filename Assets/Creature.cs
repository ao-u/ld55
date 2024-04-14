using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int team = 0;
    public string creaturename;
    Rigidbody rb;
    AudioSource aud;
    GameObject targetEnemy;
    
    GameObject nametag;
    GameObject statpage;
    //stats
    public int hp;

    public int maxhp;
    public int speed;
    public int attackspeed;

    void Start()
    {
        int totalstats = 10;
        int maxstatvalue = 5;
        maxhp = 0;
        speed = 0;
        attackspeed = 0;
        for (int i = 0; i < totalstats; i++)
        {
            int rng = Random.Range(0, 3);
            if (rng == 0 && maxhp < maxstatvalue)
            {
                maxhp++;
            }
            else if (rng == 1 && speed < maxstatvalue)
            {
                speed++;
            }
            else if (rng == 2 && attackspeed < maxstatvalue)
            {
                attackspeed++;
            }
            else i--;
        }

        Debug.Log(maxhp + " max hp " + speed + " speed " + attackspeed + " atk speed");

        hp = maxhp;

        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        aud.pitch = Random.Range(.7f, 1.3f);
        creaturename = Director.GetRandomName();



        //randomize body
        GameObject body = Instantiate(Resources.Load<GameObject>("prefabs/body/body" + Random.Range(1, 6)), transform.parent);
        body.transform.SetParent(transform, false);

        //randomize arms
        GameObject arms = Instantiate(Resources.Load<GameObject>("prefabs/arms/arms" + Random.Range(1, 4)), transform.parent);

        //slightly randomize arm position and scale
        arms.transform.position = new Vector3(arms.transform.position.x, arms.transform.position.y + Random.Range(-0.5f, 0.5f));
        float armx = Random.Range(-0.2f, 0.2f);
        float army = Random.Range(-0.2f, 0.2f);
        float armz = Random.Range(-0.1f, 0.1f);
        foreach (Transform child in arms.transform)
        {
            child.localScale = new Vector3(child.localScale.x + armx, child.localScale.y + army, child.localScale.z + armz);
        }
        arms.transform.SetParent(transform, false);

        //randomize eyes
        GameObject eyes = Instantiate(Resources.Load<GameObject>("prefabs/eyes/eyes1"), transform.parent);

        //slightly randomize eye position and scale
        eyes.transform.position = new Vector3(eyes.transform.position.x, eyes.transform.position.y + Random.Range(-0.2f, 0f));

        float eyex = Random.Range(-0.15f, 0.15f);
        float eyey = Random.Range(-0.15f, 0.15f);
        float eyez = Random.Range(-0.15f, 0.15f);
        foreach (Transform child in eyes.transform)
        {
            child.localScale = new Vector3(child.localScale.x + eyex, child.localScale.y + eyey, child.localScale.z + eyez);
        }
        eyes.transform.SetParent(transform, false);


        nametag = Instantiate(Resources.Load<GameObject>("prefabs/NameTag"), Vector3.zero, Quaternion.identity, GameObject.Find("MainCanvas").transform);
        nametag.GetComponent<TextMeshProUGUI>().text = creaturename;
        nametag.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        nametag.transform.position -= new Vector3(0, 90f, 0f);

        statpage = Instantiate(Resources.Load<GameObject>("prefabs/LogText"), Vector3.zero, Quaternion.identity, GameObject.Find("MainCanvas").transform);
       
        statpagestring =
            "MAX HP:\t" + string.Concat(Enumerable.Repeat("X", maxhp)) + "\n" +
            "SPEED:\t" + string.Concat(Enumerable.Repeat("X", speed)) + "\n" +
            "ATK SPD:\t" + string.Concat(Enumerable.Repeat("X", attackspeed));

        statpage.GetComponent<TextMeshProUGUI>().text = statpagestring;

        FindNearestEnemy();
    }
    string statpagestring;
    public string state = "none";
    private void Update()
    {
        //Vector3 realpos = transform.position - new Vector3(0, 5f, 0f);
        nametag.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        nametag.transform.position -= new Vector3(0, 90f, 0f);

        statpage.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        statpage.transform.position += new Vector3(-20f, 200f, 0f);
    }
    void FixedUpdate()
    {
        if (state == "fight")
        {
            Movement();
            statpage.GetComponent<TextMeshProUGUI>().color = new Color(0f, 0f, 0f, 0f);
        }
        
        if (state == "standstill")
        {
            rb.velocity = Vector3.zero;
            transform.localEulerAngles = new Vector3(0, 225f, 0f);
            statpage.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 1f);
        }
        
    }
    
    void Movement()
    {
        float speedasmult = ((speed - 1) / 2f) + 1f;
        //Debug.Log(speed + " SPEED " + speedasmult + " SPEED MULT");

        if (Random.Range(0, 5) == 0) FindNearestEnemy();
        if (targetEnemy == null) FindNearestEnemy();
        Quaternion targetRotation = Quaternion.LookRotation(targetEnemy.transform.position - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(0f,  targetRotation.eulerAngles.y ,0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);

        Quaternion standuprot = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, standuprot, 10f * Time.deltaTime);
        rb.AddRelativeForce(Vector3.forward * Time.deltaTime * 1000f * speedasmult);

        float maxspeed = 10f * speedasmult;
        Vector3 r = rb.velocity;
        rb.velocity = new Vector3(Mathf.Clamp(r.x, -maxspeed, maxspeed), Mathf.Clamp(r.y, -maxspeed * 2f, 0f), Mathf.Clamp(r.z, -maxspeed, maxspeed));

        if (invincibletimer > 0f) invincibletimer -= .02f;
        invincible = invincibletimer > 0f;
    }
    void FindNearestEnemy()
    {
        float lowestDistance = 99999f;
        targetEnemy = GameObject.Find("CameraHolder");
        foreach (GameObject g in Director.allCreatures)
        {
            if (g != gameObject && g.GetComponent<Creature>().team != team)
            {
                float d = Vector3.Distance(transform.position, g.transform.position);
                if (d < lowestDistance)
                {
                    lowestDistance = d;
                    targetEnemy = g;
                }
            }

        }
    }
    public bool invincible;
    float invincibletimer = -1f;
    public void TakeDamage(string attacker)
    {
        if (invincibletimer < 0f)
        {
            aud.PlayOneShot(Resources.Load<AudioClip>("audio/sound" + Random.Range(1, 6)));
            hp--;
            invincibletimer = .5f;
            if (hp <= 0)
            {
                KillThis(attacker, false);
            }
        }
    }
    public void KillThis(string killer, bool silent)
    {
        if (!silent)
        {
            Director.Log(creaturename + " was killed by " + killer, 0);
            Director.Log("+1 G", 1);
        }

        for (int i = 0; i < Director.allCreatures.Count; i++)
        {
            if (Director.allCreatures[i] == gameObject)
            {
                Director.allCreatures.RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < Director.playerCreatures.Count; i++)
        {
            if (Director.playerCreatures[i] == gameObject)
            {
                Director.playerCreatures.RemoveAt(i);
                break;
            }
        }


        foreach (Transform t in transform)
        {
            t.AddComponent<Rigidbody>();
            //t.AddComponent<BoxCOl>();
            float diff = 6f;
            t.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-diff, diff), UnityEngine.Random.Range(-diff, diff), UnityEngine.Random.Range(-diff, diff));
            t.AddComponent<Limbs>();
            t.parent = null;
        }
        Destroy(statpage);
        Destroy(nametag);
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.GetComponent<Creature>() != null)
        {
            Creature cc = c.gameObject.GetComponent<Creature>();
            if (cc.team != team)
            {
                rb.AddForceAtPosition(-transform.forward * 200f + transform.up * 100f, transform.position, ForceMode.Force);
                cc.TakeDamage(creaturename);
                FindNearestEnemy();
                
            }
            
        }
    }
}
