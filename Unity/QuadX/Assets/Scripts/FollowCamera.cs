using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

    QuadMain quad;
    Vector3 offset;
    GameObject quadObj;

	void Awake()
    {
        quadObj = GameObject.Find("quad");
    }

	void Start () {
        quad = quadObj.GetComponent<QuadMain>();
        offset = transform.position - quad.body.transform.position;
	}
	
	void LateUpdate () {
        transform.position = quad.body.transform.position + offset;
    }
}
