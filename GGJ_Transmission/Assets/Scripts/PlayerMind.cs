﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMind : MonoBehaviour
{
    [SerializeField] Rigidbody2D target;
    [SerializeField] PlayerBody bodyinfo;
    [SerializeField] GameObject soul;
    [SerializeField] bool possessed;

    bool jumpQueued;
    float jumpTimer;

    // Use this for initialization
    void Start()
    {
        jumpQueued = false;
        jumpTimer = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {
        //Center on dat bot!
        gameObject.transform.position = target.gameObject.transform.position;

        //Get dat inputs!
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        bool jumpBtn = Input.GetButtonDown("Jump");


        if (possessed) //If you are controlling a bot...
        {

            //Keep the soul centered
            Vector2 temp = new Vector2(transform.position.x, transform.position.y + bodyinfo.m_soulOffset_y);
            soul.transform.position = temp;

            Vector2 appliedForce = Vector2.zero;

            if (jumpTimer > 0)
            {
                jumpTimer -= Time.deltaTime;
            }
            Vector2 sidetest1 = transform.position;
            sidetest1.x -= 0.35f;
            Vector2 sidetest2 = transform.position;
            sidetest2.x += 0.35f;
            if (Physics2D.Raycast(gameObject.transform.position, new Vector2(0, -1f), 0.05f) || Physics2D.Raycast(sidetest2, new Vector2(0, -1f), 0.05f) || Physics2D.Raycast(sidetest1, new Vector2(0, -1f), 0.05f))
            {

                if (Input.GetButtonDown("Transmit"))
                {
                    possessed = false;
                    soul.SetActive(true);
                    jumpQueued = false;
                    jumpTimer = 0.0f;
                }
                if (!jumpQueued)
                {
                    appliedForce = new Vector2(hAxis * 10.0f, 0.0f);
                }
                if (jumpBtn)
                {
                    jumpQueued = true;
                    jumpTimer = bodyinfo.m_jumpDelay;
                }
            }
            else
            {
                appliedForce = new Vector2(hAxis * 5.0f, 0.0f);
            }
            if (jumpTimer <= 0.0f && jumpQueued)
            {
                jumpQueued = false;
                appliedForce.y = 5.0f * bodyinfo.m_jumpStrength / Time.fixedDeltaTime;
            }

            target.AddForce(appliedForce);
        }
        else
        {
            Vector3 centre = new Vector2(transform.position.x, transform.position.y + bodyinfo.m_soulOffset_y);

            if (Input.GetButtonDown("Cancel"))
            {
                possessed = true;
                soul.SetActive(false);
            }

            Vector2 vel = new Vector2(hAxis, vAxis);
            vel *= Time.fixedDeltaTime * 5.0f;
            soul.transform.Translate(vel);

            if ((soul.transform.position-centre).magnitude>3.5f) {
                Vector2 dir = soul.transform.position - centre;
                dir.Normalize();
                dir *= 3.5f;
                Vector2 temp = new Vector2(transform.position.x + dir.x, transform.position.y + dir.y + bodyinfo.m_soulOffset_y);
                soul.transform.position = temp;
            }
        }
    }
}