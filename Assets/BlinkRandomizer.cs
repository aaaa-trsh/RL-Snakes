using UnityEngine;

public class BlinkRandomizer : MonoBehaviour
{
    public float delay;
    public float alpha;
    public float speed = 10;
    public float time;
    SpriteRenderer sr;
    SnakeTile tile;
    bool on;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        tile = GetComponentInParent<SnakeTile>();
        sr.color = new Color(1, 1, 1, 0);
        delay = Random.Range(0, 10);
    }
    void Update()
    {
        time += Time.deltaTime * speed;
        alpha = Mathf.Sin(delay + time); //Mathf.PerlinNoise((transform.position.x + time + delay) / 4, (transform.position.y + time + delay) / 4);

        sr.color = new Color(0, 0, 0, ((alpha + 1)/2) * 0.2f);
        //sr.color = Color.Lerp(sr.color, new Color(sr.color.r, sr.color.g, sr.color.b, 0), Time.deltaTime * 40);
    }
}
