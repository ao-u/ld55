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
        upgradeprice = 1;
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

        hp = maxhp * 2;

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
        GameObject eyes = Instantiate(Resources.Load<GameObject>("prefabs/eyes/eyes" + Random.Range(1, 3)), transform.parent);

        //slightly randomize eye position and scale
        eyes.transform.position = new Vector3(eyes.transform.position.x, eyes.transform.position.y + Random.Range(-0.2f, 0f), eyes.transform.position.z + 0.8f);

        float eyex = Random.Range(-0.15f, 0.15f);
        float eyey = Random.Range(-0.15f, 0.15f);
        float eyez = Random.Range(-0.15f, 0.15f);
        foreach (Transform child in eyes.transform)
        {
            child.localScale = new Vector3(child.localScale.x + eyex, child.localScale.y + eyey, child.localScale.z + eyez);
        }
        eyes.transform.SetParent(transform, false);

        //randomize mouth
        GameObject mouth = Instantiate(Resources.Load<GameObject>("prefabs/mouth/mouth" + Random.Range(1, 7)), transform.parent);

        //slightly randomize mouth position and scale
        mouth.transform.position = new Vector3(mouth.transform.position.x, mouth.transform.position.y + Random.Range(-0.6f, -0.3f), mouth.transform.position.z + 0.9f);

        float mouthx = Random.Range(-0.15f, 0.15f);
        float mouthy = Random.Range(-0.15f, 0.15f);
        float mouthz = Random.Range(-0.01f, 0.01f);
        foreach (Transform child in mouth.transform)
        {
            child.localScale = new Vector3(child.localScale.x + mouthx, child.localScale.y + mouthy, child.localScale.z + mouthz);
        }
        mouth.transform.SetParent(transform, false);

        
        Color c;
        if (team == 0) c = Color.red;
        else c = Color.blue;

        float diff = .2f, diff2 = .4f;
        Color basediff = c + new Color(Random.Range(-diff2, diff2), Random.Range(-diff2, diff2), Random.Range(-diff2, diff2));

        body.GetComponentInChildren<MeshRenderer>().materials[0] = Resources.Load<Material>("materials/enemycolor");
        body.GetComponentInChildren<MeshRenderer>().materials[0].color = basediff;

        MeshRenderer[] armss = arms.GetComponentsInChildren<MeshRenderer>();
        
        armss[0].materials[0] = Resources.Load<Material>("materials/enemycolor");
        armss[0].materials[0].color = basediff + new Color(Random.Range(-diff, diff), Random.Range(-diff, diff), Random.Range(-diff, diff));
        armss[1].materials[0] = Resources.Load<Material>("materials/enemycolor");
        armss[1].materials[0].color = basediff + new Color(Random.Range(-diff, diff), Random.Range(-diff, diff), Random.Range(-diff, diff));


        nametag = Instantiate(Resources.Load<GameObject>("prefabs/NameTag"), Vector3.zero, Quaternion.identity, GameObject.Find("MainCanvas").transform);
        nametag.GetComponent<TextMeshProUGUI>().text = creaturename;
        nametag.GetComponent<TextMeshProUGUI>().color = Color.Lerp(c, Color.white, .3f);
        nametag.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        nametag.transform.position -= new Vector3(0, 150f, 0f);

        statpage = Instantiate(Resources.Load<GameObject>("prefabs/LogText"), Vector3.zero, Quaternion.identity, GameObject.Find("MainCanvas").transform);
        statpage.transform.localScale *= .6f;
        statpagestring =
            "MAX HP:\t" + string.Concat(Enumerable.Repeat("X", maxhp)) + "\n" +
            "SPEED:\t" + string.Concat(Enumerable.Repeat("X", speed)) + "\n" +
            "ATK SPD:\t" + string.Concat(Enumerable.Repeat("X", attackspeed));

        statpage.GetComponent<TextMeshProUGUI>().text = statpagestring;

        statpage.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        statpage.transform.position += new Vector3(-70f, 170f, 0f);

        FindNearestEnemy();
    }
    string statpagestring;
    public string state = "none";
    private void Update()
    {
        //Vector3 realpos = transform.position - new Vector3(0, 5f, 0f);
        nametag.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        nametag.transform.position -= new Vector3(0, 150f, 0f);

        statpage.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        statpage.transform.position += new Vector3(-70f, 170f, 0f);

        if (transform.position.y < -5f)
        {
            KillThis("the void", false);
        }
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        Quaternion standuprot = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, standuprot, 20f * Time.deltaTime);
        rb.AddRelativeForce(Vector3.forward * Time.deltaTime * 500f * speedasmult);
        rb.AddRelativeForce(new Vector3(Random.Range(-1f, 1f), 0f, 0f) * Time.deltaTime * 50f * speedasmult);
        float maxspeed = 10f * speedasmult;
        Vector3 r = rb.velocity;
        rb.velocity += new Vector3(0, -100f, 0f);
        rb.velocity = new Vector3(Mathf.Clamp(r.x, -maxspeed, maxspeed), Mathf.Clamp(r.y, -maxspeed * 10f, 2f), Mathf.Clamp(r.z, -maxspeed, maxspeed));

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
            if (t.childCount > 0)
            {
                foreach (Transform tt in t)
                {
                    tt.parent = transform;
                }
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
    public int upgradeprice;
    public void Upgrade()
    {
        int rng = Random.Range(0, 3);
        if (rng == 0)
        {
            maxhp++;
        }
        else if (rng == 1)
        {
            speed++;
        }
        else if (rng == 2)
        {
            attackspeed++;
        }

        statpagestring =
            "MAX HP:\t" + string.Concat(Enumerable.Repeat("X", maxhp)) + "\n" +
            "SPEED:\t" + string.Concat(Enumerable.Repeat("X", speed)) + "\n" +
            "ATK SPD:\t" + string.Concat(Enumerable.Repeat("X", attackspeed));

        statpage.GetComponent<TextMeshProUGUI>().text = statpagestring;
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.GetComponent<Creature>() != null)
        {
            Creature cc = c.gameObject.GetComponent<Creature>();
            if (cc.team != team)
            {
                //rb.AddForceAtPosition(-c.transform.forward * 300f, transform.position, ForceMode.Force);
                c.gameObject.GetComponent<Rigidbody>().velocity = -c.transform.forward * 10f;
                rb.AddForceAtPosition(c.transform.up * 400f, transform.position, ForceMode.Force);
                //rb.AddForceAtPosition(new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f)) * 800f , transform.position, ForceMode.Force);
                cc.TakeDamage(creaturename);
                FindNearestEnemy();
                
            }
            
        }
    }
    private void OnCollisionStay(Collision collision)
    {
    }
}
