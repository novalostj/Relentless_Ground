using System.Collections;
using Enemy.AI.NewCombat;
using UnityEngine;

namespace Enemy.AI.Combat
{
    
    public class RangedCombat : OldBaseCombat
    {
        [System.Serializable]
        public class RangedAttack : Attack
        {
            public Transform projectileSpawn;
            public GameObject projectile;
        }
        
        [Header("Ranged Variables"), SerializeField]
        private RangedAttack rangedAttack;

        protected override IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(rangedAttack.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            yield return new WaitForSeconds(rangedAttack.applyDamageOn[0]);
            LaunchProjectile(rangedAttack.projectile);
            rotateToTarget = false;
            
            //Attack Is Over
            yield return new WaitForSeconds(rangedAttack.howLongIsAttack - rangedAttack.applyDamageOn[0]);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }

        protected void LaunchProjectile(GameObject obj)
        {
            GameObject instance = Instantiate(obj, rangedAttack.projectileSpawn.position, transform.rotation);

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