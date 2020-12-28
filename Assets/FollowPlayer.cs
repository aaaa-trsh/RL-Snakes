using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float speed;
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime * speed);
    }
}
