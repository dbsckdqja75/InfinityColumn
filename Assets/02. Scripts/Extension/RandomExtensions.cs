using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomExtensions : MonoBehaviour
{
    public static bool RandomBool()
    {
        return (Random.Range(0, 1+1) == 1);
    }
}
