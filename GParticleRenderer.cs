using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GParticleRenderer : MonoBehaviour {
    public GParticles particles;
    public Material material;
    public Mesh mesh;
    public Camera targetCamera;
    ComputeBuffer argsBuffer;

    private void Reset() {
        particles = GetComponent<GParticles>();
    }

    private void Start() {
        uint[] instancingArgs = {
            (uint)mesh.GetIndexCount(0),
            (uint)particles.maxParticles,
            (uint)mesh.GetIndexStart(0),
            (uint)mesh.GetBaseVertex(0),
            0 };
        argsBuffer = new ComputeBuffer(1, instancingArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(instancingArgs);

        material.SetBuffer("_Particles", particles.particleBuffer);
    }

    private void Update() {
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, particles.bounds, argsBuffer, layer: gameObject.layer, camera: targetCamera);
    }
    private void OnDestroy() {
        if(argsBuffer!=null)
            argsBuffer.Dispose();
    }
}
