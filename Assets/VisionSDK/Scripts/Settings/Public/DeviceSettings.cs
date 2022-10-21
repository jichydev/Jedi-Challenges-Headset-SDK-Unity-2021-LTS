using System;

namespace Ximmerse.Vision
{
	[Serializable]
	public class DeviceSettings
	{
		#region UI Offsets

		/// <summary>
		/// The user interface top offset.
		/// </summary>
		public float UITopOffset;

		/// <summary>
		/// The user interface left offset.
		/// </summary>
		public float UILeftOffset;

		/// <summary>
		/// The user interface bottom offset.
		/// this is used to adjust the heights of the vr cameras.
		/// </summary>
		public float UIBottomOffset;

		/// <summary>
		/// The user interface right offset.
		/// </summary>
		public float UIRightOffset;

		/// <summary>
		/// The frame delay (Motion to Photon).
		/// </summary>
		public int FrameDelay = 3;

        //@EDIT:添加构造方法，赋值DeviceSettings初始化值
        public DeviceSettings()
        {
            this.UITopOffset = 1;
            this.UILeftOffset = 1;
            this.UIBottomOffset = 0.5f;
            this.UIRightOffset = 1;
        }

        #endregion
    }
}
