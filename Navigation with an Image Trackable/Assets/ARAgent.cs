using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("AR Components/ARAgent")]
public class ARAgent : MonoBehaviour
{

    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on " + gameObject.name);
        }
    }


    public void MoveAgent(Vector3 position)
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not initialized on " + gameObject.name);
            return;
        }

        agent.isStopped = false;
        agent.destination = position;
    }


    public void StopAgent()
    {
        agent.isStopped = true;
    }
}
