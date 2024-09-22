using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPos : MonoBehaviour
{
    public CharacterCombat characterCombat;

    public float attackDuration = 0.1f;

    private void Awake()
    {
        characterCombat = GetComponentInParent<CharacterCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == characterCombat.targetTag)
        {
            Debug.Log("Hit " + other.name);
            CharacterStats target = other.GetComponent<CharacterStats>();
            characterCombat.DealDamage(target);
        }
    }
}
