using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptMoveChaperone : MonoBehaviour
{
    public GameObject gameObjLeftHand;
    public GameObject gameObjHmd;
    public Text gameObjHUD;

    private Vector3 posLeftHandRelativeToChaperoneAtStart;
    private Quaternion quatLeftHandRelativeToChaperoneAtStart;

    // Start is called before the first frame update
    void Start()
    {
        this.posLeftHandRelativeToChaperoneAtStart = this.transform.InverseTransformPoint( this.gameObjLeftHand.transform.position );
        this.quatLeftHandRelativeToChaperoneAtStart =  Quaternion.Inverse( this.transform.rotation ) * this.gameObjLeftHand.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWithHmdCenter1WithTranslation();
        updateHUDPosInfo();
    }

    private void UpdateWithHmdCenter1WithTranslation()
    {
        float deltaTime = Time.deltaTime;

        // először kiszámolom a chaperone-hoz relatív position-t és rotation-t
        Vector3 currentRelativePosDiffLeftHandChaperone = this.transform.InverseTransformPoint( this.gameObjLeftHand.transform.position );
        Quaternion currentRelativeQuatDiffLeftHandChaperone = Quaternion.Inverse( this.transform.rotation ) * this.gameObjLeftHand.transform.rotation;

        // először kiszámolom a chaperone-hoz viszonyított rotációt
        Quaternion quatTmp1 = Quaternion.Inverse( this.quatLeftHandRelativeToChaperoneAtStart ) * currentRelativeQuatDiffLeftHandChaperone;
        // majd ehhez képest a deltaTime rotiációt
        Quaternion quatTmp2 = Quaternion.Lerp( Quaternion.identity,quatTmp1,deltaTime );

        // ezután meg kell határozni azt a chaperone eltolást(transzlációt), amely a hmd-ben történő rotáció miatt éri a chaperone-t
        Vector3 diffHmdChaperone = this.transform.InverseTransformPoint( this.gameObjHmd.transform.position );
        Vector3 diffRotHmdChaperone = quatTmp2 * diffHmdChaperone - diffHmdChaperone;

        // transzláció kezelés
        // először kiszámolom a chaperone-hoz viszonyított transzlációt, minusz a default érték
        Vector3 diffLeftHandChaperone = currentRelativePosDiffLeftHandChaperone - this.posLeftHandRelativeToChaperoneAtStart;
        Vector3 diffLeftHandChaperoneLerp = diffLeftHandChaperone * deltaTime;

        // végül módosítom a chaperone rotációját, és pozícióját
        this.transform.rotation *= quatTmp2;
        this.transform.position += diffRotHmdChaperone;

        Vector3 vecAdd = this.transform.rotation * diffLeftHandChaperoneLerp;
        this.transform.position += vecAdd;
    }

	private void updateHUDPosInfo()
	{
        Vector3 pos1 = this.posLeftHandRelativeToChaperoneAtStart;
        Vector3 pos2 = this.posLeftHandRelativeToChaperoneAtStart;
        Vector3 pos3 = this.transform.InverseTransformPoint( this.gameObjLeftHand.transform.position );
        Vector3 pos4 = ( this.transform.position - this.gameObjLeftHand.transform.position );

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
