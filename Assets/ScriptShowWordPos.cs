using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptShowWordPos : MonoBehaviour
{
    public Vector3 worldPos;
    public Vector3 localPos;
    public Vector3 localRotEuler;
    public Vector3 invParent000;
    public Quaternion worldRotation;
    public Quaternion localRotation;


    // Start is called before the first frame update
    void Start()
    {
        this.worldPos = this.transform.position;
        this.localPos = this.transform.localPosition;
        this.worldRotation = this.transform.rotation;
        this.localRotation = this.transform.localRotation;
        this.localRotEuler = this.transform.localRotation.eulerAngles;
        this.invParent000 = this.transform.InverseTransformPoint( 0f,0f,0f );
    }

    // Update is called once per frame
    void Update()
    {
        this.worldPos = this.transform.position;
        this.localPos = this.transform.localPosition;
        this.worldRotation = this.transform.rotation;
        this.localRotation = this.transform.localRotation;
        this.localRotEuler = this.transform.localRotation.eulerAngles;
        this.invParent000 = this.transform.InverseTransformPoint( 0f,0f,0f );
    }
}
