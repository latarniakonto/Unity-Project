using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed = 3f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float xTranslation = Input.GetAxis("Horizontal");
        float yTranslation = Input.GetAxis("Vertical");
        xTranslation *= speed * Time.deltaTime;
        yTranslation *= speed * Time.deltaTime;

        transform.Translate(xTranslation, yTranslation, 0f);
    }
}
