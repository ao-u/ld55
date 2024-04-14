using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int team = 0;
    public string creaturename;
    Rigidbody rb;
    AudioSource aud;
    GameObject targetEnemy;
    public int hp;
    void Start()
    {
        hp = 5;
        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        aud.pitch = Random.Range(.7f, 1.3f);
        creaturename = Director.GetRandomName();

        //randomize body
        GameObject body = Instantiate(Resources.Load<GameObject>("prefabs/body/body" + Random.Range(1, 4)), transform.parent);
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
        GameObject eyes = Instantiate(Resources.Load<GameObject>("prefabs/eyes/eyes" + Random.Range(1, 2)), transform.parent);

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

        FindNearestEnemy();
    }
    public string state = "none";
    void FixedUpdate()
    {
        if (state == "fight")
        Movement();
        
    }
    
    void Movement()
    {
        if (Random.Range(0, 5) == 0) FindNearestEnemy();
        if (targetEnemy == null) FindNearestEnemy();
        Quaternion targetRotation = Quaternion.LookRotation(targetEnemy.transform.position - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(0f,  targetRotation.eulerAngles.y ,0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

        Quaternion standuprot = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, standuprot, 10f * Time.deltaTime);
        rb.AddRelativeForce(Vector3.forward * Time.deltaTime * 1000f);

        float maxspeed = 10f;
        Vector3 r = rb.velocity;
        rb.velocity = new Vector3(Mathf.Clamp(r.x, -maxspeed, maxspeed), Mathf.Clamp(r.y, -maxspeed * 2f, 0f), Mathf.Clamp(r.z, -maxspeed, maxspeed));

        if (invincibletimer > 0f) invincibletimer -= .02f;
        invincible = invincibletimer > 0f;
    }
    void FindNearestEnemy()
    {
        float lowestDistance = 99999f;
        targetEnemy = gameObject;
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
    public void TakeDamage(string killer)
    {
        if (invincibletimer < 0f)
        {
            aud.PlayOneShot(Resources.Load<AudioClip>("audio/sound" + Random.Range(1, 6)));
            hp--;
            invincibletimer = .5f;
            if (hp <= 0)
            {
                
                for (int i = 0; i < Director.allCreatures.Count; i++)
                {
                    if (Director.allCreatures[i] == gameObject)
                    {
                        Director.allCreatures.RemoveAt(i);
                        break;
                    }
                }
                Director.Log(creaturename + " was killed by " + killer, 0);
                Director.Log("+1 G", 1);
                foreach (Transform t in transform)
                {
                    t.AddComponent<Rigidbody>();
                    float diff = 2f;
                    t.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-diff, diff), UnityEngine.Random.Range(-diff, diff), UnityEngine.Random.Range(-diff, diff));
                    t.AddComponent<Limbs>();
                    t.parent = null;
                }
                Destroy(gameObject);
            }
        }
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
