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

    private Vector3 currentTranslationAsVelocity;
    private Vector3 currentTranslationAcceleration;
    private Vector3 targetTranslationAcceleration;
    private Quaternion currentRotationAsAngleVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //this.posLeftHandRelativeToChaperoneAtStart = MyInverseTransformPoint( this.transform,this.gameObjLeftHand.transform.position );
        this.posLeftHandRelativeToChaperoneAtStart = this.gameObjLeftHand.transform.localPosition;
        //this.quatLeftHandRelativeToChaperoneAtStart =  Quaternion.Inverse( this.transform.rotation ) * this.gameObjLeftHand.transform.rotation;
        this.quatLeftHandRelativeToChaperoneAtStart = this.gameObjLeftHand.transform.localRotation;

        this.currentTranslationAsVelocity = Vector3.zero;
        this.currentTranslationAcceleration = Vector3.zero;
        this.targetTranslationAcceleration = Vector3.zero;
        this.currentRotationAsAngleVelocity = Quaternion.identity;
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

        // először kiszámolom a chaperone-hoz viszonyított rotációt
        Quaternion quatTmp1 = this.gameObjLeftHand.transform.localRotation;

        // beállítom a sebességet
        this.currentRotationAsAngleVelocity = Quaternion.Lerp( this.currentRotationAsAngleVelocity,quatTmp1,deltaTime );

        // majd ehhez képest a deltaTime rotiációt
        Quaternion quatTmp2 = Quaternion.Lerp( Quaternion.identity,this.currentRotationAsAngleVelocity,deltaTime );

        // ezután meg kell határozni azt a chaperone eltolást(transzlációt), amely a hmd-ben történő rotáció miatt éri a chaperone-t
        // el kell menteni a rotáció előtt a hmd pos-t
        Vector3 savedHdmWorldPos = this.gameObjHmd.transform.position;

        // végül módosítom a chaperone rotációját, és pozícióját
        Quaternion quatTmp3 = this.transform.rotation * quatTmp2;
        this.transform.rotation = quatTmp3.normalized;

        // kiszámolom az új rotációval a chp-ben a hmd helyzetét és eltolom a chp-t úgy, hogy a hmd 1 helyben kell maradjon
        // ez a két érték egyezik: this.gameObjHmd.transform.position = this.transform.position + this.transform.rotation * this.gameObjHmd.transform.localPosition
        Vector3 newHdmWorldPos = this.gameObjHmd.transform.position;
        this.transform.position -= ( newHdmWorldPos - savedHdmWorldPos );

        // transzláció kezelés
        // először kiszámolom a chaperone-hoz viszonyított transzlációt, minusz a default érték
        Vector3 diffLeftHandChaperone = this.gameObjLeftHand.transform.localPosition - this.posLeftHandRelativeToChaperoneAtStart;
        float diffLen = diffLeftHandChaperone.sqrMagnitude;
        if ( diffLen>8 )
            diffLen = 8;
        float accLen = Mathf.Pow( 2f,diffLen )-1f;
        this.targetTranslationAcceleration = diffLeftHandChaperone.normalized * accLen;
        Vector3 diffVelocity = this.targetTranslationAcceleration - this.currentTranslationAcceleration;
        Vector3 addVelocity = diffVelocity * deltaTime;
        if ( addVelocity.sqrMagnitude<0.01f )
            addVelocity = addVelocity.normalized * 0.01f;
        this.currentTranslationAcceleration += addVelocity;

        float currentAccelerationVectorLength = this.currentTranslationAcceleration.sqrMagnitude;
        float targetAccelerationVectorLength = this.targetTranslationAcceleration.sqrMagnitude;
        if ( currentAccelerationVectorLength>targetAccelerationVectorLength )
        {
            this.currentTranslationAcceleration = this.targetTranslationAcceleration;
            currentAccelerationVectorLength = targetAccelerationVectorLength;
        }

        if ( currentAccelerationVectorLength>4f )
        {
            this.currentTranslationAcceleration = this.currentTranslationAcceleration.normalized * 4f;
        }
        this.currentTranslationAsVelocity += this.currentTranslationAcceleration * (deltaTime/5f);
        this.transform.position += this.currentTranslationAsVelocity * (2f*deltaTime);

        //Vector3 diffLeftHandChaperoneLerp = diffLeftHandChaperone * deltaTime;
        //Vector3 vecAdd = this.transform.rotation * diffLeftHandChaperoneLerp;
        //this.transform.position += vecAdd;
    }

	private void updateHUDPosInfo()
	{
        Vector3 pos1 = this.posLeftHandRelativeToChaperoneAtStart;
        Vector3 pos2 = ( this.transform.position - this.gameObjLeftHand.transform.position );

        this.indexFormat = 0;
        this.strFormat = "";
        addVector3( "position:{{{0},0:F2}} {{{1},0:F2}} {{{2},0:F2}}\n",this.transform.position );
        addVector3( "posOrigLeftHand:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",pos1 );
        addVector3( "diff1:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",pos2 );
        addVector3( "velocity:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",this.currentTranslationAsVelocity );
        addVector3( "currentAcceleration:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",this.currentTranslationAcceleration );
        addVector3( "targetAcceleration:{{{0},0:F6}} {{{1},0:F6}} {{{2},0:F6}}\n",this.targetTranslationAcceleration );

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
