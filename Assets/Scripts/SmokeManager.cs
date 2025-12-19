using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    // Singleton manager for GPU instancing 3d smoke puffs
    public class SmokeManager : MonoBehaviour
    {
        [SerializeField] private GameObject _smokePrefab;
        [SerializeField] private AnimationCurve _scaleCurve;
        [SerializeField] private float _boomTime = 3;
        
        private const int MAX_INSANCES = 1000;
        private SmokeInstance[] _instances = new SmokeInstance[MAX_INSANCES];
        private Matrix4x4[] _matrices = new Matrix4x4[MAX_INSANCES];
        private Mesh _mesh;
        private Material _material;
        private int _instanceCount = 0;
        private float _tickCount = 0f;
        public static SmokeManager Instance { get; private set; }

        public SmokeManager()
        {
            Instance = this;
        }

        private void Start()
        {
            _mesh = _smokePrefab.GetComponent<MeshFilter>().sharedMesh;
            _material = _smokePrefab.GetComponent<MeshRenderer>().sharedMaterial;

            for (int i = 0; i < MAX_INSANCES; ++i)
            {
                _instances[i] = new SmokeInstance();
                _matrices[i] = Matrix4x4.identity;
            }
        }

        public void CreateSmoke(Vector3 pos, float size)
        {
            _instances[_instanceCount].Position = pos;
            _instances[_instanceCount].OrigScale = size;
            _instances[_instanceCount].Time = _boomTime + Random.Range(-0.2f, 0.2f);
            _instanceCount++;
            if (_instanceCount >= MAX_INSANCES)
                _instanceCount = MAX_INSANCES - 1;
        }
        
        private void Update()
        {
            _tickCount += UnityEngine.Time.deltaTime;
            UpdateBooms();
            RenderBooms();
        }
        
        private void KillBoom(int i)
        {
            var lastIndex = _instanceCount - 1;
            if (i >= lastIndex)
            {
                _instanceCount--;
                return; // No need to do anything if we're removing the last instance
            }
            // copy the last instance values to the current index
            var src = _instances[lastIndex];
            var dest = _instances[i];

            dest.Position = src.Position;
            dest.OrigScale = src.OrigScale;
            dest.Scale = src.Scale;
            dest.Time = src.Time;
        
            _instanceCount--;
        }
        
        private void UpdateBooms()
        {
            int i = 0;
            while (i < _instanceCount)
            {
                var boom = _instances[i];
                boom.Time -= UnityEngine.Time.deltaTime;
                if (boom.Time < 0)
                {
                    KillBoom(i);
                }
                else
                {
                    float scale = _scaleCurve.Evaluate(boom.Time / _boomTime) * boom.OrigScale;
                    boom.Scale = new Vector3(scale, scale, scale);
                    boom.Position += new Vector3(0, scale * 0.25f * UnityEngine.Time.deltaTime, 0);
                
                    var time = _tickCount / boom.Time;
                
                    i++;
                }
            }
        }
        
        private void RenderBooms()
        {
            for (int i = 0; i < _instanceCount; i++)
            {
                _matrices[i] = Matrix4x4.TRS(_instances[i].Position, Quaternion.identity, _instances[i].Scale);
            }

            Graphics.DrawMeshInstanced(_mesh, 0, _material, _matrices, _instanceCount);
        }
    }
}