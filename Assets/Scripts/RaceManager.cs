using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private int _gateCount = 30;
        private KartController[] _karts;
        public static RaceManager Instance;

        public RaceManager()
        {
            Instance = this;
        }
        
        public int GateCount => _gateCount;

        private void Start()
        {
            _karts = GameObject.FindObjectsByType<KartController>(sortMode: FindObjectsSortMode.None);
        }

        public void Reset()
        {
            foreach (var kart in _karts)
            {
                kart.Reset();
            }
        }
    }
}