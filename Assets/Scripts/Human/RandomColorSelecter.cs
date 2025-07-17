using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorSelecter : MonoBehaviour
{
    public static RandomColorSelecter Instance;

    private float[] possibleValues = { 0f, 0.205078125f, 0.41015625f, 0.63895625f };

    public float[] xOffsets = new float[2];
    public float[] yOffsets = new float[2];

    private void Awake()
    {
        Instance = this;

        OffsetIdentifier();
    }
    public void OffsetIdentifier()
    {
        if(xOffsets.Length == yOffsets.Length)
        {
            for (int i = 0; i < xOffsets.Length; i++)
            {
                xOffsets[i] = possibleValues[Random.Range(0, possibleValues.Length)];
                yOffsets[i] = Random.Range(0, 32) * 0.03125f;
            }
        }
    }
}
