using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Vuforia;

/*
 * - when the image is tracked, lost => event is generated 
 */

public enum TrackingStatus
{
	FOUND,
	NOT_FOUND
}

public interface IImageTrackerService
{
//	#region Image tracking
//	void RegisterImageTracking ();
//	void UnregisterImageTracking ();
//	#endregion

	#region Event Handling
	void RegisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> eventHandler);
	void UnregisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> eventHandler);

	bool ContainsTracker(string trackableName);
	#endregion
}

public class ImageTrackerService : Singleton<ImageTrackerService>, IImageTrackerService {

	bool printed = false;
	
	void Update () {

		if (!printed) {
			var trackableBehaviors = TrackerManager.Instance.GetStateManager ().GetTrackableBehaviours ();

			foreach (var item in trackableBehaviors) {
				Debug.Log (item.TrackableName);
				item.gameObject.name = item.TrackableName + " Tracker";
				var trackingHandler = new TrackingEventHandler (item.transform);

				item.RegisterTrackableEventHandler (trackingHandler);

				eventHandlers.Add (item.TrackableName, trackingHandler);
			}

			printed = true;
		}
	}

	public void RegisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> subscriber)
	{
		Debug.Assert (subscriber != null, "[ImageTrackerService]: callback cannot be null");
		TrackingEventHandler trackingHandler = null;

		if (eventHandlers.TryGetValue (trackableName, out trackingHandler)) {
			if(status == TrackingStatus.FOUND)
				trackingHandler.AddTrackingFoundCallback (subscriber);
			else
				trackingHandler.AddTrackingLostCallback (subscriber);
		} else {
			Debug.LogWarning ("[ImageTrackerService]: Registration failed. Not tracking " + trackableName);
		}
	}

	public void UnregisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> subscriber)
	{
		Debug.Assert (subscriber != null, "[ImageTrackerService]: callback cannot be null");
		TrackingEventHandler trackingHandler = null;

		if (eventHandlers.TryGetValue (trackableName, out trackingHandler)) {
			if(status == TrackingStatus.FOUND)
				trackingHandler.RemoveTrackingFoundCallback (subscriber);
			else
				trackingHandler.RemoveTrackingLostCallback (subscriber);
		} else {
			Debug.LogWarning ("[ImageTrackerService]: Registration failed. Not tracking " + trackableName);
		}
	}

	public bool ContainsTracker(string trackableName){
//		Debug.Assert (eventHandlers.Count != 0, "[ImageTrackerService]: Service is not tracking any trackers");
		return eventHandlers.ContainsKey (trackableName);
	}

	#region members

	private Dictionary<string, TrackingEventHandler> eventHandlers = new Dictionary<string, TrackingEventHandler> ();
	#endregion
}

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

	private Action<Transform> onTrackingFound;
	private Action<Transform> onTrackingNotFound;

	private Transform parentTransform;

	#endregion
}