using Assets.Script.Behaviour;
using Assets.Script.Components;
using Assets.Script.Controller;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

using Object = UnityEngine.Object;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public partial class HauntedShapesGameEditorWindow : EditorWindow
		{
				[MenuItem("Game/Haunted-Shapes Window ...")]
				public static void Init()
				{
						HauntedShapesGameEditorWindow window = GetWindow<HauntedShapesGameEditorWindow>();
						window.titleContent = new GUIContent($"Haunted Shapes Einstellung (Editor only) - {nameof(HauntedShapesGameEditorWindow)}");
						window.Show();
				}

				private const int POPUP_NOTHING_SELECTED_INDEX = -1;

				private static class MainTabControlLabels
				{
						public const string LABEL_GENERAL = "General";
						public const string LABEL_INTERACTIBLE = "Interactibles";
						public const string LABEL_MATERIALS = "Materials";
						public const string LABEL_MISSING_MESHES = "Meshes";
						public const string LABEL_CLEAN_UP = "Clean Up";
				}

				private static class CleanUpTabControlLabels
				{
						public const string LABEL_MATERIALS = "Materials";
						public const string LABEL_TEXTURES = "Texturen";
						public const string LABEL_SHADERS = "Shaders";
				}

				private static TabControl tabControl { get; } = new TabControl(new List<Tab>
				{
						new Tab(MainTabControlLabels.LABEL_GENERAL, new GeneralTabData(), BuildGeneralTab),
						new Tab(MainTabControlLabels.LABEL_INTERACTIBLE, new InteractibleTabData(), BuildInteractibleTab),
						new Tab(MainTabControlLabels.LABEL_MATERIALS, new MaterialsTabData(), BuildMaterialsTab, FetchDataMaterials),
						new Tab(MainTabControlLabels.LABEL_MISSING_MESHES, new MissingMeshTabData(), BuildMeshesTab),
						new Tab(MainTabControlLabels.LABEL_CLEAN_UP, new CleanupTabData(), BuildCleanUpTab),
				});

				private static TabControl cleanUpTabControl { get; } = new TabControl(new List<Tab>
				{
						new Tab(CleanUpTabControlLabels.LABEL_TEXTURES, GetCleanUpData(), BuildCleanUpSubTabTextures),
						new Tab(CleanUpTabControlLabels.LABEL_MATERIALS, GetCleanUpData(), BuildCleanUpSubTabMaterials),
						new Tab(CleanUpTabControlLabels.LABEL_SHADERS, GetCleanUpData(), BuildCleanUpSubTabShaders),
				});

				private static CleanupTabData GetCleanUpData()
				{
						return (CleanupTabData)tabControl.GetTabWithTitle(MainTabControlLabels.LABEL_CLEAN_UP).Data;
				}

				private static void BuildCleanUpSubTabShaders(HauntedShapesGameEditorWindow window, Tab tabOwner, object data)
				{
						CleanupTabData args = data as CleanupTabData;
						MaterialsTabData e = tabControl.GetTabWithTitle(MainTabControlLabels.LABEL_MATERIALS).Data as MaterialsTabData;

						CleanupTabData.TabData tabData = args.shaders;
						string headerPlural = "Shader";
						ICollection<AssetPath> usedPaths = e.UsedShaders.Keys;
						string AssetFilter = "t:Shader";

						OnGui_CleanUpTab_Render(tabData, headerPlural, usedPaths, AssetFilter);
				}

				private static void BuildMeshesTab(HauntedShapesGameEditorWindow window, Tab tab, object data)
				{
						MissingMeshTabData e = data as MissingMeshTabData;

						OnGui_MissingMeshes_Render(e);
				}

				private static void OnGui_MissingMeshes_Render(MissingMeshTabData e)
				{
						EditorGUILayout.HelpBox($"Fehlende Meshes führen dazu, dass der Spieler durch die Umgebung gehen oder fallen kann",
								MessageType.Info, wide: true);

						if (GUILayout.Button("Meshes finden"))
						{
								e.repairables = new List<(MeshCollider repair, Mesh mesh)>();
								e.nonRepairables = new List<MeshCollider>();

								var colliders = FindObjectsOfType<MeshCollider>().ToList();
								for (int i = 0; i < colliders.Count; i++)
								{
										MeshCollider a = colliders[i];
										var others = new List<MeshCollider>(colliders);
										for (int j = 0; j < others.Count; j++)
										{
												MeshCollider b = others[j];
												if (a == b)
												{
														continue;
												}

												if (a.gameObject == b.gameObject)
												{
														Mesh existingMesh = a.sharedMesh is { } ? a.sharedMesh : b.sharedMesh;
														MeshCollider missedMesh = a.sharedMesh == null ? a : b;
														if (missedMesh.sharedMesh == null && existingMesh != null)
														{
																e.repairables.Add((missedMesh, existingMesh));
														}
														else if (missedMesh.sharedMesh == null && existingMesh == null)
														{
																e.nonRepairables.Add(missedMesh);
														}
														colliders.Remove(b);
												}
										}
								}
						}

						string number = e.repairables == null ? "?" : e.repairables.Count.ToString();
						if (GUILayout.Button($"{number} Mesh(es) reparieren"))
						{
								for (int i = 0; i < e.repairables.Count; i++)
								{
										(MeshCollider repair, Mesh mesh) = e.repairables[i];
										repair.sharedMesh = mesh;
										Debug.Log($"Fixed Mesh: {repair.gameObject.name} - (trigger: {repair.isTrigger}, convex: {repair.convex})");
										e.repairables.RemoveAt(i--);
								}
						}

						if (e.repairables != null)
						{
								ShowScrollArea(ref e.ScrollPosition, () =>
								{
										IndentMore();
										int index = 0;
										foreach ((MeshCollider repair, Mesh mesh) in e.repairables)
										{
												if (index > 0) HorizonalLine();
												EditorGUILayout.LabelField($"[{index + 1}] {repair.gameObject.name}");
												EditorGUILayout.ObjectField("Ziel", repair, typeof(MeshCollider), allowSceneObjects: true);
												EditorGUILayout.ObjectField("Mesh", repair, typeof(Mesh), allowSceneObjects: true);
										}
										IndentLess();
								});
						}

						if (e.nonRepairables != null)
						{
								EditorGUILayout.HelpBox($"Kann nicht repariert werden: {e.nonRepairables.Count}", MessageType.Warning, wide: true);
						}
				}

				private static void BuildCleanUpSubTabMaterials(HauntedShapesGameEditorWindow window, Tab tabOwner, object data)
				{
						CleanupTabData args = data as CleanupTabData;
						MaterialsTabData e = tabControl.GetTabWithTitle(MainTabControlLabels.LABEL_MATERIALS).Data as MaterialsTabData;

						CleanupTabData.TabData tabData = args.materials;
						string headerPlural = "Materials";
						ICollection<AssetPath> usedPaths = e.UsedMaterials.Keys;
						string AssetFilter = "t:Material";

						OnGui_CleanUpTab_Render(tabData, headerPlural, usedPaths, AssetFilter);
				}


				private static void BuildCleanUpSubTabTextures(HauntedShapesGameEditorWindow window, Tab tabOwner, object data)
				{
						//OnGUI_NotUsedShaders();
						//HorizonalLine();
						CleanupTabData args = data as CleanupTabData;
						MaterialsTabData e = tabControl.GetTabWithTitle(MainTabControlLabels.LABEL_MATERIALS).Data as MaterialsTabData;

						CleanupTabData.TabData tabData = args.textures;
						string headerPlural = "Texturen";
						ICollection<AssetPath> usedPaths = e.UsedTextures.Keys;
						string AssetFilter = "t:Texture";

						OnGui_CleanUpTab_Render(tabData, headerPlural, usedPaths, AssetFilter);
				}

				private static void FetchDataMaterials(object data)
				{
						MaterialsTabData e = data as MaterialsTabData;
						OnGUI_Materials(e, false);
				}

				private static void BuildCleanUpTab(HauntedShapesGameEditorWindow window, Tab tab, object data)
				{
						CleanupTabData e = data as CleanupTabData;
						if (AssetDatabaseEvents.LastUpdate > e.textures.SearchUpdate)
						{
								EditorGUILayout.HelpBox("Assets geändert!", MessageType.Info, true);
						}

						Tab materialsTab = tab.TabControl.GetTabWithTitle(MainTabControlLabels.LABEL_MATERIALS);
						MaterialsTabData mArgs = materialsTab.Data as MaterialsTabData;
						if (mArgs.UsedTextures == null)
						{
								materialsTab.FetchData();
						}

						cleanUpTabControl.OnGUI(window);
				}

				private static void BuildMaterialsTab(HauntedShapesGameEditorWindow window, Tab tabOwner, object data)
				{
						MaterialsTabData e = data as MaterialsTabData;
						OnGUI_Materials(e, true);
				}

				private static void BuildInteractibleTab(HauntedShapesGameEditorWindow window, Tab tab, object args)
				{
						InteractibleTabData e = args as InteractibleTabData;
						OnGUI_Interactibles(tab, e);
				}

				private static void BuildGeneralTab(HauntedShapesGameEditorWindow window, Tab tab, object args)
				{
						GeneralTabData e = args as GeneralTabData;
						OnGUI_CameraSettings(tab, e);
						HorizonalLine();

						OnGUI_Crosshair(tab, e);
						HorizonalLine();

						OnGUI_OrbHitlines(tab, e);
				}

				void OnGUI()
				{
						EditorGUILayout.ObjectField("Script", this, typeof(HauntedShapesGameEditorWindow), allowSceneObjects: true);

						tabControl.OnGUI(this);
						GUILayout.FlexibleSpace();
				}

				public static List<List<T>> Chuck<T>(IEnumerable<T> source, uint chunkSize)
				{
						List<T> list = source.ToList();
						if (list.Count <= chunkSize)
						{
								return new List<List<T>> { list };
						}

						var result = new List<List<T>>();

						for (int i = 0; i < list.Count; i += (int)chunkSize)
						{
								result.Add(list.GetRange(i, Math.Min((int)chunkSize, list.Count - i)));
						}

						return result;
				}

				private static void OnGui_CleanUpTab_Render(CleanupTabData.TabData tabData, string headerPlural, ICollection<AssetPath> usedPaths, string projectAssetFilterText)
				{
						EditorGUILayout.LabelField($"Nicht verwendete {headerPlural}: {tabData.Unused?.Count ?? 0}");
						tabData.Unused ??= new HashSet<AssetPath>();
						tabData.UnusedSorted ??= new List<AssetPath>();

						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button($"Nicht verwendete {headerPlural} finden"))
						{
								FetchUnused(tabData, projectAssetFilterText, usedPaths);
						}

						GUI.enabled = tabData.UnusedSorted.Any();

						void onDelete(AssetPath path)
						{
								tabData.Unused.Remove(path);
								tabData.UnusedSorted.Remove(path);
						}

						if (tabData.ForDelete.Any())
						{
								if (GUILayout.Button($"{tabData.ForDelete.Count} Markierte löschen"))
								{
										DeleteAssetsWithDialog(tabData.ForDelete, onDelete);
										return;
								}
						}
						else
						{
								if (GUILayout.Button("Alle löschen"))
								{
										DeleteAssetsWithDialog(tabData.UnusedSorted, onDelete);
										return;
								}
						}
						GUI.enabled = true;
						EditorGUILayout.EndHorizontal();

						bool pageMode = tabData.PageMode != EntriesPerPageEnum.LIMIT_ALL;
						GUI.enabled = tabData.UnusedSorted.Any() && pageMode;

						if (GUI.enabled is false)
						{
								if (pageMode is false)
								{
										EditorGUILayout.HelpBox("Für die Suche bitte die Anzahl der Seite-Elemente reduzieren", MessageType.Info, true);
								}
								else
								{
										EditorGUILayout.HelpBox("Für die Suche bitte Elemente finden lassen", MessageType.Info, true);
								}
						}

						bool oldEnabled = GUI.enabled;
						string oldFilter = tabData.FilterText;
						tabData.FilterEnabled = EditorGUILayout.Toggle("Suchen", tabData.FilterEnabled);
						GUI.enabled = tabData.FilterEnabled;
						tabData.FilterText = EditorGUILayout.TextField("Filter (Wildcard)", tabData.FilterText);
						string filterMatch = tabData.FilterText?.Replace("*", "\\.*");
						GUI.enabled = oldEnabled;

						// reset allowance to filter
						if (!string.Equals(oldFilter, tabData.FilterText))
						{
								tabData.FilterActive = false;
								tabData.FilterPattern = null;
						}

						Regex pattern = tabData.FilterPattern;
						// allow filter again
						if (Keyboard.current.enterKey.isPressed)
						{
								tabData.FilterEnabled = true;
								// if not empty
								tabData.FilterActive = !string.IsNullOrWhiteSpace(filterMatch);

								if (tabData.FilterActive)
								{
										try
										{
												pattern = new Regex(filterMatch ?? ".*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
												tabData.FilterPattern = pattern;
										}
										catch (Exception ex)
										{
												EditorGUILayout.HelpBox(ex.Message, MessageType.Error, true);
												pattern = null;
										}
								}
						}

						// filter not active
						if (tabData.FilterActive is false)
						{
								pattern = null;
								tabData.FilterPattern = null;
						}

						List<AssetPath> filteredEntries = new List<AssetPath>();
						const int anywaysCount = (int)EntriesPerPageEnum.LIMIT_100;
						bool lessEntries = tabData.UnusedSorted.Count <= anywaysCount;
						if (!pageMode && lessEntries)
						{
								tabData.FilterEnabled = true;
						}

						if (tabData.FilterEnabled && (pageMode || lessEntries))
						{
								//EditorGUILayout.HelpBox($"Pattern: {pattern}", MessageType.Info, true);

								filteredEntries = (from path in tabData.UnusedSorted
																	 where string.IsNullOrWhiteSpace(filterMatch) ||
																	 pattern == null || pattern.IsMatch(path.Filename)
																	 select path).ToList();
						}

						if (tabData.FilterEnabled && tabData.PageMode == EntriesPerPageEnum.LIMIT_ALL)
						{
								EditorGUILayout.HelpBox("Für die Suche bitte die Einträge pro Seite reduzieren", MessageType.Info, true);
						}

						GUI.enabled = true;

						EditorGUILayout.BeginHorizontal();
						tabData.PageMode = (EntriesPerPageEnum)EditorGUILayout.EnumPopup("Anzeige", tabData.PageMode,
								GUILayout.MinWidth(GUISizeHelper.CalculateSize(tabData.PageMode.ToString(), skin => skin.label, out _).x)
								);
						List<List<AssetPath>> viewDataPerPage = new List<List<AssetPath>>();
						uint entriesPerView = (uint)tabData.PageMode;
						if (pageMode)
						{
								viewDataPerPage = Chuck(filteredEntries, entriesPerView);
						}
						else
						{
								viewDataPerPage = new List<List<AssetPath>> { filteredEntries };
						}
						EditorGUILayout.EndHorizontal();

						List<AssetPath> pageEntries = FilterPageEntriesSelector(viewDataPerPage, tabData);

						if (pageEntries.Any())
						{
								ShowScrollArea(ref tabData.ScrollPosition, () =>
								{
										for (int i = 0; i < pageEntries.Count; i++)
										{
												// if (i > 0) HorizonalLine();
												AssetPath path = pageEntries[i];
												OnGui_ListEntryAssetPath(path, tabData.ForDelete, path =>
														{
																tabData.Unused.Remove(path);
																tabData.UnusedSorted.Remove(path);
														});
										}
								});
						}
				}

				private static void OnGui_ListEntryAssetPath(AssetPath path, List<AssetPath> toDeleteList, Action<AssetPath> onDelete)
				{
						string shortName = GetShortName(path, out string dirName);
						if (shortName != null)
						{
								EditorGUILayout.BeginHorizontal();
								if (EditorGUILayout.ToggleLeft(new GUIContent(shortName, path.Path), toDeleteList.Contains(path)))
								{
										MarkForDelete(path, toDeleteList);
								}
								else
								{
										MarkForNotDelete(path, toDeleteList);
								}

								if (GUILayout.Button("Ping"))
								{
										PingAsset(path);
								}

								if (GUILayout.Button("Löschen"))
								{
										DeleteAssetsWithDialog(new[] { path }.ToList(), onDelete);
								}
								EditorGUILayout.EndHorizontal();
						}
						else
						{
								onDelete.Invoke(path);
						}
				}

				private static void MarkForNotDelete(AssetPath path, List<AssetPath> toDeleteList)
				{
						if (toDeleteList.Contains(path))
						{
								toDeleteList.Remove(path);
						}
				}

				private static void MarkForDelete(AssetPath path, List<AssetPath> toDeleteList)
				{
						if (!toDeleteList.Contains(path))
						{
								toDeleteList.Add(path);
						}
				}

				private static void FetchUnused(CleanupTabData.TabData tabData, string assetFilter, ICollection<AssetPath> usedAssets)
				{
						tabData.ForDelete.Clear();
						tabData.Unused.Clear();

						string[] allShaders = AssetDatabase.FindAssets(assetFilter);
						foreach (string guid in allShaders)
						{
								string path = AssetDatabase.GUIDToAssetPath(guid);
								AssetPath loaded = AssetPath.Build(path, guid);
								if (!usedAssets.Contains(loaded) && GetShortName(loaded, out _) != null)
								{
										Debug.Log($"{path}");
										tabData.Unused.Add(loaded);
								}
						}
						tabData.UnusedSorted = tabData.Unused.OrderBy(e => e).ToList();
						tabData.SearchUpdate = DateTime.Now;
				}

				private static void DeleteAssetsWithDialog(List<AssetPath> removables, Action<AssetPath> onDelete)
				{
						if (EditorUtility.DisplayDialog("Alle ungenutzten Elemente löschen?",
																$"In der Auswahl befinden sich {removables.Count} Dateien.\nBestätigen sie, wenn diese alle endgültig gelöscht werden sollen.",
																"Ja, Löschen", "Abbrechen"))
						{
								List<string> failedToDelete = new List<string>();
								if (AssetDatabase.DeleteAssets(removables
										.Select(path => (string)path).ToArray(),
										failedToDelete))
								{
										EditorUtility.DisplayDialog("Erfolg!",
												$"Es wurden {removables.Count - failedToDelete.Count} Dateien aus den Assets entfernt.", "Verstanden");

										var deleted = removables.SkipWhile(assetPath => !failedToDelete.Contains(assetPath.Path)).ToList();
										foreach (AssetPath deletedPath in deleted)
										{
												onDelete.Invoke(deletedPath);
										}
								}
						}
				}

				private static List<AssetPath> FilterPageEntriesSelector(List<List<AssetPath>> pagesWithEntries, CleanupTabData.TabData tabData)
				{
						EditorGUILayout.BeginHorizontal();
						int pages = pagesWithEntries.Count;
						int currentPage = tabData.PageIndex + 1; // index to number
						bool hasPages = pages > 0;

						GUI.enabled = hasPages;

						GUI.enabled = currentPage > 1 && hasPages;
						if (GUILayout.Button("|<")) currentPage = 1;
						if (GUILayout.Button("<")) currentPage = Mathf.Max(1, currentPage - 1);

						GUI.enabled = currentPage < pages && hasPages;
						if (GUILayout.Button(">")) currentPage = Mathf.Min(pages, currentPage + 1);
						if (GUILayout.Button(">|")) currentPage = pages;

						GUI.enabled = hasPages;

						currentPage = EditorGUILayout.IntField($"Seite (max. {pagesWithEntries.Count})", currentPage);
						currentPage = Mathf.Max(1, Mathf.Min(pages, currentPage));
						int pageIndex = currentPage - 1; // number to index
						tabData.PageIndex = pageIndex;
						EditorGUILayout.EndHorizontal();
						GUI.enabled = true;
						HorizonalLine();

						return tabData.PageIndex >= 0 && pagesWithEntries.Any() ? pagesWithEntries[pageIndex] : new List<AssetPath>();
				}

				private static bool PingAsset(string path)
				{
						OpenProjectWindowAndFocus();
						Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
						EditorGUIUtility.PingObject(asset);
						return asset != null;
				}

				private static string GetShortName(AssetPath path, out string dirName)
				{
						string fullPath = path;
						fullPath = fullPath.Replace("/", "\\");
						FileInfo info = new FileInfo(fullPath);
						string name = Path.GetFileNameWithoutExtension(fullPath);
						string parentFolderName = info.Directory.Name;
						string fullDir = info.DirectoryName.ToLower();
						if (fullDir.Contains("library\\")
								|| fullDir.Contains("packages\\"))
						{
								dirName = null;
								return null;
						}
						dirName = parentFolderName;
						return name;
				}

				private static void OpenProjectWindowAndFocus()
				{
						Assembly editorAssembly = typeof(EditorWindow).Assembly;
						Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");
						EditorWindow projectBrowser = GetWindow(projectBrowserType);
						projectBrowser.Focus();
				}

				private static void OnGUI_Materials(MaterialsTabData e, bool withGui)
				{
						if (withGui)
								EditorGUILayout.LabelField("Materials and Masks");

						Renderer[] allObjectsWithRenderer = FindObjectsOfType<Renderer>();

						if (withGui)
						{
								ShowScrollArea(ref e.materialsScrollPosition, () => BuildContent(e, withGui, allObjectsWithRenderer));
						}
						else
						{
								BuildContent(e, withGui, allObjectsWithRenderer);
						}
				}

				private static void BuildContent(MaterialsTabData e, bool withGui, Renderer[] allObjectsWithRenderer)
				{
						e.UsedShaders = new Dictionary<AssetPath, Shader>();
						e.UsedMaterials = new Dictionary<AssetPath, Material>();
						e.UsedTextures = new Dictionary<AssetPath, Texture>();

						RenderPipelineAsset renderPipeline = GraphicsSettings.currentRenderPipeline;
						if (renderPipeline is null)
								return;

						HDRenderPipelineAsset pipelineFirst = (HDRenderPipelineAsset)renderPipeline;
						List<string> names = pipelineFirst.renderingLayerMaskNames.ToList();

						for (int i = 0; i < allObjectsWithRenderer.Length; i++)
						{
								if (i > 0)
								{
										HorizonalLine();
								}

								Renderer renderer = allObjectsWithRenderer[i];
								var materials = renderer.sharedMaterials;
								if (materials != null)
								{
										string gameObjectName = renderer.gameObject.name;
										uint layer = renderer.renderingLayerMask;
										if (withGui)
										{
												EditorGUILayout.ObjectField("Referenz", renderer.gameObject, typeof(GameObject), allowSceneObjects: true);
												EditorGUILayout.ObjectField(gameObjectName, renderer, typeof(Renderer), allowSceneObjects: true);
										}

										foreach (Material material in materials.Where(m => m != null))
										{
												FillData(e, withGui, materials, material);
										}

										// allow change layers
										if (withGui)
										{
												LightLayerEnum value = (LightLayerEnum)layer;
												value = (LightLayerEnum)EditorGUILayout.EnumFlagsField("Rendering Layers", value);
												renderer.renderingLayerMask = (uint)value;

												EditorGUILayout.Popup("Rendering Layers (Light Layer Names)", POPUP_NOTHING_SELECTED_INDEX, GetLightLayerNamesByValue(names, value));
										}
								}
						}
				}

				private static string[] GetLightLayerNamesByValue(List<string> names, LightLayerEnum combinedEnumValue)
				{
						List<string> selected = new List<string>();
						int index = 0;
						foreach (LightLayerEnum enumValue in Enum.GetValues(typeof(LightLayerEnum)).Cast<LightLayerEnum>())
						{
								if (BitwiseIsUsed((int)enumValue, (int)combinedEnumValue))
								{
										string myName = names[index];
										selected.Add(myName);
								}
								index++;
						}
						return selected.ToArray();

						static bool BitwiseIsUsed(int val, int you) => (val & you) != 0;
				}

				private static void FillData(MaterialsTabData e, bool withGui, Material[] materials, Material material)
				{
						int materialIndex = materials.ToList().IndexOf(material);
						string shaderName = material.shader.name;
						string materialName = material.name;
						if (withGui)
						{
								GUI.enabled = false;
								EditorGUILayout.ObjectField($"[{materialIndex}]", material, typeof(Material), allowSceneObjects: true);
								GUI.enabled = true;
						}
						e.UsedShaders[GetAssetPath(material.shader)] = material.shader;
						e.UsedMaterials[GetAssetPath(material)] = material;

						List<Texture> allTextures = material.GetTexturePropertyNames()
						.Select(texName => material.GetTexture(texName))
						.Where(tex => tex is { }).ToList();
						foreach (Texture texture in allTextures)
						{
								e.UsedTextures[GetAssetPath(texture)] = texture;
						}
				}
				private static AssetPath GetAssetPath(Object instance)
				{
						string assetPath = AssetDatabase.GetAssetPath(instance);
						AssetGuid assetGuid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString().ToLower();
						return AssetPath.Build(assetPath, assetGuid.Guid);
				}

				private static void OnGUI_OrbHitlines(Tab tab, GeneralTabData e)
				{
						GhostRoom.showOrbPositions = EditorGUILayout.Toggle("Orb Positionen zeigen", GhostRoom.showOrbPositions);
				}

				private static void HorizonalLine()
				{
						EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
				}

				private static void TryRemoveComponent<T>(GameObject gameObject)
						where T : Component
				{
						if (gameObject.TryGetComponent(out T linkerScript))
						{
								DestroyImmediate(linkerScript);
						}
				}

				private static void OnGUI_Crosshair(Tab tab, GeneralTabData e)
				{
						var oneCrosshair = FindObjectOfType<CrosshairHitVisual>();
						EditorGUILayout.LabelField("Crosshair (Klick-Steuerung) (Script)");
						if (oneCrosshair != null)
						{
								EditorGUILayout.ObjectField(oneCrosshair, oneCrosshair.GetType(), allowSceneObjects: true);
						}
						else
						{
								EditorGUILayout.HelpBox($"Kein {nameof(CrosshairHitVisual)} gefunden!", MessageType.Warning, true);
						}
				}

				private static void OnGUI_Interactibles(Tab tab, InteractibleTabData e)
				{
						EditorGUILayout.LabelField("Interaktive Objekte - Übersicht");
						var equipments = FindObjectsOfType<Equipment>().ToList();
						var items = FindObjectsOfType<PickupItem>().Where(x => equipments.All(e => e != x)).ToList();
						var any = FindObjectsOfType<Interactible>().Where(x => equipments.All(e => e != x) &&
						items.All(i => i != x)).ToList();

						e.interactiblesScrollPositions = EditorGUILayout.BeginScrollView(e.interactiblesScrollPositions);
						IndentMore();
						EditorGUILayout.LabelField($"Ausrüstungs-Objekte: {equipments.Count}");
						foreach (var equipment in equipments)
						{
								EditorGUILayout.ObjectField(equipment, typeof(Equipment), allowSceneObjects: true);
						}
						HorizonalLine();
						EditorGUILayout.LabelField($"Items Objekte: {items.Count}");
						foreach (var item in items)
						{
								EditorGUILayout.ObjectField(item, typeof(PickupItem), allowSceneObjects: true);
						}
						HorizonalLine();
						EditorGUILayout.LabelField($"Interaktive Objekte: {any.Count}");
						foreach (var one in any)
						{
								EditorGUILayout.ObjectField(one, typeof(Interactible), allowSceneObjects: true);
						}
						IndentLess();
						EditorGUILayout.EndScrollView();
				}

				private static void OnGUI_CameraSettings(Tab tab, GeneralTabData e)
				{
						GUILayout.Label("Kamera Einstellungen", EditorStyles.boldLabel);

						Camera[] cameras = FindObjectsOfType<Camera>();
						EditorGUILayout.LabelField($"Kameras in Scene: {cameras.Length}");
						IndentMore();
						e.cameraListFoldoutStatus = EditorGUILayout.Foldout(e.cameraListFoldoutStatus, "Auswahl sync Kameras (Scene)", true);
						IndentLess();

						if (e.cameraListFoldoutStatus)
						{
								ShowScrollArea(ref e.camerasScrollPosition, () =>
								{
										e.viewsCanBeSynchronized = false;
										foreach (Camera cam in cameras)
										{
												ShowCameraWithCheckBox(cam, cameras);
										}
								});
						}

						EditorGUI.indentLevel++;
						ShowSelectedCameraObject(e);

						ShowSynchronizingStatusCheckBox(e);
						EditorGUI.indentLevel--;

						GUI.enabled = e.activeLinkedCamera != null;
						if (GUI.enabled is false)
						{
								EditorGUILayout.HelpBox("Wähle eine aktive Kamera", MessageType.Info, true);
						}
						if (GUILayout.Button("Zeige Kamera Sicht"))
						{
								ShowActiveCamera(e.activeLinkedCamera);
						}
						GUI.enabled = true;

						e.inGameCameraFollow = GUILayout.Toggle(e.inGameCameraFollow, "Zeige In-Game View in Szene");
						EditorGUILayout.HelpBox("Wenn von Kamera aufgerufen, wird die Szenen Camera mit der Spielerkamera gekoppelt", MessageType.Info, true);

						//-- Local Functions -------------------------------------------

						void ShowCameraWithCheckBox(Camera listItemCam, IEnumerable<Camera> allCameras)
						{
								EditorGUILayout.BeginHorizontal();

								// checked status. camera is active linked
								bool cameraWasActive = e.activeLinkedCamera != null && listItemCam == e.activeLinkedCamera;
								EditorGUILayout.ObjectField(listItemCam, typeof(Camera), true);
								bool cameraIsActive = EditorGUILayout.Toggle("Verwenden", cameraWasActive);
								// check status changed
								if (cameraWasActive != cameraIsActive)
								{
										Debug.Log($"{cameraIsActive}");
										// changed to link now:
										if (cameraIsActive)
										{
												e.viewsCanBeSynchronized = true;
												e.activeLinkedCamera = listItemCam;
												TryAddComponentIfNotPresent(listItemCam.gameObject, out e.viewsSyncComponent);

												// if old was linked?! then link again (TODO)
												if (e.cameraLinked)
												{
														e.viewsSyncComponent.StartSynchronizing();
												}
										}
										// always: only a single camera can be selected!
										// if unchoose the current camera, then the selected must be null!
										else
										{
												RemoveScriptFromOthers<Camera, SceneViewCameraAlignmentEditMode>(allCameras, listItemCam);
												TryRemoveComponent<SceneViewCameraAlignmentEditMode>(listItemCam.gameObject);
												e.activeLinkedCamera = null;
												e.viewsCanBeSynchronized = false;
												e.cameraLinked = false;
										}
								}

								EditorGUILayout.EndHorizontal();
						}
				}

				private static void IndentMore() => EditorGUI.indentLevel++;

				private static void IndentLess() => EditorGUI.indentLevel--;

				private static void ShowActiveCamera(Camera camera)
				{
						SceneView sceneView = SceneView.lastActiveSceneView;
						//sceneView.ResetCameraSettings();
						sceneView.camera.projectionMatrix = camera.projectionMatrix;
						sceneView.rotation = camera.transform.rotation;
						sceneView.pivot = camera.transform.position;
						var angles = sceneView.camera.transform.eulerAngles;
						angles.z = 0;
						sceneView.camera.transform.eulerAngles = angles;
						//sceneView.camera.CopyFrom(camera);
						Debug.Log("Szenen Kamera aktualisiert");
				}

				private static void ShowSelectedCameraObject(GeneralTabData e)
				{
						GUI.enabled = false;
						EditorGUILayout.ObjectField("Sync-View-Kamera", e.activeLinkedCamera, typeof(Camera), allowSceneObjects: true);
						GUI.enabled = true;
				}

				private static void RemoveScriptFromOthers<T, C>(IEnumerable<T> others, T keep)
						where T : Component
						where C : Component
				{
						// remove from others:
						foreach (T camera in others.SkipWhile(c => c == keep))
						{
								TryRemoveComponent<C>(keep.gameObject);
						}
				}

				private static void ShowSynchronizingStatusCheckBox(GeneralTabData e)
				{
						bool oldStatus = e.cameraLinked;
						e.cameraLinked = EditorGUILayout.Toggle("Scene/Game Sync", e.cameraLinked, GUILayout.ExpandWidth(true));
						bool changedStatus = oldStatus != e.cameraLinked;
						if (changedStatus)
						{
								// sync now:
								if (e.cameraLinked)
								{
										e.viewsSyncComponent.StartSynchronizing();
										Debug.Log("Synchronizing Scene/Game View: running");
								}
								else if (e.viewsSyncComponent is { })
								{
										e.viewsSyncComponent.StopSynchronizing();
										Debug.Log("Synchronizing Scene/Game View: stopped");
								}
						}
				}

				private static void ShowScrollArea(ref Vector2 scrollPosition, Action content)
				{
						scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
						IndentMore();
						content.Invoke();
						IndentLess();
						EditorGUILayout.EndScrollView();
				}

				private static bool TryAddComponentIfNotPresent<C>(GameObject target, out C add)
						where C : Component
				{
						if (target == null)
						{
								add = null;
								return false;
						}

						if (target.TryGetComponent(out add) is false)
						{
								add = target.AddComponent<C>();
						}
						return true;
				}
		}

#endif
}