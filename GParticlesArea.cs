using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GParticlesArea : MonoBehaviour
{
    public enum Type{
        Rect, Box, Circle
    }
    public Type type;
    public int particleCount = 100;
    [ColorUsage(true, true)]
    public Color color = Color.white;

    private void OnDrawGizmos() {
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        switch (type)
        {
            case Type.Rect:
                Gizmos.DrawLine(new Vector3(-1, 0, 1), new Vector3(1, 0, 1));
                Gizmos.DrawLine(new Vector3(-1, 0, -1), new Vector3(1, 0, -1));

                Gizmos.DrawLine(new Vector3(1, 0, 1), new Vector3(1, 0, -1));
                Gizmos.DrawLine(new Vector3(-1, 0, 1), new Vector3(-1, 0, -1));
                break;
            case Type.Box:
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(2,2,2));
                break;
            case Type.Circle:
                for (int i = 0; i < 21; i++)
                {
                    float angle = Mathf.PI*2*i/20;
                    float angle_next = Mathf.PI*2*(i+1)/20;
                    Gizmos.DrawLine(
                        new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)), 
                        new Vector3(Mathf.Cos(angle_next), 0, Mathf.Sin(angle_next)));
                }
                break;
        }
    }
    public void Instance(GParticles particles)
    {
        switch (type)
        {
            case Type.Rect:
                for (int i = 0; i < particleCount; i++) {
                    Vector3 pos = new Vector3(
                        Random.Range(-1f, 1f),
                        0,
                        Random.Range(-1f, 1f));
                    particles.AddParticleAt(transform.TransformPoint(pos), color);
                }
                break;
            case Type.Box:
                for (int i = 0; i < particleCount; i++) {
                    Vector3 pos = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f));
                    particles.AddParticleAt(transform.TransformPoint(pos), color);
                }
                break;
            case Type.Circle:
                for (int i = 0; i < particleCount; i++) {
                    Vector2 pos = Random.insideUnitCircle;
                    particles.AddParticleAt(transform.TransformPoint(new Vector3(pos.x,0,pos.y)), color);
                }
                break;
        }
    }
}
