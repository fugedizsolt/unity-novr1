using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMoveLeftHand : MonoBehaviour
{
    public GameObject gameObjChaperone;
    public GameObject gameObjHmd;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateWithOrigRotation();
        UpdateWithRelativeHandRotation();
        //UpdateWithHmdCenter();
    }

    void UpdateWithOrigRotation()
    {
        this.gameObjChaperone.transform.rotation = this.transform.rotation;
    }

    void UpdateWithRelativeHandRotation()
    {
        float deltaTime = Time.deltaTime;
        // először kiszámolom a chaperone-hoz viszonyított rotációt
        Quaternion quatTmp1 = Quaternion.Inverse( this.gameObjChaperone.transform.rotation ) * this.transform.rotation;
        // majd ehhez képest a deltaTime rotiációt
        Quaternion quatTmp2 = Quaternion.Lerp( Quaternion.identity,quatTmp1,deltaTime );

        // majd ezzel módosítom a chaperone rotációját
        this.gameObjChaperone.transform.rotation *= quatTmp2;
    }
}
