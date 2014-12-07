using UnityEngine;
using System.Collections;

public class RakugakiControl : MonoBehaviour {

    public Camera mainCamera;
    public StrokeSeed seed;
    public GameObject brushPrefab;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            RaycastHit hitInfo;
            int layerMask = 1 << 9; // 9:rakugaki layer
            if (Physics.Raycast(ray, out hitInfo, 50.0f, layerMask))
            {
                // try adding metaball cell
                Vector3 pos = hitInfo.point;
                /*
                GameObject inst = (GameObject)Instantiate(brushPrefab);
                inst.transform.position = pos;

                inst.transform.parent = hitInfo.collider.transform;
                */
                if (seed.TryAddCell(pos, 0.5f) != null)
                {
                    //seed.
                }
            }
        }
	}
}
