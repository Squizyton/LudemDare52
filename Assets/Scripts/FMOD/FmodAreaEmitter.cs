using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FmodAreaEmitter : MonoBehaviour
{
    [Header("Do you want to child colliders?")]
    [SerializeField] private bool useChildColliders;

    [Header("Elements to assign (not needed):")]
    [SerializeField] private GameObject player;
    [SerializeField] private Collider emitterCollider;
    [SerializeField] private Collider[] emitersChildColliders;
    [SerializeField] private EventReference fmodEvent;

    [Header("Variables Control:")]
    [SerializeField] private bool isPlayerInside;
    [SerializeField] private float distanceToPlayer;

    private Vector3 closestPoint;                                  
    private Vector3 emitterPoint;              
    private EventInstance AmbientEmitter;

    private EventDescription AudioDes;
    private float maxDistance;


    private void Awake()
    {   //in Awake script checks if it has neccesary objects to be operational, in other case it will disable itself                                            
        //it looks for Player (we need fmod listener position, player usually is more universal to find it), and collider or child colliders (depending on your use of single or multiple colliders in more complex shapes)

        if (player == null)             {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)             {
                Debug.LogWarning("FMOD Area Emitter could not detect a player. Will be disabled now.");
                this.GetComponent<FmodAreaEmitter>().enabled = !this.GetComponent<FmodAreaEmitter>().enabled;
            }
        }

        if (useChildColliders)          {
            emitersChildColliders = this.GetComponentsInChildren<Collider>();

            if (emitersChildColliders == null)     {
                Debug.LogWarning("FMOD Area Emitter could not detect a colliders on object. Will be disabled now.");
                this.GetComponent<FmodAreaEmitter>().enabled = !this.GetComponent<FmodAreaEmitter>().enabled;
            }
        }

        else if (emitterCollider == null)  {
            emitterCollider = this.GetComponent<Collider>();

            if (emitterCollider == null)       {
                Debug.LogWarning("FMOD Area Emitter could not detect a colliders on object. Will be disabled now.");
                this.GetComponent<FmodAreaEmitter>().enabled = !this.GetComponent<FmodAreaEmitter>().enabled;
            }
        }
    }

    private void Start()
    {
        AmbientEmitter = RuntimeManager.CreateInstance(fmodEvent);

        AudioDes = RuntimeManager.GetEventDescription(fmodEvent);
        AudioDes.getMinMaxDistance(out float minDistance, out maxDistance);         //As we don't need the minimal value and there is no function to get only max distance, minDistance is just declared here and never used

        FindPointForEmitter();

        AttachSoundToEmitter();
        AmbientEmitter.start();
    }

    private void FixedUpdate()
    {
        FindPointForEmitter();
        if (distanceToPlayer > maxDistance || distanceToPlayer < 0)
            return;

        AttachSoundToEmitter();
    }

    private void FindPointForEmitter()
    {
        if (isPlayerInside)
        {
            emitterPoint = player.transform.position;
        }
        else
        {
            if (useChildColliders)
                GetDistanceToChildColliders();
            else
                GetDistanceToCollider();

            emitterPoint = closestPoint;
        }
    }
    
    private void OnDestroy()                                                    //To make sure we will stop this sound from playing after we change level we got to stop it and release, if disabling function copy that also to "OnDisable", and start sound on "OnEnable"
    {
        AmbientEmitter.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        AmbientEmitter.release();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInside = false;
    }

    private void GetDistanceToCollider()
    {
        closestPoint = emitterCollider.ClosestPoint(player.transform.position);
        distanceToPlayer = Vector3.Distance(closestPoint, player.transform.position);
    }

    private void GetDistanceToChildColliders()
    {
        float x = 100f;
        float y = 0f;
        Vector3 newclosestpoint = closestPoint;

        foreach (Collider edge in emitersChildColliders)
        {
            Vector3 newpoint = edge.ClosestPoint(player.transform.position);
            y = Vector3.Distance(newpoint, player.transform.position);

            if (x > y)
            {
                newclosestpoint = newpoint;
                x = y;
            }
        }

        closestPoint = newclosestpoint;
        distanceToPlayer = x;
    }

    private void AttachSoundToEmitter()                             //
    {
        AmbientEmitter.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(emitterPoint)); 
    }

    private void OnDrawGizmos()                                     //This function allows us to see the emitter point in Unity with Gizmos enabled
    {
        Gizmos.DrawSphere(emitterPoint, 0.5f);
    }
}
