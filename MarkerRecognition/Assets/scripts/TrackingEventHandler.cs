using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Vuforia;

namespace DemoVuforia
{
/// <summary>
/// Tracking event handler associated with trackable item. It invokes event callbacks for different event types
/// </summary>
public class TrackingEventHandler : ITrackableEventHandler
{
	public void AddTrackingFoundCallback(Action<Transform> subscriber){
		Debug.Assert (subscriber != null, "[TrackingEventHandler]: subscriber callback cannot be null");

		onTrackingFound += subscriber;
	}

	public void AddTrackingLostCallback(Action<Transform> subscriber){
		Debug.Assert (subscriber != null, "[TrackingEventHandler]: subscriber callback cannot be null");

		onTrackingNotFound += subscriber;
	}

	public void RemoveTrackingFoundCallback(Action<Transform> subscriber){
		Debug.Assert (subscriber != null, "[TrackingEventHandler]: subscriber callback cannot be null");

		onTrackingFound -= subscriber;
	}

	public void RemoveTrackingLostCallback(Action<Transform> subscriber){
		Debug.Assert (subscriber != null, "[TrackingEventHandler]: subscriber callback cannot be null");

		onTrackingNotFound -= subscriber;
	}

	public TrackingEventHandler (Transform transform)
	{
		parentTransform = transform;

		// Default lambda
		onTrackingFound += t => Debug.Log ("Found: " + t.gameObject.name);
		onTrackingNotFound += t => Debug.Log ("Not Found: " + t.gameObject.name);
	}

	#region ITrackableEventHandler implementation
	public void OnTrackableStateChanged (TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
	{
//		Debug.Log (newStatus);
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			onTrackingFound.Invoke(parentTransform);
		}
		else
		{
			if(previousStatus == TrackableBehaviour.Status.DETECTED ||
				previousStatus == TrackableBehaviour.Status.TRACKED ||
				previousStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
				onTrackingNotFound.Invoke (parentTransform);
		}
	}
	#endregion

	#region members

	/// <summary>
	/// The on tracking found delegate
	/// </summary>
	private Action<Transform> onTrackingFound;

	/// <summary>
	/// The on tracking not found delegate
	/// </summary>
	private Action<Transform> onTrackingNotFound;

	private Transform parentTransform;

	#endregion
}
}