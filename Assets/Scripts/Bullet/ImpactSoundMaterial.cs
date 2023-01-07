using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundMaterial : MonoBehaviour
{
    public FMOD.Studio.EventInstance BulletImpact;
    public enum Material { Wood, Flesh, Dirt, Stone };
    public Material m_Material;


    public GameObject FoundObject;
    public string RaycastReturn;
    RaycastHit hit;
    float distance = 1f;

    private void Awake()
    {
        if(gameObject.layer == 1)
        {
            CheckSurfaceMaterial();
            Debug.Log("this is " + gameObject);
        }
        
    }
    public void ImpactSound(GameObject Object)
    {
        BulletImpact.setParameterByNameWithLabel("ImpactMaterial", m_Material.ToString());
        BulletImpact.start();
        Debug.Log("I'm playing impact for " + m_Material.ToString());
    }


    public void CheckMaterial(GameObject Feets)
    {
        BulletImpact.setParameterByNameWithLabel("ImpactMaterial", m_Material.ToString());
    }

    private void CheckSurfaceMaterial()
    {
        if(Physics.Raycast(transform.position, Vector3.back, out hit, distance)) 
        {
            RaycastReturn = hit.collider.gameObject.name;
            FoundObject = GameObject.Find(RaycastReturn);
                Debug.Log($"Hitted {FoundObject} ");
            if (FoundObject.TryGetComponent(out ImpactSoundMaterial soundMaterial))
            {
                soundMaterial.ImpactSound(gameObject);
                //soundMaterial.CheckMaterial(gameObject);
            }
            else
            {
                Debug.Log($"Object {FoundObject} doesn't have <b>ImpactSoundMaterial</b> on self!");
            }
        }

    }
}