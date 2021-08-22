using System;
using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Assets.Script.Behaviour.PanormaScreenshot
{
		public class Panorama360 : MonoBehaviour
		{
				public Camera sourceCamera;

				public string subfoldernameInData = @"C:\360-Screenshot";
				public PathType pathType = PathType.ABSOLUTE;

				public EventModifiers triggerModifiers = EventModifiers.Shift;
				public KeyCode triggerKey = KeyCode.Print;
				public bool overrideImage = true;
				public Resolution resolution = Resolution.HD_8K; // maximum in 2021
				public AntiAliasing antiAliasing = AntiAliasing.ANTI_ALIAS_4;
				private Coroutine running;
				private string lastPath;

				public enum PathType
				{
						SUBFOLDER,
						ABSOLUTE
				}

				public enum Resolution
				{
						HD_2K = 1024 * 2,
						HD_4K = 2048 * 2,
						HD_8K = 4096 * 2
				}

				public enum AntiAliasing
				{
						NO_ANTI_ALIAS = 1,
						ANTI_ALIAS_2 = 2,
						ANTI_ALIAS_4 = 4
				}

				private (RenderTexture cubeMap, RenderTexture output) InitializeTextures()
				{
						VerifyObjects();

						var cubemapTexture = RenderTexture.GetTemporary((int)resolution, (int)resolution, 0);
						cubemapTexture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
						cubemapTexture.antiAliasing = (int)antiAliasing;

						var outputTexture = RenderTexture.GetTemporary(cubemapTexture.width, cubemapTexture.height / 2, 0);
						outputTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
						outputTexture.antiAliasing = (int)AntiAliasing.NO_ANTI_ALIAS;

						return (cubemapTexture, outputTexture);
				}

				private void Update()
				{
						if (Input.GetKeyDown(triggerKey))
						{
								if (triggerModifiers != EventModifiers.None)
								{
										string name = triggerModifiers.ToString();
										KeyCode codeModifier = (KeyCode)Enum.Parse(typeof(KeyCode), name);
										if (Input.GetKeyDown(codeModifier))
										{
												Capture();
										}
								}
								else
								{
										Capture();
								}
						}
				}

				[ContextMenu("Take 360 Screenshot")]
				public void CaptureInEditMode()
				{
						Capture();

						StartCoroutine(WaitForFileThenOpen());

						IEnumerator WaitForFileThenOpen()
						{
								while (running is { })
								{
										yield return new WaitForEndOfFrame();
								}

								// avoid button smashing
								// yield return new WaitForSeconds(2);

								// key must be up first
								while (Input.GetKeyDown(triggerKey))
								{
										yield return null;
								}

								OpenInExplorer();
						}
				}

				private void VerifyObjects()
				{
						_ = sourceCamera ?? throw new Exception("Select a camera as source position. Was null!");
				}

				public void Capture()
				{
						if (running is { })
								return;

						running = StartCoroutine(Take360Screenshot());

						IEnumerator Take360Screenshot()
						{
								(RenderTexture cubeMap, RenderTexture output) = InitializeTextures();

								// rendering
								if (!sourceCamera.RenderToCubemap(cubeMap))
								{
										Debug.LogError("Rendering to cubemap is not supported on device/platform!");
								}
								cubeMap.ConvertToEquirect(output);

								// file system
								Save(output);

								// clean up
								RenderTexture.ReleaseTemporary(cubeMap);
								running = null;
								yield break;
						}
				}

				private void Save(RenderTexture outputTexture)
				{
						Texture2D texture = new Texture2D(outputTexture.width, outputTexture.height, TextureFormat.RGB24, false);
						RenderTexture.active = outputTexture;
						texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
						RenderTexture.active = null;
						byte[] buffer = texture.EncodeToPNG();
						buffer = PngXmpHelper.EnableXmp360ImageViewer(buffer, texture.width, texture.height);
						string path = BuildFilePath();

						FileInfo info = new FileInfo(path);
						lastPath = path;

						if (info.Directory.Exists is false)
						{
								Directory.CreateDirectory(info.Directory.FullName);
						}

						File.WriteAllBytes(path, buffer);

						RenderTexture.ReleaseTemporary(outputTexture);
						DestroyImmediate(texture);
				}

				private string BuildFilePath()
				{
						string fileName = BuildFileName();

						string path;
						if (pathType is PathType.ABSOLUTE)
						{
								path = Path.Combine(subfoldernameInData, fileName);
						}
						else
						{
								string subfolder = subfoldernameInData?.Remove(0, 1);
								if (string.IsNullOrWhiteSpace(subfolder))
								{
										path = Path.Combine(Application.dataPath, fileName);
								}
								else
								{
										path = Path.Combine(Application.dataPath, subfolder, fileName);
								}
						}

						Debug.Log($"old: {path}");
						path = path.Replace("/", "\\");
						Debug.Log($"now: {path}");

						return path;
				}

				private string BuildFileName()
				{
						string fileName;
						if (overrideImage)
						{
								fileName = $"screenshot360xmp.png";
						}
						else
						{
								fileName = $"screenshot360xmp_{DateTime.Now:yyyyMMddHHmmss}.png";
						}

						return fileName;
				}

				[ContextMenu("Show in Explorer")]
				public void OpenInExplorer()
				{
						string path = lastPath;
						if (!File.Exists(path))
						{
								Debug.Log("Could not find file");
								return;
						}

						Debug.Log($"Open and select file in explorer: {path}");
						path = Path.GetFullPath(path);
						System.Diagnostics.Process.Start("explorer.exe", $@"/select,""{path}""");
				}
		}
}