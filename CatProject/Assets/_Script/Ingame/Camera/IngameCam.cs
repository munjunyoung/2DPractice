using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCam : MonoBehaviour {

    [Header("Cam Target")]
    public Transform target;
    [Range(0, 5)]
    public float camSmoothing;
    [Range(0, 5)]
    public float camDistanceOffset;

	// Use this for initialization
	void Start () {
		
	}

    private void LateUpdate()
    {
        Vector3 targetCampos = target.position + (Vector3.back* camDistanceOffset);
        //transform.position = targetCampos;
        transform.position = Vector3.Lerp(transform.position, targetCampos, Time.deltaTime * camSmoothing);
    }
    
}
