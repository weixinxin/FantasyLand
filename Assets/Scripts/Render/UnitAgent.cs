using UnityEngine;
using System.Collections;
using Spine.Unity;
using BattleSystem.ObjectModule;

public class UnitAgent : MonoBehaviour 
{
    public const string ani_attack1 = "attack1";
    public const string ani_attack2 = "attack2";
    public const string ani_cast = "cast";
    public const string ani_death = "death";
    public const string ani_hurt = "hurt";
    public const string ani_idle = "idle";
    public const string ani_run = "run";
    public const string ani_sing = "sing";
    public const string ani_stun = "stun";
    public const string ani_super_attack = "superattack";
    public const string ani_walk = "walk";


	// Use this for initialization
    [SerializeField]
    private GameObject Prefab;

    private SkeletonAnimation mAnimation;

    private Transform mModel;

    private UnitState mState;
    public int Level { get; protected set; }
    public int TemplateID { get; protected set; }
    public int CampID { get; protected set; }

    public int ID { get; protected set; }
    public Transform mTransform { get; protected set; }

    private UnitAgent AttackTarget;
    public void Init(int ID,int templateID, int campID, int level)
    {
        this.ID = ID;
        TemplateID = templateID;
        CampID = campID;
        Level = level;
    }
    
    void Awake()
    {
        mTransform = transform;
        CreateModel();
        if (mModel != null)
            mAnimation = mModel.GetComponent<SkeletonAnimation>();
        InternalPlayAnimation(ani_idle, false);
    }

    protected virtual void CreateModel()
    {
        GameObject obj = GameObject.Instantiate(Prefab);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        mModel = obj.transform;
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(mState == UnitState.kAttack)
        {
            if(AttackTarget != null)
            {
                var scale = mModel.localScale;
                if(mTransform.position.x < AttackTarget.mTransform.position.x)
                {
                    scale.x = -Mathf.Abs(scale.x);
                }
                else
                {
                    scale.x = Mathf.Abs(scale.x);
                }
                mModel.localScale = scale;
            }
        }
	}
    

    protected void InternalPlayAnimation(string name, bool isLoop, float speed = 1.0f)
    {
        if(mAnimation != null)
        {
            mAnimation.loop = isLoop;
            mAnimation.timeScale = speed;
            mAnimation.AnimationName = name;
        }
    }

    public void UpdatePosition(float x,float y,float z)
    {
        var scale = mModel.localScale;
        if (mTransform.position.x < x)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
        }
        mModel.localScale = scale;
        mTransform.position = new Vector3(x, y, z);
    }

    public void OnIdle()
    {
        mState = UnitState.kIdel;
        InternalPlayAnimation(ani_idle, true);
    }
    public void OnMove()
    {
        mState = UnitState.kMove;
        InternalPlayAnimation(ani_run, true);
    }

    public void OnAttack(UnitAgent target)
    {
        AttackTarget = target;
        mState = UnitState.kAttack;
        InternalPlayAnimation(ani_attack1, true);
    }
    public void OnDead()
    {
        InternalPlayAnimation(ani_death, false);
    }
}
