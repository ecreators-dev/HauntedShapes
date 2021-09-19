using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Script.Behaviour
{
		public class LighterBehaviour : Equipment
		{
				private const string SHADER_PROP_FILLED = "_Filled";
				private const string SHADER_PROP_FRONT_COLOR = "_FrontColor";
				private const string SHADER_PROP_TOP_COLOR = "_TopColor";

				[SerializeField] private bool StartEnlit = false;
				[SerializeField] private MeshRenderer lightMatRenderer;
				[Range(0, 1)]
				[SerializeField] private float filling = 1f;
				[SerializeField] private Color fillingColor;
				[SerializeField] private Color fillingColorTop;
				[SerializeField] private GameObject LightCandle;
				[Tooltip("Looping idle animation")]
				[SerializeField] private Animator animator;
				[SerializeField] private ShopParameters shopInfo;
				
				private Material liquidMat;
				private float endTimer;
				private readonly Dictionary<string, int> shaderPropertyIds = new Dictionary<string, int>();

				private void Awake()
				{
						liquidMat = new Material(lightMatRenderer.material);
						lightMatRenderer.material = liquidMat;
						lightMatRenderer.materials = new[] { liquidMat };
				}

				protected override void Start()
				{
						SetShopInfo(shopInfo, this);

						base.Start();

						if (StartEnlit)
						{
								ToggleOn();
						}
						else
						{
								ToggleOff();
						}
				}

				private void ToggleOff()
				{
						SetPowered(false);
						LightCandle.SetActive(false);
						animator.enabled = false;
				}

				private void ToggleOn()
				{
						// not broken
						if (IsBroken is false)
						{
								// override progress liquid "empty"
								ResetTimeoutActiveSeconds();

								SetPowered(true);
								LightCandle.SetActive(true);
								animator.enabled = true;
						}
				}

				private void ResetTimeoutActiveSeconds()
				{
						if (endTimer <= 0)
						{
								endTimer = ShopInfo.ActiveSeconds;
								filling = 1f;
						}
				}

				protected override void Update()
				{
						base.Update();

						// if broken, no update and off
						// no broken, but liquid empty, no update and broken
						// with liquid and powered, update liquid filling
						// with liquid and powered, show light and run animator
						// with liquid and not powered, hide light and stop animator

						UpdateShaderPropertyFilled();
						if (IsBroken)
						{
								ToggleOff();
								return;
						}
						else if (filling <= 0)
						{
								// empty!
								endTimer = 0;
								filling = 0;
								SetBroken();
								return;
						}

						if (IsPowered)
						{
								endTimer -= Time.deltaTime;
								filling = endTimer / ShopInfo.ActiveSeconds;
						}
				}

				private void UpdateShaderPropertyFilled()
				{
						int filledId = 0;
						int frontColorId = 0;
						int topColorId = 0;
						if (shaderPropertyIds.Any() is false)
						{
								Shader shader = liquidMat.shader;
								int count = shader.GetPropertyCount();
								int matches = 0;
								for (int i = 0; i < count && matches < 3; i++)
								{
										string name = shader.GetPropertyName(i);
										switch (name)
										{
												case SHADER_PROP_FILLED:
														filledId = shader.GetPropertyNameId(i);
														matches++;
														break;
												case SHADER_PROP_FRONT_COLOR:
														frontColorId = shader.GetPropertyNameId(i);
														matches++;
														break;
												case SHADER_PROP_TOP_COLOR:
														topColorId = shader.GetPropertyNameId(i);
														matches++;
														break;
												default:
														break;
										}
								}

								shaderPropertyIds[SHADER_PROP_FILLED] = filledId;
								shaderPropertyIds[SHADER_PROP_FRONT_COLOR] = frontColorId;
								shaderPropertyIds[SHADER_PROP_TOP_COLOR] = topColorId;
						}

						filledId = shaderPropertyIds[SHADER_PROP_FILLED];
						float progress = RemapFillingForShader();
						liquidMat.SetFloat(filledId, progress);

						frontColorId = shaderPropertyIds[SHADER_PROP_FRONT_COLOR];
						liquidMat.SetColor(frontColorId, fillingColor);

						topColorId = shaderPropertyIds[SHADER_PROP_TOP_COLOR];
						liquidMat.SetColor(topColorId, fillingColorTop);

						lightMatRenderer.material = liquidMat;
						lightMatRenderer.UpdateGIMaterials();
				}

				/// <summary>
				/// To set in editor 0 to 1 but set another range for shader. Fixes some issues to fit the filling in the body.
				/// </summary>
				private float RemapFillingForShader()
				{
						const float MIN = 0.25f;
						const float MAX = 0.355f;
						return MIN + filling * (MAX - MIN);
				}

				public override EquipmentInfo GetEquipmentInfo()
				{
						return new EquipmentInfo
						{
								Text = GetVisibleText(),
								TimerText = !IsBroken ? $"{endTimer:0} s" : null
						};
				}

				private string GetVisibleText()
				{
						if (IsBroken)
						{
								return $"{GetTargetName()} ist leer";
						}
						else
						{
								return $"{GetTargetName()} mit {filling * 100:0} %";
						}
				}

				public override bool CanInteract(PlayerBehaviour sender)
				{
						return base.CanInteract(sender) && IsUserOrNotTaken(sender);
				}

				protected override void OnPerformedDrop()
				{
						ToggleOff();
						base.OnPerformedDrop();
				}

				public override void Interact(PlayerBehaviour sender)
				{
						if (IsTakenByPlayer)
						{
								TogglePowered();

								// toggle
								if (IsPowered)
								{
										ToggleOn();
								}
								else
								{
										ToggleOff();
								}
						}
						else
						{
								ToggleOff();
						}
				}
		}
}