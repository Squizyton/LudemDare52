using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiver : MonoBehaviour
{

    GameObject FoundObject;
    string RaycastReturn;
    RaycastHit hit;
    float distance = 50f;


    public void FmodPostFootstepsEvent()
    {
        CheckSurfaceMaterial();
        PlaySound("event:/SFX/Enemies/Enemy_Greg/Enemy_Greg_FS");
    }

    public void PlaySound(string sound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);   //FMOD
    }


    private void CheckSurfaceMaterial()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out hit, distance)) 
        {
            RaycastReturn = hit.collider.gameObject.name;
            FoundObject = GameObject.Find(RaycastReturn);
            if (FoundObject.TryGetComponent(out ImpactSoundMaterial soundMaterial))
            {
                soundMaterial.CheckMaterial(gameObject);
            }
            else
            {
                Debug.LogError($"Object {FoundObject} doesn't have <b>ImpactSoundMaterial</b> on self!");
            }
        }
    }
}
