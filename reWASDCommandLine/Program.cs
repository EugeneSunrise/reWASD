using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using reWASDCommandLine.Verbs;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using XBEliteWPF.Infrastructure;

namespace reWASDCommandLine
{
	internal class Program
	{
		private static async Task<int> Main(string[] args)
		{
			Parser parser = new Parser(delegate(ParserSettings cfg)
			{
				cfg.CaseInsensitiveEnumValues = true;
				cfg.AutoHelp = false;
				cfg.AutoVersion = false;
			});
			ParserResult<object> res = ParserExtensions.ParseArguments<ApplyConfigVerb, SelectSlotVerb, ClearSlotVerb, RemapVerb, VersionVerb, HelpVerb>(parser, args);
			await ParserResultExtensions.WithParsedAsync<object>(res, async delegate(object o)
			{
				await Program.CheckEngine();
			});
			return ParserResultExtensions.MapResult<ApplyConfigVerb, SelectSlotVerb, ClearSlotVerb, RemapVerb, VersionVerb, HelpVerb, int>(res, (ApplyConfigVerb opts) => Program.OnConfigApplyVerb(opts).Result, (SelectSlotVerb opts) => Program.OnSelectSlotVerb(opts).Result, (ClearSlotVerb opts) => Program.OnClearSlotVerb(opts).Result, (RemapVerb opts) => Program.OnRemapVerb(opts).Result, (VersionVerb opts) => Program.OnVersionVerb(), (HelpVerb opts) => Program.OutputHelp(Program.GetHelpText()), (IEnumerable<Error> errs) => Program.HandleParseError(errs, Program.MakeHelpOnError<object>(res)));
		}

		private static HelpText GetHelpText()
		{
			HelpText helpText = new HelpText();
			helpText.AdditionalNewLineAfterOption = true;
			helpText.AddNewLineBetweenHelpSections = true;
			helpText.AutoVersion = false;
			helpText.AutoHelp = false;
			helpText.Copyright = "";
			helpText.Heading = HeadingInfo.Empty;
			helpText.MaximumDisplayWidth = 300;
			helpText.AddVerbs(new Type[]
			{
				typeof(ApplyConfigVerb),
				typeof(SelectSlotVerb),
				typeof(ClearSlotVerb),
				typeof(RemapVerb),
				typeof(VersionVerb)
			});
			return helpText;
		}

		private static HelpText MakeHelpOnError<T>(ParserResult<T> res)
		{
			return HelpText.AutoBuild<T>(res, delegate(HelpText h)
			{
				h.AdditionalNewLineAfterOption = true;
				h.AddNewLineBetweenHelpSections = true;
				h.AutoVersion = false;
				h.AutoHelp = false;
				h.Copyright = "";
				h.Heading = HeadingInfo.Empty;
				h.MaximumDisplayWidth = 300;
				h.AddVerbs(new Type[]
				{
					typeof(ApplyConfigVerb),
					typeof(SelectSlotVerb),
					typeof(ClearSlotVerb),
					typeof(RemapVerb),
					typeof(VersionVerb),
					typeof(HelpVerb)
				});
				return h;
			}, 80);
		}

		private static async Task CheckEngine()
		{
			await HttpUtils.SendRequestWithTimeout(() => HttpUtils.GetRequest("EngineService/WaitForInited"), 10);
		}

