using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public GameObject dogPrefab;
    public float dogRate;
    private float dogTimer=5;

    // Update is called once per frame
    void Update()
    {
        dogTimer += Time.deltaTime;
        // On spacebar press, send dog
        if (Input.GetKeyDown(KeyCode.Space) && dogTimer > 1f/dogRate)
        {
            Instantiate(dogPrefab, transform.position, dogPrefab.transform.rotation);
            dogTimer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
