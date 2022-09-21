using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GParticles : MonoBehaviour {

    public struct Particle {
        public Vector3 position;
        public Vector3 velocity;
        public Color color;
    }
    const int ELEMENT_SIZE = sizeof(float) * (3+3+4);

    public int maxParticles = 100;
    public ComputeShader shader;
    public bool computeInit = false;
    public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 10);

    Particle[] particles;

    [HideInInspector] public ComputeBuffer particleBuffer;
    [HideInInspector] public int initKernel;
    [HideInInspector] public int solveKernel;
    [HideInInspector] public Vector3Int initThreads;
    [HideInInspector] public Vector3Int solveThreads;

    Vector3Int GetKernelThreads(int kernelID) {
        uint x, y, z;
        shader.GetKernelThreadGroupSizes(kernelID, out x, out y, out z);
        return new Vector3Int((int)x, (int)y, (int)z);
    }
    int instantiatedParticles = 0;
    void Awake()
    {
        initKernel = shader.FindKernel("Init");
        solveKernel = shader.FindKernel("Solve");
        print(initKernel);
        print(solveKernel);
        initThreads = GetKernelThreads(initKernel);
        solveThreads = GetKernelThreads(solveKernel);

        particles = new Particle[maxParticles];

        GParticlesArea[] areas = transform.GetComponentsInChildren<GParticlesArea>();
        foreach (var item in areas) {
            item.Instance(this);
        }

        particleBuffer = new ComputeBuffer(maxParticles, ELEMENT_SIZE);
        particleBuffer.SetData(particles);

        shader.SetInt("particleCount", maxParticles);
        if (computeInit)
            shader.SetBuffer(initKernel, "particles", particleBuffer);
        shader.SetBuffer(solveKernel, "particles", particleBuffer);
    }

    private void Start() {
        if(computeInit)
            shader.Dispatch(initKernel, maxParticles / initThreads.x, 1, 1);
    }

    public void AddParticleAt(Vector3 at, Color color) {
        if (instantiatedParticles >= maxParticles)
            return;
        particles[instantiatedParticles].position = at;
        particles[instantiatedParticles].color = color;
        instantiatedParticles++;
    }

    void FixedUpdate() {
        shader.SetFloat("deltaTime", Time.fixedDeltaTime);
        Shader.SetGlobalFloat("deltaTime", Time.fixedDeltaTime);

        shader.Dispatch(solveKernel, maxParticles / solveThreads.x, 1, 1);
    }

    private void OnDestroy() {
        particleBuffer?.Dispose();
    }
}
