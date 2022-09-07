using System.Collections;
using UnityEngine;

namespace Enemy.AI.NewCombat
{
    public abstract class BossCombat : ReBaseCombat
    {
        [SerializeField] 
        protected float pauseTime = 0.1f;
        
        protected abstract override void Update();

        protected abstract IEnumerator AttackingEvent1();
        protected abstract IEnumerator AttackingEvent2();
        protected abstract IEnumerator AttackingEvent3();

        protected IEnumerator AttackInterval(float time)
        {
            CanAttack = false;
            yield return new WaitForSeconds(time);
            CanAttack = true;
        }
    }
}