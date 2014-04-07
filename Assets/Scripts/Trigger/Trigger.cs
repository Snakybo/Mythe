﻿using UnityEngine;
using System.Collections;

public abstract class Trigger:MonoBehaviour {
	public enum TriggerType {
		Normal,
		Once,
	}

	[SerializeField] protected TriggerType triggerType;
	[SerializeField] protected Transform cameraEventTarget;

	protected bool triggeredOnce;
	protected bool triggered;

	/** <returns>The type of the trigger</returns> */
	public TriggerType Type {
		get { return triggerType; }
	}

	/** <returns>Whether or not the trigger has been triggered once</returns> */
	public bool TriggeredOnce {
		get { return triggeredOnce; }
	}

	/** <returns>Whether or not the trigger is currently triggered</returns> */
	public bool Triggered {
		get { return triggered; }
	}
}