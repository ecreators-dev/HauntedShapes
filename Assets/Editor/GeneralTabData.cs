using Assets.Script.Controller;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public class GeneralTabData
		{
				private Vector2 cameraScrollPos;
				private SceneViewCameraAlignmentEditMode viewsSyncComp;
				private Vector2 staticObjectsPosition;
				private Vector2 lightsPosition;

				public bool cameraListFoldoutStatus { get; set; }

				public ref Vector2 camerasScrollPosition => ref cameraScrollPos;

				public bool viewsCanBeSynchronized { get; set; }

				public Camera activeLinkedCamera { get; set; }

				public bool cameraLinked { get; set; }

				public ref SceneViewCameraAlignmentEditMode viewsSyncComponent => ref viewsSyncComp;

				public bool inGameCameraFollow { get; set; }
				public List<MeshRenderer> staticObjectsInScene { get; internal set; }

				public ref Vector2 staticObjectsScrollPosition => ref staticObjectsPosition;

				public List<Light> lights { get; internal set; }

				public ref Vector2 lightsScrollPosition => ref lightsPosition;

				public LightingSettings lightingSettingActive { get; set; }
		}

#endif
}
