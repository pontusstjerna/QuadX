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
        Vector3 qPos = quad.body.transform.position;
        float rotY = Mathf.Deg2Rad*quad.body.transform.rotation.eulerAngles.y;
        float offsetLengthXZ = -Mathf.Sqrt(Mathf.Pow(offset.x, 2) + Mathf.Pow(offset.z, 2));
        transform.position = new Vector3(qPos.x + Mathf.Sin(rotY)*offsetLengthXZ, qPos.y + offset.y, qPos.z + Mathf.Cos(rotY) * offsetLengthXZ);
        transform.LookAt(qPos);
    }
}
