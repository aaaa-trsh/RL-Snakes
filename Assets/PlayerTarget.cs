using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    public SnakeController snake;
    void Start()
    {
        Invoke("Step", 0.01f);
    }

    void Step() 
    {
        transform.position = snake.BodyLocs[0] + snake.input * 5;
        Invoke("Step", 0.01f);
    }
}
