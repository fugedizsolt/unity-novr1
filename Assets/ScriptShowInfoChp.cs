using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptShowInfoChp : MonoBehaviour
{
    public GameObject gameObjLeftHand;
    public GameObject gameObjHmd;

    public Vector3 localPosHmd;
    public Vector3 localPosHand;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.localPosHmd = gameObjHmd.transform.localPosition;
        this.localPosHand = gameObjLeftHand.transform.localPosition;
    }
}
