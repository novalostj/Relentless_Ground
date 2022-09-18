using System.Collections;
using UnityEngine;

namespace Enemy.AI.Combat
{
    public abstract class BossCombat : BaseCombat
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