using UnityEngine;
using System.Collections;

public class CellAttractor : MonoBehaviour {

    public bool bControllable = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (bControllable)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float speed = 2.0f;
            Vector3 pos = transform.position;
            pos.x += (horizontal * speed);
            pos.z += (vertical * speed);
            transform.position = pos;
        }
	}
}
