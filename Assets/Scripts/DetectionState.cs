using UnityEngine;
using System.Collections;
///==================
/// Detection State - Used to store the detection state of the player from an entity.
///==================
public class DetectionState : MonoBehaviour 
{
	// ==========
	// ENUM OF DETECTION STATES
	// ==========
	public enum eDetectionState
	{
		NOTDETECTED = 0,
		ALERTED = 1,
		DETECTED = 2,     
		WITHINRANGE = 3
	}
	public eDetectionState state;			// Current Camera State
	/// <summary>
	/// Set state to alerted
	/// </summary>
	public void PlayerAlert()
	{
		state = eDetectionState.ALERTED;
	}
	/// <summary>
	/// Sets state to detected
	/// </summary>
	public void PlayerDetected()
	{
		state = eDetectionState.DETECTED;
	}
	/// <summary>
	/// Sets the state to NotDetected
	/// </summary>
	public void PlayerLost()
	{
		state = eDetectionState.NOTDETECTED;
	}
	/// <summary>
	/// Set state to within range
	/// </summary>
	public void PlayerInRange()
	{
		state = eDetectionState.WITHINRANGE;
	}
	/// <summary>
	/// Checks whether the player is within range
	/// </summary>
	/// <returns>True if player is within range, false otherwise</returns>
	public bool IsWithinRange()
	{
		return state == eDetectionState.WITHINRANGE;
	}
	/// <summary>
	/// Checks whether player has been detected
	/// </summary>
	/// <returns>True if player has been detected, false otherwise</returns>
	public bool IsPlayerDetected()
	{
		return state == eDetectionState.DETECTED;
	}
	/// <summary>
	/// Checks whether this entity has been alerted
	/// </summary>
	/// <returns>True if this entity has been alerted, false otherwise.</returns>
	public bool IsAlerted()
	{
		return state == eDetectionState.ALERTED;
	}
}