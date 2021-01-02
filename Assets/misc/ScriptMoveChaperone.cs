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
    private Vector3 savedHdmWorldPos;

    // Start is called before the first frame update
    void Start()
    {
        //this.posLeftHandRelativeToChaperoneAtStart = MyInverseTransformPoint( this.transform,this.gameObjLeftHand.transform.position );
        this.posLeftHandRelativeToChaperoneAtStart = this.gameObjLeftHand.transform.localPosition;
        //this.quatLeftHandRelativeToChaperoneAtStart =  Quaternion.Inverse( this.transform.rotation ) * this.gameObjLeftHand.transform.rotation;
        this.quatLeftHandRelativeToChaperoneAtStart = this.gameObjLeftHand.transform.localRotation;
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
        //Vector3 currentRelativePosDiffLeftHandChaperone = MyInverseTransformPoint( this.transform,this.gameObjLeftHand.transform.position );
        //Quaternion currentRelativeQuatDiffLeftHandChaperone = Quaternion.Inverse( this.transform.rotation ) * this.gameObjLeftHand.transform.rotation;
        //Quaternion currentRelativeQuatDiffLeftHandChaperone = this.gameObjLeftHand.transform.localRotation;

        // először kiszámolom a chaperone-hoz viszonyított rotációt
        //Quaternion quatTmp1 = Quaternion.Inverse( currentRelativeQuatDiffLeftHandChaperone ) * this.quatLeftHandRelativeToChaperoneAtStart;
        Quaternion quatTmp1 = this.gameObjLeftHand.transform.localRotation;
        // majd ehhez képest a deltaTime rotiációt
        Quaternion quatTmp2 = Quaternion.Lerp( Quaternion.identity,quatTmp1,deltaTime );

        // ezután meg kell határozni azt a chaperone eltolást(transzlációt), amely a hmd-ben történő rotáció miatt éri a chaperone-t
        // itt érdekes módon worldCoord-ot kell használni, a local rotációval számolva, különben elmászik...
        //Vector3 diffHmdChaperone = this.transform.position - this.gameObjHmd.transform.position;
        //Vector3 diffRotHmdChaperone = quatTmp2 * diffHmdChaperone - diffHmdChaperone;

        this.savedHdmWorldPos = this.gameObjHmd.transform.position;

        // végül módosítom a chaperone rotációját, és pozícióját
        this.transform.rotation *= quatTmp2;
        //this.transform.position += diffRotHmdChaperone;

        // kiszámolom az új rotációval a chp-ben a hmd helyzetét és eltolom a chp-t úgy, hogy a hmd 1 helyben kell maradjon
        //Vector3 newHdmWorldPos = this.transform.position + this.transform.rotation * this.gameObjHmd.transform.localPosition;
        Vector3 newHdmWorldPos = this.gameObjHmd.transform.position;
        this.transform.position -= ( newHdmWorldPos - savedHdmWorldPos );

        // transzláció kezelés
        // először kiszámolom a chaperone-hoz viszonyított transzlációt, minusz a default érték
        Vector3 diffLeftHandChaperone = this.gameObjLeftHand.transform.localPosition - this.posLeftHandRelativeToChaperoneAtStart;
        Vector3 diffLeftHandChaperoneLerp = diffLeftHandChaperone * deltaTime;

        Vector3 vecAdd = this.transform.rotation * diffLeftHandChaperoneLerp;
        this.transform.position += vecAdd;
    }

    private Vector3 MyInverseTransformPoint( Transform transform,Vector3 worldCoordPos )
    {
        //return transform.InverseTransformVector( worldCoordPos );
        Vector3 diff = ( worldCoordPos - transform.position );
        return Quaternion.Inverse( transform.rotation ) * diff;
    }

	private void updateHUDPosInfo()
	{
        Vector3 pos1 = this.posLeftHandRelativeToChaperoneAtStart;
        Vector3 pos2 = MyInverseTransformPoint( this.transform,this.gameObjLeftHand.transform.position );
        Vector3 pos3 = ( this.transform.position - this.gameObjLeftHand.transform.position );

        this.indexFormat = 0;
        this.strFormat = "";
        addVector3( "position:{{{0},0:F2}} {{{1},0:F2}} {{{2},0:F2}}\n",this.transform.position );
        addVector3( "posOrigLeftHand:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",pos1 );
        addVector3( "diff1:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",pos2 );
        addVector3( "diff2:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",pos3 );

		gameObjHUD.text = string.Format( this.strFormat,this.objsFormat );
	}

    private int indexFormat = 0;
    private string strFormat;
    private object[] objsFormat = new object[100];

    private void addObj( string msg,object objVal )
    {
        this.strFormat += string.Format( msg,this.indexFormat );
        this.objsFormat[this.indexFormat] = objVal;
        this.indexFormat++;
    }
    private void addVector3( string msg,Vector3 vec )
    {
        this.strFormat += string.Format( msg,this.indexFormat,this.indexFormat+1,this.indexFormat+2 );
        //Debug.Log( this.strFormat );
        this.objsFormat[this.indexFormat] = vec.x;
        this.objsFormat[this.indexFormat+1] = vec.y;
        this.objsFormat[this.indexFormat+2] = vec.z;
        this.indexFormat += 3;
    }
}
