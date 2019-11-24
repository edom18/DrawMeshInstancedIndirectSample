using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ParticleSystem : MonoBehaviour
{
    [SerializeField] private int _count = 10000;
    [SerializeField] private ComputeShader _computeShader = null;
    [SerializeField] private Mesh _particleMesh = null;
    [SerializeField] private MeshFilter _targetMeshFilter = null;
    [SerializeField] private Material _particleMat = null;

    private ComputeBuffer _particleBuffer = null;
    private ComputeBuffer _argBuffer = null;
    private uint[] _args = new uint[5] { 0, 0, 0, 0, 0, };

    private int _kernelId = 0;

    private struct Particle
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector4 color;
        public float scale;
    }

    private void Start()
    {
        _kernelId = _computeShader.FindKernel("ParticleMain");

        _particleBuffer = new ComputeBuffer(_count, Marshal.SizeOf(typeof(Particle)));

        Particle[] particles = new Particle[_count];

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();

        _targetMeshFilter.mesh.GetVertices(vertices);
        _targetMeshFilter.mesh.GetNormals(normals);

        for (int i = 0; i < vertices.Count; i++)
        {
            particles[i] = new Particle
            {
                position = vertices[i],
                normal = normals[i],
                color = Random.ColorHSV(),
                scale = Random.Range(0.01f, 0.05f),
            };
        }

        _particleBuffer.SetData(particles);

        int subMeshIndex = 0;

        // Indirect args.
        _args[0] = _targetMeshFilter.mesh.GetIndexCount(subMeshIndex);
        _args[1] = (uint)_count;
        _args[2] = _targetMeshFilter.mesh.GetIndexStart(subMeshIndex);
        _args[3] = _targetMeshFilter.mesh.GetBaseVertex(subMeshIndex);

        _argBuffer = new ComputeBuffer(1, sizeof(uint) * _args.Length, ComputeBufferType.IndirectArguments);
        _argBuffer.SetData(_args);
    }

    private void Update()
    {
        _computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        _computeShader.SetBuffer(_kernelId, "_ParticleBuffer", _particleBuffer);
        _computeShader.Dispatch(_kernelId, 8, 1, 1);

        _particleMat.SetBuffer("_ParticleBuffer", _particleBuffer);

        Graphics.DrawMeshInstancedIndirect(_particleMesh, 0, _particleMat, new Bounds(Vector3.zero, Vector3.one * 32f), _argBuffer);
    }

    private void OnDestroy()
    {
        _particleBuffer?.Release();
        _argBuffer?.Release();
    }
}
