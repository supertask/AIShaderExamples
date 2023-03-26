using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsController : MonoBehaviour
{
    [SerializeField] private Mesh boidMesh;
    [SerializeField] private Material boidMaterial;
    [SerializeField] private ComputeShader boidsComputeShader;

    [SerializeField] private int boidCount = 1000;
    [SerializeField] private Vector3 bounds = new Vector3(50, 50, 50);
    [SerializeField] private float neighborRadius = 10;
    [SerializeField] private float maxSpeed = 5;
    [SerializeField] private float separationWeight = 1;
    [SerializeField] private float alignmentWeight = 1;
    [SerializeField] private float cohesionWeight = 1;
    [SerializeField] public float meshScale = 1.0f;

    private ComputeBuffer boidBuffer;

    private MaterialPropertyBlock propertyBlock;
    private int updateBoidsKernel;

    private void Start()
    {
        // Initialize the Boid buffer and property block
        boidBuffer = new ComputeBuffer(boidCount, sizeof(float) * 6, ComputeBufferType.Default);
        propertyBlock = new MaterialPropertyBlock();

        // Create initial Boid data
        Boid[] boidData = new Boid[boidCount];
        for (int i = 0; i < boidCount; ++i)
        {
            boidData[i].position = new Vector3(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y), Random.Range(-bounds.z, bounds.z));
            boidData[i].velocity = Random.insideUnitSphere * maxSpeed;
        }

        // Set the buffer data and parameters
        boidBuffer.SetData(boidData);
        boidsComputeShader.SetBuffer(updateBoidsKernel, "boids", boidBuffer);
        boidsComputeShader.SetInt("boidCount", boidCount);
        boidsComputeShader.SetFloat("neighborRadius", neighborRadius);
        boidsComputeShader.SetFloat("maxSpeed", maxSpeed);
        boidsComputeShader.SetFloat("separationWeight", separationWeight);
        boidsComputeShader.SetFloat("alignmentWeight", alignmentWeight);
        boidsComputeShader.SetFloat("cohesionWeight", cohesionWeight);

        // Get the kernel index
        updateBoidsKernel = boidsComputeShader.FindKernel("UpdateBoids");
    }

    private void Update()
    {
        // Update the Boids using the compute shader
        boidsComputeShader.SetFloat("deltaTime", Time.deltaTime);
        boidsComputeShader.SetVector("bounds", bounds);
        boidsComputeShader.Dispatch(updateBoidsKernel, boidCount / 512 + 1, 1, 1);

        // Draw the Boids using GPU instancing
        propertyBlock.SetBuffer("boids", boidBuffer);
        propertyBlock.SetFloat("_MeshScale", meshScale);
        Graphics.DrawMeshInstancedProcedural(boidMesh, 0, boidMaterial, new Bounds(Vector3.zero, bounds * 2), boidCount, propertyBlock);
    }

    private void OnDestroy()
    {
        // Release the Boid buffer
        if (boidBuffer != null)
        {
            boidBuffer.Release();
            boidBuffer = null;
        }
    }

    private struct Boid
    {
        public Vector3 position;
        public Vector3 velocity;
    }
}

