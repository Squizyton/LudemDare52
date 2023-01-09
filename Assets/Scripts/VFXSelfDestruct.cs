using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSelfDestruct : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 4);
    }
}
