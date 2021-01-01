using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptMoveLeftHand : MonoBehaviour
{
    public GameObject gameObjChaperone;
    public GameObject gameObjHmd;
    public Text gameObjHUD;

    private Vector3 posOrigLeftHand;
    private Vector3 posOrigLeftHand2;

    // Start is called before the first frame update
    void Start()
    {
        this.posOrigLeftHand = ( this.transform.position - this.gameObjChaperone.transform.position );
        this.posOrigLeftHand2 = this.gameObjChaperone.transform.InverseTransformPoint( this.transform.position );
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateWithOrigRotation();
        //UpdateWithRelativeHandRotation();
        //UpdateWithHmdCenter1();
        UpdateWithHmdCenter1WithTranslation();
        updateHUDPosInfo();
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

    private void UpdateWithHmdCenter1()
    {
        float deltaTime = Time.deltaTime;
        // először kiszámolom a chaperone-hoz viszonyított rotációt
        Quaternion quatTmp1 = Quaternion.Inverse( this.gameObjChaperone.transform.rotation ) * this.transform.rotation;
        // majd ehhez képest a deltaTime rotiációt
        Quaternion quatTmp2 = Quaternion.Lerp( Quaternion.identity,quatTmp1,deltaTime );

        // majd ezzel módosítom a chaperone rotációját
        this.gameObjChaperone.transform.rotation *= quatTmp2;

        // ezután meg kell határozni azt a chaperone eltolást(transzlációt), amely a hmd-ben történő rotáció miatt éri a chaperone-t
        Vector3 diffHmdChaperone = this.gameObjChaperone.transform.position - this.gameObjHmd.transform.position;
        Vector3 diffRotHmdChaperone = quatTmp2 * diffHmdChaperone - diffHmdChaperone;
        this.gameObjChaperone.transform.position += diffRotHmdChaperone;
    }

    private void UpdateWithHmdCenter1WithTranslation()
    {
        float deltaTime = Time.deltaTime;
        // először kiszámolom a chaperone-hoz viszonyított rotációt
        Quaternion quatTmp1 = Quaternion.Inverse( this.gameObjChaperone.transform.rotation ) * this.transform.rotation;
        // majd ehhez képest a deltaTime rotiációt
        Quaternion quatTmp2 = Quaternion.Lerp( Quaternion.identity,quatTmp1,deltaTime );

        // ezután meg kell határozni azt a chaperone eltolást(transzlációt), amely a hmd-ben történő rotáció miatt éri a chaperone-t
        Vector3 diffHmdChaperone = this.gameObjChaperone.transform.position - this.gameObjHmd.transform.position;
        Vector3 diffRotHmdChaperone = quatTmp2 * diffHmdChaperone - diffHmdChaperone;

        // transzláció kezelés
        // először kiszámolom a chaperone-hoz viszonyított transzlációt, minusz a default érték
        Vector3 diffLeftHandChaperone = this.gameObjChaperone.transform.InverseTransformPoint( this.transform.position ) - posOrigLeftHand;
        Vector3 diffLeftHandChaperoneLerp = diffLeftHandChaperone * deltaTime;

        // végül módosítom a chaperone rotációját, és pozícióját
        this.gameObjChaperone.transform.rotation *= quatTmp2;
        this.gameObjChaperone.transform.position += diffRotHmdChaperone;

        Vector3 vecAdd = this.gameObjChaperone.transform.rotation * diffLeftHandChaperoneLerp;
        this.gameObjChaperone.transform.position += vecAdd;
        //this.gameObjChaperone.transform.position += diffLeftHandChaperoneLerp;
    }

	private void updateHUDPosInfo()
	{
        Vector3 pos1 = this.posOrigLeftHand;
        Vector3 pos2 = this.posOrigLeftHand2;
        Vector3 pos3 = this.gameObjChaperone.transform.InverseTransformPoint( this.transform.position );
        Vector3 pos4 = ( this.transform.position - this.gameObjChaperone.transform.position );

		gameObjHUD.text = string.Format( 
			"counter:{0}\n" + 
			"position:{1,0:F2},{2,0:F2},{3,0:F2}\n" +
			"posOrigLeftHand: {4,0:F6} {5,0:F6} {6,0:F6}\n" +
            "posOrigLeftHand2: {7,0:F6} {8,0:F6} {9,0:F6}\n" +
            "diff1: {10,0:F6} {11,0:F6} {12,0:F6}\n" +
            "diff2: {13,0:F6} {14,0:F6} {15,0:F6}\n",
			0,
			transform.position.x,transform.position.y,transform.position.z,
            pos1.x,pos1.y,pos1.z,
            pos2.x,pos2.y,pos2.z,
            pos3.x,pos3.y,pos3.z,
            pos4.x,pos4.y,pos4.z );
	}
}
