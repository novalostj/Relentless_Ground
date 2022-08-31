using System.Collections;
using UnityEngine;

namespace Enemy.AI
{
    public class RangedSlingCombat : RangedCombat
    {
        [SerializeField] private float applySecondDamageOn = 0.6f;

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

            yield return new WaitForSeconds(applySecondDamageOn);
            LaunchProjectile(projectile);
            rotateToTarget = false;

            //Attack Is Over
            yield return new WaitForSeconds(howLongIsAttack - applyDamageOn - applySecondDamageOn);
            onAttackFinish?.Invoke();
            IsAttacking = false;
            cooldown = StartCoroutine(Cooldown());
        }
    }
}