		private static async Task<int> OnConfigApplyVerb(ApplyConfigVerb opts)
		{
			ConfigApplyInfo info = new ConfigApplyInfo
			{
				Path = opts.configPath,
				Bundle = null,
				GamepadId = ((opts.id == "all_gamepad") ? null : opts.id),
				Slot = opts.slot.GetValueOrDefault()
			};
			EnableRemapBundle bundle = new EnableRemapBundle();
			for (;;)
			{
				EnableRemapResponse enableRemapResponse = await Program.SendApplyConfigRequest(info);
				if (enableRemapResponse == null)
				{
					break;
				}
				if (enableRemapResponse.IsSucceded)
				{
					goto IL_1EE;
				}
				if (enableRemapResponse.Dialogs == null || enableRemapResponse.Dialogs.Count == 0)
				{
					goto IL_117;
				}
				foreach (EnableRemapResponseDialog enableRemapResponseDialog in enableRemapResponse.Dialogs)
				{
					if (enableRemapResponseDialog.DefaultButtonNum >= 0)
					{
						try
						{
							EnableRemapResponseButton enableRemapResponseButton = enableRemapResponseDialog.Buttons[enableRemapResponseDialog.DefaultButtonNum];
							bundle.UserActions.Add(enableRemapResponseButton.ButtonAction);
							continue;
						}
						catch
						{
							Utils.OutputText(enableRemapResponseDialog.Message);
							continue;
						}
					}
					Utils.OutputText(enableRemapResponseDialog.Message);
				}
				info.Bundle = bundle;
				if (bundle.UserActions.Count == 0 || enableRemapResponse.DontReCallEnableRemap)
				{
					goto IL_1DB;
				}
				Thread.Sleep(1000);
			}
			Utils.OutputText("Can't apply config");
			return 1;
			IL_117:
			Utils.OutputText("Can't apply config");
			return 1;
			IL_1DB:
			return 1;
			IL_1EE:
			return 0;
		}

		private static async Task<EnableRemapResponse> SendApplyConfigRequest(ConfigApplyInfo info)
		{
			string json = JsonConvert.SerializeObject(info);
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(json, "AppliedConfig"));
			EnableRemapResponse enableRemapResponse;
			if (!Program.CheckResponseError(httpResponseMessage))
			{
				enableRemapResponse = null;
			}
			else
			{
				enableRemapResponse = JsonConvert.DeserializeObject<EnableRemapResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return enableRemapResponse;
		}

