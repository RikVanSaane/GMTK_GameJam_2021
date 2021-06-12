using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxMoveSpeed = 1;
    [SerializeField] private float pullBackStrength = 25;
    [SerializeField] private int fishRemaining = 3;
    [SerializeField] private int bearSpraysRemaining = 1;
    [SerializeField] private float fishThrowRange = 3f;

    [Header("References")]
    [SerializeField] private SpriteRenderer heldFishSR;
    [SerializeField] private SpriteRenderer heldSpraySR;
    [SerializeField] private GameObject fishPrefab;

    private TMP_Text fishText;
    private TMP_Text sprayText;

    private Rigidbody2D rb;
    private BearMovement bear;
    private bool beingPulled;
    private LineRenderer previewLine;
    private bool isDead;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bear = GameObject.Find("Bear").GetComponent<BearMovement>();
        previewLine = GetComponent<LineRenderer>();
        animator = GetComponent<Animator>();

        fishText = GameObject.Find("FishText").GetComponent<TMP_Text>();
        sprayText = GameObject.Find("SprayText").GetComponent<TMP_Text>();
        fishText.text = fishRemaining.ToString();
        sprayText.text = bearSpraysRemaining.ToString();
    }
    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleFish();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleSpray();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            UseItem();
        }

        //Draw preview line if holding fish
        if (heldFishSR.enabled)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            previewLine.SetPosition(0, transform.position);
            previewLine.SetPosition(1, mousePos);
            if ((mousePos - (Vector2)transform.position).magnitude > fishThrowRange)
            {
                previewLine.startColor = Color.red;
                previewLine.endColor = Color.red;
            }
            else
            {
                previewLine.startColor = Color.green;
                previewLine.endColor = Color.green;
            }
        }
        else if (heldSpraySR.enabled)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            previewLine.SetPosition(0, transform.position);
            previewLine.SetPosition(1, mousePos);
            if (!bear.GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                previewLine.startColor = Color.red;
                previewLine.endColor = Color.red;
            }
            else
            {
                previewLine.startColor = Color.green;
                previewLine.endColor = Color.green;
            }
        }

        //Cant move while being pulled
        if (beingPulled) return;

        if (Vector2.Distance(transform.position, bear.transform.position) > bear.distractionRange)
        {
            //Get pulled back to bear
            beingPulled = true;
            StartCoroutine(PullToBear());
            //rb.MovePosition(transform.position + -(transform.position - bear.transform.position) * Time.deltaTime * pullBackStrength);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal == 0 && vertical == 0)
        {
            //If isnt playing idle play idle
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) animator.Play("Idle");
            else return;
        }
        Vector2 inputVector = new Vector2(horizontal, vertical).normalized;


        //Maybe allow free movement while in range of bear, and use AddForce while rope is tight
        //TODO replace this with some sort of check that rope is tight, for when rope is around a corner

        rb.MovePosition((Vector2)transform.position + inputVector * Time.deltaTime * maxMoveSpeed);

        float angle = Vector3.Angle(inputVector, transform.right);
        float sign = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.Cross(inputVector, transform.right)));
        float signedAngle = angle * sign;
        float lookAngle = (signedAngle + 180) % 360;
        int index = Mathf.RoundToInt(lookAngle / 90);
        if (index == 4) index = 0;
        animator.Play("Walk" + index);

    }
    public void Die()
    {
        //TODO play die anim/sound and call GameManager.GameOver
        isDead = true;
        GameObject.Find("FinishPoint").GetComponent<FinishPoint>().ReloadLevel();
    }
    private void ToggleFish()
    {
        if (fishRemaining == 0) return;

        heldSpraySR.enabled = false;
        heldFishSR.enabled = !heldFishSR.enabled;
        previewLine.enabled = heldFishSR.enabled;
        //Call function on bear to look at fish
    }
    private void ToggleSpray()
    {
        heldFishSR.enabled = false;
        heldSpraySR.enabled = !heldSpraySR.enabled;
        previewLine.enabled = heldSpraySR.enabled;
    }
    private void UseItem()
    {
        if (heldSpraySR.enabled)
        {
            //Spray
            //Get direction
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Test if bear is in range
            if (bear.GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                //Move bear in direction
                Vector2 direction = mousePos - (Vector2)transform.position;
                bear.GetBearSprayed(direction);

                bearSpraysRemaining--;
                sprayText.text = bearSpraysRemaining.ToString();
                if (bearSpraysRemaining == 0)
                {
                    heldSpraySR.enabled = false;
                    previewLine.enabled = false;
                }
            }
        }
        else if (heldFishSR.enabled)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //TODO Do a check wether mousepos is an availbable position
            if ((mousePos - (Vector2)transform.position).magnitude > fishThrowRange) return;

            //Throw fish
            GameObject thrownFish = Instantiate(fishPrefab, transform.position, Quaternion.identity);

            thrownFish.GetComponent<BearInterest>().MoveToPos(mousePos);

            fishRemaining--;
            fishText.text = fishRemaining.ToString();
            if (fishRemaining == 0)
            {
                heldFishSR.enabled = false;
                previewLine.enabled = false;
            }

        }
    }
    private IEnumerator PullToBear()
    {
        //Apply spring like force to player to "Bounce" back to bear
        //rb.AddForce((transform.position - bear.transform.position) * -pullBackStrength, ForceMode2D.Force);

        //TODO Play "Flying" anim on player to show he is being pulled back towards bear
        animator.Play("Idle");

        while (Vector2.Distance(transform.position, bear.transform.position) > bear.distractionRange)
        {
            rb.velocity = (transform.position - bear.transform.position) * -pullBackStrength;
            yield return null;
        }
        //Move a bit more while in range
        yield return new WaitForSeconds(0.2f);

        //Once in range again have player land on ass
        rb.velocity = Vector2.zero;
        beingPulled = false;

        yield break;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fishThrowRange);
    }
}
