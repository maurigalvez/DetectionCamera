using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class IsDetectionCamera : MonoBehaviour 
{
	//===========
	// EDIT PROPERTIES
	//===========
	public List<string> Targets;                                 // List of targets
	public float RangeOfView = 8.5f;						     // Range of View of camera
	public bool drawGizmos = true;			 					 // True if gizmos shuold be shown, false otherwise
	public Light cameraLight;					 			     // Instance of camera light
	public float CircleRadius = 3.0f;				  			 // Radius of detection radius
	public float OutOfRangeDistance = 16.0f;					 // Distance out of range
	public DetectionState state;                    		     // Instance of detection state
	//===========	
	// OTHER PROPERTIES
	//===========		
	private IsolateRotation RotationScript;			               // Rotation script
	private Transform eTransform;						                  // Transform of this entity
	private List<Transform> tTransform = new List<Transform>();    // List of target transforms 
	private Transform CurrentTarget;                               // Transform of Current Target
	//==========
	// GIZMOS PROPERTIES
	//==========
	private Vector3 directionToPlayer;
	//==========
	// START
	//==========
	// Use this for initialization
	void Start () 
	{
		Init();
	}
	
	//==========
	// UPDATE
	//==========
	// Update is called once per frame
	void Update () 
	{
		UpdateCameraLight();
		UpdateDetectionState();
		//----------
		// check if player is within range
		//----------
		if(state.IsWithinRange())
			// update player detection      
			StartCoroutine(UpdateTargetDetection());  
		//---------
		// check if player has been detected
		//--------
		if (state.IsPlayerDetected())
			FollowTarget();
	}
	
	//==========
	// UPDATE CAMERA STATE
	//==========
	/// <summary>
	/// Updates the state of the camera.
	/// </summary>
	void UpdateCameraLight()
	{
		// Check if camera light and state have been assigned
		if (cameraLight && state)
		{
			if (state.IsPlayerDetected())
				cameraLight.color = Color.red;      
			else
				cameraLight.color = Color.green;
		}
	}
	/// <summary>
	/// Makes camera follow the player
	/// </summary>
	void FollowTarget()
	{
		if(CurrentTarget)
		{
			eTransform.LookAt(CurrentTarget);
		}
	}
	/// ==========
	/// INIT
	/// <summary>
	/// Initializes this DetectsTargets instance
	/// </summary>
	/// ==========
	private void Init()
	{
		// obtain transform
		eTransform = this.transform;
		// obtain rotation script
		RotationScript = GetComponent<IsolateRotation>();
		// obtain state
		//state = GetComponent<DetectionState>();
		// check if entity doesn't have a state
		if (!state)
			// add a detection state component
			state = this.gameObject.AddComponent<DetectionState>();
		// FIND TARGETS
		StartCoroutine(FindTargets());
	}
	//============
	// DRAW GIZMOS
	//============
	void OnDrawGizmos()
	{
		if (drawGizmos && state)
		{
			// Check if a state has been given and player is Detected
			if (state.IsPlayerDetected())
				Gizmos.color = Color.red;
			else if (state.IsWithinRange())
				Gizmos.color = Color.yellow;
			else
				Gizmos.color = Color.green;
			// Draw gizmos
			Gizmos.DrawLine(transform.position, transform.position + transform.forward * RangeOfView);
			Gizmos.DrawWireSphere(transform.position + transform.forward * RangeOfView, CircleRadius);
			//===========
			// RAYCAST DEBUG
			//===========
			Vector3 directionToPlayer = Vector3.zero;
			if (tTransform.Count > 0)
			{
				foreach (Transform target in tTransform)
				{
					Gizmos.color = Color.cyan;
					directionToPlayer = target.position - eTransform.position;
					// Raycast to player direction
					Gizmos.DrawLine(eTransform.position, directionToPlayer);
				}
			}//check for target transform
		}// check fo drawGizmos and state
	}
	/// =====================
	/// UPDATE DETECTION STATE
	/// <summary>
	/// Updates Detection state of this entity
	/// </summary>
	/// ====================
	private void UpdateDetectionState()
	{
		
		// obtain target within range
		if (!CurrentTarget && !state.IsWithinRange() && !state.IsPlayerDetected())
			CurrentTarget = TargetWithinRange();
		// Check if a target is within range but player has not been detected
		if (CurrentTarget && !state.IsPlayerDetected())
			state.PlayerInRange();
		// check if a target has been detected but its out of range
		else if (CurrentTarget && state.IsPlayerDetected() && TargetOutOfRange())
		{
			state.PlayerLost();
			CurrentTarget = null;
			// check if rotation is disabled
			if (RotationScript && !RotationScript.enabled)
				RotationScript.enabled = true;
		}
	}
	/// ====================
	/// TARGET OUT OF RANGE
	/// <summary>
	/// Checks whether the Current target is out of range
	/// </summary>
	/// <returns>True if current target is out of range, false otherwise.</returns>
	/// ====================
	private bool TargetOutOfRange()
	{
		if(CurrentTarget)
		{
			float distance = (CurrentTarget.position - eTransform.position).magnitude;
			if (distance > OutOfRangeDistance)
				return true;
		}
		return false;
	}
	/// ==============
	/// IS TARGET WITHIN RANGE
	/// <summary>
	/// Checks whether a target is within circle radius
	/// </summary>
	/// <returns>True if target is within circle radius, false otherwise</returns>
	/// ==============
	private Transform TargetWithinRange()
	{
		// Obtain colliders in this circle
		Collider[] cols = Physics.OverlapSphere(eTransform.position + eTransform.forward * RangeOfView, CircleRadius);
		foreach (Collider c in cols)
		{
			foreach (Transform trg in tTransform)
			{
				// check if c == tTransform
				if (c.gameObject.name == trg.gameObject.name)
					return trg;
			}
		}
		return null;
	}
	/// ==========
	/// FIND TARGETS
	/// <summary>
	/// Finds all targets of this entity
	/// </summary>
	/// <returns>null</returns>
	/// ==========
	IEnumerator FindTargets()
	{
		// obtain targets
		foreach (string t in Targets)
		{
			GameObject to = GameObject.Find(t);
			if (to)             
				// add targets transform to the list
				tTransform.Add(to.transform);         
		}
		yield return null;
	}
	/// ==============
	/// UPDATE TARGET DETECTION
	/// <summary>
	/// Updates player detection using raycast to the target.
	/// </summary>
	/// ==============
	IEnumerator UpdateTargetDetection()
	{
		RaycastHit hit;    
		// obtain direction from this entity to target
		Vector3 directionToPlayer = CurrentTarget.position - eTransform.position;
		// raycast
		if (Physics.Raycast(eTransform.position, directionToPlayer, out hit))
		{
			
			if (hit.collider.gameObject.name == CurrentTarget.gameObject.name)
			{
				state.PlayerDetected();        
				// disable isolation script
				if (RotationScript)
					RotationScript.enabled = false;
			}
			else
			{
				state.PlayerInRange();
				// check if rotation is disabled
				if (RotationScript && !RotationScript.enabled)
					RotationScript.enabled = true;
			}
		}// raycast
		
		yield return null;
	}
	
	
	
	
	
}