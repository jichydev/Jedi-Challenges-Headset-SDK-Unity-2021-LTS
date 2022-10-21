using UnityEngine;
using System;
using System.Collections;

namespace Ximmerse.Vision
{
	public interface IHeartbeat
	{
		/// <summary>
		/// An event called during Unity's Update() method.
		/// </summary>
		/// <value>A basic event handler.</value>
		EventHandler<EventArgs> OnFrameUpdate { get; set; }

		/// <summary>
		/// An event called during Unity's LateUpdate() method.
		/// </summary>
		/// <value>A basic event handler.</value>
		EventHandler<EventArgs> OnFrameLateUpdate { get; set; }

		/// <summary>
		/// An event called before Unity's Render pass
		/// </summary>
		/// <value>The on before render.</value>
		EventHandler<EventArgs> OnBeforeRender { get; set; }

		/// <summary>
		/// An event called during Unity's PreRender() method.
		/// </summary>
		/// <value>A basic event handler.</value>
		EventHandler<EventArgs> OnFramePreRender { get; set; }

		/// <summary>
		/// An event called when Unity is paused.
		/// </summary>
		/// <value>The on application pause.</value>
		EventHandler<EventArgs> OnApplicationPaused { get; set; }

		/// <summary>
		/// An event called when Unity is resumed.
		/// </summary>
		/// <value>The on application resume.</value>
		EventHandler<EventArgs> OnApplicationResumed { get; set; }

		/// <summary>
		/// An event called when Unity is quit.
		/// </summary>
		/// <value>The on application exited.</value>
		EventHandler<EventArgs> OnApplicationExited { get; set; }

		/// <summary>
		/// Starts a Coroutine on Unity's main thread.
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="enumerator">The enumerator.</param>
		Coroutine StartCoroutine(IEnumerator enumerator);
	}
}