		private static async Task<int> OnSelectSlotVerb(SelectSlotVerb opts)
		{
			await Program.CheckEngine();
			SelectSlotInfo selectSlotInfo = new SelectSlotInfo
			{
				ID = opts.id,
				Slot = opts.slot
			};
			string json = JsonConvert.SerializeObject(selectSlotInfo);
			TaskAwaiter<HttpResponseMessage> taskAwaiter = HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(json, "SelectSlot")).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<HttpResponseMessage> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
			}
			return Program.CheckResponseError(taskAwaiter.GetResult()) ? 0 : 1;
		}

		private static async Task<int> OnClearSlotVerb(ClearSlotVerb opts)
		{
			List<Slot> list = new List<Slot>();
			if (opts.slot == "all")
			{
				using (IEnumerator enumerator = Enum.GetValues(typeof(Slot)).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						list.Add((Slot)obj);
					}
					goto IL_C1;
				}
			}
			try
			{
				Slot slot = (Slot)Enum.Parse(typeof(Slot), opts.slot, true);
				list.Add(slot);
			}
			catch (ArgumentException)
			{
				Utils.OutputText("Wrong Slot parameter passed.");
				return 1;
			}
			IL_C1:
			ClearSlotInfo clearSlotInfo = new ClearSlotInfo
			{
				ID = ((opts.id == "all") ? null : opts.id),
				Slots = list
			};
			string json = JsonConvert.SerializeObject(clearSlotInfo);
			TaskAwaiter<HttpResponseMessage> taskAwaiter = HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(json, "ClearSlot")).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<HttpResponseMessage> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
			}
			return Program.CheckResponseError(taskAwaiter.GetResult()) ? 0 : 1;
		}

		private static async Task<int> OnRemapVerb(RemapVerb opts)
		{
			int num;
			if (opts.state == "on")
			{
				num = await Program.OnEnableRemap(opts);
			}
			else
			{
				num = await Program.OnDisableRemap(opts);
			}
			return num;
		}

		private static async Task<int> OnDisableRemap(RemapVerb opts)
		{
			string json = JsonConvert.SerializeObject(new DisableRemapInfo
			{
				ID = ((opts.id == "all") ? null : opts.id)
			});
			TaskAwaiter<HttpResponseMessage> taskAwaiter = HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(json, "DisableRemap")).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<HttpResponseMessage> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
			}
			return Program.CheckResponseError(taskAwaiter.GetResult()) ? 0 : 1;
		}

		private static async Task<int> OnEnableRemap(RemapVerb opts)
		{
			EnableRemapInfo req = new EnableRemapInfo
			{
				ID = ((opts.id == "all") ? null : opts.id),
				Bundle = null
			};
			EnableRemapBundle bundle = new EnableRemapBundle();
			for (;;)
			{
				EnableRemapResponse enableRemapResponse = await Program.SendEnableRemapRequest(req);
				if (enableRemapResponse == null)
				{
					break;
				}
				if (enableRemapResponse.IsSucceded)
				{
					goto IL_1B5;
				}
				if (enableRemapResponse.Dialogs == null || enableRemapResponse.Dialogs.Count == 0)
				{
					goto IL_EA;
				}
				foreach (EnableRemapResponseDialog enableRemapResponseDialog in enableRemapResponse.Dialogs)
				{
					if (enableRemapResponseDialog.DefaultButtonNum >= 0)
					{
						try
						{
							EnableRemapResponseButton enableRemapResponseButton = enableRemapResponseDialog.Buttons[enableRemapResponseDialog.DefaultButtonNum];
							bundle.UserActions.Add(enableRemapResponseButton.ButtonAction);
							continue;
						}
						catch
						{
							Utils.OutputText(enableRemapResponseDialog.Message);
							continue;
						}
					}
					Utils.OutputText(enableRemapResponseDialog.Message);
				}
				req.Bundle = bundle;
				if (bundle.UserActions.Count == 0 || enableRemapResponse.DontReCallEnableRemap)
				{
					goto IL_1B1;
				}
			}
			Utils.OutputText("Can't enable remap");
			return 1;
			IL_EA:
			Utils.OutputText("Can't enable remap");
			return 1;
			IL_1B1:
			return 1;
			IL_1B5:
			return 0;
		}

		private static async Task<EnableRemapResponse> SendEnableRemapRequest(EnableRemapInfo info)
		{
			string json = JsonConvert.SerializeObject(info);
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(json, "EnableRemap"));
			EnableRemapResponse enableRemapResponse;
			if (!Program.CheckResponseError(httpResponseMessage))
			{
				enableRemapResponse = null;
			}
			else
			{
				enableRemapResponse = JsonConvert.DeserializeObject<EnableRemapResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return enableRemapResponse;
		}

		private static int OnVersionVerb()
		{
			Utils.OutputText(HeadingInfo.Default);
			Utils.OutputText(CopyrightInfo.Default);
			Utils.OutputText("Protocol version: v1.7");
			return 0;
		}

		private static int HandleParseError(IEnumerable<Error> _, HelpText txt)
		{
			Program.OutputHelp(txt);
			return 1;
		}

		private static int OutputHelp(HelpText txt)
		{
			Utils.OutputText(txt);
			return 0;
		}

		private static bool CheckResponseError(HttpResponseMessage resp)
		{
			if (resp == null)
			{
				return false;
			}
			if (!resp.IsSuccessStatusCode)
			{
				string text = "";
				if (resp.StatusCode == HttpStatusCode.BadRequest)
				{
					text = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Code: ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(Convert.ToInt32(resp.StatusCode));
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					defaultInterpolatedStringHandler.AppendFormatted(resp.ReasonPhrase);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				Utils.OutputText(text);
				return false;
			}
			return true;
		}

		private static int <Main>(string[] args)
		{
			return Program.Main(args).GetAwaiter().GetResult();
		}
	}
}
