using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class ParticlesParent : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particles;

        public void Play()
        {
            foreach (var l_particle in particles)
            {
                l_particle.Play();
            }
        }

        public void Pause()
        {
            foreach (var l_particle in particles)
            {
                l_particle.Pause();
            }
        }

        public void Stop()
        {
            foreach (var l_particle in particles)
            {
                l_particle.Stop();
            }
        }
    }
}
