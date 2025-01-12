using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkParameters", menuName = "ProceduralGeneration/RandomWalkData")]
public class RandomWalkSO : ScriptableObject
{
    [SerializeField] int iterations = 10, walkLength = 10;
    [SerializeField] bool startRandomly = true;

    public int Iterations { get => iterations; }
    public int WalkLength { get => walkLength; }
    public bool StartRandomly { get => startRandomly; }
}
