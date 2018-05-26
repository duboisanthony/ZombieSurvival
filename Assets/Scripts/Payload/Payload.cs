using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Payload : MonoBehaviour {

    // Use this for initialization
    public GameObject[] nodeList;

    public GameObject startingNode;
    public GameObject goalNode;

    public GameObject currentTarget;

    public int startingHealth = 100;                            
    public int currentHealth;                                  
    bool isDead;                                                
    bool damaged;

    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;

    public float m_PitchRange = 0.2f;
    private float m_OriginalPitch;

    [Tooltip("The Player's UI GameObject Prefab")]
    public GameObject PayloadUiPrefab;

    public float acceleration;
    public float maxSpeed;
    public float auraRadius;

    //public Slider healthSlider;                                 
    public Image damageImage;

    public GameObject Explosion;

    public float arriveMagnitude;

    private Rigidbody _rb;
    float timer;
    public bool canMove;

    public float levelLoaderDelay = 3.0f;

    void Awake()
    {
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    void Start () {

       
        if (PayloadUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(PayloadUiPrefab) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezePositionY;
        canMove = true;
        Vector3 closestNodePosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        foreach (GameObject node in nodeList)
        {
            if ((transform.position - node.transform.position).magnitude < closestNodePosition.magnitude)
            {
                closestNodePosition = node.transform.position;
                startingNode = node;
            }
            if (node.GetComponent<Node>().endNode == true)
                goalNode = node;
        }
        Debug.Log("Starting node : " + startingNode + "  Goal Node:  " + goalNode);

        currentTarget = startingNode;
	}

    void GetNewTargetFromEdgeList()
    {
        Edge[] edgeList = currentTarget.GetComponent<Node>().edgeList;
        Vector3 closestNodePosition = currentTarget.transform.position;
        foreach (Edge edge in edgeList)
        {
            Debug.Log(goalNode.transform.position + "Edge");
            if ((edge.endNode.transform.position - goalNode.transform.position).magnitude < (closestNodePosition - goalNode.transform.position).magnitude)
            {
                Debug.Log("Changing Target: " + currentTarget + " for " + edge.endNode);
                closestNodePosition = edge.endNode.transform.position;
                currentTarget = edge.endNode;

            }
        }
    }


    bool Align()
    {
        Vector3 goalFacing = (currentTarget.transform.position - transform.position).normalized;
        Quaternion lookWhereYoureGoing = Quaternion.LookRotation(goalFacing, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookWhereYoureGoing, 0.5f);
        float dotProd = Vector3.Dot(goalFacing, transform.forward);
        if (dotProd > 0.999)
        {
                return true;
        }
        return false;
    }

    bool CheckNodeList()
    {
        if ((transform.position - currentTarget.transform.position).magnitude < arriveMagnitude)
        {
            if (currentTarget.GetComponent<Node>().endNode == true)
            {
                _rb.velocity = Vector3.zero;
                PhotonNetwork.LoadLevel("GameWon");
                return true;
            }
            GetNewTargetFromEdgeList();
        }
        return false;
    }

    bool CheckAura()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, auraRadius);
        int zombieCount = 0;
        int playerCount = 0;
        foreach (Collider c in colliders)
        {
            switch (c.gameObject.tag)
            {
                case "Zombie":
                    zombieCount++;
                    break;
                case "Player":
                    playerCount++;
                    break;
            }
        }
        if (zombieCount > 0 || playerCount < 1)
            return false;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (CheckAura())
        { 
            _rb.constraints = RigidbodyConstraints.FreezePositionY;
            if (CheckNodeList() == true) return;
            if (Align() == true)
            {
                
                _rb.AddRelativeForce(Vector3.forward * acceleration * Time.deltaTime);
                if (_rb.velocity.magnitude > maxSpeed)
                    _rb.velocity = _rb.velocity.normalized * maxSpeed;
            }
            else
            {
                if (_rb.velocity.magnitude < 0.5f)
                    return;
                _rb.AddRelativeForce(-Vector3.forward * (acceleration) * Time.deltaTime);
            }
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        EngineAudio();

    }
    public void TakeDamage(int amount)
    {
        damaged = true;
        
        currentHealth -= amount;
        
        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    IEnumerator goToLevel(string level)
    {
        Debug.Log("LOST PAYLOAD, WILL LOAD LEVEL: " + level);
        yield return new WaitForSeconds(levelLoaderDelay);
        PhotonNetwork.LoadLevel("GameOver");
    }

    void Death()
    {
        if (!isDead)
            StartCoroutine("goToLevel", "GameOver");
        timer = 0f;
        isDead = true;
        PhotonNetwork.Instantiate(this.Explosion.name, this.transform.position, Quaternion.identity, 0);

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
    }

    private void EngineAudio()
    {

        if (!CheckAura())
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

}
