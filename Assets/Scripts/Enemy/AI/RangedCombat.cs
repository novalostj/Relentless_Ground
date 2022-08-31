using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

namespace Enemy.AI
{
    public class RangedCombat : BaseCombat
    {
        [SerializeField] private Transform projectileSpawn;
        [SerializeField] protected GameObject projectile;

        protected override IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            yield return new WaitForSeconds(applyDamageOn);
            LaunchProjectile(projectile);
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(howLongIsAttack - applyDamageOn);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }

        protected void LaunchProjectile(GameObject obj)
        {
            GameObject instance = Instantiate(obj, projectileSpawn.position, transform.rotation);

            Destroy(instance, 5f);
        }

        protected override void Dead()
        {
            CanAttack = false;
            Vulnerable = false;
            IsAttacking = false;
            StopAllCoroutines();
        }
    }
}