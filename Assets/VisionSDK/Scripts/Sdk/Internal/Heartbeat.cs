using System;
using UnityEngine;

namespace Ximmerse.Vision.Internal
{
	[RequireComponent(typeof(Camera))]
	public class Heartbeat : MonoBehaviour, IHeartbeat
	{
		#region Public Events

		/// <summary>
		/// An event called during Unity's Update() method.
		/// </summary>
		/// <value>A basic event handler.</value>
		public EventHandler<EventArgs> OnFrameUpdate { get; set; }

		/// <summary>
		/// An event called during Unity's LateUpdate() method.
		/// </summary>
		/// <value>A basic event handler.</value>
		public EventHandler<EventArgs> OnFrameLateUpdate { get; set; }

		/// <summary>
		/// An event called before Unity's Render pass
		/// </summary>
		/// <value>The on before render.</value>
		public EventHandler<EventArgs> OnBeforeRender { get; set; }

		/// <summary>
		/// An event called during Unity's PreRender() method.
		/// </summary>
		/// <value>A basic event handler.</value>
		public EventHandler<EventArgs> OnFramePreRender { get; set; }

		/// <summary>
		/// An event called when Unity is paused.
		/// </summary>
		/// <value>The on application pause.</value>
		public EventHandler<EventArgs> OnApplicationPaused { get; set; }

		/// <summary>
		/// An event called when Unity is resumed.
		/// </summary>
		/// <value>The on application resume.</value>
		public EventHandler<EventArgs> OnApplicationResumed { get; set; }

		/// <summary>
		/// An event called when Unity is quit.
		/// </summary>
		/// <value>The on application quit.</value>
		public EventHandler<EventArgs> OnApplicationExited { get; set; }

		#endregion

		#region Unity Events

		private void Awake()
		{
			Application.onBeforeRender += BeforeRender;
		}

		private void BeforeRender()
		{
			if (OnBeforeRender != null)
			{
				OnBeforeRender(this, new EventArgs());
			}
		}

		private void Update()
		{
			if (OnFrameUpdate != null)
			{
				OnFrameUpdate(this, new EventArgs());
			}
		}

		private void LateUpdate()
		{
			if (OnFrameLateUpdate != null)
			{
				OnFrameLateUpdate(this, new EventArgs());
			}
		}

		private void OnPreRender()
		{
			if (OnFramePreRender != null)
			{
				OnFramePreRender(this, new EventArgs());
			}
		}

		private void OnApplicationPause(bool aState)
		{
			if (aState && OnApplicationPaused != null)
			{
				OnApplicationPaused(this, new EventArgs());
			}
			else if (!aState && OnApplicationResumed != null)
			{
				OnApplicationResumed(this, new EventArgs());
			}
		}

		private void OnApplicationQuit()
		{
			if (OnApplicationExited != null)
			{
				OnApplicationExited(this, new EventArgs());
			}
		}

		private void OnDestroy()
		{
			Application.onBeforeRender -= BeforeRender;

			// Clear events
			OnFrameUpdate = null;
			OnFrameLateUpdate = null;
			OnFramePreRender = null;
			OnApplicationPaused = null;
			OnApplicationResumed = null;

			// Clear all Coroutines
			StopAllCoroutines();
		}

		#endregion
	}
}