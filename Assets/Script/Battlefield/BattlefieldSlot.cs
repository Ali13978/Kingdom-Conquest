using UnityEngine;
using System.Collections;
using System;

public enum ArmyCategory
{
	LightCavalry = 1,
	Spear = 2,
	Swordsman = 3,
	Archers = 4,
	Catapults = 5,
};

public enum ArmyCampColor
{
	Blue = 0,
	Red = 1
};


public class BattlefieldSlot : IgnoreTimeScale {
	
	public int attackPower = 10;
	public int armor = 8;
	public int health = 100;
	public int ArmyRaidRound = 1;
	public int ArmyRaidDamage = 0;
	public int ArmyDamagePerRound = 0;

	public tk2dAnimatedSprite sprite = null;
	public UILabel ArmyDamageCol = null;
	public UISlider ArmyHealthCol = null;
	
	public float attackRange = 1f;
	public float speed = 12f;
	
	public int ArmyNumBefore = 0;
	public int ArmyNumAfter = 0;
	public int ArmyNumRecover = 0;
	public int ArmyPowerAfter = 0;
	
	public int heroHealth = 0;
	public int heroArmyType = 0;
	public int heroArmyLevel = 0;
	public int heroLevel = 0;
	public ulong heroID = 0;
	public string HeroName = "";
	
	public enum Direction
	{
		Left = 0,
		Right = 1
	};
	
	public Direction toward = Direction.Right;
	
	public enum Status
	{
		Attack = 0,
		Walk = 1,
		Dead = 2,
		//Paused = 3,
	};
	
	Status action = Status.Attack;
	
	bool hasDamaged = false;
	bool hasBomb = false;
	bool hasArrow = false;
	
	public ArmyCategory armyID = ArmyCategory.Swordsman;
	public ArmyCampColor campID = ArmyCampColor.Red;
	
	Transform myTrans;
	BattlefieldStorge mCScene = null;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (ArmyDamageCol != null)
		{
			ArmyDamageCol.enabled = false;
		}
		
		if (sprite != null)
		{
			sprite.animationEventDelegate = AnimationEventDelegate;
		}
		
		// 变化 ...
		myTrans = transform;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	public void init2()
	{
		if (sprite != null)
		{
			switch (armyID)
			{
			case ArmyCategory.Spear:
				speed = 20f;
				attackRange = 30f; break;
			case ArmyCategory.Swordsman:
				speed = 22f;
				attackRange = 20; break;
				//sprite.transform.localPosition += new Vector3(0,2,0);break;
			case ArmyCategory.LightCavalry:
				speed = 28f;
				attackRange = 32f; break;
			case ArmyCategory.Archers:
				speed = 24f;
				attackRange = 110f;
				sprite.transform.localPosition += new Vector3(0,-2,0);break;
			case ArmyCategory.Catapults:
				speed = 15f;
				attackRange = 130f; 
				sprite.transform.localPosition += new Vector3(0,10,0);break; 
			default: break;
			}
			
			SwitchingAnim(Status.Walk);
			action = Status.Walk;
			
			// 启动 TICK
			StopCoroutine("OnUpdateMove1");
			StartCoroutine("OnUpdateMove1", null);
		}
	}
	
	IEnumerator OnUpdateMove1()
	{
		const float delta = 0.04f;
		while (true)
		{
			if (action == Status.Walk) 
			{
				bool overgoal = false;
				if (toward == Direction.Left)
				{
					Vector3 to = myTrans.localPosition;
					float ox = to.x;
					to.x = -220f;
		
					if (Vector3.Distance(myTrans.localPosition,to) < attackRange)
					{
						overgoal = true;
					}
					else 
					{
						float mx = speed*delta;
						ox -= mx;
						
						if (ox < to.x) 
						{
							ox = to.x + attackRange;
							myTrans.localPosition = new Vector3(ox,to.y,to.z);	
							overgoal = true;
						}
						else 
						{	
							myTrans.localPosition -= new Vector3(mx,0f,0f);
						}
					}
				}
				else if (toward == Direction.Right)
				{
					Vector3 to = myTrans.localPosition;
					float ox = to.x;
					to.x = 220f;
					
					if (Vector3.Distance(myTrans.localPosition,to) < attackRange)
					{
						overgoal = true;
					}
					else 
					{
						float mx = speed*delta;
						ox += mx;
						
						if (ox > to.x)
						{
							ox = to.x - attackRange;
							myTrans.localPosition = new Vector3(ox,to.y,to.z);	
							overgoal = true;
						}
						else 
						{
							myTrans.localPosition += new Vector3(mx,0f,0f);
						}
					}
				}
				
				
				if (overgoal)
				{
					SwitchingAnim(Status.Attack);
					action = Status.Attack;
				}	
			}
			
			yield return new WaitForSeconds(0.04f);
		}
		
	}
	
	public void SetCScene(BattlefieldStorge obj)
	{
		this.mCScene = obj;
	}
	
	public void SetHealthAS(float ox)
	{
		if (ArmyHealthCol != null) 
		{
			Transform t = ArmyHealthCol.transform;
			t.localPosition += new Vector3(ox, 0f, 0f);
		}
	}
	
