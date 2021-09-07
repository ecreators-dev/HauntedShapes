using Assets.Script.Controller;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public class GeneralTabData
		{
				private Vector2 cameraScrollPos;
				private SceneViewCameraAlignmentEditMode viewsSyncComp;

				public bool cameraListFoldoutStatus { get; set; }

				public ref Vector2 camerasScrollPosition => ref cameraScrollPos;

				public bool viewsCanBeSynchronized { get; set; }

				public Camera activeLinkedCamera { get; set; }

				public bool cameraLinked { get; set; }

				public ref SceneViewCameraAlignmentEditMode viewsSyncComponent => ref viewsSyncComp;

				public bool inGameCameraFollow { get; set; }
		}

#endif
}
