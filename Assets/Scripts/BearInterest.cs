using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearInterest : MonoBehaviour
{
    public bool isScary;   
    public int interest = 1;
    public bool isInActive;

    [SerializeField] private float speed = 5;

    public void MoveToPos(Vector2 position)
    {
        isInActive = true;
        StartCoroutine(MoveToPosition(position));
    }
    private IEnumerator MoveToPosition(Vector2 position)
    {
        while(((Vector2)transform.position - position).sqrMagnitude > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
            yield return null;
        }
        isInActive = false;
    }
}