	void SwitchingAnim(Status targetId)
	{		
		string playClipId = "";
		if (campID == ArmyCampColor.Red) 
		{
			playClipId = "red";
		}
		else if (campID == ArmyCampColor.Blue) {
			playClipId = "blue";
		}
		
		if (targetId == Status.Dead)
		{
			playClipId = "anim";
		}
		
		// arms id
		switch (armyID)
		{
		case ArmyCategory.Swordsman:
			playClipId += "/bu"; break;
		case ArmyCategory.LightCavalry:
			playClipId += "/qi"; break;
		case ArmyCategory.Archers:
			playClipId += "/gong"; break;
		case ArmyCategory.Spear:
			playClipId += "/qiang"; break;
		case ArmyCategory.Catapults:
			playClipId += "/xie"; break;
		default: break;
		}
		
		if (targetId == Status.Attack)
		{
			playClipId += "/attack";
		}
		else if (targetId == Status.Walk)
		{
			playClipId += "/walk";
		}
		else if (targetId == Status.Dead)
		{
			playClipId += "/dead";
		}
	
		
		// Play clip id fini
		sprite.Play(playClipId);
	}

	// Animation event handler 
	void AnimationEventDelegate(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
	{	
		if (frame.eventInfo == "OnceDamage")
		{
			hasDamaged = true;
		}
		else if (frame.eventInfo == "OnceBomb")
		{
			hasBomb = true;
		}
		else if (frame.eventInfo == "OnceArrow")
		{
			hasArrow = true;
		}
		else if (frame.eventInfo == "OnceDead")
		{
			NGUITools.SetActive(gameObject, false);
		}
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		// Don't play animations when not in play mode
		if (!Application.isPlaying)
			return;
#endif
	}

	/// <summary>
	/// Enmeries the damage.
	/// </summary>
	/// <param name='target'>
	/// Target.
	/// </param>
	public void EnmeryDamage(BattlefieldSlot target)
	{
		if (action == Status.Walk)
		{		
			SwitchingAnim(Status.Attack);	
			action = Status.Attack;
		}
			
		if (action == Status.Attack)
		{
			if (hasDamaged == true)
			{
				hasDamaged = false;
				
				if (ArmyRaidRound> 0)
				{
					target.RoundAS(ArmyDamagePerRound);
					target.RoundAttack(ArmyDamagePerRound);
					target.RoundHealth1();
					
					ArmyRaidRound --;
				}
				else 
				{
					target.RoundAttack(0);
				}
			}
			
			if (hasBomb == true)
			{
				hasBomb = false;
				if (mCScene != null) {
					mCScene.BombGen(target.transform.localPosition, campID);
				}
			}
			
			if (hasArrow == true)
			{
				hasArrow = false;
				if (mCScene != null) {
					mCScene.ArrowGen(myTrans, campID);
				}
			}
			
			if (target.health <= 0)
			{	
				target.SwitchingAnim(Status.Dead);
				target.action = Status.Dead;
				target.health = 0;
				target.HiddenHealth();
				target.StopCoroutine("OnUpdateMove1");
			
				SwitchingAnim(Status.Walk);
				action = Status.Walk;
			}
		}
	}
	
	public void EnmeryTower(TowerHealth target)
	{
		if (action == Status.Walk)
		{		
			SwitchingAnim(Status.Attack);	
			action = Status.Attack;
		}
		
		if (action == Status.Attack)
		{
			if (hasDamaged == true)
			{
				hasDamaged = false;
				target.RoundAttack(ArmyPowerAfter);
			}
			
			if (hasBomb == true)
			{
				hasBomb = false;
				if (mCScene != null) 
				{
					Vector3 cc = Vector3.zero;
					Vector3 BombPos = myTrans.localPosition;
					if (campID == ArmyCampColor.Red) {
						cc = new Vector3(220, BombPos.y, BombPos.z); 
					}
					else if (campID == ArmyCampColor.Blue) {
						cc = new Vector3(-220, BombPos.y, BombPos.z);
					}
					
					mCScene.BombGen(cc, campID);
				}
			}
			
			if (hasArrow == true)
			{
				hasArrow = false;
				if (mCScene != null) 
				{
					mCScene.ArrowGen(myTrans, campID);
				}
			}
		}
	}
	
	public void RoundAS(int nDamage)
	{
		health = Mathf.Max(0, health - nDamage);
	}
	
	public void RoundAttack(int nDamage)
	{
		if (ArmyDamageCol == null) return;		
		ArmyDamageCol.enabled = true;
		
		ArmyDamageCol.text = "-" + nDamage.ToString();		
		TweenColor tween_color = ArmyDamageCol.GetComponent<TweenColor>();
		tween_color.Reset(); 
		tween_color.Play(true);
		
		TweenPosition tween_position = ArmyDamageCol.GetComponent<TweenPosition>();
		tween_position.Reset();
		tween_position.Play(true);
	}
	
	public void RoundHealth1()
	{
		if (ArmyHealthCol == null) return;
		float per = (float) this.health / (float) heroHealth;
		ArmyHealthCol.sliderValue = per;
	}
	
	public void HiddenHealth()
	{
		if (ArmyHealthCol != null) {
			NGUITools.SetActive(ArmyHealthCol.gameObject, false);
		}
	}

}
