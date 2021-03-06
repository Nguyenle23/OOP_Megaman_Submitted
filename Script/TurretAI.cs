﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    public int curHealth = 10;
    public float distanceToLeft, distanceToRight;
    public float distance;
    public float wakerange=15;
    public float shootinterval;
    public float bulletspeed = 6;
    public float bullettimer;

    public bool awake = false;
    public bool lookingRight = true;

    public GameObject bullet;
    public Transform target;
    public Animator anim;
    public Transform shootpointL, shootpointR;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Awake", awake);
        anim.SetBool("LookRight", lookingRight);

        RangeCheck();

        if (target.transform.position.x > transform.position.x)
        {
            lookingRight = true;
        }

        if (target.transform.position.x < transform.position.x)
        {
            lookingRight = false;
        }

        if (curHealth < 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(1);
        yield return 0;
    }

    void RangeCheck()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);
        distanceToLeft = Vector2.Distance(shootpointL.position, target.transform.position);
        distanceToRight = Vector2.Distance(shootpointR.position, target.transform.position);
        if (distance < wakerange)
        {
            awake = true;
            if (distanceToLeft < distanceToRight)
            {
                delay();
                Attack(false);
            }
            if (distanceToRight < distanceToLeft)
            {
                delay();
                Attack(true);
            }
        }

        if (distance > wakerange)
        {
            awake = false;
        }
    }

    public void Attack(bool attackright)
    {
        bullettimer += Time.deltaTime;

        if (bullettimer >= shootinterval)
        {
            Vector2 direction = target.transform.position - transform.position;
            direction.Normalize();

            if (attackright) //Condition to shoot (Bullet out)
            {
                GameObject bulletclone;
                bulletclone = Instantiate(bullet, shootpointR.transform.position, shootpointR.transform.rotation) as GameObject;
                bulletclone.GetComponent<Rigidbody2D>().velocity = direction * bulletspeed;

                bullettimer = 0;

                FindObjectOfType<AudioManager>().Play("TurretShoot");
            }

            if (!attackright) //Condition to shoot (Bullet out)
            {
                GameObject bulletclone;
                bulletclone = Instantiate(bullet, shootpointL.transform.position, shootpointL.transform.rotation) as GameObject;
                bulletclone.GetComponent<Rigidbody2D>().velocity = direction * bulletspeed;

                bullettimer = 0;

                FindObjectOfType<AudioManager>().Play("TurretShoot");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet")) {
            Destroy(col.gameObject);
            curHealth --;
            gameObject.GetComponent<Animation>().Play("TurretRedFlash");
            FindObjectOfType<AudioManager>().Play("TurretDamage");
            if (curHealth <= 0)
            {
                FindObjectOfType<AudioManager>().Play("TurretDestroy");
                Destroy(gameObject);
            }
        }
    }
}