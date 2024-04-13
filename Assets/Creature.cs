using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int team = 0;
    public string creaturename;
    Rigidbody rb;
    GameObject targetEnemy;
    public int hp;
    void Start()
    {
        hp = 3;
        rb = GetComponent<Rigidbody>();
        creaturename = Director.GetRandomName();
        FindNearestEnemy();
    }
    


    void FixedUpdate()
    {
        Movement();
    }
    
    void Movement()
    {
        if (Random.Range(0, 5) == 0) FindNearestEnemy();
        if (targetEnemy == null) FindNearestEnemy();
        Quaternion targetRotation = Quaternion.LookRotation(targetEnemy.transform.position - transform.position);
        targetRotation = Quaternion.Euler(new Vector3(0f,  targetRotation.eulerAngles.y ,0f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        rb.AddRelativeForce(Vector3.forward * Time.deltaTime * 1000f);

        float maxspeed = 10f;
        Vector3 r = rb.velocity;
        rb.velocity = new Vector3(Mathf.Clamp(r.x, -maxspeed, maxspeed), Mathf.Clamp(r.y, -maxspeed * 2f, 0f), Mathf.Clamp(r.z, -maxspeed, maxspeed));
    }
    void FindNearestEnemy()
    {
        float lowestDistance = 99999f;
        targetEnemy = gameObject;
        foreach (GameObject g in Director.creatures)
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
    public void TakeDamage(string killer)
    {
        hp--;
        if (hp >= 0)
        {
            for (int i = 0; i < Director.creatures.Count; i++)
            {
                if (Director.creatures[i] == gameObject)
                {
                    Director.creatures.RemoveAt(i);
                    break;
                }
            }
            Director.Log(creaturename + " was killed by " + killer);
            Destroy(gameObject);
        }
        
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.GetComponent<Creature>() != null)
        {
            Creature cc = c.gameObject.GetComponent<Creature>();
            if (cc.team != team)
            {
                cc.TakeDamage(creaturename);
                FindNearestEnemy();
            }
            
        }
    }
}
