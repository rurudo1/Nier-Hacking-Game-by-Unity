using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z <= -5)
        {
            Debug.Log("Game Over");
            Destroy(gameObject);
        }
    }
}
