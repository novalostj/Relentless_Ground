using System.Collections;
using UnityEngine;

namespace Enemy.AI.Combat
{
    public class RangedSlingCombat : RangedCombat
    {
        [System.Serializable]
        public class RangedSling : RangedAttack
        {
            public float applySecondDamageOn = 0.6f;
        }
        
        [Header("Sling Variables"), SerializeField] 
        private RangedSling rangedSling;

        protected override IEnumerator AttackingEvent()
        {
            IsAttacking = true;
            CanAttack = false;
            onAttack?.Invoke();
            cHalt = StartCoroutine(baseAI.Halt(rangedSling.howLongIsAttack));
            rotateToTarget = true;
            
            //ApplyDamage
            yield return new WaitForSeconds(rangedSling.applyDamageOn[0]);
            LaunchProjectile(rangedSling.projectile);

            yield return new WaitForSeconds(rangedSling.applySecondDamageOn);
            LaunchProjectile(rangedSling.projectile);
            rotateToTarget = false;

            //Attack Is Over
            yield return new WaitForSeconds(rangedSling.howLongIsAttack - rangedSling.applyDamageOn[0] - rangedSling.applySecondDamageOn);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }
    }
}