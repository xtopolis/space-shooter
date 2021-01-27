using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private float _duration = 30f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, _duration);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Slerp(transform.localScale, new Vector3(0.25f, 0.25f, 0.25f), (1f * Time.deltaTime) / _duration);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // You can't eat asteroids, it would break the game
        if (collision.CompareTag("Asteroid") == true)
            return;

        collision.transform.position = Vector3.Lerp(collision.transform.position, transform.position, 1.5f * Time.deltaTime);
        float dist = Vector3.Distance(collision.transform.position, transform.position);

        // Affect player movement (above), but don't kill them
        if (collision.CompareTag("Player") == true)
            return;

        if (dist < 2)
        {
            collision.transform.position = Vector3.Lerp(collision.transform.position, transform.position, 2.5f * Time.deltaTime);
            collision.transform.localScale = Vector3.Slerp(collision.transform.localScale, new Vector3(0.25f, 0.25f, 0.25f), 1.5f * Time.deltaTime);
            Destroy(collision.gameObject, 2f);
        }
    }
}
