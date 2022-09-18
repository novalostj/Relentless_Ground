using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Particles
{
    public enum ParticleEnum
    {
        DashSmoke,
        Blood,
        HitCWhite,
        Poof,
        DarkMagicAura,
        HitMiscBGravity,
        HitBOrange
    }
    
    [Serializable]
    public class ParticleSet
    {
        public string name;
        public GameObject particleGameObject;
        public ParticleEnum particleEnum;

        public ParticleSet(string name, GameObject particleGameObject, ParticleEnum particleEnum)
        {
            this.name = name;
            this.particleGameObject = particleGameObject;
            this.particleEnum = particleEnum;
        }
    }

    public class GlobalParticleManager : MonoBehaviour
    {
        public delegate void Play(ParticleEnum particleName, Vector3 position, float scale = 1, bool randomRotation = true);
        public static Play play;

        [SerializeField] private int onCreateAmountPerEach = 2;
        [SerializeField] private List<ParticleSet> inactiveParticleList;
        
        [SerializeField] private List<ParticleSet> allParticle;

        private void OnEnable()
        {
            play += PlayParticle;
        }

        private void OnDisable()
        {
            play -= PlayParticle;
        }

        private void Start()
        {
            CreatePool();
        }

        private void CreatePool()
        {
            foreach (var pSet in inactiveParticleList) Destroy(pSet.particleGameObject);

            inactiveParticleList = new();

            for (int i = 0; i < onCreateAmountPerEach; i++)
                foreach (var particleSet in allParticle)
                    inactiveParticleList.Add(CreateParticleSet(particleSet));
        }

        private ParticleSet CreateParticleSet(ParticleSet particleSet, bool activeStatus = false)
        {
            var pSet = new ParticleSet(particleSet.name, Instantiate(particleSet.particleGameObject, transform), particleSet.particleEnum);
            pSet.particleGameObject.SetActive(activeStatus);
            return pSet;
        }
        

        private void PlayParticle(ParticleEnum particleEnum, Vector3 position, float scale, bool randomRotation)
        {
            var particleSet = GetParticle(particleEnum, ref inactiveParticleList) ?? CreateParticleSet(GetParticle(particleEnum, ref allParticle));

            var particleTransform = particleSet.particleGameObject.transform;
            particleTransform.position = position;
            particleTransform.localScale = new Vector3(scale, scale, scale);

            if (randomRotation)
            {
                Vector3 randomXRotation = new (Random.Range(-180f, 180f), 0, 0);
                particleTransform.Rotate(randomXRotation);
            }
            
            particleSet.particleGameObject.SetActive(true);

            inactiveParticleList.Remove(particleSet);
            StartCoroutine(CheckDisabled(particleSet));
        }

        private ParticleSet GetParticle(ParticleEnum particleEnum, ref List<ParticleSet> particleSets)
        {
            ParticleSet getParticle = null;
            
            foreach (ParticleSet particleSet in particleSets.Where(particleSet => particleSet.particleEnum == particleEnum))
                getParticle = particleSet;

            return getParticle;
        }

        private IEnumerator CheckDisabled(ParticleSet particleSet)
        {
            while (particleSet.particleGameObject.activeInHierarchy) yield return new WaitForEndOfFrame();
            
            inactiveParticleList.Add(particleSet);
        }
    }
}