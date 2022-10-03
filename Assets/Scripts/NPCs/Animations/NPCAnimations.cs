﻿using UnityEngine;

namespace NPCs
{
    public abstract class NPCAnimations : MonoBehaviour
    {
        [SerializeField] protected Animator GFXAnimator;

        public abstract void PlayStartInfection();

        public abstract void PlayDeath(LivingNPC.DeathCauses cause);

        public abstract void PlayInteractAnimation(Vector3 target);
    }
}