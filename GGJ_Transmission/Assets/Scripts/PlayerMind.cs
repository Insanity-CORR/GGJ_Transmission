using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMind : MonoBehaviour
{
    [SerializeField] Rigidbody2D target;
    [SerializeField] PlayerBody bodyinfo;
    [SerializeField] GameObject soul;
    [SerializeField] bool possessed;
    float freezeTimer;
    bool jumpQueued;
    float jumpTimer;

    bool isElevator;

    bool jumping, falling, action;

    // Use this for initialization
    void Start()
    {
        jumpQueued = false;
        jumpTimer = 0.0f;
        isElevator = true;
        freezeTimer = 0;

        jumping = false;
        falling = false;
        action  = false;
    }

    // Update is called once per frame
    void Update()
    {

        bodyinfo.GetComponentInChildren<Animator>().SetBool("Deactivated", !possessed);
        bodyinfo.GetComponentInChildren<Animator>().SetBool("Jumping", jumping);
        bodyinfo.GetComponentInChildren<Animator>().SetBool("Falling", falling);

        if (freezeTimer > 0)
        {
            freezeTimer -= Time.deltaTime;
            return;
        }

        //Center on dat bot! and LERP!
        gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, target.gameObject.transform.position, Time.deltaTime*4);

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
                if (falling || jumping)
                {
                    falling = false;
                    jumping = false;
                }
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
                    jumping = true;
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
            {

                Vector2 t = new Vector2(target.velocity.x, 0);
                target.GetComponentInChildren<Animator>().SetBool("Walking", t.magnitude > 0);
                if (t.x > 0)
                    target.GetComponentInChildren<SpriteRenderer>().flipX = false;
                if (t.x < 0)
                    target.GetComponentInChildren<SpriteRenderer>().flipX = true;
                if (t.magnitude > bodyinfo.m_maxSpeed)
                {
                    t.Normalize();
                    t *= bodyinfo.m_maxSpeed;
                    t.y = target.velocity.y;
                    target.velocity = t;
                }
            }
            if (jumping && target.velocity.y < 0)
            {
                jumping = false;
                falling = true;
            }
        }
        else
        {
            Vector3 centre = new Vector2(transform.position.x, transform.position.y + bodyinfo.m_soulOffset_y);

            if (Input.GetButtonDown("Cancel"))
            {
                possessed = true;
                soul.SetActive(false);
            }

            if (Input.GetButtonDown("Transmit"))
            {
                RaycastHit2D hit;
                if (hit = Physics2D.Raycast(soul.transform.position, new Vector2(0, 0), 0))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        target = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                        bodyinfo = hit.collider.gameObject.GetComponent<PlayerBody>();
                        possessed = true;
                        soul.SetActive(false);
                        freezeTimer = bodyinfo.m_possessDelay;
                    }
                }
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
