using UnityEngine;
using System.Collections;
///===============
/// IsolateRotation - Rotates given transform using a sine wave
///===============
public class IsolateRotation : MonoBehaviour 
{
	public float Speed = 0.1f;			// Speed of rotation
	public float Angle = 45.0f;			// Angle of rotation
	private Transform eTransform;			// Instance of transform
	private Quaternion StartRotation;		// Start rotation of this entity
	//=============
	// START
	//==============
	// Use this for initialization
	void Start () 
	{
		// obtain transform
		eTransform = this.transform;
		// obtain start rotation
		StartRotation = transform.rotation;
	}
	//=============
	// UPDATE 
	//==============
	// Update is called once per frame
	void Update () 
	{
		UpdateRotation ();
	}
	//=============
	// UPDATE ROTATION
	//==============
	///<summary>
	/// Updates rotation of eTransform in a sine wave pattern
	///</summary>
	private void UpdateRotation()
	{	
		// rotate
		eTransform.rotation = Quaternion.AngleAxis(Angle * Mathf.Sin (Time.time * Mathf.PI * 2.0f * Speed), Vector3.up) * StartRotation;
	}
}