using UnityEngine;

public class DirectionalSpriteSetter : MonoBehaviour
{
    public float lookAngle;
    public SpriteRenderer[] spriteRenderers;

    public DSSSetting updateType;
    [SerializeField] private Sprite[] sprites;
    private int[] directions;
    private Vector3 oldPos;

    private Animator animator;
    private bool usesAnim;

    private string animState = "Idle";
    private string oldAnimState;
    public enum DSSSetting
    {
        Look, Movement, Script, Manual
    }

    private void Start()
    {
        directions = new int[spriteRenderers.Length];
        animator = GetComponent<Animator>();
        if (animator) usesAnim = true;
    }
    private void Update()
    {
        if (updateType == DSSSetting.Look) LookBasedDirection();
        else if (updateType == DSSSetting.Movement) MovementBasedDirection();
        else if (updateType == DSSSetting.Script) ScriptBasedDirection();
    }
    private void LookBasedDirection()
    {
        Vector3 dir = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (lookAngle < 0)
        {
            lookAngle += 360;
        }
        //inverse because otherwise it wont work with same sprite array as movementbased
        lookAngle = 360 - lookAngle;

        ScriptBasedDirection();
    }
    private void MovementBasedDirection()
    {
        if ((transform.position - oldPos).sqrMagnitude < 1f * Time.deltaTime)
        {
            animState = "Idle";
        }
        else
        {
            animState = "Walk";
        }

        //Vector3 dir = transform.position - oldPos;
        //float angle360 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //angle360 += 180+Mathf.Abs(angle360) * System.Convert.ToInt32((angle360 < 0));

        //Vector3 dir = (oldPos - transform.position).normalized;
        //float angle180 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //float angle360 = angle180 + (540+angle180) * System.Convert.ToInt32(angle180 < 0);
        //Debug.Log(angle180 + ":180---360:"+ angle360);

        float angle = Vector3.Angle(transform.position - oldPos, transform.right);
        float sign = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.Cross(transform.position - oldPos, transform.right)));
        float signedAngle = angle * sign;
        lookAngle = (signedAngle + 180) % 360;

        //Update the sprites
        ScriptBasedDirection();

        oldPos = transform.position;
    }
    private void ScriptBasedDirection()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
            UpdateSprite(lookAngle, i);
    }
    public void UpdateSprite(float angle, int spriteRIndex)
    {
        int index = Mathf.RoundToInt(angle / 90);
        if (index == 4) index = 0;

        if (directions[spriteRIndex] != index || oldAnimState != animState)
        {
            if (usesAnim)
            {
                animator.Play(animState + index);
            }
            else
            {
                spriteRenderers[spriteRIndex].sprite = sprites[index];
            }
            directions[spriteRIndex] = index;
            oldAnimState = animState;
        }
    }
}
