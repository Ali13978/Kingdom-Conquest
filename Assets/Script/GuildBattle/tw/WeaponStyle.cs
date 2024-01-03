using System;
using UnityEngine;
using TinyWar;

/// <summary>
/// 武器攻击类型 
/// </summary>
public class WeaponStyle : MonoBehaviour {
    public virtual void Attack(Warrior enemy) { }
    public string ClipName;
    public Warrior Owner;
    public event AttackCompleteDelegate OnAttackComplete;

    protected void OnAttackCompleteHandle() {
        if (this.OnAttackComplete != null) {
            this.OnAttackComplete();
        }
    }
}

public delegate void AttackCompleteDelegate();
