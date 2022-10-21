using UnityEngine;

namespace Ximmerse.Vision.Internal
{
	public class FlyCamera : MonoBehaviour
	{
		#if UNITY_EDITOR

		#region Public Properties

		public float Speed = 100.0f;
		public float MouseScale = 0.25f;
		public float RotSens = 0.5f;
		public bool MouseLook = true;

		#endregion

		#region Private Properties

		private Vector3 lastMouse = new Vector3(255, 255, 255);
		private float totalRun = 1.0f;
		private bool firstTime = true;
		private bool lockTranslation = false;
		private bool lockAngles = false;
		private bool lockGroundParallel;
		private float lastFrameTime = 0;

		#endregion

		#region Unity Methods

		private void Update()
		{
			if (Input.GetMouseButton(1))
			{
				if (!lockAngles && MouseLook)
				{
					if (firstTime)
					{
						firstTime = false;
						lastMouse = Vector3.zero;
					}
					else
					{
						lastMouse = Input.mousePosition - lastMouse;
					}

					lastMouse = new Vector3(-lastMouse.y * MouseScale, lastMouse.x * MouseScale, 0);
					lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);

					transform.eulerAngles = lastMouse;
					lastMouse = Input.mousePosition;
				}

				if (!lockTranslation)
				{
					Vector3 p = GetBaseInput();
					totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
					p = p * Speed;
					p = p * (Time.realtimeSinceStartup - lastFrameTime);
					Vector3 newPosition = transform.position;

					// If player wants to move on X and Z axis only
					if (lockGroundParallel)
					{ 
						transform.Translate(p);
						newPosition.x = transform.position.x;
						newPosition.z = transform.position.z;
						transform.position = newPosition;
					}
					else
					{
						transform.Translate(p);
					}
				}
			}
			else
			{
				firstTime = true;
				if (!lockAngles && !MouseLook)
				{
					Vector3 p = GetBaseInput();
					totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
					p = p * Speed;
					Vector3 pScaled = new Vector3(-p.z * MouseScale, p.x * MouseScale, 0);
					transform.eulerAngles = new Vector3(transform.eulerAngles.x + pScaled.x, transform.eulerAngles.y + pScaled.y, 0);
				}
			}

			if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftShift))
			{
				lockAngles = !lockAngles;
				firstTime |= lockAngles;
			}

			if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift))
			{
				lockTranslation = !lockTranslation;
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				lockGroundParallel = !lockGroundParallel;
			}

			lastFrameTime = Time.realtimeSinceStartup;
		}

		#endregion

		#region Private Methods

		private Vector3 GetBaseInput()
		{
			Vector3 p_Velocity = new Vector3();
			if (Input.GetKey(KeyCode.W))
			{
				p_Velocity += new Vector3(0, 0, 1);
			}
			if (Input.GetKey(KeyCode.S))
			{
				p_Velocity += new Vector3(0, 0, -1);
			}
			if (Input.GetKey(KeyCode.A))
			{
				p_Velocity += new Vector3(-1, 0, 0);
			}
			if (Input.GetKey(KeyCode.D))
			{
				p_Velocity += new Vector3(1, 0, 0);
			}
			return p_Velocity;
		}
		#endregion

		#endif
	}
}