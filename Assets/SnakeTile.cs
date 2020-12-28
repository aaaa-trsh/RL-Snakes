using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeTile : MonoBehaviour
{
    public bool on;
    bool prevState;
    public Color onCol, offCol;
    public float lerpTime;
    SpriteRenderer sr;
    Animator anim;

    void Start()
    {
        //anim = GetComponent<Animator>();
        //anim.speed = 0;
        //Invoke("Intro", Random.Range(0, .6f));
        sr = GetComponent<SpriteRenderer>();
    }

    void Intro()
    {
        //anim.speed = Random.Range(0.7f, 2);
    }
    void Update()
    {
        sr.color = on ? onCol : offCol;
        //sr.color = on ? onCol : offCol;
        //if (prevState != on)
        //    anim.Play("ChangeState");
    }
}
