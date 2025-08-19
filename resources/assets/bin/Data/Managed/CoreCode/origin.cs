using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AOT;
using BeautifulTransitions.Scripts.DisplayItem;
using BeautifulTransitions.Scripts.Helper;
using BeautifulTransitions.Scripts.Shake.Components;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.Components;
using BeautifulTransitions.Scripts.Transitions.Components.Camera;
using BeautifulTransitions.Scripts.Transitions.Components.Camera.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.Components.GameObject.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.Components.Screen;
using BeautifulTransitions.Scripts.Transitions.Components.Screen.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using GoogleARCore.Examples.Common;
using GoogleARCore.Examples.ObjectManipulationInternal;
using GoogleARCoreInternal;
using GoogleARCoreInternal.CrossPlatform;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.Video;

[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyVersion("0.0.0.0")]
public class GameManager : MonoBehaviour
{
	public static AllHistory allHistorys;

	public static int sexy = 1;

	private AudioSource myAs;

	public static GameManager instance;

	public static float musicVolumn = 1f;

	public static float effectVolumn = 1f;

	public static float fullTime = 60f;

	public string musicName = "游戏背景音乐";

	public static string path;

	public void ChangeFullTime(float gameTime)
	{
		fullTime = gameTime;
	}

	public void PlayMusic()
	{
		myAs.Play();
	}

	private void SetPath()
	{
		path = Application.persistentDataPath;
	}

	private void Awake()
	{
		SetPath();
	}

	private void Start()
	{
		myAs = base.gameObject.AddComponent<AudioSource>();
		myAs.playOnAwake = false;
		myAs.clip = Resources.Load<AudioClip>(musicName);
		myAs.loop = true;
		myAs.volume = ((musicVolumn == 1f) ? 1 : 0);
		Pause(pause: false);
		UnityEngine.Debug.Log("!!!!!!!!!!!!!!!!!!!!!!" + path);
		try
		{
			if (!File.Exists(path + TxtWriteAndRead.aimPath))
			{
				allHistorys = new AllHistory();
				string text = JsonConvert.SerializeObject(allHistorys);
				TxtWriteAndRead.ReWriteMyTxtByFileStreamTxt(text);
				UnityEngine.Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^" + text);
			}
			else
			{
				string text2 = TxtWriteAndRead.ReadTxtThird();
				UnityEngine.Debug.Log("------------------------------------" + text2);
				allHistorys = JsonConvert.DeserializeObject<AllHistory>(text2);
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("------------------------" + ex.ToString() + "------------------------");
		}
		UnityEngine.Debug.Log(allHistorys.allHistory.Count);
		instance = this;
	}

	public void ChangeVolumn(bool hasMusic)
	{
		myAs.volume = (hasMusic ? 1 : 0);
		musicVolumn = myAs.volume;
	}

	public void ChangeEffectVolumn(bool hasMusic)
	{
		effectVolumn = (hasMusic ? 1 : 0);
	}

	public void Pause(bool pause)
	{
		Time.timeScale = ((!pause) ? 1 : 0);
	}

	public void ChangeScene(string name)
	{
		SceneManager.LoadScene(name);
	}
}
public class GetEndInfo : MonoBehaviour
{
	public Text juli;

	public Text zuanshi;

	public Text kaluli;

	private void OnEnable()
	{
		SetInfo();
	}

	public void SetInfo()
	{
		juli.text = SceneAction.instance.juli.ToString("0") + "m";
		zuanshi.text = Player.instance.diaCount.ToString();
		kaluli.text = SceneAction.instance.showkaluli.text + "kcal";
	}
}
public class JuliDia : MonoBehaviour
{
	public Text juli;

	public Text dia;

	private void OnEnable()
	{
		juli.text = PlayerPrefs.GetInt("dis").ToString();
		dia.text = PlayerPrefs.GetInt("dia").ToString();
	}
}
public class Player : MonoBehaviour
{
	public GameObject[] threeCube;

	public Transform hpTrans;

	private List<GameObject> hp = new List<GameObject>();

	public Text showDiaNum;

	private float middleMin;

	private float middleMax;

	public int diaCount;

	private AudioSource effectAs;

	private AudioClip rightClip;

	private AudioClip falseClip;

	private AudioClip failedClip;

	public AudioClip duckClip;

	public static Player instance;

	public GameObject scussedUi;

	public GameObject failedUi;

	private void Awake()
	{
		GameObject[] array = threeCube;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		float num = (threeCube[1].transform.position.x - threeCube[0].transform.position.x) / 2f;
		float num2 = (threeCube[2].transform.position.x - threeCube[1].transform.position.x) / 2f;
		middleMin = threeCube[1].transform.position.x - num;
		middleMax = threeCube[1].transform.position.x + num2;
		for (int j = 0; j < hpTrans.childCount; j++)
		{
			hp.Add(hpTrans.GetChild(j).gameObject);
		}
		scussedUi.SetActive(value: false);
		failedUi.SetActive(value: false);
		showDiaNum.text = diaCount.ToString();
		rightClip = Resources.Load<AudioClip>("互动音效(胜利过关卡时播放)");
		falseClip = Resources.Load<AudioClip>("游戏失败音效");
		failedClip = Resources.Load<AudioClip>("游戏结束音效");
		duckClip = Resources.Load<AudioClip>("胜利欢呼声");
		effectAs = base.gameObject.AddComponent<AudioSource>();
		effectAs.playOnAwake = false;
		effectAs.loop = false;
		effectAs.clip = rightClip;
		effectAs.volume = GameManager.effectVolumn;
		instance = this;
	}

	public void PlayEffect(AudioClip ac)
	{
		if (effectAs.clip != ac)
		{
			effectAs.clip = ac;
		}
		effectAs.Play();
	}

	private void Update()
	{
		if (base.transform.position.x > middleMin && base.transform.position.x < middleMax)
		{
			if (!threeCube[1].activeSelf)
			{
				GameObject[] array = threeCube;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
				threeCube[1].SetActive(value: true);
			}
		}
		else if (base.transform.position.x <= middleMin)
		{
			if (!threeCube[0].activeSelf)
			{
				GameObject[] array = threeCube;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
				threeCube[0].SetActive(value: true);
			}
		}
		else if (base.transform.position.x >= middleMax && !threeCube[2].activeSelf)
		{
			GameObject[] array = threeCube;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			threeCube[2].SetActive(value: true);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		switch (other.tag)
		{
		case "dia":
			PlayEffect(rightClip);
			UnityEngine.Debug.Log("dia");
			diaCount++;
			showDiaNum.text = diaCount.ToString();
			UnityEngine.Object.Destroy(other.gameObject);
			return;
		case "duck":
			PlayEffect(rightClip);
			UnityEngine.Object.Destroy(other.gameObject);
			return;
		case "muqin":
			PlayEffect(rightClip);
			return;
		}
		PlayEffect(falseClip);
		if (hp.Count == 0)
		{
			UnityEngine.Object.Destroy(this);
			SceneAction.instance.StopAllCoroutines();
			PlayEffect(failedClip);
			failedUi.gameObject.SetActive(value: true);
			int num = PlayerPrefs.GetInt("dia");
			PlayerPrefs.SetInt("dia", diaCount + num);
			int num2 = PlayerPrefs.GetInt("dis");
			PlayerPrefs.SetInt("dis", num2 + (int)SceneAction.instance.juli);
			SceneAction.instance.AddJson();
		}
		else
		{
			UnityEngine.Object.Destroy(hp[hp.Count - 1].gameObject);
			hp.RemoveAt(hp.Count - 1);
		}
	}
}
public class SceneAction : MonoBehaviour
{
	public GameObject headUi;

	private Vector3 headStartLocalPos;

	public Vector3 headEndLocalPos;

	private Vector3 dirHead;

	private float disHead;

	public Vector3 aimLocalPos;

	public Text showkaluli;

	private float kaluli;

	private Vector3 localdir;

	private float dis;

	private Vector3 startLocalPos;

	private float lastTime;

	private float bmr;

	public Text showdis;

	public static SceneAction instance;

	public float juli;

	public void AddJson()
	{
		History history = new History();
		history.kaluli = kaluli;
		history.miShu = dis;
		GameManager.allHistorys.allHistory.Add(history);
		for (int i = 0; i < GameManager.allHistorys.allHistory.Count - 1; i++)
		{
			for (int j = 0; j < GameManager.allHistorys.allHistory.Count - 1 - i; j++)
			{
				if (GameManager.allHistorys.allHistory[j].kaluli > GameManager.allHistorys.allHistory[j + 1].kaluli)
				{
					History value = GameManager.allHistorys.allHistory[j];
					GameManager.allHistorys.allHistory[j] = GameManager.allHistorys.allHistory[j + 1];
					GameManager.allHistorys.allHistory[j + 1] = value;
				}
			}
		}
		GameManager.allHistorys.allHistory.Reverse();
		if (GameManager.allHistorys.allHistory.Count > 10)
		{
			int num = GameManager.allHistorys.allHistory.Count - 10;
			for (int k = 0; k < num; k++)
			{
				GameManager.allHistorys.allHistory.RemoveAt(GameManager.allHistorys.allHistory.Count - 1);
			}
		}
		string text = JsonConvert.SerializeObject(GameManager.allHistorys);
		TxtWriteAndRead.ReWriteMyTxtByFileStreamTxt(text);
		UnityEngine.Debug.Log(text);
	}

	private void Start()
	{
		bmr = (bmr = 13.75f * PeopleInfo.tizhong + 5f * PeopleInfo.shengao - 6.67f * PeopleInfo.nianling + 66f);
		kaluli = 0f;
		showkaluli.text = kaluli.ToString();
		startLocalPos = base.transform.localPosition;
		headStartLocalPos = headUi.transform.localPosition;
		localdir = (aimLocalPos - startLocalPos).normalized;
		dirHead = (headEndLocalPos - headStartLocalPos).normalized;
		dis = Vector3.Distance(aimLocalPos, startLocalPos);
		disHead = Vector3.Distance(headEndLocalPos, headStartLocalPos);
		lastTime = GameManager.fullTime;
		showdis.text = "0";
		showkaluli.text = "0";
		instance = this;
	}

	[ContextMenu("go")]
	public void Move()
	{
		StopAllCoroutines();
		StartCoroutine(MoveIE());
	}

	private IEnumerator MoveIE()
	{
		startLocalPos = base.transform.localPosition;
		lastTime = GameManager.fullTime;
		kaluli = 0f;
		showkaluli.text = kaluli.ToString();
		float num;
		do
		{
			yield return new WaitForSeconds(Time.deltaTime);
			lastTime -= Time.deltaTime;
			num = (GameManager.fullTime - lastTime) / GameManager.fullTime;
			Vector3 localPosition = base.transform.localPosition;
			base.transform.localPosition = startLocalPos + localdir * dis * num;
			Vector3 localPosition2 = headStartLocalPos + dirHead * disHead * num;
			juli = num * dis;
			showdis.text = juli.ToString("0");
			headUi.transform.localPosition = localPosition2;
			Vector3.Distance(localPosition, base.transform.localPosition);
			float num2 = bmr / 24f * 4.8f * Time.deltaTime / 3600f * 2f;
			kaluli += num2;
			showkaluli.text = kaluli.ToString("0.00");
		}
		while (!(num >= 1f));
		Player.instance.PlayEffect(Player.instance.duckClip);
		Player.instance.scussedUi.SetActive(value: true);
		int num3 = PlayerPrefs.GetInt("dia");
		PlayerPrefs.SetInt("dia", Player.instance.diaCount + num3);
		int num4 = PlayerPrefs.GetInt("dis");
		PlayerPrefs.SetInt("dis", num4 + (int)juli);
		AddJson();
		yield return null;
	}
}
public class PeopleInfo
{
	public static float tizhong = 50f;

	public static float shengao = 172f;

	public static float nianling = 20f;
}
public class AllHistory
{
	public List<History> allHistory;

	public AllHistory()
	{
		allHistory = new List<History>();
	}
}
public class History
{
	public float kaluli;

	public float miShu;
}
public class SetHistory : MonoBehaviour
{
	public Transform father;

	public GameObject model;

	private List<GameObject> lines = new List<GameObject>();

	private void Start()
	{
		int num = 0;
		foreach (History item in GameManager.allHistorys.allHistory)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(model, father);
			Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>();
			componentsInChildren[0].text = (num + 1).ToString();
			num++;
			string text = item.kaluli.ToString("0.00") + "kcal";
			string text2 = (int)item.miShu + "m";
			int num2 = text.Length + text2.Length;
			if (num2 < 23)
			{
				string text3 = "";
				int num3 = 23 - num2;
				for (int i = 0; i < num3; i++)
				{
					text3 += " ";
				}
				componentsInChildren[1].text = text + text3 + text2;
			}
			else
			{
				componentsInChildren[1].text = text + " " + text2;
			}
			lines.Add(gameObject);
		}
	}
}
public class SettingReset : MonoBehaviour
{
	public GameObject[] music;

	public GameObject[] effect;

	public ShowDifferentPage sdp;

	public ShowDifferentPage sdp2;

	private void Start()
	{
		sdp2.ChangePageTo(GameManager.sexy);
		if (GameManager.effectVolumn == 1f)
		{
			effect[0].SetActive(value: true);
			effect[1].SetActive(value: false);
		}
		else
		{
			effect[0].SetActive(value: false);
			effect[1].SetActive(value: true);
		}
		if (GameManager.musicVolumn == 1f)
		{
			music[0].SetActive(value: true);
			music[1].SetActive(value: false);
		}
		else
		{
			music[0].SetActive(value: false);
			music[1].SetActive(value: true);
		}
		float fullTime = GameManager.fullTime;
		if (!40f.Equals(fullTime))
		{
			if (!30f.Equals(fullTime))
			{
				if (20f.Equals(fullTime))
				{
					sdp.ChangePageTo(3);
				}
				else
				{
					sdp.ChangePageTo(1);
				}
			}
			else
			{
				sdp.ChangePageTo(2);
			}
		}
		else
		{
			sdp.ChangePageTo(1);
		}
	}

	public void ChangeSexy()
	{
		if (GameManager.sexy == 1)
		{
			sdp2.ChangePageTo(2);
			GameManager.sexy = 2;
		}
		else
		{
			sdp2.ChangePageTo(1);
			GameManager.sexy = 1;
		}
	}
}
public class ShowDifferentPage : MonoBehaviour
{
	public List<GameObject> pages = new List<GameObject>();

	public float delayTime = 2f;

	private void Start()
	{
	}

	public void SetGameObjectTrue(GameObject gameObj)
	{
		gameObj.SetActive(value: true);
	}

	public void SetGameObjectFalse(GameObject gameObj)
	{
		gameObj.SetActive(value: false);
	}

	private void Update()
	{
	}

	public void ChangePageByPageName(string _name)
	{
		for (int i = 0; i < pages.Count; i++)
		{
			if (pages[i].name == _name)
			{
				ChangePageTo(i + 1);
				break;
			}
		}
	}

	public void ChangePageTo(int _pageNum)
	{
		if (_pageNum <= 0 || _pageNum > pages.Count)
		{
			return;
		}
		for (int i = 0; i < pages.Count; i++)
		{
			if (_pageNum - 1 == i)
			{
				pages[i].SetActive(value: true);
			}
			else
			{
				pages[i].SetActive(value: false);
			}
		}
	}

	public void TwoPageChangeToAnother()
	{
		if (pages.Count == 2)
		{
			if (pages[0].activeSelf)
			{
				pages[0].SetActive(value: false);
				pages[1].SetActive(value: true);
			}
			else
			{
				pages[1].SetActive(value: false);
				pages[0].SetActive(value: true);
			}
		}
		else
		{
			UnityEngine.Debug.Log("两个页面没有切换");
		}
	}

	public void Exit()
	{
		Application.Quit();
	}

	public void CloseAllPages()
	{
		foreach (GameObject page in pages)
		{
			page.SetActive(value: false);
		}
	}

	public void AddChangePage()
	{
		for (int i = 0; i < pages.Count; i++)
		{
			if (pages[i].activeInHierarchy)
			{
				int num = i + 2;
				if (num != pages.Count + 1)
				{
					ChangePageTo(num);
				}
				else
				{
					ChangePageTo(1);
				}
				break;
			}
		}
	}

	private void OnEnable()
	{
		StopAllCoroutines();
	}

	public void DelayChangePage(int page)
	{
		StartCoroutine(Wait(page));
	}

	private IEnumerator Wait(int page)
	{
		yield return new WaitForSeconds(delayTime);
		ChangePageTo(page);
	}

	public void ReduceChangePage()
	{
		for (int i = 0; i < pages.Count; i++)
		{
			if (pages[i].activeInHierarchy)
			{
				int num = i;
				if (num != 0)
				{
					ChangePageTo(num);
				}
				else
				{
					ChangePageTo(pages.Count);
				}
				break;
			}
		}
	}
}
public class ShowDir : MonoBehaviour
{
	private Text text;

	private void Start()
	{
		base.transform.GetComponent<Text>().text = GameManager.path;
	}

	private void Update()
	{
	}
}
public static class TriggleSetting
{
	public static void AddListener(this Component obj, EventTriggerType eventTriggerType, UnityAction<BaseEventData> callback)
	{
		EventTrigger eventTrigger = obj.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = obj.gameObject.AddComponent<EventTrigger>();
		}
		List<EventTrigger.Entry> list = eventTrigger.triggers;
		if (list == null)
		{
			list = new List<EventTrigger.Entry>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].eventID == eventTriggerType)
			{
				entry = list[i];
				flag = true;
			}
		}
		entry.callback.AddListener(callback);
		if (!flag)
		{
			eventTrigger.triggers.Add(entry);
		}
	}
}
public class TxtWriteAndRead : MonoBehaviour
{
	private TextAsset m_Txt;

	public static string aimPath = "/history.txt";

	private void Start()
	{
		ReadTxtFifth();
	}

	public static void AddTxtTextByFileStream(string txtText)
	{
		FileStream fileStream = new FileStream(GameManager.path + aimPath, FileMode.Create);
		byte[] bytes = Encoding.UTF8.GetBytes(txtText);
		fileStream.Write(bytes, 0, bytes.Length);
		if (fileStream != null)
		{
			fileStream.Flush();
			fileStream.Close();
			fileStream.Dispose();
		}
	}

	public static void ReWriteMyTxtByFileStreamTxt(string content)
	{
		string path = GameManager.path + aimPath;
		if (!File.Exists(path))
		{
			AddTxtTextByFileStream("");
		}
		File.WriteAllText(path, content);
	}

	public void AddTxtTextByFileInfo(string txtText)
	{
		string text = GameManager.path + aimPath;
		FileInfo fileInfo = new FileInfo(text);
		StreamWriter streamWriter = (File.Exists(text) ? fileInfo.AppendText() : fileInfo.CreateText());
		streamWriter.WriteLine(txtText);
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private void ReadTxtFirst()
	{
		m_Txt = Resources.Load<TextAsset>("readTxt");
		MonoBehaviour.print(m_Txt.text.ToString());
	}

	private void ReadTxtSecond()
	{
		string[] array = File.ReadAllLines(GameManager.path + aimPath);
		for (int i = 0; i < array.Length; i++)
		{
			MonoBehaviour.print(array[i]);
		}
	}

	public static string ReadTxtThird()
	{
		return File.ReadAllText(GameManager.path + aimPath, Encoding.UTF8);
	}

	private void ReadTxtForth()
	{
		MonoBehaviour.print(new StreamReader(GameManager.path + aimPath).ReadToEnd());
	}

	private void ReadTxtFifth()
	{
		FileStream fileStream = new FileStream(GameManager.path + aimPath, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, array.Length);
		fileStream.Close();
		MonoBehaviour.print(Encoding.UTF8.GetString(array));
	}
}
public class MultiKeyDictionary<T1, T2, T3> : Dictionary<T1, Dictionary<T2, T3>>
{
	public new Dictionary<T2, T3> this[T1 key]
	{
		get
		{
			if (!ContainsKey(key))
			{
				Add(key, new Dictionary<T2, T3>());
			}
			TryGetValue(key, out var value);
			return value;
		}
	}

	public bool ContainsKey(T1 key1, T2 key2)
	{
		TryGetValue(key1, out var value);
		return value?.ContainsKey(key2) ?? false;
	}
}
[Serializable]
public class Images
{
	public Texture2D clearImage;

	public Texture2D collapseImage;

	public Texture2D clearOnNewSceneImage;

	public Texture2D showTimeImage;

	public Texture2D showSceneImage;

	public Texture2D userImage;

	public Texture2D showMemoryImage;

	public Texture2D softwareImage;

	public Texture2D dateImage;

	public Texture2D showFpsImage;

	public Texture2D infoImage;

	public Texture2D saveLogsImage;

	public Texture2D searchImage;

	public Texture2D copyImage;

	public Texture2D closeImage;

	public Texture2D buildFromImage;

	public Texture2D systemInfoImage;

	public Texture2D graphicsInfoImage;

	public Texture2D backImage;

	public Texture2D logImage;

	public Texture2D warningImage;

	public Texture2D errorImage;

	public Texture2D barImage;

	public Texture2D button_activeImage;

	public Texture2D even_logImage;

	public Texture2D odd_logImage;

	public Texture2D selectedImage;

	public GUISkin reporterScrollerSkin;
}
public class Reporter : MonoBehaviour
{
	public enum _LogType
	{
		Assert = 1,
		Error = 0,
		Exception = 4,
		Log = 3,
		Warning = 2
	}

	public class Sample
	{
		public float time;

		public byte loadedScene;

		public float memory;

		public float fps;

		public string fpsText;

		public static float MemSize()
		{
			return 13f;
		}

		public string GetSceneName()
		{
			if (loadedScene == byte.MaxValue)
			{
				return "AssetBundleScene";
			}
			return scenes[loadedScene];
		}
	}

	public class Log
	{
		public int count = 1;

		public _LogType logType;

		public string condition;

		public string stacktrace;

		public int sampleId;

		public Log CreateCopy()
		{
			return (Log)MemberwiseClone();
		}

		public float GetMemoryUsage()
		{
			return 8 + condition.Length * 2 + stacktrace.Length * 2 + 4;
		}
	}

	private enum ReportView
	{
		None,
		Logs,
		Info,
		Snapshot
	}

	private enum DetailView
	{
		None,
		StackTrace,
		Graph
	}

	private List<Sample> samples = new List<Sample>();

	private List<Log> logs = new List<Log>();

	private List<Log> collapsedLogs = new List<Log>();

	private List<Log> currentLog = new List<Log>();

	private MultiKeyDictionary<string, string, Log> logsDic = new MultiKeyDictionary<string, string, Log>();

	private Dictionary<string, string> cachedString = new Dictionary<string, string>();

	[HideInInspector]
	public bool show;

	private bool collapse;

	private bool clearOnNewSceneLoaded;

	private bool showTime;

	private bool showScene;

	private bool showMemory;

	private bool showFps;

	private bool showGraph;

	private bool showLog = true;

	private bool showWarning = true;

	private bool showError = true;

	private int numOfLogs;

	private int numOfLogsWarning;

	private int numOfLogsError;

	private int numOfCollapsedLogs;

	private int numOfCollapsedLogsWarning;

	private int numOfCollapsedLogsError;

	private bool showClearOnNewSceneLoadedButton = true;

	private bool showTimeButton = true;

	private bool showSceneButton = true;

	private bool showMemButton = true;

	private bool showFpsButton = true;

	private bool showSearchText = true;

	private bool showCopyButton = true;

	private bool showSaveButton = true;

	private string buildDate;

	private string logDate;

	private float logsMemUsage;

	private float graphMemUsage;

	private float gcTotalMemory;

	public string UserData = "";

	public float fps;

	public string fpsText;

	private ReportView currentView = ReportView.Logs;

	private static bool created;

	public Images images;

	private GUIContent clearContent;

	private GUIContent collapseContent;

	private GUIContent clearOnNewSceneContent;

	private GUIContent showTimeContent;

	private GUIContent showSceneContent;

	private GUIContent userContent;

	private GUIContent showMemoryContent;

	private GUIContent softwareContent;

	private GUIContent dateContent;

	private GUIContent showFpsContent;

	private GUIContent infoContent;

	private GUIContent saveLogsContent;

	private GUIContent searchContent;

	private GUIContent copyContent;

	private GUIContent closeContent;

	private GUIContent buildFromContent;

	private GUIContent systemInfoContent;

	private GUIContent graphicsInfoContent;

	private GUIContent backContent;

	private GUIContent logContent;

	private GUIContent warningContent;

	private GUIContent errorContent;

	private GUIStyle barStyle;

	private GUIStyle buttonActiveStyle;

	private GUIStyle nonStyle;

	private GUIStyle lowerLeftFontStyle;

	private GUIStyle backStyle;

	private GUIStyle evenLogStyle;

	private GUIStyle oddLogStyle;

	private GUIStyle logButtonStyle;

	private GUIStyle selectedLogStyle;

	private GUIStyle selectedLogFontStyle;

	private GUIStyle stackLabelStyle;

	private GUIStyle scrollerStyle;

	private GUIStyle searchStyle;

	private GUIStyle sliderBackStyle;

	private GUIStyle sliderThumbStyle;

	private GUISkin toolbarScrollerSkin;

	private GUISkin logScrollerSkin;

	private GUISkin graphScrollerSkin;

	public Vector2 size = new Vector2(32f, 32f);

	public float maxSize = 20f;

	public int numOfCircleToShow = 1;

	private static string[] scenes;

	private string currentScene;

	private string filterText = "";

	private string deviceModel;

	private string deviceType;

	private string deviceName;

	private string graphicsMemorySize;

	private string maxTextureSize;

	private string systemMemorySize;

	public bool Initialized;

	private Rect screenRect = Rect.zero;

	private Rect toolBarRect = Rect.zero;

	private Rect logsRect = Rect.zero;

	private Rect stackRect = Rect.zero;

	private Rect graphRect = Rect.zero;

	private Rect graphMinRect = Rect.zero;

	private Rect graphMaxRect = Rect.zero;

	private Rect buttomRect = Rect.zero;

	private Vector2 stackRectTopLeft;

	private Rect detailRect = Rect.zero;

	private Vector2 scrollPosition;

	private Vector2 scrollPosition2;

	private Vector2 toolbarScrollPosition;

	private Log selectedLog;

	private float toolbarOldDrag;

	private float oldDrag;

	private float oldDrag2;

	private float oldDrag3;

	private int startIndex;

	private Rect countRect = Rect.zero;

	private Rect timeRect = Rect.zero;

	private Rect timeLabelRect = Rect.zero;

	private Rect sceneRect = Rect.zero;

	private Rect sceneLabelRect = Rect.zero;

	private Rect memoryRect = Rect.zero;

	private Rect memoryLabelRect = Rect.zero;

	private Rect fpsRect = Rect.zero;

	private Rect fpsLabelRect = Rect.zero;

	private GUIContent tempContent = new GUIContent();

	private Vector2 infoScrollPosition;

	private Vector2 oldInfoDrag;

	private Rect tempRect;

	private float graphSize = 4f;

	private int startFrame;

	private int currentFrame;

	private Vector3 tempVector1;

	private Vector3 tempVector2;

	private Vector2 graphScrollerPos;

	private float maxFpsValue;

	private float minFpsValue;

	private float maxMemoryValue;

	private float minMemoryValue;

	private List<Vector2> gestureDetector = new List<Vector2>();

	private Vector2 gestureSum = Vector2.zero;

	private float gestureLength;

	private int gestureCount;

	private float lastClickTime = -1f;

	private Vector2 startPos;

	private Vector2 downPos;

	private Vector2 mousePosition;

	private int frames;

	private bool firstTime = true;

	private float lastUpdate;

	private const int requiredFrames = 10;

	private const float updateInterval = 0.25f;

	private List<Log> threadedLogs = new List<Log>();

	public float TotalMemUsage => logsMemUsage + graphMemUsage;

	private void Awake()
	{
		if (!Initialized)
		{
			Initialize();
		}
		SceneManager.sceneLoaded += _OnLevelWasLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= _OnLevelWasLoaded;
	}

	private void OnEnable()
	{
		if (logs.Count == 0)
		{
			clear();
		}
	}

	private void OnDisable()
	{
	}

	private void addSample()
	{
		Sample sample = new Sample();
		sample.fps = fps;
		sample.fpsText = fpsText;
		sample.loadedScene = (byte)SceneManager.GetActiveScene().buildIndex;
		sample.time = Time.realtimeSinceStartup;
		sample.memory = gcTotalMemory;
		samples.Add(sample);
		graphMemUsage = (float)samples.Count * Sample.MemSize() / 1024f / 1024f;
	}

	public void Initialize()
	{
		if (!created)
		{
			try
			{
				base.gameObject.SendMessage("OnPreStart");
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
			scenes = new string[SceneManager.sceneCountInBuildSettings];
			currentScene = SceneManager.GetActiveScene().name;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Application.logMessageReceivedThreaded += CaptureLogThread;
			created = true;
			clearContent = new GUIContent("", images.clearImage, "Clear logs");
			collapseContent = new GUIContent("", images.collapseImage, "Collapse logs");
			clearOnNewSceneContent = new GUIContent("", images.clearOnNewSceneImage, "Clear logs on new scene loaded");
			showTimeContent = new GUIContent("", images.showTimeImage, "Show Hide Time");
			showSceneContent = new GUIContent("", images.showSceneImage, "Show Hide Scene");
			showMemoryContent = new GUIContent("", images.showMemoryImage, "Show Hide Memory");
			softwareContent = new GUIContent("", images.softwareImage, "Software");
			dateContent = new GUIContent("", images.dateImage, "Date");
			showFpsContent = new GUIContent("", images.showFpsImage, "Show Hide fps");
			infoContent = new GUIContent("", images.infoImage, "Information about application");
			saveLogsContent = new GUIContent("", images.saveLogsImage, "Save logs to device");
			searchContent = new GUIContent("", images.searchImage, "Search for logs");
			copyContent = new GUIContent("", images.copyImage, "Copy log to clipboard");
			closeContent = new GUIContent("", images.closeImage, "Hide logs");
			userContent = new GUIContent("", images.userImage, "User");
			buildFromContent = new GUIContent("", images.buildFromImage, "Build From");
			systemInfoContent = new GUIContent("", images.systemInfoImage, "System Info");
			graphicsInfoContent = new GUIContent("", images.graphicsInfoImage, "Graphics Info");
			backContent = new GUIContent("", images.backImage, "Back");
			logContent = new GUIContent("", images.logImage, "show or hide logs");
			warningContent = new GUIContent("", images.warningImage, "show or hide warnings");
			errorContent = new GUIContent("", images.errorImage, "show or hide errors");
			currentView = (ReportView)PlayerPrefs.GetInt("Reporter_currentView", 1);
			show = PlayerPrefs.GetInt("Reporter_show") == 1;
			collapse = PlayerPrefs.GetInt("Reporter_collapse") == 1;
			clearOnNewSceneLoaded = PlayerPrefs.GetInt("Reporter_clearOnNewSceneLoaded") == 1;
			showTime = PlayerPrefs.GetInt("Reporter_showTime") == 1;
			showScene = PlayerPrefs.GetInt("Reporter_showScene") == 1;
			showMemory = PlayerPrefs.GetInt("Reporter_showMemory") == 1;
			showFps = PlayerPrefs.GetInt("Reporter_showFps") == 1;
			showGraph = PlayerPrefs.GetInt("Reporter_showGraph") == 1;
			showLog = PlayerPrefs.GetInt("Reporter_showLog", 1) == 1;
			showWarning = PlayerPrefs.GetInt("Reporter_showWarning", 1) == 1;
			showError = PlayerPrefs.GetInt("Reporter_showError", 1) == 1;
			filterText = PlayerPrefs.GetString("Reporter_filterText");
			size.x = (size.y = PlayerPrefs.GetFloat("Reporter_size", 32f));
			showClearOnNewSceneLoadedButton = PlayerPrefs.GetInt("Reporter_showClearOnNewSceneLoadedButton", 1) == 1;
			showTimeButton = PlayerPrefs.GetInt("Reporter_showTimeButton", 1) == 1;
			showSceneButton = PlayerPrefs.GetInt("Reporter_showSceneButton", 1) == 1;
			showMemButton = PlayerPrefs.GetInt("Reporter_showMemButton", 1) == 1;
			showFpsButton = PlayerPrefs.GetInt("Reporter_showFpsButton", 1) == 1;
			showSearchText = PlayerPrefs.GetInt("Reporter_showSearchText", 1) == 1;
			showCopyButton = PlayerPrefs.GetInt("Reporter_showCopyButton", 1) == 1;
			showSaveButton = PlayerPrefs.GetInt("Reporter_showSaveButton", 1) == 1;
			initializeStyle();
			Initialized = true;
			if (show)
			{
				doShow();
			}
			deviceModel = SystemInfo.deviceModel.ToString();
			deviceType = SystemInfo.deviceType.ToString();
			deviceName = SystemInfo.deviceName.ToString();
			graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
			maxTextureSize = SystemInfo.maxTextureSize.ToString();
			systemMemorySize = SystemInfo.systemMemorySize.ToString();
		}
		else
		{
			UnityEngine.Debug.LogWarning("tow manager is exists delete the second");
			UnityEngine.Object.DestroyImmediate(base.gameObject, allowDestroyingAssets: true);
		}
	}

	private void initializeStyle()
	{
		int num = (int)(size.x * 0.2f);
		int num2 = (int)(size.y * 0.2f);
		nonStyle = new GUIStyle();
		nonStyle.clipping = TextClipping.Clip;
		nonStyle.border = new RectOffset(0, 0, 0, 0);
		nonStyle.normal.background = null;
		nonStyle.fontSize = (int)(size.y / 2f);
		nonStyle.alignment = TextAnchor.MiddleCenter;
		lowerLeftFontStyle = new GUIStyle();
		lowerLeftFontStyle.clipping = TextClipping.Clip;
		lowerLeftFontStyle.border = new RectOffset(0, 0, 0, 0);
		lowerLeftFontStyle.normal.background = null;
		lowerLeftFontStyle.fontSize = (int)(size.y / 2f);
		lowerLeftFontStyle.fontStyle = FontStyle.Bold;
		lowerLeftFontStyle.alignment = TextAnchor.LowerLeft;
		barStyle = new GUIStyle();
		barStyle.border = new RectOffset(1, 1, 1, 1);
		barStyle.normal.background = images.barImage;
		barStyle.active.background = images.button_activeImage;
		barStyle.alignment = TextAnchor.MiddleCenter;
		barStyle.margin = new RectOffset(1, 1, 1, 1);
		barStyle.clipping = TextClipping.Clip;
		barStyle.fontSize = (int)(size.y / 2f);
		buttonActiveStyle = new GUIStyle();
		buttonActiveStyle.border = new RectOffset(1, 1, 1, 1);
		buttonActiveStyle.normal.background = images.button_activeImage;
		buttonActiveStyle.alignment = TextAnchor.MiddleCenter;
		buttonActiveStyle.margin = new RectOffset(1, 1, 1, 1);
		buttonActiveStyle.fontSize = (int)(size.y / 2f);
		backStyle = new GUIStyle();
		backStyle.normal.background = images.even_logImage;
		backStyle.clipping = TextClipping.Clip;
		backStyle.fontSize = (int)(size.y / 2f);
		evenLogStyle = new GUIStyle();
		evenLogStyle.normal.background = images.even_logImage;
		evenLogStyle.fixedHeight = size.y;
		evenLogStyle.clipping = TextClipping.Clip;
		evenLogStyle.alignment = TextAnchor.UpperLeft;
		evenLogStyle.imagePosition = ImagePosition.ImageLeft;
		evenLogStyle.fontSize = (int)(size.y / 2f);
		oddLogStyle = new GUIStyle();
		oddLogStyle.normal.background = images.odd_logImage;
		oddLogStyle.fixedHeight = size.y;
		oddLogStyle.clipping = TextClipping.Clip;
		oddLogStyle.alignment = TextAnchor.UpperLeft;
		oddLogStyle.imagePosition = ImagePosition.ImageLeft;
		oddLogStyle.fontSize = (int)(size.y / 2f);
		logButtonStyle = new GUIStyle();
		logButtonStyle.fixedHeight = size.y;
		logButtonStyle.clipping = TextClipping.Clip;
		logButtonStyle.alignment = TextAnchor.UpperLeft;
		logButtonStyle.fontSize = (int)(size.y / 2f);
		logButtonStyle.padding = new RectOffset(num, num, num2, num2);
		selectedLogStyle = new GUIStyle();
		selectedLogStyle.normal.background = images.selectedImage;
		selectedLogStyle.fixedHeight = size.y;
		selectedLogStyle.clipping = TextClipping.Clip;
		selectedLogStyle.alignment = TextAnchor.UpperLeft;
		selectedLogStyle.normal.textColor = Color.white;
		selectedLogStyle.fontSize = (int)(size.y / 2f);
		selectedLogFontStyle = new GUIStyle();
		selectedLogFontStyle.normal.background = images.selectedImage;
		selectedLogFontStyle.fixedHeight = size.y;
		selectedLogFontStyle.clipping = TextClipping.Clip;
		selectedLogFontStyle.alignment = TextAnchor.UpperLeft;
		selectedLogFontStyle.normal.textColor = Color.white;
		selectedLogFontStyle.fontSize = (int)(size.y / 2f);
		selectedLogFontStyle.padding = new RectOffset(num, num, num2, num2);
		stackLabelStyle = new GUIStyle();
		stackLabelStyle.wordWrap = true;
		stackLabelStyle.fontSize = (int)(size.y / 2f);
		stackLabelStyle.padding = new RectOffset(num, num, num2, num2);
		scrollerStyle = new GUIStyle();
		scrollerStyle.normal.background = images.barImage;
		searchStyle = new GUIStyle();
		searchStyle.clipping = TextClipping.Clip;
		searchStyle.alignment = TextAnchor.LowerCenter;
		searchStyle.fontSize = (int)(size.y / 2f);
		searchStyle.wordWrap = true;
		sliderBackStyle = new GUIStyle();
		sliderBackStyle.normal.background = images.barImage;
		sliderBackStyle.fixedHeight = size.y;
		sliderBackStyle.border = new RectOffset(1, 1, 1, 1);
		sliderThumbStyle = new GUIStyle();
		sliderThumbStyle.normal.background = images.selectedImage;
		sliderThumbStyle.fixedWidth = size.x;
		GUISkin reporterScrollerSkin = images.reporterScrollerSkin;
		toolbarScrollerSkin = UnityEngine.Object.Instantiate(reporterScrollerSkin);
		toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
		toolbarScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
		toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
		toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;
		logScrollerSkin = UnityEngine.Object.Instantiate(reporterScrollerSkin);
		logScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2f;
		logScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
		logScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2f;
		logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;
		graphScrollerSkin = UnityEngine.Object.Instantiate(reporterScrollerSkin);
		graphScrollerSkin.verticalScrollbar.fixedWidth = 0f;
		graphScrollerSkin.horizontalScrollbar.fixedHeight = size.x * 2f;
		graphScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
		graphScrollerSkin.horizontalScrollbarThumb.fixedHeight = size.x * 2f;
	}

	private void Start()
	{
		logDate = DateTime.Now.ToString();
		StartCoroutine("readInfo");
	}

	private void clear()
	{
		logs.Clear();
		collapsedLogs.Clear();
		currentLog.Clear();
		logsDic.Clear();
		selectedLog = null;
		numOfLogs = 0;
		numOfLogsWarning = 0;
		numOfLogsError = 0;
		numOfCollapsedLogs = 0;
		numOfCollapsedLogsWarning = 0;
		numOfCollapsedLogsError = 0;
		logsMemUsage = 0f;
		graphMemUsage = 0f;
		samples.Clear();
		GC.Collect();
		selectedLog = null;
	}

	private void calculateCurrentLog()
	{
		bool flag = !string.IsNullOrEmpty(filterText);
		string value = "";
		if (flag)
		{
			value = filterText.ToLower();
		}
		currentLog.Clear();
		if (collapse)
		{
			for (int i = 0; i < collapsedLogs.Count; i++)
			{
				Log log = collapsedLogs[i];
				if ((log.logType == _LogType.Log && !showLog) || (log.logType == _LogType.Warning && !showWarning) || (log.logType == _LogType.Error && !showError) || (log.logType == _LogType.Assert && !showError) || (log.logType == _LogType.Exception && !showError))
				{
					continue;
				}
				if (flag)
				{
					if (log.condition.ToLower().Contains(value))
					{
						currentLog.Add(log);
					}
				}
				else
				{
					currentLog.Add(log);
				}
			}
		}
		else
		{
			for (int j = 0; j < logs.Count; j++)
			{
				Log log2 = logs[j];
				if ((log2.logType == _LogType.Log && !showLog) || (log2.logType == _LogType.Warning && !showWarning) || (log2.logType == _LogType.Error && !showError) || (log2.logType == _LogType.Assert && !showError) || (log2.logType == _LogType.Exception && !showError))
				{
					continue;
				}
				if (flag)
				{
					if (log2.condition.ToLower().Contains(value))
					{
						currentLog.Add(log2);
					}
				}
				else
				{
					currentLog.Add(log2);
				}
			}
		}
		if (selectedLog == null)
		{
			return;
		}
		int num = currentLog.IndexOf(selectedLog);
		if (num == -1)
		{
			Log item = logsDic[selectedLog.condition][selectedLog.stacktrace];
			num = currentLog.IndexOf(item);
			if (num != -1)
			{
				scrollPosition.y = (float)num * size.y;
			}
		}
		else
		{
			scrollPosition.y = (float)num * size.y;
		}
	}

	private void DrawInfo()
	{
		GUILayout.BeginArea(screenRect, backStyle);
		Vector2 drag = getDrag();
		if (drag.x != 0f && downPos != Vector2.zero)
		{
			infoScrollPosition.x -= drag.x - oldInfoDrag.x;
		}
		if (drag.y != 0f && downPos != Vector2.zero)
		{
			infoScrollPosition.y += drag.y - oldInfoDrag.y;
		}
		oldInfoDrag = drag;
		GUI.skin = toolbarScrollerSkin;
		infoScrollPosition = GUILayout.BeginScrollView(infoScrollPosition);
		GUILayout.Space(size.x);
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(buildFromContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(buildDate, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(systemInfoContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(deviceModel, nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(deviceType, nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(deviceName, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(graphicsInfoContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(SystemInfo.graphicsDeviceName, nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(graphicsMemorySize, nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(maxTextureSize, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Space(size.x);
		GUILayout.Space(size.x);
		GUILayout.Label("Screen Width " + Screen.width, nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label("Screen Height " + Screen.height, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(systemMemorySize + " mb", nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Space(size.x);
		GUILayout.Space(size.x);
		GUILayout.Label("Mem Usage Of Logs " + logsMemUsage.ToString("0.000") + " mb", nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label("GC Memory " + gcTotalMemory.ToString("0.000") + " mb", nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(softwareContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(SystemInfo.operatingSystem, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(dateContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(DateTime.Now.ToString(), nonStyle, GUILayout.Height(size.y));
		GUILayout.Label(" - Application Started At " + logDate, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(Time.realtimeSinceStartup.ToString("000"), nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(fpsText, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(userContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(UserData, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label(currentScene, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.Label("Unity Version = " + Application.unityVersion, nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		drawInfo_enableDisableToolBarButtons();
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Label("Size = " + size.x.ToString("0.0"), nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		float num = GUILayout.HorizontalSlider(size.x, 16f, 64f, sliderBackStyle, sliderThumbStyle, GUILayout.Width((float)Screen.width * 0.5f));
		if (size.x != num)
		{
			size.x = (size.y = num);
			initializeStyle();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		if (GUILayout.Button(backContent, barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			currentView = ReportView.Logs;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void drawInfo_enableDisableToolBarButtons()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		GUILayout.Label("Hide or Show tool bar buttons", nonStyle, GUILayout.Height(size.y));
		GUILayout.Space(size.x);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(size.x);
		if (GUILayout.Button(clearOnNewSceneContent, showClearOnNewSceneLoadedButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showClearOnNewSceneLoadedButton = !showClearOnNewSceneLoadedButton;
		}
		if (GUILayout.Button(showTimeContent, showTimeButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showTimeButton = !showTimeButton;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), lowerLeftFontStyle);
		if (GUILayout.Button(showSceneContent, showSceneButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showSceneButton = !showSceneButton;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label(tempRect, currentScene, lowerLeftFontStyle);
		if (GUILayout.Button(showMemoryContent, showMemButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showMemButton = !showMemButton;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label(tempRect, gcTotalMemory.ToString("0.0"), lowerLeftFontStyle);
		if (GUILayout.Button(showFpsContent, showFpsButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showFpsButton = !showFpsButton;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label(tempRect, fpsText, lowerLeftFontStyle);
		if (GUILayout.Button(searchContent, showSearchText ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showSearchText = !showSearchText;
		}
		if (GUILayout.Button(copyContent, showCopyButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showCopyButton = !showCopyButton;
		}
		if (GUILayout.Button(saveLogsContent, showSaveButton ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showSaveButton = !showSaveButton;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.TextField(tempRect, filterText, searchStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void DrawReport()
	{
		screenRect.x = 0f;
		screenRect.y = 0f;
		screenRect.width = Screen.width;
		screenRect.height = Screen.height;
		GUILayout.BeginArea(screenRect, backStyle);
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Select Photo", nonStyle, GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Coming Soon", nonStyle, GUILayout.Height(size.y));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(backContent, barStyle, GUILayout.Width(size.x), GUILayout.Height(size.y)))
		{
			currentView = ReportView.Logs;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void drawToolBar()
	{
		toolBarRect.x = 0f;
		toolBarRect.y = 0f;
		toolBarRect.width = Screen.width;
		toolBarRect.height = size.y * 2f;
		GUI.skin = toolbarScrollerSkin;
		Vector2 drag = getDrag();
		if (drag.x != 0f && downPos != Vector2.zero && downPos.y > (float)Screen.height - size.y * 2f)
		{
			toolbarScrollPosition.x -= drag.x - toolbarOldDrag;
		}
		toolbarOldDrag = drag.x;
		GUILayout.BeginArea(toolBarRect);
		toolbarScrollPosition = GUILayout.BeginScrollView(toolbarScrollPosition);
		GUILayout.BeginHorizontal(barStyle);
		if (GUILayout.Button(clearContent, barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			clear();
		}
		if (GUILayout.Button(collapseContent, collapse ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			collapse = !collapse;
			calculateCurrentLog();
		}
		if (showClearOnNewSceneLoadedButton && GUILayout.Button(clearOnNewSceneContent, clearOnNewSceneLoaded ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			clearOnNewSceneLoaded = !clearOnNewSceneLoaded;
		}
		if (showTimeButton && GUILayout.Button(showTimeContent, showTime ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showTime = !showTime;
		}
		if (showSceneButton)
		{
			tempRect = GUILayoutUtility.GetLastRect();
			GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), lowerLeftFontStyle);
			if (GUILayout.Button(showSceneContent, showScene ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
			{
				showScene = !showScene;
			}
			tempRect = GUILayoutUtility.GetLastRect();
			GUI.Label(tempRect, currentScene, lowerLeftFontStyle);
		}
		if (showMemButton)
		{
			if (GUILayout.Button(showMemoryContent, showMemory ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
			{
				showMemory = !showMemory;
			}
			tempRect = GUILayoutUtility.GetLastRect();
			GUI.Label(tempRect, gcTotalMemory.ToString("0.0"), lowerLeftFontStyle);
		}
		if (showFpsButton)
		{
			if (GUILayout.Button(showFpsContent, showFps ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
			{
				showFps = !showFps;
			}
			tempRect = GUILayoutUtility.GetLastRect();
			GUI.Label(tempRect, fpsText, lowerLeftFontStyle);
		}
		if (showSearchText)
		{
			GUILayout.Box(searchContent, barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f));
			tempRect = GUILayoutUtility.GetLastRect();
			string text = GUI.TextField(tempRect, filterText, searchStyle);
			if (text != filterText)
			{
				filterText = text;
				calculateCurrentLog();
			}
		}
		if (showCopyButton && GUILayout.Button(copyContent, barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			if (selectedLog == null)
			{
				GUIUtility.systemCopyBuffer = "No log selected";
			}
			else
			{
				GUIUtility.systemCopyBuffer = selectedLog.condition + Environment.NewLine + Environment.NewLine + selectedLog.stacktrace;
			}
		}
		if (showSaveButton && GUILayout.Button(saveLogsContent, barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			SaveLogsToDevice();
		}
		if (GUILayout.Button(infoContent, barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			currentView = ReportView.Info;
		}
		GUILayout.FlexibleSpace();
		string text2 = " ";
		text2 = ((!collapse) ? (text2 + numOfLogs) : (text2 + numOfCollapsedLogs));
		string text3 = " ";
		text3 = ((!collapse) ? (text3 + numOfLogsWarning) : (text3 + numOfCollapsedLogsWarning));
		string text4 = " ";
		text4 = ((!collapse) ? (text4 + numOfLogsError) : (text4 + numOfCollapsedLogsError));
		GUILayout.BeginHorizontal(showLog ? buttonActiveStyle : barStyle);
		if (GUILayout.Button(logContent, nonStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showLog = !showLog;
			calculateCurrentLog();
		}
		if (GUILayout.Button(text2, nonStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showLog = !showLog;
			calculateCurrentLog();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(showWarning ? buttonActiveStyle : barStyle);
		if (GUILayout.Button(warningContent, nonStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showWarning = !showWarning;
			calculateCurrentLog();
		}
		if (GUILayout.Button(text3, nonStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showWarning = !showWarning;
			calculateCurrentLog();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(showError ? buttonActiveStyle : nonStyle);
		if (GUILayout.Button(errorContent, nonStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showError = !showError;
			calculateCurrentLog();
		}
		if (GUILayout.Button(text4, nonStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			showError = !showError;
			calculateCurrentLog();
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button(closeContent, barStyle, GUILayout.Width(size.x * 2f), GUILayout.Height(size.y * 2f)))
		{
			show = false;
			UnityEngine.Object.DestroyImmediate(base.gameObject.GetComponent<ReporterGUI>());
			try
			{
				base.gameObject.SendMessage("OnHideReporter");
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void DrawLogs()
	{
		GUILayout.BeginArea(logsRect, backStyle);
		GUI.skin = logScrollerSkin;
		Vector2 drag = getDrag();
		if (drag.y != 0f && logsRect.Contains(new Vector2(downPos.x, (float)Screen.height - downPos.y)))
		{
			scrollPosition.y += drag.y - oldDrag;
		}
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		oldDrag = drag.y;
		int a = (int)((float)Screen.height * 0.75f / size.y);
		int count = currentLog.Count;
		a = Mathf.Min(a, count - startIndex);
		int num = 0;
		int num2 = (int)((float)startIndex * size.y);
		if (num2 > 0)
		{
			GUILayout.BeginHorizontal(GUILayout.Height(num2));
			GUILayout.Label("---");
			GUILayout.EndHorizontal();
		}
		int value = startIndex + a;
		value = Mathf.Clamp(value, 0, count);
		bool flag = a < count;
		int num3 = startIndex;
		while (startIndex + num < value && num3 < currentLog.Count)
		{
			Log log = currentLog[num3];
			if ((log.logType != _LogType.Log || showLog) && (log.logType != _LogType.Warning || showWarning) && (log.logType != _LogType.Error || showError) && (log.logType != _LogType.Assert || showError) && (log.logType != _LogType.Exception || showError))
			{
				if (num >= a)
				{
					break;
				}
				GUIContent gUIContent = null;
				gUIContent = ((log.logType == _LogType.Log) ? logContent : ((log.logType != _LogType.Warning) ? errorContent : warningContent));
				GUIStyle gUIStyle = (((startIndex + num) % 2 == 0) ? evenLogStyle : oddLogStyle);
				if (log == selectedLog)
				{
					gUIStyle = selectedLogStyle;
				}
				tempContent.text = log.count.ToString();
				float num4 = 0f;
				if (collapse)
				{
					num4 = barStyle.CalcSize(tempContent).x + 3f;
				}
				countRect.x = (float)Screen.width - num4;
				countRect.y = size.y * (float)num3;
				if (num2 > 0)
				{
					countRect.y += 8f;
				}
				countRect.width = num4;
				countRect.height = size.y;
				if (flag)
				{
					countRect.x -= size.x * 2f;
				}
				Sample sample = samples[log.sampleId];
				fpsRect = countRect;
				if (showFps)
				{
					tempContent.text = sample.fpsText;
					num4 = gUIStyle.CalcSize(tempContent).x + size.x;
					fpsRect.x -= num4;
					fpsRect.width = size.x;
					fpsLabelRect = fpsRect;
					fpsLabelRect.x += size.x;
					fpsLabelRect.width = num4 - size.x;
				}
				memoryRect = fpsRect;
				if (showMemory)
				{
					tempContent.text = sample.memory.ToString("0.000");
					num4 = gUIStyle.CalcSize(tempContent).x + size.x;
					memoryRect.x -= num4;
					memoryRect.width = size.x;
					memoryLabelRect = memoryRect;
					memoryLabelRect.x += size.x;
					memoryLabelRect.width = num4 - size.x;
				}
				sceneRect = memoryRect;
				if (showScene)
				{
					tempContent.text = sample.GetSceneName();
					num4 = gUIStyle.CalcSize(tempContent).x + size.x;
					sceneRect.x -= num4;
					sceneRect.width = size.x;
					sceneLabelRect = sceneRect;
					sceneLabelRect.x += size.x;
					sceneLabelRect.width = num4 - size.x;
				}
				timeRect = sceneRect;
				if (showTime)
				{
					tempContent.text = sample.time.ToString("0.000");
					num4 = gUIStyle.CalcSize(tempContent).x + size.x;
					timeRect.x -= num4;
					timeRect.width = size.x;
					timeLabelRect = timeRect;
					timeLabelRect.x += size.x;
					timeLabelRect.width = num4 - size.x;
				}
				GUILayout.BeginHorizontal(gUIStyle);
				if (log == selectedLog)
				{
					GUILayout.Box(gUIContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
					GUILayout.Label(log.condition, selectedLogFontStyle);
					if (showTime)
					{
						GUI.Box(timeRect, showTimeContent, gUIStyle);
						GUI.Label(timeLabelRect, sample.time.ToString("0.000"), gUIStyle);
					}
					if (showScene)
					{
						GUI.Box(sceneRect, showSceneContent, gUIStyle);
						GUI.Label(sceneLabelRect, sample.GetSceneName(), gUIStyle);
					}
					if (showMemory)
					{
						GUI.Box(memoryRect, showMemoryContent, gUIStyle);
						GUI.Label(memoryLabelRect, sample.memory.ToString("0.000") + " mb", gUIStyle);
					}
					if (showFps)
					{
						GUI.Box(fpsRect, showFpsContent, gUIStyle);
						GUI.Label(fpsLabelRect, sample.fpsText, gUIStyle);
					}
				}
				else
				{
					if (GUILayout.Button(gUIContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y)))
					{
						selectedLog = log;
					}
					if (GUILayout.Button(log.condition, logButtonStyle))
					{
						selectedLog = log;
					}
					if (showTime)
					{
						GUI.Box(timeRect, showTimeContent, gUIStyle);
						GUI.Label(timeLabelRect, sample.time.ToString("0.000"), gUIStyle);
					}
					if (showScene)
					{
						GUI.Box(sceneRect, showSceneContent, gUIStyle);
						GUI.Label(sceneLabelRect, sample.GetSceneName(), gUIStyle);
					}
					if (showMemory)
					{
						GUI.Box(memoryRect, showMemoryContent, gUIStyle);
						GUI.Label(memoryLabelRect, sample.memory.ToString("0.000") + " mb", gUIStyle);
					}
					if (showFps)
					{
						GUI.Box(fpsRect, showFpsContent, gUIStyle);
						GUI.Label(fpsLabelRect, sample.fpsText, gUIStyle);
					}
				}
				if (collapse)
				{
					GUI.Label(countRect, log.count.ToString(), barStyle);
				}
				GUILayout.EndHorizontal();
				num++;
			}
			num3++;
		}
		int num5 = (int)((float)(count - (startIndex + a)) * size.y);
		if (num5 > 0)
		{
			GUILayout.BeginHorizontal(GUILayout.Height(num5));
			GUILayout.Label(" ");
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		buttomRect.x = 0f;
		buttomRect.y = (float)Screen.height - size.y;
		buttomRect.width = Screen.width;
		buttomRect.height = size.y;
		if (showGraph)
		{
			drawGraph();
		}
		else
		{
			drawStack();
		}
	}

	private void drawGraph()
	{
		graphRect = stackRect;
		graphRect.height = (float)Screen.height * 0.25f;
		GUI.skin = graphScrollerSkin;
		Vector2 drag = getDrag();
		if (graphRect.Contains(new Vector2(downPos.x, (float)Screen.height - downPos.y)))
		{
			if (drag.x != 0f)
			{
				graphScrollerPos.x -= drag.x - oldDrag3;
				graphScrollerPos.x = Mathf.Max(0f, graphScrollerPos.x);
			}
			Vector2 vector = downPos;
			if (vector != Vector2.zero)
			{
				currentFrame = startFrame + (int)(vector.x / graphSize);
			}
		}
		oldDrag3 = drag.x;
		GUILayout.BeginArea(graphRect, backStyle);
		graphScrollerPos = GUILayout.BeginScrollView(graphScrollerPos);
		startFrame = (int)(graphScrollerPos.x / graphSize);
		if (graphScrollerPos.x >= (float)samples.Count * graphSize - (float)Screen.width)
		{
			graphScrollerPos.x += graphSize;
		}
		GUILayout.Label(" ", GUILayout.Width((float)samples.Count * graphSize));
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		maxFpsValue = 0f;
		minFpsValue = 100000f;
		maxMemoryValue = 0f;
		minMemoryValue = 100000f;
		for (int i = 0; (float)i < (float)Screen.width / graphSize; i++)
		{
			int num = startFrame + i;
			if (num >= samples.Count)
			{
				break;
			}
			Sample sample = samples[num];
			if (maxFpsValue < sample.fps)
			{
				maxFpsValue = sample.fps;
			}
			if (minFpsValue > sample.fps)
			{
				minFpsValue = sample.fps;
			}
			if (maxMemoryValue < sample.memory)
			{
				maxMemoryValue = sample.memory;
			}
			if (minMemoryValue > sample.memory)
			{
				minMemoryValue = sample.memory;
			}
		}
		if (currentFrame != -1 && currentFrame < samples.Count)
		{
			Sample sample2 = samples[currentFrame];
			GUILayout.BeginArea(buttomRect, backStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample2.time.ToString("0.0"), nonStyle);
			GUILayout.Space(size.x);
			GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample2.GetSceneName(), nonStyle);
			GUILayout.Space(size.x);
			GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample2.memory.ToString("0.000"), nonStyle);
			GUILayout.Space(size.x);
			GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample2.fpsText, nonStyle);
			GUILayout.Space(size.x);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		graphMaxRect = stackRect;
		graphMaxRect.height = size.y;
		GUILayout.BeginArea(graphMaxRect);
		GUILayout.BeginHorizontal();
		GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Label(maxMemoryValue.ToString("0.000"), nonStyle);
		GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Label(maxFpsValue.ToString("0.000"), nonStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		graphMinRect = stackRect;
		graphMinRect.y = stackRect.y + stackRect.height - size.y;
		graphMinRect.height = size.y;
		GUILayout.BeginArea(graphMinRect);
		GUILayout.BeginHorizontal();
		GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Label(minMemoryValue.ToString("0.000"), nonStyle);
		GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
		GUILayout.Label(minFpsValue.ToString("0.000"), nonStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void drawStack()
	{
		if (selectedLog != null)
		{
			Vector2 drag = getDrag();
			if (drag.y != 0f && stackRect.Contains(new Vector2(downPos.x, (float)Screen.height - downPos.y)))
			{
				scrollPosition2.y += drag.y - oldDrag2;
			}
			oldDrag2 = drag.y;
			GUILayout.BeginArea(stackRect, backStyle);
			scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);
			Sample sample = null;
			try
			{
				sample = samples[selectedLog.sampleId];
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label(selectedLog.condition, stackLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(size.y * 0.25f);
			GUILayout.BeginHorizontal();
			GUILayout.Label(selectedLog.stacktrace, stackLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(size.y);
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			GUILayout.BeginArea(buttomRect, backStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Box(showTimeContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample.time.ToString("0.000"), nonStyle);
			GUILayout.Space(size.x);
			GUILayout.Box(showSceneContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample.GetSceneName(), nonStyle);
			GUILayout.Space(size.x);
			GUILayout.Box(showMemoryContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample.memory.ToString("0.000"), nonStyle);
			GUILayout.Space(size.x);
			GUILayout.Box(showFpsContent, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
			GUILayout.Label(sample.fpsText, nonStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		else
		{
			GUILayout.BeginArea(stackRect, backStyle);
			GUILayout.EndArea();
			GUILayout.BeginArea(buttomRect, backStyle);
			GUILayout.EndArea();
		}
	}

	public void OnGUIDraw()
	{
		if (show)
		{
			screenRect.x = 0f;
			screenRect.y = 0f;
			screenRect.width = Screen.width;
			screenRect.height = Screen.height;
			getDownPos();
			logsRect.x = 0f;
			logsRect.y = size.y * 2f;
			logsRect.width = Screen.width;
			logsRect.height = (float)Screen.height * 0.75f - size.y * 2f;
			stackRectTopLeft.x = 0f;
			stackRect.x = 0f;
			stackRectTopLeft.y = (float)Screen.height * 0.75f;
			stackRect.y = (float)Screen.height * 0.75f;
			stackRect.width = Screen.width;
			stackRect.height = (float)Screen.height * 0.25f - size.y;
			detailRect.x = 0f;
			detailRect.y = (float)Screen.height - size.y * 3f;
			detailRect.width = Screen.width;
			detailRect.height = size.y * 3f;
			if (currentView == ReportView.Info)
			{
				DrawInfo();
			}
			else if (currentView == ReportView.Logs)
			{
				drawToolBar();
				DrawLogs();
			}
		}
	}

	private bool isGestureDone()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touches.Length != 1)
			{
				gestureDetector.Clear();
				gestureCount = 0;
			}
			else if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
			{
				gestureDetector.Clear();
			}
			else if (Input.touches[0].phase == TouchPhase.Moved)
			{
				Vector2 position = Input.touches[0].position;
				if (gestureDetector.Count == 0 || (position - gestureDetector[gestureDetector.Count - 1]).magnitude > 10f)
				{
					gestureDetector.Add(position);
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			gestureDetector.Clear();
			gestureCount = 0;
		}
		else if (Input.GetMouseButton(0))
		{
			Vector2 vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if (gestureDetector.Count == 0 || (vector - gestureDetector[gestureDetector.Count - 1]).magnitude > 10f)
			{
				gestureDetector.Add(vector);
			}
		}
		if (gestureDetector.Count < 10)
		{
			return false;
		}
		gestureSum = Vector2.zero;
		gestureLength = 0f;
		Vector2 rhs = Vector2.zero;
		for (int i = 0; i < gestureDetector.Count - 2; i++)
		{
			Vector2 vector2 = gestureDetector[i + 1] - gestureDetector[i];
			float magnitude = vector2.magnitude;
			gestureSum += vector2;
			gestureLength += magnitude;
			if (Vector2.Dot(vector2, rhs) < 0f)
			{
				gestureDetector.Clear();
				gestureCount = 0;
				return false;
			}
			rhs = vector2;
		}
		int num = (Screen.width + Screen.height) / 4;
		if (gestureLength > (float)num && gestureSum.magnitude < (float)(num / 2))
		{
			gestureDetector.Clear();
			gestureCount++;
			if (gestureCount >= numOfCircleToShow)
			{
				return true;
			}
		}
		return false;
	}

	private bool isDoubleClickDone()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touches.Length != 1)
			{
				lastClickTime = -1f;
			}
			else if (Input.touches[0].phase == TouchPhase.Began)
			{
				if (lastClickTime == -1f)
				{
					lastClickTime = Time.realtimeSinceStartup;
				}
				else
				{
					if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
					{
						lastClickTime = -1f;
						return true;
					}
					lastClickTime = Time.realtimeSinceStartup;
				}
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			if (lastClickTime == -1f)
			{
				lastClickTime = Time.realtimeSinceStartup;
			}
			else
			{
				if (Time.realtimeSinceStartup - lastClickTime < 0.2f)
				{
					lastClickTime = -1f;
					return true;
				}
				lastClickTime = Time.realtimeSinceStartup;
			}
		}
		return false;
	}

	private Vector2 getDownPos()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
			{
				downPos = Input.touches[0].position;
				return downPos;
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			downPos.x = Input.mousePosition.x;
			downPos.y = Input.mousePosition.y;
			return downPos;
		}
		return Vector2.zero;
	}

	private Vector2 getDrag()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touches.Length != 1)
			{
				return Vector2.zero;
			}
			return Input.touches[0].position - downPos;
		}
		if (Input.GetMouseButton(0))
		{
			mousePosition = Input.mousePosition;
			return mousePosition - downPos;
		}
		return Vector2.zero;
	}

	private void calculateStartIndex()
	{
		startIndex = (int)(scrollPosition.y / size.y);
		startIndex = Mathf.Clamp(startIndex, 0, currentLog.Count);
	}

	private void doShow()
	{
		show = true;
		currentView = ReportView.Logs;
		base.gameObject.AddComponent<ReporterGUI>();
		try
		{
			base.gameObject.SendMessage("OnShowReporter");
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	private void Update()
	{
		fpsText = fps.ToString("0.000");
		gcTotalMemory = (float)GC.GetTotalMemory(forceFullCollection: false) / 1024f / 1024f;
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		if (buildIndex != -1 && string.IsNullOrEmpty(scenes[buildIndex]))
		{
			scenes[SceneManager.GetActiveScene().buildIndex] = SceneManager.GetActiveScene().name;
		}
		calculateStartIndex();
		if (!show && isGestureDone())
		{
			doShow();
		}
		if (threadedLogs.Count > 0)
		{
			lock (threadedLogs)
			{
				for (int i = 0; i < threadedLogs.Count; i++)
				{
					Log log = threadedLogs[i];
					AddLog(log.condition, log.stacktrace, (LogType)log.logType);
				}
				threadedLogs.Clear();
			}
		}
		if (firstTime)
		{
			firstTime = false;
			lastUpdate = Time.realtimeSinceStartup;
			frames = 0;
			return;
		}
		frames++;
		float num = Time.realtimeSinceStartup - lastUpdate;
		if (num > 0.25f && frames > 10)
		{
			fps = (float)frames / num;
			lastUpdate = Time.realtimeSinceStartup;
			frames = 0;
		}
	}

	private void CaptureLog(string condition, string stacktrace, LogType type)
	{
		AddLog(condition, stacktrace, type);
	}

	private void AddLog(string condition, string stacktrace, LogType type)
	{
		float num = 0f;
		string text = "";
		if (cachedString.ContainsKey(condition))
		{
			text = cachedString[condition];
		}
		else
		{
			text = condition;
			cachedString.Add(text, text);
			num += (float)((!string.IsNullOrEmpty(text)) ? (text.Length * 2) : 0);
			num += (float)IntPtr.Size;
		}
		string text2 = "";
		if (cachedString.ContainsKey(stacktrace))
		{
			text2 = cachedString[stacktrace];
		}
		else
		{
			text2 = stacktrace;
			cachedString.Add(text2, text2);
			num += (float)((!string.IsNullOrEmpty(text2)) ? (text2.Length * 2) : 0);
			num += (float)IntPtr.Size;
		}
		bool flag = false;
		addSample();
		Log log = new Log
		{
			logType = (_LogType)type,
			condition = text,
			stacktrace = text2,
			sampleId = samples.Count - 1
		};
		num += log.GetMemoryUsage();
		logsMemUsage += num / 1024f / 1024f;
		if (TotalMemUsage > maxSize)
		{
			clear();
			UnityEngine.Debug.Log("Memory Usage Reach" + maxSize + " mb So It is Cleared");
			return;
		}
		bool flag2 = false;
		if (logsDic.ContainsKey(text, stacktrace))
		{
			flag2 = false;
			logsDic[text][stacktrace].count++;
		}
		else
		{
			flag2 = true;
			collapsedLogs.Add(log);
			logsDic[text][stacktrace] = log;
			switch (type)
			{
			case LogType.Log:
				numOfCollapsedLogs++;
				break;
			case LogType.Warning:
				numOfCollapsedLogsWarning++;
				break;
			default:
				numOfCollapsedLogsError++;
				break;
			}
		}
		switch (type)
		{
		case LogType.Log:
			numOfLogs++;
			break;
		case LogType.Warning:
			numOfLogsWarning++;
			break;
		default:
			numOfLogsError++;
			break;
		}
		logs.Add(log);
		if (!collapse || flag2)
		{
			bool flag3 = false;
			if (log.logType == _LogType.Log && !showLog)
			{
				flag3 = true;
			}
			if (log.logType == _LogType.Warning && !showWarning)
			{
				flag3 = true;
			}
			if (log.logType == _LogType.Error && !showError)
			{
				flag3 = true;
			}
			if (log.logType == _LogType.Assert && !showError)
			{
				flag3 = true;
			}
			if (log.logType == _LogType.Exception && !showError)
			{
				flag3 = true;
			}
			if (!flag3 && (string.IsNullOrEmpty(filterText) || log.condition.ToLower().Contains(filterText.ToLower())))
			{
				currentLog.Add(log);
				flag = true;
			}
		}
		if (flag)
		{
			calculateStartIndex();
			int count = currentLog.Count;
			int num2 = (int)((float)Screen.height * 0.75f / size.y);
			if (startIndex >= count - num2)
			{
				scrollPosition.y += size.y;
			}
		}
		try
		{
			base.gameObject.SendMessage("OnLog", log);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	private void CaptureLogThread(string condition, string stacktrace, LogType type)
	{
		Log item = new Log
		{
			condition = condition,
			stacktrace = stacktrace,
			logType = (_LogType)type
		};
		lock (threadedLogs)
		{
			threadedLogs.Add(item);
		}
	}

	private void _OnLevelWasLoaded(Scene _null1, LoadSceneMode _null2)
	{
		if (clearOnNewSceneLoaded)
		{
			clear();
		}
		currentScene = SceneManager.GetActiveScene().name;
		UnityEngine.Debug.Log("Scene " + SceneManager.GetActiveScene().name + " is loaded");
	}

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("Reporter_currentView", (int)currentView);
		PlayerPrefs.SetInt("Reporter_show", show ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_collapse", collapse ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_clearOnNewSceneLoaded", clearOnNewSceneLoaded ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showTime", showTime ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showScene", showScene ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showMemory", showMemory ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showFps", showFps ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showGraph", showGraph ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showLog", showLog ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showWarning", showWarning ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showError", showError ? 1 : 0);
		PlayerPrefs.SetString("Reporter_filterText", filterText);
		PlayerPrefs.SetFloat("Reporter_size", size.x);
		PlayerPrefs.SetInt("Reporter_showClearOnNewSceneLoadedButton", showClearOnNewSceneLoadedButton ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showTimeButton", showTimeButton ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showSceneButton", showSceneButton ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showMemButton", showMemButton ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showFpsButton", showFpsButton ? 1 : 0);
		PlayerPrefs.SetInt("Reporter_showSearchText", showSearchText ? 1 : 0);
		PlayerPrefs.Save();
	}

	private IEnumerator readInfo()
	{
		string text = "build_info";
		string text2 = text;
		if (text.IndexOf("://") == -1)
		{
			string text3 = Application.streamingAssetsPath;
			if (text3 == "")
			{
				text3 = Application.dataPath + "/StreamingAssets/";
			}
			text2 = Path.Combine(text3, text);
		}
		if (!text2.Contains("://"))
		{
			text2 = "file://" + text2;
		}
		UnityWebRequest www = UnityWebRequest.Get(text2);
		yield return www.SendWebRequest();
		if (!string.IsNullOrEmpty(www.error))
		{
			UnityEngine.Debug.LogError(www.error);
		}
		else
		{
			buildDate = www.downloadHandler.text;
		}
	}

	private void SaveLogsToDevice()
	{
		string text = Application.persistentDataPath + "/logs.txt";
		List<string> list = new List<string>();
		UnityEngine.Debug.Log("Saving logs to " + text);
		File.Delete(text);
		for (int i = 0; i < logs.Count; i++)
		{
			list.Add(string.Concat(logs[i].logType, "\n", logs[i].condition, "\n", logs[i].stacktrace));
		}
		File.WriteAllLines(text, list.ToArray());
	}
}
public class ReporterGUI : MonoBehaviour
{
	private Reporter reporter;

	private void Awake()
	{
		reporter = base.gameObject.GetComponent<Reporter>();
	}

	private void OnGUI()
	{
		reporter.OnGUIDraw();
	}
}
public class ReporterMessageReceiver : MonoBehaviour
{
	private Reporter reporter;

	private void Start()
	{
		reporter = base.gameObject.GetComponent<Reporter>();
	}

	private void OnPreStart()
	{
		if (reporter == null)
		{
			reporter = base.gameObject.GetComponent<Reporter>();
		}
		if (Screen.width < 1000)
		{
			reporter.size = new Vector2(32f, 32f);
		}
		else
		{
			reporter.size = new Vector2(48f, 48f);
		}
		reporter.UserData = "Put user date here like his account to know which user is playing on this device";
	}

	private void OnHideReporter()
	{
	}

	private void OnShowReporter()
	{
	}

	private void OnLog(Reporter.Log log)
	{
	}
}
public class Rotate2 : MonoBehaviour
{
	private Vector3 angle;

	private void Start()
	{
		angle = base.transform.eulerAngles;
	}

	private void Update()
	{
		angle.y += Time.deltaTime * 100f;
		base.transform.eulerAngles = angle;
	}
}
public class TestReporter : MonoBehaviour
{
	public int logTestCount = 100;

	public int threadLogTestCount = 100;

	public bool logEverySecond = true;

	private int currentLogTestCount;

	private Reporter reporter;

	private GUIStyle style;

	private Rect rect1;

	private Rect rect2;

	private Rect rect3;

	private Rect rect4;

	private Rect rect5;

	private Rect rect6;

	private Thread thread;

	private float elapsed;

	private void Start()
	{
		Application.runInBackground = true;
		reporter = UnityEngine.Object.FindObjectOfType(typeof(Reporter)) as Reporter;
		UnityEngine.Debug.Log("test long text sdf asdfg asdfg sdfgsdfg sdfg sfgsdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg ssssssssssssssssssssssasdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfasdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfasdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfasdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfasdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdf");
		style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		style.wordWrap = true;
		for (int i = 0; i < 10; i++)
		{
			UnityEngine.Debug.Log("Test Collapsed log");
			UnityEngine.Debug.LogWarning("Test Collapsed Warning");
			UnityEngine.Debug.LogError("Test Collapsed Error");
		}
		for (int j = 0; j < 10; j++)
		{
			UnityEngine.Debug.Log("Test Collapsed log");
			UnityEngine.Debug.LogWarning("Test Collapsed Warning");
			UnityEngine.Debug.LogError("Test Collapsed Error");
		}
		rect1 = new Rect(Screen.width / 2 - 120, Screen.height / 2 - 225, 240f, 50f);
		rect2 = new Rect(Screen.width / 2 - 120, Screen.height / 2 - 175, 240f, 100f);
		rect3 = new Rect(Screen.width / 2 - 120, Screen.height / 2 - 50, 240f, 50f);
		rect4 = new Rect(Screen.width / 2 - 120, Screen.height / 2, 240f, 50f);
		rect5 = new Rect(Screen.width / 2 - 120, Screen.height / 2 + 50, 240f, 50f);
		rect6 = new Rect(Screen.width / 2 - 120, Screen.height / 2 + 100, 240f, 50f);
		thread = new Thread(threadLogTest);
		thread.Start();
	}

	private void OnDestroy()
	{
		thread.Abort();
	}

	private void threadLogTest()
	{
		for (int i = 0; i < threadLogTestCount; i++)
		{
			UnityEngine.Debug.Log("Test Log from Thread");
			UnityEngine.Debug.LogWarning("Test Warning from Thread");
			UnityEngine.Debug.LogError("Test Error from Thread");
		}
	}

	private void Update()
	{
		int num = 0;
		while (currentLogTestCount < logTestCount && num < 10)
		{
			UnityEngine.Debug.Log("Test Log " + currentLogTestCount);
			UnityEngine.Debug.LogError("Test LogError " + currentLogTestCount);
			UnityEngine.Debug.LogWarning("Test LogWarning " + currentLogTestCount);
			num++;
			currentLogTestCount++;
		}
		elapsed += Time.deltaTime;
		if (elapsed >= 1f)
		{
			elapsed = 0f;
			UnityEngine.Debug.Log("One Second Passed");
		}
	}

	private void OnGUI()
	{
		if ((bool)reporter && !reporter.show)
		{
			GUI.Label(rect1, "Draw circle on screen to show logs", style);
			GUI.Label(rect2, "To use Reporter just create reporter from reporter menu at first scene your game start", style);
			if (GUI.Button(rect3, "Load ReporterScene"))
			{
				SceneManager.LoadScene("ReporterScene");
			}
			if (GUI.Button(rect4, "Load test1"))
			{
				SceneManager.LoadScene("test1");
			}
			if (GUI.Button(rect5, "Load test2"))
			{
				SceneManager.LoadScene("test2");
			}
			GUI.Label(rect6, "fps : " + reporter.fps.ToString("0.0"), style);
		}
	}
}
namespace GoogleARCoreInternal
{
	public static class InstantPreviewManager
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct NativeApi
		{
			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern bool InitializeInstantPreview(string adbPath, StringBuilder version, int versionStringLength);

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern void Update();

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern IntPtr GetRenderEventFunc();

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern void SendFrame(IntPtr renderTexture);

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern bool LockCameraTexture(out IntPtr pixelBytes, out int width, out int height);

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern void UnlockCameraTexture();

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern bool IsConnected();

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern bool AppShowedTouchWarning();

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern bool UnityLoggedTouchWarning();

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern void SendToast(string toastMessage);
		}

		private class Result
		{
			public bool ShouldPromptForInstall;
		}

		public const string InstantPreviewNativeApi = "arcore_instant_preview_unity_plugin";

		private const string _apkGuid = "cf7b10762fe921e40a18151a6c92a8a6";

		private const string _noDevicesFoundAdbResult = "error: no devices/emulators found";

		private const float _maxTolerableAspectRatioDifference = 0.1f;

		private const string _mismatchedAspectRatioWarningFormatString = "Instant Preview camera texture aspect ratio ({0}) is different than Game view aspect ratio ({1}).\n To avoid distorted preview while using Instant Preview, set the Game view Aspect to match the camera texture resolution ({2}x{3}).";

		private const string _instantPreviewInputWarning = "Touch ignored. Make sure your script contains `using Input = InstantPreviewInput;` when using editor Play mode.\nTo learn more, see https://developers.google.com/ar/develop/unity/instant-preview";

		private const string _warningToastFormat = "Instant Preview is not able to {0}. See Unity console.";

		private const int _warningThrottleTimeSeconds = 5;

		private const float _unknownGameViewScale = float.MinValue;

		private static readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

		private static Dictionary<string, DateTime> _sentWarnings = new Dictionary<string, DateTime>();

		private static HashSet<string> _oneTimeWarnings = new HashSet<string>();

		public static bool IsProvidingPlatform => false;

		public static bool ValidateSessionConfig(ARCoreSessionConfig config)
		{
			bool result = true;
			if (config == null)
			{
				UnityEngine.Debug.LogWarning("Attempted to check empty configuration.");
				return false;
			}
			if (config.LightEstimationMode != LightEstimationMode.Disabled)
			{
				LogLimitedSupportMessage("enable 'Light Estimation'", logOnce: true);
				result = false;
			}
			if (config.AugmentedImageDatabase != null)
			{
				LogLimitedSupportMessage("enable 'Augmented Images'", logOnce: true);
				result = false;
			}
			if (config.AugmentedFaceMode == AugmentedFaceMode.Mesh)
			{
				LogLimitedSupportMessage("enable 'Augmented Faces'", logOnce: true);
				result = false;
			}
			if (config.InstantPlacementMode != InstantPlacementMode.Disabled)
			{
				LogLimitedSupportMessage("enable 'Instant Placement'", logOnce: true);
				result = false;
			}
			return result;
		}

		public static ARCoreSessionConfig GenerateInstantPreviewSupportedConfig(ARCoreSessionConfig config)
		{
			ARCoreSessionConfig aRCoreSessionConfig = ScriptableObject.CreateInstance<ARCoreSessionConfig>();
			if (config == null)
			{
				UnityEngine.Debug.LogWarning("Attempted to generate Instant Preview Supported Config froman empty SessionConfig object.");
			}
			else
			{
				aRCoreSessionConfig.CopyFrom(config);
			}
			aRCoreSessionConfig.LightEstimationMode = LightEstimationMode.Disabled;
			aRCoreSessionConfig.AugmentedImageDatabase = null;
			aRCoreSessionConfig.AugmentedFaceMode = AugmentedFaceMode.Disabled;
			aRCoreSessionConfig.InstantPlacementMode = InstantPlacementMode.Disabled;
			return aRCoreSessionConfig;
		}

		public static void LogLimitedSupportMessage(string featureName, bool logOnce = false)
		{
			UnityEngine.Debug.LogErrorFormat("Attempted to {0} which is not yet supported by Instant Preview.\nPlease build and run on device to use this feature.", featureName);
			if (logOnce && !_oneTimeWarnings.Contains(featureName))
			{
				NativeApi.SendToast($"Instant Preview is not able to {featureName}. See Unity console.");
				_oneTimeWarnings.Add(featureName);
			}
			if (!logOnce && (!_sentWarnings.ContainsKey(featureName) || (DateTime.UtcNow - _sentWarnings[featureName]).TotalSeconds >= 5.0))
			{
				NativeApi.SendToast($"Instant Preview is not able to {featureName}. See Unity console.");
				_sentWarnings[featureName] = DateTime.UtcNow;
			}
		}

		public static IEnumerator InitializeIfNeeded()
		{
			if (!IsProvidingPlatform || (ARCoreProjectSettings.Instance != null && !ARCoreProjectSettings.Instance.IsInstantPreviewEnabled))
			{
				yield break;
			}
			string adbPath = ShellHelper.GetAdbPath();
			if (adbPath == null)
			{
				UnityEngine.Debug.LogError("Instant Preview requires your Unity Android SDK path to be set. Please set it under 'Preferences > External Tools > Android'. You may need to install the Android SDK first.");
				yield break;
			}
			if (!File.Exists(adbPath))
			{
				UnityEngine.Debug.LogErrorFormat("adb not found at \"{0}\". Please verify that 'Preferences > External Tools > Android' has the correct Android SDK path that the Android Platform Tools are installed, and that \"{0}\" exists. You may need to install the Android SDK first.", adbPath);
				yield break;
			}
			if (Environment.GetEnvironmentVariable("ADB_TRACE") != null)
			{
				UnityEngine.Debug.LogWarning("Instant Preview: ADB_TRACE was defined. Unsetting environment variable for compatibility with Instant Preview.");
				Environment.SetEnvironmentVariable("ADB_TRACE", null);
			}
			if (StartServer(adbPath, out var version))
			{
				yield return InstallApkAndRunIfConnected(adbPath, version);
				yield return UpdateLoop(adbPath);
			}
		}

		public static bool UpdateBackgroundTextureIfNeeded(ref Texture2D backgroundTexture)
		{
			if (!IsProvidingPlatform)
			{
				return false;
			}
			if (NativeApi.LockCameraTexture(out var pixelBytes, out var width, out var height))
			{
				if (backgroundTexture == null || width != backgroundTexture.width || height != backgroundTexture.height)
				{
					backgroundTexture = new Texture2D(width, height, TextureFormat.BGRA32, mipChain: false);
				}
				backgroundTexture.LoadRawTextureData(pixelBytes, width * height * 4);
				backgroundTexture.Apply();
				NativeApi.UnlockCameraTexture();
			}
			return true;
		}

		private static IEnumerator UpdateLoop(string adbPath)
		{
			IntPtr renderEventFunc = NativeApi.GetRenderEventFunc();
			bool shouldConvertToBgra = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11;
			bool loggedAspectRatioWarning = false;
			yield return _waitForEndOfFrame;
			int currentWidth = 0;
			int currentHeight = 0;
			bool needToStartActivity = true;
			bool prevFrameLandscape = false;
			RenderTexture screenTexture = null;
			RenderTexture targetTexture = null;
			RenderTexture bgrTexture = null;
			while (true)
			{
				yield return _waitForEndOfFrame;
				bool flag = Screen.width > Screen.height;
				if (prevFrameLandscape != flag)
				{
					needToStartActivity = true;
				}
				prevFrameLandscape = flag;
				if (needToStartActivity)
				{
					string text = (flag ? "InstantPreviewLandscapeActivity" : "InstantPreviewActivity");
					ShellHelper.RunCommand(adbPath, "shell am start -S -n com.google.ar.core.instantpreview/." + text, out var _, out var _);
					needToStartActivity = false;
				}
				int num = RoundUpToNearestMultipleOf16(Screen.width);
				int num2 = RoundUpToNearestMultipleOf16(Screen.height);
				if (num != currentWidth || num2 != currentHeight)
				{
					screenTexture = new RenderTexture(num, num2, 0);
					targetTexture = screenTexture;
					if (shouldConvertToBgra)
					{
						bgrTexture = new RenderTexture(screenTexture.width, screenTexture.height, 0, RenderTextureFormat.BGRA32);
						targetTexture = bgrTexture;
					}
					currentWidth = num;
					currentHeight = num2;
				}
				NativeApi.Update();
				InstantPreviewInput.Update();
				if (NativeApi.AppShowedTouchWarning())
				{
					UnityEngine.Debug.LogWarning("Touch ignored. Make sure your script contains `using Input = InstantPreviewInput;` when using editor Play mode.\nTo learn more, see https://developers.google.com/ar/develop/unity/instant-preview");
					NativeApi.UnityLoggedTouchWarning();
				}
				AddInstantPreviewTrackedPoseDriverWhenNeeded();
				Graphics.Blit(null, screenTexture);
				if (shouldConvertToBgra)
				{
					Graphics.Blit(screenTexture, bgrTexture);
				}
				Texture texture = Frame.CameraImage.Texture;
				if (!loggedAspectRatioWarning && texture != null)
				{
					int width = texture.width;
					int height = texture.height;
					float num3 = (float)width / (float)height;
					int width2 = Screen.width;
					int height2 = Screen.height;
					float num4 = (float)width2 / (float)height2;
					if (Mathf.Abs(num3 - num4) > 0.1f)
					{
						UnityEngine.Debug.LogWarningFormat("Instant Preview camera texture aspect ratio ({0}) is different than Game view aspect ratio ({1}).\n To avoid distorted preview while using Instant Preview, set the Game view Aspect to match the camera texture resolution ({2}x{3}).", num3, num4, width, height);
						loggedAspectRatioWarning = true;
					}
				}
				NativeApi.SendFrame(targetTexture.GetNativeTexturePtr());
				GL.IssuePluginEvent(renderEventFunc, 1);
			}
		}

		private static void AddInstantPreviewTrackedPoseDriverWhenNeeded()
		{
			TrackedPoseDriver[] array = UnityEngine.Object.FindObjectsOfType<TrackedPoseDriver>();
			foreach (TrackedPoseDriver obj in array)
			{
				obj.enabled = false;
				GameObject gameObject = obj.gameObject;
				if (!(gameObject.GetComponent<InstantPreviewTrackedPoseDriver>() != null))
				{
					gameObject.AddComponent<InstantPreviewTrackedPoseDriver>();
				}
			}
		}

		private static IEnumerator InstallApkAndRunIfConnected(string adbPath, string localVersion)
		{
			string apkPath = null;
			if (!File.Exists(apkPath))
			{
				UnityEngine.Debug.LogErrorFormat("Trying to install Instant Preview APK but reference to InstantPreview.apk is broken. Couldn't find an asset with .meta file guid={0}.", "cf7b10762fe921e40a18151a6c92a8a6");
				yield break;
			}
			Result result = new Result();
			Thread checkAdbThread = new Thread(delegate(object obj)
			{
				Result result2 = (Result)obj;
				ShellHelper.RunCommand(adbPath, "shell dumpsys package com.google.ar.core.instantpreview | grep versionName", out var output, out var error);
				string text = null;
				if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(error))
				{
					text = output.Substring(output.IndexOf('=') + 1);
				}
				if (string.Compare(error, "error: no devices/emulators found") != 0)
				{
					if (!string.IsNullOrEmpty(error))
					{
						UnityEngine.Debug.LogError(error);
					}
					else
					{
						if (text == null)
						{
							UnityEngine.Debug.LogFormat("Instant Preview app not installed on device.", apkPath);
						}
						else if (text != localVersion)
						{
							UnityEngine.Debug.LogFormat("Instant Preview installed version \"{0}\" does not match local version \"{1}\".", text, localVersion);
						}
						result2.ShouldPromptForInstall = text != localVersion;
					}
				}
			});
			checkAdbThread.Start(result);
			while (!checkAdbThread.Join(0))
			{
				yield return 0;
			}
			if (!result.ShouldPromptForInstall || !PromptToInstall())
			{
				yield break;
			}
			Thread installThread = new Thread((ThreadStart)delegate
			{
				UnityEngine.Debug.LogFormat("Installing Instant Preview app version {0}.", localVersion);
				ShellHelper.RunCommand(adbPath, "uninstall com.google.ar.core.instantpreview", out var output, out var error);
				ShellHelper.RunCommand(adbPath, $"install \"{apkPath}\"", out output, out error);
				if (!string.IsNullOrEmpty(output))
				{
					UnityEngine.Debug.LogFormat("Instant Preview installation:\n{0}", output);
				}
				if (!string.IsNullOrEmpty(error) && error != "Success")
				{
					UnityEngine.Debug.LogErrorFormat("Failed to install Instant Preview app:\n{0}", error);
				}
			});
			installThread.Start();
			while (!installThread.Join(0))
			{
				yield return 0;
			}
		}

		private static bool PromptToInstall()
		{
			return false;
		}

		private static bool PromptToRebuildAugmentedImagesDatabase()
		{
			return false;
		}

		private static bool StartServer(string adbPath, out string version)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			if (!NativeApi.InitializeInstantPreview(adbPath, stringBuilder, stringBuilder.Capacity))
			{
				UnityEngine.Debug.LogErrorFormat("Couldn't start Instant Preview server with adb path \"{0}\".", adbPath);
				version = null;
				return false;
			}
			version = stringBuilder.ToString();
			UnityEngine.Debug.LogFormat("Instant Preview version {0}\nTo disable Instant Preview in this project, uncheck 'Instant Preview Enabled' under 'Edit > Project Settings > ARCore'.", version);
			return true;
		}

		private static int RoundUpToNearestMultipleOf16(int value)
		{
			return (value + 15) & -16;
		}

		private static float GetMinGameViewScaleOrUnknown()
		{
			try
			{
				Type type = Type.GetType("UnityEditor.GameView,UnityEditor");
				if (type == null)
				{
					return float.MinValue;
				}
				UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
				if (array == null || array.Length == 0)
				{
					return float.MinValue;
				}
				PropertyInfo property = type.GetProperty("minScale", BindingFlags.Instance | BindingFlags.NonPublic);
				if (property == null)
				{
					return float.MinValue;
				}
				return (float)property.GetValue(array[0], null);
			}
			catch
			{
				return float.MinValue;
			}
		}
	}
	internal class NativeSession
	{
		private PointCloudManager _pointCloudManager;

		private TrackableManager _trackableManager;

		public bool IsDestroyed { get; private set; }

		public IntPtr SessionHandle { get; private set; }

		public IntPtr FrameHandle { get; private set; }

		public IntPtr PointCloudHandle => _pointCloudManager.PointCloudHandle;

		public bool IsPointCloudNew => _pointCloudManager.IsPointCloudNew;

		public AnchorApi AnchorApi { get; private set; }

		public AugmentedFaceApi AugmentedFaceApi { get; private set; }

		public AugmentedImageApi AugmentedImageApi { get; private set; }

		public AugmentedImageDatabaseApi AugmentedImageDatabaseApi { get; private set; }

		public CameraApi CameraApi { get; private set; }

		public CameraConfigApi CameraConfigApi { get; private set; }

		public CameraConfigFilterApi CameraConfigFilterApi { get; private set; }

		public CameraConfigListApi CameraConfigListApi { get; private set; }

		public CameraMetadataApi CameraMetadataApi { get; private set; }

		public FrameApi FrameApi { get; private set; }

		public HitTestApi HitTestApi { get; private set; }

		public ImageApi ImageApi { get; private set; }

		public LightEstimateApi LightEstimateApi { get; private set; }

		public PlaneApi PlaneApi { get; private set; }

		public PointApi PointApi { get; private set; }

		public PointCloudApi PointCloudApi { get; private set; }

		public PoseApi PoseApi { get; private set; }

		public RecordingConfigApi RecordingConfigApi { get; private set; }

		public TrackApi TrackApi { get; private set; }

		public TrackDataApi TrackDataApi { get; private set; }

		public TrackDataListApi TrackDataListApi { get; private set; }

		public SessionApi SessionApi { get; private set; }

		public SessionConfigApi SessionConfigApi { get; private set; }

		public TrackableApi TrackableApi { get; private set; }

		public TrackableListApi TrackableListApi { get; private set; }

		public NativeSession(IntPtr sessionHandle, IntPtr frameHandle)
		{
			IsDestroyed = false;
			SessionHandle = sessionHandle;
			FrameHandle = frameHandle;
			_pointCloudManager = new PointCloudManager(this);
			_trackableManager = new TrackableManager(this);
			AnchorApi = new AnchorApi(this);
			AugmentedFaceApi = new AugmentedFaceApi(this);
			AugmentedImageApi = new AugmentedImageApi(this);
			AugmentedImageDatabaseApi = new AugmentedImageDatabaseApi(this);
			CameraApi = new CameraApi(this);
			CameraConfigApi = new CameraConfigApi(this);
			CameraConfigFilterApi = new CameraConfigFilterApi(this);
			CameraConfigListApi = new CameraConfigListApi(this);
			CameraMetadataApi = new CameraMetadataApi(this);
			FrameApi = new FrameApi(this);
			HitTestApi = new HitTestApi(this);
			ImageApi = new ImageApi(this);
			LightEstimateApi = new LightEstimateApi(this);
			PlaneApi = new PlaneApi(this);
			PointApi = new PointApi(this);
			PointCloudApi = new PointCloudApi(this);
			PoseApi = new PoseApi(this);
			RecordingConfigApi = new RecordingConfigApi(this);
			TrackApi = new TrackApi(this);
			TrackDataApi = new TrackDataApi(this);
			TrackDataListApi = new TrackDataListApi(this);
			SessionApi = new SessionApi(this);
			SessionConfigApi = new SessionConfigApi(this);
			TrackableApi = new TrackableApi(this);
			TrackableListApi = new TrackableListApi(this);
			SessionApi.ReportEngineType();
		}

		public Trackable TrackableFactory(IntPtr nativeHandle)
		{
			return _trackableManager.TrackableFactory(nativeHandle);
		}

		public void GetTrackables<T>(List<T> trackables, TrackableQueryFilter filter) where T : Trackable
		{
			_trackableManager.GetTrackables(trackables, filter);
		}

		public void OnUpdate(IntPtr frameHandle)
		{
			FrameHandle = frameHandle;
			_pointCloudManager.OnUpdate();
		}

		public void MarkDestroyed()
		{
			IsDestroyed = true;
		}
	}
	internal enum ApiApkInstallationStatus
	{
		Uninitialized = 0,
		Requested = 1,
		Success = 100,
		Error = 200,
		ErrorDeviceNotCompatible = 201,
		ErrorUserDeclined = 203
	}
	internal enum ApiArStatus
	{
		Success = 0,
		ErrorInvalidArgument = -1,
		ErrorFatal = -2,
		ErrorSessionPaused = -3,
		ErrorSessionNotPaused = -4,
		ErrorNotTracking = -5,
		ErrorTextureNotSet = -6,
		ErrorMissingGlContext = -7,
		ErrorUnsupportedConfiguration = -8,
		ErrorCameraPermissionNotGranted = -9,
		ErrorDeadlineExceeded = -10,
		ErrorResourceExhausted = -11,
		ErrorNotYetAvailable = -12,
		ErrorCameraNotAvailable = -13,
		ErrorCloudAnchorsNotConfigured = -14,
		ErrorInternetPermissionNotGranted = -15,
		ErrorAnchorNotSupportedForHosting = -16,
		ErrorImageInsufficientQuality = -17,
		ErrorDataInvalidFormat = -18,
		ErrorDatatUnsupportedVersion = -19,
		ErrorIllegalState = -20,
		ErrorRecordingFailed = -23,
		ErrorPlaybackFailed = -24,
		ErrorSessionUnsupported = -25,
		ErrorMetadataNotFound = -26,
		UnavailableArCoreNotInstalled = -100,
		UnavailableDeviceNotCompatible = -101,
		UnavailableApkTooOld = -103,
		UnavailableSdkTooOld = -104,
		UnavailableUserDeclinedInstall = -105
	}
	internal enum ApiAugmentedFaceMode
	{
		Disabled = 0,
		Mesh3D = 2
	}
	internal enum ApiAugmentedFaceRegionType
	{
		Nose,
		ForeheadLeft,
		ForeheadRight
	}
	internal enum ApiAvailability
	{
		UnknownError = 0,
		UnknownChecking = 1,
		UnknownTimedOut = 2,
		UnsupportedDeviceNotCapable = 100,
		SupportedNotInstalled = 201,
		SupportedApkTooOld = 202,
		SupportedInstalled = 203
	}
	[Flags]
	internal enum ApiCameraConfigTargetFps
	{
		TargetFps30 = 1,
		TargetFps60 = 2
	}
	internal enum ApiCameraFocusMode
	{
		Fixed,
		Auto
	}
	internal enum ArCameraMetadataType
	{
		Byte,
		Int32,
		Float,
		Int64,
		Double,
		Rational,
		NumTypes
	}
	internal enum NdkCameraStatus
	{
		Ok = 0,
		ErrorBase = -10000,
		ErrorUnknown = -10000,
		ErrorInvalidParameter = -10001,
		ErrorMetadataNotFound = -10004
	}
	[StructLayout(LayoutKind.Explicit)]
	internal struct ArCameraMetadata
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.I4)]
		public int Tag;

		[FieldOffset(4)]
		[MarshalAs(UnmanagedType.I1)]
		public ArCameraMetadataType Type;

		[FieldOffset(8)]
		[MarshalAs(UnmanagedType.I4)]
		public int Count;

		[FieldOffset(12)]
		public IntPtr Value;
	}
	internal static class ApiConstants
	{
		public const string ARCoreNativeApi = "arcore_sdk_c";

		public const string ARCoreARKitIntegrationApi = "NOT_AVAILABLE";

		public const string ARCoreShimApi = "arcore_unity_api";

		public const string ARPrestoApi = "arpresto_api";

		public const string ARRenderingUtilsApi = "arcore_rendering_utils_api";

		public const string MediaNdk = "mediandk";

		public const string NdkCameraApi = "camera2ndk";

		public const string GLESApi = "GLESv3";
	}
	internal enum ApiCoordinates2dType
	{
		TexturePixels,
		TextureNormalized,
		ImagePixels,
		ImageNormalized,
		FeatureTrackingImage,
		FeatureTrackingImageNormalized,
		OpenGLDeviceNormalized,
		View,
		ViewNormalized
	}
	internal enum ApiDepthMode
	{
		Disabled = 0,
		Automatic = 1,
		RawDepthOnly = 3
	}
	public struct ApiDisplayUvCoords
	{
		public const int NumFloats = 8;

		public Vector2 TopLeft;

		public Vector2 TopRight;

		public Vector2 BottomLeft;

		public Vector2 BottomRight;

		public ApiDisplayUvCoords(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
		{
			TopLeft = topLeft;
			TopRight = topRight;
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
		}
	}
	public enum ApiFeaturePointOrientationMode
	{
		Identity,
		SurfaceNormal
	}
	internal enum ApiLightEstimateState
	{
		NotValid,
		Valid
	}
	public enum ApiLightEstimationMode
	{
		Disabled,
		AmbientIntensity,
		EnvironmentalHDR
	}
	internal enum ApiPlaneFindingMode
	{
		Disabled,
		Horizontal,
		Vertical,
		HorizontalAndVertical
	}
	internal enum ApiPlaneType
	{
		HorizontalUpwardFacing,
		HorizontalDownwardFacing,
		Vertical
	}
	internal enum ApiPlaybackStatus
	{
		None,
		OK,
		IOError,
		FinishedSuccess
	}
	public struct ApiPoseData
	{
		[MarshalAs(UnmanagedType.R4)]
		public float Qx;

		[MarshalAs(UnmanagedType.R4)]
		public float Qy;

		[MarshalAs(UnmanagedType.R4)]
		public float Qz;

		[MarshalAs(UnmanagedType.R4)]
		public float Qw;

		[MarshalAs(UnmanagedType.R4)]
		public float X;

		[MarshalAs(UnmanagedType.R4)]
		public float Y;

		[MarshalAs(UnmanagedType.R4)]
		public float Z;

		public ApiPoseData(Pose unityPose)
		{
			ConversionHelper.UnityPoseToApiPose(unityPose, out this);
		}

		public Pose ToUnityPose()
		{
			ConversionHelper.ApiPoseToUnityPose(this, out var unityPose);
			return unityPose;
		}

		public override string ToString()
		{
			return $"qx: {Qx}, qy: {Qy}, qz: {Qz}, qw: {Qw}, x: {X}, y: {Y}, z: {Z}";
		}
	}
	internal enum ApiPrestoCallbackResult
	{
		Success,
		InvalidCameraConfig
	}
	internal enum ApiPrestoDeviceCameraDirection
	{
		BackFacing,
		FrontFacing
	}
	internal enum ApiPrestoStatus
	{
		Uninitialized = 0,
		RequestingApkInstall = 1,
		RequestingPermission = 2,
		Resumed = 100,
		ResumedNotTracking = 101,
		Paused = 102,
		ErrorFatal = 200,
		ErrorApkNotAvailable = 201,
		ErrorPermissionNotGranted = 202,
		ErrorSessionConfigurationNotSupported = 203,
		ErrorCameraNotAvailable = 204,
		ErrorIllegalState = 205,
		ErrorInvalidCameraConfig = 206
	}
	internal enum ApiRecordingStatus
	{
		None,
		OK,
		IOError
	}
	internal enum ApiRenderEvent
	{
		Noop,
		UpdateCubemapTexture,
		WaitOnPostUpdateFence
	}
	internal enum ApiTextureDataType
	{
		Byte,
		Half,
		Float
	}
	internal enum ApiTrackableType
	{
		Invalid = 0,
		BaseTrackable = 1095893248,
		Plane = 1095893249,
		Point = 1095893250,
		AugmentedImage = 1095893252,
		AugmentedFace = 1095893253,
		DepthPoint = 1095893265,
		InstantPlacementPoint = 1095893266
	}
	internal enum ApiTrackingState
	{
		Tracking,
		Paused,
		Stopped
	}
	internal static class ApiTypeExtensions
	{
		public static ApkAvailabilityStatus ToApkAvailabilityStatus(this ApiAvailability apiStatus)
		{
			switch (apiStatus)
			{
			case ApiAvailability.UnknownError:
				return ApkAvailabilityStatus.UnknownError;
			case ApiAvailability.UnknownChecking:
				return ApkAvailabilityStatus.UnknownChecking;
			case ApiAvailability.UnknownTimedOut:
				return ApkAvailabilityStatus.UnknownTimedOut;
			case ApiAvailability.UnsupportedDeviceNotCapable:
				return ApkAvailabilityStatus.UnsupportedDeviceNotCapable;
			case ApiAvailability.SupportedNotInstalled:
				return ApkAvailabilityStatus.SupportedNotInstalled;
			case ApiAvailability.SupportedApkTooOld:
				return ApkAvailabilityStatus.SupportedApkTooOld;
			case ApiAvailability.SupportedInstalled:
				return ApkAvailabilityStatus.SupportedInstalled;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected ApiAvailability status {0}", apiStatus);
				return ApkAvailabilityStatus.UnknownError;
			}
		}

		public static ApkInstallationStatus ToApkInstallationStatus(this ApiApkInstallationStatus apiStatus)
		{
			switch (apiStatus)
			{
			case ApiApkInstallationStatus.Uninitialized:
				return ApkInstallationStatus.Uninitialized;
			case ApiApkInstallationStatus.Requested:
				return ApkInstallationStatus.Requested;
			case ApiApkInstallationStatus.Success:
				return ApkInstallationStatus.Success;
			case ApiApkInstallationStatus.Error:
				return ApkInstallationStatus.Error;
			case ApiApkInstallationStatus.ErrorDeviceNotCompatible:
				return ApkInstallationStatus.ErrorDeviceNotCompatible;
			case ApiApkInstallationStatus.ErrorUserDeclined:
				return ApkInstallationStatus.ErrorUserDeclined;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected ApiApkInstallStatus status {0}", apiStatus);
				return ApkInstallationStatus.Error;
			}
		}

		public static SessionStatus ToSessionStatus(this ApiPrestoStatus prestoStatus)
		{
			switch (prestoStatus)
			{
			case ApiPrestoStatus.Uninitialized:
				return SessionStatus.None;
			case ApiPrestoStatus.RequestingApkInstall:
			case ApiPrestoStatus.RequestingPermission:
				return SessionStatus.Initializing;
			case ApiPrestoStatus.Resumed:
				return SessionStatus.Tracking;
			case ApiPrestoStatus.ResumedNotTracking:
				return SessionStatus.LostTracking;
			case ApiPrestoStatus.Paused:
				return SessionStatus.NotTracking;
			case ApiPrestoStatus.ErrorFatal:
				return SessionStatus.FatalError;
			case ApiPrestoStatus.ErrorApkNotAvailable:
				return SessionStatus.ErrorApkNotAvailable;
			case ApiPrestoStatus.ErrorPermissionNotGranted:
				return SessionStatus.ErrorPermissionNotGranted;
			case ApiPrestoStatus.ErrorSessionConfigurationNotSupported:
				return SessionStatus.ErrorSessionConfigurationNotSupported;
			case ApiPrestoStatus.ErrorCameraNotAvailable:
				return SessionStatus.ErrorCameraNotAvailable;
			case ApiPrestoStatus.ErrorIllegalState:
				return SessionStatus.ErrorIllegalState;
			case ApiPrestoStatus.ErrorInvalidCameraConfig:
				return SessionStatus.ErrorInvalidCameraConfig;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected presto status {0}", prestoStatus);
				return SessionStatus.FatalError;
			}
		}

		public static TrackingState ToTrackingState(this ApiTrackingState apiTrackingState)
		{
			return apiTrackingState switch
			{
				ApiTrackingState.Tracking => TrackingState.Tracking, 
				ApiTrackingState.Paused => TrackingState.Paused, 
				ApiTrackingState.Stopped => TrackingState.Stopped, 
				_ => TrackingState.Stopped, 
			};
		}

		public static XPTrackingState ToXPTrackingState(this TrackingState apiTrackingState)
		{
			return apiTrackingState switch
			{
				TrackingState.Tracking => XPTrackingState.Tracking, 
				TrackingState.Paused => XPTrackingState.Paused, 
				TrackingState.Stopped => XPTrackingState.Stopped, 
				_ => XPTrackingState.Stopped, 
			};
		}

		public static LostTrackingReason ToLostTrackingReason(this ApiTrackingFailureReason apiTrackingFailureReason)
		{
			return apiTrackingFailureReason switch
			{
				ApiTrackingFailureReason.None => LostTrackingReason.None, 
				ApiTrackingFailureReason.BadState => LostTrackingReason.BadState, 
				ApiTrackingFailureReason.InsufficientLight => LostTrackingReason.InsufficientLight, 
				ApiTrackingFailureReason.ExcessiveMotion => LostTrackingReason.ExcessiveMotion, 
				ApiTrackingFailureReason.InsufficientFeatures => LostTrackingReason.InsufficientFeatures, 
				ApiTrackingFailureReason.CameraUnavailable => LostTrackingReason.CameraUnavailable, 
				_ => LostTrackingReason.None, 
			};
		}

		public static LightEstimateState ToLightEstimateState(this ApiLightEstimateState apiState)
		{
			return apiState switch
			{
				ApiLightEstimateState.NotValid => LightEstimateState.NotValid, 
				ApiLightEstimateState.Valid => LightEstimateState.Valid, 
				_ => LightEstimateState.NotValid, 
			};
		}

		public static ApiLightEstimationMode ToApiLightEstimationMode(this LightEstimationMode mode)
		{
			switch (mode)
			{
			case LightEstimationMode.Disabled:
				return ApiLightEstimationMode.Disabled;
			case LightEstimationMode.AmbientIntensity:
				return ApiLightEstimationMode.AmbientIntensity;
			case LightEstimationMode.EnvironmentalHDRWithoutReflections:
			case LightEstimationMode.EnvironmentalHDRWithReflections:
				return ApiLightEstimationMode.EnvironmentalHDR;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected LightEstimationMode {0}", mode);
				return ApiLightEstimationMode.Disabled;
			}
		}

		public static ApiPlaneFindingMode ToApiPlaneFindingMode(this DetectedPlaneFindingMode mode)
		{
			switch (mode)
			{
			case DetectedPlaneFindingMode.Disabled:
				return ApiPlaneFindingMode.Disabled;
			case DetectedPlaneFindingMode.Horizontal:
				return ApiPlaneFindingMode.Horizontal;
			case DetectedPlaneFindingMode.Vertical:
				return ApiPlaneFindingMode.Vertical;
			case DetectedPlaneFindingMode.HorizontalAndVertical:
				return ApiPlaneFindingMode.HorizontalAndVertical;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected DetectedPlaneFindingMode {0}", mode);
				return ApiPlaneFindingMode.Disabled;
			}
		}

		public static ApiAugmentedFaceMode ToApiAugmentedFaceMode(this AugmentedFaceMode mode)
		{
			switch (mode)
			{
			case AugmentedFaceMode.Disabled:
				return ApiAugmentedFaceMode.Disabled;
			case AugmentedFaceMode.Mesh:
				return ApiAugmentedFaceMode.Mesh3D;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected AugmentedFaceMode {0}", mode);
				return ApiAugmentedFaceMode.Disabled;
			}
		}

		public static ApiCameraFocusMode ToApiCameraFocusMode(this CameraFocusMode mode)
		{
			switch (mode)
			{
			case CameraFocusMode.FixedFocus:
				return ApiCameraFocusMode.Fixed;
			case CameraFocusMode.AutoFocus:
				return ApiCameraFocusMode.Auto;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected CameraFocusMode {0}", mode);
				return ApiCameraFocusMode.Fixed;
			}
		}

		public static DepthStatus ToDepthStatus(this ApiArStatus apiStatus)
		{
			return apiStatus switch
			{
				ApiArStatus.Success => DepthStatus.Success, 
				ApiArStatus.ErrorNotYetAvailable => DepthStatus.NotYetAvailable, 
				ApiArStatus.ErrorNotTracking => DepthStatus.NotTracking, 
				ApiArStatus.ErrorIllegalState => DepthStatus.IllegalState, 
				_ => DepthStatus.InternalError, 
			};
		}

		public static ApiDepthMode ToApiDepthMode(this DepthMode depthMode)
		{
			if (depthMode != DepthMode.Disabled && depthMode == DepthMode.Automatic)
			{
				return ApiDepthMode.Automatic;
			}
			return ApiDepthMode.Disabled;
		}

		public static ApiCloudAnchorMode ToApiCloudAnchorMode(this CloudAnchorMode mode)
		{
			switch (mode)
			{
			case CloudAnchorMode.Disabled:
				return ApiCloudAnchorMode.Disabled;
			case CloudAnchorMode.Enabled:
				return ApiCloudAnchorMode.Enabled;
			default:
				UnityEngine.Debug.LogErrorFormat("Unexpected CloudAnchorMode {0}", mode);
				return ApiCloudAnchorMode.Disabled;
			}
		}

		public static DetectedPlaneType ToDetectedPlaneType(this ApiPlaneType apiPlaneType)
		{
			return apiPlaneType switch
			{
				ApiPlaneType.HorizontalUpwardFacing => DetectedPlaneType.HorizontalUpwardFacing, 
				ApiPlaneType.HorizontalDownwardFacing => DetectedPlaneType.HorizontalDownwardFacing, 
				ApiPlaneType.Vertical => DetectedPlaneType.Vertical, 
				_ => DetectedPlaneType.HorizontalUpwardFacing, 
			};
		}

		public static DisplayUvCoords ToDisplayUvCoords(this ApiDisplayUvCoords apiCoords)
		{
			return new DisplayUvCoords(apiCoords.TopLeft, apiCoords.TopRight, apiCoords.BottomLeft, apiCoords.BottomRight);
		}

		public static FeaturePointOrientationMode ToFeaturePointOrientationMode(this ApiFeaturePointOrientationMode apiMode)
		{
			switch (apiMode)
			{
			case ApiFeaturePointOrientationMode.Identity:
				return FeaturePointOrientationMode.Identity;
			case ApiFeaturePointOrientationMode.SurfaceNormal:
				return FeaturePointOrientationMode.SurfaceNormal;
			default:
				ARDebug.LogError("Invalid value for ApiFeaturePointOrientationMode.");
				return FeaturePointOrientationMode.Identity;
			}
		}

		public static CloudServiceResponse ToCloudServiceResponse(this ApiArStatus status)
		{
			switch (status)
			{
			case ApiArStatus.Success:
				return CloudServiceResponse.Success;
			case ApiArStatus.ErrorCloudAnchorsNotConfigured:
				return CloudServiceResponse.ErrorNotSupportedByConfiguration;
			case ApiArStatus.ErrorNotTracking:
			case ApiArStatus.ErrorSessionPaused:
				return CloudServiceResponse.ErrorNotTracking;
			case ApiArStatus.ErrorResourceExhausted:
				return CloudServiceResponse.ErrorTooManyCloudAnchors;
			default:
				return CloudServiceResponse.ErrorInternal;
			}
		}

		public static CloudServiceResponse ToCloudServiceResponse(this ApiCloudAnchorState anchorState)
		{
			return anchorState switch
			{
				ApiCloudAnchorState.Success => CloudServiceResponse.Success, 
				ApiCloudAnchorState.ErrorNotAuthorized => CloudServiceResponse.ErrorNotAuthorized, 
				ApiCloudAnchorState.ErrorResourceExhausted => CloudServiceResponse.ErrorApiQuotaExceeded, 
				ApiCloudAnchorState.ErrorHostingDatasetProcessingFailed => CloudServiceResponse.ErrorDatasetInadequate, 
				ApiCloudAnchorState.ErrorResolveingCloudIdNotFound => CloudServiceResponse.ErrorCloudIdNotFound, 
				ApiCloudAnchorState.ErrorResolvingSDKTooOld => CloudServiceResponse.ErrorSDKTooOld, 
				ApiCloudAnchorState.ErrorResolvingSDKTooNew => CloudServiceResponse.ErrorSDKTooNew, 
				ApiCloudAnchorState.ErrorHostingServiceUnavailable => CloudServiceResponse.ErrorHostingServiceUnavailable, 
				_ => CloudServiceResponse.ErrorInternal, 
			};
		}

		public static RecordingStatus ToRecordingStatus(this ApiRecordingStatus recordingStatus)
		{
			switch (recordingStatus)
			{
			case ApiRecordingStatus.OK:
				return RecordingStatus.OK;
			case ApiRecordingStatus.IOError:
				return RecordingStatus.IOError;
			case ApiRecordingStatus.None:
				return RecordingStatus.None;
			default:
				UnityEngine.Debug.LogErrorFormat("Unrecognized ApiRecordingStatus value {0}", recordingStatus);
				return RecordingStatus.None;
			}
		}

		public static RecordingResult ToRecordingResult(this ApiArStatus recordingResult)
		{
			switch (recordingResult)
			{
			case ApiArStatus.Success:
				return RecordingResult.OK;
			case ApiArStatus.ErrorIllegalState:
				return RecordingResult.ErrorIllegalState;
			case ApiArStatus.ErrorInvalidArgument:
				return RecordingResult.ErrorInvalidArgument;
			case ApiArStatus.ErrorRecordingFailed:
				return RecordingResult.ErrorRecordingFailed;
			default:
				UnityEngine.Debug.LogErrorFormat("Attempt to record failed with unexpected status: {0}", recordingResult);
				return RecordingResult.ErrorRecordingFailed;
			}
		}

		public static PlaybackStatus ToPlaybackStatus(this ApiPlaybackStatus playbackStatus)
		{
			switch (playbackStatus)
			{
			case ApiPlaybackStatus.None:
				return PlaybackStatus.None;
			case ApiPlaybackStatus.OK:
				return PlaybackStatus.OK;
			case ApiPlaybackStatus.IOError:
				return PlaybackStatus.IOError;
			case ApiPlaybackStatus.FinishedSuccess:
				return PlaybackStatus.FinishedSuccess;
			default:
				UnityEngine.Debug.LogErrorFormat("Unrecognized ApiPlaybackStatus value {0}", playbackStatus);
				return PlaybackStatus.None;
			}
		}
	}
	internal enum ApiUpdateMode
	{
		Blocking,
		LatestCameraImage
	}
	internal class AnchorApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchor_getPose(IntPtr sessionHandle, IntPtr anchorHandle, IntPtr poseHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchor_getTrackingState(IntPtr sessionHandle, IntPtr anchorHandle, ref ApiTrackingState trackingState);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchor_getCloudAnchorState(IntPtr sessionHandle, IntPtr anchorHandle, ref ApiCloudAnchorState state);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchor_acquireCloudAnchorId(IntPtr sessionHandle, IntPtr anchorHandle, ref IntPtr hostingIdHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchor_release(IntPtr anchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchor_detach(IntPtr sessionHandle, IntPtr anchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArString_release(IntPtr stringHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchorList_create(IntPtr sessionHandle, ref IntPtr outputAnchorListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchorList_destroy(IntPtr anchorListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchorList_getSize(IntPtr sessionHandle, IntPtr anchorListHandle, ref int outputSize);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAnchorList_acquireItem(IntPtr sessionHandle, IntPtr anchorListHandle, int index, ref IntPtr outputAnchorHandle);
		}

		private NativeSession _nativeSession;

		public AnchorApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public static void Release(IntPtr anchorHandle)
		{
			ExternApi.ArAnchor_release(anchorHandle);
		}

		public Pose GetPose(IntPtr anchorHandle)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArAnchor_getPose(_nativeSession.SessionHandle, anchorHandle, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public TrackingState GetTrackingState(IntPtr anchorHandle)
		{
			ApiTrackingState trackingState = ApiTrackingState.Stopped;
			ExternApi.ArAnchor_getTrackingState(_nativeSession.SessionHandle, anchorHandle, ref trackingState);
			return trackingState.ToTrackingState();
		}

		public ApiCloudAnchorState GetCloudAnchorState(IntPtr anchorHandle)
		{
			ApiCloudAnchorState state = ApiCloudAnchorState.None;
			ExternApi.ArAnchor_getCloudAnchorState(_nativeSession.SessionHandle, anchorHandle, ref state);
			return state;
		}

		public string GetCloudAnchorId(IntPtr anchorHandle)
		{
			IntPtr hostingIdHandle = IntPtr.Zero;
			ExternApi.ArAnchor_acquireCloudAnchorId(_nativeSession.SessionHandle, anchorHandle, ref hostingIdHandle);
			string result = Marshal.PtrToStringAnsi(hostingIdHandle);
			ExternApi.ArString_release(hostingIdHandle);
			return result;
		}

		public void Detach(IntPtr anchorHandle)
		{
			if (LifecycleManager.Instance.NativeSession == _nativeSession)
			{
				ExternApi.ArAnchor_detach(_nativeSession.SessionHandle, anchorHandle);
			}
		}

		public IntPtr CreateList()
		{
			IntPtr outputAnchorListHandle = IntPtr.Zero;
			ExternApi.ArAnchorList_create(_nativeSession.SessionHandle, ref outputAnchorListHandle);
			return outputAnchorListHandle;
		}

		public int GetListSize(IntPtr anchorListHandle)
		{
			int outputSize = 0;
			ExternApi.ArAnchorList_getSize(_nativeSession.SessionHandle, anchorListHandle, ref outputSize);
			return outputSize;
		}

		public IntPtr AcquireListItem(IntPtr anchorListHandle, int index)
		{
			IntPtr outputAnchorHandle = IntPtr.Zero;
			ExternApi.ArAnchorList_acquireItem(_nativeSession.SessionHandle, anchorListHandle, index, ref outputAnchorHandle);
			return outputAnchorHandle;
		}

		public void DestroyList(IntPtr anchorListHandle)
		{
			ExternApi.ArAnchorList_destroy(anchorListHandle);
		}
	}
	internal class AugmentedFaceApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedFace_getCenterPose(IntPtr sessionHandle, IntPtr faceHandle, IntPtr poseHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedFace_getMeshVertices(IntPtr sessionHandle, IntPtr faceHandle, ref IntPtr vertices, ref int numberOfVertices);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedFace_getMeshNormals(IntPtr sessionHandle, IntPtr faceHandle, ref IntPtr normals, ref int numberOfVertices);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedFace_getMeshTextureCoordinates(IntPtr sessionHandle, IntPtr faceHandle, ref IntPtr uvs, ref int numberOfVertices);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedFace_getMeshTriangleIndices(IntPtr sessionHandle, IntPtr faceHandle, ref IntPtr indices, ref int numberOfTriangles);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedFace_getRegionPose(IntPtr sessionHandle, IntPtr faceHandle, ApiAugmentedFaceRegionType regionType, IntPtr poseHandle);
		}

		private NativeSession _nativeSession;

		private float[] _tempVertices;

		private float[] _tempNormals;

		private float[] _tempUVs;

		private short[] _tempIndices;

		public AugmentedFaceApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public Pose GetCenterPose(IntPtr faceHandle)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArAugmentedFace_getCenterPose(_nativeSession.SessionHandle, faceHandle, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public Pose GetRegionPose(IntPtr faceHandle, ApiAugmentedFaceRegionType regionType)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArAugmentedFace_getRegionPose(_nativeSession.SessionHandle, faceHandle, regionType, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public void GetVertices(IntPtr faceHandle, List<Vector3> vertices)
		{
			IntPtr vertices2 = IntPtr.Zero;
			int numberOfVertices = 0;
			ExternApi.ArAugmentedFace_getMeshVertices(_nativeSession.SessionHandle, faceHandle, ref vertices2, ref numberOfVertices);
			int num = numberOfVertices * 3;
			if (_tempVertices == null || _tempVertices.Length != num)
			{
				_tempVertices = new float[num];
			}
			Marshal.Copy(vertices2, _tempVertices, 0, num);
			vertices.Clear();
			vertices.Capacity = numberOfVertices;
			for (int i = 0; i < num; i += 3)
			{
				vertices.Add(new Vector3(_tempVertices[i], _tempVertices[i + 1], 0f - _tempVertices[i + 2]));
			}
		}

		public void GetNormals(IntPtr faceHandle, List<Vector3> normals)
		{
			IntPtr normals2 = IntPtr.Zero;
			int numberOfVertices = 0;
			ExternApi.ArAugmentedFace_getMeshNormals(_nativeSession.SessionHandle, faceHandle, ref normals2, ref numberOfVertices);
			int num = numberOfVertices * 3;
			if (_tempNormals == null || _tempNormals.Length != num)
			{
				_tempNormals = new float[num];
			}
			Marshal.Copy(normals2, _tempNormals, 0, num);
			normals.Clear();
			normals.Capacity = numberOfVertices;
			for (int i = 0; i < num; i += 3)
			{
				normals.Add(new Vector3(_tempNormals[i], _tempNormals[i + 1], 0f - _tempNormals[i + 2]));
			}
		}

		public void GetTextureCoordinates(IntPtr faceHandle, List<Vector2> textureCoordinates)
		{
			IntPtr uvs = IntPtr.Zero;
			int numberOfVertices = 0;
			ExternApi.ArAugmentedFace_getMeshTextureCoordinates(_nativeSession.SessionHandle, faceHandle, ref uvs, ref numberOfVertices);
			int num = numberOfVertices * 2;
			if (_tempUVs == null || _tempUVs.Length != num)
			{
				_tempUVs = new float[num];
			}
			Marshal.Copy(uvs, _tempUVs, 0, num);
			textureCoordinates.Clear();
			textureCoordinates.Capacity = numberOfVertices;
			for (int i = 0; i < num; i += 2)
			{
				textureCoordinates.Add(new Vector2(_tempUVs[i], _tempUVs[i + 1]));
			}
		}

		public void GetTriangleIndices(IntPtr faceHandle, List<int> indices)
		{
			IntPtr indices2 = IntPtr.Zero;
			int numberOfTriangles = 0;
			ExternApi.ArAugmentedFace_getMeshTriangleIndices(_nativeSession.SessionHandle, faceHandle, ref indices2, ref numberOfTriangles);
			int num = numberOfTriangles * 3;
			if (_tempIndices == null || _tempIndices.Length != num)
			{
				_tempIndices = new short[num];
			}
			Marshal.Copy(indices2, _tempIndices, 0, num);
			indices.Clear();
			indices.Capacity = num;
			for (int i = 0; i < num; i += 3)
			{
				indices.Add(_tempIndices[i]);
				indices.Add(_tempIndices[i + 2]);
				indices.Add(_tempIndices[i + 1]);
			}
		}
	}
	internal class AugmentedImageApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImage_getIndex(IntPtr sessionHandle, IntPtr augmentedImageHandle, ref int outIndex);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImage_getCenterPose(IntPtr sessionHandle, IntPtr augmentedImageHandle, IntPtr outPoseHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImage_getExtentX(IntPtr sessionHandle, IntPtr augmentedImageHandle, ref float outExtentX);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImage_getExtentZ(IntPtr sessionHandle, IntPtr augmentedImageHandle, ref float outExtentZ);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImage_acquireName(IntPtr sessionHandle, IntPtr augmentedImageHandle, ref IntPtr outName);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImage_getTrackingMethod(IntPtr sessionHandle, IntPtr augmentedImageHandle, ref AugmentedImageTrackingMethod trackingMethod);

			[DllImport("arcore_sdk_c")]
			public static extern void ArString_release(IntPtr str);
		}

		private NativeSession _nativeSession;

		public AugmentedImageApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public int GetDatabaseIndex(IntPtr augmentedImageHandle)
		{
			int outIndex = -1;
			ExternApi.ArAugmentedImage_getIndex(_nativeSession.SessionHandle, augmentedImageHandle, ref outIndex);
			return outIndex;
		}

		public Pose GetCenterPose(IntPtr augmentedImageHandle)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArAugmentedImage_getCenterPose(_nativeSession.SessionHandle, augmentedImageHandle, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public float GetExtentX(IntPtr augmentedImageHandle)
		{
			float outExtentX = 0f;
			ExternApi.ArAugmentedImage_getExtentX(_nativeSession.SessionHandle, augmentedImageHandle, ref outExtentX);
			return outExtentX;
		}

		public float GetExtentZ(IntPtr augmentedImageHandle)
		{
			float outExtentZ = 0f;
			ExternApi.ArAugmentedImage_getExtentZ(_nativeSession.SessionHandle, augmentedImageHandle, ref outExtentZ);
			return outExtentZ;
		}

		public string GetName(IntPtr augmentedImageHandle)
		{
			IntPtr outName = IntPtr.Zero;
			ExternApi.ArAugmentedImage_acquireName(_nativeSession.SessionHandle, augmentedImageHandle, ref outName);
			string result = Marshal.PtrToStringAnsi(outName);
			ExternApi.ArString_release(outName);
			return result;
		}

		public AugmentedImageTrackingMethod GetTrackingMethod(IntPtr augmentedImageHandle)
		{
			AugmentedImageTrackingMethod trackingMethod = AugmentedImageTrackingMethod.NotTracking;
			ExternApi.ArAugmentedImage_getTrackingMethod(_nativeSession.SessionHandle, augmentedImageHandle, ref trackingMethod);
			return trackingMethod;
		}
	}
	internal class AugmentedImageDatabaseApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImageDatabase_create(IntPtr session, ref IntPtr out_augmented_image_database);

			[DllImport("arcore_sdk_c")]
			public static extern void ArAugmentedImageDatabase_destroy(IntPtr augmented_image_database);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArAugmentedImageDatabase_deserialize(IntPtr session, IntPtr database_raw_bytes, long database_raw_bytes_size, ref IntPtr out_augmented_image_database);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArAugmentedImageDatabase_addImageWithPhysicalSize(IntPtr session, IntPtr augmented_image_database, string image_name, IntPtr image_grayscale_pixels, int image_width_in_pixels, int image_height_in_pixels, int image_stride_in_pixels, float image_width_in_meters, ref int out_index);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArAugmentedImageDatabase_addImage(IntPtr session, IntPtr augmented_image_database, string image_name, IntPtr image_grayscale_pixels, int image_width_in_pixels, int image_height_in_pixels, int image_stride_in_pixels, ref int out_index);
		}

		private NativeSession _nativeSession;

		public AugmentedImageDatabaseApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public static void Release(IntPtr augmentedImageDatabaseHandle)
		{
			ExternApi.ArAugmentedImageDatabase_destroy(augmentedImageDatabaseHandle);
		}

		public IntPtr Create(byte[] rawData)
		{
			IntPtr out_augmented_image_database = IntPtr.Zero;
			if (rawData != null)
			{
				GCHandle gCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
				ApiArStatus apiArStatus = ExternApi.ArAugmentedImageDatabase_deserialize(_nativeSession.SessionHandle, gCHandle.AddrOfPinnedObject(), rawData.Length, ref out_augmented_image_database);
				if (apiArStatus != ApiArStatus.Success)
				{
					UnityEngine.Debug.LogWarningFormat("Failed to deserialize AugmentedImageDatabase raw data with status: {0}", apiArStatus);
					out_augmented_image_database = IntPtr.Zero;
				}
				if (gCHandle.IsAllocated)
				{
					gCHandle.Free();
				}
			}
			else
			{
				ExternApi.ArAugmentedImageDatabase_create(_nativeSession.SessionHandle, ref out_augmented_image_database);
			}
			return out_augmented_image_database;
		}

		public int AddAugmentedImageAtRuntime(IntPtr augmentedImageDatabaseHandle, string name, AugmentedImageSrc imageSrc, float width)
		{
			int out_index = -1;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("add images to Augmented Image database");
				return out_index;
			}
			GCHandle gCHandle = ConvertTextureToGrayscaleBytes(imageSrc);
			if (gCHandle.AddrOfPinnedObject() == IntPtr.Zero)
			{
				return -1;
			}
			ApiArStatus apiArStatus = ((!(width > 0f)) ? ExternApi.ArAugmentedImageDatabase_addImage(_nativeSession.SessionHandle, augmentedImageDatabaseHandle, name, gCHandle.AddrOfPinnedObject(), imageSrc._width, imageSrc._height, imageSrc._width, ref out_index) : ExternApi.ArAugmentedImageDatabase_addImageWithPhysicalSize(_nativeSession.SessionHandle, augmentedImageDatabaseHandle, name, gCHandle.AddrOfPinnedObject(), imageSrc._width, imageSrc._height, imageSrc._width, width, ref out_index));
			if (gCHandle.IsAllocated)
			{
				gCHandle.Free();
			}
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to add aumented image at runtime with status {0}", apiArStatus);
				return -1;
			}
			return out_index;
		}

		private GCHandle ConvertTextureToGrayscaleBytes(AugmentedImageSrc imageSrc)
		{
			byte[] array = null;
			if (imageSrc._format == TextureFormat.RGB24 || imageSrc._format == TextureFormat.RGBA32)
			{
				Color[] pixels = imageSrc._pixels;
				array = new byte[pixels.Length];
				for (int i = 0; i < imageSrc._height; i++)
				{
					int num = i * imageSrc._width;
					for (int j = 0; j < imageSrc._width; j++)
					{
						int num2 = (imageSrc._height - 1 - i) * imageSrc._width + j;
						array[num + j] = (byte)((0.213 * (double)pixels[num2].r + 0.715 * (double)pixels[num2].g + 0.072 * (double)pixels[num2].b) * 255.0);
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Unsupported texture format " + imageSrc._format);
			}
			return GCHandle.Alloc(array, GCHandleType.Pinned);
		}
	}
	internal class CameraApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArCamera_getTrackingState(IntPtr sessionHandle, IntPtr cameraHandle, ref ApiTrackingState outTrackingState);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCamera_getTrackingFailureReason(IntPtr sessionHandle, IntPtr cameraHandle, ref ApiTrackingFailureReason outTrackingState);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCamera_getDisplayOrientedPose(IntPtr sessionHandle, IntPtr cameraHandle, IntPtr outPose);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCamera_getProjectionMatrix(IntPtr sessionHandle, IntPtr cameraHandle, float near, float far, ref Matrix4x4 outMatrix);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCamera_getTextureIntrinsics(IntPtr sessionHandle, IntPtr cameraHandle, IntPtr outCameraIntrinsics);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCamera_getImageIntrinsics(IntPtr sessionHandle, IntPtr cameraHandle, IntPtr outCameraIntrinsics);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCamera_release(IntPtr cameraHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraIntrinsics_create(IntPtr sessionHandle, ref IntPtr outCameraIntrinsics);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraIntrinsics_getFocalLength(IntPtr sessionHandle, IntPtr intrinsicsHandle, ref float outFx, ref float outFy);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraIntrinsics_getPrincipalPoint(IntPtr sessionHandle, IntPtr intrinsicsHandle, ref float outCx, ref float outCy);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraIntrinsics_getImageDimensions(IntPtr sessionHandle, IntPtr intrinsicsHandle, ref int outWidth, ref int outWeight);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraIntrinsics_destroy(IntPtr intrinsicsHandle);
		}

		private NativeSession _nativeSession;

		public CameraApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public TrackingState GetTrackingState(IntPtr cameraHandle)
		{
			ApiTrackingState outTrackingState = ApiTrackingState.Stopped;
			ExternApi.ArCamera_getTrackingState(_nativeSession.SessionHandle, cameraHandle, ref outTrackingState);
			return outTrackingState.ToTrackingState();
		}

		public LostTrackingReason GetLostTrackingReason(IntPtr cameraHandle)
		{
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("determine tracking failure reasons");
				return LostTrackingReason.None;
			}
			ApiTrackingFailureReason outTrackingState = ApiTrackingFailureReason.None;
			ExternApi.ArCamera_getTrackingFailureReason(_nativeSession.SessionHandle, cameraHandle, ref outTrackingState);
			return outTrackingState.ToLostTrackingReason();
		}

		public Pose GetPose(IntPtr cameraHandle)
		{
			if (cameraHandle == IntPtr.Zero)
			{
				return Pose.identity;
			}
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArCamera_getDisplayOrientedPose(_nativeSession.SessionHandle, cameraHandle, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public Matrix4x4 GetProjectionMatrix(IntPtr cameraHandle, float near, float far)
		{
			Matrix4x4 outMatrix = Matrix4x4.identity;
			ExternApi.ArCamera_getProjectionMatrix(_nativeSession.SessionHandle, cameraHandle, near, far, ref outMatrix);
			return outMatrix;
		}

		public CameraIntrinsics GetTextureIntrinsics(IntPtr cameraHandle)
		{
			IntPtr outCameraIntrinsics = IntPtr.Zero;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access GPU texture intrinsics");
				return default(CameraIntrinsics);
			}
			ExternApi.ArCameraIntrinsics_create(_nativeSession.SessionHandle, ref outCameraIntrinsics);
			ExternApi.ArCamera_getTextureIntrinsics(_nativeSession.SessionHandle, cameraHandle, outCameraIntrinsics);
			CameraIntrinsics cameraIntrinsicsFromHandle = GetCameraIntrinsicsFromHandle(outCameraIntrinsics);
			ExternApi.ArCameraIntrinsics_destroy(outCameraIntrinsics);
			return cameraIntrinsicsFromHandle;
		}

		public CameraIntrinsics GetImageIntrinsics(IntPtr cameraHandle)
		{
			IntPtr outCameraIntrinsics = IntPtr.Zero;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access CPU image intrinsics");
				return default(CameraIntrinsics);
			}
			ExternApi.ArCameraIntrinsics_create(_nativeSession.SessionHandle, ref outCameraIntrinsics);
			ExternApi.ArCamera_getImageIntrinsics(_nativeSession.SessionHandle, cameraHandle, outCameraIntrinsics);
			CameraIntrinsics cameraIntrinsicsFromHandle = GetCameraIntrinsicsFromHandle(outCameraIntrinsics);
			ExternApi.ArCameraIntrinsics_destroy(outCameraIntrinsics);
			return cameraIntrinsicsFromHandle;
		}

		public void Release(IntPtr cameraHandle)
		{
			ExternApi.ArCamera_release(cameraHandle);
		}

		private CameraIntrinsics GetCameraIntrinsicsFromHandle(IntPtr intrinsicsHandle)
		{
			float outFy;
			float outCx;
			float outCy;
			float outFx = (outFy = (outCx = (outCy = 0f)));
			int outWeight;
			int outWidth = (outWeight = 0);
			ExternApi.ArCameraIntrinsics_getFocalLength(_nativeSession.SessionHandle, intrinsicsHandle, ref outFx, ref outFy);
			ExternApi.ArCameraIntrinsics_getPrincipalPoint(_nativeSession.SessionHandle, intrinsicsHandle, ref outCx, ref outCy);
			ExternApi.ArCameraIntrinsics_getImageDimensions(_nativeSession.SessionHandle, intrinsicsHandle, ref outWidth, ref outWeight);
			return new CameraIntrinsics(new Vector2(outFx, outFy), new Vector2(outCx, outCy), new Vector2Int(outWidth, outWeight));
		}
	}
	internal class CameraConfigApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_create(IntPtr sessionHandle, ref IntPtr cameraConfigHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_destroy(IntPtr cameraConfigHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_getImageDimensions(IntPtr sessionHandle, IntPtr cameraConfigHandle, ref int width, ref int height);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_getTextureDimensions(IntPtr sessionHandle, IntPtr cameraConfigHandle, ref int width, ref int height);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_getFacingDirection(IntPtr sessionHandle, IntPtr cameraConfigHandle, ref DeviceCameraDirection facingDirection);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_getFpsRange(IntPtr sessionHandle, IntPtr cameraConfigHandle, ref int minFps, ref int maxFps);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_getDepthSensorUsage(IntPtr sessionHandle, IntPtr cameraConfigHandle, ref int depthSensorUsage);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfig_getStereoCameraUsage(IntPtr sessionHandle, IntPtr cameraConfigHandle, ref int stereoCameraUsage);
		}

		private NativeSession _nativeSession;

		public CameraConfigApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create()
		{
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("create ARCamera config");
				return IntPtr.Zero;
			}
			IntPtr cameraConfigHandle = IntPtr.Zero;
			ExternApi.ArCameraConfig_create(_nativeSession.SessionHandle, ref cameraConfigHandle);
			return cameraConfigHandle;
		}

		public void Destroy(IntPtr cameraConfigHandle)
		{
			ExternApi.ArCameraConfig_destroy(cameraConfigHandle);
		}

		public void GetImageDimensions(IntPtr cameraConfigHandle, out int width, out int height)
		{
			width = 0;
			height = 0;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access ARCamera image dimensions");
			}
			else
			{
				ExternApi.ArCameraConfig_getImageDimensions(_nativeSession.SessionHandle, cameraConfigHandle, ref width, ref height);
			}
		}

		public void GetTextureDimensions(IntPtr cameraConfigHandle, out int width, out int height)
		{
			width = 0;
			height = 0;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access ARCamera texture dimensions");
			}
			else
			{
				ExternApi.ArCameraConfig_getTextureDimensions(_nativeSession.SessionHandle, cameraConfigHandle, ref width, ref height);
			}
		}

		public DeviceCameraDirection GetFacingDirection(IntPtr cameraConfigHandle)
		{
			DeviceCameraDirection facingDirection = DeviceCameraDirection.BackFacing;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access ARCamera facing direction");
				return facingDirection;
			}
			ExternApi.ArCameraConfig_getFacingDirection(_nativeSession.SessionHandle, cameraConfigHandle, ref facingDirection);
			return facingDirection;
		}

		public void GetFpsRange(IntPtr cameraConfigHandle, out int minFps, out int maxFps)
		{
			minFps = 0;
			maxFps = 0;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access ARCamera FpsRange");
			}
			else
			{
				ExternApi.ArCameraConfig_getFpsRange(_nativeSession.SessionHandle, cameraConfigHandle, ref minFps, ref maxFps);
			}
		}

		public CameraConfigDepthSensorUsage GetDepthSensorUsage(IntPtr cameraConfigHandle)
		{
			int depthSensorUsage = 2;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access ARCamera DepthSensorUsage");
				return (CameraConfigDepthSensorUsage)depthSensorUsage;
			}
			ExternApi.ArCameraConfig_getDepthSensorUsage(_nativeSession.SessionHandle, cameraConfigHandle, ref depthSensorUsage);
			return (CameraConfigDepthSensorUsage)depthSensorUsage;
		}

		public CameraConfigStereoCameraUsage GetStereoCameraUsage(IntPtr cameraConfigHandle)
		{
			int stereoCameraUsage = 2;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access ARCamera StereoCameraUsage");
				return (CameraConfigStereoCameraUsage)stereoCameraUsage;
			}
			ExternApi.ArCameraConfig_getStereoCameraUsage(_nativeSession.SessionHandle, cameraConfigHandle, ref stereoCameraUsage);
			return (CameraConfigStereoCameraUsage)stereoCameraUsage;
		}
	}
	internal class CameraConfigFilterApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigFilter_create(IntPtr sessionHandle, ref IntPtr cameraConfigFilterHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigFilter_destroy(IntPtr cameraConfigFilterHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigFilter_setFacingDirection(IntPtr session, IntPtr filter, DeviceCameraDirection facing_direction_filter);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigFilter_setTargetFps(IntPtr sessionHandle, IntPtr cameraConfigFilterHandle, int fpsFilter);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigFilter_setDepthSensorUsage(IntPtr sessionHandle, IntPtr cameraConfigFilterHandle, int depthFilter);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigFilter_setStereoCameraUsage(IntPtr sessionHandle, IntPtr cameraConfigFilterHandle, int stereoFilter);
		}

		private NativeSession _nativeSession;

		public CameraConfigFilterApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create(DeviceCameraDirection direction, ARCoreCameraConfigFilter filter)
		{
			IntPtr cameraConfigFilterHandle = IntPtr.Zero;
			ExternApi.ArCameraConfigFilter_create(_nativeSession.SessionHandle, ref cameraConfigFilterHandle);
			ExternApi.ArCameraConfigFilter_setFacingDirection(_nativeSession.SessionHandle, cameraConfigFilterHandle, direction);
			if (filter != null)
			{
				if (filter.TargetCameraFramerate != null)
				{
					ExternApi.ArCameraConfigFilter_setTargetFps(_nativeSession.SessionHandle, cameraConfigFilterHandle, ConvertToFpsFilter(filter.TargetCameraFramerate));
				}
				if (filter.DepthSensorUsage != null)
				{
					ExternApi.ArCameraConfigFilter_setDepthSensorUsage(_nativeSession.SessionHandle, cameraConfigFilterHandle, ConvertToDepthFilter(filter.DepthSensorUsage));
				}
				if (filter.StereoCameraUsage != null)
				{
					ExternApi.ArCameraConfigFilter_setStereoCameraUsage(_nativeSession.SessionHandle, cameraConfigFilterHandle, ConvertToStereoFilter(filter.StereoCameraUsage));
				}
			}
			return cameraConfigFilterHandle;
		}

		public void Destroy(IntPtr cameraConfigListHandle)
		{
			ExternApi.ArCameraConfigFilter_destroy(cameraConfigListHandle);
		}

		private int ConvertToFpsFilter(ARCoreCameraConfigFilter.TargetCameraFramerateFilter targetCameraFramerate)
		{
			int num = 0;
			if (targetCameraFramerate.Target30FPS)
			{
				num |= 1;
			}
			if (targetCameraFramerate.Target60FPS)
			{
				num |= 2;
			}
			return num;
		}

		private int ConvertToDepthFilter(ARCoreCameraConfigFilter.DepthSensorUsageFilter depthSensorUsage)
		{
			int num = 0;
			if (depthSensorUsage.RequireAndUse)
			{
				num |= 1;
			}
			if (depthSensorUsage.DoNotUse)
			{
				num |= 2;
			}
			return num;
		}

		private int ConvertToStereoFilter(ARCoreCameraConfigFilter.StereoCameraUsageFilter stereoCameraUsage)
		{
			int num = 0;
			if (stereoCameraUsage.RequireAndUse)
			{
				num |= 1;
			}
			if (stereoCameraUsage.DoNotUse)
			{
				num |= 2;
			}
			return num;
		}
	}
	internal class CameraConfigListApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigList_create(IntPtr sessionHandle, ref IntPtr cameraConfigListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigList_destroy(IntPtr cameraConfigListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigList_getSize(IntPtr sessionHandle, IntPtr cameraConfigListHandle, ref int size);

			[DllImport("arcore_sdk_c")]
			public static extern void ArCameraConfigList_getItem(IntPtr sessionHandle, IntPtr cameraConfigListHandle, int index, IntPtr itemHandle);
		}

		private NativeSession _nativeSession;

		public CameraConfigListApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create()
		{
			IntPtr cameraConfigListHandle = IntPtr.Zero;
			ExternApi.ArCameraConfigList_create(_nativeSession.SessionHandle, ref cameraConfigListHandle);
			return cameraConfigListHandle;
		}

		public void Destroy(IntPtr cameraConfigListHandle)
		{
			ExternApi.ArCameraConfigList_destroy(cameraConfigListHandle);
		}

		public int GetSize(IntPtr cameraConfigListHandle)
		{
			int size = 0;
			ExternApi.ArCameraConfigList_getSize(_nativeSession.SessionHandle, cameraConfigListHandle, ref size);
			return size;
		}

		public void GetItemAt(IntPtr cameraConfigListHandle, int index, IntPtr cameraConfigHandle)
		{
			ExternApi.ArCameraConfigList_getItem(_nativeSession.SessionHandle, cameraConfigListHandle, index, cameraConfigHandle);
		}
	}
	internal class CameraMetadataApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArImageMetadata_release(IntPtr metadata);

			[DllImport("arcore_sdk_c")]
			public static extern void ArImageMetadata_getAllKeys(IntPtr session, IntPtr image_metadata, ref int out_number_of_tags, ref IntPtr out_tags);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArImageMetadata_getConstEntry(IntPtr session, IntPtr image_metadata, uint tag, ref ArCameraMetadata out_metadata_entry);
		}

		private const int _maximumTagCountForWarning = 5000;

		private HashSet<int> _warningTags = new HashSet<int>();

		private NativeSession _nativeSession;

		public CameraMetadataApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public void Release(IntPtr arCameraMetadataHandle)
		{
			ExternApi.ArImageMetadata_release(arCameraMetadataHandle);
		}

		public bool TryGetValues(IntPtr cameraMetadataHandle, CameraMetadataTag tag, List<CameraMetadataValue> resultList)
		{
			resultList.Clear();
			ArCameraMetadata out_metadata_entry = default(ArCameraMetadata);
			ApiArStatus apiArStatus = ExternApi.ArImageMetadata_getConstEntry(_nativeSession.SessionHandle, cameraMetadataHandle, (uint)tag, ref out_metadata_entry);
			if (apiArStatus != ApiArStatus.Success)
			{
				ARDebug.LogErrorFormat("ArImageMetadata_getConstEntry error with native camera error code: {0}", apiArStatus);
				return false;
			}
			if (out_metadata_entry.Count > 5000 && !_warningTags.Contains((int)tag))
			{
				UnityEngine.Debug.LogWarningFormat("TryGetValues for tag {0} has {1} values. Accessing tags with a large number of values may impede performance.", tag, out_metadata_entry.Count);
				_warningTags.Add((int)tag);
			}
			for (int i = 0; i < out_metadata_entry.Count; i++)
			{
				switch (out_metadata_entry.Type)
				{
				case ArCameraMetadataType.Byte:
				{
					sbyte byteValue = (sbyte)Marshal.PtrToStructure(MarshalingHelper.GetPtrToUnmanagedArrayElement<sbyte>(out_metadata_entry.Value, i), typeof(sbyte));
					resultList.Add(new CameraMetadataValue(byteValue));
					break;
				}
				case ArCameraMetadataType.Int32:
				{
					int intValue = (int)Marshal.PtrToStructure(MarshalingHelper.GetPtrToUnmanagedArrayElement<int>(out_metadata_entry.Value, i), typeof(int));
					resultList.Add(new CameraMetadataValue(intValue));
					break;
				}
				case ArCameraMetadataType.Float:
				{
					float floatValue = (float)Marshal.PtrToStructure(MarshalingHelper.GetPtrToUnmanagedArrayElement<float>(out_metadata_entry.Value, i), typeof(float));
					resultList.Add(new CameraMetadataValue(floatValue));
					break;
				}
				case ArCameraMetadataType.Int64:
				{
					long longValue = (long)Marshal.PtrToStructure(MarshalingHelper.GetPtrToUnmanagedArrayElement<long>(out_metadata_entry.Value, i), typeof(long));
					resultList.Add(new CameraMetadataValue(longValue));
					break;
				}
				case ArCameraMetadataType.Double:
				{
					double doubleValue = (double)Marshal.PtrToStructure(MarshalingHelper.GetPtrToUnmanagedArrayElement<double>(out_metadata_entry.Value, i), typeof(double));
					resultList.Add(new CameraMetadataValue(doubleValue));
					break;
				}
				case ArCameraMetadataType.Rational:
				{
					CameraMetadataRational rationalValue = (CameraMetadataRational)Marshal.PtrToStructure(MarshalingHelper.GetPtrToUnmanagedArrayElement<CameraMetadataRational>(out_metadata_entry.Value, i), typeof(CameraMetadataRational));
					resultList.Add(new CameraMetadataValue(rationalValue));
					break;
				}
				default:
					return false;
				}
			}
			return true;
		}

		public bool GetAllCameraMetadataTags(IntPtr cameraMetadataHandle, List<CameraMetadataTag> resultList)
		{
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access camera metadata tags");
				return false;
			}
			IntPtr out_tags = IntPtr.Zero;
			int out_number_of_tags = 0;
			ExternApi.ArImageMetadata_getAllKeys(_nativeSession.SessionHandle, cameraMetadataHandle, ref out_number_of_tags, ref out_tags);
			if (out_number_of_tags == 0 || out_tags == IntPtr.Zero)
			{
				ARDebug.LogError("ArImageMetadata_getAllKeys error with empty metadata keys list.");
				return false;
			}
			for (int i = 0; i < out_number_of_tags; i++)
			{
				resultList.Add((CameraMetadataTag)Marshal.PtrToStructure(MarshalingHelper.GetPtrToUnmanagedArrayElement<int>(out_tags, i), typeof(int)));
			}
			return true;
		}
	}
	internal class FrameApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_release(IntPtr frame);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_getTimestamp(IntPtr sessionHandle, IntPtr frame, ref long timestamp);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_acquireCamera(IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr cameraHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArFrame_acquireCameraImage(IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr imageHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArFrame_acquirePointCloud(IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr pointCloudHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_transformDisplayUvCoords(IntPtr session, IntPtr frame, int numElements, ref ApiDisplayUvCoords uvsIn, ref ApiDisplayUvCoords uvsOut);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_transformCoordinates2d(IntPtr session, IntPtr frame, ApiCoordinates2dType inputType, int numVertices, ref Vector2 uvsIn, ApiCoordinates2dType outputType, ref Vector2 uvsOut);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_getUpdatedTrackables(IntPtr sessionHandle, IntPtr frameHandle, ApiTrackableType filterType, IntPtr outTrackableList);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_getLightEstimate(IntPtr sessionHandle, IntPtr frameHandle, IntPtr lightEstimateHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArFrame_acquireImageMetadata(IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr outMetadata);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_getCameraTextureName(IntPtr sessionHandle, IntPtr frameHandle, ref int outTextureId);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArFrame_recordTrackData(IntPtr sessionHandle, IntPtr frameHandle, IntPtr trackIdBytes, IntPtr dataBytes, int dataSize);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_getUpdatedTrackData(IntPtr sessionHandle, IntPtr frameHandle, IntPtr trackId, IntPtr trackDataList);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArFrame_acquireDepthImage(IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr imageHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArImage_getPlanePixelStride(IntPtr sessionHandle, IntPtr imageHandle, int planeIndex, ref int pixelStride);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArFrame_acquireRawDepthImage(IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr imageHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArFrame_acquireRawDepthConfidenceImage(IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr imageHandle);
		}

		private NativeSession _nativeSession;

		private float[,] _ambientSH = new float[9, 3];

		public FrameApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public void Release(IntPtr frameHandle)
		{
			ExternApi.ArFrame_release(frameHandle);
		}

		public long GetTimestamp()
		{
			long timestamp = 0L;
			ExternApi.ArFrame_getTimestamp(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref timestamp);
			return timestamp;
		}

		public IntPtr AcquireCamera()
		{
			IntPtr cameraHandle = IntPtr.Zero;
			ExternApi.ArFrame_acquireCamera(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref cameraHandle);
			return cameraHandle;
		}

		public CameraImageBytes AcquireCameraImageBytes()
		{
			IntPtr imageHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = ExternApi.ArFrame_acquireCameraImage(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref imageHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to acquire camera image with status {0}.", apiArStatus);
				return new CameraImageBytes(IntPtr.Zero);
			}
			return new CameraImageBytes(imageHandle);
		}

		public bool TryAcquirePointCloudHandle(out IntPtr pointCloudHandle)
		{
			pointCloudHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = ExternApi.ArFrame_acquirePointCloud(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref pointCloudHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to acquire point cloud with status {0}", apiArStatus);
				return false;
			}
			return true;
		}

		public bool AcquireImageMetadata(ref IntPtr imageMetadataHandle)
		{
			ApiArStatus apiArStatus = ExternApi.ArFrame_acquireImageMetadata(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref imageMetadataHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogErrorFormat("Failed to aquire camera image metadata with status {0}", apiArStatus);
				return false;
			}
			return true;
		}

		public LightEstimate GetLightEstimate()
		{
			IntPtr lightEstimateHandle = _nativeSession.LightEstimateApi.Create();
			ExternApi.ArFrame_getLightEstimate(_nativeSession.SessionHandle, _nativeSession.FrameHandle, lightEstimateHandle);
			LightEstimateState state = _nativeSession.LightEstimateApi.GetState(lightEstimateHandle);
			Color colorCorrection = _nativeSession.LightEstimateApi.GetColorCorrection(lightEstimateHandle);
			long timestamp = _nativeSession.LightEstimateApi.GetTimestamp(_nativeSession.SessionHandle, lightEstimateHandle);
			Quaternion lightRotation = Quaternion.identity;
			Color lightColor = Color.black;
			_nativeSession.LightEstimateApi.GetMainDirectionalLight(_nativeSession.SessionHandle, lightEstimateHandle, out lightRotation, out lightColor);
			_nativeSession.LightEstimateApi.GetAmbientSH(_nativeSession.SessionHandle, lightEstimateHandle, _ambientSH);
			_nativeSession.LightEstimateApi.Destroy(lightEstimateHandle);
			return new LightEstimate(state, colorCorrection.a, new Color(colorCorrection.r, colorCorrection.g, colorCorrection.b, 1f), lightRotation, lightColor, _ambientSH, timestamp);
		}

		public Cubemap GetReflectionCubemap()
		{
			IntPtr lightEstimateHandle = _nativeSession.LightEstimateApi.Create();
			ExternApi.ArFrame_getLightEstimate(_nativeSession.SessionHandle, _nativeSession.FrameHandle, lightEstimateHandle);
			if (_nativeSession.LightEstimateApi.GetState(lightEstimateHandle) != LightEstimateState.Valid)
			{
				return null;
			}
			Cubemap reflectionCubemap = _nativeSession.LightEstimateApi.GetReflectionCubemap(_nativeSession.SessionHandle, lightEstimateHandle);
			_nativeSession.LightEstimateApi.Destroy(lightEstimateHandle);
			return reflectionCubemap;
		}

		public void TransformDisplayUvCoords(ref ApiDisplayUvCoords uv)
		{
			ApiDisplayUvCoords uvsOut = default(ApiDisplayUvCoords);
			ExternApi.ArFrame_transformDisplayUvCoords(_nativeSession.SessionHandle, _nativeSession.FrameHandle, 8, ref uv, ref uvsOut);
			uv = uvsOut;
		}

		public void TransformCoordinates2d(ref Vector2 uv, DisplayUvCoordinateType inputType, DisplayUvCoordinateType outputType)
		{
			Vector2 uvsOut = new Vector2(uv.x, uv.y);
			ExternApi.ArFrame_transformCoordinates2d(_nativeSession.SessionHandle, _nativeSession.FrameHandle, inputType.ToApiCoordinates2dType(), 1, ref uv, outputType.ToApiCoordinates2dType(), ref uvsOut);
			uv = uvsOut;
		}

		public void GetUpdatedTrackables(List<Trackable> trackables)
		{
			IntPtr intPtr = _nativeSession.TrackableListApi.Create();
			ExternApi.ArFrame_getUpdatedTrackables(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ApiTrackableType.BaseTrackable, intPtr);
			trackables.Clear();
			int count = _nativeSession.TrackableListApi.GetCount(intPtr);
			for (int i = 0; i < count; i++)
			{
				IntPtr intPtr2 = _nativeSession.TrackableListApi.AcquireItem(intPtr, i);
				if (_nativeSession.TrackableApi.GetType(intPtr2) == ApiTrackableType.AugmentedFace)
				{
					_nativeSession.TrackableApi.Release(intPtr2);
					continue;
				}
				Trackable trackable = _nativeSession.TrackableFactory(intPtr2);
				if (trackable != null)
				{
					trackables.Add(trackable);
				}
				else
				{
					_nativeSession.TrackableApi.Release(intPtr2);
				}
			}
			_nativeSession.TrackableListApi.Destroy(intPtr);
		}

		public int GetCameraTextureName()
		{
			int outTextureId = -1;
			ExternApi.ArFrame_getCameraTextureName(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref outTextureId);
			return outTextureId;
		}

		public DepthStatus UpdateDepthTexture(ref Texture2D depthTexture)
		{
			IntPtr imageHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = ExternApi.ArFrame_acquireDepthImage(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref imageHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogErrorFormat("[ARCore] failed to acquire depth image with status {0}", apiArStatus.ToString());
				return apiArStatus.ToDepthStatus();
			}
			if (!UpdateDepthTexture(ref depthTexture, imageHandle))
			{
				return DepthStatus.InternalError;
			}
			return DepthStatus.Success;
		}

		public DepthStatus UpdateRawDepthTexture(ref Texture2D depthTexture)
		{
			IntPtr imageHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = ExternApi.ArFrame_acquireRawDepthImage(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref imageHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogErrorFormat("[ARCore] failed to acquire raw depth image with status {0}", apiArStatus.ToString());
				return apiArStatus.ToDepthStatus();
			}
			if (!UpdateDepthTexture(ref depthTexture, imageHandle))
			{
				return DepthStatus.InternalError;
			}
			return DepthStatus.Success;
		}

		public DepthStatus UpdateRawDepthConfidenceTexture(ref Texture2D confidenceTexture)
		{
			IntPtr imageHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = ExternApi.ArFrame_acquireRawDepthConfidenceImage(_nativeSession.SessionHandle, _nativeSession.FrameHandle, ref imageHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogErrorFormat("[ARCore] failed to acquire raw depth confidence image with status {0}", apiArStatus.ToString());
				return apiArStatus.ToDepthStatus();
			}
			if (!UpdateDepthTexture(ref confidenceTexture, imageHandle))
			{
				return DepthStatus.InternalError;
			}
			return DepthStatus.Success;
		}

		public RecordingResult RecordTrackData(Guid trackId, byte[] data)
		{
			GCHandle gCHandle = GCHandle.Alloc(trackId.ToByteArray(), GCHandleType.Pinned);
			GCHandle gCHandle2 = GCHandle.Alloc(data, GCHandleType.Pinned);
			ApiArStatus recordingResult = ExternApi.ArFrame_recordTrackData(_nativeSession.SessionHandle, _nativeSession.FrameHandle, gCHandle.AddrOfPinnedObject(), gCHandle2.AddrOfPinnedObject(), data.Length);
			if (gCHandle.IsAllocated)
			{
				gCHandle.Free();
			}
			if (gCHandle2.IsAllocated)
			{
				gCHandle2.Free();
			}
			return recordingResult.ToRecordingResult();
		}

		public List<TrackData> GetUpdatedTrackData(Guid trackId)
		{
			List<TrackData> list = new List<TrackData>();
			IntPtr intPtr = _nativeSession.TrackDataListApi.Create();
			GCHandle gCHandle = GCHandle.Alloc(trackId.ToByteArray(), GCHandleType.Pinned);
			ExternApi.ArFrame_getUpdatedTrackData(_nativeSession.SessionHandle, _nativeSession.FrameHandle, gCHandle.AddrOfPinnedObject(), intPtr);
			if (gCHandle.IsAllocated)
			{
				gCHandle.Free();
			}
			int count = _nativeSession.TrackDataListApi.GetCount(intPtr);
			TrackData item = default(TrackData);
			for (int i = 0; i < count; i++)
			{
				IntPtr trackDataHandle = _nativeSession.TrackDataListApi.AcquireItem(intPtr, i);
				item.FrameTimestamp = _nativeSession.TrackDataApi.GetFrameTimestamp(trackDataHandle);
				item.Data = _nativeSession.TrackDataApi.GetData(trackDataHandle);
				list.Add(item);
			}
			_nativeSession.TrackDataListApi.Destroy(intPtr);
			return list;
		}

		private bool UpdateDepthTexture(ref Texture2D texture, IntPtr imageHandle)
		{
			int width = _nativeSession.ImageApi.GetWidth(imageHandle);
			int height = _nativeSession.ImageApi.GetHeight(imageHandle);
			IntPtr surfaceData = IntPtr.Zero;
			int dataLength = 0;
			_nativeSession.ImageApi.GetPlaneData(imageHandle, 0, ref surfaceData, ref dataLength);
			IntPtr data = new IntPtr(surfaceData.ToInt64());
			int pixelStride = 0;
			ExternApi.ArImage_getPlanePixelStride(_nativeSession.SessionHandle, imageHandle, 0, ref pixelStride);
			if (width != texture.width || height != texture.height)
			{
				TextureFormat textureFormat = ((pixelStride != 2) ? TextureFormat.Alpha8 : TextureFormat.RGB565);
				if (!texture.Resize(width, height, textureFormat, hasMipMap: false))
				{
					UnityEngine.Debug.LogErrorFormat("Unable to resize texture. Current: width {0} height {1} format {2} Desired: width {3} height {4} format {5} ", texture.width, texture.height, texture.format.ToString(), width, height, textureFormat);
					_nativeSession.ImageApi.Release(imageHandle);
					return false;
				}
			}
			texture.LoadRawTextureData(data, dataLength);
			texture.Apply();
			_nativeSession.ImageApi.Release(imageHandle);
			return true;
		}
	}
	internal class HitTestApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_hitTest(IntPtr session, IntPtr frame, float pixel_x, float pixel_y, IntPtr hit_result_list);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_hitTestRay(IntPtr session, IntPtr frame, ref Vector3 origin, ref Vector3 direction, IntPtr hit_result_list);

			[DllImport("arcore_sdk_c")]
			public static extern void ArFrame_hitTestInstantPlacement(IntPtr session, IntPtr frame, float pixel_x, float pixel_y, float guessed_distance_meters, IntPtr hit_result_list);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResultList_create(IntPtr session, ref IntPtr out_hit_result_list);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResultList_destroy(IntPtr hit_result_list);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResultList_getSize(IntPtr session, IntPtr hit_result_list, ref int out_size);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResultList_getItem(IntPtr session, IntPtr hit_result_list, int index, IntPtr out_hit_result);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResult_create(IntPtr session, ref IntPtr out_hit_result);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResult_destroy(IntPtr hit_result);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResult_getDistance(IntPtr session, IntPtr hit_result, ref float out_distance);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResult_getHitPose(IntPtr session, IntPtr hit_result, IntPtr out_pose);

			[DllImport("arcore_sdk_c")]
			public static extern void ArHitResult_acquireTrackable(IntPtr session, IntPtr hit_result, ref IntPtr out_trackable);
		}

		private NativeSession _nativeSession;

		public HitTestApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public bool Raycast(IntPtr frameHandle, float x, float y, TrackableHitFlags filter, List<TrackableHit> outHitList)
		{
			outHitList.Clear();
			IntPtr out_hit_result_list = IntPtr.Zero;
			ExternApi.ArHitResultList_create(_nativeSession.SessionHandle, ref out_hit_result_list);
			ExternApi.ArFrame_hitTest(_nativeSession.SessionHandle, frameHandle, x, y, out_hit_result_list);
			FilterTrackableHits(out_hit_result_list, float.PositiveInfinity, filter, outHitList);
			ExternApi.ArHitResultList_destroy(out_hit_result_list);
			return outHitList.Count != 0;
		}

		public bool Raycast(IntPtr frameHandle, float x, float y, float approximateDistanceMeters, List<TrackableHit> outHitList)
		{
			outHitList.Clear();
			IntPtr out_hit_result_list = IntPtr.Zero;
			ExternApi.ArHitResultList_create(_nativeSession.SessionHandle, ref out_hit_result_list);
			ExternApi.ArFrame_hitTestInstantPlacement(_nativeSession.SessionHandle, frameHandle, x, y, approximateDistanceMeters, out_hit_result_list);
			FilterTrackableHits(out_hit_result_list, float.PositiveInfinity, TrackableHitFlags.None, outHitList);
			ExternApi.ArHitResultList_destroy(out_hit_result_list);
			return outHitList.Count != 0;
		}

		public bool Raycast(IntPtr frameHandle, Vector3 origin, Vector3 direction, float maxDistance, TrackableHitFlags filter, List<TrackableHit> outHitList)
		{
			outHitList.Clear();
			IntPtr out_hit_result_list = IntPtr.Zero;
			ExternApi.ArHitResultList_create(_nativeSession.SessionHandle, ref out_hit_result_list);
			origin.z = 0f - origin.z;
			direction.z = 0f - direction.z;
			ExternApi.ArFrame_hitTestRay(_nativeSession.SessionHandle, frameHandle, ref origin, ref direction, out_hit_result_list);
			FilterTrackableHits(out_hit_result_list, maxDistance, filter, outHitList);
			ExternApi.ArHitResultList_destroy(out_hit_result_list);
			return outHitList.Count != 0;
		}

		private void FilterTrackableHits(IntPtr hitResultListHandle, float maxDistance, TrackableHitFlags filter, List<TrackableHit> outHitList)
		{
			int out_size = 0;
			ExternApi.ArHitResultList_getSize(_nativeSession.SessionHandle, hitResultListHandle, ref out_size);
			for (int i = 0; i < out_size; i++)
			{
				if (HitResultListGetItemAt(hitResultListHandle, i, out var outTrackableHit))
				{
					if ((filter & outTrackableHit.Flags) != TrackableHitFlags.None && outTrackableHit.Distance <= maxDistance)
					{
						outHitList.Add(outTrackableHit);
					}
					else if (outTrackableHit.Trackable is InstantPlacementPoint && outTrackableHit.Distance <= maxDistance)
					{
						outHitList.Add(outTrackableHit);
					}
				}
			}
		}

		private bool HitResultListGetItemAt(IntPtr hitResultListHandle, int index, out TrackableHit outTrackableHit)
		{
			outTrackableHit = default(TrackableHit);
			IntPtr out_hit_result = IntPtr.Zero;
			ExternApi.ArHitResult_create(_nativeSession.SessionHandle, ref out_hit_result);
			ExternApi.ArHitResultList_getItem(_nativeSession.SessionHandle, hitResultListHandle, index, out_hit_result);
			if (out_hit_result == IntPtr.Zero)
			{
				ExternApi.ArHitResult_destroy(out_hit_result);
				return false;
			}
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArHitResult_getHitPose(_nativeSession.SessionHandle, out_hit_result, intPtr);
			Pose pose = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			float out_distance = 0f;
			ExternApi.ArHitResult_getDistance(_nativeSession.SessionHandle, out_hit_result, ref out_distance);
			IntPtr out_trackable = IntPtr.Zero;
			ExternApi.ArHitResult_acquireTrackable(_nativeSession.SessionHandle, out_hit_result, ref out_trackable);
			Trackable trackable = _nativeSession.TrackableFactory(out_trackable);
			_nativeSession.TrackableApi.Release(out_trackable);
			TrackableHitFlags trackableHitFlags = TrackableHitFlags.None;
			if (trackable == null)
			{
				UnityEngine.Debug.Log("Could not create trackable from hit result.");
				_nativeSession.PoseApi.Destroy(intPtr);
				return false;
			}
			if (trackable is DetectedPlane)
			{
				if (_nativeSession.PlaneApi.IsPoseInPolygon(out_trackable, intPtr))
				{
					trackableHitFlags |= TrackableHitFlags.PlaneWithinPolygon;
				}
				if (_nativeSession.PlaneApi.IsPoseInExtents(out_trackable, intPtr))
				{
					trackableHitFlags |= TrackableHitFlags.PlaneWithinBounds;
				}
				trackableHitFlags |= TrackableHitFlags.PlaneWithinInfinity;
			}
			else if (trackable is FeaturePoint)
			{
				FeaturePoint obj = trackable as FeaturePoint;
				trackableHitFlags |= TrackableHitFlags.FeaturePoint;
				if (obj.OrientationMode == FeaturePointOrientationMode.SurfaceNormal)
				{
					trackableHitFlags |= TrackableHitFlags.FeaturePointWithSurfaceNormal;
				}
			}
			else if (!(trackable is InstantPlacementPoint))
			{
				if (trackable is DepthPoint)
				{
					trackableHitFlags |= TrackableHitFlags.Depth;
				}
				else
				{
					ApiTrackableType type = _nativeSession.TrackableApi.GetType(out_trackable);
					if (!ExperimentManager.Instance.IsManagingTrackableType((int)type))
					{
						_nativeSession.PoseApi.Destroy(intPtr);
						return false;
					}
					trackableHitFlags |= ExperimentManager.Instance.GetTrackableHitFlags((int)type);
				}
			}
			outTrackableHit = new TrackableHit(pose, out_distance, trackableHitFlags, trackable);
			_nativeSession.PoseApi.Destroy(intPtr);
			return true;
		}
	}
	internal class ImageApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArImage_release(IntPtr imageHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArImage_getWidth(IntPtr sessionHandle, IntPtr imageHandle, out int width);

			[DllImport("arcore_sdk_c")]
			public static extern void ArImage_getHeight(IntPtr sessionHandle, IntPtr imageHandle, out int height);

			[DllImport("arcore_sdk_c")]
			public static extern void ArImage_getPlaneData(IntPtr sessionHandle, IntPtr imageHandle, int planeIndex, ref IntPtr surfaceData, ref int dataLength);

			[DllImport("arcore_sdk_c")]
			public static extern void ArImage_getPlanePixelStride(IntPtr sessionHandle, IntPtr imageHandle, int planeIdx, ref int pixelStride);

			[DllImport("arcore_sdk_c")]
			public static extern void ArImage_getPlaneRowStride(IntPtr sessionHandle, IntPtr imageHandle, int planeIdx, ref int rowStride);
		}

		private NativeSession _nativeSession;

		public ImageApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public int GetPlanePixelStride(IntPtr imageHandle, int planeIndex)
		{
			int pixelStride = 0;
			ExternApi.ArImage_getPlanePixelStride(_nativeSession.SessionHandle, imageHandle, planeIndex, ref pixelStride);
			return pixelStride;
		}

		public int GetPlaneRowStride(IntPtr imageHandle, int planeIndex)
		{
			int rowStride = 0;
			ExternApi.ArImage_getPlaneRowStride(_nativeSession.SessionHandle, imageHandle, planeIndex, ref rowStride);
			return rowStride;
		}

		public void GetPlaneData(IntPtr imageHandle, int planeIndex, ref IntPtr surfaceData, ref int dataLength)
		{
			ExternApi.ArImage_getPlaneData(_nativeSession.SessionHandle, imageHandle, planeIndex, ref surfaceData, ref dataLength);
		}

		public int GetWidth(IntPtr imageHandle)
		{
			int width = 0;
			ExternApi.ArImage_getWidth(_nativeSession.SessionHandle, imageHandle, out width);
			return width;
		}

		public int GetHeight(IntPtr imageHandle)
		{
			int height = 0;
			ExternApi.ArImage_getHeight(_nativeSession.SessionHandle, imageHandle, out height);
			return height;
		}

		public void Release(IntPtr imageHandle)
		{
			ExternApi.ArImage_release(imageHandle);
		}
	}
	internal class LightEstimateApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_create(IntPtr sessionHandle, ref IntPtr lightEstimateHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_destroy(IntPtr lightEstimateHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_getState(IntPtr sessionHandle, IntPtr lightEstimateHandle, ref ApiLightEstimateState state);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_getPixelIntensity(IntPtr sessionHandle, IntPtr lightEstimateHandle, ref float pixelIntensity);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_getColorCorrection(IntPtr sessionHandle, IntPtr lightEstimateHandle, ref Color colorCorrection);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_getEnvironmentalHdrMainLightDirection(IntPtr sessionHandle, IntPtr lightEstimateHandle, float[] direction);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_getEnvironmentalHdrMainLightIntensity(IntPtr sessionHandle, IntPtr lightEstimateHandle, float[] color);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_getEnvironmentalHdrAmbientSphericalHarmonics(IntPtr session, IntPtr light_estimate, float[] out_coefficients_27);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_acquireEnvironmentalHdrCubemap(IntPtr session, IntPtr light_estimate, ref IntPtr out_textures_6);

			[DllImport("arcore_sdk_c")]
			public static extern void ArLightEstimate_getTimestamp(IntPtr session, IntPtr light_estimate, ref long timestamp);

			[DllImport("arcore_rendering_utils_api")]
			public static extern void ARCoreRenderingUtils_SetTextureDataType(ApiTextureDataType texture_data_type, bool create_gl_texture);

			[DllImport("arcore_rendering_utils_api")]
			public static extern void ARCoreRenderingUtils_SetActiveColorSpace(bool is_gamma_space);

			[DllImport("arcore_rendering_utils_api")]
			public static extern void ARCoreRenderingUtils_SetARCoreLightEstimation(IntPtr session, IntPtr cubemap_image);

			[DllImport("arcore_rendering_utils_api")]
			public static extern void ARCoreRenderingUtils_GetCubemapTexture(ref int out_texture_id, ref int out_width_height);

			[DllImport("arcore_rendering_utils_api")]
			public static extern void ARCoreRenderingUtils_GetCubemapRawColors(int face_index, Color[] out_pixel_colors);

			[DllImport("arcore_rendering_utils_api")]
			public static extern IntPtr ARCoreRenderingUtils_GetRenderEventFunc();
		}

		internal static readonly float[] _shConstants = new float[9] { 0.886227f, 1.023328f, 1.023328f, 1.023328f, 0.858086f, 0.858086f, 0.247708f, 0.858086f, 0.429043f };

		private NativeSession _nativeSession;

		private float[] _tempVector = new float[3];

		private float[] _tempColor = new float[3];

		private float[] _tempSHCoefficients = new float[27];

		private Cubemap _hdrCubemap;

		private long _cubemapTimestamp = -1L;

		private int _cubemapTextureId;

		private bool _pluginInitialized;

		public LightEstimateApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create()
		{
			IntPtr lightEstimateHandle = IntPtr.Zero;
			ExternApi.ArLightEstimate_create(_nativeSession.SessionHandle, ref lightEstimateHandle);
			return lightEstimateHandle;
		}

		public void Destroy(IntPtr lightEstimateHandle)
		{
			ExternApi.ArLightEstimate_destroy(lightEstimateHandle);
		}

		public LightEstimateState GetState(IntPtr lightEstimateHandle)
		{
			ApiLightEstimateState state = ApiLightEstimateState.NotValid;
			ExternApi.ArLightEstimate_getState(_nativeSession.SessionHandle, lightEstimateHandle, ref state);
			return state.ToLightEstimateState();
		}

		public float GetPixelIntensity(IntPtr lightEstimateHandle)
		{
			float pixelIntensity = 0f;
			ExternApi.ArLightEstimate_getPixelIntensity(_nativeSession.SessionHandle, lightEstimateHandle, ref pixelIntensity);
			return pixelIntensity;
		}

		public Color GetColorCorrection(IntPtr lightEstimateHandle)
		{
			Color colorCorrection = Color.black;
			ExternApi.ArLightEstimate_getColorCorrection(_nativeSession.SessionHandle, lightEstimateHandle, ref colorCorrection);
			return colorCorrection;
		}

		public void GetMainDirectionalLight(IntPtr sessionHandle, IntPtr lightEstimateHandle, out Quaternion lightRotation, out Color lightColor)
		{
			lightColor = Color.black;
			ExternApi.ArLightEstimate_getEnvironmentalHdrMainLightIntensity(sessionHandle, lightEstimateHandle, _tempColor);
			lightColor.r = _tempColor[0];
			lightColor.g = _tempColor[1];
			lightColor.b = _tempColor[2];
			lightColor /= (float)Math.PI;
			ExternApi.ArLightEstimate_getEnvironmentalHdrMainLightDirection(sessionHandle, lightEstimateHandle, _tempVector);
			Vector3 unityVector = Vector3.one;
			ConversionHelper.ApiVectorToUnityVector(_tempVector, out unityVector);
			lightRotation = Quaternion.LookRotation(-unityVector);
		}

		public void GetAmbientSH(IntPtr sessionHandle, IntPtr lightEstimateHandle, float[,] outSHCoefficients)
		{
			ExternApi.ArLightEstimate_getEnvironmentalHdrAmbientSphericalHarmonics(sessionHandle, lightEstimateHandle, _tempSHCoefficients);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					outSHCoefficients[j, i] = _tempSHCoefficients[j * 3 + i];
					if (j == 2 || j == 5 || j == 7)
					{
						outSHCoefficients[j, i] *= -1f;
					}
					outSHCoefficients[j, i] *= _shConstants[j];
					outSHCoefficients[j, i] /= (float)Math.PI;
				}
			}
		}

		public Cubemap GetReflectionCubemap(IntPtr sessionHandle, IntPtr lightEstimateHandle)
		{
			int out_width_height = 0;
			bool is_gamma_space = QualitySettings.activeColorSpace == ColorSpace.Gamma;
			int out_texture_id = 0;
			ApiTextureDataType apiTextureDataType = ((SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3) ? ApiTextureDataType.Half : ApiTextureDataType.Byte);
			TextureFormat format = ((apiTextureDataType == ApiTextureDataType.Half) ? TextureFormat.RGBAHalf : TextureFormat.RGBA32);
			if (!_pluginInitialized)
			{
				ExternApi.ARCoreRenderingUtils_SetTextureDataType(apiTextureDataType, create_gl_texture: true);
				ExternApi.ARCoreRenderingUtils_SetActiveColorSpace(is_gamma_space);
				_pluginInitialized = true;
			}
			ExternApi.ARCoreRenderingUtils_GetCubemapTexture(ref out_texture_id, ref out_width_height);
			if (out_texture_id != 0 && (_hdrCubemap == null || out_texture_id != _cubemapTextureId))
			{
				_hdrCubemap = Cubemap.CreateExternalTexture(out_width_height, format, mipmap: true, new IntPtr(out_texture_id));
				_cubemapTextureId = out_texture_id;
			}
			long timestamp = GetTimestamp(sessionHandle, lightEstimateHandle);
			if (_cubemapTimestamp != timestamp)
			{
				ExternApi.ARCoreRenderingUtils_SetARCoreLightEstimation(sessionHandle, lightEstimateHandle);
				_cubemapTimestamp = timestamp;
			}
			GL.IssuePluginEvent(ExternApi.ARCoreRenderingUtils_GetRenderEventFunc(), 1);
			return _hdrCubemap;
		}

		public long GetTimestamp(IntPtr sessionHandle, IntPtr lightEstimateHandle)
		{
			long timestamp = -1L;
			ExternApi.ArLightEstimate_getTimestamp(sessionHandle, lightEstimateHandle, ref timestamp);
			return timestamp;
		}
	}
	internal class PlaneApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_getCenterPose(IntPtr sessionHandle, IntPtr planeHandle, IntPtr poseHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_acquireSubsumedBy(IntPtr sessionHandle, IntPtr planeHandle, ref IntPtr subsumerHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_getExtentX(IntPtr sessionHandle, IntPtr planeHandle, ref float extentX);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_getExtentZ(IntPtr sessionHandle, IntPtr planeHandle, ref float extentZ);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_getType(IntPtr sessionHandle, IntPtr planeHandle, ref ApiPlaneType planeType);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_getPolygonSize(IntPtr sessionHandle, IntPtr planeHandle, ref int polygonSize);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_getPolygon(IntPtr sessionHandle, IntPtr planeHandle, IntPtr polygonXZ);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_isPoseInExtents(IntPtr sessionHandle, IntPtr planeHandle, IntPtr poseHandle, ref int isPoseInExtents);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPlane_isPoseInPolygon(IntPtr sessionHandle, IntPtr planeHandle, IntPtr poseHandle, ref int isPoseInPolygon);
		}

		private const int _maxPolygonSize = 1024;

		private NativeSession _nativeSession;

		private float[] _tmpPoints;

		private GCHandle _tmpPointsHandle;

		public PlaneApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
			_tmpPoints = new float[2048];
			_tmpPointsHandle = GCHandle.Alloc(_tmpPoints, GCHandleType.Pinned);
		}

		~PlaneApi()
		{
			_tmpPointsHandle.Free();
		}

		public Pose GetCenterPose(IntPtr planeHandle)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArPlane_getCenterPose(_nativeSession.SessionHandle, planeHandle, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public float GetExtentX(IntPtr planeHandle)
		{
			float extentX = 0f;
			ExternApi.ArPlane_getExtentX(_nativeSession.SessionHandle, planeHandle, ref extentX);
			return extentX;
		}

		public float GetExtentZ(IntPtr planeHandle)
		{
			float extentZ = 0f;
			ExternApi.ArPlane_getExtentZ(_nativeSession.SessionHandle, planeHandle, ref extentZ);
			return extentZ;
		}

		public void GetPolygon(IntPtr planeHandle, List<Vector3> points)
		{
			points.Clear();
			int polygonSize = 0;
			ExternApi.ArPlane_getPolygonSize(_nativeSession.SessionHandle, planeHandle, ref polygonSize);
			if (polygonSize >= 1)
			{
				if (polygonSize > 1024)
				{
					UnityEngine.Debug.LogError("GetPolygon::Plane polygon size exceeds buffer capacity.");
					polygonSize = 1024;
				}
				ExternApi.ArPlane_getPolygon(_nativeSession.SessionHandle, planeHandle, _tmpPointsHandle.AddrOfPinnedObject());
				Pose centerPose = GetCenterPose(planeHandle);
				Matrix4x4 matrix4x = Matrix4x4.TRS(centerPose.position, centerPose.rotation, Vector3.one);
				for (int num = polygonSize - 2; num >= 0; num -= 2)
				{
					Vector3 item = matrix4x.MultiplyPoint3x4(new Vector3(_tmpPoints[num], 0f, 0f - _tmpPoints[num + 1]));
					points.Add(item);
				}
			}
		}

		public DetectedPlane GetSubsumedBy(IntPtr planeHandle)
		{
			IntPtr subsumerHandle = IntPtr.Zero;
			ExternApi.ArPlane_acquireSubsumedBy(_nativeSession.SessionHandle, planeHandle, ref subsumerHandle);
			return _nativeSession.TrackableFactory(subsumerHandle) as DetectedPlane;
		}

		public DetectedPlaneType GetPlaneType(IntPtr planeHandle)
		{
			ApiPlaneType planeType = ApiPlaneType.HorizontalDownwardFacing;
			ExternApi.ArPlane_getType(_nativeSession.SessionHandle, planeHandle, ref planeType);
			return planeType.ToDetectedPlaneType();
		}

		public bool IsPoseInExtents(IntPtr planeHandle, Pose pose)
		{
			int isPoseInExtents = 0;
			IntPtr intPtr = _nativeSession.PoseApi.Create(pose);
			ExternApi.ArPlane_isPoseInExtents(_nativeSession.SessionHandle, planeHandle, intPtr, ref isPoseInExtents);
			_nativeSession.PoseApi.Destroy(intPtr);
			return isPoseInExtents != 0;
		}

		public bool IsPoseInExtents(IntPtr planeHandle, IntPtr poseHandle)
		{
			int isPoseInExtents = 0;
			ExternApi.ArPlane_isPoseInExtents(_nativeSession.SessionHandle, planeHandle, poseHandle, ref isPoseInExtents);
			return isPoseInExtents != 0;
		}

		public bool IsPoseInPolygon(IntPtr planeHandle, Pose pose)
		{
			int isPoseInPolygon = 0;
			IntPtr intPtr = _nativeSession.PoseApi.Create(pose);
			ExternApi.ArPlane_isPoseInPolygon(_nativeSession.SessionHandle, planeHandle, intPtr, ref isPoseInPolygon);
			_nativeSession.PoseApi.Destroy(intPtr);
			return isPoseInPolygon != 0;
		}

		public bool IsPoseInPolygon(IntPtr planeHandle, IntPtr poseHandle)
		{
			int isPoseInPolygon = 0;
			ExternApi.ArPlane_isPoseInPolygon(_nativeSession.SessionHandle, planeHandle, poseHandle, ref isPoseInPolygon);
			return isPoseInPolygon != 0;
		}
	}
	internal class PointApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArPoint_getPose(IntPtr session, IntPtr point, IntPtr out_pose);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPoint_getOrientationMode(IntPtr session, IntPtr point, ref ApiFeaturePointOrientationMode orientationMode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArInstantPlacementPoint_getPose(IntPtr session, IntPtr instantPlacementPoint, IntPtr out_pose);

			[DllImport("arcore_sdk_c")]
			public static extern void ArInstantPlacementPoint_getTrackingMethod(IntPtr session, IntPtr instantPlacementPoint, ref InstantPlacementPointTrackingMethod trackingMethod);
		}

		private NativeSession _nativeSession;

		public PointApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public Pose GetPose(IntPtr pointHandle)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArPoint_getPose(_nativeSession.SessionHandle, pointHandle, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public Pose GetInstantPlacementPointPose(IntPtr instantPlacementPointHandle)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create();
			ExternApi.ArInstantPlacementPoint_getPose(_nativeSession.SessionHandle, instantPlacementPointHandle, intPtr);
			Pose result = _nativeSession.PoseApi.ExtractPoseValue(intPtr);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public InstantPlacementPointTrackingMethod GetInstantPlacementPointTrackingMethod(IntPtr instantPlacementPointHandle)
		{
			InstantPlacementPointTrackingMethod trackingMethod = InstantPlacementPointTrackingMethod.NotTracking;
			ExternApi.ArInstantPlacementPoint_getTrackingMethod(_nativeSession.SessionHandle, instantPlacementPointHandle, ref trackingMethod);
			return trackingMethod;
		}

		public FeaturePointOrientationMode GetOrientationMode(IntPtr pointHandle)
		{
			ApiFeaturePointOrientationMode orientationMode = ApiFeaturePointOrientationMode.Identity;
			ExternApi.ArPoint_getOrientationMode(_nativeSession.SessionHandle, pointHandle, ref orientationMode);
			return orientationMode.ToFeaturePointOrientationMode();
		}
	}
	internal class PointCloudApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArPointCloud_getTimestamp(IntPtr session, IntPtr pointCloudHandle, ref long timestamp);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPointCloud_getNumberOfPoints(IntPtr session, IntPtr pointCloudHandle, ref int pointCount);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPointCloud_getData(IntPtr session, IntPtr pointCloudHandle, ref IntPtr pointCloudData);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPointCloud_getPointIds(IntPtr session, IntPtr pointCloudHandle, ref IntPtr pointIds);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPointCloud_release(IntPtr pointCloudHandle);
		}

		private NativeSession _nativeSession;

		private float[] _cachedVector = new float[4];

		public PointCloudApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public long GetTimestamp(IntPtr pointCloudHandle)
		{
			long timestamp = 0L;
			ExternApi.ArPointCloud_getTimestamp(_nativeSession.SessionHandle, pointCloudHandle, ref timestamp);
			return timestamp;
		}

		public int GetNumberOfPoints(IntPtr pointCloudHandle)
		{
			int pointCount = 0;
			ExternApi.ArPointCloud_getNumberOfPoints(_nativeSession.SessionHandle, pointCloudHandle, ref pointCount);
			return pointCount;
		}

		public PointCloudPoint GetPoint(IntPtr pointCloudHandle, int index)
		{
			IntPtr pointCloudData = IntPtr.Zero;
			ExternApi.ArPointCloud_getData(_nativeSession.SessionHandle, pointCloudHandle, ref pointCloudData);
			Marshal.Copy(new IntPtr(pointCloudData.ToInt64() + Marshal.SizeOf(typeof(Vector4)) * index), _cachedVector, 0, 4);
			Vector3 position = new Vector3(_cachedVector[0], _cachedVector[1], 0f - _cachedVector[2]);
			float confidence = _cachedVector[3];
			return new PointCloudPoint(GetPointId(pointCloudHandle, index), position, confidence);
		}

		public void Release(IntPtr pointCloudHandle)
		{
			ExternApi.ArPointCloud_release(pointCloudHandle);
		}

		private int GetPointId(IntPtr pointCloudHandle, int index)
		{
			IntPtr pointIds = IntPtr.Zero;
			ExternApi.ArPointCloud_getPointIds(_nativeSession.SessionHandle, pointCloudHandle, ref pointIds);
			return Marshal.ReadInt32(new IntPtr(pointIds.ToInt64() + Marshal.SizeOf(typeof(int)) * index));
		}
	}
	internal class PoseApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArPose_create(IntPtr session, ref ApiPoseData rawPose, ref IntPtr poseHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPose_destroy(IntPtr poseHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArPose_getPoseRaw(IntPtr sessionHandle, IntPtr poseHandle, ref ApiPoseData rawPose);
		}

		private NativeSession _nativeSession;

		public PoseApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create()
		{
			return Create(Pose.identity);
		}

		public IntPtr Create(Pose pose)
		{
			ApiPoseData rawPose = new ApiPoseData(pose);
			IntPtr poseHandle = IntPtr.Zero;
			ExternApi.ArPose_create(_nativeSession.SessionHandle, ref rawPose, ref poseHandle);
			return poseHandle;
		}

		public void Destroy(IntPtr nativePose)
		{
			ExternApi.ArPose_destroy(nativePose);
		}

		public Pose ExtractPoseValue(IntPtr poseHandle)
		{
			ApiPoseData rawPose = new ApiPoseData(Pose.identity);
			ExternApi.ArPose_getPoseRaw(_nativeSession.SessionHandle, poseHandle, ref rawPose);
			return rawPose.ToUnityPose();
		}
	}
	internal class RecordingConfigApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArRecordingConfig_create(IntPtr session, ref IntPtr configHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArRecordingConfig_destroy(IntPtr configHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArRecordingConfig_setMp4DatasetFilePath(IntPtr session, IntPtr configHandle, string datasetPath);

			[DllImport("arcore_sdk_c")]
			public static extern void ArRecordingConfig_setAutoStopOnPause(IntPtr session, IntPtr configHandle, int configEnabled);

			[DllImport("arcore_sdk_c")]
			public static extern void ArRecordingConfig_addTrack(IntPtr session, IntPtr configHandle, IntPtr trackHandle);
		}

		private NativeSession _nativeSession;

		public RecordingConfigApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create(ARCoreRecordingConfig config)
		{
			IntPtr configHandle = IntPtr.Zero;
			ExternApi.ArRecordingConfig_create(_nativeSession.SessionHandle, ref configHandle);
			if (config != null)
			{
				ExternApi.ArRecordingConfig_setMp4DatasetFilePath(_nativeSession.SessionHandle, configHandle, config.Mp4DatasetFilepath);
				ExternApi.ArRecordingConfig_setAutoStopOnPause(_nativeSession.SessionHandle, configHandle, config.AutoStopOnPause ? 1 : 0);
				foreach (Track track in config.Tracks)
				{
					IntPtr trackHandle = _nativeSession.TrackApi.Create(track);
					ExternApi.ArRecordingConfig_addTrack(_nativeSession.SessionHandle, configHandle, trackHandle);
					_nativeSession.TrackApi.Destroy(trackHandle);
				}
			}
			return configHandle;
		}

		public void Destory(IntPtr recordingConfigHandle)
		{
			ExternApi.ArRecordingConfig_destroy(recordingConfigHandle);
		}
	}
	internal class SessionApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern int ArSession_configure(IntPtr sessionHandle, IntPtr config);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_getSupportedCameraConfigsWithFilter(IntPtr sessionHandle, IntPtr cameraConfigFilterHandle, IntPtr cameraConfigListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_setCameraConfig(IntPtr sessionHandle, IntPtr cameraConfigHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_getCameraConfig(IntPtr sessionHandle, IntPtr cameraConfigHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_getAllTrackables(IntPtr sessionHandle, ApiTrackableType filterType, IntPtr trackableList);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_setDisplayGeometry(IntPtr sessionHandle, int rotation, int width, int height);

			[DllImport("arcore_sdk_c")]
			public static extern int ArSession_acquireNewAnchor(IntPtr sessionHandle, IntPtr poseHandle, ref IntPtr anchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_isDepthModeSupported(IntPtr sessionHandle, ApiDepthMode depthMode, ref int isSupported);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_reportEngineType(IntPtr sessionHandle, string engineType, string engineVersion);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_hostAndAcquireNewCloudAnchor(IntPtr sessionHandle, IntPtr anchorHandle, ref IntPtr cloudAnchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_resolveAndAcquireNewCloudAnchor(IntPtr sessionHandle, string cloudAnchorId, ref IntPtr cloudAnchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_hostAndAcquireNewCloudAnchorWithTtl(IntPtr sessionHandle, IntPtr anchorHandle, int ttlDays, ref IntPtr cloudAnchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_setAuthToken(IntPtr sessionHandle, string authToken);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_estimateFeatureMapQualityForHosting(IntPtr sessionHandle, IntPtr poseHandle, ref int featureMapQuality);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_getRecordingStatus(IntPtr sessionHandle, ref ApiRecordingStatus recordingStatus);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_startRecording(IntPtr sessionHandle, IntPtr recordingConfigHandle);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_stopRecording(IntPtr sessionHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArSession_getPlaybackStatus(IntPtr sessionHandle, ref ApiPlaybackStatus playbackStatus);

			[DllImport("arcore_sdk_c")]
			public static extern ApiArStatus ArSession_setPlaybackDataset(IntPtr sessionHandle, string mp4DatasetFilePath);
		}

		private NativeSession _nativeSession;

		public SessionApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public void ReportEngineType()
		{
			ExternApi.ArSession_reportEngineType(_nativeSession.SessionHandle, "Unity", Application.unityVersion);
		}

		public void GetSupportedCameraConfigurationsWithFilter(ARCoreCameraConfigFilter cameraConfigFilter, IntPtr cameraConfigListHandle, List<IntPtr> supportedCameraConfigHandles, List<CameraConfig> supportedCameraConfigs, DeviceCameraDirection cameraFacingDirection)
		{
			IntPtr intPtr = _nativeSession.CameraConfigFilterApi.Create(cameraFacingDirection, cameraConfigFilter);
			ExternApi.ArSession_getSupportedCameraConfigsWithFilter(_nativeSession.SessionHandle, intPtr, cameraConfigListHandle);
			_nativeSession.CameraConfigFilterApi.Destroy(intPtr);
			supportedCameraConfigHandles.Clear();
			supportedCameraConfigs.Clear();
			int size = _nativeSession.CameraConfigListApi.GetSize(cameraConfigListHandle);
			for (int i = 0; i < size; i++)
			{
				IntPtr intPtr2 = _nativeSession.CameraConfigApi.Create();
				_nativeSession.CameraConfigListApi.GetItemAt(cameraConfigListHandle, i, intPtr2);
				supportedCameraConfigHandles.Add(intPtr2);
				supportedCameraConfigs.Add(CreateCameraConfig(intPtr2));
			}
		}

		public ApiArStatus SetCameraConfig(IntPtr cameraConfigHandle)
		{
			return ExternApi.ArSession_setCameraConfig(_nativeSession.SessionHandle, cameraConfigHandle);
		}

		public CameraConfig GetCameraConfig()
		{
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("access camera config");
				return default(CameraConfig);
			}
			IntPtr cameraConfigHandle = _nativeSession.CameraConfigApi.Create();
			ExternApi.ArSession_getCameraConfig(_nativeSession.SessionHandle, cameraConfigHandle);
			CameraConfig result = CreateCameraConfig(cameraConfigHandle);
			_nativeSession.CameraConfigApi.Destroy(cameraConfigHandle);
			return result;
		}

		public void GetAllTrackables(List<Trackable> trackables)
		{
			IntPtr intPtr = _nativeSession.TrackableListApi.Create();
			ExternApi.ArSession_getAllTrackables(_nativeSession.SessionHandle, ApiTrackableType.BaseTrackable, intPtr);
			trackables.Clear();
			int count = _nativeSession.TrackableListApi.GetCount(intPtr);
			for (int i = 0; i < count; i++)
			{
				IntPtr intPtr2 = _nativeSession.TrackableListApi.AcquireItem(intPtr, i);
				Trackable trackable = _nativeSession.TrackableFactory(intPtr2);
				if (trackable != null)
				{
					trackables.Add(trackable);
				}
				else
				{
					_nativeSession.TrackableApi.Release(intPtr2);
				}
			}
			_nativeSession.TrackableListApi.Destroy(intPtr);
		}

		public void SetDisplayGeometry(ScreenOrientation orientation, int width, int height)
		{
			int rotation = 0;
			switch (orientation)
			{
			case ScreenOrientation.LandscapeLeft:
				rotation = 1;
				break;
			case ScreenOrientation.LandscapeRight:
				rotation = 3;
				break;
			case ScreenOrientation.Portrait:
				rotation = 0;
				break;
			case ScreenOrientation.PortraitUpsideDown:
				rotation = 2;
				break;
			}
			ExternApi.ArSession_setDisplayGeometry(_nativeSession.SessionHandle, rotation, width, height);
		}

		public Anchor CreateAnchor(Pose pose)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create(pose);
			IntPtr anchorHandle = IntPtr.Zero;
			ExternApi.ArSession_acquireNewAnchor(_nativeSession.SessionHandle, intPtr, ref anchorHandle);
			Anchor result = Anchor.Factory(_nativeSession, anchorHandle);
			_nativeSession.PoseApi.Destroy(intPtr);
			return result;
		}

		public ApiArStatus CreateCloudAnchor(IntPtr platformAnchorHandle, out IntPtr cloudAnchorHandle)
		{
			cloudAnchorHandle = IntPtr.Zero;
			return ExternApi.ArSession_hostAndAcquireNewCloudAnchor(_nativeSession.SessionHandle, platformAnchorHandle, ref cloudAnchorHandle);
		}

		public ApiArStatus ResolveCloudAnchor(string cloudAnchorId, out IntPtr cloudAnchorHandle)
		{
			cloudAnchorHandle = IntPtr.Zero;
			return ExternApi.ArSession_resolveAndAcquireNewCloudAnchor(_nativeSession.SessionHandle, cloudAnchorId, ref cloudAnchorHandle);
		}

		public bool IsDepthModeSupported(ApiDepthMode depthMode)
		{
			int isSupported = 0;
			ExternApi.ArSession_isDepthModeSupported(_nativeSession.SessionHandle, depthMode, ref isSupported);
			return isSupported != 0;
		}

		public ApiArStatus HostCloudAnchor(IntPtr platformAnchorHandle, int ttlDays, out IntPtr cloudAnchorHandle)
		{
			cloudAnchorHandle = IntPtr.Zero;
			return ExternApi.ArSession_hostAndAcquireNewCloudAnchorWithTtl(_nativeSession.SessionHandle, platformAnchorHandle, ttlDays, ref cloudAnchorHandle);
		}

		public void SetAuthToken(string authToken)
		{
			ExternApi.ArSession_setAuthToken(_nativeSession.SessionHandle, authToken);
		}

		public FeatureMapQuality EstimateFeatureMapQualityForHosting(Pose pose)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create(pose);
			int featureMapQuality = 0;
			ApiArStatus apiArStatus = ExternApi.ArSession_estimateFeatureMapQualityForHosting(_nativeSession.SessionHandle, intPtr, ref featureMapQuality);
			_nativeSession.PoseApi.Destroy(intPtr);
			if (apiArStatus != ApiArStatus.Success)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to estimate feature map quality with status {0}.", apiArStatus);
			}
			return (FeatureMapQuality)featureMapQuality;
		}

		public PlaybackStatus GetPlaybackStatus()
		{
			ApiPlaybackStatus playbackStatus = ApiPlaybackStatus.None;
			ExternApi.ArSession_getPlaybackStatus(_nativeSession.SessionHandle, ref playbackStatus);
			return playbackStatus.ToPlaybackStatus();
		}

		public PlaybackResult SetPlaybackDataset(string datasetFilepath)
		{
			ApiArStatus apiArStatus = ExternApi.ArSession_setPlaybackDataset(_nativeSession.SessionHandle, datasetFilepath);
			switch (apiArStatus)
			{
			case ApiArStatus.Success:
				return PlaybackResult.OK;
			case ApiArStatus.ErrorSessionNotPaused:
				return PlaybackResult.ErrorSessionNotPaused;
			case ApiArStatus.ErrorSessionUnsupported:
				return PlaybackResult.ErrorSessionUnsupported;
			case ApiArStatus.ErrorPlaybackFailed:
				return PlaybackResult.ErrorPlaybackFailed;
			default:
				UnityEngine.Debug.LogErrorFormat("Attempt to set a playback dataset path failed with unexpected status: {0}", apiArStatus);
				return PlaybackResult.ErrorPlaybackFailed;
			}
		}

		public RecordingStatus GetRecordingStatus()
		{
			ApiRecordingStatus recordingStatus = ApiRecordingStatus.None;
			ExternApi.ArSession_getRecordingStatus(_nativeSession.SessionHandle, ref recordingStatus);
			return recordingStatus.ToRecordingStatus();
		}

		public RecordingResult StartRecording(ARCoreRecordingConfig config)
		{
			IntPtr recordingConfigHandle = _nativeSession.RecordingConfigApi.Create(config);
			ApiArStatus apiArStatus = ExternApi.ArSession_startRecording(_nativeSession.SessionHandle, recordingConfigHandle);
			_nativeSession.RecordingConfigApi.Destory(recordingConfigHandle);
			switch (apiArStatus)
			{
			case ApiArStatus.Success:
				return RecordingResult.OK;
			case ApiArStatus.ErrorIllegalState:
				return RecordingResult.ErrorIllegalState;
			case ApiArStatus.ErrorInvalidArgument:
				return RecordingResult.ErrorInvalidArgument;
			case ApiArStatus.ErrorRecordingFailed:
				return RecordingResult.ErrorRecordingFailed;
			default:
				UnityEngine.Debug.LogErrorFormat("Attempt to start a recording failed with unexpected status: {0}", apiArStatus);
				return RecordingResult.ErrorRecordingFailed;
			}
		}

		public RecordingResult StopRecording()
		{
			ApiArStatus apiArStatus = ExternApi.ArSession_stopRecording(_nativeSession.SessionHandle);
			switch (apiArStatus)
			{
			case ApiArStatus.Success:
				return RecordingResult.OK;
			case ApiArStatus.ErrorRecordingFailed:
				return RecordingResult.ErrorRecordingFailed;
			default:
				UnityEngine.Debug.LogErrorFormat("Attempt to stop recording failed with unexpected status: {0}", apiArStatus);
				return RecordingResult.ErrorRecordingFailed;
			}
		}

		private CameraConfig CreateCameraConfig(IntPtr cameraConfigHandle)
		{
			DeviceCameraDirection facingDirection = _nativeSession.CameraConfigApi.GetFacingDirection(cameraConfigHandle);
			int width = 0;
			int height = 0;
			int width2 = 0;
			int height2 = 0;
			int minFps = 0;
			int maxFps = 0;
			CameraConfigDepthSensorUsage depthSensorUsage = _nativeSession.CameraConfigApi.GetDepthSensorUsage(cameraConfigHandle);
			CameraConfigStereoCameraUsage stereoCameraUsage = _nativeSession.CameraConfigApi.GetStereoCameraUsage(cameraConfigHandle);
			_nativeSession.CameraConfigApi.GetImageDimensions(cameraConfigHandle, out width, out height);
			_nativeSession.CameraConfigApi.GetTextureDimensions(cameraConfigHandle, out width2, out height2);
			_nativeSession.CameraConfigApi.GetFpsRange(cameraConfigHandle, out minFps, out maxFps);
			return new CameraConfig(facingDirection, new Vector2(width, height), new Vector2(width2, height2), minFps, maxFps, stereoCameraUsage, depthSensorUsage);
		}
	}
	internal class SessionConfigApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_create(IntPtr session, ref IntPtr out_config);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_destroy(IntPtr config);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setLightEstimationMode(IntPtr session, IntPtr config, ApiLightEstimationMode light_estimation_mode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setPlaneFindingMode(IntPtr session, IntPtr config, ApiPlaneFindingMode plane_finding_mode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setUpdateMode(IntPtr session, IntPtr config, ApiUpdateMode update_mode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setCloudAnchorMode(IntPtr session, IntPtr config, ApiCloudAnchorMode cloud_anchor_mode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setAugmentedImageDatabase(IntPtr session, IntPtr config, IntPtr augmented_image_database);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setAugmentedFaceMode(IntPtr session, IntPtr config, ApiAugmentedFaceMode augmented_face_mode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setFocusMode(IntPtr session, IntPtr config, ApiCameraFocusMode focus_mode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setDepthMode(IntPtr session, IntPtr config, ApiDepthMode mode);

			[DllImport("arcore_sdk_c")]
			public static extern void ArConfig_setInstantPlacementMode(IntPtr session, IntPtr config, InstantPlacementMode instant_placement_mode);
		}

		private NativeSession _nativeSession;

		public SessionConfigApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public static void UpdateApiConfigWithARCoreSessionConfig(IntPtr sessionHandle, IntPtr configHandle, ARCoreSessionConfig sessionConfig)
		{
			ApiLightEstimationMode light_estimation_mode = sessionConfig.LightEstimationMode.ToApiLightEstimationMode();
			ExternApi.ArConfig_setLightEstimationMode(sessionHandle, configHandle, light_estimation_mode);
			ApiPlaneFindingMode plane_finding_mode = sessionConfig.PlaneFindingMode.ToApiPlaneFindingMode();
			ExternApi.ArConfig_setPlaneFindingMode(sessionHandle, configHandle, plane_finding_mode);
			ApiUpdateMode update_mode = ((!sessionConfig.MatchCameraFramerate) ? ApiUpdateMode.LatestCameraImage : ApiUpdateMode.Blocking);
			ExternApi.ArConfig_setUpdateMode(sessionHandle, configHandle, update_mode);
			ApiCloudAnchorMode cloud_anchor_mode = sessionConfig.CloudAnchorMode.ToApiCloudAnchorMode();
			ExternApi.ArConfig_setCloudAnchorMode(sessionHandle, configHandle, cloud_anchor_mode);
			IntPtr zero = IntPtr.Zero;
			if (sessionConfig.AugmentedImageDatabase != null)
			{
				zero = sessionConfig.AugmentedImageDatabase._nativeHandle;
				ExternApi.ArConfig_setAugmentedImageDatabase(sessionHandle, configHandle, zero);
			}
			else
			{
				ExternApi.ArConfig_setAugmentedImageDatabase(sessionHandle, configHandle, IntPtr.Zero);
			}
			ApiAugmentedFaceMode augmented_face_mode = sessionConfig.AugmentedFaceMode.ToApiAugmentedFaceMode();
			ExternApi.ArConfig_setAugmentedFaceMode(sessionHandle, configHandle, augmented_face_mode);
			ApiCameraFocusMode focus_mode = sessionConfig.CameraFocusMode.ToApiCameraFocusMode();
			ExternApi.ArConfig_setFocusMode(sessionHandle, configHandle, focus_mode);
			if (!InstantPreviewManager.IsProvidingPlatform)
			{
				ApiDepthMode mode = sessionConfig.DepthMode.ToApiDepthMode();
				ExternApi.ArConfig_setDepthMode(sessionHandle, configHandle, mode);
			}
			if (!InstantPreviewManager.IsProvidingPlatform)
			{
				ExternApi.ArConfig_setInstantPlacementMode(sessionHandle, configHandle, sessionConfig.InstantPlacementMode);
			}
		}

		public IntPtr Create()
		{
			IntPtr out_config = IntPtr.Zero;
			ExternApi.ArConfig_create(_nativeSession.SessionHandle, ref out_config);
			return out_config;
		}

		public void Destroy(IntPtr configHandle)
		{
			ExternApi.ArConfig_destroy(configHandle);
		}
	}
	internal class TrackableApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackable_getType(IntPtr sessionHandle, IntPtr trackableHandle, ref ApiTrackableType trackableType);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackable_getTrackingState(IntPtr sessionHandle, IntPtr trackableHandle, ref ApiTrackingState trackingState);

			[DllImport("arcore_sdk_c")]
			public static extern int ArTrackable_acquireNewAnchor(IntPtr sessionHandle, IntPtr trackableHandle, IntPtr poseHandle, ref IntPtr anchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackable_release(IntPtr trackableHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackable_getAnchors(IntPtr sessionHandle, IntPtr trackableHandle, IntPtr outputListHandle);
		}

		private NativeSession _nativeSession;

		public TrackableApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public ApiTrackableType GetType(IntPtr trackableHandle)
		{
			ApiTrackableType trackableType = ApiTrackableType.Plane;
			ExternApi.ArTrackable_getType(_nativeSession.SessionHandle, trackableHandle, ref trackableType);
			return trackableType;
		}

		public TrackingState GetTrackingState(IntPtr trackableHandle)
		{
			ApiTrackingState trackingState = ApiTrackingState.Stopped;
			ExternApi.ArTrackable_getTrackingState(_nativeSession.SessionHandle, trackableHandle, ref trackingState);
			return trackingState.ToTrackingState();
		}

		public bool AcquireNewAnchor(IntPtr trackableHandle, Pose pose, out IntPtr anchorHandle)
		{
			IntPtr intPtr = _nativeSession.PoseApi.Create(pose);
			anchorHandle = IntPtr.Zero;
			int num = ExternApi.ArTrackable_acquireNewAnchor(_nativeSession.SessionHandle, trackableHandle, intPtr, ref anchorHandle);
			_nativeSession.PoseApi.Destroy(intPtr);
			return num == 0;
		}

		public void Release(IntPtr trackableHandle)
		{
			ExternApi.ArTrackable_release(trackableHandle);
		}

		public void GetAnchors(IntPtr trackableHandle, List<Anchor> anchors)
		{
			IntPtr intPtr = _nativeSession.AnchorApi.CreateList();
			ExternApi.ArTrackable_getAnchors(_nativeSession.SessionHandle, trackableHandle, intPtr);
			anchors.Clear();
			int listSize = _nativeSession.AnchorApi.GetListSize(intPtr);
			for (int i = 0; i < listSize; i++)
			{
				IntPtr intPtr2 = _nativeSession.AnchorApi.AcquireListItem(intPtr, i);
				Anchor anchor = Anchor.Factory(_nativeSession, intPtr2, isCreate: false);
				if (anchor == null)
				{
					UnityEngine.Debug.LogFormat("Unable to find Anchor component for handle {0}", intPtr2);
				}
				else
				{
					anchors.Add(anchor);
				}
			}
			_nativeSession.AnchorApi.DestroyList(intPtr);
		}
	}
	internal class TrackableListApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackableList_create(IntPtr sessionHandle, ref IntPtr trackableListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackableList_destroy(IntPtr trackableListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackableList_getSize(IntPtr sessionHandle, IntPtr trackableListHandle, ref int outSize);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackableList_acquireItem(IntPtr sessionHandle, IntPtr trackableListHandle, int index, ref IntPtr outTrackable);
		}

		private NativeSession _nativeSession;

		public TrackableListApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create()
		{
			IntPtr trackableListHandle = IntPtr.Zero;
			ExternApi.ArTrackableList_create(_nativeSession.SessionHandle, ref trackableListHandle);
			return trackableListHandle;
		}

		public void Destroy(IntPtr listHandle)
		{
			ExternApi.ArTrackableList_destroy(listHandle);
		}

		public int GetCount(IntPtr listHandle)
		{
			int outSize = 0;
			ExternApi.ArTrackableList_getSize(_nativeSession.SessionHandle, listHandle, ref outSize);
			return outSize;
		}

		public IntPtr AcquireItem(IntPtr listHandle, int index)
		{
			IntPtr outTrackable = IntPtr.Zero;
			ExternApi.ArTrackableList_acquireItem(_nativeSession.SessionHandle, listHandle, index, ref outTrackable);
			return outTrackable;
		}
	}
	internal class TrackApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArTrack_create(IntPtr session, ref IntPtr trackHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrack_destroy(IntPtr trackHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrack_setId(IntPtr session, IntPtr trackHandle, IntPtr trackIdBytes);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrack_setMetadata(IntPtr session, IntPtr trackHandle, IntPtr metadataBytes, int metadataBufferSize);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrack_setMimeType(IntPtr session, IntPtr trackHandle, string mimeType);
		}

		private NativeSession _nativeSession;

		public TrackApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create(Track track)
		{
			IntPtr trackHandle = IntPtr.Zero;
			ExternApi.ArTrack_create(_nativeSession.SessionHandle, ref trackHandle);
			GCHandle gCHandle = GCHandle.Alloc(track.Id.ToByteArray(), GCHandleType.Pinned);
			ExternApi.ArTrack_setId(_nativeSession.SessionHandle, trackHandle, gCHandle.AddrOfPinnedObject());
			if (gCHandle.IsAllocated)
			{
				gCHandle.Free();
			}
			GCHandle gCHandle2 = GCHandle.Alloc(track.Metadata, GCHandleType.Pinned);
			ExternApi.ArTrack_setMetadata(_nativeSession.SessionHandle, trackHandle, gCHandle2.AddrOfPinnedObject(), track.Metadata.Length);
			if (gCHandle2.IsAllocated)
			{
				gCHandle2.Free();
			}
			ExternApi.ArTrack_setMimeType(_nativeSession.SessionHandle, trackHandle, track.MimeType);
			return trackHandle;
		}

		public void Destroy(IntPtr trackHandle)
		{
			ExternApi.ArTrack_destroy(trackHandle);
		}
	}
	internal class TrackDataApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackData_getFrameTimestamp(IntPtr sessionHandle, IntPtr trackDataHandle, ref long timestamp);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackData_getData(IntPtr sessionHandle, IntPtr trackDataHandle, ref IntPtr dataBytesHandle, ref int size);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackData_release(IntPtr trackDataHandle);
		}

		private NativeSession _nativeSession;

		public TrackDataApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public long GetFrameTimestamp(IntPtr trackDataHandle)
		{
			long timestamp = 0L;
			ExternApi.ArTrackData_getFrameTimestamp(_nativeSession.SessionHandle, trackDataHandle, ref timestamp);
			return timestamp;
		}

		public byte[] GetData(IntPtr trackDataHandle)
		{
			IntPtr dataBytesHandle = IntPtr.Zero;
			int size = 0;
			ExternApi.ArTrackData_getData(_nativeSession.SessionHandle, trackDataHandle, ref dataBytesHandle, ref size);
			byte[] array = new byte[size];
			if (size > 0)
			{
				Marshal.Copy(dataBytesHandle, array, 0, size);
			}
			return array;
		}

		public void Release(IntPtr trackDataHandle)
		{
			ExternApi.ArTrackData_release(trackDataHandle);
		}
	}
	internal class TrackDataListApi
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackDataList_create(IntPtr sessionHandle, ref IntPtr trackDataListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackDataList_destroy(IntPtr trackDataListHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackDataList_getSize(IntPtr sessionHandle, IntPtr trackDataListHandle, ref int outSize);

			[DllImport("arcore_sdk_c")]
			public static extern void ArTrackDataList_acquireItem(IntPtr sessionHandle, IntPtr trackDataListHandle, int index, ref IntPtr outTrackData);
		}

		private NativeSession _nativeSession;

		public TrackDataListApi(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
		}

		public IntPtr Create()
		{
			IntPtr trackDataListHandle = IntPtr.Zero;
			ExternApi.ArTrackDataList_create(_nativeSession.SessionHandle, ref trackDataListHandle);
			return trackDataListHandle;
		}

		public void Destroy(IntPtr listHandle)
		{
			ExternApi.ArTrackDataList_destroy(listHandle);
		}

		public int GetCount(IntPtr listHandle)
		{
			int outSize = 0;
			ExternApi.ArTrackDataList_getSize(_nativeSession.SessionHandle, listHandle, ref outSize);
			return outSize;
		}

		public IntPtr AcquireItem(IntPtr listHandle, int index)
		{
			IntPtr outTrackData = IntPtr.Zero;
			ExternApi.ArTrackDataList_acquireItem(_nativeSession.SessionHandle, listHandle, index, ref outTrackData);
			return outTrackData;
		}
	}
	public class WaitForTaskCompletionYieldInstruction<T> : CustomYieldInstruction
	{
		private AsyncTask<T> _task;

		public override bool keepWaiting => !_task.IsComplete;

		public WaitForTaskCompletionYieldInstruction(AsyncTask<T> task)
		{
			_task = task;
		}
	}
	internal abstract class ExperimentBase
	{
		public virtual int GetExperimentalFeatureFlags()
		{
			return 0;
		}

		public virtual void OnUpdateSessionFeatures()
		{
		}

		public virtual void OnEarlyUpdate()
		{
		}

		public virtual bool IsConfigurationDirty()
		{
			return false;
		}

		public virtual void OnSetConfiguration(IntPtr sessionHandle, IntPtr configHandle)
		{
		}

		public virtual bool IsManagingTrackableType(int trackableType)
		{
			return false;
		}

		public virtual TrackableHitFlags GetTrackableHitFlags(int trackableType)
		{
			return TrackableHitFlags.None;
		}

		public virtual Trackable TrackableFactory(int trackableType, IntPtr trackableHandle)
		{
			return null;
		}
	}
	internal class ExperimentManager
	{
		private static ExperimentManager _instance;

		private List<ExperimentBase> _experiments;

		public static ExperimentManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ExperimentManager();
				}
				return _instance;
			}
		}

		public bool IsSessionExperimental { get; private set; }

		public bool IsConfigurationDirty
		{
			get
			{
				bool flag = false;
				foreach (ExperimentBase experiment in _experiments)
				{
					flag = flag || experiment.IsConfigurationDirty();
				}
				return flag;
			}
		}

		public ExperimentManager()
		{
			_experiments = new List<ExperimentBase>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<Type> list = new List<Type>();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				try
				{
					Type[] types = assembly.GetTypes();
					list.AddRange(types);
				}
				catch (ReflectionTypeLoadException ex)
				{
					UnityEngine.Debug.Log("Unable to load types from assembly:: " + assembly.ToString() + ":: " + ex.Message);
				}
			}
			foreach (Type item in list)
			{
				if (item.IsClass && !item.IsAbstract && typeof(ExperimentBase).IsAssignableFrom(item))
				{
					_experiments.Add(Activator.CreateInstance(item) as ExperimentBase);
				}
			}
		}

		public void Initialize()
		{
			LifecycleManager.Instance.EarlyUpdate += _instance.OnEarlyUpdate;
			LifecycleManager.Instance.UpdateSessionFeatures += _instance.OnUpdateSessionFeatures;
			LifecycleManager.Instance.OnSetConfiguration += _instance.SetConfiguration;
		}

		public bool IsManagingTrackableType(int trackableType)
		{
			return GetTrackableTypeManager(trackableType) != null;
		}

		public TrackableHitFlags GetTrackableHitFlags(int trackableType)
		{
			return GetTrackableTypeManager(trackableType)?.GetTrackableHitFlags(trackableType) ?? TrackableHitFlags.None;
		}

		public Trackable TrackableFactory(int trackableType, IntPtr trackableHandle)
		{
			return GetTrackableTypeManager(trackableType)?.TrackableFactory(trackableType, trackableHandle);
		}

		public void OnUpdateSessionFeatures()
		{
			foreach (ExperimentBase experiment in _experiments)
			{
				experiment.OnUpdateSessionFeatures();
			}
		}

		private void OnEarlyUpdate()
		{
			foreach (ExperimentBase experiment in _experiments)
			{
				experiment.OnEarlyUpdate();
			}
		}

		private void SetConfiguration(IntPtr sessionHandle, IntPtr configHandle)
		{
			foreach (ExperimentBase experiment in _experiments)
			{
				experiment.OnSetConfiguration(sessionHandle, configHandle);
			}
		}

		private ExperimentBase GetTrackableTypeManager(int trackableType)
		{
			foreach (ExperimentBase experiment in _experiments)
			{
				if (experiment.IsManagingTrackableType(trackableType))
				{
					return experiment;
				}
			}
			return null;
		}
	}
	internal class ARCoreAndroidLifecycleManager : ILifecycleManager
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_unity_api")]
			public static extern int ArCoreUnity_getBackgroundTextureId();

			[DllImport("arpresto_api")]
			public static extern void ArPresto_setDisplayGeometry(AndroidNativeHelper.AndroidSurfaceRotation rotation, int width, int height);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_getSession(ref IntPtr sessionHandle);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_setDeviceCameraDirection(ApiPrestoDeviceCameraDirection cameraDirection);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_setCameraTextureNames(int numberOfTextures, int[] textureIds);

			[DllImport("arcore_rendering_utils_api")]
			public static extern void ARCoreRenderingUtils_CreatePostUpdateFence();

			[DllImport("arpresto_api")]
			public static extern void ArPresto_setEnabled(bool isEnabled);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_getFrame(ref IntPtr frameHandle);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_getStatus(ref ApiPrestoStatus prestoStatus);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_update();

			[DllImport("arpresto_api")]
			public static extern void ArPresto_setConfigurationDirty();

			[DllImport("arpresto_api")]
			public static extern void ArPresto_reset();
		}

		private const int _mtNumTextureIds = 4;

		private const int _invalidTextureId = -1;

		private const int _nullTextureId = 0;

		private static ARCoreAndroidLifecycleManager _instance;

		private IntPtr _cachedSessionHandle = IntPtr.Zero;

		private IntPtr _cachedFrameHandle = IntPtr.Zero;

		private Dictionary<IntPtr, NativeSession> _nativeSessions = new Dictionary<IntPtr, NativeSession>(new IntPtrEqualityComparer());

		private DeviceCameraDirection? _cachedCameraDirection;

		private ARCoreSessionConfig _cachedConfig;

		private ScreenOrientation? _cachedScreenOrientation;

		private bool? _desiredSessionState;

		private bool _disabledSessionOnErrorState;

		private bool _haveDisableToEnableTransition;

		private AsyncTask<AndroidPermissionsRequestResult> _androidPermissionRequest;

		private HashSet<string> _requiredPermissionNames = new HashSet<string>();

		private AndroidNativeHelper.AndroidSurfaceRotation _cachedDisplayRotation;

		private List<IntPtr> _tempCameraConfigHandles = new List<IntPtr>();

		private List<CameraConfig> _tempCameraConfigs = new List<CameraConfig>();

		private int[] _cameraTextureIds;

		private Dictionary<int, Texture2D> _textureIdToTexture2D = new Dictionary<int, Texture2D>();

		public static ARCoreAndroidLifecycleManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ARCoreAndroidLifecycleManager();
					_instance.Initialize();
					ARPrestoCallbackManager.Instance.EarlyUpdate += _instance.OnEarlyUpdate;
					ARPrestoCallbackManager.Instance.BeforeResumeSession += _instance.OnBeforeResumeSession;
					ARPrestoCallbackManager.Instance.OnSetConfiguration += _instance.SetSessionConfiguration;
					ExperimentManager.Instance.Initialize();
				}
				return _instance;
			}
		}

		public SessionStatus SessionStatus { get; private set; }

		public LostTrackingReason LostTrackingReason { get; private set; }

		public ARCoreSession SessionComponent { get; private set; }

		public NativeSession NativeSession
		{
			get
			{
				if (_cachedSessionHandle == IntPtr.Zero)
				{
					return null;
				}
				return GetNativeSession(_cachedSessionHandle);
			}
		}

		public bool IsSessionChangedThisFrame { get; private set; }

		public Texture2D BackgroundTexture { get; private set; }

		public event Action UpdateSessionFeatures;

		public event Action EarlyUpdate;

		public event Action<bool> OnSessionSetEnabled;

		public event Action<IntPtr, IntPtr> OnSetConfiguration;

		public event Action OnResetInstance;

		public AsyncTask<ApkAvailabilityStatus> CheckApkAvailability()
		{
			return ARPrestoCallbackManager.Instance.CheckApkAvailability();
		}

		public AsyncTask<ApkInstallationStatus> RequestApkInstallation(bool userRequested)
		{
			return ARPrestoCallbackManager.Instance.RequestApkInstallation(userRequested);
		}

		public void CreateSession(ARCoreSession sessionComponent)
		{
			sessionComponent.StartCoroutine(InstantPreviewManager.InitializeIfNeeded());
			if (SessionComponent != null)
			{
				UnityEngine.Debug.LogError("Multiple ARCore session components cannot exist in the scene. Destroying the newest.");
				UnityEngine.Object.Destroy(sessionComponent);
			}
			else
			{
				SessionComponent = sessionComponent;
			}
		}

		public void EnableSession()
		{
			if (_desiredSessionState.HasValue && !_desiredSessionState.Value)
			{
				_haveDisableToEnableTransition = true;
			}
			_desiredSessionState = true;
		}

		public void DisableSession()
		{
			_desiredSessionState = false;
		}

		public void ResetSession()
		{
			FireOnSessionSetEnabled(isEnabled: false);
			Initialize();
			ExternApi.ArPresto_reset();
		}

		internal static void ResetInstance()
		{
			if (_instance != null && _instance.OnResetInstance != null)
			{
				_instance.OnResetInstance();
			}
			_instance = null;
		}

		private ApiPrestoCallbackResult OnBeforeResumeSession(IntPtr sessionHandle)
		{
			ApiPrestoCallbackResult result = ApiPrestoCallbackResult.InvalidCameraConfig;
			if (SessionComponent == null || sessionHandle == IntPtr.Zero)
			{
				return result;
			}
			NativeSession nativeSession = GetNativeSession(sessionHandle);
			IntPtr cameraConfigListHandle = nativeSession.CameraConfigListApi.Create();
			nativeSession.SessionApi.GetSupportedCameraConfigurationsWithFilter(SessionComponent.CameraConfigFilter, cameraConfigListHandle, _tempCameraConfigHandles, _tempCameraConfigs, SessionComponent.DeviceCameraDirection);
			if (_tempCameraConfigHandles.Count == 0)
			{
				UnityEngine.Debug.LogWarning("Unable to choose a custom camera configuration because none are available.");
			}
			else
			{
				int num = 0;
				if (SessionComponent.GetChooseCameraConfigurationCallback() != null)
				{
					num = SessionComponent.GetChooseCameraConfigurationCallback()(_tempCameraConfigs);
				}
				if (num >= 0 && num < _tempCameraConfigHandles.Count)
				{
					ApiArStatus apiArStatus = nativeSession.SessionApi.SetCameraConfig(_tempCameraConfigHandles[num]);
					if (apiArStatus != ApiArStatus.Success)
					{
						UnityEngine.Debug.LogErrorFormat("Failed to set the ARCore camera configuration: {0}", apiArStatus);
					}
					else
					{
						result = ApiPrestoCallbackResult.Success;
						ExternApi.ArPresto_setConfigurationDirty();
					}
				}
				for (int i = 0; i < _tempCameraConfigHandles.Count; i++)
				{
					nativeSession.CameraConfigApi.Destroy(_tempCameraConfigHandles[i]);
				}
			}
			nativeSession.CameraConfigListApi.Destroy(cameraConfigListHandle);
			_tempCameraConfigHandles.Clear();
			_tempCameraConfigs.Clear();
			return result;
		}

		private void OnEarlyUpdate()
		{
			SetCameraTextureNameIfNecessary();
			if (_haveDisableToEnableTransition)
			{
				SetSessionEnabled(sessionEnabled: false);
				SetSessionEnabled(sessionEnabled: true);
				_haveDisableToEnableTransition = false;
				if (_desiredSessionState.HasValue && _desiredSessionState.Value)
				{
					_desiredSessionState = null;
				}
			}
			if (_desiredSessionState.HasValue)
			{
				SetSessionEnabled(_desiredSessionState.Value);
				_desiredSessionState = null;
			}
			if (SessionComponent != null)
			{
				IntPtr sessionHandle = IntPtr.Zero;
				ExternApi.ArPresto_getSession(ref sessionHandle);
				if (this.UpdateSessionFeatures != null)
				{
					this.UpdateSessionFeatures();
				}
				SetCameraDirection(SessionComponent.DeviceCameraDirection);
				IntPtr sessionHandle2 = IntPtr.Zero;
				ExternApi.ArPresto_getSession(ref sessionHandle2);
				if (sessionHandle != sessionHandle2)
				{
					FireOnSessionSetEnabled(isEnabled: false);
					FireOnSessionSetEnabled(isEnabled: true);
				}
				if (InstantPreviewManager.IsProvidingPlatform && SessionComponent.SessionConfig != null && !InstantPreviewManager.ValidateSessionConfig(SessionComponent.SessionConfig))
				{
					SessionComponent.SessionConfig = InstantPreviewManager.GenerateInstantPreviewSupportedConfig(SessionComponent.SessionConfig);
				}
				if (_requiredPermissionNames.Count > 0)
				{
					RequestPermissions();
				}
				UpdateConfiguration(SessionComponent.SessionConfig);
			}
			UpdateDisplayGeometry();
			ExternApi.ArPresto_update();
			if (SystemInfo.graphicsMultiThreaded && !InstantPreviewManager.IsProvidingPlatform)
			{
				ExternApi.ARCoreRenderingUtils_CreatePostUpdateFence();
			}
			SessionStatus sessionStatus = SessionStatus;
			ApiPrestoStatus prestoStatus = ApiPrestoStatus.Uninitialized;
			ExternApi.ArPresto_getStatus(ref prestoStatus);
			SessionStatus = prestoStatus.ToSessionStatus();
			LostTrackingReason = LostTrackingReason.None;
			if (NativeSession != null && SessionStatus == SessionStatus.LostTracking)
			{
				IntPtr cameraHandle = NativeSession.FrameApi.AcquireCamera();
				LostTrackingReason = NativeSession.CameraApi.GetLostTrackingReason(cameraHandle);
				NativeSession.CameraApi.Release(cameraHandle);
			}
			if (SessionStatus.IsError() && sessionStatus.IsError() != SessionStatus.IsError())
			{
				FireOnSessionSetEnabled(isEnabled: false);
				_disabledSessionOnErrorState = true;
			}
			else if (SessionStatus.IsValid() && _disabledSessionOnErrorState)
			{
				if (SessionComponent.enabled)
				{
					FireOnSessionSetEnabled(isEnabled: true);
				}
				_disabledSessionOnErrorState = false;
			}
			IntPtr sessionHandle3 = IntPtr.Zero;
			ExternApi.ArPresto_getSession(ref sessionHandle3);
			IsSessionChangedThisFrame = _cachedSessionHandle != sessionHandle3;
			_cachedSessionHandle = sessionHandle3;
			ExternApi.ArPresto_getFrame(ref _cachedFrameHandle);
			if (NativeSession != null)
			{
				NativeSession.OnUpdate(_cachedFrameHandle);
			}
			UpdateTextureIfNeeded();
			if (this.EarlyUpdate != null)
			{
				this.EarlyUpdate();
			}
		}

		private void SetCameraTextureNameIfNecessary()
		{
			if (!InstantPreviewManager.IsProvidingPlatform)
			{
				if (_cameraTextureIds == null)
				{
					GenerateCameraTextureNames();
				}
				if (NativeSession != null && !ArCoreHasValidTextureName())
				{
					ExternApi.ArPresto_setCameraTextureNames(_cameraTextureIds.Length, _cameraTextureIds);
				}
			}
		}

		private bool ArCoreHasValidTextureName()
		{
			int cameraTextureName = NativeSession.FrameApi.GetCameraTextureName();
			if (cameraTextureName != -1)
			{
				return cameraTextureName != 0;
			}
			return false;
		}

		private void GenerateCameraTextureNames()
		{
			int num = ((!SystemInfo.graphicsMultiThreaded) ? 1 : 4);
			UnityEngine.Debug.LogFormat("Using {0} textures for ARCore session", num);
			_cameraTextureIds = new int[num];
			OpenGL.glGenTextures(_cameraTextureIds.Length, _cameraTextureIds);
			int num2 = OpenGL.glGetError();
			if (num2 != 0)
			{
				UnityEngine.Debug.LogErrorFormat("OpenGL glGenTextures error: {0}", num2);
			}
			int[] cameraTextureIds = _cameraTextureIds;
			foreach (int num3 in cameraTextureIds)
			{
				OpenGL.glBindTexture(OpenGL.Target.GL_TEXTURE_EXTERNAL_OES, num3);
				Texture2D value = Texture2D.CreateExternalTexture(0, 0, TextureFormat.ARGB32, mipChain: false, linear: false, new IntPtr(num3));
				_textureIdToTexture2D[num3] = value;
			}
		}

		private void Initialize()
		{
			if (_nativeSessions != null)
			{
				foreach (NativeSession value in _nativeSessions.Values)
				{
					value.MarkDestroyed();
				}
			}
			_nativeSessions = new Dictionary<IntPtr, NativeSession>(new IntPtrEqualityComparer());
			_cachedSessionHandle = IntPtr.Zero;
			_cachedFrameHandle = IntPtr.Zero;
			_cachedConfig = null;
			_desiredSessionState = null;
			_haveDisableToEnableTransition = false;
			_requiredPermissionNames = new HashSet<string>();
			_androidPermissionRequest = null;
			BackgroundTexture = null;
			SessionComponent = null;
			IsSessionChangedThisFrame = true;
			SessionStatus = SessionStatus.None;
			LostTrackingReason = LostTrackingReason.None;
		}

		private void UpdateTextureIfNeeded()
		{
			Texture2D backgroundTexture = BackgroundTexture;
			if (InstantPreviewManager.UpdateBackgroundTextureIfNeeded(ref backgroundTexture))
			{
				BackgroundTexture = backgroundTexture;
				return;
			}
			IntPtr frameHandle = IntPtr.Zero;
			ExternApi.ArPresto_getFrame(ref frameHandle);
			if (!(frameHandle == IntPtr.Zero))
			{
				int cameraTextureName = NativeSession.FrameApi.GetCameraTextureName();
				Texture2D value = null;
				if (_textureIdToTexture2D.TryGetValue(cameraTextureName, out value))
				{
					BackgroundTexture = value;
				}
			}
		}

		private void SetSessionEnabled(bool sessionEnabled)
		{
			if (!sessionEnabled || !(SessionComponent == null))
			{
				if (!SessionStatus.IsError())
				{
					FireOnSessionSetEnabled(sessionEnabled);
				}
				ExternApi.ArPresto_setEnabled(sessionEnabled);
			}
		}

		private bool SetCameraDirection(DeviceCameraDirection cameraDirection)
		{
			if (_cachedCameraDirection.HasValue && _cachedCameraDirection.Value == cameraDirection)
			{
				return false;
			}
			if (InstantPreviewManager.IsProvidingPlatform && cameraDirection == DeviceCameraDirection.BackFacing)
			{
				return false;
			}
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("enable front-facing (selfie) camera");
				_cachedCameraDirection = DeviceCameraDirection.BackFacing;
				if (SessionComponent != null)
				{
					SessionComponent.DeviceCameraDirection = DeviceCameraDirection.BackFacing;
				}
				return false;
			}
			_cachedCameraDirection = cameraDirection;
			ApiPrestoStatus prestoStatus = ApiPrestoStatus.Uninitialized;
			ExternApi.ArPresto_getStatus(ref prestoStatus);
			if (prestoStatus == ApiPrestoStatus.ErrorInvalidCameraConfig)
			{
				OnBeforeResumeSession(_cachedSessionHandle);
			}
			return true;
		}

		private void SetSessionConfiguration(IntPtr sessionHandle, IntPtr configHandle)
		{
			if (configHandle == IntPtr.Zero)
			{
				UnityEngine.Debug.LogWarning("Cannot set configuration for invalid configHanlde.");
			}
			else if (sessionHandle == IntPtr.Zero && !InstantPreviewManager.IsProvidingPlatform)
			{
				UnityEngine.Debug.LogWarning("Cannot set configuration for invalid sessionHandle.");
			}
			else
			{
				if (_cachedConfig == null)
				{
					return;
				}
				if (_cachedConfig.DepthMode != DepthMode.Disabled && !GetNativeSession(sessionHandle).SessionApi.IsDepthModeSupported(_cachedConfig.DepthMode.ToApiDepthMode()))
				{
					_cachedConfig.DepthMode = DepthMode.Disabled;
				}
				NativeSession nativeSession = GetNativeSession(sessionHandle);
				if (_cachedCameraDirection.HasValue && nativeSession.SessionApi.GetCameraConfig().FacingDirection != _cachedCameraDirection)
				{
					_cachedConfig = null;
					return;
				}
				SessionConfigApi.UpdateApiConfigWithARCoreSessionConfig(sessionHandle, configHandle, _cachedConfig);
				if (this.OnSetConfiguration != null)
				{
					this.OnSetConfiguration(sessionHandle, configHandle);
				}
			}
		}

		private void UpdateConfiguration(ARCoreSessionConfig config)
		{
			if (!(config == null) && (!(_cachedConfig != null) || !config.Equals(_cachedConfig) || (!(config.AugmentedImageDatabase == null) && config.AugmentedImageDatabase._isDirty) || ExperimentManager.Instance.IsConfigurationDirty))
			{
				_cachedConfig = ScriptableObject.CreateInstance<ARCoreSessionConfig>();
				_cachedConfig.CopyFrom(config);
				if (_requiredPermissionNames.Count > 0)
				{
					RequestPermissions();
				}
				else
				{
					ExternApi.ArPresto_setConfigurationDirty();
				}
			}
		}

		private void UpdateDisplayGeometry()
		{
			if (!_cachedScreenOrientation.HasValue || Screen.orientation != _cachedScreenOrientation)
			{
				_cachedScreenOrientation = Screen.orientation;
				_cachedDisplayRotation = AndroidNativeHelper.GetDisplayRotation();
			}
			ExternApi.ArPresto_setDisplayGeometry(_cachedDisplayRotation, Screen.width, Screen.height);
		}

		private NativeSession GetNativeSession(IntPtr sessionHandle)
		{
			if (!_nativeSessions.TryGetValue(sessionHandle, out var value))
			{
				value = new NativeSession(sessionHandle, _cachedFrameHandle);
				_nativeSessions.Add(sessionHandle, value);
			}
			return value;
		}

		private void FireOnSessionSetEnabled(bool isEnabled)
		{
			if (this.OnSessionSetEnabled != null)
			{
				this.OnSessionSetEnabled(isEnabled);
			}
		}

		private void RequestPermissions()
		{
			if (_requiredPermissionNames.Count == 0 || !ARPrestoCallbackManager.Instance.IsCameraPermissionGranted() || _androidPermissionRequest != null)
			{
				return;
			}
			_androidPermissionRequest = AndroidPermissionsManager.RequestPermission(_requiredPermissionNames.First());
			if (_androidPermissionRequest != null)
			{
				_requiredPermissionNames.Remove(_requiredPermissionNames.First());
				_androidPermissionRequest.ThenAction(delegate
				{
					_androidPermissionRequest = null;
					ExternApi.ArPresto_setConfigurationDirty();
				});
			}
		}
	}
	internal class ARPrestoCallbackManager
	{
		public delegate void EarlyUpdateCallback();

		private delegate void OnBeforeSetConfigurationCallback(IntPtr sessionHandle, IntPtr configHandle);

		private delegate ApiPrestoCallbackResult OnBeforeResumeSessionCallback(IntPtr sessionHandle);

		private delegate void CameraPermissionRequestProvider(CameraPermissionsResultCallback onComplete, IntPtr context);

		private delegate void CameraPermissionsResultCallback(bool granted, IntPtr context);

		private delegate void CheckApkAvailabilityResultCallback(ApiAvailability status, IntPtr context);

		private delegate void RequestApkInstallationResultCallback(ApiApkInstallationStatus status, IntPtr context);

		private delegate void SessionCreationResultCallback(IntPtr sessionHandle, IntPtr frameHandle, IntPtr context, ApiArStatus status);

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_unity_api")]
			public static extern void ArCoreUnity_getJniInfo(ref IntPtr javaVM, ref IntPtr activity);

			[DllImport("arcore_unity_api")]
			public static extern bool ArCoreUnity_setArPrestoInitialized(EarlyUpdateCallback onEarlyUpdate);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_initialize(IntPtr javaVM, IntPtr activity, CameraPermissionRequestProvider requestCameraPermission, OnBeforeSetConfigurationCallback onBeforeSetConfiguration, OnBeforeResumeSessionCallback onBeforeResumeSession);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_checkApkAvailability(CheckApkAvailabilityResultCallback onResult, IntPtr context);

			[DllImport("arpresto_api")]
			public static extern void ArPresto_requestApkInstallation(bool user_requested, RequestApkInstallationResultCallback onResult, IntPtr context);
		}

		internal const string _cameraPermissionName = "android.permission.CAMERA";

		private static ARPrestoCallbackManager _instance;

		private static IAndroidPermissionsCheck _androidPermissionCheck;

		private CheckApkAvailabilityResultCallback _checkApkAvailabilityResultCallback;

		private RequestApkInstallationResultCallback _requestApkInstallationResultCallback;

		private CameraPermissionRequestProvider _requestCameraPermissionCallback;

		private EarlyUpdateCallback _earlyUpdateCallback;

		private OnBeforeSetConfigurationCallback _onBeforeSetConfigurationCallback;

		private OnBeforeResumeSessionCallback _onBeforeResumeSessionCallback;

		private List<Action<ApkAvailabilityStatus>> _pendingAvailabilityCheckCallbacks = new List<Action<ApkAvailabilityStatus>>();

		private List<Action<ApkInstallationStatus>> _pendingInstallationRequestCallbacks = new List<Action<ApkInstallationStatus>>();

		public static ARPrestoCallbackManager Instance
		{
			get
			{
				if (_instance == null)
				{
					if (_androidPermissionCheck == null && !InstantPreviewManager.IsProvidingPlatform)
					{
						_androidPermissionCheck = AndroidPermissionsManager.GetInstance();
					}
					_instance = new ARPrestoCallbackManager();
					_instance.Initialize();
				}
				return _instance;
			}
		}

		public event Action EarlyUpdate;

		public event Func<IntPtr, ApiPrestoCallbackResult> BeforeResumeSession;

		public event Action<IntPtr, IntPtr> OnSetConfiguration;

		public AsyncTask<ApkAvailabilityStatus> CheckApkAvailability()
		{
			Action<ApkAvailabilityStatus> asyncOperationComplete;
			AsyncTask<ApkAvailabilityStatus> result = new AsyncTask<ApkAvailabilityStatus>(out asyncOperationComplete);
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("determine ARCore APK availability");
				return result;
			}
			ExternApi.ArPresto_checkApkAvailability(_checkApkAvailabilityResultCallback, IntPtr.Zero);
			_pendingAvailabilityCheckCallbacks.Add(asyncOperationComplete);
			return result;
		}

		public AsyncTask<ApkInstallationStatus> RequestApkInstallation(bool userRequested)
		{
			Action<ApkInstallationStatus> asyncOperationComplete;
			AsyncTask<ApkInstallationStatus> result = new AsyncTask<ApkInstallationStatus>(out asyncOperationComplete);
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				InstantPreviewManager.LogLimitedSupportMessage("request installation of ARCore APK");
				return result;
			}
			ExternApi.ArPresto_requestApkInstallation(userRequested, _requestApkInstallationResultCallback, IntPtr.Zero);
			_pendingInstallationRequestCallbacks.Add(asyncOperationComplete);
			return result;
		}

		public bool IsCameraPermissionGranted()
		{
			return AndroidPermissionsManager.IsPermissionGranted("android.permission.CAMERA");
		}

		internal static void ResetInstance()
		{
			_instance = null;
			_androidPermissionCheck = null;
		}

		internal static void SetAndroidPermissionCheck(IAndroidPermissionsCheck androidPermissionsCheck)
		{
			_androidPermissionCheck = androidPermissionsCheck;
		}

		[MonoPInvokeCallback(typeof(CheckApkAvailabilityResultCallback))]
		private static void OnCheckApkAvailabilityResultTrampoline(ApiAvailability status, IntPtr context)
		{
			Instance.OnCheckApkAvailabilityResult(status.ToApkAvailabilityStatus());
		}

		[MonoPInvokeCallback(typeof(RequestApkInstallationResultCallback))]
		private static void OnApkInstallationResultTrampoline(ApiApkInstallationStatus status, IntPtr context)
		{
			Instance.OnRequestApkInstallationResult(status.ToApkInstallationStatus());
		}

		[MonoPInvokeCallback(typeof(CameraPermissionRequestProvider))]
		private static void RequestCameraPermissionTrampoline(CameraPermissionsResultCallback onComplete, IntPtr context)
		{
			Instance.RequestCameraPermission(onComplete, context);
		}

		[MonoPInvokeCallback(typeof(EarlyUpdateCallback))]
		private static void EarlyUpdateTrampoline()
		{
			if (Instance.EarlyUpdate != null)
			{
				Instance.EarlyUpdate();
			}
		}

		[MonoPInvokeCallback(typeof(OnBeforeSetConfigurationCallback))]
		private static void BeforeSetConfigurationTrampoline(IntPtr sessionHandle, IntPtr configHandle)
		{
			if (Instance.OnSetConfiguration != null)
			{
				Instance.OnSetConfiguration(sessionHandle, configHandle);
			}
		}

		[MonoPInvokeCallback(typeof(OnBeforeResumeSessionCallback))]
		private static ApiPrestoCallbackResult BeforeResumeSessionTrampoline(IntPtr sessionHandle)
		{
			if (Instance.BeforeResumeSession != null)
			{
				return Instance.BeforeResumeSession(sessionHandle);
			}
			return ApiPrestoCallbackResult.Success;
		}

		private void Initialize()
		{
			_earlyUpdateCallback = EarlyUpdateTrampoline;
			if (InstantPreviewManager.IsProvidingPlatform)
			{
				ExternApi.ArCoreUnity_setArPrestoInitialized(_earlyUpdateCallback);
			}
			else if (!ExternApi.ArCoreUnity_setArPrestoInitialized(_earlyUpdateCallback))
			{
				UnityEngine.Debug.LogError("Missing Unity Engine ARCore support.  Please ensure that the Unity project has the 'Player Settings > XR Settings > ARCore Supported' checkbox enabled.");
			}
			IntPtr javaVM = IntPtr.Zero;
			IntPtr activity = IntPtr.Zero;
			ExternApi.ArCoreUnity_getJniInfo(ref javaVM, ref activity);
			_checkApkAvailabilityResultCallback = OnCheckApkAvailabilityResultTrampoline;
			_requestApkInstallationResultCallback = OnApkInstallationResultTrampoline;
			_requestCameraPermissionCallback = RequestCameraPermissionTrampoline;
			_onBeforeSetConfigurationCallback = BeforeSetConfigurationTrampoline;
			_onBeforeResumeSessionCallback = BeforeResumeSessionTrampoline;
			ExternApi.ArPresto_initialize(javaVM, activity, _requestCameraPermissionCallback, _onBeforeSetConfigurationCallback, _onBeforeResumeSessionCallback);
		}

		private void OnCheckApkAvailabilityResult(ApkAvailabilityStatus status)
		{
			foreach (Action<ApkAvailabilityStatus> pendingAvailabilityCheckCallback in _pendingAvailabilityCheckCallbacks)
			{
				pendingAvailabilityCheckCallback(status);
			}
			_pendingAvailabilityCheckCallbacks.Clear();
		}

		private void OnRequestApkInstallationResult(ApkInstallationStatus status)
		{
			foreach (Action<ApkInstallationStatus> pendingInstallationRequestCallback in _pendingInstallationRequestCallbacks)
			{
				pendingInstallationRequestCallback(status);
			}
			_pendingInstallationRequestCallbacks.Clear();
		}

		private void RequestCameraPermission(CameraPermissionsResultCallback onComplete, IntPtr context)
		{
			if (_androidPermissionCheck != null)
			{
				_androidPermissionCheck.RequestAndroidPermission("android.permission.CAMERA").ThenAction(delegate(AndroidPermissionsRequestResult grantResult)
				{
					onComplete(grantResult.IsAllGranted, context);
				});
			}
		}
	}
	internal interface ILifecycleManager
	{
		SessionStatus SessionStatus { get; }

		LostTrackingReason LostTrackingReason { get; }

		ARCoreSession SessionComponent { get; }

		NativeSession NativeSession { get; }

		bool IsSessionChangedThisFrame { get; }

		event Action UpdateSessionFeatures;

		event Action EarlyUpdate;

		event Action<bool> OnSessionSetEnabled;

		event Action<IntPtr, IntPtr> OnSetConfiguration;

		event Action OnResetInstance;

		AsyncTask<ApkAvailabilityStatus> CheckApkAvailability();

		AsyncTask<ApkInstallationStatus> RequestApkInstallation(bool userRequested);

		void CreateSession(ARCoreSession session);

		void EnableSession();

		void DisableSession();

		void ResetSession();
	}
	internal class LifecycleManager
	{
		private static ILifecycleManager _instance;

		public static ILifecycleManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ARCoreAndroidLifecycleManager.Instance;
				}
				return _instance;
			}
		}

		internal static void ResetInstance()
		{
			if (_instance != null)
			{
				if (_instance is ARCoreAndroidLifecycleManager)
				{
					ARCoreAndroidLifecycleManager.ResetInstance();
				}
				_instance = null;
			}
		}
	}
	internal class PointCloudManager
	{
		private NativeSession _nativeSession;

		private float _lastReleasedPointcloudTimestamp;

		public IntPtr PointCloudHandle { get; private set; }

		public bool IsPointCloudNew => (float)_nativeSession.PointCloudApi.GetTimestamp(PointCloudHandle) != _lastReleasedPointcloudTimestamp;

		public PointCloudManager(NativeSession session)
		{
			_nativeSession = session;
		}

		public void OnUpdate()
		{
			if (PointCloudHandle != IntPtr.Zero)
			{
				_lastReleasedPointcloudTimestamp = _nativeSession.PointCloudApi.GetTimestamp(PointCloudHandle);
				_nativeSession.PointCloudApi.Release(PointCloudHandle);
				PointCloudHandle = IntPtr.Zero;
			}
			_nativeSession.FrameApi.TryAcquirePointCloudHandle(out var pointCloudHandle);
			PointCloudHandle = pointCloudHandle;
		}
	}
	internal class TrackableManager
	{
		private Dictionary<IntPtr, Trackable> _trackableDict = new Dictionary<IntPtr, Trackable>(new IntPtrEqualityComparer());

		private NativeSession _nativeSession;

		private int _lastUpdateFrame = -1;

		private List<Trackable> _newTrackables = new List<Trackable>();

		private List<Trackable> _allTrackables = new List<Trackable>();

		private List<Trackable> _updatedTrackables = new List<Trackable>();

		private HashSet<Trackable> _oldTrackables = new HashSet<Trackable>();

		public TrackableManager(NativeSession nativeSession)
		{
			_nativeSession = nativeSession;
			LifecycleManager.Instance.OnResetInstance += ClearCachedTrackables;
		}

		public Trackable TrackableFactory(IntPtr nativeHandle)
		{
			if (nativeHandle == IntPtr.Zero)
			{
				return null;
			}
			if (_trackableDict.TryGetValue(nativeHandle, out var value))
			{
				_nativeSession.TrackableApi.Release(nativeHandle);
				return value;
			}
			ApiTrackableType type = _nativeSession.TrackableApi.GetType(nativeHandle);
			switch (type)
			{
			case ApiTrackableType.Plane:
				value = new TrackedPlane(nativeHandle, _nativeSession);
				break;
			case ApiTrackableType.Point:
				value = new TrackedPoint(nativeHandle, _nativeSession);
				break;
			case ApiTrackableType.InstantPlacementPoint:
				value = new InstantPlacementPoint(nativeHandle, _nativeSession);
				break;
			case ApiTrackableType.AugmentedImage:
				value = new AugmentedImage(nativeHandle, _nativeSession);
				break;
			case ApiTrackableType.AugmentedFace:
				value = new AugmentedFace(nativeHandle, _nativeSession);
				break;
			case ApiTrackableType.DepthPoint:
				value = new DepthPoint(nativeHandle, _nativeSession);
				break;
			default:
				if (ExperimentManager.Instance.IsManagingTrackableType((int)type))
				{
					value = ExperimentManager.Instance.TrackableFactory((int)type, nativeHandle);
				}
				else
				{
					UnityEngine.Debug.LogWarning("TrackableFactory::No constructor for requested trackable type.");
				}
				break;
			}
			if (value != null)
			{
				_trackableDict.Add(nativeHandle, value);
			}
			return value;
		}

		public void GetTrackables<T>(List<T> trackables, TrackableQueryFilter filter) where T : Trackable
		{
			if (_lastUpdateFrame < Time.frameCount)
			{
				_nativeSession.FrameApi.GetUpdatedTrackables(_updatedTrackables);
				_nativeSession.SessionApi.GetAllTrackables(_allTrackables);
				_newTrackables.Clear();
				for (int i = 0; i < _allTrackables.Count; i++)
				{
					Trackable item = _allTrackables[i];
					if (!_oldTrackables.Contains(item))
					{
						_newTrackables.Add(item);
						_oldTrackables.Add(item);
					}
				}
				_lastUpdateFrame = Time.frameCount;
			}
			trackables.Clear();
			switch (filter)
			{
			case TrackableQueryFilter.All:
			{
				for (int k = 0; k < _allTrackables.Count; k++)
				{
					SafeAdd(_allTrackables[k], trackables);
				}
				break;
			}
			case TrackableQueryFilter.New:
			{
				for (int l = 0; l < _newTrackables.Count; l++)
				{
					SafeAdd(_newTrackables[l], trackables);
				}
				break;
			}
			case TrackableQueryFilter.Updated:
			{
				for (int j = 0; j < _updatedTrackables.Count; j++)
				{
					SafeAdd(_updatedTrackables[j], trackables);
				}
				break;
			}
			}
		}

		private void SafeAdd<T>(Trackable trackable, List<T> trackables) where T : Trackable
		{
			if (trackable is T)
			{
				trackables.Add(trackable as T);
			}
		}

		private void ClearCachedTrackables()
		{
			_trackableDict.Clear();
			_newTrackables.Clear();
			_allTrackables.Clear();
			_updatedTrackables.Clear();
			_oldTrackables.Clear();
		}
	}
	public class AndroidNativeHelper
	{
		public enum AndroidSurfaceRotation
		{
			Rotation0,
			Rotation90,
			Rotation180,
			Rotation270
		}

		public static AndroidSurfaceRotation GetDisplayRotation()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return AndroidSurfaceRotation.Rotation0;
			}
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Context");
			return (AndroidSurfaceRotation)new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getSystemService", new object[1] { androidJavaClass.GetStatic<string>("WINDOW_SERVICE") }).Call<AndroidJavaObject>("getDefaultDisplay", Array.Empty<object>())
				.Call<int>("getRotation", Array.Empty<object>());
		}
	}
	[Serializable]
	public enum AndroidAuthenticationStrategy
	{
		[DisplayName("Do Not Use")]
		DoNotUse,
		[DisplayName("API Key")]
		ApiKey,
		[DisplayName("Keyless (recommended)")]
		Keyless
	}
	[Serializable]
	public enum IOSAuthenticationStrategy
	{
		[DisplayName("Do Not Use")]
		DoNotUse,
		[DisplayName("API Key")]
		ApiKey,
		[DisplayName("Authentication Token (recommended)")]
		AuthenticationToken
	}
	[Serializable]
	public class ARCoreProjectSettings
	{
		[HideInInspector]
		public string Version;

		[DisplayName("ARCore Required")]
		public bool IsARCoreRequired;

		[DisplayName("Instant Preview Enabled")]
		public bool IsInstantPreviewEnabled;

		[DisplayName("iOS Support Enabled")]
		public bool IsIOSSupportEnabled;

		[DisplayName("Android Authentication Strategy")]
		[DynamicHelp("GetAndroidStrategyHelpInfo")]
		public AndroidAuthenticationStrategy AndroidAuthenticationStrategySetting;

		[DisplayCondition("IsAndroidApiKeyFieldDisplayed")]
		[DisplayName("Android API Key")]
		public string CloudServicesApiKey;

		[DynamicHelp("GetIosStrategyHelpInfo")]
		[DisplayName("iOS Authentication Strategy")]
		public IOSAuthenticationStrategy IOSAuthenticationStrategySetting;

		[DisplayCondition("IsIosApiKeyFieldDisplayed")]
		[DisplayName("iOS API Key")]
		public string IosCloudServicesApiKey;

		private const string _projectSettingsPath = "ProjectSettings/ARCoreProjectSettings.json";

		private static ARCoreProjectSettings _instance;

		public static ARCoreProjectSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					if (Application.isEditor)
					{
						_instance = new ARCoreProjectSettings();
						_instance.Load();
					}
					else
					{
						UnityEngine.Debug.LogError("Cannot access ARCoreProjectSettings outside of Unity Editor.");
					}
				}
				return _instance;
			}
		}

		public void Load()
		{
			Version = VersionInfo.Version;
			IsARCoreRequired = true;
			IsInstantPreviewEnabled = true;
			CloudServicesApiKey = string.Empty;
			IosCloudServicesApiKey = string.Empty;
			AndroidAuthenticationStrategySetting = AndroidAuthenticationStrategy.DoNotUse;
			IOSAuthenticationStrategySetting = IOSAuthenticationStrategy.DoNotUse;
			string text = Application.dataPath + "/../ProjectSettings/ARCoreProjectSettings.json";
			if (File.Exists(text))
			{
				ARCoreProjectSettings obj = JsonUtility.FromJson<ARCoreProjectSettings>(File.ReadAllText(text));
				FieldInfo[] fields = GetType().GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					fieldInfo.SetValue(this, fieldInfo.GetValue(obj));
				}
			}
			else
			{
				UnityEngine.Debug.Log("Cannot find ARCoreProjectSettings at " + text);
			}
			if (!string.IsNullOrEmpty(CloudServicesApiKey))
			{
				AndroidAuthenticationStrategySetting = AndroidAuthenticationStrategy.ApiKey;
			}
			if (!string.IsNullOrEmpty(IosCloudServicesApiKey))
			{
				IOSAuthenticationStrategySetting = IOSAuthenticationStrategy.ApiKey;
			}
			if (Version.Equals("V1.0.0"))
			{
				IsInstantPreviewEnabled = true;
				Version = "V1.1.0";
			}
			if (Version.Equals("V1.1.0"))
			{
				IosCloudServicesApiKey = CloudServicesApiKey;
				Version = "V1.3.0";
			}
			if (!Version.Equals(VersionInfo.Version))
			{
				Version = VersionInfo.Version;
			}
		}

		public void Save()
		{
			File.WriteAllText("ProjectSettings/ARCoreProjectSettings.json", JsonUtility.ToJson(this));
		}

		public bool IsAndroidApiKeyFieldDisplayed()
		{
			if (AndroidAuthenticationStrategySetting == AndroidAuthenticationStrategy.ApiKey)
			{
				return true;
			}
			CloudServicesApiKey = string.Empty;
			return false;
		}

		public HelpAttribute GetAndroidStrategyHelpInfo()
		{
			if (AndroidAuthenticationStrategySetting == AndroidAuthenticationStrategy.ApiKey)
			{
				return new HelpAttribute("Cloud Anchor persistence will not be availble on Android when 'API Key' authentication strategy is selected.", HelpAttribute.HelpMessageType.Warning);
			}
			return null;
		}

		public bool IsIosApiKeyFieldDisplayed()
		{
			if (IOSAuthenticationStrategySetting == IOSAuthenticationStrategy.ApiKey)
			{
				return true;
			}
			IosCloudServicesApiKey = string.Empty;
			return false;
		}

		public HelpAttribute GetIosStrategyHelpInfo()
		{
			if (IOSAuthenticationStrategySetting == IOSAuthenticationStrategy.ApiKey)
			{
				return new HelpAttribute("Cloud Anchor persistence will not be availble on iOS when 'API Key' authentication strategy is selected.", HelpAttribute.HelpMessageType.Warning);
			}
			return null;
		}
	}
	public class DisplayConditionAttribute : Attribute
	{
		public readonly string CheckingFunction;

		public DisplayConditionAttribute(string checkingFunction)
		{
			CheckingFunction = checkingFunction;
		}
	}
	public class DisplayNameAttribute : Attribute
	{
		public readonly string DisplayString;

		public DisplayNameAttribute(string displayString)
		{
			DisplayString = displayString;
		}
	}
	public class DynamicHelpAttribute : Attribute
	{
		public readonly string CheckingFunction;

		public DynamicHelpAttribute(string checkingFunction)
		{
			CheckingFunction = checkingFunction;
		}
	}
	public class EnumRangeAttribute : Attribute
	{
		public readonly string CheckingFunction;

		public EnumRangeAttribute(string checkingFunction)
		{
			CheckingFunction = checkingFunction;
		}
	}
	public class ARDebug
	{
		public static void LogError(object message)
		{
			UnityEngine.Debug.LogErrorFormat(string.Concat(message, "\n{0}"), new StackTrace(1));
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			object[] array = new object[args.Length + 1];
			Array.Copy(args, array, args.Length);
			array[args.Length] = new StackTrace(1);
			UnityEngine.Debug.LogErrorFormat(format + "\n{" + args.Length + "}", array);
		}
	}
	internal class ConversionHelper
	{
		private static readonly Matrix4x4 _unityWorldToGLWorld = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));

		private static readonly Matrix4x4 _unityWorldToGLWorldInverse = _unityWorldToGLWorld.inverse;

		public static void UnityPoseToApiPose(Pose unityPose, out ApiPoseData apiPose)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(unityPose.position, unityPose.rotation, Vector3.one);
			Matrix4x4 matrix4x2 = _unityWorldToGLWorld * matrix4x * _unityWorldToGLWorldInverse;
			Vector3 vector = matrix4x2.GetColumn(3);
			Quaternion quaternion = Quaternion.LookRotation(matrix4x2.GetColumn(2), matrix4x2.GetColumn(1));
			apiPose.X = vector.x;
			apiPose.Y = vector.y;
			apiPose.Z = vector.z;
			apiPose.Qx = quaternion.x;
			apiPose.Qy = quaternion.y;
			apiPose.Qz = quaternion.z;
			apiPose.Qw = quaternion.w;
		}

		public static void ApiPoseToUnityPose(ApiPoseData apiPose, out Pose unityPose)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(apiPose.X, apiPose.Y, apiPose.Z), new Quaternion(apiPose.Qx, apiPose.Qy, apiPose.Qz, apiPose.Qw), Vector3.one);
			Matrix4x4 matrix4x2 = _unityWorldToGLWorld * matrix4x * _unityWorldToGLWorldInverse;
			Vector3 position = matrix4x2.GetColumn(3);
			Quaternion rotation = Quaternion.LookRotation(matrix4x2.GetColumn(2), matrix4x2.GetColumn(1));
			unityPose = new Pose(position, rotation);
		}

		public static void ApiVectorToUnityVector(float[] ApiVector, out Vector3 unityVector)
		{
			unityVector = _unityWorldToGLWorld * new Vector3(ApiVector[0], ApiVector[1], ApiVector[2]);
		}
	}
	public class DependentModulesManager
	{
		private static List<IDependentModule> _modules;

		public static List<IDependentModule> GetModules()
		{
			if (_modules == null)
			{
				_modules = new List<IDependentModule>();
			}
			return _modules;
		}
	}
	public interface IDependentModule
	{
		bool IsEnabled(ARCoreProjectSettings settings);

		string GetAndroidManifestSnippet(ARCoreProjectSettings settings);

		bool IsCompatibleWithSessionConfig(ARCoreProjectSettings settings, ARCoreSessionConfig sessionConfig);
	}
	internal class DllImportNoop : Attribute
	{
		public DllImportNoop(string dllName)
		{
		}
	}
	public class HelpAttribute : PropertyAttribute
	{
		public enum HelpMessageType
		{
			None,
			Info,
			Warning,
			Error
		}

		public readonly string HelpMessage;

		public readonly HelpMessageType MessageType;

		public HelpAttribute(string helpMessage, HelpMessageType messageType = HelpMessageType.None)
		{
			HelpMessage = helpMessage;
			MessageType = messageType;
		}
	}
	internal class IntPtrEqualityComparer : IEqualityComparer<IntPtr>
	{
		public bool Equals(IntPtr intPtr1, IntPtr intPtr2)
		{
			return intPtr1 == intPtr2;
		}

		public int GetHashCode(IntPtr intPtr)
		{
			return intPtr.GetHashCode();
		}
	}
	public class MarshalingHelper
	{
		public static void AddUnmanagedStructArrayToList<T>(IntPtr arrayPtr, int arrayLength, List<T> list) where T : struct
		{
			if (!(arrayPtr == IntPtr.Zero) && list != null)
			{
				for (int i = 0; i < arrayLength; i++)
				{
					list.Add((T)Marshal.PtrToStructure(GetPtrToUnmanagedArrayElement<T>(arrayPtr, i), typeof(T)));
				}
			}
		}

		public static IntPtr GetPtrToUnmanagedArrayElement<T>(IntPtr arrayPtr, int arrayIndex) where T : struct
		{
			return new IntPtr(arrayPtr.ToInt64() + Marshal.SizeOf(typeof(T)) * arrayIndex);
		}
	}
	public static class OpenGL
	{
		public enum Target
		{
			GL_TEXTURE_EXTERNAL_OES = 36197
		}

		[DllImport("GLESv3")]
		public static extern int glGetError();

		[DllImport("GLESv3")]
		public static extern void glGenTextures(int n, int[] textures);

		[DllImport("GLESv3")]
		public static extern void glBindTexture(Target target, int texture);
	}
	public static class ShellHelper
	{
		public static void RunCommand(string fileName, string arguments, out string output, out string error)
		{
			using Process process = new Process();
			ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.CreateNoWindow = true;
			process.StartInfo = processStartInfo;
			StringBuilder outputBuilder = new StringBuilder();
			StringBuilder errorBuilder = new StringBuilder();
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs ef)
			{
				outputBuilder.AppendLine(ef.Data);
			};
			process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs ef)
			{
				errorBuilder.AppendLine(ef.Data);
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();
			process.Close();
			output = outputBuilder.ToString().Trim();
			error = errorBuilder.ToString().Trim();
		}

		public static string GetAdbPath()
		{
			string text = null;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return Path.Combine(Path.GetFullPath(text), Path.Combine("platform-tools", GetAdbFileName()));
		}

		public static string GetAdbFileName()
		{
			string text = "adb";
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				text = Path.ChangeExtension(text, "exe");
			}
			return text;
		}
	}
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class SuppressMemoryAllocationErrorAttribute : Attribute
	{
		public bool IsWarning { get; set; }

		public string Reason { get; set; }

		public SuppressMemoryAllocationErrorAttribute()
		{
			IsWarning = false;
			Reason = "Unknown";
		}
	}
	internal class ThrottledLogMessage
	{
		private float _lastMessageTime;

		private float _minLogIntervalSeconds;

		public ThrottledLogMessage(float minLogIntervalSeconds)
		{
			_minLogIntervalSeconds = minLogIntervalSeconds;
			_lastMessageTime = 0f - minLogIntervalSeconds - 1f;
		}

		public void ThrottledLogWarningFormat(string format, params object[] args)
		{
			if (ShouldLog())
			{
				UnityEngine.Debug.LogWarningFormat(format, args);
			}
		}

		private bool ShouldLog()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - _lastMessageTime > _minLogIntervalSeconds)
			{
				_lastMessageTime = realtimeSinceStartup;
				return true;
			}
			return false;
		}
	}
}
namespace GoogleARCoreInternal.CrossPlatform
{
	internal enum ApiCloudAnchorMode
	{
		Disabled,
		Enabled
	}
	internal enum ApiCloudAnchorState
	{
		None = 0,
		TaskInProgress = 1,
		Success = 2,
		ErrorInternal = -1,
		ErrorNotAuthorized = -2,
		ErrorResourceExhausted = -4,
		ErrorHostingDatasetProcessingFailed = -5,
		ErrorResolveingCloudIdNotFound = -6,
		ErrorResolvingSDKTooOld = -8,
		ErrorResolvingSDKTooNew = -9,
		ErrorHostingServiceUnavailable = -10
	}
	internal class CloudServiceManager
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_sdk_c")]
			public static extern void ARKitAnchor_create(IntPtr poseHandle, ref IntPtr arkitAnchorHandle);

			[DllImport("arcore_sdk_c")]
			public static extern void ARKitAnchor_release(IntPtr arkitAnchorHandle);
		}

		private class CloudAnchorRequest
		{
			public bool IsComplete;

			public NativeSession NativeSession;

			public string CloudAnchorId;

			public IntPtr AnchorHandle;

			public Action<CloudAnchorResult> OnTaskComplete;
		}

		private static CloudServiceManager _instance;

		private List<CloudAnchorRequest> _cloudAnchorRequests = new List<CloudAnchorRequest>();

		public static CloudServiceManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CloudServiceManager();
					LifecycleManager.Instance.EarlyUpdate += _instance.OnEarlyUpdate;
					LifecycleManager.Instance.OnResetInstance += ResetInstance;
				}
				return _instance;
			}
		}

		public AsyncTask<CloudAnchorResult> CreateCloudAnchor(Anchor anchor)
		{
			if (!CreateCloudAnchorResultAsyncTask(out var onComplete, out var task))
			{
				return task;
			}
			CreateCloudAnchor(onComplete, anchor._nativeHandle);
			return task;
		}

		public AsyncTask<CloudAnchorResult> CreateCloudAnchor(Pose pose)
		{
			if (!CreateCloudAnchorResultAsyncTask(out var onComplete, out var task))
			{
				return task;
			}
			IntPtr intPtr = LifecycleManager.Instance.NativeSession.PoseApi.Create(pose);
			IntPtr arkitAnchorHandle = IntPtr.Zero;
			ExternApi.ARKitAnchor_create(intPtr, ref arkitAnchorHandle);
			CreateCloudAnchor(onComplete, arkitAnchorHandle);
			LifecycleManager.Instance.NativeSession.PoseApi.Destroy(intPtr);
			ExternApi.ARKitAnchor_release(arkitAnchorHandle);
			return task;
		}

		public AsyncTask<CloudAnchorResult> ResolveCloudAnchor(string cloudAnchorId)
		{
			if (!CreateCloudAnchorResultAsyncTask(out var onComplete, out var task))
			{
				return task;
			}
			IntPtr cloudAnchorHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = LifecycleManager.Instance.NativeSession.SessionApi.ResolveCloudAnchor(cloudAnchorId, out cloudAnchorHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				onComplete(new CloudAnchorResult
				{
					Response = apiArStatus.ToCloudServiceResponse(),
					Anchor = null
				});
				return task;
			}
			CreateAndTrackCloudAnchorRequest(cloudAnchorHandle, onComplete, cloudAnchorId);
			return task;
		}

		public void CancelCloudAnchorAsyncTask(string cloudAnchorId)
		{
			if (string.IsNullOrEmpty(cloudAnchorId))
			{
				UnityEngine.Debug.LogWarning("Couldn't find pending operation for empty cloudAnchorId.");
			}
			else
			{
				CancelCloudAnchorRequest(cloudAnchorId);
			}
		}

		public AsyncTask<CloudAnchorResult> CreateCloudAnchor(Anchor anchor, int ttlDays)
		{
			if (!CreateCloudAnchorResultAsyncTask(out var onComplete, out var task))
			{
				return task;
			}
			CreateCloudAnchor(onComplete, anchor._nativeHandle, ttlDays);
			return task;
		}

		public AsyncTask<CloudAnchorResult> CreateCloudAnchor(Pose pose, int ttlDays)
		{
			if (!CreateCloudAnchorResultAsyncTask(out var onComplete, out var task))
			{
				return task;
			}
			IntPtr intPtr = LifecycleManager.Instance.NativeSession.PoseApi.Create(pose);
			IntPtr arkitAnchorHandle = IntPtr.Zero;
			ExternApi.ARKitAnchor_create(intPtr, ref arkitAnchorHandle);
			CreateCloudAnchor(onComplete, arkitAnchorHandle, ttlDays);
			LifecycleManager.Instance.NativeSession.PoseApi.Destroy(intPtr);
			ExternApi.ARKitAnchor_release(arkitAnchorHandle);
			return task;
		}

		public void SetAuthToken(string authToken)
		{
			if (string.IsNullOrEmpty(authToken))
			{
				UnityEngine.Debug.LogWarning("Cannot set token in applications with empty token.");
			}
			else if (LifecycleManager.Instance.NativeSession == null)
			{
				UnityEngine.Debug.LogWarning("Cannot set token before ARCore session is created.");
			}
			else
			{
				LifecycleManager.Instance.NativeSession.SessionApi.SetAuthToken(authToken);
			}
		}

		public FeatureMapQuality EstimateFeatureMapQualityForHosting(Pose pose)
		{
			return LifecycleManager.Instance.NativeSession.SessionApi.EstimateFeatureMapQualityForHosting(pose);
		}

		protected internal bool CreateCloudAnchorResultAsyncTask(out Action<CloudAnchorResult> onComplete, out AsyncTask<CloudAnchorResult> task)
		{
			task = new AsyncTask<CloudAnchorResult>(out onComplete);
			if (LifecycleManager.Instance.NativeSession == null)
			{
				onComplete(new CloudAnchorResult
				{
					Response = CloudServiceResponse.ErrorNotSupportedByConfiguration,
					Anchor = null
				});
				return false;
			}
			return true;
		}

		protected internal void CreateAndTrackCloudAnchorRequest(IntPtr cloudAnchorHandle, Action<CloudAnchorResult> onComplete, string cloudAnchorId = null)
		{
			if (LifecycleManager.Instance.NativeSession == null || cloudAnchorHandle == IntPtr.Zero)
			{
				UnityEngine.Debug.LogError("Cannot create cloud anchor request when NativeSession is null or cloud anchor handle is IntPtr.Zero.");
				onComplete(new CloudAnchorResult
				{
					Response = CloudServiceResponse.ErrorInternal,
					Anchor = null
				});
			}
			else
			{
				CloudAnchorRequest request = new CloudAnchorRequest
				{
					IsComplete = false,
					NativeSession = LifecycleManager.Instance.NativeSession,
					CloudAnchorId = cloudAnchorId,
					AnchorHandle = cloudAnchorHandle,
					OnTaskComplete = onComplete
				};
				UpdateCloudAnchorRequest(request, isNewRequest: true);
			}
		}

		protected internal void CreateCloudAnchor(Action<CloudAnchorResult> onComplete, IntPtr anchorNativeHandle)
		{
			IntPtr cloudAnchorHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = LifecycleManager.Instance.NativeSession.SessionApi.CreateCloudAnchor(anchorNativeHandle, out cloudAnchorHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				onComplete(new CloudAnchorResult
				{
					Response = apiArStatus.ToCloudServiceResponse(),
					Anchor = null
				});
			}
			else
			{
				CreateAndTrackCloudAnchorRequest(cloudAnchorHandle, onComplete);
			}
		}

		protected internal void CreateCloudAnchor(Action<CloudAnchorResult> onComplete, IntPtr anchorNativeHandle, int ttlDays)
		{
			IntPtr cloudAnchorHandle = IntPtr.Zero;
			ApiArStatus apiArStatus = LifecycleManager.Instance.NativeSession.SessionApi.HostCloudAnchor(anchorNativeHandle, ttlDays, out cloudAnchorHandle);
			if (apiArStatus != ApiArStatus.Success)
			{
				onComplete(new CloudAnchorResult
				{
					Response = apiArStatus.ToCloudServiceResponse(),
					Anchor = null
				});
			}
			else
			{
				CreateAndTrackCloudAnchorRequest(cloudAnchorHandle, onComplete);
			}
		}

		protected internal void CancelCloudAnchorRequest(string cloudAnchorId)
		{
			bool flag = false;
			foreach (CloudAnchorRequest cloudAnchorRequest in _cloudAnchorRequests)
			{
				if (cloudAnchorRequest.CloudAnchorId != null && cloudAnchorRequest.CloudAnchorId.Equals(cloudAnchorId))
				{
					if (cloudAnchorRequest.NativeSession != null && !cloudAnchorRequest.NativeSession.IsDestroyed)
					{
						cloudAnchorRequest.NativeSession.AnchorApi.Detach(cloudAnchorRequest.AnchorHandle);
					}
					AnchorApi.Release(cloudAnchorRequest.AnchorHandle);
					CloudAnchorResult obj = new CloudAnchorResult
					{
						Response = CloudServiceResponse.ErrorRequestCancelled,
						Anchor = null
					};
					cloudAnchorRequest.OnTaskComplete(obj);
					cloudAnchorRequest.IsComplete = true;
					flag = true;
				}
			}
			_cloudAnchorRequests.RemoveAll((CloudAnchorRequest x) => x.IsComplete);
			if (!flag)
			{
				UnityEngine.Debug.LogWarning("Didn't find pending operation for cloudAnchorId: " + cloudAnchorId);
			}
		}

		private static void ResetInstance()
		{
			_instance = null;
		}

		private void OnEarlyUpdate()
		{
			foreach (CloudAnchorRequest cloudAnchorRequest in _cloudAnchorRequests)
			{
				UpdateCloudAnchorRequest(cloudAnchorRequest);
			}
			_cloudAnchorRequests.RemoveAll((CloudAnchorRequest x) => x.IsComplete);
		}

		private void UpdateCloudAnchorRequest(CloudAnchorRequest request, bool isNewRequest = false)
		{
			ApiCloudAnchorState cloudAnchorState = request.NativeSession.AnchorApi.GetCloudAnchorState(request.AnchorHandle);
			switch (cloudAnchorState)
			{
			case ApiCloudAnchorState.Success:
			{
				XPAnchor anchor = null;
				CloudServiceResponse response = CloudServiceResponse.Success;
				try
				{
					anchor = XPAnchor.Factory(request.NativeSession, request.AnchorHandle);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("Failed to create XP Anchor: " + ex.Message);
					response = CloudServiceResponse.ErrorInternal;
				}
				CloudAnchorResult obj = new CloudAnchorResult
				{
					Response = response,
					Anchor = anchor
				};
				request.OnTaskComplete(obj);
				request.IsComplete = true;
				break;
			}
			default:
			{
				if (request.NativeSession != null && !request.NativeSession.IsDestroyed)
				{
					request.NativeSession.AnchorApi.Detach(request.AnchorHandle);
				}
				AnchorApi.Release(request.AnchorHandle);
				CloudAnchorResult obj2 = new CloudAnchorResult
				{
					Response = cloudAnchorState.ToCloudServiceResponse(),
					Anchor = null
				};
				request.OnTaskComplete(obj2);
				request.IsComplete = true;
				break;
			}
			case ApiCloudAnchorState.TaskInProgress:
				if (isNewRequest)
				{
					_cloudAnchorRequests.Add(request);
				}
				break;
			}
		}
	}
}
namespace GoogleARCore
{
	public static class InstantPreviewInput
	{
		private struct NativeTouch
		{
			public TouchPhase Phase;

			public float X;

			public float Y;

			public float Pressure;

			public int Id;
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct NativeApi
		{
			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern void GetTouches(out IntPtr touches, out int count);

			[DllImport("arcore_instant_preview_unity_plugin")]
			public static extern void UnityGotTouches();
		}

		private static Touch[] _touches = new Touch[0];

		private static List<Touch> _touchList = new List<Touch>();

		public static string inputString => Input.inputString;

		public static Touch[] touches
		{
			get
			{
				NativeApi.UnityGotTouches();
				return _touches;
			}
		}

		public static int touchCount => touches.Length;

		public static Vector3 mousePosition => Input.mousePosition;

		public static bool mousePresent => Input.mousePresent;

		public static Touch GetTouch(int index)
		{
			return touches[index];
		}

		public static bool GetKey(KeyCode keyCode)
		{
			return Input.GetKey(keyCode);
		}

		public static bool GetMouseButton(int button)
		{
			return Input.GetMouseButton(button);
		}

		public static bool GetMouseButtonDown(int button)
		{
			return Input.GetMouseButtonDown(button);
		}

		public static bool GetMouseButtonUp(int button)
		{
			return Input.GetMouseButtonUp(button);
		}

		public static void Update()
		{
			if (!InstantPreviewManager.IsProvidingPlatform)
			{
				return;
			}
			for (int i = 0; i < _touchList.Count; i++)
			{
				if (_touchList[i].phase == TouchPhase.Ended)
				{
					_touchList.RemoveAt(i);
					i--;
					continue;
				}
				Touch value = _touchList[i];
				value.phase = TouchPhase.Stationary;
				value.deltaPosition = Vector2.zero;
				_touchList[i] = value;
			}
			NativeApi.GetTouches(out var intPtr, out var count);
			int num = Marshal.SizeOf(typeof(NativeTouch));
			for (int j = 0; j < count; j++)
			{
				NativeTouch nativeTouch = (NativeTouch)Marshal.PtrToStructure(new IntPtr(intPtr.ToInt64() + j * num), typeof(NativeTouch));
				Touch newTouch = new Touch
				{
					fingerId = nativeTouch.Id,
					phase = nativeTouch.Phase,
					pressure = nativeTouch.Pressure,
					position = new Vector2((float)Screen.width * nativeTouch.X, (float)Screen.height * (1f - nativeTouch.Y))
				};
				int num2 = _touchList.FindIndex((Touch touch2) => touch2.fingerId == newTouch.fingerId);
				if (num2 < 0)
				{
					_touchList.Add(newTouch);
					continue;
				}
				Touch touch = _touchList[num2];
				newTouch.deltaPosition += newTouch.position - touch.position;
				_touchList[num2] = newTouch;
			}
			_touches = _touchList.ToArray();
		}
	}
	public class InstantPreviewTrackedPoseDriver : MonoBehaviour
	{
		public void Update()
		{
			if (Application.isEditor)
			{
				base.transform.localPosition = Frame.Pose.position;
				base.transform.localRotation = Frame.Pose.rotation;
			}
		}
	}
	[HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/Anchor")]
	public class Anchor : MonoBehaviour
	{
		private static Dictionary<IntPtr, Anchor> _anchorDict = new Dictionary<IntPtr, Anchor>(new IntPtrEqualityComparer());

		private TrackingState _lastFrameTrackingState = TrackingState.Stopped;

		private bool _isSessionDestroyed;

		public TrackingState TrackingState
		{
			get
			{
				if (IsSessionDestroyed())
				{
					return TrackingState.Stopped;
				}
				return _nativeSession.AnchorApi.GetTrackingState(_nativeHandle);
			}
		}

		internal NativeSession _nativeSession { get; private set; }

		internal IntPtr _nativeHandle { get; private set; }

		internal static Anchor Factory(NativeSession nativeApi, IntPtr anchorNativeHandle, bool isCreate = true)
		{
			if (anchorNativeHandle == IntPtr.Zero)
			{
				return null;
			}
			if (_anchorDict.TryGetValue(anchorNativeHandle, out var value))
			{
				AnchorApi.Release(anchorNativeHandle);
				return value;
			}
			if (isCreate)
			{
				Anchor anchor = new GameObject().AddComponent<Anchor>();
				anchor.gameObject.name = "Anchor";
				anchor._nativeHandle = anchorNativeHandle;
				anchor._nativeSession = nativeApi;
				anchor.Update();
				_anchorDict.Add(anchorNativeHandle, anchor);
				return anchor;
			}
			return null;
		}

		internal void Update()
		{
			if (_nativeHandle == IntPtr.Zero)
			{
				UnityEngine.Debug.LogError("Anchor components instantiated outside of ARCore are not supported. Please use a 'Create' method within ARCore to instantiate anchors.");
			}
			else
			{
				if (IsSessionDestroyed())
				{
					return;
				}
				Pose pose = _nativeSession.AnchorApi.GetPose(_nativeHandle);
				base.transform.position = pose.position;
				base.transform.rotation = pose.rotation;
				TrackingState trackingState = TrackingState;
				if (_lastFrameTrackingState == trackingState)
				{
					return;
				}
				bool active = trackingState == TrackingState.Tracking;
				foreach (Transform item in base.transform)
				{
					item.gameObject.SetActive(active);
				}
				_lastFrameTrackingState = trackingState;
			}
		}

		private void OnDestroy()
		{
			if (!(_nativeHandle == IntPtr.Zero))
			{
				if (_nativeSession != null && !_nativeSession.IsDestroyed)
				{
					_nativeSession.AnchorApi.Detach(_nativeHandle);
				}
				_anchorDict.Remove(_nativeHandle);
				AnchorApi.Release(_nativeHandle);
			}
		}

		private bool IsSessionDestroyed()
		{
			if (!_isSessionDestroyed && LifecycleManager.Instance.NativeSession != _nativeSession)
			{
				UnityEngine.Debug.LogErrorFormat("The session which created this anchor has been destroyed. The anchor on GameObject {0} can no longer update.", (base.gameObject != null) ? base.gameObject.name : "Unknown");
				_isSessionDestroyed = true;
			}
			return _isSessionDestroyed;
		}
	}
	public class AndroidPermissionsManager : AndroidJavaProxy, IAndroidPermissionsCheck
	{
		private static AndroidPermissionsManager _instance;

		private static AndroidJavaObject _activity;

		private static AndroidJavaObject _permissionService;

		private static AsyncTask<AndroidPermissionsRequestResult> _currentRequest;

		private static Action<AndroidPermissionsRequestResult> _onPermissionsRequestFinished;

		public AndroidPermissionsManager()
			: base("com.unity3d.plugin.UnityAndroidPermissions$IPermissionRequestResult")
		{
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Allocates new objects the first time is called")]
		public static bool IsPermissionGranted(string permissionName)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return true;
			}
			return GetPermissionsService().Call<bool>("IsPermissionGranted", new object[2]
			{
				GetUnityActivity(),
				permissionName
			});
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Allocates new objects the first time is called")]
		public static AsyncTask<AndroidPermissionsRequestResult> RequestPermission(string permissionName)
		{
			if (IsPermissionGranted(permissionName))
			{
				return new AsyncTask<AndroidPermissionsRequestResult>(new AndroidPermissionsRequestResult(new string[1] { permissionName }, new bool[1] { true }));
			}
			if (_currentRequest != null)
			{
				ARDebug.LogError("Attempted to make simultaneous Android permissions requests.");
				return null;
			}
			GetPermissionsService().Call("RequestPermissionAsync", GetUnityActivity(), new string[1] { permissionName }, GetInstance());
			_currentRequest = new AsyncTask<AndroidPermissionsRequestResult>(out _onPermissionsRequestFinished);
			return _currentRequest;
		}

		public AsyncTask<AndroidPermissionsRequestResult> RequestAndroidPermission(string permissionName)
		{
			return RequestPermission(permissionName);
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Implements java object interface.")]
		public virtual void OnPermissionGranted(string permissionName)
		{
			OnPermissionResult(permissionName, granted: true);
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Implements java object interface.")]
		public virtual void OnPermissionDenied(string permissionName)
		{
			OnPermissionResult(permissionName, granted: false);
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Implements java object interface.")]
		public virtual void OnActivityResult()
		{
		}

		internal static AndroidPermissionsManager GetInstance()
		{
			if (_instance == null)
			{
				_instance = new AndroidPermissionsManager();
			}
			return _instance;
		}

		private static AndroidJavaObject GetUnityActivity()
		{
			if (_activity == null)
			{
				_activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			}
			return _activity;
		}

		private static AndroidJavaObject GetPermissionsService()
		{
			if (_permissionService == null)
			{
				_permissionService = new AndroidJavaObject("com.unity3d.plugin.UnityAndroidPermissions");
			}
			return _permissionService;
		}

		private void OnPermissionResult(string permissionName, bool granted)
		{
			if (_onPermissionsRequestFinished == null)
			{
				UnityEngine.Debug.LogErrorFormat("AndroidPermissionsManager received an unexpected permissions result {0}", permissionName);
				return;
			}
			Action<AndroidPermissionsRequestResult> onPermissionsRequestFinished = _onPermissionsRequestFinished;
			_currentRequest = null;
			_onPermissionsRequestFinished = null;
			onPermissionsRequestFinished(new AndroidPermissionsRequestResult(new string[1] { permissionName }, new bool[1] { granted }));
		}
	}
	public struct AndroidPermissionsRequestResult
	{
		public string[] PermissionNames { get; private set; }

		public bool[] GrantResults { get; private set; }

		public bool IsAllGranted
		{
			[SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
			get
			{
				if (PermissionNames == null || GrantResults == null)
				{
					return false;
				}
				for (int i = 0; i < GrantResults.Length; i++)
				{
					if (!GrantResults[i])
					{
						return false;
					}
				}
				return true;
			}
		}

		public AndroidPermissionsRequestResult(string[] permissionNames, bool[] grantResults)
		{
			this = default(AndroidPermissionsRequestResult);
			PermissionNames = permissionNames;
			GrantResults = grantResults;
		}
	}
	internal enum ApiTrackingFailureReason
	{
		None,
		BadState,
		InsufficientLight,
		ExcessiveMotion,
		InsufficientFeatures,
		CameraUnavailable
	}
	public enum ApkAvailabilityStatus
	{
		UnknownError = 0,
		UnknownChecking = 1,
		UnknownTimedOut = 2,
		UnsupportedDeviceNotCapable = 100,
		SupportedNotInstalled = 201,
		SupportedApkTooOld = 202,
		SupportedInstalled = 203
	}
	public enum ApkInstallationStatus
	{
		Uninitialized = 0,
		Requested = 1,
		Success = 100,
		Error = 200,
		ErrorDeviceNotCompatible = 201,
		[Obsolete("Merged with ErrorDeviceNotCompatible. Use that instead.")]
		ErrorAndroidVersionNotSupported = 202,
		ErrorUserDeclined = 203
	}
	[HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/ARCoreBackgroundRenderer")]
	[RequireComponent(typeof(Camera))]
	public class ARCoreBackgroundRenderer : MonoBehaviour
	{
		private enum BackgroundTransitionState
		{
			BlackScreen,
			FadingIn,
			CameraImage
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			[DllImport("arcore_rendering_utils_api")]
			public static extern IntPtr ARCoreRenderingUtils_GetRenderEventFunc();
		}

		[Tooltip("A material used to render the AR background image.")]
		public Material BackgroundMaterial;

		private static readonly float _blackScreenDuration = 0.5f;

		private static readonly float _fadingInDuration = 0.5f;

		private Camera _camera;

		private Texture _transitionImageTexture;

		private BackgroundTransitionState _transitionState;

		private float _currentStateElapsed;

		private bool _sessionEnabled;

		private bool _userInvertCullingValue;

		private CameraClearFlags _cameraClearFlags = CameraClearFlags.Skybox;

		private CommandBuffer _commandBuffer;

		private void OnEnable()
		{
			if (BackgroundMaterial == null)
			{
				UnityEngine.Debug.LogError("ArCameraBackground:: No material assigned.");
				return;
			}
			LifecycleManager.Instance.OnSessionSetEnabled += OnSessionSetEnabled;
			_camera = GetComponent<Camera>();
			_transitionImageTexture = Resources.Load<Texture2D>("ViewInARIcon");
			BackgroundMaterial.SetTexture("_TransitionIconTex", _transitionImageTexture);
			EnableARBackgroundRendering();
		}

		private void OnDisable()
		{
			LifecycleManager.Instance.OnSessionSetEnabled -= OnSessionSetEnabled;
			_transitionState = BackgroundTransitionState.BlackScreen;
			_currentStateElapsed = 0f;
			_camera.ResetProjectionMatrix();
			DisableARBackgroundRendering();
		}

		private void OnPreRender()
		{
			_userInvertCullingValue = GL.invertCulling;
			ARCoreSession sessionComponent = LifecycleManager.Instance.SessionComponent;
			if (sessionComponent != null && sessionComponent.DeviceCameraDirection == DeviceCameraDirection.FrontFacing)
			{
				GL.invertCulling = true;
			}
		}

		private void OnPostRender()
		{
			GL.invertCulling = _userInvertCullingValue;
		}

		private void Update()
		{
			_currentStateElapsed += Time.deltaTime;
			UpdateState();
			UpdateShaderVariables();
		}

		private void UpdateState()
		{
			if (!_sessionEnabled && _transitionState != BackgroundTransitionState.BlackScreen)
			{
				_transitionState = BackgroundTransitionState.BlackScreen;
				_currentStateElapsed = 0f;
			}
			else if (_sessionEnabled && _transitionState == BackgroundTransitionState.BlackScreen && _currentStateElapsed > _blackScreenDuration)
			{
				_transitionState = BackgroundTransitionState.FadingIn;
				_currentStateElapsed = 0f;
			}
			else if (_sessionEnabled && _transitionState == BackgroundTransitionState.FadingIn && _currentStateElapsed > _fadingInDuration)
			{
				_transitionState = BackgroundTransitionState.CameraImage;
				_currentStateElapsed = 0f;
			}
		}

		private void UpdateShaderVariables()
		{
			if (_transitionState == BackgroundTransitionState.BlackScreen)
			{
				BackgroundMaterial.SetFloat("_Brightness", 0f);
			}
			else if (_transitionState == BackgroundTransitionState.FadingIn)
			{
				BackgroundMaterial.SetFloat("_Brightness", CosineLerp(_currentStateElapsed, _fadingInDuration));
			}
			else
			{
				BackgroundMaterial.SetFloat("_Brightness", 1f);
			}
			BackgroundMaterial.SetVector("_TransitionIconTexTransform", TextureTransform());
			if (_transitionState != BackgroundTransitionState.BlackScreen && !(Frame.CameraImage.Texture == null))
			{
				BackgroundMaterial.SetTexture("_MainTex", Frame.CameraImage.Texture);
				DisplayUvCoords textureDisplayUvs = Frame.CameraImage.TextureDisplayUvs;
				BackgroundMaterial.SetVector("_UvTopLeftRight", new Vector4(textureDisplayUvs.TopLeft.x, textureDisplayUvs.TopLeft.y, textureDisplayUvs.TopRight.x, textureDisplayUvs.TopRight.y));
				BackgroundMaterial.SetVector("_UvBottomLeftRight", new Vector4(textureDisplayUvs.BottomLeft.x, textureDisplayUvs.BottomLeft.y, textureDisplayUvs.BottomRight.x, textureDisplayUvs.BottomRight.y));
				_camera.projectionMatrix = Frame.CameraImage.GetCameraProjectionMatrix(_camera.nearClipPlane, _camera.farClipPlane);
			}
		}

		private void OnSessionSetEnabled(bool sessionEnabled)
		{
			_sessionEnabled = sessionEnabled;
			if (!_sessionEnabled)
			{
				UpdateState();
				UpdateShaderVariables();
			}
		}

		private float CosineLerp(float elapsed, float duration)
		{
			return Mathf.Cos((Mathf.Clamp(elapsed, 0f, duration) / duration - 1f) * ((float)Math.PI / 2f));
		}

		private Vector4 TextureTransform()
		{
			float y = (float)(_transitionImageTexture.width - Screen.width) / (2f * (float)_transitionImageTexture.width);
			float w = (float)(_transitionImageTexture.height - Screen.height) / (2f * (float)_transitionImageTexture.height);
			return new Vector4((float)Screen.width / (float)_transitionImageTexture.width, y, (float)Screen.height / (float)_transitionImageTexture.height, w);
		}

		private void EnableARBackgroundRendering()
		{
			if (BackgroundMaterial == null || _camera == null)
			{
				return;
			}
			_cameraClearFlags = _camera.clearFlags;
			_camera.clearFlags = CameraClearFlags.Depth;
			_commandBuffer = new CommandBuffer();
			if (SystemInfo.graphicsMultiThreaded && !InstantPreviewManager.IsProvidingPlatform)
			{
				_commandBuffer.IssuePluginEvent(ExternApi.ARCoreRenderingUtils_GetRenderEventFunc(), 2);
				ARCoreSession sessionComponent = LifecycleManager.Instance.SessionComponent;
				if (sessionComponent != null && sessionComponent.DeviceCameraDirection == DeviceCameraDirection.FrontFacing)
				{
					_commandBuffer.SetInvertCulling(invertCulling: true);
				}
			}
			_commandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, BackgroundMaterial);
			_camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);
			_camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, _commandBuffer);
		}

		private void DisableARBackgroundRendering()
		{
			if (_commandBuffer != null && !(_camera == null))
			{
				_camera.clearFlags = _cameraClearFlags;
				_camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);
				_camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, _commandBuffer);
			}
		}
	}
	[CreateAssetMenu(fileName = "ARCoreCameraConfigFilter", menuName = "Google ARCore/CameraConfigFilter", order = 2)]
	public class ARCoreCameraConfigFilter : ScriptableObject
	{
		[Serializable]
		public class TargetCameraFramerateFilter
		{
			[Tooltip("Target 30fps camera capture frame rate. Available on all ARCore supported devices.")]
			public bool Target30FPS = true;

			[Tooltip("Target 60fps camera capture frame rate on supported devices.")]
			public bool Target60FPS = true;
		}

		[Serializable]
		public class DepthSensorUsageFilter
		{
			[Tooltip("ARCore requires a depth sensor to be present and will use it. Not supported on all devices.")]
			public bool RequireAndUse = true;

			[Tooltip("ARCore will not use the hardware depth sensor, such as a time-of-flight sensor (or ToF sensor), even if it is present. Available on all supported devices.")]
			public bool DoNotUse = true;
		}

		[Serializable]
		public class StereoCameraUsageFilter
		{
			[Tooltip("ARCore requires a stereo camera to be present on the device. Not available on all ARCore supported devices.")]
			public bool RequireAndUse = true;

			[Tooltip("ARCore will not use the stereo camera, even if it is present. Available on all supported devices.")]
			public bool DoNotUse = true;
		}

		public TargetCameraFramerateFilter TargetCameraFramerate;

		public DepthSensorUsageFilter DepthSensorUsage;

		public StereoCameraUsageFilter StereoCameraUsage;

		public void OnValidate()
		{
			if (!TargetCameraFramerate.Target30FPS && !TargetCameraFramerate.Target60FPS)
			{
				UnityEngine.Debug.LogError("No options in Target Camera Framerate are selected, there will be no camera configs and this app will fail to run.");
			}
			else if (!TargetCameraFramerate.Target30FPS)
			{
				UnityEngine.Debug.LogWarning("Framerate30FPS is not selected, this may cause no camera config be available for this filter and the app may not run on all devices.");
			}
			if (!DepthSensorUsage.DoNotUse && !DepthSensorUsage.RequireAndUse)
			{
				UnityEngine.Debug.LogError("No options in Depth Sensor Usage are selected, there will be no camera configs and this app will fail to run.");
			}
			else if (!DepthSensorUsage.DoNotUse)
			{
				UnityEngine.Debug.LogWarning("DoNotUseDepthSensor is not selected, this may cause no camera config be available for this filter and the app may not run on all devices.");
			}
			if (!StereoCameraUsage.DoNotUse && !StereoCameraUsage.RequireAndUse)
			{
				UnityEngine.Debug.LogError("No options in Stereo Camera Usage are selected, there will be no camera configs and this app will fail to run.");
			}
			else if (!StereoCameraUsage.DoNotUse)
			{
				UnityEngine.Debug.LogWarning("DoNotUseStereoCamera is not selected, this may cause no camera config be available for this filter and the app may not run on all devices.");
			}
		}
	}
	[CreateAssetMenu(fileName = "ARCoreRecordingConfig", menuName = "Google ARCore/ARCore Recording Config", order = 3)]
	public class ARCoreRecordingConfig : ScriptableObject
	{
		public string Mp4DatasetFilepath;

		public bool AutoStopOnPause = true;

		[HideInInspector]
		public List<Track> Tracks = new List<Track>();
	}
	[HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/ARCoreSession")]
	public class ARCoreSession : MonoBehaviour
	{
		public delegate int OnChooseCameraConfigurationDelegate(List<CameraConfig> supportedConfigurations);

		[Tooltip("The direction of the device camera used by the session.")]
		public DeviceCameraDirection DeviceCameraDirection;

		[Tooltip("A scriptable object specifying the ARCore session configuration.")]
		public ARCoreSessionConfig SessionConfig;

		[Tooltip("Configuration options to select the camera mode and features.")]
		public ARCoreCameraConfigFilter CameraConfigFilter;

		private OnChooseCameraConfigurationDelegate _onChooseCameraConfiguration;

		[SuppressMemoryAllocationError(Reason = "Could create new LifecycleManager")]
		public virtual void Awake()
		{
			if (SessionConfig != null && SessionConfig.LightEstimationMode != LightEstimationMode.Disabled && UnityEngine.Object.FindObjectsOfType<EnvironmentalLight>().Length == 0)
			{
				UnityEngine.Debug.Log("Light Estimation may not work properly when EnvironmentalLight is not attached to the scene.");
			}
			LifecycleManager.Instance.CreateSession(this);
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
		public virtual void OnDestroy()
		{
			LifecycleManager.Instance.ResetSession();
		}

		[SuppressMemoryAllocationError(Reason = "Enabling session creates a new ARSessionConfiguration")]
		public void OnEnable()
		{
			LifecycleManager.Instance.EnableSession();
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
		public void OnDisable()
		{
			LifecycleManager.Instance.DisableSession();
		}

		public void OnValidate()
		{
			if (DeviceCameraDirection == DeviceCameraDirection.FrontFacing && SessionConfig != null)
			{
				if (SessionConfig.PlaneFindingMode != DetectedPlaneFindingMode.Disabled)
				{
					UnityEngine.Debug.LogErrorFormat("Plane Finding requires back-facing camera.");
				}
				if (SessionConfig.LightEstimationMode == LightEstimationMode.EnvironmentalHDRWithoutReflections || SessionConfig.LightEstimationMode == LightEstimationMode.EnvironmentalHDRWithReflections)
				{
					UnityEngine.Debug.LogErrorFormat("LightEstimationMode.{0} is incompatible withfront-facing (selfie) camera.", SessionConfig.LightEstimationMode);
				}
				if (SessionConfig.CloudAnchorMode != CloudAnchorMode.Disabled)
				{
					UnityEngine.Debug.LogErrorFormat("Cloud Anchors require back-facing camera.");
				}
				if (SessionConfig.AugmentedImageDatabase != null)
				{
					UnityEngine.Debug.LogErrorFormat("Augmented Images require back-facing camera.");
				}
			}
			if (DeviceCameraDirection == DeviceCameraDirection.BackFacing && SessionConfig != null && SessionConfig.AugmentedFaceMode == AugmentedFaceMode.Mesh)
			{
				UnityEngine.Debug.LogErrorFormat("AugmentedFaceMode.{0} requires front-facing (selfie) camera.", SessionConfig.AugmentedFaceMode);
			}
			if (SessionConfig == null)
			{
				UnityEngine.Debug.LogError("SessionConfig is required by ARCoreSession.");
			}
			if (CameraConfigFilter == null)
			{
				UnityEngine.Debug.LogError("CameraConfigFilter is required by ARCoreSession. To get all available configurations, set CameraConfigFilter to a filter with all options selected.");
			}
		}

		public void RegisterChooseCameraConfigurationCallback(OnChooseCameraConfigurationDelegate onChooseCameraConfiguration)
		{
			_onChooseCameraConfiguration = onChooseCameraConfiguration;
		}

		internal OnChooseCameraConfigurationDelegate GetChooseCameraConfigurationCallback()
		{
			return _onChooseCameraConfiguration;
		}
	}
	[CreateAssetMenu(fileName = "ARCoreSessionConfig", menuName = "Google ARCore/SessionConfig", order = 1)]
	[HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/ARCoreSessionConfig")]
	public class ARCoreSessionConfig : ScriptableObject
	{
		[Header("Performance")]
		[Tooltip("Toggles whether the rendering frame rate matches the background camera frame rate")]
		public bool MatchCameraFramerate = true;

		[FormerlySerializedAs("EnablePlaneFinding")]
		[Header("Plane Finding")]
		[Tooltip("Chooses which plane finding mode will be used.")]
		public DetectedPlaneFindingMode PlaneFindingMode = DetectedPlaneFindingMode.HorizontalAndVertical;

		[Header("Light Estimation")]
		[Tooltip("Chooses which light estimation mode will be used in ARCore session.")]
		[FormerlySerializedAs("EnableLightEstimation")]
		[Help("When \"Environmental HDR Without Reflections\" light is selected, ARCore:\n1. Updates rotation and color of the directional light on the EnvironmentalLight component.\n2. Updates Skybox ambient lighting Spherical Harmonics.\n\nWhen \"Environmental HDR With Reflections\" light is selected, ARCore also:\n3. Overrides the environmental reflections in the scene with a realtime reflections cubemap.", HelpAttribute.HelpMessageType.None)]
		public LightEstimationMode LightEstimationMode = LightEstimationMode.EnvironmentalHDRWithReflections;

		[Header("Cloud Anchors")]
		[Tooltip("Chooses which Cloud Anchors mode will be used in ARCore session.")]
		[FormerlySerializedAs("EnableCloudAnchor")]
		public CloudAnchorMode CloudAnchorMode;

		[Header("Augmented Images")]
		[Tooltip("The database to use for detecting AugmentedImage Trackables.")]
		public AugmentedImageDatabase AugmentedImageDatabase;

		[Header("Camera")]
		[Help("Note, on devices where ARCore does not support auto focus mode due to the use of a fixed focus camera, setting focus mode to Auto Focus will be ignored. Similarly, on devices where tracking requires auto focus mode, seting focus mode to Fixed Focus will be ignored.", HelpAttribute.HelpMessageType.None)]
		[Tooltip("On supported devices, chooses the desired focus mode to be used by the ARCore camera.")]
		public CameraFocusMode CameraFocusMode;

		public AugmentedFaceMode AugmentedFaceMode;

		[Tooltip("Chooses which DepthMode will be used in the ARCore session.")]
		public DepthMode DepthMode;

		[Tooltip("Chooses the desired Instant Placement mode.")]
		[Header("Instant Placement")]
		public InstantPlacementMode InstantPlacementMode;

		[Obsolete("This field has be replaced by ARCoreSessionConfig.DetectedPlaneFindingMode. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.2.0")]
		public bool EnablePlaneFinding
		{
			get
			{
				return PlaneFindingMode != DetectedPlaneFindingMode.Disabled;
			}
			set
			{
				PlaneFindingMode = (value ? DetectedPlaneFindingMode.HorizontalAndVertical : DetectedPlaneFindingMode.Disabled);
			}
		}

		[Obsolete("This field has been replaced by ARCoreSessionConfig.LightEstimationMode. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.10.0")]
		public bool EnableLightEstimation
		{
			get
			{
				return LightEstimationMode != LightEstimationMode.Disabled;
			}
			set
			{
				LightEstimationMode = (value ? LightEstimationMode.AmbientIntensity : LightEstimationMode.Disabled);
			}
		}

		[Obsolete("This field has been replaced by ARCoreSessionConfig.CloudAnchorMode. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.15.0")]
		public bool EnableCloudAnchor
		{
			get
			{
				return CloudAnchorMode != CloudAnchorMode.Disabled;
			}
			set
			{
				CloudAnchorMode = (value ? CloudAnchorMode.Enabled : CloudAnchorMode.Disabled);
			}
		}

		public override bool Equals(object other)
		{
			ARCoreSessionConfig aRCoreSessionConfig = other as ARCoreSessionConfig;
			if (other == null)
			{
				return false;
			}
			if (MatchCameraFramerate != aRCoreSessionConfig.MatchCameraFramerate || PlaneFindingMode != aRCoreSessionConfig.PlaneFindingMode || LightEstimationMode != aRCoreSessionConfig.LightEstimationMode || CloudAnchorMode != aRCoreSessionConfig.CloudAnchorMode || AugmentedImageDatabase != aRCoreSessionConfig.AugmentedImageDatabase || CameraFocusMode != aRCoreSessionConfig.CameraFocusMode || DepthMode != aRCoreSessionConfig.DepthMode || InstantPlacementMode != aRCoreSessionConfig.InstantPlacementMode || AugmentedFaceMode != aRCoreSessionConfig.AugmentedFaceMode)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void CopyFrom(ARCoreSessionConfig other)
		{
			MatchCameraFramerate = other.MatchCameraFramerate;
			PlaneFindingMode = other.PlaneFindingMode;
			LightEstimationMode = other.LightEstimationMode;
			CloudAnchorMode = other.CloudAnchorMode;
			AugmentedImageDatabase = other.AugmentedImageDatabase;
			CameraFocusMode = other.CameraFocusMode;
			AugmentedFaceMode = other.AugmentedFaceMode;
			DepthMode = other.DepthMode;
			InstantPlacementMode = other.InstantPlacementMode;
		}

		public void OnValidate()
		{
			if ((LightEstimationMode == LightEstimationMode.EnvironmentalHDRWithoutReflections || LightEstimationMode == LightEstimationMode.EnvironmentalHDRWithReflections) && AugmentedFaceMode == AugmentedFaceMode.Mesh)
			{
				UnityEngine.Debug.LogErrorFormat("LightEstimationMode.{0} is incompatible with AugmentedFaceMode.{1}, please use other LightEstimationMode or disable Augmented Face.", LightEstimationMode, AugmentedFaceMode);
			}
		}
	}
	public class AsyncTask<T>
	{
		private List<Action<T>> _actionsUponTaskCompletion;

		public bool IsComplete { get; private set; }

		public T Result { get; private set; }

		internal AsyncTask(out Action<T> asyncOperationComplete)
		{
			if (!AsyncTask.IsInitialized)
			{
				AsyncTask.InitAsyncTask();
			}
			IsComplete = false;
			asyncOperationComplete = delegate(T result)
			{
				Result = result;
				IsComplete = true;
				if (_actionsUponTaskCompletion != null)
				{
					AsyncTask.PerformActionInUpdate(delegate
					{
						for (int i = 0; i < _actionsUponTaskCompletion.Count; i++)
						{
							_actionsUponTaskCompletion[i](result);
						}
					});
				}
			};
		}

		internal AsyncTask(T result)
		{
			Result = result;
			IsComplete = true;
		}

		[SuppressMemoryAllocationError(Reason = "Creates a new CustomYieldInstruction")]
		public CustomYieldInstruction WaitForCompletion()
		{
			return new WaitForTaskCompletionYieldInstruction<T>(this);
		}

		[SuppressMemoryAllocationError(Reason = "Could allocate new List")]
		public AsyncTask<T> ThenAction(Action<T> doAfterTaskComplete)
		{
			if (IsComplete)
			{
				doAfterTaskComplete(Result);
				return this;
			}
			if (_actionsUponTaskCompletion == null)
			{
				_actionsUponTaskCompletion = new List<Action<T>>();
			}
			_actionsUponTaskCompletion.Add(doAfterTaskComplete);
			return this;
		}
	}
	internal class AsyncTask
	{
		private static Queue<Action> _updateActionQueue = new Queue<Action>();

		private static object _lockObject = new object();

		public static bool IsInitialized { get; private set; }

		public static void PerformActionInUpdate(Action action)
		{
			lock (_lockObject)
			{
				_updateActionQueue.Enqueue(action);
			}
		}

		public static void OnUpdate()
		{
			lock (_lockObject)
			{
				while (_updateActionQueue.Count > 0)
				{
					_updateActionQueue.Dequeue()();
				}
			}
		}

		public static void InitAsyncTask()
		{
			if (!IsInitialized)
			{
				LifecycleManager.Instance.EarlyUpdate += OnUpdate;
				LifecycleManager.Instance.OnResetInstance += ResetAsyncTask;
				IsInitialized = true;
			}
		}

		public static void ResetAsyncTask()
		{
			if (IsInitialized)
			{
				LifecycleManager.Instance.EarlyUpdate -= OnUpdate;
				IsInitialized = false;
			}
		}
	}
	public class AugmentedFace : Trackable
	{
		public Pose CenterPose
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("CenterPose:: Trying to access a session that has already been destroyed.");
					return default(Pose);
				}
				return _nativeSession.AugmentedFaceApi.GetCenterPose(_trackableNativeHandle);
			}
		}

		internal AugmentedFace(IntPtr nativeHandle, NativeSession nativeApi)
			: base(nativeHandle, nativeApi)
		{
			_trackableNativeHandle = nativeHandle;
			_nativeSession = nativeApi;
		}

		public Pose GetRegionPose(AugmentedFaceRegion region)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("GetRegionPose: Trying to access a session that has already been destroyed.");
				return default(Pose);
			}
			return _nativeSession.AugmentedFaceApi.GetRegionPose(_trackableNativeHandle, (ApiAugmentedFaceRegionType)region);
		}

		public void GetVertices(List<Vector3> vertices)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("GetVertices:: Trying to access a session that has already been destroyed.");
			}
			else
			{
				_nativeSession.AugmentedFaceApi.GetVertices(_trackableNativeHandle, vertices);
			}
		}

		public void GetTextureCoordinates(List<Vector2> textureCoordinates)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("GetTextureCoordinates:: Trying to access a session that has already been destroyed.");
			}
			else
			{
				_nativeSession.AugmentedFaceApi.GetTextureCoordinates(_trackableNativeHandle, textureCoordinates);
			}
		}

		public void GetNormals(List<Vector3> normals)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("GetNormals:: Trying to access a session that has already been destroyed.");
			}
			else
			{
				_nativeSession.AugmentedFaceApi.GetNormals(_trackableNativeHandle, normals);
			}
		}

		public void GetTriangleIndices(List<int> indices)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("GetTriangleIndices:: Trying to access a session that has already been destroyed.");
			}
			else
			{
				_nativeSession.AugmentedFaceApi.GetTriangleIndices(_trackableNativeHandle, indices);
			}
		}
	}
	public enum AugmentedFaceMode
	{
		Disabled = 0,
		Mesh = 2
	}
	public enum AugmentedFaceRegion
	{
		NoseTip,
		ForeheadLeft,
		ForeheadRight
	}
	public class AugmentedImage : Trackable
	{
		public int DatabaseIndex => _nativeSession.AugmentedImageApi.GetDatabaseIndex(_trackableNativeHandle);

		public string Name
		{
			[SuppressMemoryAllocationError(IsWarning = true, Reason = "Allocates new string")]
			get
			{
				return _nativeSession.AugmentedImageApi.GetName(_trackableNativeHandle);
			}
		}

		public Pose CenterPose => _nativeSession.AugmentedImageApi.GetCenterPose(_trackableNativeHandle);

		public float ExtentX => _nativeSession.AugmentedImageApi.GetExtentX(_trackableNativeHandle);

		public float ExtentZ => _nativeSession.AugmentedImageApi.GetExtentZ(_trackableNativeHandle);

		public AugmentedImageTrackingMethod TrackingMethod => _nativeSession.AugmentedImageApi.GetTrackingMethod(_trackableNativeHandle);

		internal AugmentedImage(IntPtr nativeHandle, NativeSession nativeApi)
			: base(nativeHandle, nativeApi)
		{
		}
	}
	public class AugmentedImageDatabase : ScriptableObject
	{
		private IntPtr _arAugmentedImageDatabase = IntPtr.Zero;

		[SerializeField]
		[FormerlySerializedAs("m_Images")]
		private List<AugmentedImageDatabaseEntry> _images = new List<AugmentedImageDatabaseEntry>();

		[SerializeField]
		[FormerlySerializedAs("m_RawData")]
		private byte[] _rawData;

		[SerializeField]
		[FormerlySerializedAs("m_IsRawDataDirty")]
		private bool _isRawDataDirty = true;

		[SerializeField]
		[FormerlySerializedAs("m_CliVersion")]
		private string _cliVersion = string.Empty;

		public int Count
		{
			get
			{
				lock (_images)
				{
					return _images.Count;
				}
			}
		}

		internal bool _isDirty { get; private set; }

		internal IntPtr _nativeHandle
		{
			get
			{
				if (_arAugmentedImageDatabase == IntPtr.Zero)
				{
					NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
					if (nativeSession == null || InstantPreviewManager.IsProvidingPlatform)
					{
						return IntPtr.Zero;
					}
					_arAugmentedImageDatabase = nativeSession.AugmentedImageDatabaseApi.Create(_rawData);
				}
				_isDirty = false;
				return _arAugmentedImageDatabase;
			}
			private set
			{
				_arAugmentedImageDatabase = value;
			}
		}

		public AugmentedImageDatabaseEntry this[int index]
		{
			get
			{
				lock (_images)
				{
					return _images[index];
				}
			}
		}

		public AugmentedImageDatabase()
		{
			_isDirty = true;
		}

		[SuppressMemoryAllocationError(Reason = "Allocates memory for the image.")]
		public int AddImage(string name, Texture2D image, float width = 0f)
		{
			return AddImage(name, new AugmentedImageSrc(image), width);
		}

		[SuppressMemoryAllocationError(Reason = "Allocates memory for the image.")]
		public int AddImage(string name, AugmentedImageSrc imageSrc, float width = 0f)
		{
			NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
			if (nativeSession == null)
			{
				return -1;
			}
			int num = nativeSession.AugmentedImageDatabaseApi.AddAugmentedImageAtRuntime(_nativeHandle, name, imageSrc, width);
			if (num != -1)
			{
				lock (_images)
				{
					_images.Add(new AugmentedImageDatabaseEntry(name, width));
					_isDirty = true;
				}
			}
			return num;
		}
	}
	[Serializable]
	public struct AugmentedImageDatabaseEntry
	{
		public string Name;

		public float Width;

		public string Quality;

		public string TextureGUID;

		public string LastModifiedTime;

		public AugmentedImageDatabaseEntry(string name, float width)
		{
			Name = name;
			TextureGUID = string.Empty;
			Width = width;
			LastModifiedTime = string.Empty;
			Quality = string.Empty;
		}
	}
	public class AugmentedImageSrc
	{
		internal TextureFormat _format { get; private set; }

		internal Color[] _pixels { get; private set; }

		internal int _height { get; private set; }

		internal int _width { get; private set; }

		public AugmentedImageSrc(Texture2D image)
		{
			_format = image.format;
			_pixels = image.GetPixels();
			_height = image.height;
			_width = image.width;
		}
	}
	public enum AugmentedImageTrackingMethod
	{
		NotTracking,
		FullTracking,
		LastKnownPose
	}
	[Flags]
	public enum CameraConfigDepthSensorUsage
	{
		RequireAndUse = 1,
		DoNotUse = 2
	}
	[Flags]
	public enum CameraConfigStereoCameraUsage
	{
		RequireAndUse = 1,
		DoNotUse = 2
	}
	public struct CameraConfig
	{
		public DeviceCameraDirection FacingDirection { get; private set; }

		public Vector2 ImageSize { get; private set; }

		public Vector2 TextureSize { get; private set; }

		public int MinFPS { get; private set; }

		public int MaxFPS { get; private set; }

		public CameraConfigStereoCameraUsage StereoCameraUsage { get; private set; }

		public CameraConfigDepthSensorUsage DepthSensorUsage { get; private set; }

		internal CameraConfig(DeviceCameraDirection facingDirection, Vector2 imageSize, Vector2 textureSize, int minFPS, int maxFPS, CameraConfigStereoCameraUsage stereoCamera, CameraConfigDepthSensorUsage depthSensor)
		{
			this = default(CameraConfig);
			FacingDirection = facingDirection;
			ImageSize = imageSize;
			TextureSize = textureSize;
			MinFPS = minFPS;
			MaxFPS = maxFPS;
			DepthSensorUsage = depthSensor;
			StereoCameraUsage = stereoCamera;
		}
	}
	public enum CameraFocusMode
	{
		FixedFocus,
		AutoFocus
	}
	public struct CameraImageBytes : IDisposable
	{
		private IntPtr _imageHandle;

		public bool IsAvailable { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public IntPtr Y { get; private set; }

		public IntPtr U { get; private set; }

		public IntPtr V { get; private set; }

		public int YRowStride { get; private set; }

		public int UVPixelStride { get; private set; }

		public int UVRowStride { get; private set; }

		internal CameraImageBytes(IntPtr imageHandle)
		{
			this = default(CameraImageBytes);
			_imageHandle = imageHandle;
			if (_imageHandle != IntPtr.Zero)
			{
				IntPtr surfaceData2;
				IntPtr surfaceData3;
				IntPtr surfaceData = (surfaceData2 = (surfaceData3 = IntPtr.Zero));
				int dataLength = 0;
				IsAvailable = true;
				Width = LifecycleManager.Instance.NativeSession.ImageApi.GetWidth(imageHandle);
				Height = LifecycleManager.Instance.NativeSession.ImageApi.GetHeight(imageHandle);
				LifecycleManager.Instance.NativeSession.ImageApi.GetPlaneData(imageHandle, 0, ref surfaceData, ref dataLength);
				LifecycleManager.Instance.NativeSession.ImageApi.GetPlaneData(imageHandle, 1, ref surfaceData2, ref dataLength);
				LifecycleManager.Instance.NativeSession.ImageApi.GetPlaneData(imageHandle, 2, ref surfaceData3, ref dataLength);
				YRowStride = LifecycleManager.Instance.NativeSession.ImageApi.GetPlaneRowStride(imageHandle, 0);
				UVPixelStride = LifecycleManager.Instance.NativeSession.ImageApi.GetPlanePixelStride(imageHandle, 1);
				UVRowStride = LifecycleManager.Instance.NativeSession.ImageApi.GetPlaneRowStride(imageHandle, 1);
				Y = surfaceData;
				U = surfaceData2;
				V = surfaceData3;
			}
			else
			{
				IsAvailable = false;
				int width = (Height = 0);
				Width = width;
				IntPtr intPtr = (V = IntPtr.Zero);
				IntPtr y = (U = intPtr);
				Y = y;
				int num2 = (UVRowStride = 0);
				width = (UVPixelStride = num2);
				YRowStride = width;
			}
		}

		public void Release()
		{
			if (_imageHandle != IntPtr.Zero)
			{
				LifecycleManager.Instance.NativeSession.ImageApi.Release(_imageHandle);
				_imageHandle = IntPtr.Zero;
			}
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
		public void Dispose()
		{
			Release();
		}
	}
	public struct CameraIntrinsics
	{
		public Vector2 FocalLength;

		public Vector2 PrincipalPoint;

		public Vector2Int ImageDimensions;

		internal CameraIntrinsics(Vector2 focalLength, Vector2 principalPoint, Vector2Int imageDimensions)
		{
			FocalLength = focalLength;
			PrincipalPoint = principalPoint;
			ImageDimensions = imageDimensions;
		}
	}
	public enum CameraMetadataTag
	{
		SectionColorCorrection = 0,
		SectionControl = 1,
		SectionEdge = 3,
		SectionFlash = 4,
		SectionFlashInfo = 5,
		SectionHotPixel = 6,
		SectionJpeg = 7,
		SectionLens = 8,
		SectionLensInfo = 9,
		SectionNoiseReduction = 10,
		SectionRequest = 12,
		SectionScaler = 13,
		SectionSensor = 14,
		SectionSensorInfo = 15,
		SectionShading = 16,
		SectionStatistics = 17,
		SectionStatisticsInfo = 18,
		SectionTonemap = 19,
		SectionInfo = 21,
		SectionBlackLevel = 22,
		SectionSync = 23,
		SectionDepth = 25,
		ColorCorrectionStart = 0,
		ControlStart = 65536,
		EdgeStart = 196608,
		FlashStart = 262144,
		FlashInfoStart = 327680,
		HotPixelStart = 393216,
		JpegStart = 458752,
		LensStart = 524288,
		LensInfoStart = 589824,
		NoiseReductionStart = 655360,
		RequestStart = 786432,
		ScalerStart = 851968,
		SensorStart = 917504,
		SensorInfoStart = 983040,
		ShadingStart = 1048576,
		StatisticsStart = 1114112,
		StatisticsInfoStart = 1179648,
		TonemapStart = 1245184,
		InfoStart = 1376256,
		BlackLevelStart = 1441792,
		SyncStart = 1507328,
		DepthStart = 1638400,
		ColorCorrectionMode = 0,
		ColorCorrectionTransform = 1,
		ColorCorrectionGains = 2,
		ColorCorrectionAberrationMode = 3,
		ControlAeAntibandingMode = 65536,
		ControlAeExposureCompensation = 65537,
		ControlAeLock = 65538,
		ControlAeMode = 65539,
		ControlAeRegions = 65540,
		ControlAeTargetFpsRange = 65541,
		ControlAePrecaptureTrigger = 65542,
		ControlAfMode = 65543,
		ControlAfRegions = 65544,
		ControlAfTrigger = 65545,
		ControlAwbLock = 65546,
		ControlAwbMode = 65547,
		ControlAwbRegions = 65548,
		ControlCaptureIntent = 65549,
		ControlEffectMode = 65550,
		ControlMode = 65551,
		ControlSceneMode = 65552,
		ControlVideoStabilizationMode = 65553,
		ControlAeState = 65567,
		ControlAfState = 65568,
		ControlAwbState = 65570,
		ControlPostRawSensitivityBoost = 65576,
		EdgeMode = 196608,
		FlashMode = 262146,
		FlashState = 262149,
		HotPixelMode = 393216,
		JpegGpsCoordinates = 458752,
		JpegGpsProcessingMethod = 458753,
		JpegGpsTimestamp = 458754,
		JpegOrientation = 458755,
		JpegQuality = 458756,
		JpegThumbnailQuality = 458757,
		JpegThumbnailSize = 458758,
		LensAperture = 524288,
		LensFilterDensity = 524289,
		LensFocalLength = 524290,
		LensFocusDistance = 524291,
		LensOpticalStabilizationMode = 524292,
		LensPoseRotation = 524294,
		LensPoseTranslation = 524295,
		LensFocusRange = 524296,
		LensState = 524297,
		LensIntrinsicCalibration = 524298,
		LensRadialDistortion = 524299,
		NoiseReductionMode = 655360,
		RequestPipelineDepth = 786441,
		ScalerCropRegion = 851968,
		SensorExposureTime = 917504,
		SensorFrameDuration = 917505,
		SensorSensitivity = 917506,
		SensorTimestamp = 917520,
		SensorNeutralColorPoint = 917522,
		SensorNoiseProfile = 917523,
		SensorGreenSplit = 917526,
		SensorTestPatternData = 917527,
		SensorTestPatternMode = 917528,
		SensorRollingShutterSkew = 917530,
		SensorDynamicBlackLevel = 917532,
		SensorDynamicWhiteLevel = 917533,
		ShadingMode = 1048576,
		StatisticsFaceDetectMode = 1114112,
		StatisticsHotPixelMapMode = 1114115,
		StatisticsFaceIds = 1114116,
		StatisticsFaceLandmarks = 1114117,
		StatisticsFaceRectangles = 1114118,
		StatisticsFaceScores = 1114119,
		StatisticsLensShadingMap = 1114123,
		StatisticsSceneFlicker = 1114126,
		StatisticsHotPixelMap = 1114127,
		StatisticsLensShadingMapMode = 1114128,
		TonemapCurveBlue = 1245184,
		TonemapCurveGreen = 1245185,
		TonemapCurveRed = 1245186,
		TonemapMode = 1245187,
		TonemapGamma = 1245190,
		TonemapPresetCurve = 1245191,
		BlackLevelLock = 1441792,
		SyncFrameNumber = 1507328
	}
	[StructLayout(LayoutKind.Explicit)]
	public struct CameraMetadataValue
	{
		[FieldOffset(0)]
		private ArCameraMetadataType _type;

		[FieldOffset(4)]
		private sbyte _byteValue;

		[FieldOffset(4)]
		private int _intValue;

		[FieldOffset(4)]
		private long _longValue;

		[FieldOffset(4)]
		private float _floatValue;

		[FieldOffset(4)]
		private double _doubleValue;

		[FieldOffset(4)]
		private CameraMetadataRational _rationalValue;

		public Type ValueType => _type switch
		{
			ArCameraMetadataType.Byte => typeof(byte), 
			ArCameraMetadataType.Int32 => typeof(int), 
			ArCameraMetadataType.Float => typeof(float), 
			ArCameraMetadataType.Int64 => typeof(long), 
			ArCameraMetadataType.Double => typeof(double), 
			ArCameraMetadataType.Rational => typeof(CameraMetadataRational), 
			_ => null, 
		};

		public CameraMetadataValue(sbyte byteValue)
		{
			_intValue = 0;
			_longValue = 0L;
			_floatValue = 0f;
			_doubleValue = 0.0;
			_rationalValue = default(CameraMetadataRational);
			_type = ArCameraMetadataType.Byte;
			_byteValue = byteValue;
		}

		public CameraMetadataValue(int intValue)
		{
			_byteValue = 0;
			_longValue = 0L;
			_floatValue = 0f;
			_doubleValue = 0.0;
			_rationalValue = default(CameraMetadataRational);
			_type = ArCameraMetadataType.Int32;
			_intValue = intValue;
		}

		public CameraMetadataValue(long longValue)
		{
			_byteValue = 0;
			_intValue = 0;
			_floatValue = 0f;
			_doubleValue = 0.0;
			_rationalValue = default(CameraMetadataRational);
			_type = ArCameraMetadataType.Int64;
			_longValue = longValue;
		}

		public CameraMetadataValue(float floatValue)
		{
			_byteValue = 0;
			_intValue = 0;
			_longValue = 0L;
			_doubleValue = 0.0;
			_rationalValue = default(CameraMetadataRational);
			_type = ArCameraMetadataType.Float;
			_floatValue = floatValue;
		}

		public CameraMetadataValue(double doubleValue)
		{
			_byteValue = 0;
			_intValue = 0;
			_longValue = 0L;
			_floatValue = 0f;
			_rationalValue = default(CameraMetadataRational);
			_type = ArCameraMetadataType.Double;
			_doubleValue = doubleValue;
		}

		public CameraMetadataValue(CameraMetadataRational rationalValue)
		{
			_byteValue = 0;
			_intValue = 0;
			_longValue = 0L;
			_floatValue = 0f;
			_doubleValue = 0.0;
			_type = ArCameraMetadataType.Rational;
			_rationalValue = rationalValue;
		}

		public sbyte AsByte()
		{
			if (_type != ArCameraMetadataType.Byte)
			{
				LogError(ArCameraMetadataType.Byte);
			}
			return _byteValue;
		}

		public int AsInt()
		{
			if (_type != ArCameraMetadataType.Int32)
			{
				LogError(ArCameraMetadataType.Int32);
			}
			return _intValue;
		}

		public float AsFloat()
		{
			if (_type != ArCameraMetadataType.Float)
			{
				LogError(ArCameraMetadataType.Float);
			}
			return _floatValue;
		}

		public long AsLong()
		{
			if (_type != ArCameraMetadataType.Int64)
			{
				LogError(ArCameraMetadataType.Int64);
			}
			return _longValue;
		}

		public double AsDouble()
		{
			if (_type != ArCameraMetadataType.Double)
			{
				LogError(ArCameraMetadataType.Double);
			}
			return _doubleValue;
		}

		public CameraMetadataRational AsRational()
		{
			if (_type != ArCameraMetadataType.Rational)
			{
				LogError(ArCameraMetadataType.Rational);
			}
			return _rationalValue;
		}

		private void LogError(ArCameraMetadataType requestedType)
		{
			ARDebug.LogErrorFormat("Error getting value from CameraMetadataType due to type mismatch. requested type = {0}, internal type = {1}\nAre you sure you are querying the correct type?", requestedType, _type);
		}
	}
	public struct CameraMetadataRational
	{
		public int Numerator;

		public int Denominator;
	}
	public enum CloudAnchorMode
	{
		Disabled,
		Enabled
	}
	[Obsolete("This class has been renamed to DetectedPlane. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.2.0")]
	public class TrackedPlane : DetectedPlane
	{
		public new TrackedPlane SubsumedBy => (TrackedPlane)base.SubsumedBy;

		internal TrackedPlane(IntPtr nativeHandle, NativeSession nativeApi)
			: base(nativeHandle, nativeApi)
		{
		}
	}
	[Obsolete("This class has been renamed to FeaturePoint. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.2.0")]
	public class TrackedPoint : FeaturePoint
	{
		public new TrackedPointOrientationMode OrientationMode => (TrackedPointOrientationMode)base.OrientationMode;

		internal TrackedPoint(IntPtr nativeHandle, NativeSession nativeApi)
			: base(nativeHandle, nativeApi)
		{
		}
	}
	[Obsolete("This enum has been renamed to FeaturePointOrientationMode. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.2.0")]
	public enum TrackedPointOrientationMode
	{
		Identity,
		SurfaceNormal
	}
	public enum DepthMode
	{
		Disabled = 0,
		Automatic = 1,
		RawDepthOnly = 3
	}
	public class DepthPoint : Trackable
	{
		public Pose Pose
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("Pose:: Trying to access a session that has already been destroyed.");
					return default(Pose);
				}
				return _nativeSession.PointApi.GetPose(_trackableNativeHandle);
			}
		}

		internal DepthPoint(IntPtr nativeHandle, NativeSession nativeSession)
			: base(nativeHandle, nativeSession)
		{
		}
	}
	public enum DepthStatus
	{
		Success,
		InternalError,
		NotYetAvailable,
		NotTracking,
		IllegalState
	}
	public class DetectedPlane : Trackable
	{
		public DetectedPlane SubsumedBy
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("SubsumedBy:: Trying to access a session that has already been destroyed.");
					return null;
				}
				return _nativeSession.PlaneApi.GetSubsumedBy(_trackableNativeHandle);
			}
		}

		public Pose CenterPose
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("CenterPose:: Trying to access a session that has already been destroyed.");
					return default(Pose);
				}
				return _nativeSession.PlaneApi.GetCenterPose(_trackableNativeHandle);
			}
		}

		public float ExtentX
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("ExtentX:: Trying to access a session that has already been destroyed.");
					return 0f;
				}
				return _nativeSession.PlaneApi.GetExtentX(_trackableNativeHandle);
			}
		}

		public float ExtentZ
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("ExtentZ:: Trying to access a session that has already been destroyed.");
					return 0f;
				}
				return _nativeSession.PlaneApi.GetExtentZ(_trackableNativeHandle);
			}
		}

		public DetectedPlaneType PlaneType
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("PlaneType:: Trying to access a session that has already been destroyed.");
					return DetectedPlaneType.HorizontalUpwardFacing;
				}
				return _nativeSession.PlaneApi.GetPlaneType(_trackableNativeHandle);
			}
		}

		internal DetectedPlane(IntPtr nativeHandle, NativeSession nativeApi)
			: base(nativeHandle, nativeApi)
		{
			_trackableNativeHandle = nativeHandle;
			_nativeSession = nativeApi;
		}

		[SuppressMemoryAllocationError(Reason = "List could be resized.")]
		public void GetBoundaryPolygon(List<Vector3> boundaryPolygonPoints)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("GetBoundaryPolygon:: Trying to access a session that has already been destroyed.");
			}
			else
			{
				_nativeSession.PlaneApi.GetPolygon(_trackableNativeHandle, boundaryPolygonPoints);
			}
		}
	}
	public enum DetectedPlaneFindingMode
	{
		Disabled,
		HorizontalAndVertical,
		Horizontal,
		Vertical
	}
	public enum DetectedPlaneType
	{
		HorizontalUpwardFacing,
		HorizontalDownwardFacing,
		Vertical
	}
	public enum DeviceCameraDirection
	{
		BackFacing,
		FrontFacing
	}
	public enum DisplayUvCoordinateType
	{
		BackgroundTexture,
		BackgroundImage,
		UnityScreen
	}
	internal static class DisplayUvCoordinateTypeExtension
	{
		public static ApiCoordinates2dType ToApiCoordinates2dType(this DisplayUvCoordinateType coordinateType)
		{
			return coordinateType switch
			{
				DisplayUvCoordinateType.BackgroundImage => ApiCoordinates2dType.ImageNormalized, 
				DisplayUvCoordinateType.BackgroundTexture => ApiCoordinates2dType.TextureNormalized, 
				DisplayUvCoordinateType.UnityScreen => ApiCoordinates2dType.ViewNormalized, 
				_ => ApiCoordinates2dType.ViewNormalized, 
			};
		}
	}
	public struct DisplayUvCoords
	{
		public static readonly DisplayUvCoords FullScreenUvCoords = new DisplayUvCoords(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(1f, 0f));

		public Vector2 TopLeft;

		public Vector2 TopRight;

		public Vector2 BottomLeft;

		public Vector2 BottomRight;

		public DisplayUvCoords(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
		{
			TopLeft = topLeft;
			TopRight = topRight;
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
		}

		[Obsolete("The type ApiDisplayCoords is deprecated.  Please use GoogleARCore.DisplayUvCoords.")]
		public static implicit operator ApiDisplayUvCoords(DisplayUvCoords coords)
		{
			return new ApiDisplayUvCoords(coords.TopLeft, coords.TopRight, coords.BottomLeft, coords.BottomRight);
		}
	}
	[HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/EnvironmentalLight")]
	[ExecuteInEditMode]
	public class EnvironmentalLight : MonoBehaviour
	{
		public Light DirectionalLight;

		private long _lightEstimateTimestamp = -1L;

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
		public void Update()
		{
			if (Application.isEditor && (!Application.isPlaying || !ARCoreProjectSettings.Instance.IsInstantPreviewEnabled))
			{
				Shader.SetGlobalColor("_GlobalColorCorrection", Color.white);
				Shader.SetGlobalFloat("_GlobalLightEstimation", 1f);
				return;
			}
			LightEstimate lightEstimate = Frame.LightEstimate;
			if (lightEstimate.State != LightEstimateState.Valid || lightEstimate.Mode == LightEstimationMode.Disabled)
			{
				return;
			}
			if (lightEstimate.Mode == LightEstimationMode.AmbientIntensity)
			{
				float num = lightEstimate.PixelIntensity / 0.466f;
				Shader.SetGlobalColor("_GlobalColorCorrection", lightEstimate.ColorCorrection * num);
				Shader.SetGlobalFloat("_GlobalLightEstimation", num);
			}
			else
			{
				if (_lightEstimateTimestamp == lightEstimate.Timestamp)
				{
					return;
				}
				_lightEstimateTimestamp = lightEstimate.Timestamp;
				if (DirectionalLight != null)
				{
					if (!DirectionalLight.gameObject.activeSelf || !DirectionalLight.enabled)
					{
						DirectionalLight.gameObject.SetActive(value: true);
						DirectionalLight.enabled = true;
					}
					DirectionalLight.transform.rotation = lightEstimate.DirectionalLightRotation;
					DirectionalLight.color = lightEstimate.DirectionalLightColor;
				}
				RenderSettings.ambientMode = AmbientMode.Skybox;
				RenderSettings.ambientProbe = lightEstimate.AmbientProbe;
				if (lightEstimate.Mode == LightEstimationMode.EnvironmentalHDRWithReflections)
				{
					RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
					RenderSettings.customReflection = lightEstimate.ReflectionProbe;
				}
			}
		}
	}
	public class FeaturePoint : Trackable
	{
		public Pose Pose
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("Pose:: Trying to access a session that has already been destroyed.");
					return default(Pose);
				}
				return _nativeSession.PointApi.GetPose(_trackableNativeHandle);
			}
		}

		public FeaturePointOrientationMode OrientationMode
		{
			[SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("OrientationMode:: Trying to access a session that has already been destroyed.");
					return FeaturePointOrientationMode.Identity;
				}
				return _nativeSession.PointApi.GetOrientationMode(_trackableNativeHandle);
			}
		}

		internal FeaturePoint(IntPtr nativeHandle, NativeSession nativeApi)
			: base(nativeHandle, nativeApi)
		{
		}
	}
	public enum FeaturePointOrientationMode
	{
		Identity,
		SurfaceNormal
	}
	public class Frame
	{
		public static class CameraMetadata
		{
			public static bool TryGetValues(CameraMetadataTag metadataTag, List<CameraMetadataValue> outMetadataList)
			{
				outMetadataList.Clear();
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				if (nativeSession == null)
				{
					return false;
				}
				IntPtr imageMetadataHandle = IntPtr.Zero;
				if (!nativeSession.FrameApi.AcquireImageMetadata(ref imageMetadataHandle))
				{
					return false;
				}
				bool result = nativeSession.CameraMetadataApi.TryGetValues(imageMetadataHandle, metadataTag, outMetadataList);
				nativeSession.CameraMetadataApi.Release(imageMetadataHandle);
				return result;
			}

			public static bool GetAllCameraMetadataTags(List<CameraMetadataTag> outMetadataTags)
			{
				outMetadataTags.Clear();
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				if (nativeSession == null)
				{
					return false;
				}
				IntPtr imageMetadataHandle = IntPtr.Zero;
				if (!nativeSession.FrameApi.AcquireImageMetadata(ref imageMetadataHandle))
				{
					return false;
				}
				bool allCameraMetadataTags = nativeSession.CameraMetadataApi.GetAllCameraMetadataTags(imageMetadataHandle, outMetadataTags);
				nativeSession.CameraMetadataApi.Release(imageMetadataHandle);
				return allCameraMetadataTags;
			}
		}

		public static class PointCloud
		{
			public static bool IsUpdatedThisFrame
			{
				get
				{
					if (LifecycleManager.Instance.IsSessionChangedThisFrame)
					{
						return true;
					}
					return LifecycleManager.Instance.NativeSession?.IsPointCloudNew ?? false;
				}
			}

			public static int PointCount
			{
				get
				{
					NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
					return nativeSession?.PointCloudApi.GetNumberOfPoints(nativeSession.PointCloudHandle) ?? 0;
				}
			}

			[Obsolete("Frame.PointCloud.GetPoint has been deprecated. Please use Frame.PointCloud.GetPointAsStruct instead.")]
			public static Vector4 GetPoint(int index)
			{
				PointCloudPoint pointAsStruct = GetPointAsStruct(index);
				return new Vector4(pointAsStruct.Position.x, pointAsStruct.Position.y, pointAsStruct.Position.z, pointAsStruct.Confidence);
			}

			public static PointCloudPoint GetPointAsStruct(int index)
			{
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				if (nativeSession == null || index >= PointCount)
				{
					return new PointCloudPoint(-1, Vector3.zero, 0f);
				}
				return nativeSession.PointCloudApi.GetPoint(nativeSession.PointCloudHandle, index);
			}

			[Obsolete("Frame.PointCloud.CopyPoints has been deprecated. Please copy points manually instead.")]
			public static void CopyPoints(List<Vector4> points)
			{
				points.Clear();
				if (LifecycleManager.Instance.NativeSession != null)
				{
					for (int i = 0; i < PointCount; i++)
					{
						PointCloudPoint pointAsStruct = GetPointAsStruct(i);
						points.Add(new Vector4(pointAsStruct.Position.x, pointAsStruct.Position.y, pointAsStruct.Position.z, pointAsStruct.Confidence));
					}
				}
			}
		}

		public static class CameraImage
		{
			public static Texture Texture
			{
				get
				{
					NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
					if (nativeSession == null || nativeSession.FrameApi.GetTimestamp() == 0L)
					{
						return null;
					}
					return ARCoreAndroidLifecycleManager.Instance.BackgroundTexture;
				}
			}

			[Obsolete("This field has been deprecated. Please use Frame.CameraImage.TextureDisplayUvs.")]
			public static DisplayUvCoords DisplayUvCoords => TextureDisplayUvs;

			public static DisplayUvCoords TextureDisplayUvs
			{
				get
				{
					DisplayUvCoords fullScreenUvCoords = DisplayUvCoords.FullScreenUvCoords;
					if (InstantPreviewManager.IsProvidingPlatform)
					{
						NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
						if (nativeSession == null)
						{
							return default(DisplayUvCoords);
						}
						ApiDisplayUvCoords uv = new ApiDisplayUvCoords(fullScreenUvCoords.TopLeft, fullScreenUvCoords.TopRight, fullScreenUvCoords.BottomLeft, fullScreenUvCoords.BottomRight);
						nativeSession.FrameApi.TransformDisplayUvCoords(ref uv);
						return uv.ToDisplayUvCoords();
					}
					fullScreenUvCoords.TopLeft = TransformCoordinate(fullScreenUvCoords.TopLeft, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundTexture);
					fullScreenUvCoords.TopRight = TransformCoordinate(fullScreenUvCoords.TopRight, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundTexture);
					fullScreenUvCoords.BottomLeft = TransformCoordinate(fullScreenUvCoords.BottomLeft, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundTexture);
					fullScreenUvCoords.BottomRight = TransformCoordinate(fullScreenUvCoords.BottomRight, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundTexture);
					return fullScreenUvCoords;
				}
			}

			public static DisplayUvCoords ImageDisplayUvs
			{
				get
				{
					if (InstantPreviewManager.IsProvidingPlatform)
					{
						InstantPreviewManager.LogLimitedSupportMessage("access CPU image display UVs");
						return DisplayUvCoords.FullScreenUvCoords;
					}
					DisplayUvCoords fullScreenUvCoords = DisplayUvCoords.FullScreenUvCoords;
					fullScreenUvCoords.TopLeft = TransformCoordinate(fullScreenUvCoords.TopLeft, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundImage);
					fullScreenUvCoords.TopRight = TransformCoordinate(fullScreenUvCoords.TopRight, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundImage);
					fullScreenUvCoords.BottomLeft = TransformCoordinate(fullScreenUvCoords.BottomLeft, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundImage);
					fullScreenUvCoords.BottomRight = TransformCoordinate(fullScreenUvCoords.BottomRight, DisplayUvCoordinateType.UnityScreen, DisplayUvCoordinateType.BackgroundImage);
					return fullScreenUvCoords;
				}
			}

			public static CameraIntrinsics TextureIntrinsics
			{
				get
				{
					NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
					if (nativeSession == null)
					{
						return default(CameraIntrinsics);
					}
					IntPtr cameraHandle = nativeSession.FrameApi.AcquireCamera();
					CameraIntrinsics textureIntrinsics = nativeSession.CameraApi.GetTextureIntrinsics(cameraHandle);
					nativeSession.CameraApi.Release(cameraHandle);
					return textureIntrinsics;
				}
			}

			public static CameraIntrinsics ImageIntrinsics
			{
				get
				{
					NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
					if (nativeSession == null)
					{
						return default(CameraIntrinsics);
					}
					IntPtr cameraHandle = nativeSession.FrameApi.AcquireCamera();
					CameraIntrinsics imageIntrinsics = nativeSession.CameraApi.GetImageIntrinsics(cameraHandle);
					nativeSession.CameraApi.Release(cameraHandle);
					return imageIntrinsics;
				}
			}

			public static Vector2 TransformCoordinate(Vector2 coordinate, DisplayUvCoordinateType sourceType, DisplayUvCoordinateType targetType)
			{
				if (InstantPreviewManager.IsProvidingPlatform)
				{
					InstantPreviewManager.LogLimitedSupportMessage("access 'Frame.TransformCoordinate'");
					return Vector2.zero;
				}
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				if (nativeSession == null)
				{
					UnityEngine.Debug.LogError("Cannot transform coordinate when native session is null.");
					return Vector2.zero;
				}
				nativeSession.FrameApi.TransformCoordinates2d(ref coordinate, sourceType, targetType);
				return coordinate;
			}

			public static CameraImageBytes AcquireCameraImageBytes()
			{
				return LifecycleManager.Instance.NativeSession?.FrameApi.AcquireCameraImageBytes() ?? new CameraImageBytes(IntPtr.Zero);
			}

			public static Matrix4x4 GetCameraProjectionMatrix(float nearClipping, float farClipping)
			{
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				if (nativeSession == null || Texture == null)
				{
					return Matrix4x4.identity;
				}
				IntPtr cameraHandle = nativeSession.FrameApi.AcquireCamera();
				Matrix4x4 projectionMatrix = nativeSession.CameraApi.GetProjectionMatrix(cameraHandle, nearClipping, farClipping);
				nativeSession.CameraApi.Release(cameraHandle);
				return projectionMatrix;
			}

			public static DepthStatus UpdateDepthTexture(ref Texture2D depthTexture)
			{
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				ARCoreSession sessionComponent = LifecycleManager.Instance.SessionComponent;
				if (nativeSession == null || sessionComponent == null || sessionComponent.SessionConfig.DepthMode == DepthMode.Disabled)
				{
					return DepthStatus.InternalError;
				}
				return nativeSession.FrameApi.UpdateDepthTexture(ref depthTexture);
			}

			public static DepthStatus UpdateRawDepthTexture(ref Texture2D depthTexture)
			{
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				ARCoreSession sessionComponent = LifecycleManager.Instance.SessionComponent;
				if (nativeSession == null || sessionComponent == null || sessionComponent.SessionConfig.DepthMode == DepthMode.Disabled)
				{
					return DepthStatus.InternalError;
				}
				return nativeSession.FrameApi.UpdateRawDepthTexture(ref depthTexture);
			}

			public static DepthStatus UpdateRawDepthConfidenceTexture(ref Texture2D confidenceTexture)
			{
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				ARCoreSession sessionComponent = LifecycleManager.Instance.SessionComponent;
				if (nativeSession == null || sessionComponent == null || sessionComponent.SessionConfig.DepthMode == DepthMode.Disabled)
				{
					return DepthStatus.InternalError;
				}
				return nativeSession.FrameApi.UpdateRawDepthConfidenceTexture(ref confidenceTexture);
			}
		}

		private static List<TrackableHit> _tmpTrackableHitList = new List<TrackableHit>();

		public static long Timestamp => LifecycleManager.Instance.NativeSession?.FrameApi.GetTimestamp() ?? 0;

		public static Pose Pose
		{
			get
			{
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				if (nativeSession == null)
				{
					return Pose.identity;
				}
				IntPtr cameraHandle = nativeSession.FrameApi.AcquireCamera();
				Pose pose = nativeSession.CameraApi.GetPose(cameraHandle);
				nativeSession.CameraApi.Release(cameraHandle);
				return pose;
			}
		}

		public static LightEstimate LightEstimate
		{
			get
			{
				NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
				ARCoreSession sessionComponent = LifecycleManager.Instance.SessionComponent;
				if (nativeSession == null || sessionComponent == null || sessionComponent.SessionConfig.LightEstimationMode == LightEstimationMode.Disabled)
				{
					return new LightEstimate(LightEstimateState.NotValid, 0f, Color.black, Quaternion.LookRotation(Vector3.down), Color.white, null, -1L);
				}
				return nativeSession.FrameApi.GetLightEstimate();
			}
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "List could be resized")]
		public static bool Raycast(float x, float y, TrackableHitFlags filter, out TrackableHit hitResult)
		{
			hitResult = default(TrackableHit);
			NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
			if (nativeSession == null)
			{
				return false;
			}
			bool num = nativeSession.HitTestApi.Raycast(nativeSession.FrameHandle, x, (float)Screen.height - y, filter, _tmpTrackableHitList);
			if (num && _tmpTrackableHitList.Count != 0)
			{
				hitResult = _tmpTrackableHitList[0];
			}
			return num;
		}

		public static RecordingResult RecordTrackData(Guid trackId, byte[] data)
		{
			return LifecycleManager.Instance.NativeSession?.FrameApi.RecordTrackData(trackId, data) ?? RecordingResult.ErrorRecordingFailed;
		}

		public static List<TrackData> GetUpdatedTrackData(Guid trackId)
		{
			return LifecycleManager.Instance.NativeSession.FrameApi.GetUpdatedTrackData(trackId);
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "List could be resized")]
		public static bool RaycastInstantPlacement(float x, float y, float approximateDistanceMeters, out TrackableHit hitResult)
		{
			hitResult = default(TrackableHit);
			NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
			if (nativeSession == null)
			{
				return false;
			}
			bool num = nativeSession.HitTestApi.Raycast(nativeSession.FrameHandle, x, (float)Screen.height - y, approximateDistanceMeters, _tmpTrackableHitList);
			if (num && _tmpTrackableHitList.Count != 0)
			{
				hitResult = _tmpTrackableHitList[0];
			}
			return num;
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "List could be resized")]
		public static bool Raycast(Vector3 origin, Vector3 direction, out TrackableHit hitResult, float maxDistance = float.PositiveInfinity, TrackableHitFlags filter = TrackableHitFlags.Default)
		{
			hitResult = default(TrackableHit);
			NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
			if (nativeSession == null)
			{
				return false;
			}
			bool num = nativeSession.HitTestApi.Raycast(nativeSession.FrameHandle, origin, direction, maxDistance, filter, _tmpTrackableHitList);
			if (num && _tmpTrackableHitList.Count != 0)
			{
				hitResult = _tmpTrackableHitList[0];
			}
			return num;
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "List could be resized")]
		public static bool RaycastAll(float x, float y, TrackableHitFlags filter, List<TrackableHit> hitResults)
		{
			hitResults.Clear();
			NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
			return nativeSession?.HitTestApi.Raycast(nativeSession.FrameHandle, x, (float)Screen.height - y, filter, hitResults) ?? false;
		}

		[SuppressMemoryAllocationError(IsWarning = true, Reason = "List could be resized")]
		public static bool RaycastAll(Vector3 origin, Vector3 direction, List<TrackableHit> hitResults, float maxDistance = float.PositiveInfinity, TrackableHitFlags filter = TrackableHitFlags.Default)
		{
			hitResults.Clear();
			NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
			return nativeSession?.HitTestApi.Raycast(nativeSession.FrameHandle, origin, direction, maxDistance, filter, hitResults) ?? false;
		}
	}
	public interface IAndroidPermissionsCheck
	{
		AsyncTask<AndroidPermissionsRequestResult> RequestAndroidPermission(string permissionName);
	}
	public enum InstantPlacementMode
	{
		Disabled = 0,
		LocalYUp = 2
	}
	public class InstantPlacementPoint : Trackable
	{
		public Pose Pose
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("Pose:: Trying to access a session that has already been destroyed.");
					return default(Pose);
				}
				return _nativeSession.PointApi.GetInstantPlacementPointPose(_trackableNativeHandle);
			}
		}

		public InstantPlacementPointTrackingMethod TrackingMethod
		{
			get
			{
				if (IsSessionDestroyed())
				{
					UnityEngine.Debug.LogError("InstantPlacementPointTrackingMethod:: Trying to access a session that has already been destroyed.");
					return InstantPlacementPointTrackingMethod.NotTracking;
				}
				return _nativeSession.PointApi.GetInstantPlacementPointTrackingMethod(_trackableNativeHandle);
			}
		}

		internal InstantPlacementPoint(IntPtr nativeHandle, NativeSession nativeApi)
			: base(nativeHandle, nativeApi)
		{
		}
	}
	public enum InstantPlacementPointTrackingMethod
	{
		NotTracking,
		ScreenspaceWithApproximateDistance,
		FullTracking
	}
	public struct LightEstimate
	{
		private float _pixelIntensity;

		private Color _colorCorrection;

		private Quaternion _directionalLightRotation;

		private Color _directionalLightColor;

		private SphericalHarmonicsL2 _ambientProbe;

		private Cubemap _cachedCubemap;

		public LightEstimationMode Mode { get; private set; }

		public LightEstimateState State { get; private set; }

		public float PixelIntensity
		{
			get
			{
				if (Mode != LightEstimationMode.AmbientIntensity)
				{
					UnityEngine.Debug.LogWarning("PixelIntensity value is not meaningful when LightEstimationMode is not AmbientIntensity.");
				}
				return _pixelIntensity;
			}
			private set
			{
				_pixelIntensity = value;
			}
		}

		public Color ColorCorrection
		{
			get
			{
				if (Mode != LightEstimationMode.AmbientIntensity)
				{
					UnityEngine.Debug.LogWarning("ColorCorrection value is not meaningful when LightEstimationMode is not AmbientIntensity.");
				}
				return _colorCorrection;
			}
			private set
			{
				_colorCorrection = value;
			}
		}

		public Quaternion DirectionalLightRotation
		{
			get
			{
				if (Mode != LightEstimationMode.EnvironmentalHDRWithoutReflections && Mode != LightEstimationMode.EnvironmentalHDRWithReflections)
				{
					UnityEngine.Debug.LogWarning("DirectionalLightRotation value is not meaningful when LightEstimationMode is not one of the Environmental HDR modes.");
				}
				return _directionalLightRotation;
			}
			private set
			{
				_directionalLightRotation = value;
			}
		}

		public Color DirectionalLightColor
		{
			get
			{
				if (Mode != LightEstimationMode.EnvironmentalHDRWithoutReflections && Mode != LightEstimationMode.EnvironmentalHDRWithReflections)
				{
					UnityEngine.Debug.LogWarning("DirectionalLightColor value is not meaningful when LightEstimationMode is not one of the Environmental HDR modes.");
				}
				return _directionalLightColor;
			}
			private set
			{
				_directionalLightColor = value;
			}
		}

		public SphericalHarmonicsL2 AmbientProbe
		{
			get
			{
				if (Mode != LightEstimationMode.EnvironmentalHDRWithoutReflections && Mode != LightEstimationMode.EnvironmentalHDRWithReflections)
				{
					UnityEngine.Debug.LogWarning("AmbientProbe value is not meaningful when LightEstimationMode is not one of the Environmental HDR modes.");
				}
				return _ambientProbe;
			}
			private set
			{
				_ambientProbe = value;
			}
		}

		public Cubemap ReflectionProbe
		{
			get
			{
				if (Mode != LightEstimationMode.EnvironmentalHDRWithReflections)
				{
					UnityEngine.Debug.LogWarning("ReflectionProbe value is not meaningful when LightEstimationMode is not EnvironmentalHDRWithReflections.");
					return null;
				}
				if (_cachedCubemap == null)
				{
					NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
					if (nativeSession == null)
					{
						return null;
					}
					_cachedCubemap = nativeSession.FrameApi.GetReflectionCubemap();
				}
				return _cachedCubemap;
			}
		}

		public long Timestamp { get; private set; }

		[Obsolete("LightEstimate(LightEstimateState, float, Color) has been deprecated. Please use new constructor instead.")]
		public LightEstimate(LightEstimateState state, float pixelIntensity, Color colorCorrection)
			: this(state, pixelIntensity, colorCorrection, Quaternion.identity, Color.black, null, -1L)
		{
		}

		public LightEstimate(LightEstimateState state, float pixelIntensity, Color colorCorrection, Quaternion directionalLightRotation, Color directionalLightColor, float[,] ambientSHCoefficients, long timestamp)
		{
			this = default(LightEstimate);
			InitializeLightEstimateMode();
			State = state;
			Timestamp = timestamp;
			_pixelIntensity = pixelIntensity;
			_colorCorrection = colorCorrection;
			_directionalLightRotation = directionalLightRotation;
			_directionalLightColor = directionalLightColor;
			_directionalLightColor = _directionalLightColor.gamma;
			SphericalHarmonicsL2 ambientProbe = default(SphericalHarmonicsL2);
			if (ambientSHCoefficients != null)
			{
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 9; j++)
					{
						ambientProbe[i, j] = ambientSHCoefficients[j, i];
					}
				}
			}
			_ambientProbe = ambientProbe;
			_cachedCubemap = null;
		}

		private void InitializeLightEstimateMode()
		{
			Mode = LightEstimationMode.Disabled;
			if (LifecycleManager.Instance.SessionComponent != null)
			{
				Mode = LifecycleManager.Instance.SessionComponent.SessionConfig.LightEstimationMode;
			}
		}
	}
	public enum LightEstimateState
	{
		NotValid,
		Valid
	}
	public enum LightEstimationMode
	{
		Disabled,
		AmbientIntensity,
		EnvironmentalHDRWithoutReflections,
		EnvironmentalHDRWithReflections
	}
	public enum LostTrackingReason
	{
		None,
		BadState,
		InsufficientLight,
		ExcessiveMotion,
		InsufficientFeatures,
		CameraUnavailable
	}
	public enum PlaybackResult
	{
		OK,
		ErrorSessionNotPaused,
		ErrorSessionUnsupported,
		ErrorPlaybackFailed
	}
	public enum PlaybackStatus
	{
		None,
		OK,
		IOError,
		FinishedSuccess
	}
	public struct PointCloudPoint
	{
		public const int InvalidPointId = -1;

		private int _id;

		public int Id
		{
			get
			{
				if (InstantPreviewManager.IsProvidingPlatform)
				{
					InstantPreviewManager.LogLimitedSupportMessage("access Point Cloud IDs");
					return 0;
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public Vector3 Position { get; private set; }

		public float Confidence { get; private set; }

		public PointCloudPoint(int id, Vector3 position, float confidence)
		{
			this = default(PointCloudPoint);
			Id = id;
			Position = position;
			Confidence = confidence;
		}

		public static implicit operator Vector3(PointCloudPoint point)
		{
			return point.Position;
		}
	}
	public enum RecordingResult
	{
		OK,
		ErrorInvalidArgument,
		ErrorRecordingFailed,
		ErrorIllegalState
	}
	public enum RecordingStatus
	{
		None,
		OK,
		IOError
	}
	public static class Session
	{
		public static SessionStatus Status => LifecycleManager.Instance.SessionStatus;

		public static LostTrackingReason LostTrackingReason => LifecycleManager.Instance.LostTrackingReason;

		public static RecordingStatus RecordingStatus => LifecycleManager.Instance.NativeSession?.SessionApi.GetRecordingStatus() ?? RecordingStatus.None;

		public static PlaybackStatus PlaybackStatus => LifecycleManager.Instance.NativeSession?.SessionApi.GetPlaybackStatus() ?? PlaybackStatus.None;

		[SuppressMemoryAllocationError(Reason = "Could allocate a new Anchor object")]
		public static Anchor CreateAnchor(Pose pose, Trackable trackable = null)
		{
			NativeSession nativeSession = LifecycleManager.Instance.NativeSession;
			if (nativeSession == null)
			{
				return null;
			}
			if (trackable == null)
			{
				return nativeSession.SessionApi.CreateAnchor(pose);
			}
			return trackable.CreateAnchor(pose);
		}

		[SuppressMemoryAllocationError(Reason = "List could be resized.")]
		public static void GetTrackables<T>(List<T> trackables, TrackableQueryFilter filter = TrackableQueryFilter.All) where T : Trackable
		{
			trackables.Clear();
			LifecycleManager.Instance.NativeSession?.GetTrackables(trackables, filter);
		}

		public static CameraConfig GetCameraConfig()
		{
			return LifecycleManager.Instance.NativeSession?.SessionApi.GetCameraConfig() ?? default(CameraConfig);
		}

		[SuppressMemoryAllocationError(Reason = "Creates a new AsyncTask")]
		public static AsyncTask<ApkAvailabilityStatus> CheckApkAvailability()
		{
			return LifecycleManager.Instance.CheckApkAvailability();
		}

		[SuppressMemoryAllocationError(Reason = "Creates a new AsyncTask")]
		public static AsyncTask<ApkInstallationStatus> RequestApkInstallation(bool userRequested)
		{
			return LifecycleManager.Instance.RequestApkInstallation(userRequested);
		}

		public static bool IsDepthModeSupported(DepthMode depthMode)
		{
			return LifecycleManager.Instance.NativeSession?.SessionApi.IsDepthModeSupported(depthMode.ToApiDepthMode()) ?? false;
		}

		public static RecordingResult StartRecording(ARCoreRecordingConfig config)
		{
			return LifecycleManager.Instance.NativeSession?.SessionApi.StartRecording(config) ?? RecordingResult.ErrorRecordingFailed;
		}

		public static RecordingResult StopRecording()
		{
			return LifecycleManager.Instance.NativeSession?.SessionApi.StopRecording() ?? RecordingResult.ErrorRecordingFailed;
		}

		public static PlaybackResult SetPlaybackDataset(string datasetFilepath)
		{
			return LifecycleManager.Instance.NativeSession?.SessionApi.SetPlaybackDataset(datasetFilepath) ?? PlaybackResult.ErrorPlaybackFailed;
		}
	}
	public enum SessionStatus
	{
		None = 0,
		Initializing = 1,
		Tracking = 100,
		LostTracking = 101,
		NotTracking = 102,
		FatalError = 200,
		ErrorApkNotAvailable = 201,
		ErrorPermissionNotGranted = 202,
		ErrorSessionConfigurationNotSupported = 203,
		ErrorCameraNotAvailable = 204,
		ErrorIllegalState = 205,
		ErrorInvalidCameraConfig = 206
	}
	public static class SessionStatusExtensions
	{
		private const int _notInitializedGroupStart = 0;

		private const int _validSessionGroupStart = 100;

		private const int _errorGroupStart = 200;

		public static bool IsNotInitialized(this SessionStatus status)
		{
			if (status >= SessionStatus.None)
			{
				return status < SessionStatus.Tracking;
			}
			return false;
		}

		public static bool IsValid(this SessionStatus status)
		{
			int num = (int)(status - 100);
			if (num >= 0)
			{
				return num < 100;
			}
			return false;
		}

		public static bool IsError(this SessionStatus status)
		{
			int num = (int)(status - 200);
			if (num >= 0)
			{
				return num < 100;
			}
			return false;
		}
	}
	public struct Track
	{
		public Guid Id;

		public byte[] Metadata;

		public string MimeType;
	}
	public abstract class Trackable
	{
		internal IntPtr _trackableNativeHandle = IntPtr.Zero;

		internal NativeSession _nativeSession;

		public virtual TrackingState TrackingState
		{
			[SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
			get
			{
				if (IsSessionDestroyed())
				{
					return TrackingState.Stopped;
				}
				return _nativeSession.TrackableApi.GetTrackingState(_trackableNativeHandle);
			}
		}

		internal Trackable()
		{
		}

		internal Trackable(IntPtr trackableNativeHandle, NativeSession nativeSession)
		{
			_trackableNativeHandle = trackableNativeHandle;
			_nativeSession = nativeSession;
		}

		~Trackable()
		{
			_nativeSession.TrackableApi.Release(_trackableNativeHandle);
		}

		[SuppressMemoryAllocationError(Reason = "Could allocate a new Anchor object")]
		public virtual Anchor CreateAnchor(Pose pose)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("CreateAnchor:: Trying to access a session that has already been destroyed.");
				return null;
			}
			if (!_nativeSession.TrackableApi.AcquireNewAnchor(_trackableNativeHandle, pose, out var anchorHandle))
			{
				UnityEngine.Debug.Log("Failed to create anchor on trackable.");
				return null;
			}
			return Anchor.Factory(_nativeSession, anchorHandle);
		}

		[SuppressMemoryAllocationError(Reason = "List could be resized.")]
		public virtual void GetAllAnchors(List<Anchor> anchors)
		{
			if (IsSessionDestroyed())
			{
				UnityEngine.Debug.LogError("GetAllAnchors:: Trying to access a session that has already been destroyed.");
				anchors.Clear();
			}
			else
			{
				_nativeSession.TrackableApi.GetAnchors(_trackableNativeHandle, anchors);
			}
		}

		protected bool IsSessionDestroyed()
		{
			return _nativeSession.IsDestroyed;
		}
	}
	public struct TrackableHit
	{
		public Pose Pose { get; private set; }

		public float Distance { get; private set; }

		public TrackableHitFlags Flags { get; private set; }

		public Trackable Trackable { get; private set; }

		internal TrackableHit(Pose pose, float distance, TrackableHitFlags flags, Trackable trackable)
		{
			this = default(TrackableHit);
			Pose = pose;
			Distance = distance;
			Flags = flags;
			Trackable = trackable;
		}
	}
	[Flags]
	public enum TrackableHitFlags
	{
		None = 0,
		PlaneWithinPolygon = 1,
		PlaneWithinBounds = 2,
		PlaneWithinInfinity = 4,
		FeaturePoint = 8,
		FeaturePointWithSurfaceNormal = 0x10,
		Depth = 0x20,
		Default = 0x31
	}
	public enum TrackableQueryFilter
	{
		All,
		New,
		Updated
	}
	public struct TrackData
	{
		public long FrameTimestamp;

		public byte[] Data;
	}
	public enum TrackingState
	{
		Tracking,
		Paused,
		Stopped
	}
	public static class VersionInfo
	{
		public static readonly string Version = "1.25.0";
	}
}
namespace GoogleARCore.CrossPlatform
{
	public struct CloudAnchorResult
	{
		public CloudServiceResponse Response;

		public XPAnchor Anchor;
	}
	public enum CloudServiceResponse
	{
		Success,
		ErrorNotSupportedByConfiguration,
		ErrorNotTracking,
		[Obsolete("In the case of Cloud Anchor creation, this error has been replaced by CloudServiceResponse.ErrorHostingServiceUnavailable. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.12.0 to learn more.")]
		ErrorServiceUnreachable,
		ErrorNotAuthorized,
		ErrorApiQuotaExceeded,
		ErrorDatasetInadequate,
		ErrorCloudIdNotFound,
		[Obsolete("This enum has been deprecated. See https://github.com/google-ar/arcore-unity-sdk/releases/tag/v1.12.0")]
		ErrorLocalizationFailed,
		ErrorSDKTooOld,
		ErrorSDKTooNew,
		ErrorInternal,
		ErrorHostingServiceUnavailable,
		ErrorRequestCancelled,
		ErrorTooManyCloudAnchors
	}
	public enum FeatureMapQuality
	{
		Insufficient,
		Sufficient,
		Good
	}
	[HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/CrossPlatform/XPAnchor")]
	public class XPAnchor : MonoBehaviour
	{
		private static Dictionary<IntPtr, XPAnchor> _anchorDict = new Dictionary<IntPtr, XPAnchor>(new IntPtrEqualityComparer());

		private XPTrackingState _lastFrameTrackingState = XPTrackingState.Stopped;

		private bool _isSessionDestroyed;

		public string CloudId { get; private set; }

		public XPTrackingState TrackingState
		{
			get
			{
				if (IsSessionDestroyed())
				{
					return XPTrackingState.Stopped;
				}
				return _nativeSession.AnchorApi.GetTrackingState(_nativeHandle).ToXPTrackingState();
			}
		}

		internal NativeSession _nativeSession { get; private set; }

		internal IntPtr _nativeHandle { get; private set; }

		internal static XPAnchor Factory(NativeSession nativeSession, IntPtr anchorHandle, bool isCreate = true)
		{
			if (anchorHandle == IntPtr.Zero)
			{
				return null;
			}
			if (_anchorDict.TryGetValue(anchorHandle, out var value))
			{
				AnchorApi.Release(anchorHandle);
				return value;
			}
			if (isCreate)
			{
				XPAnchor xPAnchor = new GameObject().AddComponent<XPAnchor>();
				xPAnchor.gameObject.name = "XPAnchor";
				xPAnchor.CloudId = nativeSession.AnchorApi.GetCloudAnchorId(anchorHandle);
				xPAnchor._nativeHandle = anchorHandle;
				xPAnchor._nativeSession = nativeSession;
				xPAnchor.Update();
				_anchorDict.Add(anchorHandle, xPAnchor);
				return xPAnchor;
			}
			return null;
		}

		private void Update()
		{
			if (_nativeHandle == IntPtr.Zero)
			{
				UnityEngine.Debug.LogError("Anchor components instantiated outside of ARCore are not supported. Please use a 'Create' method within ARCore to instantiate anchors.");
			}
			else
			{
				if (IsSessionDestroyed())
				{
					return;
				}
				Pose pose = _nativeSession.AnchorApi.GetPose(_nativeHandle);
				base.transform.position = pose.position;
				base.transform.rotation = pose.rotation;
				XPTrackingState trackingState = TrackingState;
				if (_lastFrameTrackingState == trackingState)
				{
					return;
				}
				bool active = trackingState == XPTrackingState.Tracking;
				foreach (Transform item in base.transform)
				{
					item.gameObject.SetActive(active);
				}
				_lastFrameTrackingState = trackingState;
			}
		}

		private void OnDestroy()
		{
			if (!(_nativeHandle == IntPtr.Zero))
			{
				if (_nativeSession != null && !_nativeSession.IsDestroyed)
				{
					_nativeSession.AnchorApi.Detach(_nativeHandle);
				}
				_anchorDict.Remove(_nativeHandle);
				AnchorApi.Release(_nativeHandle);
			}
		}

		private bool IsSessionDestroyed()
		{
			if (!_isSessionDestroyed && LifecycleManager.Instance.NativeSession != _nativeSession)
			{
				UnityEngine.Debug.LogErrorFormat("The session which created this anchor has been destroyed. The anchor on GameObject {0} can no longer update.", (base.gameObject != null) ? base.gameObject.name : "Unknown");
				_isSessionDestroyed = true;
			}
			return _isSessionDestroyed;
		}
	}
	public static class XPSession
	{
		public static AsyncTask<CloudAnchorResult> CreateCloudAnchor(Anchor anchor)
		{
			return CloudServiceManager.Instance.CreateCloudAnchor(anchor);
		}

		public static AsyncTask<CloudAnchorResult> ResolveCloudAnchor(string cloudAnchorId)
		{
			return CloudServiceManager.Instance.ResolveCloudAnchor(cloudAnchorId);
		}

		public static void CancelCloudAnchorAsyncTask(string cloudAnchorId)
		{
			CloudServiceManager.Instance.CancelCloudAnchorAsyncTask(cloudAnchorId);
		}

		public static AsyncTask<CloudAnchorResult> CreateCloudAnchor(Anchor anchor, int ttlDays)
		{
			return CloudServiceManager.Instance.CreateCloudAnchor(anchor, ttlDays);
		}

		public static FeatureMapQuality EstimateFeatureMapQualityForHosting(Pose pose)
		{
			return CloudServiceManager.Instance.EstimateFeatureMapQualityForHosting(pose);
		}
	}
	public enum XPTrackingState
	{
		Tracking,
		Paused,
		Stopped
	}
}
namespace GoogleARCore.Examples.PersistentCloudAnchors
{
	public class ARViewManager : MonoBehaviour
	{
		public PersistentCloudAnchorsController Controller;

		public GameObject CloudAnchorPrefab;

		public GameObject MapQualityIndicatorPrefab;

		public GameObject InstructionBar;

		public GameObject NamePanel;

		public GameObject CopyPanel;

		public GameObject InputFieldWarning;

		public InputField NameField;

		public Text InstructionText;

		public Text DebugText;

		public Button SaveButton;

		public Button ShareButton;

		private const float _startPrepareTime = 3f;

		private float _timeSinceStart;

		private bool _isReturning;

		private bool _isHosting;

		private MapQualityIndicator _qualityIndicator;

		private CloudAnchorHistory _hostedCloudAnchor;

		private Pose? _hitPose;

		private Component _anchorComponent;

		private List<Component> _cachedComponents = new List<Component>();

		private HashSet<string> _pendingTask = new HashSet<string>();

		private Color _activeColor;

		public static Pose GetCameraPose()
		{
			Pose result = default(Pose);
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				return Frame.Pose;
			}
			return result;
		}

		public void OnInputFieldValueChanged(string inputString)
		{
			Regex regex = new Regex("^[a-zA-Z0-9-_]*$");
			InputFieldWarning.SetActive(!regex.IsMatch(inputString));
			SetSaveButtonActive(!InputFieldWarning.activeSelf && inputString.Length > 0);
		}

		public void OnSaveButtonClicked()
		{
			_hostedCloudAnchor.Name = NameField.text;
			Controller.SaveCloudAnchorHistory(_hostedCloudAnchor);
			DebugText.text = $"Saved Cloud Anchor:\n{_hostedCloudAnchor.Name}.";
			ShareButton.gameObject.SetActive(value: true);
			NamePanel.SetActive(value: false);
		}

		public void OnShareButtonClicked()
		{
			GUIUtility.systemCopyBuffer = _hostedCloudAnchor.Id;
			DebugText.text = "Copied cloud id: " + _hostedCloudAnchor.Id;
		}

		public void OnCopyCompleted()
		{
			CopyPanel.SetActive(value: false);
		}

		public void Awake()
		{
			_activeColor = SaveButton.GetComponentInChildren<Text>().color;
		}

		public void OnEnable()
		{
			_timeSinceStart = 0f;
			_isReturning = false;
			_isHosting = false;
			_hitPose = null;
			_anchorComponent = null;
			_qualityIndicator = null;
			_cachedComponents.Clear();
			InstructionBar.SetActive(value: true);
			NamePanel.SetActive(value: false);
			CopyPanel.SetActive(value: false);
			InputFieldWarning.SetActive(value: false);
			ShareButton.gameObject.SetActive(value: false);
			Controller.PlaneGenerator.SetActive(value: true);
			switch (Controller.Mode)
			{
			case PersistentCloudAnchorsController.ApplicationMode.Ready:
				ReturnToHomePage("Invalid application mode, returning to home page...");
				break;
			case PersistentCloudAnchorsController.ApplicationMode.Hosting:
			case PersistentCloudAnchorsController.ApplicationMode.Resolving:
				InstructionText.text = "Detecting flat surface...";
				DebugText.text = "ARCore is preparing for " + Controller.Mode;
				break;
			}
		}

		public void OnDisable()
		{
			if (_pendingTask.Count > 0)
			{
				UnityEngine.Debug.LogFormat("Cancelling pending tasks for {0} Cloud Anchor(s): {1}", _pendingTask.Count, string.Join(",", new List<string>(_pendingTask).ToArray()));
				foreach (string item in _pendingTask)
				{
					XPSession.CancelCloudAnchorAsyncTask(item);
				}
				_pendingTask.Clear();
			}
			if (_qualityIndicator != null)
			{
				UnityEngine.Object.Destroy(_qualityIndicator.gameObject);
				_qualityIndicator = null;
			}
			if (_anchorComponent != null)
			{
				UnityEngine.Object.Destroy(_anchorComponent.gameObject);
				_anchorComponent = null;
			}
			if (_cachedComponents.Count <= 0)
			{
				return;
			}
			foreach (Component cachedComponent in _cachedComponents)
			{
				UnityEngine.Object.Destroy(cachedComponent.gameObject);
			}
			_cachedComponents.Clear();
		}

		public void Update()
		{
			if (_timeSinceStart < 3f)
			{
				_timeSinceStart += Time.deltaTime;
				if (_timeSinceStart >= 3f)
				{
					UpdateInitialInstruction();
				}
				return;
			}
			ARCoreLifecycleUpdate();
			if (_isReturning)
			{
				return;
			}
			if (Controller.Mode == PersistentCloudAnchorsController.ApplicationMode.Resolving)
			{
				ResolvingCloudAnchors();
			}
			else
			{
				if (Controller.Mode != PersistentCloudAnchorsController.ApplicationMode.Hosting)
				{
					return;
				}
				if (!_hitPose.HasValue)
				{
					if (Input.touchCount < 1)
					{
						return;
					}
					Touch touch2;
					Touch touch = (touch2 = Input.GetTouch(0));
					if (touch.phase != TouchPhase.Began || EventSystem.current.IsPointerOverGameObject(touch2.fingerId))
					{
						return;
					}
					PerformHitTest(touch2.position);
				}
				HostingCloudAnchor();
			}
		}

		private void PerformHitTest(Vector2 touchPos)
		{
			DetectedPlaneType detectedPlaneType = DetectedPlaneType.HorizontalUpwardFacing;
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				TrackableHit hitResult = default(TrackableHit);
				if (Frame.Raycast(touchPos.x, touchPos.y, TrackableHitFlags.PlaneWithinPolygon, out hitResult))
				{
					if (!(hitResult.Trackable is DetectedPlane detectedPlane))
					{
						UnityEngine.Debug.LogWarning("Hit test result has invalid trackable type: " + hitResult.Trackable.GetType());
						return;
					}
					detectedPlaneType = detectedPlane.PlaneType;
					_hitPose = hitResult.Pose;
					_anchorComponent = hitResult.Trackable.CreateAnchor(hitResult.Pose);
				}
			}
			if (_anchorComponent != null)
			{
				UnityEngine.Object.Instantiate(CloudAnchorPrefab, _anchorComponent.transform);
				GameObject gameObject = UnityEngine.Object.Instantiate(MapQualityIndicatorPrefab, _anchorComponent.transform);
				_qualityIndicator = gameObject.GetComponent<MapQualityIndicator>();
				_qualityIndicator.DrawIndicator(detectedPlaneType, Controller.MainCamera);
				InstructionText.text = " To save this location, walk around the object to capture it from different angles";
				DebugText.text = "Waiting for sufficient mapping quaility...";
				Controller.PlaneGenerator.SetActive(value: false);
			}
		}

		private void HostingCloudAnchor()
		{
			if (_anchorComponent == null || _isHosting)
			{
				return;
			}
			float magnitude = (_qualityIndicator.transform.position - Controller.MainCamera.transform.position).magnitude;
			if (magnitude < _qualityIndicator.Radius * 1.5f)
			{
				InstructionText.text = "You are too close, move backward.";
				return;
			}
			if (magnitude > 10f)
			{
				InstructionText.text = "You are too far, come closer.";
				return;
			}
			if (_qualityIndicator.ReachTopviewAngle)
			{
				InstructionText.text = "You are looking from the top view, move around from all sides.";
				return;
			}
			if (!_qualityIndicator.ReachQualityThreshold)
			{
				InstructionText.text = "Save the object here by capturing it from all sides.";
				DebugText.text = "Current mapping quality: " + XPSession.EstimateFeatureMapQualityForHosting(GetCameraPose());
				return;
			}
			_isHosting = true;
			InstructionText.text = "Processing...";
			DebugText.text = "Mapping quality has reached sufficient threshold, creating Cloud Anchor.";
			DebugText.text = $"FeatureMapQuality has reached {XPSession.EstimateFeatureMapQualityForHosting(GetCameraPose())}, triggering CreateCloudAnchor.";
			XPSession.CreateCloudAnchor((Anchor)_anchorComponent, 1).ThenAction(delegate(CloudAnchorResult result)
			{
				if (_isHosting)
				{
					if (result.Response != CloudServiceResponse.Success)
					{
						UnityEngine.Debug.LogFormat("Failed to host cloud anchor: {0}", result.Response);
						OnAnchorHostedFinished(success: false, result.Response.ToString());
					}
					else
					{
						UnityEngine.Debug.LogFormat("Succeed to host cloud anchor: {0}", result.Anchor.CloudId);
						int count = Controller.LoadCloudAnchorHistory().Collection.Count;
						_hostedCloudAnchor = new CloudAnchorHistory("CloudAnchor" + count, result.Anchor.CloudId);
						OnAnchorHostedFinished(success: true, result.Anchor.CloudId);
					}
				}
			});
		}

		private void ResolvingCloudAnchors()
		{
			if (Controller.ResolvingSet.Count == 0 || Session.Status != SessionStatus.Tracking)
			{
				return;
			}
			UnityEngine.Debug.LogFormat("Attempting to resolve {0} anchor(s): {1}", Controller.ResolvingSet.Count, string.Join(",", new List<string>(Controller.ResolvingSet).ToArray()));
			foreach (string cloudId in Controller.ResolvingSet)
			{
				_pendingTask.Add(cloudId);
				XPSession.ResolveCloudAnchor(cloudId).ThenAction(delegate(CloudAnchorResult result)
				{
					_pendingTask.Remove(cloudId);
					if (result.Response != CloudServiceResponse.Success)
					{
						UnityEngine.Debug.LogFormat("Faild to resolve cloud anchor {0} for {1}", cloudId, result.Response);
						OnAnchorResolvedFinished(success: false, result.Response.ToString());
					}
					else
					{
						UnityEngine.Debug.LogFormat("Succeed to resolve cloud anchor: {0}", cloudId);
						OnAnchorResolvedFinished(success: true, cloudId);
						UnityEngine.Object.Instantiate(CloudAnchorPrefab, result.Anchor.transform);
						_cachedComponents.Add(result.Anchor);
					}
				});
			}
			Controller.ResolvingSet.Clear();
		}

		private void OnAnchorHostedFinished(bool success, string response)
		{
			if (success)
			{
				InstructionText.text = "Finish!";
				Invoke("DoHideInstructionBar", 1.5f);
				DebugText.text = "Succeed to host cloud anchor: " + response;
				NameField.text = _hostedCloudAnchor.Name;
				NamePanel.SetActive(value: true);
				SetSaveButtonActive(active: true);
			}
			else
			{
				InstructionText.text = "Host failed.";
				DebugText.text = "Failed to host cloud anchor: " + response;
			}
		}

		private void OnAnchorResolvedFinished(bool success, string response)
		{
			if (success)
			{
				InstructionText.text = "Resolve success!";
				DebugText.text = "Succeed to resolve cloud anchor: " + response;
			}
			else
			{
				InstructionText.text = "Resolve failed.";
				DebugText.text = "Failed to resolve cloud anchor: " + response;
			}
		}

		private void UpdateInitialInstruction()
		{
			switch (Controller.Mode)
			{
			case PersistentCloudAnchorsController.ApplicationMode.Hosting:
				InstructionText.text = "Tap to place an object.";
				DebugText.text = "Tap a vertical or horizontal plane...";
				break;
			case PersistentCloudAnchorsController.ApplicationMode.Resolving:
				InstructionText.text = "Look at the location you expect to see the AR experience appear.";
				DebugText.text = $"Attempting to resolve {Controller.ResolvingSet.Count} anchors...";
				break;
			}
		}

		private void ARCoreLifecycleUpdate()
		{
			int sleepTimeout = -1;
			if (Session.Status != SessionStatus.Tracking)
			{
				sleepTimeout = -2;
			}
			Screen.sleepTimeout = sleepTimeout;
			if (!_isReturning)
			{
				if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
				{
					ReturnToHomePage("Camera permission is needed to run this application.");
				}
				else if (Session.Status.IsError())
				{
					ReturnToHomePage("ARCore encountered a problem connecting. Please start the app again.");
				}
			}
		}

		private void ReturnToHomePage(string reason)
		{
			UnityEngine.Debug.Log("Returning home for reason: " + reason);
			if (!_isReturning)
			{
				_isReturning = true;
				DebugText.text = reason;
				Invoke("DoReturnToHomePage", 3f);
			}
		}

		private void DoReturnToHomePage()
		{
			Controller.SwitchToHomePage();
		}

		private void DoHideInstructionBar()
		{
			InstructionBar.SetActive(value: false);
		}

		private void SetSaveButtonActive(bool active)
		{
			SaveButton.enabled = active;
			SaveButton.GetComponentInChildren<Text>().color = (active ? _activeColor : Color.gray);
		}
	}
	[RequireComponent(typeof(LineRenderer))]
	public class CircleRenderer : MonoBehaviour
	{
		[Range(4f, 720f)]
		public int Segment = 72;

		private const string _varColor = "_TintColor";

		private LineRenderer _lineRenderer;

		private float _alpha = 1f;

		public void SetAlpha(float alpha)
		{
			_alpha = alpha;
		}

		public void DrawArc(Vector3 centerDir, float radius, float range)
		{
			range = Mathf.Clamp(range, 0f, 360f);
			if (range == 0f)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			int num = (int)(range * (float)Segment / 360f);
			float num2 = range / (float)num;
			if (_lineRenderer == null)
			{
				_lineRenderer = base.gameObject.GetComponent<LineRenderer>();
			}
			_lineRenderer.positionCount = num + 1;
			_lineRenderer.useWorldSpace = false;
			for (int i = 0; i < num + 1; i++)
			{
				Vector3 position = Quaternion.AngleAxis((0f - range) / 2f + num2 * (float)i, Vector3.up) * centerDir * radius + base.transform.position;
				_lineRenderer.SetPosition(i, base.transform.InverseTransformPoint(position));
			}
			base.gameObject.SetActive(value: true);
		}

		public void Update()
		{
			if (!(_lineRenderer == null))
			{
				Renderer component = _lineRenderer.GetComponent<Renderer>();
				Color color = component.material.GetColor("_TintColor");
				color.a = _alpha;
				component.material.SetColor("_TintColor", color);
			}
		}
	}
	[Serializable]
	public struct CloudAnchorHistory
	{
		public string Name;

		public string Id;

		public string SerializedTime;

		public DateTime CreatedTime => Convert.ToDateTime(SerializedTime);

		public CloudAnchorHistory(string name, string id, DateTime time)
		{
			Name = name;
			Id = id;
			SerializedTime = time.ToString();
		}

		public CloudAnchorHistory(string name, string id)
			: this(name, id, DateTime.Now)
		{
		}

		public override string ToString()
		{
			return JsonUtility.ToJson(this);
		}
	}
	[Serializable]
	public class CloudAnchorHistoryCollection
	{
		public List<CloudAnchorHistory> Collection = new List<CloudAnchorHistory>();
	}
	public class DoubleLabelsItem : MonoBehaviour
	{
		public Text FirstLabel;

		public Text SecondLabel;

		public void SetLabels(string first, string second)
		{
			if (FirstLabel != null)
			{
				FirstLabel.text = first;
			}
			if (SecondLabel != null)
			{
				SecondLabel.text = second;
			}
		}
	}
	public class MapQualityBar : MonoBehaviour
	{
		public Animator Animator;

		public Renderer Renderer;

		public Color InitialColor = Color.white;

		public Color LowQualityColor = Color.red;

		public Color MediumQualityColor = Color.yellow;

		public Color HighQualityColor = Color.green;

		private const string _varColor = "_Color";

		private static readonly int _paramQuality = Animator.StringToHash("Quality");

		private static readonly int _paramIsVisited = Animator.StringToHash("IsVisited");

		private static readonly int _paramColorCurve = Animator.StringToHash("ColorCurve");

		private static readonly int _stateLow = Animator.StringToHash("Base Layer.Low");

		private static readonly int _stateMedium = Animator.StringToHash("Base Layer.Medium");

		private static readonly int _stateHigh = Animator.StringToHash("Base Layer.High");

		private bool _isVisited;

		private int _state;

		private float _alpha = 1f;

		public bool IsVisited
		{
			get
			{
				return _isVisited;
			}
			set
			{
				_isVisited = value;
				Animator.SetBool(_paramIsVisited, _isVisited);
			}
		}

		public int QualityState
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
				Animator.SetInteger(_paramQuality, _state);
			}
		}

		public float Weight
		{
			get
			{
				if (IsVisited)
				{
					return _state switch
					{
						0 => 0.1f, 
						1 => 0.5f, 
						2 => 1f, 
						_ => 0f, 
					};
				}
				return 0f;
			}
		}

		public void SetAlpha(float alpha)
		{
			_alpha = alpha;
		}

		public void Update()
		{
			AnimatorStateInfo currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
			float t = Animator.GetFloat(_paramColorCurve);
			Color value = InitialColor;
			if (currentAnimatorStateInfo.fullPathHash == _stateLow)
			{
				value = Color.Lerp(InitialColor, LowQualityColor, t);
			}
			else if (currentAnimatorStateInfo.fullPathHash == _stateMedium)
			{
				value = Color.Lerp(LowQualityColor, MediumQualityColor, t);
			}
			else if (currentAnimatorStateInfo.fullPathHash == _stateHigh)
			{
				value = Color.Lerp(MediumQualityColor, HighQualityColor, t);
			}
			value.a = _alpha;
			Renderer.material.SetColor("_Color", value);
		}
	}
	public class MapQualityIndicator : MonoBehaviour
	{
		public GameObject MapQualityBarPrefab;

		public CircleRenderer CircleRenderer;

		[Range(0f, 360f)]
		public float Range = 150f;

		public float Radius = 0.1f;

		private const float _verticalRange = 150f;

		private const float _horizontalRange = 180f;

		private const float _qualityThreshold = 0.6f;

		private const float _topviewThreshold = 15f;

		private const float _disappearDuration = 0.5f;

		private const float _fadingDuration = 3f;

		private const float _barSpacing = 7.5f;

		private const float _circleFadingRange = 10f;

		private Camera _mainCamera;

		private Vector3 _centerDir;

		private float _fadingTimer = -1f;

		private float _disappearTimer = -1f;

		private List<MapQualityBar> _mapQualityBars = new List<MapQualityBar>();

		public bool ReachQualityThreshold
		{
			get
			{
				float num = 0f;
				foreach (MapQualityBar mapQualityBar in _mapQualityBars)
				{
					num += mapQualityBar.Weight;
				}
				return num / (float)_mapQualityBars.Count >= 0.6f;
			}
		}

		public bool ReachTopviewAngle => Vector3.Angle(_mainCamera.transform.position - base.transform.position, Vector3.up) < 15f;

		public void DrawIndicator(DetectedPlaneType detectedPlaneType, Camera camera)
		{
			Range = ((detectedPlaneType == DetectedPlaneType.Vertical) ? 150f : 180f);
			_mainCamera = camera;
			_centerDir = ((detectedPlaneType == DetectedPlaneType.Vertical) ? base.transform.TransformVector(Vector3.up) : base.transform.TransformVector(-Vector3.forward));
			DrawBars();
			DrawRing();
			base.gameObject.SetActive(value: true);
		}

		public void Awake()
		{
			base.gameObject.SetActive(value: false);
		}

		public void Update()
		{
			if (ReachTopviewAngle)
			{
				if (_fadingTimer >= 3f)
				{
					return;
				}
				if (_fadingTimer < 0f)
				{
					_fadingTimer = 0f;
				}
				_fadingTimer += Time.deltaTime;
				float alpha = Mathf.Clamp(1f - _fadingTimer / 3f, 0f, 1f);
				CircleRenderer.SetAlpha(alpha);
				{
					foreach (MapQualityBar mapQualityBar in _mapQualityBars)
					{
						mapQualityBar.SetAlpha(alpha);
					}
					return;
				}
			}
			if (_fadingTimer > 0f)
			{
				_fadingTimer -= Time.deltaTime;
				float alpha2 = Mathf.Clamp(1f - _fadingTimer / 3f, 0f, 1f);
				CircleRenderer.SetAlpha(alpha2);
				foreach (MapQualityBar mapQualityBar2 in _mapQualityBars)
				{
					mapQualityBar2.SetAlpha(alpha2);
				}
			}
			foreach (MapQualityBar mapQualityBar3 in _mapQualityBars)
			{
				if (IsLookingAtBar(mapQualityBar3))
				{
					mapQualityBar3.IsVisited = true;
					mapQualityBar3.QualityState = (int)XPSession.EstimateFeatureMapQualityForHosting(ARViewManager.GetCameraPose());
				}
			}
			PlayDisappearAnimation();
		}

		private void DrawRing()
		{
			CircleRenderer.DrawArc(_centerDir, Radius, Range + 20f);
		}

		private void DrawBars()
		{
			Vector3 position = base.transform.position;
			Vector3 vector = _centerDir * Radius;
			Quaternion rotation = Quaternion.AngleAxis(0f, Vector3.up);
			GameObject gameObject = UnityEngine.Object.Instantiate(MapQualityBarPrefab, position + vector, rotation, base.transform);
			_mapQualityBars.Add(gameObject.GetComponent<MapQualityBar>());
			for (float num = 7.5f; num < Range / 2f; num += 7.5f)
			{
				rotation = Quaternion.AngleAxis(num, Vector3.up);
				vector = rotation * _centerDir * Radius;
				gameObject = UnityEngine.Object.Instantiate(MapQualityBarPrefab, position + vector, rotation, base.transform);
				_mapQualityBars.Add(gameObject.GetComponent<MapQualityBar>());
				rotation = Quaternion.AngleAxis(0f - num, Vector3.up);
				vector = rotation * _centerDir * Radius;
				gameObject = UnityEngine.Object.Instantiate(MapQualityBarPrefab, position + vector, rotation, base.transform);
				_mapQualityBars.Add(gameObject.GetComponent<MapQualityBar>());
			}
		}

		private bool IsLookingAtBar(MapQualityBar bar)
		{
			Vector3 vector = _mainCamera.WorldToViewportPoint(bar.transform.position);
			if (vector.z <= 0f || vector.x <= 0f || vector.x >= 1f || vector.y <= 0f || vector.y >= 1f)
			{
				return false;
			}
			if ((base.transform.position - _mainCamera.transform.position).magnitude <= Radius)
			{
				return false;
			}
			Vector3 vector2 = Vector3.ProjectOnPlane(_mainCamera.transform.position - base.transform.position, Vector3.up);
			Vector3 to = Vector3.ProjectOnPlane(bar.transform.position - base.transform.position, Vector3.up);
			return Vector3.Angle(vector2, to) < 7.5f;
		}

		private void PlayDisappearAnimation()
		{
			if (_disappearTimer < 0f && ReachQualityThreshold)
			{
				_disappearTimer = 0f;
			}
			if (_disappearTimer >= 0f && _disappearTimer < 0.5f)
			{
				_disappearTimer += Time.deltaTime;
				float num = Mathf.Max(0f, (0.5f - _disappearTimer) / 0.5f);
				base.transform.localScale = new Vector3(num, num, num);
			}
			if (_disappearTimer >= 0.5f)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
	[RequireComponent(typeof(RectTransform))]
	public class MultiselectionDropdown : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICancelHandler
	{
		[Serializable]
		public class OptionData
		{
			[SerializeField]
			public string MajorInfo;

			[SerializeField]
			public string MinorInfo;

			public OptionData(string major, string minor)
			{
				MajorInfo = major;
				MinorInfo = minor;
			}
		}

		public GameObject OptionRect;

		public GameObject HeadingTextPrefab;

		public GameObject MultiselectionItemPrefab;

		public Text CaptionText;

		public int TextLimit = 20;

		public Action OnValueChanged;

		private float _itemHeight;

		private float _maxHeight;

		private bool _optionChanged = true;

		private List<OptionData> _options = new List<OptionData>();

		private List<Toggle> _optionToggles = new List<Toggle>();

		public List<OptionData> Options
		{
			get
			{
				return _options;
			}
			set
			{
				if (CaptionText != null)
				{
					CaptionText.text = "Select";
				}
				_optionToggles.Clear();
				_optionChanged = true;
				_options = value;
				if (_options.Count == 0)
				{
					CaptionText.text = "No option available";
				}
			}
		}

		public List<int> SelectedValues
		{
			get
			{
				List<int> list = new List<int>();
				for (int i = 0; i < _optionToggles.Count; i++)
				{
					if (_optionToggles[i].isOn)
					{
						list.Add(i);
					}
				}
				return list;
			}
		}

		public void Deselect()
		{
			if (OptionRect.activeSelf)
			{
				OptionRect.SetActive(value: false);
			}
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			bool activeSelf = OptionRect.activeSelf;
			if (!activeSelf)
			{
				UpdateOptionRect();
				OptionRect.SetActive(!activeSelf);
			}
			else if (eventData.pointerCurrentRaycast.gameObject.GetHashCode() != OptionRect.GetHashCode())
			{
				OptionRect.SetActive(value: false);
			}
		}

		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			if (OnValueChanged != null)
			{
				OnValueChanged();
			}
			Deselect();
		}

		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			Deselect();
		}

		protected override void Awake()
		{
			_maxHeight = OptionRect.GetComponent<RectTransform>().rect.height;
			_itemHeight = OptionRect.GetComponent<ScrollRect>().content.GetComponent<RectTransform>().rect.height;
			_optionChanged = true;
		}

		private void OnSelectionChanged(bool isSelected)
		{
			List<string> list = new List<string>();
			foreach (Toggle optionToggle in _optionToggles)
			{
				if (optionToggle.isOn)
				{
					list.Add(optionToggle.GetComponent<DoubleLabelsItem>().FirstLabel.text);
				}
			}
			if (CaptionText != null && _options.Count > 0)
			{
				if (list.Count == 0)
				{
					CaptionText.text = "Select";
				}
				else
				{
					string text = string.Join(",", list.ToArray());
					if (TextLimit > 0 && text.Length > TextLimit)
					{
						text = text.Substring(0, TextLimit) + "...";
					}
					CaptionText.text = text;
				}
			}
			if (OnValueChanged != null)
			{
				OnValueChanged();
			}
		}

		private void UpdateOptionRect()
		{
			if (!_optionChanged)
			{
				return;
			}
			RectTransform component = OptionRect.GetComponent<RectTransform>();
			RectTransform content = OptionRect.GetComponent<ScrollRect>().content;
			_optionToggles.Clear();
			content.transform.DetachChildren();
			int num = 0;
			if (HeadingTextPrefab != null && _options.Count > 0)
			{
				UnityEngine.Object.Instantiate(HeadingTextPrefab).transform.SetParent(content.transform, worldPositionStays: false);
				num++;
			}
			foreach (OptionData option in Options)
			{
				GameObject obj = UnityEngine.Object.Instantiate(MultiselectionItemPrefab);
				obj.transform.SetParent(content.transform, worldPositionStays: false);
				obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f - _itemHeight * (float)num);
				obj.GetComponent<DoubleLabelsItem>().SetLabels(option.MajorInfo, option.MinorInfo);
				Toggle component2 = obj.GetComponent<Toggle>();
				component2.onValueChanged.AddListener(OnSelectionChanged);
				_optionToggles.Add(component2);
				num++;
			}
			component.sizeDelta = new Vector2(component.sizeDelta.x, Mathf.Min((float)num * _itemHeight, _maxHeight));
			content.sizeDelta = new Vector2(content.sizeDelta.x, (float)num * _itemHeight);
			CaptionText.text = ((_options.Count == 0) ? "No option available" : "Select");
			_optionChanged = false;
		}
	}
	public class PersistentCloudAnchorsController : MonoBehaviour
	{
		public enum ApplicationMode
		{
			Ready,
			Hosting,
			Resolving
		}

		[Header("ARCore")]
		public GameObject ARCoreRoot;

		public Camera ARCoreCamera;

		public GameObject ARCorePlaneGenerator;

		[Header("ARKit")]
		public GameObject ARKitRoot;

		public Camera ARKitCamera;

		public GameObject ARKitPlaneGenerator;

		[Header("UI")]
		public GameObject HomePage;

		public GameObject ResolveMenu;

		public GameObject PrivacyPrompt;

		public GameObject ARView;

		[HideInInspector]
		public ApplicationMode Mode;

		public HashSet<string> ResolvingSet = new HashSet<string>();

		private const string _hasDisplayedStartInfoKey = "HasDisplayedStartInfo";

		private const string _persistentCloudAnchorsStorageKey = "PersistentCloudAnchors";

		private const int _storageLimit = 40;

		public Camera MainCamera
		{
			get
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					return ARKitCamera;
				}
				return ARCoreCamera;
			}
		}

		public GameObject PlaneGenerator
		{
			get
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					return ARKitPlaneGenerator;
				}
				return ARCorePlaneGenerator;
			}
		}

		public void OnHostButtonClicked()
		{
			Mode = ApplicationMode.Hosting;
			SwitchToPrivacyPrompt();
		}

		public void OnResolveButtonClicked()
		{
			Mode = ApplicationMode.Resolving;
			SwitchToResolveMenu();
		}

		public void OnLearnMoreButtonClicked()
		{
			Application.OpenURL("https://developers.google.com/ar/cloud-anchors-privacy");
		}

		public void SwitchToHomePage()
		{
			ResetAllViews();
			Mode = ApplicationMode.Ready;
			ResolvingSet.Clear();
			HomePage.SetActive(value: true);
		}

		public void SwitchToResolveMenu()
		{
			ResetAllViews();
			ResolveMenu.SetActive(value: true);
		}

		public void SwitchToPrivacyPrompt()
		{
			if (PlayerPrefs.HasKey("HasDisplayedStartInfo"))
			{
				SwitchToARView();
				return;
			}
			ResetAllViews();
			PrivacyPrompt.SetActive(value: true);
		}

		public void SwitchToARView()
		{
			ResetAllViews();
			PlayerPrefs.SetInt("HasDisplayedStartInfo", 1);
			ARView.SetActive(value: true);
			SetPlatformActive(active: true);
		}

		public CloudAnchorHistoryCollection LoadCloudAnchorHistory()
		{
			if (PlayerPrefs.HasKey("PersistentCloudAnchors"))
			{
				CloudAnchorHistoryCollection cloudAnchorHistoryCollection = JsonUtility.FromJson<CloudAnchorHistoryCollection>(PlayerPrefs.GetString("PersistentCloudAnchors"));
				DateTime current = DateTime.Now;
				cloudAnchorHistoryCollection.Collection.RemoveAll((CloudAnchorHistory data) => current.Subtract(data.CreatedTime).Days > 0);
				PlayerPrefs.SetString("PersistentCloudAnchors", JsonUtility.ToJson(cloudAnchorHistoryCollection));
				return cloudAnchorHistoryCollection;
			}
			return new CloudAnchorHistoryCollection();
		}

		public void SaveCloudAnchorHistory(CloudAnchorHistory data)
		{
			CloudAnchorHistoryCollection cloudAnchorHistoryCollection = LoadCloudAnchorHistory();
			cloudAnchorHistoryCollection.Collection.Add(data);
			cloudAnchorHistoryCollection.Collection.Sort((CloudAnchorHistory left, CloudAnchorHistory right) => right.CreatedTime.CompareTo(left.CreatedTime));
			if (cloudAnchorHistoryCollection.Collection.Count > 40)
			{
				cloudAnchorHistoryCollection.Collection.RemoveRange(40, cloudAnchorHistoryCollection.Collection.Count - 40);
			}
			PlayerPrefs.SetString("PersistentCloudAnchors", JsonUtility.ToJson(cloudAnchorHistoryCollection));
		}

		public void Awake()
		{
			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
			Screen.autorotateToPortraitUpsideDown = false;
			Screen.orientation = ScreenOrientation.Portrait;
			Application.targetFrameRate = 60;
			SwitchToHomePage();
		}

		public void Update()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				if (HomePage.activeSelf)
				{
					Application.Quit();
				}
				else
				{
					SwitchToHomePage();
				}
			}
		}

		private void ResetAllViews()
		{
			Screen.sleepTimeout = -2;
			SetPlatformActive(active: false);
			ARView.SetActive(value: false);
			PrivacyPrompt.SetActive(value: false);
			ResolveMenu.SetActive(value: false);
			HomePage.SetActive(value: false);
		}

		private void SetPlatformActive(bool active)
		{
			if (!active)
			{
				ARCoreRoot.SetActive(value: false);
				ARKitRoot.SetActive(value: false);
			}
			else if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				ARCoreRoot.SetActive(value: true);
				ARKitRoot.SetActive(value: false);
			}
			else
			{
				ARCoreRoot.SetActive(value: false);
				ARKitRoot.SetActive(value: true);
			}
		}
	}
	public class ResolveMenuManager : MonoBehaviour
	{
		public PersistentCloudAnchorsController Controller;

		public MultiselectionDropdown Multiselection;

		public InputField InputField;

		public GameObject InvalidInputWarning;

		public Button ResolveButton;

		private CloudAnchorHistoryCollection _history = new CloudAnchorHistoryCollection();

		private Color _activeColor;

		public void OnInputFieldValueChanged(string inputString)
		{
			Regex regex = new Regex("^[a-zA-Z0-9-_,]*$");
			InvalidInputWarning.SetActive(!regex.IsMatch(inputString));
		}

		public void OnInputFieldEndEdit(string inputString)
		{
			if (!InvalidInputWarning.activeSelf)
			{
				OnResolvingSelectionChanged();
			}
		}

		public void OnResolvingSelectionChanged()
		{
			Controller.ResolvingSet.Clear();
			List<int> selectedValues = Multiselection.SelectedValues;
			if (selectedValues.Count > 0)
			{
				foreach (int item in selectedValues)
				{
					Controller.ResolvingSet.Add(_history.Collection[item].Id);
				}
			}
			if (!InvalidInputWarning.activeSelf && InputField.text.Length > 0)
			{
				string[] array = InputField.text.Split(',');
				if (array.Length != 0)
				{
					Controller.ResolvingSet.UnionWith(array);
				}
			}
			SetButtonActive(ResolveButton, Controller.ResolvingSet.Count > 0);
		}

		public void Awake()
		{
			_activeColor = ResolveButton.GetComponent<Image>().color;
		}

		public void OnEnable()
		{
			SetButtonActive(ResolveButton, active: false);
			InvalidInputWarning.SetActive(value: false);
			InputField.text = string.Empty;
			_history = Controller.LoadCloudAnchorHistory();
			MultiselectionDropdown multiselection = Multiselection;
			multiselection.OnValueChanged = (Action)Delegate.Combine(multiselection.OnValueChanged, new Action(OnResolvingSelectionChanged));
			List<MultiselectionDropdown.OptionData> list = new List<MultiselectionDropdown.OptionData>();
			foreach (CloudAnchorHistory item in _history.Collection)
			{
				list.Add(new MultiselectionDropdown.OptionData(item.Name, FormatDateTime(item.CreatedTime)));
			}
			Multiselection.Options = list;
		}

		public void OnDisable()
		{
			MultiselectionDropdown multiselection = Multiselection;
			multiselection.OnValueChanged = (Action)Delegate.Remove(multiselection.OnValueChanged, new Action(OnResolvingSelectionChanged));
			Multiselection.Deselect();
			Multiselection.Options.Clear();
			_history.Collection.Clear();
		}

		private string FormatDateTime(DateTime time)
		{
			TimeSpan timeSpan = DateTime.Now.Subtract(time);
			if (timeSpan.Hours != 0)
			{
				return $"{timeSpan.Hours}h ago";
			}
			if (timeSpan.Minutes != 0)
			{
				return $"{timeSpan.Minutes}m ago";
			}
			return "Just now";
		}

		private void SetButtonActive(Button button, bool active)
		{
			button.GetComponent<Image>().color = (active ? _activeColor : Color.grey);
			button.enabled = active;
		}
	}
}
namespace GoogleARCore.Examples.ObjectManipulationInternal
{
	public abstract class Gesture<T> where T : Gesture<T>
	{
		private bool _hasStarted;

		public bool WasCancelled { get; private set; }

		public GameObject TargetObject { get; protected set; }

		protected internal GestureRecognizer<T> _recognizer { get; private set; }

		public event Action<T> onStart;

		public event Action<T> onUpdated;

		public event Action<T> onFinished;

		internal Gesture(GestureRecognizer<T> recognizer)
		{
			_recognizer = recognizer;
		}

		internal void Update()
		{
			if (!_hasStarted && CanStart())
			{
				Start();
			}
			else if (_hasStarted && UpdateGesture() && this.onUpdated != null)
			{
				this.onUpdated(this as T);
			}
		}

		internal void Cancel()
		{
			WasCancelled = true;
			OnCancel();
			Complete();
		}

		protected internal abstract bool CanStart();

		protected internal abstract void OnStart();

		protected internal abstract bool UpdateGesture();

		protected internal abstract void OnCancel();

		protected internal abstract void OnFinish();

		protected internal void Complete()
		{
			OnFinish();
			if (this.onFinished != null)
			{
				this.onFinished(this as T);
			}
		}

		private void Start()
		{
			_hasStarted = true;
			OnStart();
			if (this.onStart != null)
			{
				this.onStart(this as T);
			}
		}
	}
	public abstract class GestureRecognizer<T> where T : Gesture<T>
	{
		protected List<T> _gestures = new List<T>();

		public event Action<T> onGestureStarted;

		public void Update()
		{
			TryCreateGestures();
			for (int i = 0; i < _gestures.Count; i++)
			{
				_gestures[i].Update();
			}
		}

		protected internal abstract void TryCreateGestures();

		protected internal void TryCreateOneFingerGestureOnTouchBegan(Func<Touch, T> createGestureFunction)
		{
			for (int i = 0; i < Input.touches.Length; i++)
			{
				Touch touch = Input.touches[i];
				if (touch.phase == TouchPhase.Began && !GestureTouchesUtility.IsFingerIdRetained(touch.fingerId) && !GestureTouchesUtility.IsTouchOffScreenEdge(touch))
				{
					T val = createGestureFunction(touch);
					val.onStart += OnStart;
					val.onFinished += OnFinished;
					_gestures.Add(val);
				}
			}
		}

		protected internal void TryCreateTwoFingerGestureOnTouchBegan(Func<Touch, Touch, T> createGestureFunction)
		{
			if (Input.touches.Length >= 2)
			{
				for (int i = 0; i < Input.touches.Length; i++)
				{
					TryCreateGestureTwoFingerGestureOnTouchBeganForTouchIndex(i, createGestureFunction);
				}
			}
		}

		private void TryCreateGestureTwoFingerGestureOnTouchBeganForTouchIndex(int touchIndex, Func<Touch, Touch, T> createGestureFunction)
		{
			if (Input.touches[touchIndex].phase != TouchPhase.Began)
			{
				return;
			}
			Touch touch = Input.touches[touchIndex];
			if (GestureTouchesUtility.IsFingerIdRetained(touch.fingerId) || GestureTouchesUtility.IsTouchOffScreenEdge(touch))
			{
				return;
			}
			for (int i = 0; i < Input.touches.Length; i++)
			{
				if (i != touchIndex && (i >= touchIndex || Input.touches[i].phase != TouchPhase.Began))
				{
					Touch touch2 = Input.touches[i];
					if (!GestureTouchesUtility.IsFingerIdRetained(touch2.fingerId) && !GestureTouchesUtility.IsTouchOffScreenEdge(touch2))
					{
						T val = createGestureFunction(touch, touch2);
						val.onStart += OnStart;
						val.onFinished += OnFinished;
						_gestures.Add(val);
					}
				}
			}
		}

		private void OnStart(T gesture)
		{
			if (this.onGestureStarted != null)
			{
				this.onGestureStarted(gesture);
			}
		}

		private void OnFinished(T gesture)
		{
			_gestures.Remove(gesture);
		}
	}
	internal class GestureTouchesUtility
	{
		private const float _edgeThresholdInches = 0.1f;

		private static GestureTouchesUtility _instance;

		private HashSet<int> _retainedFingerIds = new HashSet<int>();

		private GestureTouchesUtility()
		{
		}

		public static bool TryFindTouch(int fingerId, out Touch touch)
		{
			for (int i = 0; i < Input.touches.Length; i++)
			{
				if (Input.touches[i].fingerId == fingerId)
				{
					touch = Input.touches[i];
					return true;
				}
			}
			touch = default(Touch);
			return false;
		}

		public static float PixelsToInches(float pixels)
		{
			return pixels / Screen.dpi;
		}

		public static float InchesToPixels(float inches)
		{
			return inches * Screen.dpi;
		}

		public static bool IsTouchOffScreenEdge(Touch touch)
		{
			float num = InchesToPixels(0.1f);
			return (touch.position.x <= num) | (touch.position.y <= num) | (touch.position.x >= (float)Screen.width - num) | (touch.position.y >= (float)Screen.height - num);
		}

		public static bool RaycastFromCamera(Vector2 screenPos, out RaycastHit result)
		{
			if (Camera.main == null)
			{
				result = default(RaycastHit);
				return false;
			}
			if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPos), out var hitInfo))
			{
				result = hitInfo;
				return true;
			}
			result = hitInfo;
			return false;
		}

		public static void LockFingerId(int fingerId)
		{
			if (!IsFingerIdRetained(fingerId))
			{
				GetInstance()._retainedFingerIds.Add(fingerId);
			}
		}

		public static void ReleaseFingerId(int fingerId)
		{
			if (IsFingerIdRetained(fingerId))
			{
				GetInstance()._retainedFingerIds.Remove(fingerId);
			}
		}

		public static bool IsFingerIdRetained(int fingerId)
		{
			return GetInstance()._retainedFingerIds.Contains(fingerId);
		}

		private static GestureTouchesUtility GetInstance()
		{
			if (_instance == null)
			{
				_instance = new GestureTouchesUtility();
			}
			return _instance;
		}
	}
	public static class TransformationUtility
	{
		public enum TranslationMode
		{
			Horizontal,
			Vertical,
			Any
		}

		public struct Placement
		{
			public Vector3? HoveringPosition;

			public Vector3? PlacementPosition;

			public Quaternion? PlacementRotation;

			public TrackableHit? PlacementPlane;

			public float UpdatedGroundingPlaneHeight;
		}

		private const float _downRayOffset = 0.01f;

		private const float _maxScreenTouchOffset = 0.4f;

		private const float _hoverDistanceThreshold = 1f;

		public static Placement GetBestPlacementPosition(Vector3 currentAnchorPosition, Vector2 screenPos, float groundingPlaneHeight, float hoverOffset, float maxTranslationDistance, TranslationMode translationMode)
		{
			Placement result = new Placement
			{
				UpdatedGroundingPlaneHeight = groundingPlaneHeight
			};
			float num = Vector3.Angle(Camera.main.transform.forward, Vector3.down);
			num = 90f - num;
			float inches = Mathf.Clamp01(num / 90f) * 0.4f;
			screenPos.y += GestureTouchesUtility.InchesToPixels(inches);
			float num2 = Mathf.Clamp01(num / 45f);
			hoverOffset *= num2;
			float num3 = Mathf.Clamp01((Camera.main.transform.position - currentAnchorPosition).magnitude / 1f);
			hoverOffset *= num3;
			if (Frame.Raycast(screenPos.x, screenPos.y, TrackableHitFlags.PlaneWithinBounds, out var hitResult))
			{
				if (!(hitResult.Trackable is DetectedPlane))
				{
					return result;
				}
				DetectedPlane detectedPlane = hitResult.Trackable as DetectedPlane;
				if (!IsPlaneTypeAllowed(translationMode, detectedPlane.PlaneType))
				{
					return result;
				}
				if (hitResult.Trackable is DetectedPlane && Vector3.Dot(Camera.main.transform.position - hitResult.Pose.position, hitResult.Pose.rotation * Vector3.up) < 0f)
				{
					UnityEngine.Debug.Log("Hit at back of the current DetectedPlane");
					return result;
				}
				Vector3 value;
				if (detectedPlane.PlaneType == DetectedPlaneType.Vertical || detectedPlane.PlaneType == DetectedPlaneType.HorizontalDownwardFacing)
				{
					value = LimitTranslation(hitResult.Pose.position, currentAnchorPosition, maxTranslationDistance);
					result.PlacementPlane = hitResult;
					result.PlacementPosition = value;
					result.HoveringPosition = value;
					result.UpdatedGroundingPlaneHeight = value.y;
					result.PlacementRotation = hitResult.Pose.rotation;
					return result;
				}
				if (detectedPlane.PlaneType != DetectedPlaneType.HorizontalUpwardFacing)
				{
					return result;
				}
				if (num < 0f)
				{
					return result;
				}
				value = LimitTranslation(hitResult.Pose.position, currentAnchorPosition, maxTranslationDistance);
				result.HoveringPosition = value + Vector3.up * hoverOffset;
				if (value.y > groundingPlaneHeight)
				{
					result.PlacementPlane = hitResult;
					result.PlacementPosition = value;
					result.UpdatedGroundingPlaneHeight = hitResult.Pose.position.y;
					result.PlacementRotation = hitResult.Pose.rotation;
					return result;
				}
			}
			if (num < 0f)
			{
				return result;
			}
			Ray ray = Camera.main.ScreenPointToRay(screenPos);
			if (new Plane(Vector3.up, new Vector3(0f, groundingPlaneHeight, 0f)).Raycast(ray, out var enter))
			{
				Vector3 value = LimitTranslation(ray.GetPoint(enter), currentAnchorPosition, maxTranslationDistance);
				result.HoveringPosition = value + Vector3.up * hoverOffset;
				if (Frame.Raycast(value + Vector3.up * 0.01f, Vector3.down, out hitResult, float.PositiveInfinity, TrackableHitFlags.PlaneWithinBounds))
				{
					result.PlacementPosition = hitResult.Pose.position;
					result.PlacementPlane = hitResult;
					result.PlacementRotation = hitResult.Pose.rotation;
					return result;
				}
				return result;
			}
			return result;
		}

		private static Vector3 LimitTranslation(Vector3 desiredPosition, Vector3 currentPosition, float maxTranslationDistance)
		{
			if ((desiredPosition - currentPosition).magnitude > maxTranslationDistance)
			{
				return currentPosition + (desiredPosition - currentPosition).normalized * maxTranslationDistance;
			}
			return desiredPosition;
		}

		private static bool IsPlaneTypeAllowed(TranslationMode translationMode, DetectedPlaneType planeType)
		{
			switch (translationMode)
			{
			case TranslationMode.Any:
				return true;
			case TranslationMode.Horizontal:
				if (planeType == DetectedPlaneType.HorizontalDownwardFacing || planeType == DetectedPlaneType.HorizontalUpwardFacing)
				{
					return true;
				}
				break;
			}
			if (translationMode == TranslationMode.Vertical && planeType == DetectedPlaneType.Vertical)
			{
				return true;
			}
			return false;
		}
	}
}
namespace GoogleARCore.Examples.ObjectManipulation
{
	public class DragGesture : Gesture<DragGesture>
	{
		public int FingerId { get; private set; }

		public Vector2 StartPosition { get; private set; }

		public Vector2 Position { get; private set; }

		public Vector2 Delta { get; private set; }

		public DragGesture(DragGestureRecognizer recognizer, Touch touch)
			: base((GestureRecognizer<DragGesture>)recognizer)
		{
			FingerId = touch.fingerId;
			StartPosition = touch.position;
			Position = StartPosition;
		}

		protected internal override bool CanStart()
		{
			if (GestureTouchesUtility.IsFingerIdRetained(FingerId))
			{
				Cancel();
				return false;
			}
			if (Input.touches.Length > 1)
			{
				for (int i = 0; i < Input.touches.Length; i++)
				{
					Touch touch = Input.touches[i];
					if (touch.fingerId != FingerId && !GestureTouchesUtility.IsFingerIdRetained(touch.fingerId))
					{
						return false;
					}
				}
			}
			if (GestureTouchesUtility.TryFindTouch(FingerId, out var touch2))
			{
				if (GestureTouchesUtility.PixelsToInches((touch2.position - StartPosition).magnitude) >= 0.1f)
				{
					return true;
				}
			}
			else
			{
				Cancel();
			}
			return false;
		}

		protected internal override void OnStart()
		{
			GestureTouchesUtility.LockFingerId(FingerId);
			if (GestureTouchesUtility.RaycastFromCamera(StartPosition, out var result))
			{
				GameObject gameObject = result.transform.gameObject;
				if (gameObject != null)
				{
					base.TargetObject = gameObject.GetComponentInParent<Manipulator>().gameObject;
				}
			}
			GestureTouchesUtility.TryFindTouch(FingerId, out var touch);
			Position = touch.position;
		}

		protected internal override bool UpdateGesture()
		{
			if (GestureTouchesUtility.TryFindTouch(FingerId, out var touch))
			{
				if (touch.phase == TouchPhase.Moved)
				{
					Delta = touch.position - Position;
					Position = touch.position;
					return true;
				}
				if (touch.phase == TouchPhase.Ended)
				{
					Complete();
				}
				else if (touch.phase == TouchPhase.Canceled)
				{
					Cancel();
				}
			}
			else
			{
				Cancel();
			}
			return false;
		}

		protected internal override void OnCancel()
		{
		}

		protected internal override void OnFinish()
		{
			GestureTouchesUtility.ReleaseFingerId(FingerId);
		}
	}
	public class DragGestureRecognizer : GestureRecognizer<DragGesture>
	{
		internal const float _slopInches = 0.1f;

		internal DragGesture CreateGesture(Touch touch)
		{
			return new DragGesture(this, touch);
		}

		protected internal override void TryCreateGestures()
		{
			TryCreateOneFingerGestureOnTouchBegan(CreateGesture);
		}
	}
	public class PinchGesture : Gesture<PinchGesture>
	{
		public int FingerId1 { get; private set; }

		public int FingerId2 { get; private set; }

		public Vector2 StartPosition1 { get; private set; }

		public Vector2 StartPosition2 { get; private set; }

		public float Gap { get; private set; }

		public float GapDelta { get; private set; }

		public PinchGesture(PinchGestureRecognizer recognizer, Touch touch1, Touch touch2)
			: base((GestureRecognizer<PinchGesture>)recognizer)
		{
			FingerId1 = touch1.fingerId;
			FingerId2 = touch2.fingerId;
			StartPosition1 = touch1.position;
			StartPosition2 = touch2.position;
		}

		protected internal override bool CanStart()
		{
			if (GestureTouchesUtility.IsFingerIdRetained(FingerId1) || GestureTouchesUtility.IsFingerIdRetained(FingerId2))
			{
				Cancel();
				return false;
			}
			Touch touch;
			bool flag = GestureTouchesUtility.TryFindTouch(FingerId1, out touch);
			if (!(GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2) && flag))
			{
				Cancel();
				return false;
			}
			if (touch.deltaPosition == Vector2.zero && touch2.deltaPosition == Vector2.zero)
			{
				return false;
			}
			Vector3 vector = (StartPosition1 - StartPosition2).normalized;
			float f = Vector3.Dot(touch.deltaPosition.normalized, -vector);
			float f2 = Vector3.Dot(touch2.deltaPosition.normalized, vector);
			float num = Mathf.Cos((float)Math.PI / 6f);
			if (touch.deltaPosition != Vector2.zero && Mathf.Abs(f) < num)
			{
				return false;
			}
			if (touch2.deltaPosition != Vector2.zero && Mathf.Abs(f2) < num)
			{
				return false;
			}
			float magnitude = (StartPosition1 - StartPosition2).magnitude;
			Gap = (touch.position - touch2.position).magnitude;
			if (GestureTouchesUtility.PixelsToInches(Mathf.Abs(Gap - magnitude)) < 0.05f)
			{
				return false;
			}
			return true;
		}

		protected internal override void OnStart()
		{
			GestureTouchesUtility.LockFingerId(FingerId1);
			GestureTouchesUtility.LockFingerId(FingerId2);
		}

		protected internal override bool UpdateGesture()
		{
			Touch touch;
			bool flag = GestureTouchesUtility.TryFindTouch(FingerId1, out touch);
			if (!(GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2) && flag))
			{
				Cancel();
				return false;
			}
			if (touch.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Canceled)
			{
				Cancel();
				return false;
			}
			if (touch.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
			{
				Complete();
				return false;
			}
			if (touch.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
			{
				float magnitude = (touch.position - touch2.position).magnitude;
				GapDelta = magnitude - Gap;
				Gap = magnitude;
				return true;
			}
			return false;
		}

		protected internal override void OnCancel()
		{
		}

		protected internal override void OnFinish()
		{
			GestureTouchesUtility.ReleaseFingerId(FingerId1);
			GestureTouchesUtility.ReleaseFingerId(FingerId2);
		}
	}
	public class PinchGestureRecognizer : GestureRecognizer<PinchGesture>
	{
		internal const float _slopInches = 0.05f;

		internal const float _slopMotionDirectionDegrees = 30f;

		internal PinchGesture CreateGesture(Touch touch1, Touch touch2)
		{
			return new PinchGesture(this, touch1, touch2);
		}

		protected internal override void TryCreateGestures()
		{
			TryCreateTwoFingerGestureOnTouchBegan(CreateGesture);
		}
	}
	public class TapGesture : Gesture<TapGesture>
	{
		private float _elapsedTime;

		public int FingerId { get; private set; }

		public Vector2 StartPosition { get; private set; }

		internal TapGesture(TapGestureRecognizer recognizer, Touch touch)
			: base((GestureRecognizer<TapGesture>)recognizer)
		{
			FingerId = touch.fingerId;
			StartPosition = touch.position;
		}

		protected internal override bool CanStart()
		{
			if (GestureTouchesUtility.IsFingerIdRetained(FingerId))
			{
				Cancel();
				return false;
			}
			return true;
		}

		protected internal override void OnStart()
		{
			if (GestureTouchesUtility.RaycastFromCamera(StartPosition, out var result))
			{
				GameObject gameObject = result.transform.gameObject;
				if (gameObject != null)
				{
					base.TargetObject = gameObject.GetComponentInParent<Manipulator>().gameObject;
				}
			}
		}

		protected internal override bool UpdateGesture()
		{
			if (GestureTouchesUtility.TryFindTouch(FingerId, out var touch))
			{
				_elapsedTime += touch.deltaTime;
				if (_elapsedTime > 0.3f)
				{
					Cancel();
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					if (GestureTouchesUtility.PixelsToInches((touch.position - StartPosition).magnitude) > 0.1f)
					{
						Cancel();
					}
				}
				else if (touch.phase == TouchPhase.Ended)
				{
					Complete();
				}
			}
			else
			{
				Cancel();
			}
			return false;
		}

		protected internal override void OnCancel()
		{
		}

		protected internal override void OnFinish()
		{
		}
	}
	public class TapGestureRecognizer : GestureRecognizer<TapGesture>
	{
		internal const float _slopInches = 0.1f;

		internal const float _timeSeconds = 0.3f;

		internal TapGesture CreateGesture(Touch touch)
		{
			return new TapGesture(this, touch);
		}

		protected internal override void TryCreateGestures()
		{
			TryCreateOneFingerGestureOnTouchBegan(CreateGesture);
		}
	}
	public class TwistGesture : Gesture<TwistGesture>
	{
		private Vector2 _previousPosition1;

		private Vector2 _previousPosition2;

		public int FingerId1 { get; private set; }

		public int FingerId2 { get; private set; }

		public Vector2 StartPosition1 { get; private set; }

		public Vector2 StartPosition2 { get; private set; }

		public float DeltaRotation { get; private set; }

		public TwistGesture(TwistGestureRecognizer recognizer, Touch touch1, Touch touch2)
			: base((GestureRecognizer<TwistGesture>)recognizer)
		{
			FingerId1 = touch1.fingerId;
			FingerId2 = touch2.fingerId;
			StartPosition1 = touch1.position;
			StartPosition2 = touch2.position;
		}

		protected internal override bool CanStart()
		{
			if (GestureTouchesUtility.IsFingerIdRetained(FingerId1) || GestureTouchesUtility.IsFingerIdRetained(FingerId2))
			{
				Cancel();
				return false;
			}
			Touch touch;
			bool flag = GestureTouchesUtility.TryFindTouch(FingerId1, out touch);
			if (!(GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2) && flag))
			{
				Cancel();
				return false;
			}
			if (touch.deltaPosition == Vector2.zero || touch2.deltaPosition == Vector2.zero)
			{
				return false;
			}
			if (Mathf.Abs(CalculateDeltaRotation(touch.position, touch2.position, StartPosition1, StartPosition2)) < 10f)
			{
				return false;
			}
			return true;
		}

		protected internal override void OnStart()
		{
			GestureTouchesUtility.LockFingerId(FingerId1);
			GestureTouchesUtility.LockFingerId(FingerId2);
			GestureTouchesUtility.TryFindTouch(FingerId1, out var touch);
			GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2);
			_previousPosition1 = touch.position;
			_previousPosition2 = touch2.position;
		}

		protected internal override bool UpdateGesture()
		{
			Touch touch;
			bool flag = GestureTouchesUtility.TryFindTouch(FingerId1, out touch);
			if (!(GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2) && flag))
			{
				Cancel();
				return false;
			}
			if (touch.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Canceled)
			{
				Cancel();
				return false;
			}
			if (touch.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
			{
				Complete();
				return false;
			}
			if (touch.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
			{
				float deltaRotation = CalculateDeltaRotation(touch.position, touch2.position, _previousPosition1, _previousPosition2);
				DeltaRotation = deltaRotation;
				_previousPosition1 = touch.position;
				_previousPosition2 = touch2.position;
				return true;
			}
			_previousPosition1 = touch.position;
			_previousPosition2 = touch2.position;
			DeltaRotation = 0f;
			return false;
		}

		protected internal override void OnCancel()
		{
		}

		protected internal override void OnFinish()
		{
			GestureTouchesUtility.ReleaseFingerId(FingerId1);
			GestureTouchesUtility.ReleaseFingerId(FingerId2);
		}

		private static float CalculateDeltaRotation(Vector2 currentPosition1, Vector2 currentPosition2, Vector2 previousPosition1, Vector2 previousPosition2)
		{
			Vector2 normalized = (currentPosition1 - currentPosition2).normalized;
			Vector2 normalized2 = (previousPosition1 - previousPosition2).normalized;
			float num = Mathf.Sign(normalized2.x * normalized.y - normalized2.y * normalized.x);
			return Vector2.Angle(normalized, normalized2) * num;
		}
	}
	public class TwistGestureRecognizer : GestureRecognizer<TwistGesture>
	{
		internal const float _slopRotation = 10f;

		internal TwistGesture CreateGesture(Touch touch1, Touch touch2)
		{
			return new TwistGesture(this, touch1, touch2);
		}

		protected internal override void TryCreateGestures()
		{
			TryCreateTwoFingerGestureOnTouchBegan(CreateGesture);
		}
	}
	public class TwoFingerDragGesture : Gesture<TwoFingerDragGesture>
	{
		public int FingerId1 { get; private set; }

		public int FingerId2 { get; private set; }

		public Vector2 StartPosition1 { get; private set; }

		public Vector2 StartPosition2 { get; private set; }

		public Vector2 Position { get; private set; }

		public Vector2 Delta { get; private set; }

		public TwoFingerDragGesture(TwoFingerDragGestureRecognizer recognizer, Touch touch1, Touch touch2)
			: base((GestureRecognizer<TwoFingerDragGesture>)recognizer)
		{
			FingerId1 = touch1.fingerId;
			StartPosition1 = touch1.position;
			FingerId2 = touch2.fingerId;
			StartPosition2 = touch2.position;
			Position = (StartPosition1 + StartPosition2) / 2f;
		}

		protected internal override bool CanStart()
		{
			if (GestureTouchesUtility.IsFingerIdRetained(FingerId1) || GestureTouchesUtility.IsFingerIdRetained(FingerId2))
			{
				Cancel();
				return false;
			}
			Touch touch;
			bool flag = GestureTouchesUtility.TryFindTouch(FingerId1, out touch);
			if (!(GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2) && flag))
			{
				Cancel();
				return false;
			}
			if (touch.deltaPosition == Vector2.zero && touch2.deltaPosition == Vector2.zero)
			{
				return false;
			}
			float magnitude = (touch.position - StartPosition1).magnitude;
			float magnitude2 = (touch2.position - StartPosition2).magnitude;
			float num = 0.1f;
			if (GestureTouchesUtility.PixelsToInches(magnitude) < num || GestureTouchesUtility.PixelsToInches(magnitude2) < num)
			{
				return false;
			}
			if (Vector3.Dot(touch.deltaPosition.normalized, touch2.deltaPosition.normalized) < Mathf.Cos((float)Math.PI / 6f))
			{
				return false;
			}
			return true;
		}

		protected internal override void OnStart()
		{
			GestureTouchesUtility.LockFingerId(FingerId1);
			GestureTouchesUtility.LockFingerId(FingerId2);
			RaycastHit result2;
			if (GestureTouchesUtility.RaycastFromCamera(StartPosition1, out var result))
			{
				GameObject gameObject = result.transform.gameObject;
				if (gameObject != null)
				{
					base.TargetObject = gameObject.GetComponentInParent<Manipulator>().gameObject;
				}
			}
			else if (GestureTouchesUtility.RaycastFromCamera(StartPosition2, out result2))
			{
				GameObject gameObject2 = result2.transform.gameObject;
				if (gameObject2 != null)
				{
					base.TargetObject = gameObject2.GetComponentInParent<Manipulator>().gameObject;
				}
			}
			GestureTouchesUtility.TryFindTouch(FingerId1, out var touch);
			GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2);
			Position = (touch.position + touch2.position) / 2f;
		}

		protected internal override bool UpdateGesture()
		{
			Touch touch;
			bool flag = GestureTouchesUtility.TryFindTouch(FingerId1, out touch);
			if (!(GestureTouchesUtility.TryFindTouch(FingerId2, out var touch2) && flag))
			{
				Cancel();
				return false;
			}
			if (touch.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Canceled)
			{
				Cancel();
				return false;
			}
			if (touch.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
			{
				Complete();
				return false;
			}
			if (touch.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
			{
				Delta = (touch.position + touch2.position) / 2f - Position;
				Position = (touch.position + touch2.position) / 2f;
				return true;
			}
			return false;
		}

		protected internal override void OnCancel()
		{
		}

		protected internal override void OnFinish()
		{
			GestureTouchesUtility.ReleaseFingerId(FingerId1);
			GestureTouchesUtility.ReleaseFingerId(FingerId2);
		}
	}
	public class TwoFingerDragGestureRecognizer : GestureRecognizer<TwoFingerDragGesture>
	{
		internal const float _slopInches = 0.1f;

		internal const float _angleThresholdRadians = (float)Math.PI / 6f;

		internal TwoFingerDragGesture CreateGesture(Touch touch1, Touch touch2)
		{
			return new TwoFingerDragGesture(this, touch1, touch2);
		}

		protected internal override void TryCreateGestures()
		{
			TryCreateTwoFingerGestureOnTouchBegan(CreateGesture);
		}
	}
	public class ManipulationSystem : MonoBehaviour
	{
		private static ManipulationSystem _instance;

		private DragGestureRecognizer _dragGestureRecognizer = new DragGestureRecognizer();

		private PinchGestureRecognizer _pinchGestureRecognizer = new PinchGestureRecognizer();

		private TwoFingerDragGestureRecognizer _twoFingerDragGestureRecognizer = new TwoFingerDragGestureRecognizer();

		private TapGestureRecognizer _tapGestureRecognizer = new TapGestureRecognizer();

		private TwistGestureRecognizer _twistGestureRecognizer = new TwistGestureRecognizer();

		public static ManipulationSystem Instance
		{
			get
			{
				if (_instance == null)
				{
					ManipulationSystem[] array = UnityEngine.Object.FindObjectsOfType<ManipulationSystem>();
					if (array.Length != 0)
					{
						_instance = array[0];
					}
					else
					{
						UnityEngine.Debug.LogError("No instance of ManipulationSystem exists in the scene.");
					}
				}
				return _instance;
			}
		}

		public DragGestureRecognizer DragGestureRecognizer => _dragGestureRecognizer;

		public PinchGestureRecognizer PinchGestureRecognizer => _pinchGestureRecognizer;

		public TwoFingerDragGestureRecognizer TwoFingerDragGestureRecognizer => _twoFingerDragGestureRecognizer;

		public TapGestureRecognizer TapGestureRecognizer => _tapGestureRecognizer;

		public TwistGestureRecognizer TwistGestureRecognizer => _twistGestureRecognizer;

		public GameObject SelectedObject { get; private set; }

		public void Awake()
		{
			if (Instance != this)
			{
				UnityEngine.Debug.LogWarning("Multiple instances of ManipulationSystem detected in the scene. Only one instance can exist at a time. The duplicate instances will be destroyed.");
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
			else
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		public void Update()
		{
			DragGestureRecognizer.Update();
			PinchGestureRecognizer.Update();
			TwoFingerDragGestureRecognizer.Update();
			TapGestureRecognizer.Update();
			TwistGestureRecognizer.Update();
		}

		internal void Deselect()
		{
			SelectedObject = null;
		}

		internal void Select(GameObject target)
		{
			if (!(SelectedObject == target))
			{
				Deselect();
				SelectedObject = target;
			}
		}
	}
	[RequireComponent(typeof(SelectionManipulator))]
	public class ElevationManipulator : Manipulator
	{
		public LineRenderer LineRenderer;

		private Vector3 _origin;

		protected override bool CanStartManipulationForGesture(TwoFingerDragGesture gesture)
		{
			if (!IsSelected())
			{
				return false;
			}
			if (gesture.TargetObject != null)
			{
				return false;
			}
			if (base.transform.parent.up != Vector3.up && base.transform.parent.up != Vector3.down)
			{
				return false;
			}
			return true;
		}

		protected override void OnStartManipulation(TwoFingerDragGesture gesture)
		{
			_origin = base.transform.localPosition;
			_origin.y = base.transform.InverseTransformPoint(base.transform.parent.position).y;
			_origin = base.transform.TransformPoint(_origin);
			OnStartElevationVisualization(_origin, base.transform.position);
		}

		protected override void OnContinueManipulation(TwoFingerDragGesture gesture)
		{
			float num = 0.25f;
			float y = (Camera.main.transform.rotation * gesture.Delta).y / Screen.dpi * num;
			base.transform.Translate(0f, y, 0f);
			if (base.transform.localPosition.y < base.transform.parent.InverseTransformPoint(_origin).y)
			{
				base.transform.position = base.transform.parent.TransformPoint(new Vector3(base.transform.localPosition.x, base.transform.parent.InverseTransformPoint(_origin).y, base.transform.localPosition.z));
			}
			GetComponent<SelectionManipulator>().OnElevationChangedScaled(Mathf.Abs(base.transform.position.y - _origin.y));
			OnContinueElevationVisualization(base.transform.position);
		}

		protected override void OnEndManipulation(TwoFingerDragGesture gesture)
		{
			OnEndElevationVisualization();
		}

		private void OnStartElevationVisualization(Vector3 startPosition, Vector3 currentPosition)
		{
			if (LineRenderer != null)
			{
				LineRenderer.SetPosition(0, startPosition);
				LineRenderer.SetPosition(1, currentPosition);
				LineRenderer.enabled = true;
			}
		}

		private void OnContinueElevationVisualization(Vector3 currentPosition)
		{
			if (LineRenderer != null)
			{
				LineRenderer.SetPosition(1, currentPosition);
			}
		}

		private void OnEndElevationVisualization()
		{
			if (LineRenderer != null)
			{
				LineRenderer.enabled = false;
			}
		}
	}
	public abstract class Manipulator : MonoBehaviour
	{
		private bool _isManipulating;

		private GameObject _selectedObject;

		public void Select()
		{
			ManipulationSystem.Instance.Select(base.gameObject);
		}

		public void Deselect()
		{
			if (IsSelected())
			{
				ManipulationSystem.Instance.Deselect();
			}
		}

		public bool IsSelected()
		{
			return _selectedObject == base.gameObject;
		}

		protected virtual bool CanStartManipulationForGesture(DragGesture gesture)
		{
			return false;
		}

		protected virtual bool CanStartManipulationForGesture(PinchGesture gesture)
		{
			return false;
		}

		protected virtual bool CanStartManipulationForGesture(TapGesture gesture)
		{
			return false;
		}

		protected virtual bool CanStartManipulationForGesture(TwistGesture gesture)
		{
			return false;
		}

		protected virtual bool CanStartManipulationForGesture(TwoFingerDragGesture gesture)
		{
			return false;
		}

		protected virtual void OnStartManipulation(DragGesture gesture)
		{
		}

		protected virtual void OnStartManipulation(PinchGesture gesture)
		{
		}

		protected virtual void OnStartManipulation(TapGesture gesture)
		{
		}

		protected virtual void OnStartManipulation(TwistGesture gesture)
		{
		}

		protected virtual void OnStartManipulation(TwoFingerDragGesture gesture)
		{
		}

		protected virtual void OnContinueManipulation(DragGesture gesture)
		{
		}

		protected virtual void OnContinueManipulation(PinchGesture gesture)
		{
		}

		protected virtual void OnContinueManipulation(TapGesture gesture)
		{
		}

		protected virtual void OnContinueManipulation(TwistGesture gesture)
		{
		}

		protected virtual void OnContinueManipulation(TwoFingerDragGesture gesture)
		{
		}

		protected virtual void OnEndManipulation(DragGesture gesture)
		{
		}

		protected virtual void OnEndManipulation(PinchGesture gesture)
		{
		}

		protected virtual void OnEndManipulation(TapGesture gesture)
		{
		}

		protected virtual void OnEndManipulation(TwistGesture gesture)
		{
		}

		protected virtual void OnEndManipulation(TwoFingerDragGesture gesture)
		{
		}

		protected virtual void OnSelected()
		{
		}

		protected virtual void OnDeselected()
		{
		}

		protected virtual void OnEnable()
		{
			ConnectToRecognizers();
		}

		protected virtual void OnDisable()
		{
			DisconnectFromRecognizers();
		}

		protected virtual void Update()
		{
			if (_selectedObject == base.gameObject && ManipulationSystem.Instance.SelectedObject != base.gameObject)
			{
				_selectedObject = ManipulationSystem.Instance.SelectedObject;
				OnDeselected();
			}
			else if (_selectedObject != base.gameObject && ManipulationSystem.Instance.SelectedObject == base.gameObject)
			{
				_selectedObject = ManipulationSystem.Instance.SelectedObject;
				OnSelected();
			}
			else
			{
				_selectedObject = ManipulationSystem.Instance.SelectedObject;
			}
		}

		private void ConnectToRecognizers()
		{
			if (ManipulationSystem.Instance == null)
			{
				UnityEngine.Debug.LogError("Manipulation system not found in scene.");
				return;
			}
			DragGestureRecognizer dragGestureRecognizer = ManipulationSystem.Instance.DragGestureRecognizer;
			if (dragGestureRecognizer != null)
			{
				dragGestureRecognizer.onGestureStarted += OnGestureStarted;
			}
			PinchGestureRecognizer pinchGestureRecognizer = ManipulationSystem.Instance.PinchGestureRecognizer;
			if (pinchGestureRecognizer != null)
			{
				pinchGestureRecognizer.onGestureStarted += OnGestureStarted;
			}
			TapGestureRecognizer tapGestureRecognizer = ManipulationSystem.Instance.TapGestureRecognizer;
			if (tapGestureRecognizer != null)
			{
				tapGestureRecognizer.onGestureStarted += OnGestureStarted;
			}
			TwistGestureRecognizer twistGestureRecognizer = ManipulationSystem.Instance.TwistGestureRecognizer;
			if (twistGestureRecognizer != null)
			{
				twistGestureRecognizer.onGestureStarted += OnGestureStarted;
			}
			TwoFingerDragGestureRecognizer twoFingerDragGestureRecognizer = ManipulationSystem.Instance.TwoFingerDragGestureRecognizer;
			if (twoFingerDragGestureRecognizer != null)
			{
				twoFingerDragGestureRecognizer.onGestureStarted += OnGestureStarted;
			}
		}

		private void DisconnectFromRecognizers()
		{
			if (ManipulationSystem.Instance == null)
			{
				UnityEngine.Debug.LogError("Manipulation system not found in scene.");
				return;
			}
			DragGestureRecognizer dragGestureRecognizer = ManipulationSystem.Instance.DragGestureRecognizer;
			if (dragGestureRecognizer != null)
			{
				dragGestureRecognizer.onGestureStarted -= OnGestureStarted;
			}
			PinchGestureRecognizer pinchGestureRecognizer = ManipulationSystem.Instance.PinchGestureRecognizer;
			if (pinchGestureRecognizer != null)
			{
				pinchGestureRecognizer.onGestureStarted -= OnGestureStarted;
			}
			TapGestureRecognizer tapGestureRecognizer = ManipulationSystem.Instance.TapGestureRecognizer;
			if (tapGestureRecognizer != null)
			{
				tapGestureRecognizer.onGestureStarted -= OnGestureStarted;
			}
			TwistGestureRecognizer twistGestureRecognizer = ManipulationSystem.Instance.TwistGestureRecognizer;
			if (twistGestureRecognizer != null)
			{
				twistGestureRecognizer.onGestureStarted -= OnGestureStarted;
			}
			TwoFingerDragGestureRecognizer twoFingerDragGestureRecognizer = ManipulationSystem.Instance.TwoFingerDragGestureRecognizer;
			if (twoFingerDragGestureRecognizer != null)
			{
				twoFingerDragGestureRecognizer.onGestureStarted -= OnGestureStarted;
			}
		}

		private void OnGestureStarted(DragGesture gesture)
		{
			if (!_isManipulating && CanStartManipulationForGesture(gesture))
			{
				_isManipulating = true;
				gesture.onUpdated += OnUpdated;
				gesture.onFinished += OnFinished;
				OnStartManipulation(gesture);
			}
		}

		private void OnGestureStarted(PinchGesture gesture)
		{
			if (!_isManipulating && CanStartManipulationForGesture(gesture))
			{
				_isManipulating = true;
				gesture.onUpdated += OnUpdated;
				gesture.onFinished += OnFinished;
				OnStartManipulation(gesture);
			}
		}

		private void OnGestureStarted(TapGesture gesture)
		{
			if (!_isManipulating && CanStartManipulationForGesture(gesture))
			{
				_isManipulating = true;
				gesture.onUpdated += OnUpdated;
				gesture.onFinished += OnFinished;
				OnStartManipulation(gesture);
			}
		}

		private void OnGestureStarted(TwistGesture gesture)
		{
			if (!_isManipulating && CanStartManipulationForGesture(gesture))
			{
				_isManipulating = true;
				gesture.onUpdated += OnUpdated;
				gesture.onFinished += OnFinished;
				OnStartManipulation(gesture);
			}
		}

		private void OnGestureStarted(TwoFingerDragGesture gesture)
		{
			if (!_isManipulating && CanStartManipulationForGesture(gesture))
			{
				_isManipulating = true;
				gesture.onUpdated += OnUpdated;
				gesture.onFinished += OnFinished;
				OnStartManipulation(gesture);
			}
		}

		private void OnUpdated(DragGesture gesture)
		{
			if (_isManipulating)
			{
				if (ManipulationSystem.Instance.SelectedObject != base.gameObject)
				{
					_isManipulating = false;
					OnEndManipulation(gesture);
				}
				else
				{
					OnContinueManipulation(gesture);
				}
			}
		}

		private void OnUpdated(PinchGesture gesture)
		{
			if (_isManipulating)
			{
				if (ManipulationSystem.Instance.SelectedObject != base.gameObject)
				{
					_isManipulating = false;
					OnEndManipulation(gesture);
				}
				else
				{
					OnContinueManipulation(gesture);
				}
			}
		}

		private void OnUpdated(TapGesture gesture)
		{
			if (_isManipulating)
			{
				if (ManipulationSystem.Instance.SelectedObject != base.gameObject)
				{
					_isManipulating = false;
					OnEndManipulation(gesture);
				}
				else
				{
					OnContinueManipulation(gesture);
				}
			}
		}

		private void OnUpdated(TwistGesture gesture)
		{
			if (_isManipulating)
			{
				if (ManipulationSystem.Instance.SelectedObject != base.gameObject)
				{
					_isManipulating = false;
					OnEndManipulation(gesture);
				}
				else
				{
					OnContinueManipulation(gesture);
				}
			}
		}

		private void OnUpdated(TwoFingerDragGesture gesture)
		{
			if (_isManipulating)
			{
				if (ManipulationSystem.Instance.SelectedObject != base.gameObject)
				{
					_isManipulating = false;
					OnEndManipulation(gesture);
				}
				else
				{
					OnContinueManipulation(gesture);
				}
			}
		}

		private void OnFinished(DragGesture gesture)
		{
			_isManipulating = false;
			OnEndManipulation(gesture);
		}

		private void OnFinished(PinchGesture gesture)
		{
			_isManipulating = false;
			OnEndManipulation(gesture);
		}

		private void OnFinished(TapGesture gesture)
		{
			_isManipulating = false;
			OnEndManipulation(gesture);
		}

		private void OnFinished(TwistGesture gesture)
		{
			_isManipulating = false;
			OnEndManipulation(gesture);
		}

		private void OnFinished(TwoFingerDragGesture gesture)
		{
			_isManipulating = false;
			OnEndManipulation(gesture);
		}
	}
	public class RotationManipulator : Manipulator
	{
		private const float _rotationRateDegreesDrag = 100f;

		private const float _rotationRateDegreesTwist = 2.5f;

		protected override bool CanStartManipulationForGesture(DragGesture gesture)
		{
			if (!IsSelected())
			{
				return false;
			}
			if (gesture.TargetObject != null)
			{
				return false;
			}
			return true;
		}

		protected override bool CanStartManipulationForGesture(TwistGesture gesture)
		{
			if (!IsSelected())
			{
				return false;
			}
			if (gesture.TargetObject != null)
			{
				return false;
			}
			return true;
		}

		protected override void OnContinueManipulation(DragGesture gesture)
		{
			Quaternion quaternion = Quaternion.Inverse(Quaternion.LookRotation(Camera.main.transform.TransformPoint(Vector3.forward), Vector3.up));
			Quaternion rotation = Camera.main.transform.rotation;
			float yAngle = -1f * ((quaternion * rotation * gesture.Delta).x / Screen.dpi) * 100f;
			base.transform.Rotate(0f, yAngle, 0f);
		}

		protected override void OnContinueManipulation(TwistGesture gesture)
		{
			float yAngle = (0f - gesture.DeltaRotation) * 2.5f;
			base.transform.Rotate(0f, yAngle, 0f);
		}
	}
	public class ScaleManipulator : Manipulator
	{
		[Range(0.1f, 10f)]
		public float MinScale = 0.75f;

		[Range(0.1f, 10f)]
		public float MaxScale = 1.75f;

		private const float _elasticRatioLimit = 0.8f;

		private const float _sensitivity = 0.75f;

		private const float _elasticity = 0.15f;

		private float _currentScaleRatio;

		private bool _isScaling;

		private float _scaleDelta
		{
			get
			{
				if (MinScale > MaxScale)
				{
					UnityEngine.Debug.LogError("minScale must be smaller than maxScale.");
					return 0f;
				}
				return MaxScale - MinScale;
			}
		}

		private float _clampedScaleRatio => Mathf.Clamp01(_currentScaleRatio);

		private float _currentScale
		{
			get
			{
				float num = _clampedScaleRatio + ElasticDelta();
				return MinScale + num * _scaleDelta;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_currentScaleRatio = (base.transform.localScale.x - MinScale) / _scaleDelta;
		}

		protected override bool CanStartManipulationForGesture(PinchGesture gesture)
		{
			if (!IsSelected())
			{
				return false;
			}
			if (gesture.TargetObject != null)
			{
				return false;
			}
			return true;
		}

		protected override void OnStartManipulation(PinchGesture gesture)
		{
			_isScaling = true;
			_currentScaleRatio = (base.transform.localScale.x - MinScale) / _scaleDelta;
		}

		protected override void OnContinueManipulation(PinchGesture gesture)
		{
			_currentScaleRatio += 0.75f * GestureTouchesUtility.PixelsToInches(gesture.GapDelta);
			float currentScale = _currentScale;
			base.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
			if (_currentScaleRatio < -0.8f || _currentScaleRatio > 1.8f)
			{
				gesture.Cancel();
			}
		}

		protected override void OnEndManipulation(PinchGesture gesture)
		{
			_isScaling = false;
		}

		private float ElasticDelta()
		{
			float num = 0f;
			if (_currentScaleRatio > 1f)
			{
				num = _currentScaleRatio - 1f;
			}
			else
			{
				if (!(_currentScaleRatio < 0f))
				{
					return 0f;
				}
				num = _currentScaleRatio;
			}
			return (1f - 1f / (Mathf.Abs(num) * 0.15f + 1f)) * Mathf.Sign(num);
		}

		private void LateUpdate()
		{
			if (!_isScaling)
			{
				_currentScaleRatio = Mathf.Lerp(_currentScaleRatio, _clampedScaleRatio, Time.deltaTime * 8f);
				float currentScale = _currentScale;
				base.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
			}
		}
	}
	public class SelectionManipulator : Manipulator
	{
		public GameObject SelectionVisualization;

		private float _scaledElevation;

		public void OnElevationChanged(float elevation)
		{
			_scaledElevation = elevation * base.transform.localScale.y;
			SelectionVisualization.transform.localPosition = new Vector3(0f, 0f - elevation, 0f);
		}

		public void OnElevationChangedScaled(float scaledElevation)
		{
			_scaledElevation = scaledElevation;
			SelectionVisualization.transform.localPosition = new Vector3(0f, (0f - scaledElevation) / base.transform.localScale.y, 0f);
		}

		protected override void Update()
		{
			base.Update();
			if (base.transform.hasChanged)
			{
				float y = (0f - _scaledElevation) / base.transform.localScale.y;
				SelectionVisualization.transform.localPosition = new Vector3(0f, y, 0f);
			}
		}

		protected override bool CanStartManipulationForGesture(TapGesture gesture)
		{
			return true;
		}

		protected override void OnEndManipulation(TapGesture gesture)
		{
			if (!gesture.WasCancelled && !(ManipulationSystem.Instance == null))
			{
				if (gesture.TargetObject == base.gameObject)
				{
					Select();
				}
				TrackableHitFlags filter = TrackableHitFlags.PlaneWithinPolygon;
				if (!Frame.Raycast(gesture.StartPosition.x, gesture.StartPosition.y, filter, out var _))
				{
					Deselect();
				}
			}
		}

		protected override void OnSelected()
		{
			SelectionVisualization.SetActive(value: true);
		}

		protected override void OnDeselected()
		{
			SelectionVisualization.SetActive(value: false);
		}
	}
	[RequireComponent(typeof(SelectionManipulator))]
	public class TranslationManipulator : Manipulator
	{
		public TransformationUtility.TranslationMode ObjectTranslationMode;

		public float MaxTranslationDistance;

		private const float _positionSpeed = 12f;

		private const float _diffThreshold = 0.0001f;

		private bool _isActive;

		private Vector3 _desiredAnchorPosition;

		private Vector3 _desiredLocalPosition;

		private Quaternion _desiredRotation;

		private float _groundingPlaneHeight;

		private TrackableHit _lastHit;

		protected void Start()
		{
			_desiredLocalPosition = new Vector3(0f, 0f, 0f);
		}

		protected override void Update()
		{
			base.Update();
			UpdatePosition();
		}

		protected override bool CanStartManipulationForGesture(DragGesture gesture)
		{
			if (gesture.TargetObject == null)
			{
				return false;
			}
			if (gesture.TargetObject != base.gameObject)
			{
				return false;
			}
			Select();
			return true;
		}

		protected override void OnStartManipulation(DragGesture gesture)
		{
			_groundingPlaneHeight = base.transform.parent.position.y;
		}

		protected override void OnContinueManipulation(DragGesture gesture)
		{
			_isActive = true;
			TransformationUtility.Placement bestPlacementPosition = TransformationUtility.GetBestPlacementPosition(base.transform.parent.position, gesture.Position, _groundingPlaneHeight, 0.03f, MaxTranslationDistance, ObjectTranslationMode);
			if (!bestPlacementPosition.HoveringPosition.HasValue || !bestPlacementPosition.PlacementPosition.HasValue)
			{
				return;
			}
			_desiredLocalPosition = base.transform.parent.InverseTransformPoint(bestPlacementPosition.HoveringPosition.Value);
			_desiredAnchorPosition = bestPlacementPosition.PlacementPosition.Value;
			_groundingPlaneHeight = bestPlacementPosition.UpdatedGroundingPlaneHeight;
			if (bestPlacementPosition.PlacementRotation.HasValue)
			{
				if ((bestPlacementPosition.PlacementRotation.Value * Vector3.up - base.transform.up).magnitude > 0.0001f)
				{
					_desiredRotation = bestPlacementPosition.PlacementRotation.Value;
				}
				else
				{
					_desiredRotation = base.transform.rotation;
				}
			}
			if (bestPlacementPosition.PlacementPlane.HasValue)
			{
				_lastHit = bestPlacementPosition.PlacementPlane.Value;
			}
		}

		protected override void OnEndManipulation(DragGesture gesture)
		{
			GameObject obj = base.transform.parent.gameObject;
			Pose pose = new Pose(_desiredAnchorPosition, _lastHit.Pose.rotation);
			Vector3 position = base.transform.parent.InverseTransformPoint(pose.position);
			if (position.magnitude > MaxTranslationDistance)
			{
				position = position.normalized * MaxTranslationDistance;
			}
			pose.position = base.transform.parent.TransformPoint(position);
			Anchor anchor = _lastHit.Trackable.CreateAnchor(pose);
			base.transform.parent = anchor.transform;
			UnityEngine.Object.Destroy(obj);
			_desiredLocalPosition = Vector3.zero;
			if ((pose.rotation * Vector3.up - base.transform.up).magnitude > 0.0001f)
			{
				_desiredRotation = pose.rotation;
			}
			else
			{
				_desiredRotation = base.transform.rotation;
			}
			_isActive = true;
		}

		private void UpdatePosition()
		{
			if (_isActive)
			{
				Vector3 vector = Vector3.Lerp(base.transform.localPosition, _desiredLocalPosition, Time.deltaTime * 12f);
				if ((_desiredLocalPosition - vector).magnitude < 0.0001f)
				{
					vector = _desiredLocalPosition;
					_isActive = false;
				}
				base.transform.localPosition = vector;
				Quaternion rotation = Quaternion.Lerp(base.transform.rotation, _desiredRotation, Time.deltaTime * 12f);
				base.transform.rotation = rotation;
				float elevation = Mathf.Max(0f, 0f - base.transform.InverseTransformPoint(_desiredAnchorPosition).y);
				GetComponent<SelectionManipulator>().OnElevationChanged(elevation);
			}
		}
	}
	public class ObjectManipulationController : MonoBehaviour
	{
		private bool _isQuitting;

		public void Update()
		{
			UpdateApplicationLifecycle();
		}

		public void Awake()
		{
			Application.targetFrameRate = 60;
		}

		private void UpdateApplicationLifecycle()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
			if (Session.Status != SessionStatus.Tracking)
			{
				Screen.sleepTimeout = -2;
			}
			else
			{
				Screen.sleepTimeout = -1;
			}
			if (!_isQuitting)
			{
				if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
				{
					ShowAndroidToastMessage("Camera permission is needed to run this application.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
				else if (Session.Status.IsError())
				{
					ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
			}
		}

		private void DoQuit()
		{
			Application.Quit();
		}

		private void ShowAndroidToastMessage(string message)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (unityActivity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
				{
					toastClass.CallStatic<AndroidJavaObject>("makeText", new object[3] { unityActivity, message, 0 }).Call("show");
				});
			}
		}
	}
	public class PawnManipulator : Manipulator
	{
		public Camera FirstPersonCamera;

		public GameObject PawnPrefab;

		public GameObject ManipulatorPrefab;

		protected override bool CanStartManipulationForGesture(TapGesture gesture)
		{
			if (gesture.TargetObject == null)
			{
				return true;
			}
			return false;
		}

		protected override void OnEndManipulation(TapGesture gesture)
		{
			if (gesture.WasCancelled || gesture.TargetObject != null)
			{
				return;
			}
			TrackableHitFlags filter = TrackableHitFlags.PlaneWithinPolygon;
			if (Frame.Raycast(gesture.StartPosition.x, gesture.StartPosition.y, filter, out var hitResult))
			{
				if (hitResult.Trackable is DetectedPlane && Vector3.Dot(FirstPersonCamera.transform.position - hitResult.Pose.position, hitResult.Pose.rotation * Vector3.up) < 0f)
				{
					UnityEngine.Debug.Log("Hit at back of the current DetectedPlane");
					return;
				}
				GameObject obj = UnityEngine.Object.Instantiate(PawnPrefab, hitResult.Pose.position, hitResult.Pose.rotation);
				GameObject gameObject = UnityEngine.Object.Instantiate(ManipulatorPrefab, hitResult.Pose.position, hitResult.Pose.rotation);
				obj.transform.parent = gameObject.transform;
				Anchor anchor = hitResult.Trackable.CreateAnchor(hitResult.Pose);
				gameObject.transform.parent = anchor.transform;
				gameObject.GetComponent<Manipulator>().Select();
			}
		}
	}
}
namespace GoogleARCore.Examples.HelloAR
{
	public class HelloARController : MonoBehaviour
	{
		public DepthMenu DepthMenu;

		public InstantPlacementMenu InstantPlacementMenu;

		public GameObject InstantPlacementPrefab;

		public Camera FirstPersonCamera;

		public GameObject GameObjectVerticalPlanePrefab;

		public GameObject GameObjectHorizontalPlanePrefab;

		public GameObject GameObjectPointPrefab;

		public GameObject GameObjectDepthPointPrefab;

		private const float _prefabRotation = 180f;

		private bool _isQuitting;

		public void Awake()
		{
			Application.targetFrameRate = 60;
		}

		public void Update()
		{
			UpdateApplicationLifecycle();
			if (Input.touchCount < 1)
			{
				return;
			}
			Touch touch2;
			Touch touch = (touch2 = Input.GetTouch(0));
			if (touch.phase != TouchPhase.Began || EventSystem.current.IsPointerOverGameObject(touch2.fingerId))
			{
				return;
			}
			bool flag = false;
			TrackableHitFlags trackableHitFlags = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
			trackableHitFlags |= TrackableHitFlags.Depth;
			flag = Frame.Raycast(touch2.position.x, touch2.position.y, trackableHitFlags, out var hitResult);
			if (!flag && InstantPlacementMenu.IsInstantPlacementEnabled())
			{
				flag = Frame.RaycastInstantPlacement(touch2.position.x, touch2.position.y, 1f, out hitResult);
			}
			if (!flag)
			{
				return;
			}
			if (hitResult.Trackable is DetectedPlane && Vector3.Dot(FirstPersonCamera.transform.position - hitResult.Pose.position, hitResult.Pose.rotation * Vector3.up) < 0f)
			{
				UnityEngine.Debug.Log("Hit at back of the current DetectedPlane");
				return;
			}
			if (DepthMenu != null)
			{
				DepthMenu.ConfigureDepthBeforePlacingFirstAsset();
			}
			GameObject original = ((hitResult.Trackable is InstantPlacementPoint) ? InstantPlacementPrefab : ((hitResult.Trackable is FeaturePoint) ? GameObjectPointPrefab : ((hitResult.Trackable is DepthPoint) ? GameObjectDepthPointPrefab : ((!(hitResult.Trackable is DetectedPlane)) ? GameObjectHorizontalPlanePrefab : (((hitResult.Trackable as DetectedPlane).PlaneType != DetectedPlaneType.Vertical) ? GameObjectHorizontalPlanePrefab : GameObjectVerticalPlanePrefab)))));
			GameObject gameObject = UnityEngine.Object.Instantiate(original, hitResult.Pose.position, hitResult.Pose.rotation);
			gameObject.transform.Rotate(0f, 180f, 0f, Space.Self);
			Anchor anchor = hitResult.Trackable.CreateAnchor(hitResult.Pose);
			gameObject.transform.parent = anchor.transform;
			if (hitResult.Trackable is InstantPlacementPoint)
			{
				gameObject.GetComponentInChildren<InstantPlacementEffect>().InitializeWithTrackable(hitResult.Trackable);
			}
		}

		private void UpdateApplicationLifecycle()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
			if (Session.Status != SessionStatus.Tracking)
			{
				Screen.sleepTimeout = -2;
			}
			else
			{
				Screen.sleepTimeout = -1;
			}
			if (!_isQuitting)
			{
				if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
				{
					ShowAndroidToastMessage("Camera permission is needed to run this application.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
				else if (Session.Status.IsError())
				{
					ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
			}
		}

		private void DoQuit()
		{
			Application.Quit();
		}

		private void ShowAndroidToastMessage(string message)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (unityActivity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
				{
					toastClass.CallStatic<AndroidJavaObject>("makeText", new object[3] { unityActivity, message, 0 }).Call("show");
				});
			}
		}
	}
	public class SettingsMenu : MonoBehaviour
	{
		[SerializeField]
		[Header("Common Settings")]
		private GameObject _menuWindow;

		[SerializeField]
		private GameObject _settingMenuUi;

		[SerializeField]
		private Button _settingButton;

		[SerializeField]
		private PlaneDiscoveryGuide _planeDiscoveryGuide;

		[SerializeField]
		[Header("Depth Settings")]
		private GameObject _depthMenuUi;

		[SerializeField]
		private Button _depthButton;

		[SerializeField]
		private DepthMenu _depthMenu;

		[SerializeField]
		[Header("Instant Placement Settings")]
		private GameObject _instantPlacementMenuUi;

		[SerializeField]
		private Button _instantPlacementButton;

		public void Start()
		{
			_menuWindow.SetActive(value: false);
			_settingMenuUi.SetActive(value: false);
			_settingButton.onClick.AddListener(OnMenuButtonClick);
			_depthMenuUi.SetActive(value: false);
			_depthButton.onClick.AddListener(OnClickDepthMenu);
			_instantPlacementMenuUi.SetActive(value: false);
			_instantPlacementButton.onClick.AddListener(OnClickInstantPlacementMenu);
		}

		public void OnDestroy()
		{
			_settingButton.onClick.RemoveListener(OnMenuButtonClick);
			_depthButton.onClick.RemoveListener(OnClickDepthMenu);
			_instantPlacementButton.onClick.RemoveListener(OnClickInstantPlacementMenu);
		}

		public void OnMenuClosed()
		{
			_menuWindow.SetActive(value: false);
			_settingMenuUi.SetActive(value: false);
			_depthMenuUi.SetActive(value: false);
			_instantPlacementMenuUi.SetActive(value: false);
			_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: true);
		}

		private void OnMenuButtonClick()
		{
			_menuWindow.SetActive(value: true);
			_depthMenuUi.SetActive(value: true);
			_depthMenu.OnMenuButtonClicked();
			_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: false);
		}

		private void OnClickDepthMenu()
		{
			_settingMenuUi.SetActive(value: false);
			_depthMenuUi.SetActive(value: true);
			_depthMenu.OnMenuButtonClicked();
		}

		private void OnClickInstantPlacementMenu()
		{
			_settingMenuUi.SetActive(value: false);
			_instantPlacementMenuUi.SetActive(value: true);
		}
	}
}
namespace GoogleARCore.Examples.ComputerVision
{
	public class ComputerVisionController : MonoBehaviour
	{
		public ARCoreSession ARSessionManager;

		public Image EdgeDetectionBackgroundImage;

		public Text CameraIntrinsicsOutput;

		public Text SnackbarText;

		public Toggle LowResConfigToggle;

		public Toggle HighResConfigToggle;

		public PointClickHandler ImageTextureToggle;

		public Toggle AutoFocusToggle;

		private static float _frameRateUpdateInterval = 2f;

		private byte[] _edgeDetectionResultImage;

		private Texture2D _edgeDetectionBackgroundTexture;

		private DisplayUvCoords _cameraImageToDisplayUvTransformation;

		private ScreenOrientation? _cachedOrientation;

		private Vector2 _cachedScreenDimensions = Vector2.zero;

		private bool _isQuitting;

		private bool _useHighResCPUTexture;

		private ARCoreSession.OnChooseCameraConfigurationDelegate _onChoseCameraConfiguration;

		private int _highestResolutionConfigIndex;

		private int _lowestResolutionConfigIndex;

		private bool _resolutioninitialized;

		private Text _imageTextureToggleText;

		private float _renderingFrameRate;

		private float _renderingFrameTime;

		private int _frameCounter;

		private float _framePassedTime;

		public void Awake()
		{
			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
			Screen.autorotateToPortraitUpsideDown = false;
			Screen.orientation = ScreenOrientation.Portrait;
			Application.targetFrameRate = 60;
			_onChoseCameraConfiguration = ChooseCameraConfiguration;
			ARSessionManager.RegisterChooseCameraConfigurationCallback(_onChoseCameraConfiguration);
		}

		public void Start()
		{
			Screen.sleepTimeout = -1;
			ImageTextureToggle.OnPointClickDetected += OnBackgroundClicked;
			_imageTextureToggleText = ImageTextureToggle.GetComponentInChildren<Text>();
			SnackbarText.text = string.Empty;
		}

		public void Update()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
			QuitOnConnectionErrors();
			UpdateFrameRate();
			LowResConfigToggle.gameObject.SetActive(EdgeDetectionBackgroundImage.enabled);
			HighResConfigToggle.gameObject.SetActive(EdgeDetectionBackgroundImage.enabled);
			_imageTextureToggleText.text = (EdgeDetectionBackgroundImage.enabled ? "Switch to GPU Texture" : "Switch to CPU Image");
			if (!Session.Status.IsValid())
			{
				return;
			}
			using (CameraImageBytes cameraImageBytes = Frame.CameraImage.AcquireCameraImageBytes())
			{
				if (!cameraImageBytes.IsAvailable)
				{
					return;
				}
				OnImageAvailable(cameraImageBytes.Width, cameraImageBytes.Height, cameraImageBytes.YRowStride, cameraImageBytes.Y, 0);
			}
			CameraIntrinsics intrinsics = (EdgeDetectionBackgroundImage.enabled ? Frame.CameraImage.ImageIntrinsics : Frame.CameraImage.TextureIntrinsics);
			string intrinsicsType = (EdgeDetectionBackgroundImage.enabled ? "CPU Image" : "GPU Texture");
			CameraIntrinsicsOutput.text = CameraIntrinsicsToString(intrinsics, intrinsicsType);
		}

		public void OnLowResolutionCheckboxValueChanged(bool newValue)
		{
			_useHighResCPUTexture = !newValue;
			HighResConfigToggle.isOn = !newValue;
			ARSessionManager.enabled = false;
			ARSessionManager.enabled = true;
		}

		public void OnHighResolutionCheckboxValueChanged(bool newValue)
		{
			_useHighResCPUTexture = newValue;
			LowResConfigToggle.isOn = !newValue;
			ARSessionManager.enabled = false;
			ARSessionManager.enabled = true;
		}

		public void OnAutoFocusCheckboxValueChanged(bool autoFocusEnabled)
		{
			ARCoreSessionConfig sessionConfig = ARSessionManager.SessionConfig;
			if (sessionConfig != null)
			{
				sessionConfig.CameraFocusMode = (autoFocusEnabled ? CameraFocusMode.AutoFocus : CameraFocusMode.FixedFocus);
			}
		}

		private void OnBackgroundClicked()
		{
			EdgeDetectionBackgroundImage.enabled = !EdgeDetectionBackgroundImage.enabled;
		}

		private void UpdateFrameRate()
		{
			_frameCounter++;
			_framePassedTime += Time.deltaTime;
			if (_framePassedTime > _frameRateUpdateInterval)
			{
				_renderingFrameTime = 1000f * _framePassedTime / (float)_frameCounter;
				_renderingFrameRate = 1000f / _renderingFrameTime;
				_framePassedTime = 0f;
				_frameCounter = 0;
			}
		}

		private void OnImageAvailable(int width, int height, int rowStride, IntPtr pixelBuffer, int bufferSize)
		{
			if (EdgeDetectionBackgroundImage.enabled)
			{
				if (_edgeDetectionBackgroundTexture == null || _edgeDetectionResultImage == null || _edgeDetectionBackgroundTexture.width != width || _edgeDetectionBackgroundTexture.height != height)
				{
					_edgeDetectionBackgroundTexture = new Texture2D(width, height, TextureFormat.R8, mipChain: false, linear: false);
					_edgeDetectionResultImage = new byte[width * height];
					_cameraImageToDisplayUvTransformation = Frame.CameraImage.ImageDisplayUvs;
				}
				if (_cachedOrientation != Screen.orientation || _cachedScreenDimensions.x != (float)Screen.width || _cachedScreenDimensions.y != (float)Screen.height)
				{
					_cameraImageToDisplayUvTransformation = Frame.CameraImage.ImageDisplayUvs;
					_cachedOrientation = Screen.orientation;
					_cachedScreenDimensions = new Vector2(Screen.width, Screen.height);
				}
				if (EdgeDetector.Detect(_edgeDetectionResultImage, pixelBuffer, width, height, rowStride))
				{
					_edgeDetectionBackgroundTexture.LoadRawTextureData(_edgeDetectionResultImage);
					_edgeDetectionBackgroundTexture.Apply();
					EdgeDetectionBackgroundImage.material.SetTexture("_ImageTex", _edgeDetectionBackgroundTexture);
					EdgeDetectionBackgroundImage.material.SetVector("_UvTopLeftRight", new Vector4(_cameraImageToDisplayUvTransformation.TopLeft.x, _cameraImageToDisplayUvTransformation.TopLeft.y, _cameraImageToDisplayUvTransformation.TopRight.x, _cameraImageToDisplayUvTransformation.TopRight.y));
					EdgeDetectionBackgroundImage.material.SetVector("_UvBottomLeftRight", new Vector4(_cameraImageToDisplayUvTransformation.BottomLeft.x, _cameraImageToDisplayUvTransformation.BottomLeft.y, _cameraImageToDisplayUvTransformation.BottomRight.x, _cameraImageToDisplayUvTransformation.BottomRight.y));
				}
			}
		}

		private void QuitOnConnectionErrors()
		{
			if (!_isQuitting)
			{
				if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
				{
					ShowAndroidToastMessage("Camera permission is needed to run this application.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
				else if (Session.Status == SessionStatus.ErrorInvalidCameraConfig)
				{
					ShowAndroidToastMessage("Cannot find a valid camera config. Please try a less restrictive filter and start the app again.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
				else if (Session.Status == SessionStatus.FatalError)
				{
					ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
			}
		}

		private void ShowAndroidToastMessage(string message)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (unityActivity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
				{
					toastClass.CallStatic<AndroidJavaObject>("makeText", new object[3] { unityActivity, message, 0 }).Call("show");
				});
			}
		}

		private void DoQuit()
		{
			Application.Quit();
		}

		private string CameraIntrinsicsToString(CameraIntrinsics intrinsics, string intrinsicsType)
		{
			float num = 114.59156f * Mathf.Atan2(intrinsics.ImageDimensions.x, 2f * intrinsics.FocalLength.x);
			float num2 = 114.59156f * Mathf.Atan2(intrinsics.ImageDimensions.y, 2f * intrinsics.FocalLength.y);
			string text = ((_renderingFrameRate < 1f) ? "Calculating..." : string.Format("{0}ms ({1}fps)", _renderingFrameTime.ToString("0.0"), _renderingFrameRate.ToString("0.0")));
			return string.Format("Unrotated Camera {4} Intrinsics:{0}  Focal Length: {1}{0}  Principal Point: {2}{0}  Image Dimensions: {3}{0}  Unrotated Field of View: ({5}°, {6}°){0}Render Frame Time: {7}", Environment.NewLine, intrinsics.FocalLength.ToString(), intrinsics.PrincipalPoint.ToString(), intrinsics.ImageDimensions.ToString(), intrinsicsType, num, num2, text);
		}

		private int ChooseCameraConfiguration(List<CameraConfig> supportedConfigurations)
		{
			if (!_resolutioninitialized)
			{
				_highestResolutionConfigIndex = 0;
				_lowestResolutionConfigIndex = 0;
				CameraConfig cameraConfig = supportedConfigurations[0];
				CameraConfig cameraConfig2 = supportedConfigurations[0];
				for (int i = 1; i < supportedConfigurations.Count; i++)
				{
					CameraConfig cameraConfig3 = supportedConfigurations[i];
					if ((cameraConfig3.ImageSize.x > cameraConfig.ImageSize.x && cameraConfig3.ImageSize.y > cameraConfig.ImageSize.y) || (cameraConfig3.ImageSize.x == cameraConfig.ImageSize.x && cameraConfig3.ImageSize.y == cameraConfig.ImageSize.y && cameraConfig3.MaxFPS > cameraConfig.MaxFPS))
					{
						_highestResolutionConfigIndex = i;
						cameraConfig = cameraConfig3;
					}
					if ((cameraConfig3.ImageSize.x < cameraConfig2.ImageSize.x && cameraConfig3.ImageSize.y < cameraConfig2.ImageSize.y) || (cameraConfig3.ImageSize.x == cameraConfig2.ImageSize.x && cameraConfig3.ImageSize.y == cameraConfig2.ImageSize.y && cameraConfig3.MaxFPS > cameraConfig2.MaxFPS))
					{
						_lowestResolutionConfigIndex = i;
						cameraConfig2 = cameraConfig3;
					}
				}
				string empty = string.Empty;
				string empty2 = string.Empty;
				empty += $"Facing Direction: {cameraConfig2.FacingDirection}, ";
				empty2 += $"Facing Direction: {cameraConfig.FacingDirection}, ";
				empty += $"Low Resolution CPU Image ({cameraConfig2.ImageSize.x} x {cameraConfig2.ImageSize.y}), Target FPS: ({cameraConfig2.MinFPS} - {cameraConfig2.MaxFPS}), Depth Sensor Usage: {cameraConfig2.DepthSensorUsage}";
				empty2 = $"High Resolution CPU Image ({cameraConfig.ImageSize.x} x {cameraConfig.ImageSize.y}), Target FPS: ({cameraConfig.MinFPS} - {cameraConfig.MaxFPS}), Depth Sensor Usage: {cameraConfig.DepthSensorUsage}";
				empty += $", Stereo Camera Usage: {cameraConfig2.StereoCameraUsage}";
				empty2 += $", Stereo Camera Usage: {cameraConfig.StereoCameraUsage}";
				LowResConfigToggle.GetComponentInChildren<Text>().text = empty;
				HighResConfigToggle.GetComponentInChildren<Text>().text = empty2;
				_resolutioninitialized = true;
			}
			if (_useHighResCPUTexture)
			{
				return _highestResolutionConfigIndex;
			}
			return _lowestResolutionConfigIndex;
		}
	}
	public class EdgeDetector
	{
		private static byte[] _imageBuffer = new byte[0];

		private static int _imageBufferSize = 0;

		public static bool Detect(byte[] outputImage, IntPtr pixelBuffer, int width, int height, int rowStride)
		{
			if (outputImage.Length < width * height)
			{
				UnityEngine.Debug.Log("Input buffer is too small!");
				return false;
			}
			Sobel(outputImage, pixelBuffer, width, height, rowStride);
			return true;
		}

		private static void Sobel(byte[] outputImage, IntPtr inputImage, int width, int height, int rowStride)
		{
			int num = rowStride * height;
			if (num != _imageBufferSize || _imageBuffer.Length == 0)
			{
				_imageBufferSize = num;
				_imageBuffer = new byte[num];
			}
			Marshal.Copy(inputImage, _imageBuffer, 0, num);
			int num2 = 16384;
			for (int i = 1; i < height - 1; i++)
			{
				for (int j = 1; j < width - 1; j++)
				{
					int num3 = i * rowStride + j;
					byte num4 = _imageBuffer[num3 - rowStride - 1];
					int num5 = _imageBuffer[num3 - rowStride];
					int num6 = _imageBuffer[num3 - rowStride + 1];
					int num7 = _imageBuffer[num3 - 1];
					int num8 = _imageBuffer[num3 + 1];
					int num9 = _imageBuffer[num3 + rowStride - 1];
					int num10 = _imageBuffer[num3 + rowStride];
					int num11 = _imageBuffer[num3 + rowStride + 1];
					int num12 = -num4 - 2 * num7 - num9 + num6 + 2 * num8 + num11;
					int num13 = num4 + 2 * num5 + num6 - num9 - 2 * num10 - num11;
					if (num12 * num12 + num13 * num13 > num2)
					{
						outputImage[i * width + j] = byte.MaxValue;
					}
					else
					{
						outputImage[i * width + j] = 31;
					}
				}
			}
		}
	}
	public class PointClickHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public event Action OnPointClickDetected;

		public void OnPointerClick(PointerEventData pointerEventData)
		{
			if (this.OnPointClickDetected != null)
			{
				this.OnPointClickDetected();
			}
		}
	}
	public class TextureReader : MonoBehaviour
	{
		public delegate void OnImageAvailableCallbackFunc(TextureReaderApi.ImageFormatType format, int width, int height, IntPtr pixelBuffer, int bufferSize);

		public enum SampleMode
		{
			KeepAspectRatio,
			CoverFullViewport
		}

		private enum CommandType
		{
			None,
			ProcessNextFrame,
			Create,
			Reset,
			ReleasePreviousBuffer
		}

		public int ImageWidth = 1920;

		public int ImageHeight = 1080;

		public SampleMode ImageSampleMode = SampleMode.CoverFullViewport;

		public TextureReaderApi.ImageFormatType ImageFormat = TextureReaderApi.ImageFormatType.ImageFormatGrayscale;

		private const int _arCoreTextureWidth = 1920;

		private const int _arCoreTextureHeight = 1080;

		private TextureReaderApi _textureReaderApi;

		private CommandType _command;

		private int _imageBufferIndex = -1;

		public event OnImageAvailableCallbackFunc OnImageAvailableCallback;

		public void Start()
		{
			if (_textureReaderApi == null)
			{
				_textureReaderApi = new TextureReaderApi();
				_command = CommandType.Create;
				_imageBufferIndex = -1;
			}
		}

		public void Apply()
		{
			_command = CommandType.Reset;
		}

		public void Update()
		{
			if (!base.enabled)
			{
				return;
			}
			switch (_command)
			{
			case CommandType.Create:
				_textureReaderApi.Create(ImageFormat, ImageWidth, ImageHeight, ImageSampleMode == SampleMode.KeepAspectRatio);
				break;
			case CommandType.Reset:
				_textureReaderApi.ReleaseFrame(_imageBufferIndex);
				_textureReaderApi.Destroy();
				_textureReaderApi.Create(ImageFormat, ImageWidth, ImageHeight, ImageSampleMode == SampleMode.KeepAspectRatio);
				_imageBufferIndex = -1;
				break;
			case CommandType.ReleasePreviousBuffer:
				_textureReaderApi.ReleaseFrame(_imageBufferIndex);
				_imageBufferIndex = -1;
				break;
			case CommandType.ProcessNextFrame:
				if (_imageBufferIndex >= 0)
				{
					int bufferSize = 0;
					IntPtr intPtr = _textureReaderApi.AcquireFrame(_imageBufferIndex, ref bufferSize);
					if (intPtr != IntPtr.Zero && this.OnImageAvailableCallback != null)
					{
						this.OnImageAvailableCallback(ImageFormat, ImageWidth, ImageHeight, intPtr, bufferSize);
					}
					_textureReaderApi.ReleaseFrame(_imageBufferIndex);
				}
				break;
			}
			if (Frame.CameraImage.Texture != null)
			{
				int textureId = Frame.CameraImage.Texture.GetNativeTexturePtr().ToInt32();
				_imageBufferIndex = _textureReaderApi.SubmitFrame(textureId, 1920, 1080);
			}
			_command = CommandType.ProcessNextFrame;
		}

		private void OnDestroy()
		{
			if (_textureReaderApi != null)
			{
				_textureReaderApi.Destroy();
				_textureReaderApi = null;
			}
		}

		private void OnDisable()
		{
			_command = CommandType.ReleasePreviousBuffer;
		}
	}
	public class TextureReaderApi
	{
		public enum ImageFormatType
		{
			ImageFormatColor,
			ImageFormatGrayscale
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct ExternApi
		{
			public const string ARCoreCameraUtilityAPI = "arcore_camera_utility";

			[DllImport("arcore_camera_utility")]
			public static extern void TextureReader_create(int format, int width, int height, bool keepAspectRatio);

			[DllImport("arcore_camera_utility")]
			public static extern void TextureReader_destroy();

			[DllImport("arcore_camera_utility")]
			public static extern int TextureReader_submitFrame(int textureId, int textureWidth, int textureHeight);

			[DllImport("arcore_camera_utility")]
			public static extern IntPtr TextureReader_acquireFrame(int bufferIndex, ref int bufferSize);

			[DllImport("arcore_camera_utility")]
			public static extern void TextureReader_releaseFrame(int bufferIndex);
		}

		public void Create(ImageFormatType format, int width, int height, bool keepAspectRatio)
		{
			ExternApi.TextureReader_create((int)format, width, height, keepAspectRatio);
		}

		public void Destroy()
		{
			ExternApi.TextureReader_destroy();
		}

		public int SubmitFrame(int textureId, int textureWidth, int textureHeight)
		{
			int result = ExternApi.TextureReader_submitFrame(textureId, textureWidth, textureHeight);
			GL.InvalidateState();
			return result;
		}

		public IntPtr AcquireFrame(int bufferIndex, ref int bufferSize)
		{
			return ExternApi.TextureReader_acquireFrame(bufferIndex, ref bufferSize);
		}

		public void ReleaseFrame(int bufferIndex)
		{
			ExternApi.TextureReader_releaseFrame(bufferIndex);
		}
	}
}
namespace GoogleARCore.Examples.Common
{
	[RequireComponent(typeof(Camera))]
	public class DepthEffect : MonoBehaviour
	{
		public const string BackgroundTexturePropertyName = "_BackgroundTexture";

		public Shader OcclusionShader;

		public DepthMenu DepthMenu;

		[Range(0f, 1f)]
		public float OcclusionTransparency = 1f;

		[Space]
		public float OcclusionOffset = 0.08f;

		public float OcclusionFadeVelocity = 4f;

		public float TransitionSize = 0.1f;

		private static readonly string _currentDepthTexturePropertyName = "_CurrentDepthTexture";

		private static readonly string _topLeftRightPropertyName = "_UvTopLeftRight";

		private static readonly string _bottomLeftRightPropertyName = "_UvBottomLeftRight";

		private Camera _camera;

		private Material _depthMaterial;

		private Texture2D _depthTexture;

		private float _currentOcclusionTransparency = 1f;

		private ARCoreBackgroundRenderer _backgroundRenderer;

		private CommandBuffer _depthBuffer;

		private CommandBuffer _backgroundBuffer;

		private int _backgroundTextureID = -1;

		public void Awake()
		{
			_currentOcclusionTransparency = OcclusionTransparency;
			_depthMaterial = new Material(OcclusionShader);
			_depthMaterial.SetFloat("_OcclusionTransparency", _currentOcclusionTransparency);
			_depthMaterial.SetFloat("_OcclusionOffsetMeters", OcclusionOffset);
			_depthMaterial.SetFloat("_TransitionSize", TransitionSize);
			_depthTexture = new Texture2D(2, 2);
			_depthTexture.filterMode = FilterMode.Bilinear;
			_depthMaterial.SetTexture(_currentDepthTexturePropertyName, _depthTexture);
			_camera = Camera.main;
			_camera.depthTextureMode |= DepthTextureMode.Depth;
			_depthBuffer = new CommandBuffer();
			_depthBuffer.name = "Auxilary occlusion textures";
			int num = Shader.PropertyToID("_OcclusionMap");
			_depthBuffer.GetTemporaryRT(num, -1, -1, 0, FilterMode.Bilinear);
			_depthBuffer.Blit(BuiltinRenderTextureType.CameraTarget, num, _depthMaterial, 0);
			_depthBuffer.SetGlobalTexture("_OcclusionMap", num);
			_camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _depthBuffer);
			_camera.AddCommandBuffer(CameraEvent.AfterGBuffer, _depthBuffer);
			_backgroundRenderer = UnityEngine.Object.FindObjectOfType<ARCoreBackgroundRenderer>();
			if (_backgroundRenderer == null)
			{
				UnityEngine.Debug.LogError("BackgroundTextureProvider requires ARCoreBackgroundRenderer anywhere in the scene.");
				return;
			}
			_backgroundBuffer = new CommandBuffer();
			_backgroundBuffer.name = "Camera texture";
			_backgroundTextureID = Shader.PropertyToID("_BackgroundTexture");
			_backgroundBuffer.GetTemporaryRT(_backgroundTextureID, -1, -1, 0, FilterMode.Bilinear);
			Material backgroundMaterial = _backgroundRenderer.BackgroundMaterial;
			if (backgroundMaterial != null)
			{
				_backgroundBuffer.Blit(backgroundMaterial.mainTexture, _backgroundTextureID, backgroundMaterial);
			}
			_backgroundBuffer.SetGlobalTexture("_BackgroundTexture", _backgroundTextureID);
			_camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _backgroundBuffer);
			_camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, _backgroundBuffer);
		}

		public void Update()
		{
			_currentOcclusionTransparency += (OcclusionTransparency - _currentOcclusionTransparency) * Time.deltaTime * OcclusionFadeVelocity;
			_currentOcclusionTransparency = Mathf.Clamp(_currentOcclusionTransparency, 0f, OcclusionTransparency);
			_depthMaterial.SetFloat("_OcclusionTransparency", _currentOcclusionTransparency);
			_depthMaterial.SetFloat("_TransitionSize", TransitionSize);
			if (Session.Status == SessionStatus.Tracking && DepthMenu != null && DepthMenu.IsDepthEnabled())
			{
				Frame.CameraImage.UpdateDepthTexture(ref _depthTexture);
			}
			UpdateScreenOrientationOnMaterial();
		}

		public void OnEnable()
		{
			if (_depthBuffer != null)
			{
				_camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _depthBuffer);
				_camera.AddCommandBuffer(CameraEvent.AfterGBuffer, _depthBuffer);
			}
			if (_backgroundBuffer != null)
			{
				_camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _backgroundBuffer);
				_camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, _backgroundBuffer);
			}
		}

		public void OnDisable()
		{
			if (_depthBuffer != null)
			{
				_camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _depthBuffer);
				_camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, _depthBuffer);
			}
			if (_backgroundBuffer != null)
			{
				_camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _backgroundBuffer);
				_camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, _backgroundBuffer);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Session.Status == SessionStatus.Tracking)
			{
				Graphics.Blit(source, destination, _depthMaterial, 1);
			}
		}

		private void UpdateScreenOrientationOnMaterial()
		{
			DisplayUvCoords textureDisplayUvs = Frame.CameraImage.TextureDisplayUvs;
			_depthMaterial.SetVector(_topLeftRightPropertyName, new Vector4(textureDisplayUvs.TopLeft.x, textureDisplayUvs.TopLeft.y, textureDisplayUvs.TopRight.x, textureDisplayUvs.TopRight.y));
			_depthMaterial.SetVector(_bottomLeftRightPropertyName, new Vector4(textureDisplayUvs.BottomLeft.x, textureDisplayUvs.BottomLeft.y, textureDisplayUvs.BottomRight.x, textureDisplayUvs.BottomRight.y));
		}
	}
	public class DepthMenu : MonoBehaviour
	{
		public enum DepthState
		{
			DepthNotAvailable,
			DepthDisabled,
			DepthEnabled,
			DepthMap
		}

		[FormerlySerializedAs("m_PlaneDiscoveryGuide")]
		[SerializeField]
		private PlaneDiscoveryGuide _planeDiscoveryGuide;

		[FormerlySerializedAs("m_DebugVisualizer")]
		[SerializeField]
		private GameObject _debugVisualizer;

		[FormerlySerializedAs("m_Camera")]
		[SerializeField]
		private Camera _camera;

		[FormerlySerializedAs("m_DepthCardWindow")]
		[SerializeField]
		private GameObject _depthCardWindow;

		[SerializeField]
		[FormerlySerializedAs("m_ApplyButton")]
		private Button _applyButton;

		[SerializeField]
		[FormerlySerializedAs("m_CancelButton")]
		private Button _cancelButton;

		[SerializeField]
		[FormerlySerializedAs("m_EnableDepthButton")]
		private Button _enableDepthButton;

		[SerializeField]
		[FormerlySerializedAs("m_DisableDepthButton")]
		private Button _disableDepthButton;

		[SerializeField]
		[FormerlySerializedAs("m_MenuText")]
		private Text _menuText;

		[SerializeField]
		[FormerlySerializedAs("m_EnableDepthToggle")]
		private Toggle _enableDepthToggle;

		[SerializeField]
		[FormerlySerializedAs("m_EnableDepthToggleLabel")]
		private Text _enableDepthToggleLabel;

		[SerializeField]
		[FormerlySerializedAs("m_DepthMapToggle")]
		private Toggle _depthMapToggle;

		[FormerlySerializedAs("m_DepthMapToggleLabel")]
		[SerializeField]
		private Text _depthMapToggleLabel;

		private bool _depthConfigured;

		private DepthState _depthState;

		public void Start()
		{
			_applyButton.onClick.AddListener(OnApplyButtonClicked);
			_cancelButton.onClick.AddListener(OnCancelButtonClicked);
			_enableDepthButton.onClick.AddListener(OnEnableDepthButtonClicked);
			_disableDepthButton.onClick.AddListener(OnDisableDepthButtonClicked);
			_enableDepthToggle.onValueChanged.AddListener(OnEnableDepthToggleValueChanged);
			_depthCardWindow.SetActive(value: false);
			_debugVisualizer.SetActive(value: false);
		}

		public void OnDestroy()
		{
			_applyButton.onClick.RemoveListener(OnApplyButtonClicked);
			_cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
			_enableDepthButton.onClick.RemoveListener(OnEnableDepthButtonClicked);
			_disableDepthButton.onClick.RemoveListener(OnDisableDepthButtonClicked);
			_enableDepthToggle.onValueChanged.RemoveListener(OnEnableDepthToggleValueChanged);
		}

		public void ConfigureDepthBeforePlacingFirstAsset()
		{
			if (!_depthConfigured)
			{
				if (Session.IsDepthModeSupported(DepthMode.Automatic))
				{
					_depthState = DepthState.DepthDisabled;
					_menuText.text = "Your device supports depth.";
					_depthCardWindow.SetActive(value: true);
					_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: false);
				}
				else
				{
					_depthState = DepthState.DepthNotAvailable;
					_menuText.text = "Your device doesn't support depth.";
				}
				_depthConfigured = true;
			}
		}

		public bool IsDepthEnabled()
		{
			if (_depthState != DepthState.DepthEnabled)
			{
				return _depthState == DepthState.DepthMap;
			}
			return true;
		}

		public void OnMenuButtonClicked()
		{
			if (!_depthConfigured)
			{
				if (Session.IsDepthModeSupported(DepthMode.Automatic))
				{
					_depthState = DepthState.DepthDisabled;
					_menuText.text = "Your device supports depth.";
				}
				else
				{
					ConfigureDepth(depthEnabled: false);
					_depthState = DepthState.DepthNotAvailable;
					_menuText.text = "Your device doesn't support depth.";
				}
				_depthConfigured = true;
				ApplyDepthState();
			}
			_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: false);
		}

		private void OnApplyButtonClicked()
		{
			ConfigureDepth(_enableDepthToggle.isOn);
			if (_depthMapToggle.isOn)
			{
				_depthState = DepthState.DepthMap;
				_debugVisualizer.SetActive(value: true);
			}
			else
			{
				_debugVisualizer.SetActive(value: false);
			}
			_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: true);
		}

		private void OnCancelButtonClicked()
		{
			ApplyDepthState();
			_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: true);
		}

		private void OnEnableDepthButtonClicked()
		{
			ConfigureDepth(depthEnabled: true);
			ApplyDepthState();
			_depthCardWindow.SetActive(value: false);
			_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: true);
		}

		private void OnDisableDepthButtonClicked()
		{
			ConfigureDepth(depthEnabled: false);
			ApplyDepthState();
			_depthCardWindow.SetActive(value: false);
			_planeDiscoveryGuide.EnablePlaneDiscoveryGuide(guideEnabled: true);
		}

		private void OnEnableDepthToggleValueChanged(bool enabled)
		{
			if (enabled)
			{
				_depthMapToggle.interactable = true;
				_depthMapToggleLabel.color = Color.black;
			}
			else
			{
				_depthMapToggle.interactable = false;
				_depthMapToggle.isOn = false;
				_depthMapToggleLabel.color = _enableDepthToggle.colors.disabledColor;
			}
		}

		private void ConfigureDepth(bool depthEnabled)
		{
			(_camera.GetComponent(typeof(DepthEffect)) as MonoBehaviour).enabled = depthEnabled;
			_depthState = ((!depthEnabled) ? DepthState.DepthDisabled : DepthState.DepthEnabled);
		}

		private void ApplyDepthState()
		{
			switch (_depthState)
			{
			case DepthState.DepthEnabled:
				_enableDepthToggle.interactable = true;
				_enableDepthToggleLabel.color = Color.black;
				_depthMapToggle.interactable = true;
				_depthMapToggleLabel.color = Color.black;
				_enableDepthToggle.isOn = true;
				_depthMapToggle.isOn = false;
				break;
			case DepthState.DepthDisabled:
				_enableDepthToggle.interactable = true;
				_enableDepthToggleLabel.color = Color.black;
				_depthMapToggle.interactable = false;
				_depthMapToggleLabel.color = _enableDepthToggle.colors.disabledColor;
				_enableDepthToggle.isOn = false;
				_depthMapToggle.isOn = false;
				break;
			case DepthState.DepthMap:
				_enableDepthToggle.interactable = true;
				_enableDepthToggleLabel.color = Color.black;
				_depthMapToggle.interactable = true;
				_depthMapToggleLabel.color = Color.black;
				_enableDepthToggle.isOn = true;
				_depthMapToggle.isOn = true;
				break;
			default:
				_enableDepthToggle.interactable = false;
				_enableDepthToggleLabel.color = _enableDepthToggle.colors.disabledColor;
				_depthMapToggle.interactable = false;
				_depthMapToggleLabel.color = _enableDepthToggle.colors.disabledColor;
				_enableDepthToggle.isOn = false;
				_depthMapToggle.isOn = false;
				break;
			}
		}
	}
	[RequireComponent(typeof(Renderer))]
	public class DepthTexture : MonoBehaviour
	{
		private static readonly string _currentDepthTexturePropertyName = "_CurrentDepthTexture";

		private static readonly string _topLeftRightPropertyName = "_UvTopLeftRight";

		private static readonly string _bottomLeftRightPropertyName = "_UvBottomLeftRight";

		private Texture2D _depthTexture;

		private Material _material;

		public void Start()
		{
			_depthTexture = new Texture2D(2, 2);
			_depthTexture.filterMode = FilterMode.Bilinear;
			_material = GetComponent<Renderer>().sharedMaterial;
			_material.SetTexture(_currentDepthTexturePropertyName, _depthTexture);
			UpdateScreenOrientationOnMaterial();
		}

		public void Update()
		{
			Frame.CameraImage.UpdateDepthTexture(ref _depthTexture);
			UpdateScreenOrientationOnMaterial();
		}

		private void UpdateScreenOrientationOnMaterial()
		{
			DisplayUvCoords textureDisplayUvs = Frame.CameraImage.TextureDisplayUvs;
			_material.SetVector(_topLeftRightPropertyName, new Vector4(textureDisplayUvs.TopLeft.x, textureDisplayUvs.TopLeft.y, textureDisplayUvs.TopRight.x, textureDisplayUvs.TopRight.y));
			_material.SetVector(_bottomLeftRightPropertyName, new Vector4(textureDisplayUvs.BottomLeft.x, textureDisplayUvs.BottomLeft.y, textureDisplayUvs.BottomRight.x, textureDisplayUvs.BottomRight.y));
		}
	}
	public class DetectedPlaneGenerator : MonoBehaviour
	{
		public GameObject DetectedPlanePrefab;

		private List<DetectedPlane> _newPlanes = new List<DetectedPlane>();

		public void Update()
		{
			if (Session.Status == SessionStatus.Tracking)
			{
				Session.GetTrackables(_newPlanes, TrackableQueryFilter.New);
				for (int i = 0; i < _newPlanes.Count; i++)
				{
					UnityEngine.Object.Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, base.transform).GetComponent<DetectedPlaneVisualizer>().Initialize(_newPlanes[i]);
				}
			}
		}
	}
	public class DetectedPlaneVisualizer : MonoBehaviour
	{
		private DetectedPlane _detectedPlane;

		private List<Vector3> _previousFrameMeshVertices = new List<Vector3>();

		private List<Vector3> _meshVertices = new List<Vector3>();

		private Vector3 _planeCenter;

		private List<Color> _meshColors = new List<Color>();

		private List<int> _meshIndices = new List<int>();

		private Mesh _mesh;

		private MeshRenderer _meshRenderer;

		public void Awake()
		{
			_mesh = GetComponent<MeshFilter>().mesh;
			_meshRenderer = GetComponent<MeshRenderer>();
		}

		public void Update()
		{
			if (_detectedPlane != null)
			{
				if (_detectedPlane.SubsumedBy != null)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				if (_detectedPlane.TrackingState != TrackingState.Tracking)
				{
					_meshRenderer.enabled = false;
					return;
				}
				_meshRenderer.enabled = true;
				UpdateMeshIfNeeded();
			}
		}

		public void Initialize(DetectedPlane plane)
		{
			_detectedPlane = plane;
			_meshRenderer.material.SetColor("_GridColor", Color.white);
			_meshRenderer.material.SetFloat("_UvRotation", UnityEngine.Random.Range(0f, 360f));
			Update();
		}

		private void UpdateMeshIfNeeded()
		{
			_detectedPlane.GetBoundaryPolygon(_meshVertices);
			if (!AreVerticesListsEqual(_previousFrameMeshVertices, _meshVertices))
			{
				_previousFrameMeshVertices.Clear();
				_previousFrameMeshVertices.AddRange(_meshVertices);
				_planeCenter = _detectedPlane.CenterPose.position;
				Vector3 vector = _detectedPlane.CenterPose.rotation * Vector3.up;
				_meshRenderer.material.SetVector("_PlaneNormal", vector);
				int count = _meshVertices.Count;
				_meshColors.Clear();
				for (int i = 0; i < count; i++)
				{
					_meshColors.Add(Color.clear);
				}
				for (int j = 0; j < count; j++)
				{
					Vector3 vector2 = _meshVertices[j] - _planeCenter;
					float num = 1f - Mathf.Min(0.2f / vector2.magnitude, 0.2f);
					_meshVertices.Add(num * vector2 + _planeCenter);
					_meshColors.Add(Color.white);
				}
				_meshIndices.Clear();
				int num2 = 0;
				int num3 = count;
				for (int k = 0; k < count - 2; k++)
				{
					_meshIndices.Add(num3);
					_meshIndices.Add(num3 + k + 1);
					_meshIndices.Add(num3 + k + 2);
				}
				for (int l = 0; l < count; l++)
				{
					int item = num2 + l;
					int item2 = num2 + (l + 1) % count;
					int item3 = num3 + l;
					int item4 = num3 + (l + 1) % count;
					_meshIndices.Add(item);
					_meshIndices.Add(item2);
					_meshIndices.Add(item3);
					_meshIndices.Add(item3);
					_meshIndices.Add(item2);
					_meshIndices.Add(item4);
				}
				_mesh.Clear();
				_mesh.SetVertices(_meshVertices);
				_mesh.SetTriangles(_meshIndices, 0);
				_mesh.SetColors(_meshColors);
			}
		}

		private bool AreVerticesListsEqual(List<Vector3> firstList, List<Vector3> secondList)
		{
			if (firstList.Count != secondList.Count)
			{
				return false;
			}
			for (int i = 0; i < firstList.Count; i++)
			{
				if (firstList[i] != secondList[i])
				{
					return false;
				}
			}
			return true;
		}
	}
	[RequireComponent(typeof(MeshRenderer))]
	public class InstantPlacementEffect : MonoBehaviour
	{
		public Material[] HolographicMaterials;

		public Material[] OriginalMaterials;

		private bool _isOn;

		private InstantPlacementPoint _instantPlacementPoint;

		public void InitializeWithTrackable(Trackable trackable)
		{
			if (trackable is InstantPlacementPoint)
			{
				_instantPlacementPoint = trackable as InstantPlacementPoint;
				_isOn = _instantPlacementPoint.TrackingMethod != InstantPlacementPointTrackingMethod.FullTracking;
			}
			else
			{
				_isOn = false;
			}
			if (_isOn)
			{
				GetComponent<MeshRenderer>().materials = HolographicMaterials;
			}
		}

		public void Update()
		{
			if (_isOn && _instantPlacementPoint != null && _instantPlacementPoint.TrackingMethod == InstantPlacementPointTrackingMethod.FullTracking)
			{
				GetComponent<MeshRenderer>().materials = OriginalMaterials;
				_isOn = false;
			}
		}
	}
	public class InstantPlacementMenu : MonoBehaviour
	{
		[SerializeField]
		private ARCoreSession _arCoreSession;

		[SerializeField]
		private Toggle _instantPlacementToggle;

		[SerializeField]
		private Button _applyButton;

		[SerializeField]
		private Button _cancelButton;

		public void Start()
		{
			_applyButton.onClick.AddListener(ApplySettings);
			_cancelButton.onClick.AddListener(ResetSettings);
		}

		public void OnDestroy()
		{
			_applyButton.onClick.RemoveListener(ApplySettings);
			_cancelButton.onClick.RemoveListener(ResetSettings);
		}

		public bool IsInstantPlacementEnabled()
		{
			return _arCoreSession.SessionConfig.InstantPlacementMode != InstantPlacementMode.Disabled;
		}

		private void ApplySettings()
		{
			if (!(_instantPlacementToggle == null) && !(_arCoreSession == null))
			{
				InstantPlacementMode instantPlacementMode = (_instantPlacementToggle.isOn ? InstantPlacementMode.LocalYUp : InstantPlacementMode.Disabled);
				_arCoreSession.SessionConfig.InstantPlacementMode = instantPlacementMode;
			}
		}

		private void ResetSettings()
		{
			if (!(_instantPlacementToggle == null) && !(_arCoreSession == null))
			{
				_instantPlacementToggle.isOn = _arCoreSession.SessionConfig.InstantPlacementMode != InstantPlacementMode.Disabled;
			}
		}
	}
	public class PlaneDiscoveryGuide : MonoBehaviour
	{
		[Tooltip("The time to delay, after ARCore loses tracking of any planes, showing the plane discovery guide.")]
		public float DisplayGuideDelay = 3f;

		[Tooltip("The time to delay, after displaying the plane discovery guide, offering more detailed instructions on how to find a plane.")]
		public float OfferDetailedInstructionsDelay = 8f;

		private const float _onStartDelay = 1f;

		private const float _hideGuideDelay = 0.75f;

		private const float _animationFadeDuration = 0.15f;

		[SerializeField]
		[FormerlySerializedAs("m_FeaturePoints")]
		[Tooltip("The Game Object that provides feature points visualization.")]
		private GameObject _featurePoints;

		[FormerlySerializedAs("m_HandAnimation")]
		[Tooltip("The RawImage that provides rotating hand animation.")]
		[SerializeField]
		private RawImage _handAnimation;

		[SerializeField]
		[FormerlySerializedAs("m_SnackBar")]
		[Tooltip("The snackbar Game Object.")]
		private GameObject _snackBar;

		[FormerlySerializedAs("m_SnackBarText")]
		[SerializeField]
		[Tooltip("The snackbar text.")]
		private Text _snackBarText;

		[Tooltip("The Game Object that contains the button to open the help window.")]
		[FormerlySerializedAs("m_OpenButton")]
		[SerializeField]
		private GameObject _openButton;

		[Tooltip("The Game Object that contains the window with more instructions on how to find a plane.")]
		[FormerlySerializedAs("m_MoreHelpWindow")]
		[SerializeField]
		private GameObject _moreHelpWindow;

		[FormerlySerializedAs("m_GotItButton")]
		[SerializeField]
		[Tooltip("The Game Object that contains the button to close the help window.")]
		private Button _gotItButton;

		private float _detectedPlaneElapsed;

		private float _notDetectedPlaneElapsed;

		private bool _isLostTrackingDisplayed;

		private List<DetectedPlane> _detectedPlanes = new List<DetectedPlane>();

		public void Start()
		{
			_openButton.GetComponent<Button>().onClick.AddListener(OnOpenButtonClicked);
			_gotItButton.onClick.AddListener(OnGotItButtonClicked);
			CheckFieldsAreNotNull();
			_moreHelpWindow.SetActive(value: false);
			_isLostTrackingDisplayed = false;
			_notDetectedPlaneElapsed = DisplayGuideDelay - 1f;
		}

		public void OnDestroy()
		{
			_openButton.GetComponent<Button>().onClick.RemoveListener(OnOpenButtonClicked);
			_gotItButton.onClick.RemoveListener(OnGotItButtonClicked);
		}

		public void Update()
		{
			UpdateDetectedPlaneTrackingState();
			UpdateUI();
		}

		public void EnablePlaneDiscoveryGuide(bool guideEnabled)
		{
			if (guideEnabled)
			{
				base.enabled = true;
				return;
			}
			base.enabled = false;
			_featurePoints.SetActive(value: false);
			_handAnimation.enabled = false;
			_snackBar.SetActive(value: false);
		}

		private void OnOpenButtonClicked()
		{
			_moreHelpWindow.SetActive(value: true);
			base.enabled = false;
			_featurePoints.SetActive(value: false);
			_handAnimation.enabled = false;
			_snackBar.SetActive(value: false);
		}

		private void OnGotItButtonClicked()
		{
			_moreHelpWindow.SetActive(value: false);
			base.enabled = true;
		}

		private void UpdateDetectedPlaneTrackingState()
		{
			if (Session.Status != SessionStatus.Tracking)
			{
				return;
			}
			Session.GetTrackables(_detectedPlanes);
			foreach (DetectedPlane detectedPlane in _detectedPlanes)
			{
				if (detectedPlane.TrackingState == TrackingState.Tracking)
				{
					_detectedPlaneElapsed += Time.deltaTime;
					_notDetectedPlaneElapsed = 0f;
					return;
				}
			}
			_detectedPlaneElapsed = 0f;
			_notDetectedPlaneElapsed += Time.deltaTime;
		}

		private void UpdateUI()
		{
			if (Session.Status == SessionStatus.LostTracking && Session.LostTrackingReason != LostTrackingReason.None)
			{
				_featurePoints.SetActive(value: false);
				_handAnimation.enabled = false;
				_snackBar.SetActive(value: true);
				switch (Session.LostTrackingReason)
				{
				case LostTrackingReason.InsufficientLight:
					_snackBarText.text = "Too dark. Try moving to a well-lit area.";
					break;
				case LostTrackingReason.InsufficientFeatures:
					_snackBarText.text = "Aim device at a surface with more texture or color.";
					break;
				case LostTrackingReason.ExcessiveMotion:
					_snackBarText.text = "Moving too fast. Slow down.";
					break;
				case LostTrackingReason.CameraUnavailable:
					_snackBarText.text = "Another app is using the camera. Tap on this app or try closing the other one.";
					break;
				default:
					_snackBarText.text = "Motion tracking is lost.";
					break;
				}
				_openButton.SetActive(value: false);
				_isLostTrackingDisplayed = true;
				return;
			}
			if (_isLostTrackingDisplayed)
			{
				_snackBar.SetActive(value: false);
				_isLostTrackingDisplayed = false;
			}
			if (_notDetectedPlaneElapsed > DisplayGuideDelay)
			{
				_featurePoints.SetActive(value: true);
				if (!_handAnimation.enabled)
				{
					_handAnimation.GetComponent<CanvasRenderer>().SetAlpha(0f);
					_handAnimation.CrossFadeAlpha(1f, 0.15f, ignoreTimeScale: false);
				}
				_handAnimation.enabled = true;
				_snackBar.SetActive(value: true);
				if (_notDetectedPlaneElapsed > OfferDetailedInstructionsDelay)
				{
					_snackBarText.text = "Need Help?";
					_openButton.SetActive(value: true);
				}
				else
				{
					_snackBarText.text = "Point your camera to where you want to place an object.";
					_openButton.SetActive(value: false);
				}
			}
			else if (_notDetectedPlaneElapsed > 0f || _detectedPlaneElapsed > 0.75f)
			{
				_featurePoints.SetActive(value: false);
				_snackBar.SetActive(value: false);
				_openButton.SetActive(value: false);
				if (_handAnimation.enabled)
				{
					_handAnimation.GetComponent<CanvasRenderer>().SetAlpha(1f);
					_handAnimation.CrossFadeAlpha(0f, 0.15f, ignoreTimeScale: false);
				}
				_handAnimation.enabled = false;
			}
		}

		private void CheckFieldsAreNotNull()
		{
			if (_moreHelpWindow == null)
			{
				UnityEngine.Debug.LogError("MoreHelpWindow is null");
			}
			if (_gotItButton == null)
			{
				UnityEngine.Debug.LogError("GotItButton is null");
			}
			if (_snackBarText == null)
			{
				UnityEngine.Debug.LogError("SnackBarText is null");
			}
			if (_snackBar == null)
			{
				UnityEngine.Debug.LogError("SnackBar is null");
			}
			if (_openButton == null)
			{
				UnityEngine.Debug.LogError("OpenButton is null");
			}
			else if (_openButton.GetComponent<Button>() == null)
			{
				UnityEngine.Debug.LogError("OpenButton does not have a Button Component.");
			}
			if (_handAnimation == null)
			{
				UnityEngine.Debug.LogError("HandAnimation is null");
			}
			if (_featurePoints == null)
			{
				UnityEngine.Debug.LogError("FeaturePoints is null");
			}
		}
	}
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class PointcloudVisualizer : MonoBehaviour
	{
		private struct PointInfo
		{
			public Vector3 Position;

			public Vector2 Size;

			public float CreationTime;

			public PointInfo(Vector3 position, Vector2 size, float creationTime)
			{
				Position = position;
				Size = size;
				CreationTime = creationTime;
			}
		}

		[Tooltip("The color of the feature points.")]
		public Color PointColor;

		[Tooltip("Whether to enable the pop animation for the feature points.")]
		public bool EnablePopAnimation = true;

		[Tooltip("The maximum number of points to add per frame.")]
		public int MaxPointsToAddPerFrame = 1;

		[Tooltip("The time interval that the animation lasts in seconds.")]
		public float AnimationDuration = 0.3f;

		[FormerlySerializedAs("m_MaxPointCount")]
		[SerializeField]
		[Tooltip("The maximum number of points to show on the screen.")]
		private int _maxPointCount = 1000;

		[SerializeField]
		[FormerlySerializedAs("m_DefaultSize")]
		[Tooltip("The default size of the points.")]
		private int _defaultSize = 10;

		[FormerlySerializedAs("m_PopSize")]
		[SerializeField]
		[Tooltip("The maximum size that the points will have when they pop.")]
		private int _popSize = 50;

		private Mesh _mesh;

		private MeshRenderer _meshRenderer;

		private int _screenWidthId;

		private int _screenHeightId;

		private int _colorId;

		private MaterialPropertyBlock _propertyBlock;

		private Resolution _cachedResolution;

		private Color _cachedColor;

		private LinkedList<PointInfo> _cachedPoints;

		public void Start()
		{
			_meshRenderer = GetComponent<MeshRenderer>();
			_mesh = GetComponent<MeshFilter>().mesh;
			if (_mesh == null)
			{
				_mesh = new Mesh();
			}
			_mesh.Clear();
			_cachedColor = PointColor;
			_screenWidthId = Shader.PropertyToID("_ScreenWidth");
			_screenHeightId = Shader.PropertyToID("_ScreenHeight");
			_colorId = Shader.PropertyToID("_Color");
			_propertyBlock = new MaterialPropertyBlock();
			_meshRenderer.GetPropertyBlock(_propertyBlock);
			_propertyBlock.SetColor(_colorId, _cachedColor);
			_meshRenderer.SetPropertyBlock(_propertyBlock);
			_cachedPoints = new LinkedList<PointInfo>();
		}

		public void OnDisable()
		{
			ClearCachedPoints();
		}

		public void Update()
		{
			if (Session.Status != SessionStatus.Tracking)
			{
				ClearCachedPoints();
				return;
			}
			if (Screen.currentResolution.height != _cachedResolution.height || Screen.currentResolution.width != _cachedResolution.width)
			{
				UpdateResolution();
			}
			if (_cachedColor != PointColor)
			{
				UpdateColor();
			}
			if (EnablePopAnimation)
			{
				AddPointsIncrementallyToCache();
				UpdatePointSize();
			}
			else
			{
				AddAllPointsToCache();
			}
			UpdateMesh();
		}

		private void ClearCachedPoints()
		{
			_cachedPoints.Clear();
			_mesh.Clear();
		}

		private void UpdateResolution()
		{
			_cachedResolution = Screen.currentResolution;
			if (_meshRenderer != null)
			{
				_meshRenderer.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetFloat(_screenWidthId, _cachedResolution.width);
				_propertyBlock.SetFloat(_screenHeightId, _cachedResolution.height);
				_meshRenderer.SetPropertyBlock(_propertyBlock);
			}
		}

		private void UpdateColor()
		{
			_cachedColor = PointColor;
			_meshRenderer.GetPropertyBlock(_propertyBlock);
			_propertyBlock.SetColor("_Color", _cachedColor);
			_meshRenderer.SetPropertyBlock(_propertyBlock);
		}

		private void AddPointsIncrementallyToCache()
		{
			if (Frame.PointCloud.PointCount > 0 && Frame.PointCloud.IsUpdatedThisFrame)
			{
				int num = Mathf.Min(MaxPointsToAddPerFrame, Frame.PointCloud.PointCount);
				for (int i = 0; i < num; i++)
				{
					Vector3 point = Frame.PointCloud.GetPointAsStruct(UnityEngine.Random.Range(0, Frame.PointCloud.PointCount - 1));
					AddPointToCache(point);
				}
			}
		}

		private void AddAllPointsToCache()
		{
			if (Frame.PointCloud.IsUpdatedThisFrame)
			{
				for (int i = 0; i < Frame.PointCloud.PointCount; i++)
				{
					AddPointToCache(Frame.PointCloud.GetPointAsStruct(i));
				}
			}
		}

		private void AddPointToCache(Vector3 point)
		{
			if (_cachedPoints.Count >= _maxPointCount)
			{
				_cachedPoints.RemoveFirst();
			}
			_cachedPoints.AddLast(new PointInfo(point, new Vector2(_defaultSize, _defaultSize), Time.time));
		}

		private void UpdatePointSize()
		{
			if (_cachedPoints.Count <= 0 || !EnablePopAnimation)
			{
				return;
			}
			for (LinkedListNode<PointInfo> linkedListNode = _cachedPoints.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				float num = Time.time - linkedListNode.Value.CreationTime;
				if (!(num >= AnimationDuration))
				{
					float num2 = num / AnimationDuration;
					float num3 = 0f;
					num3 = ((!(num2 < 0.5f)) ? Mathf.Lerp(_popSize, _defaultSize, (num2 - 0.5f) * 2f) : Mathf.Lerp(_defaultSize, _popSize, num2 * 2f));
					linkedListNode.Value = new PointInfo(linkedListNode.Value.Position, new Vector2(num3, num3), linkedListNode.Value.CreationTime);
				}
			}
		}

		private void UpdateMesh()
		{
			_mesh.Clear();
			_mesh.vertices = _cachedPoints.Select((PointInfo p) => p.Position).ToArray();
			_mesh.uv = _cachedPoints.Select((PointInfo p) => p.Size).ToArray();
			_mesh.SetIndices(Enumerable.Range(0, _cachedPoints.Count).ToArray(), MeshTopology.Points, 0);
		}
	}
	[RequireComponent(typeof(RawImage))]
	[RequireComponent(typeof(VideoPlayer))]
	public class RawImageVideoPlayer : MonoBehaviour
	{
		public RawImage RawImage;

		public VideoPlayer VideoPlayer;

		private Texture _rawImageTexture;

		public void Start()
		{
			VideoPlayer.enabled = false;
			_rawImageTexture = RawImage.texture;
			VideoPlayer.prepareCompleted += PrepareCompleted;
		}

		public void Update()
		{
			if (!Session.Status.IsValid() || Session.Status.IsError())
			{
				VideoPlayer.Stop();
			}
			else if (RawImage.enabled && !VideoPlayer.enabled)
			{
				VideoPlayer.enabled = true;
				VideoPlayer.Play();
			}
			else if (!RawImage.enabled && VideoPlayer.enabled)
			{
				VideoPlayer.Stop();
				RawImage.texture = _rawImageTexture;
				VideoPlayer.enabled = false;
			}
		}

		private void PrepareCompleted(VideoPlayer player)
		{
			RawImage.texture = player.texture;
		}
	}
	public class SafeAreaScaler : MonoBehaviour
	{
		private Rect _screenSafeArea = new Rect(0f, 0f, 0f, 0f);

		public void Update()
		{
			Rect safeArea = Screen.safeArea;
			if (_screenSafeArea != safeArea)
			{
				_screenSafeArea = safeArea;
				MatchRectTransformToSafeArea();
			}
		}

		private void MatchRectTransformToSafeArea()
		{
			RectTransform component = GetComponent<RectTransform>();
			Vector2 offsetMin = new Vector2(_screenSafeArea.xMin, (float)Screen.height - _screenSafeArea.yMax);
			Vector2 offsetMax = new Vector2(_screenSafeArea.xMax - (float)Screen.width, 0f - _screenSafeArea.yMin);
			component.offsetMin = offsetMin;
			component.offsetMax = offsetMax;
		}
	}
	public class ShadowQuadHelper : MonoBehaviour
	{
		private DepthMenu _depthMenu;

		private GameObject _shadowQuad;

		public void Start()
		{
			_shadowQuad = base.gameObject.transform.Find("ShadowQuad").gameObject;
			_depthMenu = UnityEngine.Object.FindObjectOfType<DepthMenu>();
		}

		public void Update()
		{
			if (_shadowQuad.activeSelf == _depthMenu.IsDepthEnabled())
			{
				_shadowQuad.SetActive(!_depthMenu.IsDepthEnabled());
			}
		}
	}
}
namespace GoogleARCore.Examples.CloudAnchors
{
	public class AnchorController : NetworkBehaviour
	{
		private const float _resolvingTimeout = 10f;

		[SyncVar(hook = "OnChangeId")]
		private string _cloudAnchorId = string.Empty;

		private bool _isHost;

		private bool _shouldResolve;

		private float _timeSinceStartResolving;

		private bool _passedResolvingTimeout;

		private GameObject _anchorMesh;

		private CloudAnchorsExampleController _cloudAnchorsExampleController;

		private static int kCmdCmdSetCloudAnchorId;

		public string Network_cloudAnchorId
		{
			get
			{
				return _cloudAnchorId;
			}
			[param: In]
			set
			{
				ref string cloudAnchorId = ref _cloudAnchorId;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					OnChangeId(value);
					base.syncVarHookGuard = false;
				}
				SetSyncVar(value, ref cloudAnchorId, 1u);
			}
		}

		public void Awake()
		{
			_cloudAnchorsExampleController = GameObject.Find("CloudAnchorsExampleController").GetComponent<CloudAnchorsExampleController>();
			_anchorMesh = base.transform.Find("AnchorMesh").gameObject;
			_anchorMesh.SetActive(value: false);
		}

		public override void OnStartClient()
		{
			if (_cloudAnchorId != string.Empty)
			{
				_shouldResolve = true;
			}
		}

		public void Update()
		{
			if (!_shouldResolve || !_cloudAnchorsExampleController.IsResolvingPrepareTimePassed())
			{
				return;
			}
			if (!_passedResolvingTimeout)
			{
				_timeSinceStartResolving += Time.deltaTime;
				if (_timeSinceStartResolving > 10f)
				{
					_passedResolvingTimeout = true;
					_cloudAnchorsExampleController.OnResolvingTimeoutPassed();
				}
			}
			ResolveAnchorFromId(_cloudAnchorId);
		}

		[Command]
		public void CmdSetCloudAnchorId(string cloudAnchorId)
		{
			Network_cloudAnchorId = cloudAnchorId;
		}

		public string GetCloudAnchorId()
		{
			return _cloudAnchorId;
		}

		public void HostLastPlacedAnchor(Component lastPlacedAnchor)
		{
			_isHost = true;
			_anchorMesh.SetActive(value: true);
			XPSession.CreateCloudAnchor((Anchor)lastPlacedAnchor).ThenAction(delegate(CloudAnchorResult result)
			{
				if (result.Response != CloudServiceResponse.Success)
				{
					UnityEngine.Debug.Log($"Failed to host Cloud Anchor: {result.Response}");
					_cloudAnchorsExampleController.OnAnchorHosted(success: false, result.Response.ToString());
				}
				else
				{
					UnityEngine.Debug.Log($"Cloud Anchor {result.Anchor.CloudId} was created and saved.");
					CallCmdSetCloudAnchorId(result.Anchor.CloudId);
					_cloudAnchorsExampleController.OnAnchorHosted(success: true, result.Response.ToString());
				}
			});
		}

		private void ResolveAnchorFromId(string cloudAnchorId)
		{
			_cloudAnchorsExampleController.OnAnchorInstantiated(isHost: false);
			if (Session.Status != SessionStatus.Tracking)
			{
				return;
			}
			_shouldResolve = false;
			XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction(delegate(CloudAnchorResult result)
			{
				if (result.Response != CloudServiceResponse.Success)
				{
					UnityEngine.Debug.LogError($"Client could not resolve Cloud Anchor {cloudAnchorId}: {result.Response}");
					_cloudAnchorsExampleController.OnAnchorResolved(success: false, result.Response.ToString());
					_shouldResolve = true;
				}
				else
				{
					UnityEngine.Debug.Log($"Client successfully resolved Cloud Anchor {cloudAnchorId}.");
					_cloudAnchorsExampleController.OnAnchorResolved(success: true, result.Response.ToString());
					OnResolved(result.Anchor.transform);
					_anchorMesh.SetActive(value: true);
				}
			});
		}

		private void OnResolved(Transform anchorTransform)
		{
			GameObject.Find("CloudAnchorsExampleController").GetComponent<CloudAnchorsExampleController>().SetWorldOrigin(anchorTransform);
			_passedResolvingTimeout = true;
		}

		private void OnChangeId(string newId)
		{
			if (!_isHost && newId != string.Empty)
			{
				Network_cloudAnchorId = newId;
				_shouldResolve = true;
			}
		}

		private void UNetVersion()
		{
		}

		protected static void InvokeCmdCmdSetCloudAnchorId(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				UnityEngine.Debug.LogError("Command CmdSetCloudAnchorId called on client.");
			}
			else
			{
				((AnchorController)obj).CmdSetCloudAnchorId(reader.ReadString());
			}
		}

		public void CallCmdSetCloudAnchorId(string cloudAnchorId)
		{
			if (!NetworkClient.active)
			{
				UnityEngine.Debug.LogError("Command function CmdSetCloudAnchorId called on server.");
				return;
			}
			if (base.isServer)
			{
				CmdSetCloudAnchorId(cloudAnchorId);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write((short)0);
			networkWriter.Write((short)5);
			networkWriter.WritePackedUInt32((uint)kCmdCmdSetCloudAnchorId);
			networkWriter.Write(GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(cloudAnchorId);
			SendCommandInternal(networkWriter, 0, "CmdSetCloudAnchorId");
		}

		static AnchorController()
		{
			kCmdCmdSetCloudAnchorId = 789846388;
			NetworkBehaviour.RegisterCommandDelegate(typeof(AnchorController), kCmdCmdSetCloudAnchorId, InvokeCmdCmdSetCloudAnchorId);
			NetworkCRC.RegisterBehaviour("AnchorController", 0);
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(_cloudAnchorId);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1) != 0)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(_cloudAnchorId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				_cloudAnchorId = reader.ReadString();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				OnChangeId(reader.ReadString());
			}
		}

		public override void PreStartClient()
		{
		}
	}
	public class ARCoreWorldOriginHelper : MonoBehaviour
	{
		public Transform ARCoreDeviceTransform;

		public GameObject DetectedPlanePrefab;

		private List<DetectedPlane> _newPlanes = new List<DetectedPlane>();

		private List<GameObject> _planesBeforeOrigin = new List<GameObject>();

		private bool _isOriginPlaced;

		private Transform _anchorTransform;

		public void Update()
		{
			if (Session.Status != SessionStatus.Tracking)
			{
				return;
			}
			Pose pose = WorldToAnchorPose(Pose.identity);
			Session.GetTrackables(_newPlanes, TrackableQueryFilter.New);
			for (int i = 0; i < _newPlanes.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(DetectedPlanePrefab, pose.position, pose.rotation, base.transform);
				gameObject.GetComponent<DetectedPlaneVisualizer>().Initialize(_newPlanes[i]);
				if (!_isOriginPlaced)
				{
					_planesBeforeOrigin.Add(gameObject);
				}
			}
		}

		public void SetWorldOrigin(Transform anchorTransform)
		{
			if (_isOriginPlaced)
			{
				UnityEngine.Debug.LogWarning("The World Origin can be set only once.");
				return;
			}
			_isOriginPlaced = true;
			_anchorTransform = anchorTransform;
			Pose pose = WorldToAnchorPose(new Pose(ARCoreDeviceTransform.position, ARCoreDeviceTransform.rotation));
			ARCoreDeviceTransform.SetPositionAndRotation(pose.position, pose.rotation);
			foreach (GameObject item in _planesBeforeOrigin)
			{
				if (item != null)
				{
					item.transform.SetPositionAndRotation(pose.position, pose.rotation);
				}
			}
		}

		public bool Raycast(float x, float y, TrackableHitFlags filter, out TrackableHit hitResult)
		{
			bool num = Frame.Raycast(x, y, filter, out hitResult);
			if (num)
			{
				Pose pose = WorldToAnchorPose(hitResult.Pose);
				TrackableHit trackableHit = new TrackableHit(pose, hitResult.Distance, hitResult.Flags, hitResult.Trackable);
				hitResult = trackableHit;
			}
			return num;
		}

		private Pose WorldToAnchorPose(Pose pose)
		{
			if (!_isOriginPlaced)
			{
				return pose;
			}
			Matrix4x4 inverse = Matrix4x4.TRS(_anchorTransform.position, _anchorTransform.rotation, Vector3.one).inverse;
			Vector3 position = inverse.MultiplyPoint(pose.position);
			Quaternion rotation = pose.rotation * Quaternion.LookRotation(inverse.GetColumn(2), inverse.GetColumn(1));
			return new Pose(position, rotation);
		}
	}
	public class ARKitHelper
	{
		public bool RaycastPlane(Camera camera, float x, float y, out Pose hitPose)
		{
			hitPose = default(Pose);
			return false;
		}

		public Component CreateAnchor(Pose pose)
		{
			GameObject gameObject = new GameObject("User Anchor");
			Component result = gameObject.AddComponent<Component>();
			gameObject.transform.position = pose.position;
			gameObject.transform.rotation = pose.rotation;
			return result;
		}

		public void SetWorldOrigin(Transform transform)
		{
		}
	}
	public class CloudAnchorsExampleController : MonoBehaviour
	{
		public enum ApplicationMode
		{
			Ready,
			Hosting,
			Resolving
		}

		public enum ActiveScreen
		{
			LobbyScreen,
			StartScreen,
			ARScreen
		}

		[Header("ARCore")]
		public GameObject ARCoreRoot;

		public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;

		[Header("ARKit")]
		public GameObject ARKitRoot;

		public Camera ARKitFirstPersonCamera;

		[Header("UI")]
		public NetworkManagerUIController NetworkUIController;

		public GameObject LobbyScreen;

		public GameObject StartScreen;

		public GameObject ARScreen;

		public GameObject StatusScreen;

		private const string _hasDisplayedStartInfoKey = "HasDisplayedStartInfo";

		private const float _resolvingPrepareTime = 3f;

		private float _timeSinceStart;

		private bool _passedResolvingPreparedTime;

		private ARKitHelper _arKit = new ARKitHelper();

		private bool _anchorAlreadyInstantiated;

		private bool _anchorFinishedHosting;

		private bool _isQuitting;

		private Component _worldOriginAnchor;

		private Pose? _lastHitPose;

		private ApplicationMode _currentMode;

		private ActiveScreen _currentActiveScreen;

		private CloudAnchorsNetworkManager _networkManager;

		public bool IsOriginPlaced { get; private set; }

		public void OnStartNowButtonClicked()
		{
			SwitchActiveScreen(ActiveScreen.ARScreen);
		}

		public void OnLearnMoreButtonClicked()
		{
			Application.OpenURL("https://developers.google.com/ar/cloud-anchors-privacy");
		}

		public void Awake()
		{
			Application.targetFrameRate = 60;
		}

		public void Start()
		{
			_networkManager = NetworkUIController.GetComponent<CloudAnchorsNetworkManager>();
			_networkManager.OnClientConnected += OnConnectedToServer;
			_networkManager.OnClientDisconnected += OnDisconnectedFromServer;
			base.gameObject.name = "CloudAnchorsExampleController";
			ARCoreRoot.SetActive(value: false);
			ARKitRoot.SetActive(value: false);
			ResetStatus();
		}

		public void Update()
		{
			UpdateApplicationLifecycle();
			if (_currentActiveScreen != ActiveScreen.ARScreen || (_currentMode != ApplicationMode.Hosting && _currentMode != ApplicationMode.Resolving))
			{
				return;
			}
			if (_currentMode == ApplicationMode.Resolving && !_passedResolvingPreparedTime)
			{
				_timeSinceStart += Time.deltaTime;
				if (_timeSinceStart > 3f)
				{
					_passedResolvingPreparedTime = true;
					NetworkUIController.ShowDebugMessage("Waiting for Cloud Anchor to be hosted...");
				}
			}
			if ((_currentMode == ApplicationMode.Resolving && !IsOriginPlaced) || Input.touchCount < 1)
			{
				return;
			}
			Touch touch2;
			Touch touch = (touch2 = Input.GetTouch(0));
			if (touch.phase != TouchPhase.Began || EventSystem.current.IsPointerOverGameObject(touch2.fingerId))
			{
				return;
			}
			TrackableHit hitResult = default(TrackableHit);
			_lastHitPose = null;
			Pose hitPose;
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				if (ARCoreWorldOriginHelper.Raycast(touch2.position.x, touch2.position.y, TrackableHitFlags.PlaneWithinPolygon, out hitResult))
				{
					_lastHitPose = hitResult.Pose;
				}
			}
			else if (_arKit.RaycastPlane(ARKitFirstPersonCamera, touch2.position.x, touch2.position.y, out hitPose))
			{
				_lastHitPose = hitPose;
			}
			if (!_lastHitPose.HasValue)
			{
				return;
			}
			if (CanPlaceStars())
			{
				InstantiateStar();
			}
			else if (!IsOriginPlaced && _currentMode == ApplicationMode.Hosting)
			{
				if (Application.platform != RuntimePlatform.IPhonePlayer)
				{
					_worldOriginAnchor = hitResult.Trackable.CreateAnchor(hitResult.Pose);
				}
				else
				{
					_worldOriginAnchor = _arKit.CreateAnchor(_lastHitPose.Value);
				}
				SetWorldOrigin(_worldOriginAnchor.transform);
				InstantiateAnchor();
				OnAnchorInstantiated(isHost: true);
			}
		}

		public bool IsResolvingPrepareTimePassed()
		{
			if (_currentMode != ApplicationMode.Ready)
			{
				return _timeSinceStart > 3f;
			}
			return false;
		}

		public void SetWorldOrigin(Transform anchorTransform)
		{
			if (IsOriginPlaced)
			{
				UnityEngine.Debug.LogWarning("The World Origin can be set only once.");
				return;
			}
			IsOriginPlaced = true;
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);
			}
			else
			{
				_arKit.SetWorldOrigin(anchorTransform);
			}
		}

		public void OnLobbyVisibilityChanged(bool visible)
		{
			if (visible)
			{
				SwitchActiveScreen(ActiveScreen.LobbyScreen);
			}
			else if (PlayerPrefs.HasKey("HasDisplayedStartInfo"))
			{
				SwitchActiveScreen(ActiveScreen.ARScreen);
			}
			else
			{
				SwitchActiveScreen(ActiveScreen.StartScreen);
			}
		}

		public void OnResolvingTimeoutPassed()
		{
			if (_currentMode != ApplicationMode.Resolving)
			{
				UnityEngine.Debug.LogWarning("OnResolvingTimeoutPassed shouldn't be calledwhen the application is not in resolving mode.");
			}
			else
			{
				NetworkUIController.ShowDebugMessage("Still resolving the anchor.Please make sure you're looking at where the Cloud Anchor was hosted.Or, try to re-join the room.");
			}
		}

		public void OnEnterHostingModeClick()
		{
			if (_currentMode == ApplicationMode.Hosting)
			{
				_currentMode = ApplicationMode.Ready;
				ResetStatus();
				UnityEngine.Debug.Log("Reset ApplicationMode from Hosting to Ready.");
			}
			_currentMode = ApplicationMode.Hosting;
		}

		public void OnEnterResolvingModeClick()
		{
			if (_currentMode == ApplicationMode.Resolving)
			{
				_currentMode = ApplicationMode.Ready;
				ResetStatus();
				UnityEngine.Debug.Log("Reset ApplicationMode from Resolving to Ready.");
			}
			_currentMode = ApplicationMode.Resolving;
		}

		public void OnAnchorInstantiated(bool isHost)
		{
			if (!_anchorAlreadyInstantiated)
			{
				_anchorAlreadyInstantiated = true;
				NetworkUIController.OnAnchorInstantiated(isHost);
			}
		}

		public void OnAnchorHosted(bool success, string response)
		{
			_anchorFinishedHosting = success;
			NetworkUIController.OnAnchorHosted(success, response);
		}

		public void OnAnchorResolved(bool success, string response)
		{
			NetworkUIController.OnAnchorResolved(success, response);
		}

		private void OnConnectedToServer()
		{
			if (_currentMode == ApplicationMode.Hosting)
			{
				NetworkUIController.ShowDebugMessage("Find a plane, tap to create a Cloud Anchor.");
			}
			else if (_currentMode == ApplicationMode.Resolving)
			{
				NetworkUIController.ShowDebugMessage("Look at the same scene as the hosting phone.");
			}
			else
			{
				ReturnToLobbyWithReason("Connected to server with neither Hosting nor Resolvingmode. Please start the app again.");
			}
		}

		private void OnDisconnectedFromServer()
		{
			ReturnToLobbyWithReason("Network session disconnected! Please start the app again and try another room.");
		}

		private void InstantiateAnchor()
		{
			GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>().SpawnAnchor(Vector3.zero, Quaternion.identity, _worldOriginAnchor);
		}

		private void InstantiateStar()
		{
			GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>().CallCmdSpawnStar(_lastHitPose.Value.position, _lastHitPose.Value.rotation);
		}

		private void SetPlatformActive()
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				ARCoreRoot.SetActive(value: true);
				ARKitRoot.SetActive(value: false);
			}
			else
			{
				ARCoreRoot.SetActive(value: false);
				ARKitRoot.SetActive(value: true);
			}
		}

		private bool CanPlaceStars()
		{
			if (_currentMode == ApplicationMode.Resolving)
			{
				return IsOriginPlaced;
			}
			if (_currentMode == ApplicationMode.Hosting)
			{
				if (IsOriginPlaced)
				{
					return _anchorFinishedHosting;
				}
				return false;
			}
			return false;
		}

		private void ResetStatus()
		{
			_currentMode = ApplicationMode.Ready;
			if (_worldOriginAnchor != null)
			{
				UnityEngine.Object.Destroy(_worldOriginAnchor.gameObject);
			}
			IsOriginPlaced = false;
			_worldOriginAnchor = null;
		}

		private void SwitchActiveScreen(ActiveScreen activeScreen)
		{
			LobbyScreen.SetActive(activeScreen == ActiveScreen.LobbyScreen);
			StatusScreen.SetActive(activeScreen != ActiveScreen.StartScreen);
			StartScreen.SetActive(activeScreen == ActiveScreen.StartScreen);
			ARScreen.SetActive(activeScreen == ActiveScreen.ARScreen);
			_currentActiveScreen = activeScreen;
			if (_currentActiveScreen == ActiveScreen.StartScreen)
			{
				PlayerPrefs.SetInt("HasDisplayedStartInfo", 1);
			}
			if (_currentActiveScreen == ActiveScreen.ARScreen)
			{
				_timeSinceStart = 0f;
				SetPlatformActive();
			}
		}

		private void UpdateApplicationLifecycle()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
			int sleepTimeout = -1;
			if (Session.Status != SessionStatus.Tracking)
			{
				sleepTimeout = -2;
			}
			Screen.sleepTimeout = sleepTimeout;
			if (!_isQuitting)
			{
				if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
				{
					ReturnToLobbyWithReason("Camera permission is needed to run this application.");
				}
				else if (Session.Status.IsError())
				{
					QuitWithReason("ARCore encountered a problem connecting. Please start the app again.");
				}
			}
		}

		private void QuitWithReason(string reason)
		{
			if (!_isQuitting)
			{
				NetworkUIController.ShowDebugMessage(reason);
				_isQuitting = true;
				Invoke("DoQuit", 5f);
			}
		}

		private void ReturnToLobbyWithReason(string reason)
		{
			if (!_isQuitting)
			{
				NetworkUIController.ShowDebugMessage(reason);
				Invoke("DoReturnToLobby", 3f);
			}
		}

		private void DoQuit()
		{
			Application.Quit();
		}

		private void DoReturnToLobby()
		{
			NetworkManager.Shutdown();
			SceneManager.LoadScene("CloudAnchors");
		}
	}
	public class CloudAnchorsNetworkManager : NetworkManager
	{
		public event Action OnClientConnected;

		public event Action OnClientDisconnected;

		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);
			if (conn.lastError == NetworkError.Ok)
			{
				UnityEngine.Debug.Log("Successfully connected to server.");
			}
			else
			{
				UnityEngine.Debug.LogError("Connected to server with error: " + conn.lastError);
			}
			if (this.OnClientConnected != null)
			{
				this.OnClientConnected();
			}
		}

		public override void OnClientDisconnect(NetworkConnection conn)
		{
			base.OnClientDisconnect(conn);
			if (conn.lastError == NetworkError.Ok)
			{
				UnityEngine.Debug.Log("Successfully disconnected from the server.");
			}
			else
			{
				UnityEngine.Debug.LogError("Disconnected from the server with error: " + conn.lastError);
			}
			if (this.OnClientDisconnected != null)
			{
				this.OnClientDisconnected();
			}
		}
	}
	public class LocalPlayerController : NetworkBehaviour
	{
		public GameObject StarPrefab;

		public GameObject AnchorPrefab;

		private static int kCmdCmdSpawnStar;

		public override void OnStartLocalPlayer()
		{
			base.OnStartLocalPlayer();
			base.gameObject.name = "LocalPlayer";
		}

		public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor)
		{
			GameObject obj = UnityEngine.Object.Instantiate(AnchorPrefab, position, rotation);
			obj.GetComponent<AnchorController>().HostLastPlacedAnchor(anchor);
			NetworkServer.Spawn(obj);
		}

		[Command]
		public void CmdSpawnStar(Vector3 position, Quaternion rotation)
		{
			NetworkServer.Spawn(UnityEngine.Object.Instantiate(StarPrefab, position, rotation));
		}

		private void UNetVersion()
		{
		}

		protected static void InvokeCmdCmdSpawnStar(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				UnityEngine.Debug.LogError("Command CmdSpawnStar called on client.");
			}
			else
			{
				((LocalPlayerController)obj).CmdSpawnStar(reader.ReadVector3(), reader.ReadQuaternion());
			}
		}

		public void CallCmdSpawnStar(Vector3 position, Quaternion rotation)
		{
			if (!NetworkClient.active)
			{
				UnityEngine.Debug.LogError("Command function CmdSpawnStar called on server.");
				return;
			}
			if (base.isServer)
			{
				CmdSpawnStar(position, rotation);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write((short)0);
			networkWriter.Write((short)5);
			networkWriter.WritePackedUInt32((uint)kCmdCmdSpawnStar);
			networkWriter.Write(GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(position);
			networkWriter.Write(rotation);
			SendCommandInternal(networkWriter, 0, "CmdSpawnStar");
		}

		static LocalPlayerController()
		{
			kCmdCmdSpawnStar = -1684342459;
			NetworkBehaviour.RegisterCommandDelegate(typeof(LocalPlayerController), kCmdCmdSpawnStar, InvokeCmdCmdSpawnStar);
			NetworkCRC.RegisterBehaviour("LocalPlayerController", 0);
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result = default(bool);
			return result;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		public override void PreStartClient()
		{
		}
	}
	[RequireComponent(typeof(CloudAnchorsNetworkManager))]
	public class NetworkManagerUIController : MonoBehaviour
	{
		public Text SnackbarText;

		public GameObject CurrentRoomLabel;

		public GameObject ReturnButton;

		public CloudAnchorsExampleController CloudAnchorsExampleController;

		public GameObject RoomListPanel;

		public Text NoPreviousRoomsText;

		public GameObject JoinRoomListRowPrefab;

		private const int _matchPageSize = 5;

		private CloudAnchorsNetworkManager _manager;

		private string _currentRoomNumber;

		private List<GameObject> _joinRoomButtonsPool = new List<GameObject>();

		public void Awake()
		{
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(JoinRoomListRowPrefab);
				gameObject.transform.SetParent(RoomListPanel.transform, worldPositionStays: false);
				gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -(100 * i));
				gameObject.SetActive(value: true);
				gameObject.GetComponentInChildren<Text>().text = string.Empty;
				_joinRoomButtonsPool.Add(gameObject);
			}
			_manager = GetComponent<CloudAnchorsNetworkManager>();
			_manager.StartMatchMaker();
			_manager.matchMaker.ListMatches(0, 5, string.Empty, filterOutPrivateMatchesFromResults: false, 0, 0, OnMatchList);
			ChangeLobbyUIVisibility(visible: true);
		}

		public void OnCreateRoomClicked()
		{
			_manager.matchMaker.CreateMatch(_manager.matchName, _manager.matchSize, matchAdvertise: true, string.Empty, string.Empty, string.Empty, 0, 0, OnMatchCreate);
		}

		public void OnReturnToLobbyClick()
		{
			ReturnButton.GetComponent<Button>().interactable = false;
			if (_manager.matchInfo == null)
			{
				OnMatchDropped(success: true, null);
			}
			else
			{
				_manager.matchMaker.DropConnection(_manager.matchInfo.networkId, _manager.matchInfo.nodeId, _manager.matchInfo.domain, OnMatchDropped);
			}
		}

		public void OnRefhreshRoomListClicked()
		{
			_manager.matchMaker.ListMatches(0, 5, string.Empty, filterOutPrivateMatchesFromResults: false, 0, 0, OnMatchList);
		}

		public void OnAnchorInstantiated(bool isHost)
		{
			if (isHost)
			{
				SnackbarText.text = "Hosting Cloud Anchor...";
			}
			else
			{
				SnackbarText.text = "Cloud Anchor added to session! Attempting to resolve anchor...";
			}
		}

		public void OnAnchorHosted(bool success, string response)
		{
			if (success)
			{
				SnackbarText.text = "Cloud Anchor successfully hosted! Tap to place more stars.";
			}
			else
			{
				SnackbarText.text = "Cloud Anchor could not be hosted. " + response;
			}
		}

		public void OnAnchorResolved(bool success, string response)
		{
			if (success)
			{
				SnackbarText.text = "Cloud Anchor successfully resolved! Tap to place more stars.";
			}
			else
			{
				SnackbarText.text = "Cloud Anchor could not be resolved. Will attempt again. " + response;
			}
		}

		public void ShowDebugMessage(string debugMessage)
		{
			SnackbarText.text = debugMessage;
		}

		private void OnJoinRoomClicked(MatchInfoSnapshot match)
		{
			_manager.matchName = match.name;
			_manager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty, string.Empty, 0, 0, OnMatchJoined);
		}

		private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
		{
			if (!success)
			{
				SnackbarText.text = "Could not list matches: " + extendedInfo;
				return;
			}
			_manager.OnMatchList(success, extendedInfo, matches);
			if (_manager.matches == null)
			{
				return;
			}
			foreach (GameObject item in _joinRoomButtonsPool)
			{
				item.SetActive(value: false);
				item.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
				item.GetComponentInChildren<Text>().text = string.Empty;
			}
			NoPreviousRoomsText.gameObject.SetActive(_manager.matches.Count == 0);
			int num = 0;
			foreach (MatchInfoSnapshot match in _manager.matches)
			{
				if (num >= 5)
				{
					break;
				}
				string text = "Room " + GetRoomNumberFromNetworkId(match.networkId);
				GameObject obj = _joinRoomButtonsPool[num++];
				obj.GetComponentInChildren<Text>().text = text;
				obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
				{
					OnJoinRoomClicked(match);
				});
				obj.GetComponentInChildren<Button>().onClick.AddListener(CloudAnchorsExampleController.OnEnterResolvingModeClick);
				obj.SetActive(value: true);
			}
		}

		private void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			if (!success)
			{
				SnackbarText.text = "Could not create match: " + extendedInfo;
				return;
			}
			_manager.OnMatchCreate(success, extendedInfo, matchInfo);
			_currentRoomNumber = GetRoomNumberFromNetworkId(matchInfo.networkId);
			SnackbarText.text = "Connecting to server...";
			ChangeLobbyUIVisibility(visible: false);
			CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + _currentRoomNumber;
		}

		private void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			if (!success)
			{
				SnackbarText.text = "Could not join to match: " + extendedInfo;
				return;
			}
			_manager.OnMatchJoined(success, extendedInfo, matchInfo);
			_currentRoomNumber = GetRoomNumberFromNetworkId(matchInfo.networkId);
			SnackbarText.text = "Connecting to server...";
			ChangeLobbyUIVisibility(visible: false);
			CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + _currentRoomNumber;
		}

		private void OnMatchDropped(bool success, string extendedInfo)
		{
			ReturnButton.GetComponent<Button>().interactable = true;
			if (!success)
			{
				SnackbarText.text = "Could not drop the match: " + extendedInfo;
				return;
			}
			_manager.OnDropConnection(success, extendedInfo);
			NetworkManager.Shutdown();
			SceneManager.LoadScene("CloudAnchors");
		}

		private void ChangeLobbyUIVisibility(bool visible)
		{
			foreach (GameObject item in _joinRoomButtonsPool)
			{
				bool active = visible && item.GetComponentInChildren<Text>().text != string.Empty;
				item.SetActive(active);
			}
			CloudAnchorsExampleController.OnLobbyVisibilityChanged(visible);
		}

		private string GetRoomNumberFromNetworkId(NetworkID networkID)
		{
			return (Convert.ToInt64(networkID.ToString()) % 10000).ToString();
		}
	}
	public class StarController : MonoBehaviour
	{
		private GameObject _starMesh;

		private CloudAnchorsExampleController _cloudAnchorsExampleController;

		public void Awake()
		{
			_cloudAnchorsExampleController = GameObject.Find("CloudAnchorsExampleController").GetComponent<CloudAnchorsExampleController>();
			_starMesh = base.transform.Find("StarMesh").gameObject;
			_starMesh.SetActive(value: false);
		}

		public void Update()
		{
			if (!_starMesh.activeSelf && _cloudAnchorsExampleController.IsOriginPlaced)
			{
				_starMesh.SetActive(value: true);
			}
		}
	}
}
namespace GoogleARCore.Examples.AugmentedImage
{
	public class AugmentedImageExampleController : MonoBehaviour
	{
		public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;

		public GameObject FitToScanOverlay;

		private Dictionary<int, AugmentedImageVisualizer> _visualizers = new Dictionary<int, AugmentedImageVisualizer>();

		private List<GoogleARCore.AugmentedImage> _tempAugmentedImages = new List<GoogleARCore.AugmentedImage>();

		public void Awake()
		{
			Application.targetFrameRate = 60;
		}

		public void Update()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
			if (Session.Status != SessionStatus.Tracking)
			{
				Screen.sleepTimeout = -2;
			}
			else
			{
				Screen.sleepTimeout = -1;
			}
			Session.GetTrackables(_tempAugmentedImages, TrackableQueryFilter.Updated);
			foreach (GoogleARCore.AugmentedImage tempAugmentedImage in _tempAugmentedImages)
			{
				AugmentedImageVisualizer value = null;
				_visualizers.TryGetValue(tempAugmentedImage.DatabaseIndex, out value);
				if (tempAugmentedImage.TrackingState == TrackingState.Tracking && value == null)
				{
					Anchor anchor = tempAugmentedImage.CreateAnchor(tempAugmentedImage.CenterPose);
					value = UnityEngine.Object.Instantiate(AugmentedImageVisualizerPrefab, anchor.transform);
					value.Image = tempAugmentedImage;
					_visualizers.Add(tempAugmentedImage.DatabaseIndex, value);
				}
				else if (tempAugmentedImage.TrackingState == TrackingState.Stopped && value != null)
				{
					_visualizers.Remove(tempAugmentedImage.DatabaseIndex);
					UnityEngine.Object.Destroy(value.gameObject);
				}
			}
			foreach (AugmentedImageVisualizer value2 in _visualizers.Values)
			{
				if (value2.Image.TrackingState == TrackingState.Tracking)
				{
					FitToScanOverlay.SetActive(value: false);
					return;
				}
			}
			FitToScanOverlay.SetActive(value: true);
		}
	}
	public class AugmentedImageVisualizer : MonoBehaviour
	{
		public GoogleARCore.AugmentedImage Image;

		public GameObject FrameLowerLeft;

		public GameObject FrameLowerRight;

		public GameObject FrameUpperLeft;

		public GameObject FrameUpperRight;

		public void Update()
		{
			if (Image == null || Image.TrackingState != TrackingState.Tracking)
			{
				FrameLowerLeft.SetActive(value: false);
				FrameLowerRight.SetActive(value: false);
				FrameUpperLeft.SetActive(value: false);
				FrameUpperRight.SetActive(value: false);
				return;
			}
			float num = Image.ExtentX / 2f;
			float num2 = Image.ExtentZ / 2f;
			FrameLowerLeft.transform.localPosition = num * Vector3.left + num2 * Vector3.back;
			FrameLowerRight.transform.localPosition = num * Vector3.right + num2 * Vector3.back;
			FrameUpperLeft.transform.localPosition = num * Vector3.left + num2 * Vector3.forward;
			FrameUpperRight.transform.localPosition = num * Vector3.right + num2 * Vector3.forward;
			FrameLowerLeft.SetActive(value: true);
			FrameLowerRight.SetActive(value: true);
			FrameUpperLeft.SetActive(value: true);
			FrameUpperRight.SetActive(value: true);
		}
	}
}
namespace GoogleARCore.Examples.AugmentedFaces
{
	[RequireComponent(typeof(MeshFilter))]
	public class ARCoreAugmentedFaceMeshFilter : MonoBehaviour
	{
		public bool AutoBind;

		private AugmentedFace _augmentedFace;

		private List<AugmentedFace> _augmentedFaceList;

		private List<Vector3> _meshVertices = new List<Vector3>();

		private List<Vector3> _meshNormals = new List<Vector3>();

		private List<Vector2> _meshUVs = new List<Vector2>();

		private List<int> _meshIndices = new List<int>();

		private Mesh _mesh;

		private bool _meshInitialized;

		public AugmentedFace AumgnetedFace
		{
			get
			{
				return _augmentedFace;
			}
			set
			{
				_augmentedFace = value;
				Update();
			}
		}

		public void Awake()
		{
			_mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = _mesh;
			_augmentedFaceList = new List<AugmentedFace>();
		}

		public void Update()
		{
			if (AutoBind)
			{
				_augmentedFaceList.Clear();
				Session.GetTrackables(_augmentedFaceList);
				if (_augmentedFaceList.Count != 0)
				{
					_augmentedFace = _augmentedFaceList[0];
				}
			}
			if (_augmentedFace != null)
			{
				base.transform.position = _augmentedFace.CenterPose.position;
				base.transform.rotation = _augmentedFace.CenterPose.rotation;
				UpdateMesh();
			}
		}

		private void UpdateMesh()
		{
			_augmentedFace.GetVertices(_meshVertices);
			_augmentedFace.GetNormals(_meshNormals);
			if (!_meshInitialized)
			{
				_augmentedFace.GetTextureCoordinates(_meshUVs);
				_augmentedFace.GetTriangleIndices(_meshIndices);
				_meshInitialized = true;
			}
			_mesh.Clear();
			_mesh.SetVertices(_meshVertices);
			_mesh.SetNormals(_meshNormals);
			_mesh.SetTriangles(_meshIndices, 0);
			_mesh.SetUVs(0, _meshUVs);
			_mesh.RecalculateBounds();
		}
	}
	[ExecuteInEditMode]
	public class ARCoreAugmentedFaceRig : MonoBehaviour
	{
		public bool AutoBind;

		private static readonly Dictionary<AugmentedFaceRegion, string> _regionTransformNames = new Dictionary<AugmentedFaceRegion, string>
		{
			{
				AugmentedFaceRegion.NoseTip,
				"NOSE_TIP"
			},
			{
				AugmentedFaceRegion.ForeheadLeft,
				"FOREHEAD_LEFT"
			},
			{
				AugmentedFaceRegion.ForeheadRight,
				"FOREHEAD_RIGHT"
			}
		};

		private AugmentedFace _augmentedFace;

		private List<AugmentedFace> _augmentedFaceList = new List<AugmentedFace>();

		private Dictionary<AugmentedFaceRegion, Transform> _regionGameObjects = new Dictionary<AugmentedFaceRegion, Transform>();

		public AugmentedFace AumgnetedFace
		{
			get
			{
				return _augmentedFace;
			}
			set
			{
				_augmentedFace = value;
				Update();
			}
		}

		public void Awake()
		{
			_augmentedFaceList = new List<AugmentedFace>();
			InitializeFaceRegions();
		}

		public void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (AutoBind)
			{
				_augmentedFaceList.Clear();
				Session.GetTrackables(_augmentedFaceList);
				if (_augmentedFaceList.Count != 0)
				{
					_augmentedFace = _augmentedFaceList[0];
				}
			}
			if (_augmentedFace != null)
			{
				UpdateRegions();
			}
		}

		private void InitializeFaceRegions()
		{
			foreach (AugmentedFaceRegion key in _regionTransformNames.Keys)
			{
				string text = _regionTransformNames[key];
				Transform transform = FindChildTransformRecursive(base.transform, text);
				if (transform == null)
				{
					GameObject obj = new GameObject(text);
					obj.transform.SetParent(base.transform);
					transform = obj.transform;
				}
				_regionGameObjects[key] = transform;
			}
		}

		private Transform FindChildTransformRecursive(Transform target, string name)
		{
			if (target.name == name)
			{
				return target;
			}
			foreach (Transform item in target)
			{
				if (item.name.Contains(name))
				{
					return item;
				}
				Transform transform2 = FindChildTransformRecursive(item, name);
				if (transform2 != null)
				{
					return transform2;
				}
			}
			return null;
		}

		private void UpdateRegions()
		{
			bool flag = _augmentedFace.TrackingState == TrackingState.Tracking;
			if (flag)
			{
				base.transform.position = _augmentedFace.CenterPose.position;
				base.transform.rotation = _augmentedFace.CenterPose.rotation;
			}
			foreach (AugmentedFaceRegion key in _regionGameObjects.Keys)
			{
				Transform transform = _regionGameObjects[key];
				transform.gameObject.SetActive(flag);
				if (flag)
				{
					Pose regionPose = _augmentedFace.GetRegionPose(key);
					transform.position = regionPose.position;
					transform.rotation = regionPose.rotation;
				}
			}
		}
	}
	public class AugmentedFacesExampleController : MonoBehaviour
	{
		public GameObject FaceAttachment;

		private bool _isQuitting;

		private List<AugmentedFace> _tempAugmentedFaces = new List<AugmentedFace>();

		public void Awake()
		{
			Application.targetFrameRate = 60;
		}

		public void Update()
		{
			UpdateApplicationLifecycle();
			Session.GetTrackables(_tempAugmentedFaces);
			if (_tempAugmentedFaces.Count == 0)
			{
				Screen.sleepTimeout = -2;
				FaceAttachment.SetActive(value: false);
			}
			else
			{
				Screen.sleepTimeout = -1;
				FaceAttachment.SetActive(value: true);
			}
		}

		private void UpdateApplicationLifecycle()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
			if (!_isQuitting)
			{
				if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
				{
					ShowAndroidToastMessage("Camera permission is needed to run this application.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
				else if (Session.Status.IsError())
				{
					ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
					_isQuitting = true;
					Invoke("DoQuit", 0.5f);
				}
			}
		}

		private void DoQuit()
		{
			Application.Quit();
		}

		private void ShowAndroidToastMessage(string message)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (unityActivity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
				{
					toastClass.CallStatic<AndroidJavaObject>("makeText", new object[3] { unityActivity, message, 0 }).Call("show");
				});
			}
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions
{
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionHelper
	{
		public enum TweenType
		{
			none = 0,
			easeInQuad = 1,
			easeOutQuad = 2,
			easeInOutQuad = 3,
			easeInCubic = 4,
			easeOutCubic = 5,
			easeInOutCubic = 6,
			easeInQuart = 7,
			easeOutQuart = 8,
			easeInOutQuart = 9,
			easeInQuint = 10,
			easeOutQuint = 11,
			easeInOutQuint = 12,
			easeInSine = 13,
			easeOutSine = 14,
			easeInOutSine = 15,
			easeInExpo = 16,
			easeOutExpo = 17,
			easeInOutExpo = 18,
			easeInCirc = 19,
			easeOutCirc = 20,
			easeInOutCirc = 21,
			linear = 22,
			spring = 23,
			easeInBounce = 24,
			easeOutBounce = 25,
			easeInOutBounce = 26,
			easeInBack = 27,
			easeOutBack = 28,
			easeInOutBack = 29,
			easeInElastic = 30,
			easeOutElastic = 31,
			easeInOutElastic = 32,
			AnimationCurve = 999
		}

		public static TweenMethods.TweenFunction GetTweenFunction(TweenType progressMode)
		{
			TweenMethods.TweenFunction result = null;
			switch (progressMode)
			{
			case TweenType.easeInQuad:
				result = TweenMethods.easeInQuad;
				break;
			case TweenType.easeOutQuad:
				result = TweenMethods.easeOutQuad;
				break;
			case TweenType.easeInOutQuad:
				result = TweenMethods.easeInOutQuad;
				break;
			case TweenType.easeInCubic:
				result = TweenMethods.easeInCubic;
				break;
			case TweenType.easeOutCubic:
				result = TweenMethods.easeOutCubic;
				break;
			case TweenType.easeInOutCubic:
				result = TweenMethods.easeInOutCubic;
				break;
			case TweenType.easeInQuart:
				result = TweenMethods.easeInQuart;
				break;
			case TweenType.easeOutQuart:
				result = TweenMethods.easeOutQuart;
				break;
			case TweenType.easeInOutQuart:
				result = TweenMethods.easeInOutQuart;
				break;
			case TweenType.easeInQuint:
				result = TweenMethods.easeInQuint;
				break;
			case TweenType.easeOutQuint:
				result = TweenMethods.easeOutQuint;
				break;
			case TweenType.easeInOutQuint:
				result = TweenMethods.easeInOutQuint;
				break;
			case TweenType.easeInSine:
				result = TweenMethods.easeInSine;
				break;
			case TweenType.easeOutSine:
				result = TweenMethods.easeOutSine;
				break;
			case TweenType.easeInOutSine:
				result = TweenMethods.easeInOutSine;
				break;
			case TweenType.easeInExpo:
				result = TweenMethods.easeInExpo;
				break;
			case TweenType.easeOutExpo:
				result = TweenMethods.easeOutExpo;
				break;
			case TweenType.easeInOutExpo:
				result = TweenMethods.easeInOutExpo;
				break;
			case TweenType.easeInCirc:
				result = TweenMethods.easeInCirc;
				break;
			case TweenType.easeOutCirc:
				result = TweenMethods.easeOutCirc;
				break;
			case TweenType.easeInOutCirc:
				result = TweenMethods.easeInOutCirc;
				break;
			case TweenType.linear:
				result = TweenMethods.linear;
				break;
			case TweenType.spring:
				result = TweenMethods.spring;
				break;
			case TweenType.easeInBounce:
				result = TweenMethods.easeInBounce;
				break;
			case TweenType.easeOutBounce:
				result = TweenMethods.easeOutBounce;
				break;
			case TweenType.easeInOutBounce:
				result = TweenMethods.easeInOutBounce;
				break;
			case TweenType.easeInBack:
				result = TweenMethods.easeInBack;
				break;
			case TweenType.easeOutBack:
				result = TweenMethods.easeOutBack;
				break;
			case TweenType.easeInOutBack:
				result = TweenMethods.easeInOutBack;
				break;
			case TweenType.easeInElastic:
				result = TweenMethods.easeInElastic;
				break;
			case TweenType.easeOutElastic:
				result = TweenMethods.easeOutElastic;
				break;
			case TweenType.easeInOutElastic:
				result = TweenMethods.easeInOutElastic;
				break;
			}
			return result;
		}

		public static bool ContainsTransition(GameObject gameObject)
		{
			return gameObject.GetComponents<TransitionBase>().Length != 0;
		}

		public static List<TransitionBase> TransitionIn(GameObject gameObject, Action onComplete = null)
		{
			List<TransitionBase> list = TransitionIn(gameObject, isRecursiveCall: false);
			if (onComplete != null && list.Count > 0)
			{
				TransitionController.Instance.StartCoroutine(CallActionAfterDelay(GetTransitionInTime(list), onComplete));
			}
			return list;
		}

		private static List<TransitionBase> TransitionIn(GameObject gameObject, bool isRecursiveCall)
		{
			TransitionBase[] components = gameObject.GetComponents<TransitionBase>();
			List<TransitionBase> list = new List<TransitionBase>();
			bool flag = false;
			TransitionBase[] array = components;
			foreach (TransitionBase transitionBase in array)
			{
				if (transitionBase.isActiveAndEnabled && (!isRecursiveCall || !transitionBase.TransitionInConfig.MustTriggerDirect))
				{
					transitionBase.TransitionIn();
					list.Add(transitionBase);
					if (transitionBase.TransitionInConfig.TransitionChildren)
					{
						flag = true;
					}
				}
			}
			if (components.Length == 0 || flag)
			{
				for (int j = 0; j < gameObject.transform.childCount; j++)
				{
					Transform child = gameObject.transform.GetChild(j);
					list.AddRange(TransitionIn(child.gameObject, isRecursiveCall: true));
				}
			}
			return list;
		}

		public static List<TransitionBase> TransitionOut(GameObject gameObject, Action onComplete = null)
		{
			List<TransitionBase> list = TransitionOut(gameObject, isRecursiveCall: false);
			if (onComplete != null && list.Count > 0)
			{
				TransitionController.Instance.StartCoroutine(CallActionAfterDelay(GetTransitionOutTime(list), onComplete));
			}
			return list;
		}

		private static List<TransitionBase> TransitionOut(GameObject gameObject, bool isRecursiveCall)
		{
			TransitionBase[] components = gameObject.GetComponents<TransitionBase>();
			List<TransitionBase> list = new List<TransitionBase>();
			bool flag = false;
			TransitionBase[] array = components;
			foreach (TransitionBase transitionBase in array)
			{
				if (transitionBase.isActiveAndEnabled && (!isRecursiveCall || !transitionBase.TransitionOutConfig.MustTriggerDirect))
				{
					transitionBase.TransitionOut();
					list.Add(transitionBase);
					if (transitionBase.TransitionOutConfig.TransitionChildren)
					{
						flag = true;
					}
				}
			}
			if (components.Length == 0 || flag)
			{
				for (int j = 0; j < gameObject.transform.childCount; j++)
				{
					Transform child = gameObject.transform.GetChild(j);
					list.AddRange(TransitionOut(child.gameObject, isRecursiveCall: true));
				}
			}
			return list;
		}

		public static float GetTransitionInTime(List<TransitionBase> transitionBases)
		{
			float num = 0f;
			foreach (TransitionBase transitionBasis in transitionBases)
			{
				num = Mathf.Max(num, transitionBasis.TransitionInConfig.Delay + transitionBasis.TransitionInConfig.Duration);
			}
			return num;
		}

		public static float GetTransitionOutTime(List<TransitionBase> transitionBases)
		{
			float num = 0f;
			foreach (TransitionBase transitionBasis in transitionBases)
			{
				num = Mathf.Max(num, transitionBasis.TransitionOutConfig.Delay + transitionBasis.TransitionOutConfig.Duration);
			}
			return num;
		}

		public static IEnumerator CallActionAfterDelay(float delay, Action action)
		{
			yield return new WaitForSeconds(delay);
			action();
		}

		public static Texture2D TakeScreenshot()
		{
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false, linear: false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, recalculateMipMaps: false);
			texture2D.Apply();
			return texture2D;
		}

		public static void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.TransitionSteps
{
	public class ColorTransition : TransitionStep
	{
		private Color _startValue;

		private Color _endValue;

		private Image[] _images = new Image[0];

		private RawImage[] _rawImages = new RawImage[0];

		private Text[] _texts = new Text[0];

		private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[0];

		private Material[] _materials = new Material[0];

		private bool _hasComponentReferences;

		public Color StartValue
		{
			get
			{
				return _startValue;
			}
			set
			{
				_startValue = value;
				if (Gradient == null)
				{
					Gradient = new Gradient();
				}
				List<GradientColorKey> list = new List<GradientColorKey>(Gradient.colorKeys);
				if (Mathf.Approximately(Gradient.colorKeys[0].time, 0f))
				{
					list[0] = new GradientColorKey(EndValue, 0f);
				}
				else
				{
					list.Insert(0, new GradientColorKey(StartValue, 0f));
				}
				Gradient.colorKeys = list.ToArray();
				List<GradientAlphaKey> list2 = new List<GradientAlphaKey>(Gradient.alphaKeys);
				if (Mathf.Approximately(Gradient.alphaKeys[0].time, 0f))
				{
					list2[0] = new GradientAlphaKey(EndValue.a, 0f);
				}
				else
				{
					list2.Insert(0, new GradientAlphaKey(StartValue.a, 0f));
				}
				Gradient.alphaKeys = list2.ToArray();
			}
		}

		public Color EndValue
		{
			get
			{
				return _endValue;
			}
			set
			{
				_endValue = value;
				if (Gradient == null)
				{
					Gradient = new Gradient();
				}
				List<GradientColorKey> list = new List<GradientColorKey>(Gradient.colorKeys);
				if (Mathf.Approximately(Gradient.colorKeys[Gradient.colorKeys.Length - 1].time, 1f))
				{
					list[list.Count - 1] = new GradientColorKey(EndValue, 1f);
				}
				else
				{
					list.Add(new GradientColorKey(EndValue, 1f));
				}
				Gradient.colorKeys = list.ToArray();
				List<GradientAlphaKey> list2 = new List<GradientAlphaKey>(Gradient.alphaKeys);
				if (Mathf.Approximately(Gradient.alphaKeys[Gradient.alphaKeys.Length - 1].time, 1f))
				{
					list2[Gradient.alphaKeys.Length - 1] = new GradientAlphaKey(EndValue.a, 1f);
				}
				else
				{
					list2.Add(new GradientAlphaKey(EndValue.a, 1f));
				}
				Gradient.alphaKeys = list2.ToArray();
			}
		}

		public Color Value { get; set; }

		public Color OriginalValue { get; set; }

		public Gradient Gradient { get; set; }

		public ColorTransition(GameObject target, Color startColor, Color endColor, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, CoordinateSpaceType.Global, onStart, onUpdate, onComplete)
		{
			Gradient = new Gradient();
			StartValue = startColor;
			EndValue = endColor;
			OriginalValue = GetCurrent();
		}

		public ColorTransition(GameObject target, Gradient gradient = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, CoordinateSpaceType.Global, onStart, onUpdate, onComplete)
		{
			Gradient = gradient;
			OriginalValue = GetCurrent();
		}

		private TransitionStep SetStartValue(Color value)
		{
			StartValue = value;
			return this;
		}

		private TransitionStep SetEndValue(Color value)
		{
			EndValue = value;
			return this;
		}

		public Color GetCurrent()
		{
			if (!_hasComponentReferences)
			{
				SetupComponentReferences();
			}
			if (_images.Length != 0)
			{
				return _images[0].color;
			}
			if (_rawImages.Length != 0)
			{
				return _rawImages[0].color;
			}
			if (_texts.Length != 0)
			{
				return _texts[0].color;
			}
			if (_spriteRenderers.Length != 0)
			{
				return _spriteRenderers[0].color;
			}
			if (_materials.Length != 0)
			{
				return _materials[0].color;
			}
			return Color.black;
		}

		public void SetCurrent(Color color)
		{
			if (!_hasComponentReferences)
			{
				SetupComponentReferences();
			}
			Image[] images = _images;
			for (int i = 0; i < images.Length; i++)
			{
				images[i].color = color;
			}
			RawImage[] rawImages = _rawImages;
			for (int i = 0; i < rawImages.Length; i++)
			{
				rawImages[i].color = color;
			}
			Text[] texts = _texts;
			for (int i = 0; i < texts.Length; i++)
			{
				texts[i].color = color;
			}
			SpriteRenderer[] spriteRenderers = _spriteRenderers;
			for (int i = 0; i < spriteRenderers.Length; i++)
			{
				spriteRenderers[i].color = color;
			}
			Material[] materials = _materials;
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].color = color;
			}
		}

		private void SetupComponentReferences()
		{
			_images = new Image[0];
			_rawImages = new RawImage[0];
			_texts = new Text[0];
			_spriteRenderers = new SpriteRenderer[0];
			_materials = new Material[0];
			Image component = base.Target.GetComponent<Image>();
			if (component != null)
			{
				_images = _images.Concat(Enumerable.Repeat(component, 1)).ToArray();
			}
			RawImage component2 = base.Target.GetComponent<RawImage>();
			if (component2 != null)
			{
				_rawImages = _rawImages.Concat(Enumerable.Repeat(component2, 1)).ToArray();
			}
			Text component3 = base.Target.GetComponent<Text>();
			if (component3 != null)
			{
				_texts = _texts.Concat(Enumerable.Repeat(component3, 1)).ToArray();
			}
			SpriteRenderer component4 = base.Target.GetComponent<SpriteRenderer>();
			if (component4 != null)
			{
				_spriteRenderers = _spriteRenderers.Concat(Enumerable.Repeat(component4, 1)).ToArray();
			}
			MeshRenderer component5 = base.Target.GetComponent<MeshRenderer>();
			if (component5 != null && component5.material != null)
			{
				_materials = _materials.Concat(Enumerable.Repeat(component5.material, 1)).ToArray();
			}
			_hasComponentReferences = true;
		}

		public override void Start()
		{
			if (base.TransitionMode == TransitionModeType.ToOriginal)
			{
				EndValue = OriginalValue;
			}
			else if (base.TransitionMode == TransitionModeType.ToCurrent)
			{
				EndValue = GetCurrent();
			}
			else if (base.TransitionMode == TransitionModeType.FromCurrent)
			{
				StartValue = GetCurrent();
			}
			else if (base.TransitionMode == TransitionModeType.FromOriginal)
			{
				StartValue = OriginalValue;
			}
			base.Start();
		}

		protected override void ProgressUpdated()
		{
			SetCurrent(Gradient.Evaluate(base.ProgressTweened));
		}
	}
	public static class ColorTransitionExtensions
	{
		public static ColorTransition ColorTransition(this TransitionStep transitionStep, Gradient gradient, float delay = 0f, float duration = 0.5f, TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			ColorTransition colorTransition = new ColorTransition(transitionStep.Target, gradient, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, onComplete);
			colorTransition.AddToChain(transitionStep, runAtStart);
			return colorTransition;
		}

		public static ColorTransition ColorTransitionToOriginal(this TransitionStep transitionStep, Gradient gradient, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(gradient, delay, duration, TransitionStep.TransitionModeType.ToOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static ColorTransition ColorTransitionToCurrent(this TransitionStep transitionStep, Gradient gradient, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(gradient, delay, duration, TransitionStep.TransitionModeType.ToCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static ColorTransition ColorTransitionFromOriginal(this TransitionStep transitionStep, Gradient gradient, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(gradient, delay, duration, TransitionStep.TransitionModeType.FromOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static ColorTransition ColorTransitionFromCurrent(this TransitionStep transitionStep, Gradient gradient, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(gradient, delay, duration, TransitionStep.TransitionModeType.FromCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static ColorTransition ColorTransition(this TransitionStep transitionStep, Color startColor, Color endColor, float delay = 0f, float duration = 0.5f, TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			ColorTransition colorTransition = new ColorTransition(transitionStep.Target, startColor, endColor, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, onComplete);
			colorTransition.AddToChain(transitionStep, runAtStart);
			return colorTransition;
		}

		public static ColorTransition ColorTransitionToOriginal(this TransitionStep transitionStep, Color startColor, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(startColor, Color.black, delay, duration, TransitionStep.TransitionModeType.ToOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static ColorTransition ColorTransitionToCurrent(this TransitionStep transitionStep, Color startColor, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(startColor, Color.black, delay, duration, TransitionStep.TransitionModeType.ToCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static ColorTransition ColorTransitionFromOriginal(this TransitionStep transitionStep, Color endColor, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(Color.black, endColor, delay, duration, TransitionStep.TransitionModeType.FromOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static ColorTransition ColorTransitionFromCurrent(this TransitionStep transitionStep, Color endColor, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.ColorTransition(Color.black, endColor, delay, duration, TransitionStep.TransitionModeType.FromCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}
	}
	public class Fade : TransitionStepFloat
	{
		private CanvasGroup[] _canvasGroups = new CanvasGroup[0];

		private Image[] _images = new Image[0];

		private RawImage[] _rawImages = new RawImage[0];

		private Text[] _texts = new Text[0];

		private SpriteRenderer[] _spriteRenderers = new SpriteRenderer[0];

		private Material[] _materials = new Material[0];

		private bool _hasComponentReferences;

		public Fade(GameObject target, float startTransparency = 0f, float endTransparency = 1f, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, startTransparency, endTransparency, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, CoordinateSpaceType.Global, onStart, onUpdate, onComplete)
		{
		}

		public override float GetCurrent()
		{
			if (!_hasComponentReferences)
			{
				SetupComponentReferences();
			}
			if (_canvasGroups.Length != 0)
			{
				return _canvasGroups[0].alpha;
			}
			if (_images.Length != 0)
			{
				return _images[0].color.a;
			}
			if (_rawImages.Length != 0)
			{
				return _rawImages[0].color.a;
			}
			if (_texts.Length != 0)
			{
				return _texts[0].color.a;
			}
			if (_spriteRenderers.Length != 0)
			{
				return _spriteRenderers[0].color.a;
			}
			if (_materials.Length != 0)
			{
				return _materials[0].color.a;
			}
			return 1f;
		}

		public override void SetCurrent(float transparency)
		{
			if (!_hasComponentReferences)
			{
				SetupComponentReferences();
			}
			CanvasGroup[] canvasGroups = _canvasGroups;
			for (int i = 0; i < canvasGroups.Length; i++)
			{
				canvasGroups[i].alpha = transparency;
			}
			Image[] images = _images;
			foreach (Image image in images)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, transparency);
			}
			RawImage[] rawImages = _rawImages;
			foreach (RawImage rawImage in rawImages)
			{
				rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, transparency);
			}
			Text[] texts = _texts;
			foreach (Text text in texts)
			{
				text.color = new Color(text.color.r, text.color.g, text.color.b, transparency);
			}
			SpriteRenderer[] spriteRenderers = _spriteRenderers;
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
			{
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, transparency);
			}
			Material[] materials = _materials;
			foreach (Material material in materials)
			{
				material.color = new Color(material.color.r, material.color.g, material.color.b, transparency);
			}
		}

		private void SetupComponentReferences()
		{
			_canvasGroups = new CanvasGroup[0];
			_images = new Image[0];
			_rawImages = new RawImage[0];
			_texts = new Text[0];
			_spriteRenderers = new SpriteRenderer[0];
			_materials = new Material[0];
			CanvasGroup component = base.Target.GetComponent<CanvasGroup>();
			if (component != null)
			{
				_canvasGroups = _canvasGroups.Concat(Enumerable.Repeat(component, 1)).ToArray();
			}
			else
			{
				Image component2 = base.Target.GetComponent<Image>();
				if (component2 != null)
				{
					_images = _images.Concat(Enumerable.Repeat(component2, 1)).ToArray();
				}
				RawImage component3 = base.Target.GetComponent<RawImage>();
				if (component3 != null)
				{
					_rawImages = _rawImages.Concat(Enumerable.Repeat(component3, 1)).ToArray();
				}
				Text component4 = base.Target.GetComponent<Text>();
				if (component4 != null)
				{
					_texts = _texts.Concat(Enumerable.Repeat(component4, 1)).ToArray();
				}
			}
			SpriteRenderer component5 = base.Target.GetComponent<SpriteRenderer>();
			if (component5 != null)
			{
				_spriteRenderers = _spriteRenderers.Concat(Enumerable.Repeat(component5, 1)).ToArray();
			}
			MeshRenderer component6 = base.Target.GetComponent<MeshRenderer>();
			if (component6 != null && component6.material != null)
			{
				_materials = _materials.Concat(Enumerable.Repeat(component6.material, 1)).ToArray();
			}
			_hasComponentReferences = true;
		}
	}
	public static class FadeExtensions
	{
		public static Fade Fade(this TransitionStep transitionStep, float startTransparency, float endTransparency, float delay = 0f, float duration = 0.5f, TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			Fade fade = new Fade(transitionStep.Target, startTransparency, endTransparency, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, onComplete);
			fade.AddToChain(transitionStep, runAtStart);
			return fade;
		}

		public static Fade FadeToOriginal(this TransitionStep transitionStep, float startTransparency, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Fade(startTransparency, 0f, delay, duration, TransitionStep.TransitionModeType.ToOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Fade FadeToCurrent(this TransitionStep transitionStep, float startTransparency, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Fade(startTransparency, 0f, delay, duration, TransitionStep.TransitionModeType.ToCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Fade FadeFromOriginal(this TransitionStep transitionStep, float endTransparency, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Fade(0f, endTransparency, delay, duration, TransitionStep.TransitionModeType.FromOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Fade FadeFromCurrent(this TransitionStep transitionStep, float endTransparency, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Fade(0f, endTransparency, delay, duration, TransitionStep.TransitionModeType.FromCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}
	}
	public class Move : TransitionStepVector3
	{
		public Move(GameObject target, Vector3? startPosition = null, Vector3? endPosition = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, startPosition, endPosition, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, coordinateSpace, onStart, onUpdate, onComplete)
		{
		}

		public override Vector3 GetCurrent()
		{
			if (base.CoordinateSpace == CoordinateSpaceType.Global)
			{
				return base.Target.transform.position;
			}
			if (base.CoordinateSpace == CoordinateSpaceType.Local)
			{
				return base.Target.transform.localPosition;
			}
			return ((RectTransform)base.Target.transform).anchoredPosition;
		}

		public override void SetCurrent(Vector3 position)
		{
			if (base.CoordinateSpace == CoordinateSpaceType.Global)
			{
				base.Target.transform.position = position;
			}
			else if (base.CoordinateSpace == CoordinateSpaceType.Local)
			{
				base.Target.transform.localPosition = position;
			}
			else
			{
				((RectTransform)base.Target.transform).anchoredPosition = position;
			}
		}
	}
	public static class MoveExtensions
	{
		public static Move Move(this TransitionStep transitionStep, Vector3 startPosition, Vector3 endPosition, float delay = 0f, float duration = 0.5f, TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			Move move = new Move(transitionStep.Target, startPosition, endPosition, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, coordinateMode, onStart, onUpdate, onComplete);
			move.AddToChain(transitionStep, runAtStart);
			return move;
		}

		public static Move MoveToOriginal(this TransitionStep transitionStep, Vector3 startPosition, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Move(startPosition, Vector3.zero, delay, duration, TransitionStep.TransitionModeType.ToOriginal, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Move MoveToCurrent(this TransitionStep transitionStep, Vector3 startPosition, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Move(startPosition, Vector3.zero, delay, duration, TransitionStep.TransitionModeType.ToCurrent, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Move MoveFromOriginal(this TransitionStep transitionStep, Vector3 endPosition, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Move(Vector3.zero, endPosition, delay, duration, TransitionStep.TransitionModeType.FromOriginal, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Move MoveFromCurrent(this TransitionStep transitionStep, Vector3 endPosition, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Move(Vector3.zero, endPosition, delay, duration, TransitionStep.TransitionModeType.FromCurrent, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}
	}
	public class Rotate : TransitionStepVector3
	{
		public Rotate(GameObject target, Vector3? startRotation = null, Vector3? endRotation = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, startRotation, endRotation, delay, duration, transitionMode, TimeUpdateMethodType.GameTime, tweenType, animationCurve, coordinateSpace, onStart, onUpdate, onComplete)
		{
		}

		public override Vector3 GetCurrent()
		{
			if (base.CoordinateSpace == CoordinateSpaceType.Global)
			{
				return base.Target.transform.eulerAngles;
			}
			return base.Target.transform.localEulerAngles;
		}

		public override void SetCurrent(Vector3 rotation)
		{
			if (base.CoordinateSpace == CoordinateSpaceType.Global)
			{
				base.Target.transform.eulerAngles = rotation;
			}
			else
			{
				base.Target.transform.localEulerAngles = rotation;
			}
		}
	}
	public static class RotateExtensions
	{
		public static Rotate Rotate(this TransitionStep transitionStep, Vector3 startRotation, Vector3 endRotation, float delay = 0f, float duration = 0.5f, TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			Rotate rotate = new Rotate(transitionStep.Target, startRotation, endRotation, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, coordinateMode, onStart, onUpdate, onComplete);
			rotate.AddToChain(transitionStep, runAtStart);
			return rotate;
		}

		public static Rotate RotateToOriginal(this TransitionStep transitionStep, Vector3 startRotation, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Rotate(startRotation, Vector3.zero, delay, duration, TransitionStep.TransitionModeType.ToOriginal, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Rotate RotateToCurrent(this TransitionStep transitionStep, Vector3 startRotation, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Rotate(startRotation, Vector3.zero, delay, duration, TransitionStep.TransitionModeType.ToCurrent, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Rotate RotateFromOriginal(this TransitionStep transitionStep, Vector3 endRotation, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Rotate(Vector3.zero, endRotation, delay, duration, TransitionStep.TransitionModeType.FromOriginal, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Rotate RotateFromCurrent(this TransitionStep transitionStep, Vector3 endRotation, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Rotate(Vector3.zero, endRotation, delay, duration, TransitionStep.TransitionModeType.FromCurrent, timeUpdateMethod, tweenType, animationCurve, coordinateMode, runAtStart, onStart, onUpdate, onComplete);
		}
	}
	public class Scale : TransitionStepVector3
	{
		public Scale(GameObject target, Vector3? startScale = null, Vector3? endScale = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, startScale, endScale, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, CoordinateSpaceType.Global, onStart, onUpdate, onComplete)
		{
		}

		public override Vector3 GetCurrent()
		{
			return base.Target.transform.localScale;
		}

		public override void SetCurrent(Vector3 scale)
		{
			base.Target.transform.localScale = scale;
		}
	}
	public static class ScaleExtensions
	{
		public static Scale Scale(this TransitionStep transitionStep, Vector3 startScale, Vector3 endScale, float delay = 0f, float duration = 0.5f, TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			Scale scale = new Scale(transitionStep.Target, startScale, endScale, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, onComplete);
			scale.AddToChain(transitionStep, runAtStart);
			return scale;
		}

		public static Scale ScaleToOriginal(this TransitionStep transitionStep, Vector3 startScale, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Scale(startScale, Vector3.zero, delay, duration, TransitionStep.TransitionModeType.ToOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Scale ScaleToCurrent(this TransitionStep transitionStep, Vector3 startScale, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Scale(startScale, Vector3.zero, delay, duration, TransitionStep.TransitionModeType.ToCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Scale ScaleFromOriginal(this TransitionStep transitionStep, Vector3 endScale, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Scale(Vector3.zero, endScale, delay, duration, TransitionStep.TransitionModeType.FromOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static Scale ScaleFromCurrent(this TransitionStep transitionStep, Vector3 endScale, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.Scale(Vector3.zero, endScale, delay, duration, TransitionStep.TransitionModeType.FromCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}
	}
	public class ScreenFade : TransitionStepScreen
	{
		public Texture2D Texture;

		public Color Color;

		private readonly ScreenFadeComponents _screenFadeComponents;

		public ScreenFade(GameObject target, Color? color = null, Texture2D texture = null, float delay = 0f, float duration = 0.5f, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, SceneChangeModeType.None, null, skipOnCrossTransition: true, delay, duration, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, null, onComplete)
		{
			_screenFadeComponents = new ScreenFadeComponents
			{
				PersistantAcrossScenes = true
			};
			Color = (color.HasValue ? color.Value : Color.white);
			Texture = texture;
		}

		public override void Start()
		{
			SetConfiguration(Texture, Color);
			base.Start();
		}

		public override void SetCurrent(float progress)
		{
			TargetComponents().FadeRawImage.color = new Color(TargetComponents().FadeRawImage.color.r, TargetComponents().FadeRawImage.color.g, TargetComponents().FadeRawImage.color.b, base.Value);
		}

		private void SetConfiguration(Texture2D texture, Color color)
		{
			TargetComponents().FadeRawImage.texture = texture;
			TargetComponents().FadeRawImage.color = color;
		}

		protected override void SetTransitionDisplayedState(bool isDisplayed)
		{
			if (base.SiblingRawImage != null)
			{
				base.SetTransitionDisplayedState(isDisplayed);
			}
			else
			{
				TargetComponents().BaseGameObject.SetActive(isDisplayed);
			}
			if (base.SceneChangeMode == SceneChangeModeType.CrossTransition)
			{
				if (isDisplayed)
				{
					TargetComponents().FadeRawImage.texture = TransitionController.Instance.ScreenSnapshot;
				}
				else
				{
					TargetComponents().DeleteComponents();
				}
			}
		}

		private ScreenFadeComponents TargetComponents()
		{
			if (base.SceneChangeMode != SceneChangeModeType.CrossTransition)
			{
				return TransitionController.Instance.SharedScreenFadeComponents;
			}
			return _screenFadeComponents;
		}
	}
	public class ScreenFadeComponents
	{
		private GameObject _baseGameObject;

		private RawImage _fadeRawImage;

		public bool PersistantAcrossScenes { get; set; }

		public GameObject BaseGameObject
		{
			get
			{
				if (_baseGameObject == null)
				{
					CreateComponents();
				}
				return _baseGameObject;
			}
			private set
			{
				_baseGameObject = value;
			}
		}

		public RawImage FadeRawImage
		{
			get
			{
				if (_fadeRawImage == null)
				{
					CreateComponents();
				}
				return _fadeRawImage;
			}
			set
			{
				_fadeRawImage = value;
			}
		}

		private void CreateComponents()
		{
			BaseGameObject = new GameObject("(Beautiful Transitions - ScreenFade");
			if (PersistantAcrossScenes)
			{
				BaseGameObject.transform.SetParent(TransitionController.Instance.gameObject.transform);
			}
			BaseGameObject.SetActive(value: false);
			Canvas canvas = BaseGameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 999;
			FadeRawImage = BaseGameObject.AddComponent<RawImage>();
		}

		public void DeleteComponents()
		{
			if (BaseGameObject != null)
			{
				UnityEngine.Object.Destroy(BaseGameObject);
			}
		}
	}
	public static class ScreenFadeExtensions
	{
		public static ScreenFade ScreenFade(this TransitionStep transitionStep, Color? color = null, Texture2D texture = null, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			ScreenFade screenFade = new ScreenFade(transitionStep.Target, color, texture, delay, duration, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, onComplete);
			screenFade.AddToChain(transitionStep, runAtStart: false);
			return screenFade;
		}
	}
	public class ScreenWipe : TransitionStepScreen
	{
		public Texture2D Texture;

		public Color Color;

		public Texture2D MaskTexture;

		public bool InvertMask;

		public float Softness;

		private readonly ScreenWipeComponents _screenWipeComponents;

		public ScreenWipe(GameObject target, Texture2D maskTexture, bool invertMask = false, Color? color = null, Texture2D texture = null, float softness = 0f, float delay = 0f, float duration = 0.5f, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, SceneChangeModeType.None, null, skipOnCrossTransition: true, delay, duration, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, null, onComplete)
		{
			_screenWipeComponents = new ScreenWipeComponents
			{
				PersistantAcrossScenes = true
			};
			MaskTexture = maskTexture;
			InvertMask = invertMask;
			Color = (color.HasValue ? color.Value : Color.white);
			Texture = texture;
			Softness = softness;
		}

		public override void Start()
		{
			SetConfiguration(Texture, Color, MaskTexture, InvertMask, Softness);
			base.Start();
		}

		public override void SetCurrent(float progress)
		{
			TargetComponents().WipeMaterial.SetFloat("_Amount", base.Value);
		}

		private void SetConfiguration(Texture2D texture, Color color, Texture2D maskTexture, bool invertMask, float softness = 0f)
		{
			TargetComponents().WipeRawImage.texture = texture;
			TargetComponents().WipeMaterial.SetColor("_Color", color);
			TargetComponents().WipeMaterial.SetTexture("_MaskTex", maskTexture);
			if (invertMask)
			{
				TargetComponents().WipeMaterial.EnableKeyword("INVERT_MASK");
			}
			else
			{
				TargetComponents().WipeMaterial.DisableKeyword("INVERT_MASK");
			}
			TargetComponents().WipeMaterial.SetFloat("_Softness", softness);
		}

		protected override void SetTransitionDisplayedState(bool isDisplayed)
		{
			if (base.SiblingRawImage != null)
			{
				base.SetTransitionDisplayedState(isDisplayed);
			}
			else
			{
				TargetComponents().BaseGameObject.SetActive(isDisplayed);
			}
			if (base.SceneChangeMode == SceneChangeModeType.CrossTransition)
			{
				if (isDisplayed)
				{
					TargetComponents().WipeRawImage.texture = TransitionController.Instance.ScreenSnapshot;
				}
				else
				{
					TargetComponents().DeleteComponents();
				}
			}
		}

		private ScreenWipeComponents TargetComponents()
		{
			if (base.SceneChangeMode != SceneChangeModeType.CrossTransition)
			{
				return TransitionController.Instance.SharedScreenWipeComponents;
			}
			return _screenWipeComponents;
		}
	}
	public class ScreenWipeComponents
	{
		private GameObject _baseGameObject;

		private RawImage _wipeRawImage;

		private Material _wipeMaterial;

		public bool PersistantAcrossScenes { get; set; }

		public GameObject BaseGameObject
		{
			get
			{
				if (_baseGameObject == null)
				{
					CreateComponents();
				}
				return _baseGameObject;
			}
			private set
			{
				_baseGameObject = value;
			}
		}

		public RawImage WipeRawImage
		{
			get
			{
				if (_wipeRawImage == null)
				{
					CreateComponents();
				}
				return _wipeRawImage;
			}
			set
			{
				_wipeRawImage = value;
			}
		}

		public Material WipeMaterial
		{
			get
			{
				if (_wipeMaterial == null)
				{
					CreateComponents();
				}
				return _wipeMaterial;
			}
			set
			{
				_wipeMaterial = value;
			}
		}

		private void CreateComponents()
		{
			BaseGameObject = new GameObject("(Beautiful Transitions - ScreenWipe");
			if (PersistantAcrossScenes)
			{
				BaseGameObject.transform.SetParent(TransitionController.Instance.gameObject.transform);
			}
			BaseGameObject.SetActive(value: false);
			Canvas canvas = BaseGameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 999;
			WipeRawImage = BaseGameObject.AddComponent<RawImage>();
			Shader shader = Shader.Find("FlipWebApps/BeautifulTransitions/WipeScreen");
			if (shader != null && shader.isSupported)
			{
				RawImage wipeRawImage = WipeRawImage;
				Material material = (WipeMaterial = new Material(shader));
				wipeRawImage.material = material;
			}
			else
			{
				UnityEngine.Debug.Log("WipScreen: Shader is not found or supported on this platform.");
			}
		}

		public void DeleteComponents()
		{
			if (BaseGameObject != null)
			{
				UnityEngine.Object.Destroy(BaseGameObject);
			}
		}
	}
	public static class ScreenWipeExtensions
	{
		public static ScreenWipe ScreenWipe(this TransitionStep transitionStep, Texture2D maskTexture, bool invertMask = false, Color? color = null, Texture2D texture = null, float softness = 0f, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			ScreenWipe screenWipe = new ScreenWipe(transitionStep.Target, maskTexture, invertMask, color, texture, softness, delay, duration, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, onComplete);
			screenWipe.AddToChain(transitionStep, runAtStart: false);
			return screenWipe;
		}
	}
	public class TransitionController : MonoBehaviour
	{
		private static TransitionController _instance;

		private bool _newSceneLoaded;

		public Texture2D ScreenSnapshot { get; set; }

		public bool IsInCrossTransition { get; set; }

		public ScreenWipeComponents SharedScreenWipeComponents { get; set; }

		public ScreenFadeComponents SharedScreenFadeComponents { get; set; }

		public static TransitionController Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject obj = new GameObject("(Beautiful Transitions - Controller)");
					_instance = obj.AddComponent<TransitionController>();
					_instance.Setup();
					UnityEngine.Object.DontDestroyOnLoad(obj);
				}
				return _instance;
			}
			private set
			{
				_instance = value;
			}
		}

		public static bool IsActive => Instance != null;

		private void Setup()
		{
			SharedScreenWipeComponents = new ScreenWipeComponents();
			SharedScreenFadeComponents = new ScreenFadeComponents();
		}

		public IEnumerator LoadSceneAndWaitForLoad(string sceneToLoad)
		{
			SceneManager.sceneLoaded += OnSceneFinishedLoading;
			TransitionHelper.LoadScene(sceneToLoad);
			while (!_newSceneLoaded)
			{
				yield return null;
			}
			_newSceneLoaded = false;
			SceneManager.sceneLoaded -= OnSceneFinishedLoading;
		}

		private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
		{
			_newSceneLoaded = true;
		}

		public void TakeScreenshot()
		{
			StartCoroutine(TakeScreenshotCoroutine());
		}

		public IEnumerator TakeScreenshotCoroutine()
		{
			yield return new WaitForEndOfFrame();
			ScreenSnapshot = TransitionHelper.TakeScreenshot();
		}
	}
	public class TransitionStepFloat : TransitionStepValue<float>
	{
		public TransitionStepFloat(GameObject target = null, float? startValue = null, float? endValue = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, coordinateSpace, onStart, onUpdate, onComplete)
		{
			base.StartValue = startValue.GetValueOrDefault();
			base.EndValue = endValue.GetValueOrDefault();
			base.OriginalValue = GetCurrent();
		}

		private TransitionStep SetStartValue(float value)
		{
			base.StartValue = value;
			return this;
		}

		private TransitionStep SetEndValue(float value)
		{
			base.EndValue = value;
			return this;
		}

		public override void Start()
		{
			if (base.TransitionMode == TransitionModeType.ToOriginal)
			{
				base.EndValue = base.OriginalValue;
			}
			else if (base.TransitionMode == TransitionModeType.ToCurrent)
			{
				base.EndValue = GetCurrent();
			}
			else if (base.TransitionMode == TransitionModeType.FromCurrent)
			{
				base.StartValue = GetCurrent();
			}
			else if (base.TransitionMode == TransitionModeType.FromOriginal)
			{
				base.StartValue = base.OriginalValue;
			}
			base.Start();
		}

		protected override void ProgressUpdated()
		{
			base.Value = ValueFromProgressTweened(base.StartValue, base.EndValue);
			SetCurrent(base.Value);
		}
	}
	public class TransitionStepScreen : TransitionStepFloat
	{
		public enum SceneChangeModeType
		{
			None,
			CrossTransition,
			End
		}

		protected RawImage SiblingRawImage { get; set; }

		public SceneChangeModeType SceneChangeMode { get; set; }

		public string SceneToLoad { get; set; }

		public bool SkipOnCrossTransition { get; set; }

		public TransitionStepScreen(GameObject target, SceneChangeModeType sceneChangeMode = SceneChangeModeType.None, string sceneToLoad = null, bool skipOnCrossTransition = true, float delay = 0f, float duration = 0.5f, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, TransitionStep onCompleteItem = null, Action<TransitionStep> onComplete = null, Action<object> onCompleteWithData = null, object onCompleteData = null)
			: base(target, null, null, delay, duration, TransitionModeType.Specified, timeUpdateMethod, tweenType, animationCurve, CoordinateSpaceType.Global, onStart, onUpdate, onComplete)
		{
			SceneChangeMode = sceneChangeMode;
			SceneToLoad = sceneToLoad;
			SkipOnCrossTransition = skipOnCrossTransition;
			SetupComponents();
		}

		protected override IEnumerator TransitionLoop()
		{
			if (SkipOnCrossTransition && TransitionController.Instance.IsInCrossTransition)
			{
				yield break;
			}
			if (Mathf.Approximately(base.Delay + base.Duration, 0f))
			{
				SetProgressToEnd();
				TransitionStarted();
			}
			else
			{
				TransitionStarted();
				if (!Mathf.Approximately(base.Delay, 0f))
				{
					yield return new WaitForSeconds(base.Delay);
				}
				if (SceneChangeMode == SceneChangeModeType.CrossTransition)
				{
					TransitionController.Instance.IsInCrossTransition = true;
					yield return TransitionController.Instance.StartCoroutine(TransitionController.Instance.TakeScreenshotCoroutine());
					SetTransitionDisplayedState(isDisplayed: true);
					base.StartValue = 1f;
					base.EndValue = 0f;
					SetProgressToStart();
					yield return TransitionController.Instance.StartCoroutine(TransitionController.Instance.LoadSceneAndWaitForLoad(SceneToLoad));
				}
				else
				{
					SetTransitionDisplayedState(isDisplayed: true);
					SetProgressToStart();
				}
				float normalisedFactor = (Mathf.Approximately(base.Duration, 0f) ? float.MaxValue : (1f / base.Duration));
				while (base.Progress < 1f && !base.IsStopped)
				{
					if (!base.IsPaused)
					{
						SetProgress(base.Progress + normalisedFactor * Time.deltaTime);
					}
					yield return 0;
				}
			}
			if (Mathf.Approximately(base.Progress, 1f) && !base.IsStopped)
			{
				TransitionCompleted();
			}
		}

		protected override void TransitionCompleted()
		{
			if (Mathf.Approximately(base.EndValue, 0f))
			{
				SetTransitionDisplayedState(isDisplayed: false);
			}
			base.TransitionCompleted();
			if (SceneChangeMode == SceneChangeModeType.End)
			{
				TransitionHelper.LoadScene(SceneToLoad);
			}
			if (SceneChangeMode == SceneChangeModeType.CrossTransition)
			{
				TransitionController.Instance.IsInCrossTransition = false;
			}
		}

		protected virtual void SetupComponents()
		{
			SiblingRawImage = base.Target.GetComponent<RawImage>();
			if (SiblingRawImage != null)
			{
				SiblingRawImage.enabled = false;
			}
		}

		protected virtual void SetTransitionDisplayedState(bool isDisplayed)
		{
			if (SiblingRawImage != null)
			{
				SiblingRawImage.enabled = isDisplayed;
			}
		}
	}
	public class TransitionStepVector3 : TransitionStepValue<Vector3>
	{
		public TransitionStepVector3(GameObject target = null, Vector3? startValue = null, Vector3? endValue = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, coordinateSpace, onStart, onUpdate, onComplete)
		{
			base.StartValue = startValue.GetValueOrDefault();
			base.EndValue = endValue.GetValueOrDefault();
			base.OriginalValue = GetCurrent();
		}

		private TransitionStep SetStartValue(Vector3 value)
		{
			base.StartValue = value;
			return this;
		}

		private TransitionStep SetEndValue(Vector3 value)
		{
			base.EndValue = value;
			return this;
		}

		public override void Start()
		{
			if (base.TransitionMode == TransitionModeType.ToOriginal)
			{
				base.EndValue = base.OriginalValue;
			}
			else if (base.TransitionMode == TransitionModeType.ToCurrent)
			{
				base.EndValue = GetCurrent();
			}
			else if (base.TransitionMode == TransitionModeType.FromCurrent)
			{
				base.StartValue = GetCurrent();
			}
			else if (base.TransitionMode == TransitionModeType.FromOriginal)
			{
				base.StartValue = base.OriginalValue;
			}
			base.Start();
		}

		protected override void ProgressUpdated()
		{
			base.Value = new Vector3(ValueFromProgressTweened(base.StartValue.x, base.EndValue.x), ValueFromProgressTweened(base.StartValue.y, base.EndValue.y), ValueFromProgressTweened(base.StartValue.z, base.EndValue.z));
			SetCurrent(base.Value);
		}
	}
	public class TriggerAnimation : TransitionStepFloat
	{
		public float Speed { get; set; }

		public Animator Animator { get; set; }

		public string Trigger { get; set; }

		public string TargetState { get; set; }

		public TriggerAnimation(GameObject target, float speed = 1f, float delay = 0f, float duration = 0.5f, string trigger = "TransitionIn", string targetState = "TransitionOut", Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, null, null, delay, duration, TransitionModeType.Specified, TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, CoordinateSpaceType.Global, onStart, onUpdate, onComplete)
		{
			SetupComponentReferences();
			Animator.enabled = false;
			Speed = speed;
			Trigger = trigger;
			TargetState = targetState;
		}

		protected override IEnumerator TransitionLoop()
		{
			if (Mathf.Approximately(base.Delay + base.Duration, 0f))
			{
				SetProgressToEnd();
				TransitionStarted();
				yield break;
			}
			SetProgressToStart();
			TransitionStarted();
			if (!Mathf.Approximately(base.Delay, 0f))
			{
				yield return new WaitForSeconds(base.Delay);
			}
			Animator.enabled = true;
			Animator.SetTrigger(Trigger);
			Animator.speed = Speed;
			bool stateReached = false;
			while (!stateReached)
			{
				yield return new WaitForEndOfFrame();
				if (!Animator.IsInTransition(0))
				{
					stateReached = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f || !Animator.GetCurrentAnimatorStateInfo(0).IsName(TargetState);
				}
			}
			if (Mathf.Approximately(base.Progress, 1f) && !base.IsStopped)
			{
				TransitionCompleted();
			}
		}

		private void SetupComponentReferences()
		{
			Animator = base.Target.GetComponent<Animator>();
		}
	}
	public static class AnimationExtensions
	{
		public static TriggerAnimation TriggerAnimation(this TransitionStep transitionStep, float speed = 1f, float delay = 0f, float duration = 0.5f, string trigger = "TransitionIn", string targetState = "TransitionOut", bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			TriggerAnimation triggerAnimation = new TriggerAnimation(transitionStep.Target, speed, delay, duration, trigger, targetState, onStart, onUpdate, onComplete);
			triggerAnimation.AddToChain(transitionStep, runAtStart);
			return triggerAnimation;
		}
	}
	public class VolumeTransition : TransitionStepFloat
	{
		private AudioSource[] _audioSources = new AudioSource[0];

		private bool _hasComponentReferences;

		public VolumeTransition(GameObject target, float startVolume = 0f, float endVolume = 1f, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, startVolume, endVolume, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, CoordinateSpaceType.Global, onStart, onUpdate, onComplete)
		{
		}

		public override float GetCurrent()
		{
			if (!_hasComponentReferences)
			{
				SetupComponentReferences();
			}
			if (_audioSources.Length != 0)
			{
				return _audioSources[0].volume;
			}
			return 1f;
		}

		public override void SetCurrent(float volume)
		{
			if (!_hasComponentReferences)
			{
				SetupComponentReferences();
			}
			AudioSource[] audioSources = _audioSources;
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].volume = volume;
			}
		}

		private void SetupComponentReferences()
		{
			_audioSources = new AudioSource[0];
			AudioSource component = base.Target.GetComponent<AudioSource>();
			if (component != null)
			{
				_audioSources = _audioSources.Concat(Enumerable.Repeat(component, 1)).ToArray();
			}
			_hasComponentReferences = true;
		}
	}
	public static class VolumeTransitionExtensions
	{
		public static VolumeTransition VolumeTransition(this TransitionStep transitionStep, float startVolume, float endVolume, float delay = 0f, float duration = 0.5f, TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			VolumeTransition volumeTransition = new VolumeTransition(transitionStep.Target, startVolume, endVolume, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, onStart, onUpdate, onComplete);
			volumeTransition.AddToChain(transitionStep, runAtStart);
			return volumeTransition;
		}

		public static VolumeTransition VolumeToOriginal(this TransitionStep transitionStep, float startVolume, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.VolumeTransition(startVolume, 0f, delay, duration, TransitionStep.TransitionModeType.ToOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static VolumeTransition VolumeToCurrent(this TransitionStep transitionStep, float startVolume, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.VolumeTransition(startVolume, 0f, delay, duration, TransitionStep.TransitionModeType.ToCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static VolumeTransition VolumeFromOriginal(this TransitionStep transitionStep, float endVolume, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.VolumeTransition(0f, endVolume, delay, duration, TransitionStep.TransitionModeType.FromOriginal, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}

		public static VolumeTransition VolumeFromCurrent(this TransitionStep transitionStep, float endVolume, float delay = 0f, float duration = 0.5f, TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			return transitionStep.VolumeTransition(0f, endVolume, delay, duration, TransitionStep.TransitionModeType.FromCurrent, timeUpdateMethod, tweenType, animationCurve, runAtStart, onStart, onUpdate, onComplete);
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses
{
	public class TransitionStep
	{
		public enum TimeUpdateMethodType
		{
			GameTime,
			UnscaledGameTime
		}

		public enum CoordinateSpaceType
		{
			Global,
			Local,
			AnchoredPosition
		}

		public enum TransitionModeType
		{
			Specified,
			ToOriginal,
			FromCurrent,
			FromOriginal,
			ToCurrent
		}

		public enum LoopModeType
		{
			None,
			Loop,
			PingPong
		}

		private TransitionHelper.TweenType _tweenType;

		private TweenMethods.TweenFunction _tweenFunction;

		public GameObject Target { get; private set; }

		public float Progress { get; private set; }

		public float ProgressTweened { get; private set; }

		public float Delay { get; set; }

		public float Duration { get; set; }

		public TransitionHelper.TweenType TweenType
		{
			get
			{
				return _tweenType;
			}
			set
			{
				_tweenType = value;
				_tweenFunction = TransitionHelper.GetTweenFunction(TweenType);
			}
		}

		public TimeUpdateMethodType TimeUpdateMethod { get; set; }

		public LoopModeType LoopMode { get; set; }

		public TransitionModeType TransitionMode { get; set; }

		public AnimationCurve AnimationCurve { get; set; }

		public CoordinateSpaceType CoordinateSpace { get; set; }

		public TransitionStep Parent { get; set; }

		public Action<TransitionStep> OnStart { get; set; }

		public Action<TransitionStep> OnUpdate { get; set; }

		public Action<TransitionStep> OnComplete { get; set; }

		public object UserData { get; set; }

		public bool IsStopped { get; protected set; }

		public bool IsPaused { get; protected set; }

		public TransitionStep(GameObject target = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			Target = target;
			Delay = delay;
			Duration = duration;
			TransitionMode = transitionMode;
			TimeUpdateMethod = timeUpdateMethod;
			TweenType = tweenType;
			AnimationCurve = animationCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			CoordinateSpace = coordinateSpace;
			AddOnStartAction(onStart);
			AddOnUpdateAction(onUpdate);
			AddOnCompleteAction(onComplete);
		}

		public TransitionStep SetDelay(float delay)
		{
			Delay = delay;
			return this;
		}

		public TransitionStep SetDuration(float duration)
		{
			Duration = duration;
			return this;
		}

		public TransitionStep SetTweenType(TransitionHelper.TweenType tweenType)
		{
			TweenType = tweenType;
			return this;
		}

		public TransitionStep SetTimeUpdateMethod(TimeUpdateMethodType timeUpdateMethod)
		{
			TimeUpdateMethod = timeUpdateMethod;
			return this;
		}

		public TransitionStep SetLoopMode(LoopModeType loopMode)
		{
			LoopMode = loopMode;
			return this;
		}

		public TransitionStep SetTransitionMode(TransitionModeType transitionMode)
		{
			TransitionMode = transitionMode;
			return this;
		}

		public TransitionStep SetAnimationCurve(AnimationCurve animationCurve)
		{
			AnimationCurve = animationCurve;
			return this;
		}

		public TransitionStep SetCoordinateMode(CoordinateSpaceType coordinateMode)
		{
			CoordinateSpace = coordinateMode;
			return this;
		}

		public TransitionStep AddOnStartAction(Action<TransitionStep> action)
		{
			OnStart = (Action<TransitionStep>)Delegate.Combine(OnStart, action);
			return this;
		}

		public TransitionStep AddOnUpdateAction(Action<TransitionStep> action)
		{
			OnUpdate = (Action<TransitionStep>)Delegate.Combine(OnUpdate, action);
			return this;
		}

		public TransitionStep AddOnCompleteAction(Action<TransitionStep> action)
		{
			OnComplete = (Action<TransitionStep>)Delegate.Combine(OnComplete, action);
			return this;
		}

		public TransitionStep AddOnCompleteAction(Action<TransitionStep> action, object userData)
		{
			OnComplete = (Action<TransitionStep>)Delegate.Combine(OnComplete, action);
			UserData = userData;
			return this;
		}

		public TransitionStep AddOnStartTransitionStep(TransitionStep transitionStep)
		{
			if (transitionStep != null)
			{
				OnStart = (Action<TransitionStep>)Delegate.Combine(OnStart, new Action<TransitionStep>(transitionStep.Start));
			}
			return this;
		}

		public TransitionStep AddOnCompleteTransitionStep(TransitionStep transitionStep)
		{
			if (transitionStep != null)
			{
				OnComplete = (Action<TransitionStep>)Delegate.Combine(OnComplete, new Action<TransitionStep>(transitionStep.Start));
			}
			return this;
		}

		public TransitionStep ChainCustomTransitionStep(float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, bool runAtStart = false, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
		{
			TransitionStep transitionStep = new TransitionStep(Target, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, CoordinateSpaceType.Global, onStart, onUpdate, onComplete);
			transitionStep.AddToChain(this, runAtStart);
			return transitionStep;
		}

		public TransitionStep GetChainRoot()
		{
			TransitionStep transitionStep = this;
			while (transitionStep.Parent != null)
			{
				transitionStep = transitionStep.Parent;
			}
			return transitionStep;
		}

		public void AddToChain(TransitionStep parent, bool runAtStart)
		{
			if (runAtStart)
			{
				parent.AddOnStartTransitionStep(this);
			}
			else
			{
				parent.AddOnCompleteTransitionStep(this);
			}
			Parent = parent;
		}

		public virtual void Start()
		{
			IsStopped = false;
			IsPaused = false;
			TransitionController.Instance.StartCoroutine(TransitionLoop());
		}

		public virtual void Start(TransitionStep transitionStep)
		{
			Start();
		}

		public virtual void Stop()
		{
			IsStopped = true;
		}

		public virtual void Pause()
		{
			IsPaused = true;
		}

		public virtual void Resume()
		{
			IsPaused = false;
		}

		public virtual void Complete()
		{
			SetProgressToEnd();
		}

		protected virtual IEnumerator TransitionLoop()
		{
			if (Mathf.Approximately(Delay + Duration, 0f))
			{
				SetProgressToEnd();
				TransitionStarted();
			}
			else
			{
				SetProgressToStart();
				TransitionStarted();
				if (!Mathf.Approximately(Delay, 0f))
				{
					float delayTime = 0f;
					while (delayTime < Delay)
					{
						if (!IsPaused)
						{
							delayTime += ((TimeUpdateMethod == TimeUpdateMethodType.GameTime) ? Time.deltaTime : Time.unscaledDeltaTime);
						}
						yield return 0;
					}
				}
				float normalisedFactor = (Mathf.Approximately(Duration, 0f) ? float.MaxValue : (1f / Duration));
				while (!IsStopped)
				{
					if (!IsPaused)
					{
						Progress += normalisedFactor * ((TimeUpdateMethod == TimeUpdateMethodType.GameTime) ? Time.deltaTime : Time.unscaledDeltaTime);
						if (LoopMode == LoopModeType.Loop && Progress >= 1f)
						{
							Progress = 0f;
						}
						if (LoopMode == LoopModeType.PingPong && Progress >= 1f)
						{
							normalisedFactor *= -1f;
							Progress = 1f;
						}
						if (LoopMode == LoopModeType.PingPong && Progress <= 0f)
						{
							normalisedFactor *= -1f;
							Progress = 0f;
						}
						SetProgress(Progress);
						if (LoopMode == LoopModeType.None && Progress >= 1f)
						{
							break;
						}
					}
					yield return 0;
				}
			}
			if (Mathf.Approximately(Progress, 1f) && !IsStopped)
			{
				TransitionCompleted();
			}
		}

		protected virtual void TransitionStarted()
		{
			if (OnStart != null)
			{
				OnStart(this);
			}
		}

		protected virtual void TransitionCompleted()
		{
			if (OnComplete != null)
			{
				OnComplete(this);
			}
		}

		public void SetProgressToStart()
		{
			SetProgress(0f);
		}

		public void SetProgressToEnd()
		{
			SetProgress(1f);
		}

		public void SetProgress(float progress)
		{
			try
			{
				Progress = Mathf.Max(0f, Mathf.Min(1f, progress));
				ProgressTweened = ValueFromProgress(0f, 1f);
				ProgressUpdated();
				if (OnUpdate != null)
				{
					OnUpdate(this);
				}
			}
			catch (Exception)
			{
				Stop();
			}
		}

		protected virtual void ProgressUpdated()
		{
		}

		protected float ValueFromProgressTweened(float start, float end)
		{
			return ProgressTweened * (end - start) + start;
		}

		protected float ValueFromProgress(float start, float end)
		{
			if (TweenType == TransitionHelper.TweenType.AnimationCurve)
			{
				return ValueFromProgressAnimationCurve(start, end);
			}
			if (_tweenFunction != null)
			{
				return _tweenFunction(start, end, Progress);
			}
			return end;
		}

		private float ValueFromProgressAnimationCurve(float start, float end)
		{
			float time = AnimationCurve.keys[0].time;
			float num = AnimationCurve.keys[AnimationCurve.keys.Length - 1].time - time;
			return start + (end - start) * AnimationCurve.Evaluate(time + num * Progress);
		}
	}
	public abstract class TransitionStepValue<T> : TransitionStep where T : struct
	{
		public T StartValue { get; set; }

		public T EndValue { get; set; }

		public T Value { get; set; }

		public T OriginalValue { get; set; }

		public TransitionStepValue(GameObject target = null, float delay = 0f, float duration = 0.5f, TransitionModeType transitionMode = TransitionModeType.Specified, TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime, TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear, AnimationCurve animationCurve = null, CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global, Action<TransitionStep> onStart = null, Action<TransitionStep> onUpdate = null, Action<TransitionStep> onComplete = null)
			: base(target, delay, duration, transitionMode, timeUpdateMethod, tweenType, animationCurve, coordinateSpace, onStart, onUpdate, onComplete)
		{
		}

		public virtual T GetCurrent()
		{
			return default(T);
		}

		public virtual void SetCurrent(T value)
		{
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.Components
{
	public abstract class TransitionBase : MonoBehaviour
	{
		public enum TransitionModeType
		{
			None,
			In,
			Out
		}

		[Serializable]
		public class TransitionSettings
		{
			[Tooltip("Whether the transition should auto run.\nFor in transitions this will happen when the gameobject is activated, for out transitions after the in transition is complete.")]
			public bool AutoRun;

			[Tooltip("Whether to automatically check and run transitions on child GameObjects.")]
			public bool TransitionChildren;

			[Tooltip("Whether this must be transitioned specifically. If not set it will run automatically when a parent transition is run that has the TransitionChildren property set.")]
			public bool MustTriggerDirect;

			[Tooltip("Time in seconds before this transition should be started.")]
			public float Delay;

			[Tooltip("How long this transition will / should run for.")]
			public float Duration = 0.3f;

			[Tooltip("What time source is used to update transitions")]
			public TransitionStep.TimeUpdateMethodType TimeUpdateMethod;

			[Tooltip("How the transition should be run.")]
			public TransitionHelper.TweenType TransitionType = TransitionHelper.TweenType.linear;

			[Tooltip("A custom curve to show how the transition should be run.")]
			public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

			[Tooltip("The transitions looping mode.")]
			public TransitionStep.LoopModeType LoopMode;

			[Tooltip("Methods that should be called when the transition is started.")]
			public TransitionStepEvent OnTransitionStart;

			[Tooltip("Methods that should be called when the transition progress is updated.")]
			public TransitionStepEvent OnTransitionUpdate;

			[Tooltip("Methods that should be called when the transition has completed.")]
			public TransitionStepEvent OnTransitionComplete;
		}

		[Serializable]
		public class TransitionStepEvent : UnityEvent<TransitionStep>
		{
		}

		[Tooltip("Whether to set up ready for transitioning in.")]
		public bool InitForTransitionIn = true;

		[Tooltip("Whether to automatically run the transition in effect in the OnEnable state.")]
		public bool AutoRun;

		[Tooltip("Whether to repeat initialisation and /or auto run in subsequent enabling of the gameitem.")]
		public bool RepeatWhenEnabled;

		public TransitionSettings TransitionInConfig;

		public TransitionSettings TransitionOutConfig;

		private bool _isInitialStateSet;

		public TransitionModeType TransitionMode { get; set; }

		public TransitionStep CurrentTransitionStep { get; set; }

		public virtual void OnEnable()
		{
			if (_isInitialStateSet && RepeatWhenEnabled)
			{
				Setup();
			}
		}

		public virtual void Start()
		{
			Setup();
		}

		private void Setup()
		{
			if (InitForTransitionIn || AutoRun)
			{
				InitTransitionIn();
			}
			if (AutoRun)
			{
				TransitionIn();
			}
		}

		public virtual void InitTransitionIn()
		{
			SetupInitialStateOnce();
			TransitionMode = TransitionModeType.In;
			CurrentTransitionStep = CreateTransitionStepIn();
			CurrentTransitionStep.SetProgressToStart();
		}

		public virtual void TransitionIn()
		{
			SetupInitialStateOnce();
			InitTransitionIn();
			CurrentTransitionStep.Start();
		}

		public virtual void InitTransitionOut()
		{
			SetupInitialStateOnce();
			TransitionMode = TransitionModeType.Out;
			CurrentTransitionStep = CreateTransitionStepOut();
			CurrentTransitionStep.SetProgressToStart();
		}

		public virtual void TransitionOut()
		{
			SetupInitialStateOnce();
			InitTransitionOut();
			CurrentTransitionStep.Start();
		}

		private void SetupInitialStateOnce()
		{
			if (!_isInitialStateSet)
			{
				_isInitialStateSet = true;
				SetupInitialState();
			}
		}

		public virtual void SetupInitialState()
		{
		}

		protected virtual void TransitionInStart(TransitionStep transitionStep)
		{
			if (TransitionInConfig.OnTransitionStart != null)
			{
				TransitionInConfig.OnTransitionStart.Invoke(transitionStep);
			}
		}

		protected virtual void TransitionOutStart(TransitionStep transitionStep)
		{
			if (TransitionOutConfig.OnTransitionStart != null)
			{
				TransitionOutConfig.OnTransitionStart.Invoke(transitionStep);
			}
		}

		protected virtual void TransitionInUpdate(TransitionStep transitionStep)
		{
			if (TransitionInConfig.OnTransitionUpdate != null)
			{
				TransitionInConfig.OnTransitionUpdate.Invoke(transitionStep);
			}
		}

		protected virtual void TransitionOutUpdate(TransitionStep transitionStep)
		{
			if (TransitionOutConfig.OnTransitionUpdate != null)
			{
				TransitionOutConfig.OnTransitionUpdate.Invoke(transitionStep);
			}
		}

		protected virtual void TransitionInComplete(TransitionStep transitionStep)
		{
			TransitionMode = TransitionModeType.Out;
			if (TransitionInConfig.OnTransitionComplete != null)
			{
				TransitionInConfig.OnTransitionComplete.Invoke(transitionStep);
			}
		}

		protected virtual void TransitionOutComplete(TransitionStep transitionStep)
		{
			if (TransitionOutConfig.OnTransitionComplete != null)
			{
				TransitionOutConfig.OnTransitionComplete.Invoke(transitionStep);
			}
		}

		public abstract TransitionStep CreateTransitionStep();

		public virtual TransitionStep CreateTransitionStepIn()
		{
			TransitionStep transitionStep = CurrentTransitionStep ?? CreateTransitionStep();
			SetupTransitionStepIn(transitionStep);
			return transitionStep;
		}

		public virtual void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			transitionStep.Delay = TransitionInConfig.Delay;
			transitionStep.Duration = TransitionInConfig.Duration;
			transitionStep.TimeUpdateMethod = TransitionInConfig.TimeUpdateMethod;
			transitionStep.TweenType = TransitionInConfig.TransitionType;
			transitionStep.AnimationCurve = TransitionInConfig.AnimationCurve;
			transitionStep.LoopMode = TransitionInConfig.LoopMode;
			transitionStep.OnStart = TransitionInStart;
			transitionStep.OnComplete = TransitionInComplete;
			transitionStep.OnUpdate = TransitionInUpdate;
		}

		public virtual TransitionStep CreateTransitionStepOut()
		{
			TransitionStep transitionStep = CurrentTransitionStep ?? CreateTransitionStep();
			SetupTransitionStepOut(transitionStep);
			return transitionStep;
		}

		public virtual void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			transitionStep.Delay = TransitionOutConfig.Delay;
			transitionStep.Duration = TransitionOutConfig.Duration;
			transitionStep.TimeUpdateMethod = TransitionOutConfig.TimeUpdateMethod;
			transitionStep.TweenType = TransitionOutConfig.TransitionType;
			transitionStep.AnimationCurve = TransitionOutConfig.AnimationCurve;
			transitionStep.LoopMode = TransitionOutConfig.LoopMode;
			transitionStep.OnStart = TransitionOutStart;
			transitionStep.OnComplete = TransitionOutComplete;
			transitionStep.OnUpdate = TransitionOutUpdate;
		}
	}
	[AddComponentMenu("Beautiful Transitions/Transition Manager")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionManager : MonoBehaviour
	{
		[Tooltip("The default transitions that will be used when transitioning to a new scene. If not specified then it is assumed that they are on the same gameobject as this component.")]
		public UnityEngine.GameObject[] DefaultSceneTransitions;

		public static TransitionManager Instance { get; private set; }

		public static bool IsActive => Instance != null;

		private void Awake()
		{
			if (Instance != null)
			{
				if (Instance != this)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			else
			{
				Instance = this;
			}
		}

		private void OnDestroy()
		{
			_ = Instance == this;
		}

		public void TransitionOutAndLoadScene(string sceneName)
		{
			if (DefaultSceneTransitions.Length == 0)
			{
				TransitionOutAndLoadScene(sceneName, base.gameObject);
			}
			else
			{
				TransitionOutAndLoadScene(sceneName, DefaultSceneTransitions);
			}
		}

		public void TransitionOutAndLoadScene(string sceneName, params UnityEngine.GameObject[] transitionGameObjects)
		{
			float delay = TransitionOut(transitionGameObjects);
			LoadSceneDelayed(sceneName, delay);
		}

		public void TransitionOut()
		{
			if (DefaultSceneTransitions.Length == 0)
			{
				TransitionOut(new UnityEngine.GameObject[1] { base.gameObject });
			}
			else
			{
				TransitionOut(DefaultSceneTransitions);
			}
		}

		public float TransitionOut(UnityEngine.GameObject[] transitionGameObjects)
		{
			List<TransitionBase> list = new List<TransitionBase>();
			foreach (UnityEngine.GameObject gameObject in transitionGameObjects)
			{
				list.AddRange(TransitionHelper.TransitionOut(gameObject));
			}
			return TransitionHelper.GetTransitionOutTime(list);
		}

		public void LoadSceneDelayed(string sceneName, float delay = 0f)
		{
			if (!Mathf.Approximately(delay, 0f))
			{
				StartCoroutine(LoadSceneDelayedCoroutine(sceneName, delay));
			}
			else
			{
				TransitionHelper.LoadScene(sceneName);
			}
		}

		private static IEnumerator LoadSceneDelayedCoroutine(string sceneName, float delay)
		{
			yield return new WaitForSeconds(delay);
			TransitionHelper.LoadScene(sceneName);
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.Components.Screen
{
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	[ExecuteInEditMode]
	[AddComponentMenu("Beautiful Transitions/Screen/Fade Screen Transition")]
	public class FadeScreen : TransitionScreenBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color = Color.black;

			[Tooltip("Skip running this if there is already a cross transition in progress. Useful for e.g. your entry scene where on first run you enter directly (running this transition), but might later cross transition to from another scene and so not want this transition to run.")]
			public bool SkipOnCrossTransition = true;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color = Color.black;

			[Tooltip("Whether and how to transition to a new scene.")]
			public TransitionStepScreen.SceneChangeModeType SceneChangeMode;

			[Tooltip("If transitioning to a new scene then the name of the scene to transition to.")]
			public string SceneToLoad;
		}

		[Header("Fade Specific")]
		public InSettings InConfig;

		public OutSettings OutConfig;

		public override TransitionStep CreateTransitionStep()
		{
			return new ScreenFade(base.gameObject);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is ScreenFade screenFade)
			{
				screenFade.Color = InConfig.Color;
				screenFade.Texture = InConfig.Texture;
				screenFade.SkipOnCrossTransition = InConfig.SkipOnCrossTransition;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is ScreenFade screenFade)
			{
				screenFade.Color = OutConfig.Color;
				screenFade.Texture = OutConfig.Texture;
				screenFade.SceneChangeMode = OutConfig.SceneChangeMode;
				screenFade.SceneToLoad = OutConfig.SceneToLoad;
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
	[AddComponentMenu("Beautiful Transitions/Screen/Wipe Screen Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	[ExecuteInEditMode]
	public class WipeScreen : TransitionScreenBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color = Color.white;

			[Tooltip("Gray scale wipe mask.")]
			public Texture2D MaskTexture;

			[Tooltip("Whether to invery the wipe mask.")]
			public bool InvertMask;

			[Tooltip("The amount of softness to apply to the wipe")]
			[Range(0f, 1f)]
			public float Softness;

			[Tooltip("Skip running this if there is already a cross transition in progress. Useful for e.g. your entry scene where on first run you enter directly (running this transition), but might later cross transition to from another scene and so not want this transition to run.")]
			public bool SkipOnCrossTransition = true;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color = Color.white;

			[Tooltip("Gray scale wipe mask. Look in the folder 'FlipWebApps\\BeautifulTransitions\\Textures' for sample mask textures you can drag and add here.")]
			public Texture2D MaskTexture;

			[Tooltip("Whether to invert the wipe mask.")]
			public bool InvertMask;

			[Tooltip("The amount of softness to apply to the wipe.")]
			[Range(0f, 1f)]
			public float Softness;

			[Tooltip("Whether and how to transition to a new scene.")]
			public TransitionStepScreen.SceneChangeModeType SceneChangeMode;

			[Tooltip("If transitioning to a new scene then the name of the scene to transition to.")]
			public string SceneToLoad;
		}

		[Header("Wipe Specific")]
		public InSettings InConfig;

		public OutSettings OutConfig;

		public override TransitionStep CreateTransitionStep()
		{
			return new ScreenWipe(base.gameObject, null);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is ScreenWipe screenWipe)
			{
				screenWipe.MaskTexture = InConfig.MaskTexture;
				screenWipe.InvertMask = InConfig.InvertMask;
				screenWipe.Color = InConfig.Color;
				screenWipe.Texture = InConfig.Texture;
				screenWipe.Softness = InConfig.Softness;
				screenWipe.SkipOnCrossTransition = InConfig.SkipOnCrossTransition;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is ScreenWipe screenWipe)
			{
				screenWipe.MaskTexture = OutConfig.MaskTexture;
				screenWipe.InvertMask = OutConfig.InvertMask;
				screenWipe.Color = OutConfig.Color;
				screenWipe.Texture = OutConfig.Texture;
				screenWipe.Softness = OutConfig.Softness;
				screenWipe.SceneChangeMode = OutConfig.SceneChangeMode;
				screenWipe.SceneToLoad = OutConfig.SceneToLoad;
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.Components.Screen.AbstractClasses
{
	[ExecuteInEditMode]
	public abstract class TransitionScreenBase : TransitionBase
	{
		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is TransitionStepFloat transitionStepFloat)
			{
				transitionStepFloat.StartValue = 1f;
				transitionStepFloat.EndValue = 0f;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is TransitionStepFloat transitionStepFloat)
			{
				transitionStepFloat.StartValue = 0f;
				transitionStepFloat.EndValue = 1f;
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.Components.GameObject
{
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Animate Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionAnimation : TransitionGameObjectBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("The Animator Speed.")]
			public float Speed = 1f;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("The Animator Speed.")]
			public float Speed = 1f;
		}

		[Header("Animation Specific")]
		public InSettings InConfig;

		public OutSettings OutConfig;

		public override TransitionStep CreateTransitionStep()
		{
			return new TriggerAnimation(Target);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is TriggerAnimation triggerAnimation)
			{
				triggerAnimation.Speed = InConfig.Speed;
				triggerAnimation.Trigger = "TransitionIn";
				triggerAnimation.TargetState = "TransitionIn";
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is TriggerAnimation triggerAnimation)
			{
				triggerAnimation.Speed = OutConfig.Speed;
				triggerAnimation.Trigger = "TransitionOut";
				triggerAnimation.TargetState = "TransitionOut";
			}
			base.SetupTransitionStepOut(transitionStep);
		}

		public override void InitTransitionIn()
		{
			base.InitTransitionIn();
			if (base.CurrentTransitionStep is TriggerAnimation triggerAnimation)
			{
				triggerAnimation.Animator.enabled = true;
				triggerAnimation.Animator.speed = 0f;
			}
		}

		public override void InitTransitionOut()
		{
			if (base.CurrentTransitionStep is TriggerAnimation triggerAnimation)
			{
				triggerAnimation.Animator.enabled = true;
				triggerAnimation.Animator.speed = 0f;
			}
			base.InitTransitionOut();
		}
	}
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Color Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionColor : TransitionGameObjectBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Gradient to use for the transition in. Note the end color will be overridden with the current color when the transition runs.")]
			public Gradient Gradient;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Gradient to use for the transition out. Note the start color will be overridden with the current color when the transition runs.")]
			public Gradient Gradient;
		}

		public InSettings InConfig;

		public OutSettings OutConfig;

		private Color _originalColor;

		public override void SetupInitialState()
		{
			_originalColor = ((ColorTransition)CreateTransitionStep()).OriginalValue;
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new ColorTransition(Target);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is ColorTransition colorTransition)
			{
				colorTransition.Gradient = InConfig.Gradient;
				colorTransition.EndValue = _originalColor;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is ColorTransition colorTransition)
			{
				colorTransition.Gradient = OutConfig.Gradient;
				colorTransition.StartValue = colorTransition.GetCurrent();
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Custom Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionCustom : TransitionGameObjectBase
	{
		public override TransitionStep CreateTransitionStep()
		{
			return new TransitionStep(Target);
		}
	}
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Fade Transition")]
	public class TransitionFade : TransitionGameObjectBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Normalised transparency at the start of the transition (ends at the GameObjects initial transparency).")]
			public float StartTransparency;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Normalised transparency at the end of the transition (starts at the GameObjects current transparency).")]
			public float EndTransparency;
		}

		[Header("Fade Specific")]
		public InSettings FadeInConfig;

		public OutSettings FadeOutConfig;

		private float _originalTransparency;

		public override void SetupInitialState()
		{
			_originalTransparency = ((Fade)CreateTransitionStep()).OriginalValue;
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new Fade(Target);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is Fade fade)
			{
				fade.StartValue = FadeInConfig.StartTransparency;
				fade.EndValue = _originalTransparency;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is Fade fade)
			{
				fade.StartValue = fade.GetCurrent();
				fade.EndValue = FadeOutConfig.EndTransparency;
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Move Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionMove : TransitionGameObjectBase
	{
		public enum MoveModeType
		{
			Global,
			Local,
			AnchoredPosition
		}

		public enum MoveType
		{
			FixedPosition,
			Delta
		}

		[Serializable]
		public class InSettings
		{
			[Tooltip("Movement type.")]
			public MoveType StartPositionType;

			[Tooltip("Starting position (end at the GameObjects initial position).")]
			public Vector3 StartPosition = new Vector3(0f, 0f, 0f);
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Movement type.")]
			public MoveType EndPositionType;

			[Tooltip("End position (end at the GameObjects current position).")]
			public Vector3 EndPosition = new Vector3(0f, 0f, 0f);
		}

		public MoveModeType MoveMode;

		public InSettings InConfig;

		public OutSettings OutConfig;

		private Vector3 _originalPosition;

		public override void SetupInitialState()
		{
			_originalPosition = ((Move)CreateTransitionStep()).OriginalValue;
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new Move(Target, null, null, 0f, 0.5f, TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, ConvertMoveMode());
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is Move move)
			{
				move.StartValue = ((InConfig.StartPositionType == MoveType.FixedPosition) ? InConfig.StartPosition : (_originalPosition + InConfig.StartPosition));
				move.EndValue = _originalPosition;
				move.CoordinateSpace = ConvertMoveMode();
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is Move move)
			{
				move.StartValue = move.GetCurrent();
				move.EndValue = ((OutConfig.EndPositionType == MoveType.FixedPosition) ? OutConfig.EndPosition : (_originalPosition + OutConfig.EndPosition));
				move.CoordinateSpace = ConvertMoveMode();
			}
			base.SetupTransitionStepOut(transitionStep);
		}

		private TransitionStep.CoordinateSpaceType ConvertMoveMode()
		{
			if (MoveMode == MoveModeType.Global)
			{
				return TransitionStep.CoordinateSpaceType.Global;
			}
			if (MoveMode == MoveModeType.Local)
			{
				return TransitionStep.CoordinateSpaceType.Local;
			}
			return TransitionStep.CoordinateSpaceType.AnchoredPosition;
		}
	}
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Move Target Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionMoveTraget : TransitionGameObjectBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("GameObject used as a starting position (end at the GameObjects initial position).")]
			public UnityEngine.GameObject StartTarget;

			[Tooltip("Whether to move in the X direction. Clear this to keep the gameobjects original X position.")]
			public bool MoveX = true;

			[Tooltip("Whether to move in the Y direction. Clear this to keep the gameobjects original Y position.")]
			public bool MoveY = true;

			[Tooltip("Whether to move in the Z direction. Clear this to keep the gameobjects original Z position.")]
			public bool MoveZ = true;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("GameObject used as the ending position (starts at the GameObjects current position).")]
			public UnityEngine.GameObject EndTarget;

			[Tooltip("Whether to move in the X direction. Clear this to keep the gameobjects original X position.")]
			public bool MoveX = true;

			[Tooltip("Whether to move in the Y direction. Clear this to keep the gameobjects original Y position.")]
			public bool MoveY = true;

			[Tooltip("Whether to move in the Z direction. Clear this to keep the gameobjects original Z position.")]
			public bool MoveZ = true;
		}

		public InSettings MoveInConfig;

		public OutSettings MoveOutConfig;

		private Vector3 _originalPosition;

		public override void SetupInitialState()
		{
			_originalPosition = ((Move)CreateTransitionStep()).OriginalValue;
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new Move(Target);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is Move move)
			{
				move.StartValue = new Vector3(MoveInConfig.MoveX ? MoveInConfig.StartTarget.transform.position.x : _originalPosition.x, MoveInConfig.MoveY ? MoveInConfig.StartTarget.transform.position.y : _originalPosition.y, MoveInConfig.MoveZ ? MoveInConfig.StartTarget.transform.position.z : _originalPosition.z);
				move.EndValue = _originalPosition;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is Move move)
			{
				move.StartValue = move.GetCurrent();
				move.EndValue = new Vector3(MoveOutConfig.MoveX ? MoveOutConfig.EndTarget.transform.position.x : move.GetCurrent().x, MoveOutConfig.MoveY ? MoveOutConfig.EndTarget.transform.position.y : move.GetCurrent().y, MoveOutConfig.MoveZ ? MoveOutConfig.EndTarget.transform.position.z : move.GetCurrent().z);
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Rotate Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionRotate : TransitionGameObjectBase
	{
		public enum RotationModeType
		{
			Global,
			Local
		}

		[Serializable]
		public class InSettings
		{
			[Tooltip("Start rotation (end at the GameObjects initial rotation).")]
			public Vector3 StartRotation = new Vector3(0f, 0f, 0f);
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("End rotation (starts at the GameObjects current position).")]
			public Vector3 EndRotation = new Vector3(0f, 0f, 0f);
		}

		public RotationModeType RotationMode = RotationModeType.Local;

		public InSettings InConfig;

		public OutSettings OutConfig;

		private Vector3 _originalRotation;

		public override void SetupInitialState()
		{
			_originalRotation = ((Rotate)CreateTransitionStep()).OriginalValue;
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new Rotate(Target, null, null, 0f, 0.5f, TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, ConvertRotationMode());
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is Rotate rotate)
			{
				rotate.StartValue = InConfig.StartRotation;
				rotate.EndValue = _originalRotation;
				rotate.CoordinateSpace = ConvertRotationMode();
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is Rotate rotate)
			{
				rotate.StartValue = rotate.GetCurrent();
				rotate.EndValue = OutConfig.EndRotation;
				rotate.CoordinateSpace = ConvertRotationMode();
			}
			base.SetupTransitionStepOut(transitionStep);
		}

		private TransitionStep.CoordinateSpaceType ConvertRotationMode()
		{
			if (RotationMode == RotationModeType.Global)
			{
				return TransitionStep.CoordinateSpaceType.Global;
			}
			return TransitionStep.CoordinateSpaceType.Local;
		}
	}
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Scale Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionScale : TransitionGameObjectBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Start scale (end at the GameObjects initial scale).")]
			public Vector3 StartScale = new Vector3(0f, 0f, 0f);
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("End scale (starts at the GameObjects current scale).")]
			public Vector3 EndScale = new Vector3(0f, 0f, 0f);
		}

		public InSettings InConfig;

		public OutSettings OutConfig;

		private Vector3 _originalScale;

		public override void SetupInitialState()
		{
			_originalScale = ((Scale)CreateTransitionStep()).OriginalValue;
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new Scale(Target);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is Scale scale)
			{
				scale.StartValue = InConfig.StartScale;
				scale.EndValue = _originalScale;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is Scale scale)
			{
				scale.StartValue = scale.GetCurrent();
				scale.EndValue = OutConfig.EndScale;
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
	[AddComponentMenu("Beautiful Transitions/GameObject + UI/Volume Transition")]
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	public class TransitionVolume : TransitionGameObjectBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Normalised volume at the start of the transition (ends at the GameObjects initial volume).")]
			public float StartVolume;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Normalised volume at the end of the transition (starts at the GameObjects current volume).")]
			public float EndVolume;
		}

		[Header("Volume Specific")]
		public InSettings InConfig;

		public OutSettings OutConfig;

		private float _originalVolume;

		public override void SetupInitialState()
		{
			_originalVolume = ((VolumeTransition)CreateTransitionStep()).OriginalValue;
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new VolumeTransition(Target);
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is VolumeTransition volumeTransition)
			{
				volumeTransition.StartValue = InConfig.StartVolume;
				volumeTransition.EndValue = _originalVolume;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is VolumeTransition volumeTransition)
			{
				volumeTransition.StartValue = volumeTransition.GetCurrent();
				volumeTransition.EndValue = OutConfig.EndVolume;
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.Components.GameObject.AbstractClasses
{
	public abstract class TransitionGameObjectBase : TransitionBase
	{
		[Tooltip("The target gameobject upon which to perform the transition. If not specified then the transition will typically operate upon the current gameobject.")]
		public UnityEngine.GameObject Target;

		public void Awake()
		{
			if (Target == null)
			{
				Target = base.gameObject;
			}
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.Components.Camera
{
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	[ExecuteInEditMode]
	[AddComponentMenu("Beautiful Transitions/Camera/Fade Camera Transition")]
	public class FadeCamera : TransitionCameraBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color;
		}

		[Header("Fade Specific")]
		public InSettings InConfig;

		public OutSettings OutConfig;

		private Material _material;

		public void Awake()
		{
			Shader shader = Shader.Find("Hidden/FlipWebApps/BeautifulTransitions/FadeCamera");
			if (shader != null && shader.isSupported)
			{
				_material = new Material(shader);
			}
			else
			{
				UnityEngine.Debug.Log("FadeCamera: Shader is not found or supported on this platform.");
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (base.CurrentTransitionStep is TransitionStepFloat transitionStepFloat && _material != null && !Mathf.Approximately(transitionStepFloat.Value, 0f))
			{
				if (CrossTransitionTarget != null)
				{
					_material.SetTexture("_OverlayTex", CrossTransitionRenderTexture);
				}
				else
				{
					_material.SetTexture("_OverlayTex", (base.TransitionMode == TransitionModeType.In) ? InConfig.Texture : OutConfig.Texture);
				}
				_material.SetColor("_Color", (base.TransitionMode == TransitionModeType.In) ? InConfig.Color : OutConfig.Color);
				_material.SetFloat("_Amount", transitionStepFloat.Value);
				Graphics.Blit(source, destination, _material);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}
	}
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	[AddComponentMenu("Beautiful Transitions/Camera/Wipe Camera Transition")]
	[ExecuteInEditMode]
	public class WipeCamera : TransitionCameraBase
	{
		[Serializable]
		public class InSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color = Color.white;

			[Tooltip("Gray scale wipe mask. Look in the folder 'FlipWebApps\\BeautifulTransitions\\Textures' for sample mask textures you can drag and add here.")]
			public Texture2D MaskTexture;

			[Tooltip("Whether to invery the wipe mask.")]
			public bool InvertMask;

			[Tooltip("The amount of softness to apply to the wipe.")]
			[Range(0f, 1f)]
			public float Softness;
		}

		[Serializable]
		public class OutSettings
		{
			[Tooltip("Optional overlay texture to use.")]
			public Texture2D Texture;

			[Tooltip("Tint color.")]
			public Color Color = Color.white;

			[Tooltip("Gray scale wipe mask.")]
			public Texture2D MaskTexture;

			[Tooltip("Whether to invery the wipe mask.")]
			public bool InvertMask;

			[Range(0f, 1f)]
			[Tooltip("The amount of softness to apply to the wipe.")]
			public float Softness;
		}

		[Header("Wipe Specific")]
		public InSettings InConfig;

		public OutSettings OutConfig;

		private Material _material;

		public void Awake()
		{
			Shader shader = Shader.Find("Hidden/FlipWebApps/BeautifulTransitions/WipeCamera");
			if (shader != null && shader.isSupported)
			{
				_material = new Material(shader);
			}
			else
			{
				UnityEngine.Debug.Log("WipeCamera: Shader is not found or supported on this platform.");
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (base.CurrentTransitionStep is TransitionStepFloat transitionStepFloat && _material != null && !Mathf.Approximately(transitionStepFloat.Value, 0f))
			{
				if (CrossTransitionTarget != null)
				{
					_material.SetTexture("_OverlayTex", CrossTransitionRenderTexture);
				}
				else
				{
					_material.SetTexture("_OverlayTex", (base.TransitionMode == TransitionModeType.In) ? InConfig.Texture : OutConfig.Texture);
				}
				_material.SetColor("_Color", (base.TransitionMode == TransitionModeType.In) ? InConfig.Color : OutConfig.Color);
				_material.SetTexture("_MaskTex", (base.TransitionMode == TransitionModeType.In) ? InConfig.MaskTexture : OutConfig.MaskTexture);
				_material.SetFloat("_Amount", transitionStepFloat.Value);
				if ((base.TransitionMode == TransitionModeType.In) ? InConfig.InvertMask : OutConfig.InvertMask)
				{
					_material.EnableKeyword("INVERT_MASK");
				}
				else
				{
					_material.DisableKeyword("INVERT_MASK");
				}
				_material.SetFloat("_Softness", (base.TransitionMode == TransitionModeType.In) ? InConfig.Softness : OutConfig.Softness);
				Graphics.Blit(source, destination, _material);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}
	}
}
namespace BeautifulTransitions.Scripts.Transitions.Components.Camera.AbstractClasses
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(UnityEngine.Camera))]
	public abstract class TransitionCameraBase : TransitionBase
	{
		public bool SkipIdleRendering;

		protected RenderTexture CrossTransitionRenderTexture;

		protected UnityEngine.Camera CrossTransitionTarget;

		public void CrossTransition(UnityEngine.Camera target)
		{
			CrossTransitionTarget = target;
			TransitionOut();
		}

		protected override void TransitionOutStart(TransitionStep transitionStep)
		{
			if (CrossTransitionTarget != null)
			{
				CrossTransitionRenderTexture = new RenderTexture(UnityEngine.Screen.width, UnityEngine.Screen.height, 24);
				CrossTransitionTarget.gameObject.SetActive(value: true);
				CrossTransitionTarget.targetTexture = CrossTransitionRenderTexture;
			}
			base.TransitionOutStart(transitionStep);
		}

		protected override void TransitionOutComplete(TransitionStep transitionStep)
		{
			if (CrossTransitionTarget != null)
			{
				CrossTransitionTarget.gameObject.SetActive(value: false);
				CrossTransitionTarget.targetTexture = null;
				UnityEngine.Object.Destroy(CrossTransitionRenderTexture);
				CrossTransitionRenderTexture = null;
				base.transform.position = CrossTransitionTarget.transform.position;
				base.transform.rotation = CrossTransitionTarget.transform.rotation;
				base.transform.localScale = CrossTransitionTarget.transform.localScale;
				CrossTransitionTarget = null;
				if (base.CurrentTransitionStep is TransitionStepFloat transitionStepFloat)
				{
					transitionStepFloat.Value = 0f;
				}
			}
			base.TransitionOutComplete(transitionStep);
		}

		public override TransitionStep CreateTransitionStep()
		{
			return new TransitionStepFloat();
		}

		public override void SetupTransitionStepIn(TransitionStep transitionStep)
		{
			if (transitionStep is TransitionStepFloat transitionStepFloat)
			{
				transitionStepFloat.StartValue = 1f;
				transitionStepFloat.EndValue = 0f;
			}
			base.SetupTransitionStepIn(transitionStep);
		}

		public override void SetupTransitionStepOut(TransitionStep transitionStep)
		{
			if (transitionStep is TransitionStepFloat transitionStepFloat)
			{
				transitionStepFloat.StartValue = 0f;
				transitionStepFloat.EndValue = 1f;
			}
			base.SetupTransitionStepOut(transitionStep);
		}
	}
}
namespace BeautifulTransitions.Scripts.Shake
{
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/shake/")]
	public class ShakeHelper
	{
		private static readonly List<int> ActiveShakes = new List<int>(2);

		public static void Shake(MonoBehaviour caller, Transform transform, float duration, Vector3 range, float decayStart = 1f)
		{
			caller.StartCoroutine(ShakeCoroutine(transform, duration, range, decayStart));
		}

		private static IEnumerator ShakeCoroutine(Transform transform, float duration, Vector3 range, float decayStart = 1f)
		{
			if (ActiveShakes.Contains(transform.GetInstanceID()))
			{
				yield break;
			}
			ActiveShakes.Add(transform.GetInstanceID());
			Vector3 originalPosition = transform.localPosition;
			int randomAngle = UnityEngine.Random.Range(0, 361);
			float decay = 0f;
			float decayFactor = (Mathf.Approximately(decayStart, 1f) ? 0f : (1f / (1f - decayStart)));
			for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
			{
				if (transform.gameObject != null)
				{
					float num = elapsedTime / duration;
					if (num >= decayStart)
					{
						decay = 1f - decayFactor + decayFactor * num;
					}
					randomAngle += 180 + UnityEngine.Random.Range(-60, 60);
					float num2 = Mathf.Sin(randomAngle);
					float num3 = Mathf.Cos(randomAngle);
					Vector3 vector = new Vector3(num3 * num2 * range.x, num2 * num2 * range.y, num3 * range.z);
					Vector3 localPosition = originalPosition + vector * (1f - decay);
					transform.localPosition = localPosition;
				}
				yield return null;
			}
			transform.localPosition = originalPosition;
			ActiveShakes.Remove(transform.GetInstanceID());
		}
	}
}
namespace BeautifulTransitions.Scripts.Shake.Components
{
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/shake/")]
	[AddComponentMenu("Beautiful Transitions/Shake/Shake Camera")]
	public class ShakeCamera : MonoBehaviour
	{
		[Tooltip("A list of cameras to shake. If left empty cameras on the same gameobject as the component will be used, or if none are found the main camera.")]
		public List<Camera> Cameras;

		[Tooltip("The duration to shake the camera for.")]
		public float Duration = 1f;

		[Tooltip("The offset relative to duration after which to start decaying (slowing down) the movement in the range 0 to 1.")]
		[Range(0f, 1f)]
		public float DecayStart = 0.75f;

		[Tooltip("The shake movement range from the origin. Set any dimension to 0 to stop movement along that axis.")]
		public Vector3 Range = Vector3.one;

		public static ShakeCamera Instance { get; private set; }

		public static bool IsActive => Instance != null;

		private void Awake()
		{
			if (Instance != null)
			{
				if (Instance != this)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			else
			{
				Instance = this;
				Setup();
			}
		}

		private void OnDestroy()
		{
			_ = Instance == this;
		}

		private void Setup()
		{
			if (Cameras.Count < 1)
			{
				if ((bool)GetComponent<Camera>())
				{
					Cameras.Add(GetComponent<Camera>());
				}
				if (Cameras.Count < 1 && (bool)Camera.main)
				{
					Cameras.Add(Camera.main);
				}
			}
		}

		public void Shake()
		{
			Shake(Duration, Range, DecayStart);
		}

		public void Shake(float duration, Vector3 range, float decayStart)
		{
			foreach (Camera camera in Cameras)
			{
				ShakeHelper.Shake(this, camera.transform, duration, range, decayStart);
			}
		}
	}
}
namespace BeautifulTransitions.Scripts.Helper
{
	public class SmoothRandom
	{
		private static FractalNoise s_Noise;

		public static Vector3 GetVector3(float speed)
		{
			float x = Time.time * 0.01f * speed;
			return new Vector3(Get().HybridMultifractal(x, 15.73f, 0.58f), Get().HybridMultifractal(x, 63.94f, 0.58f), Get().HybridMultifractal(x, 0.2f, 0.58f));
		}

		public static Vector2 GetVector2(float speed)
		{
			float x = Time.time * 0.01f * speed;
			return new Vector2(Get().HybridMultifractal(x, 15.73f, 0.58f), Get().HybridMultifractal(x, 63.94f, 0.58f));
		}

		public static float Get(float speed)
		{
			float num = Time.time * 0.01f * speed;
			return Get().HybridMultifractal(num * 0.01f, 15.7f, 0.65f);
		}

		private static FractalNoise Get()
		{
			if (s_Noise == null)
			{
				s_Noise = new FractalNoise(1.27f, 2.04f, 8.36f);
			}
			return s_Noise;
		}
	}
	public class Perlin
	{
		private const int B = 256;

		private const int BM = 255;

		private const int N = 4096;

		private int[] p = new int[514];

		private float[,] g3 = new float[514, 3];

		private float[,] g2 = new float[514, 2];

		private float[] g1 = new float[514];

		private float s_curve(float t)
		{
			return t * t * (3f - 2f * t);
		}

		private float lerp(float t, float a, float b)
		{
			return a + t * (b - a);
		}

		private void setup(float value, out int b0, out int b1, out float r0, out float r1)
		{
			float num = value + 4096f;
			b0 = (int)num & 0xFF;
			b1 = (b0 + 1) & 0xFF;
			r0 = num - (float)(int)num;
			r1 = r0 - 1f;
		}

		private float at2(float rx, float ry, float x, float y)
		{
			return rx * x + ry * y;
		}

		private float at3(float rx, float ry, float rz, float x, float y, float z)
		{
			return rx * x + ry * y + rz * z;
		}

		public float Noise(float arg)
		{
			setup(arg, out var b, out var b2, out var r, out var r2);
			float t = s_curve(r);
			float a = r * g1[p[b]];
			float b3 = r2 * g1[p[b2]];
			return lerp(t, a, b3);
		}

		public float Noise(float x, float y)
		{
			setup(x, out var b, out var b2, out var r, out var r2);
			setup(y, out var b3, out var b4, out var r3, out var r4);
			int num = p[b];
			int num2 = p[b2];
			int num3 = p[num + b3];
			int num4 = p[num2 + b3];
			int num5 = p[num + b4];
			int num6 = p[num2 + b4];
			float t = s_curve(r);
			float t2 = s_curve(r3);
			float a = at2(r, r3, g2[num3, 0], g2[num3, 1]);
			float b5 = at2(r2, r3, g2[num4, 0], g2[num4, 1]);
			float a2 = lerp(t, a, b5);
			a = at2(r, r4, g2[num5, 0], g2[num5, 1]);
			b5 = at2(r2, r4, g2[num6, 0], g2[num6, 1]);
			float b6 = lerp(t, a, b5);
			return lerp(t2, a2, b6);
		}

		public float Noise(float x, float y, float z)
		{
			setup(x, out var b, out var b2, out var r, out var r2);
			setup(y, out var b3, out var b4, out var r3, out var r4);
			setup(z, out var b5, out var b6, out var r5, out var r6);
			int num = p[b];
			int num2 = p[b2];
			int num3 = p[num + b3];
			int num4 = p[num2 + b3];
			int num5 = p[num + b4];
			int num6 = p[num2 + b4];
			float t = s_curve(r);
			float t2 = s_curve(r3);
			float t3 = s_curve(r5);
			float a = at3(r, r3, r5, g3[num3 + b5, 0], g3[num3 + b5, 1], g3[num3 + b5, 2]);
			float b7 = at3(r2, r3, r5, g3[num4 + b5, 0], g3[num4 + b5, 1], g3[num4 + b5, 2]);
			float a2 = lerp(t, a, b7);
			a = at3(r, r4, r5, g3[num5 + b5, 0], g3[num5 + b5, 1], g3[num5 + b5, 2]);
			b7 = at3(r2, r4, r5, g3[num6 + b5, 0], g3[num6 + b5, 1], g3[num6 + b5, 2]);
			float b8 = lerp(t, a, b7);
			float a3 = lerp(t2, a2, b8);
			a = at3(r, r3, r6, g3[num3 + b6, 0], g3[num3 + b6, 2], g3[num3 + b6, 2]);
			b7 = at3(r2, r3, r6, g3[num4 + b6, 0], g3[num4 + b6, 1], g3[num4 + b6, 2]);
			a2 = lerp(t, a, b7);
			a = at3(r, r4, r6, g3[num5 + b6, 0], g3[num5 + b6, 1], g3[num5 + b6, 2]);
			b7 = at3(r2, r4, r6, g3[num6 + b6, 0], g3[num6 + b6, 1], g3[num6 + b6, 2]);
			b8 = lerp(t, a, b7);
			float b9 = lerp(t2, a2, b8);
			return lerp(t3, a3, b9);
		}

		private void normalize2(ref float x, ref float y)
		{
			float num = (float)Math.Sqrt(x * x + y * y);
			x = y / num;
			y /= num;
		}

		private void normalize3(ref float x, ref float y, ref float z)
		{
			float num = (float)Math.Sqrt(x * x + y * y + z * z);
			x = y / num;
			y /= num;
			z /= num;
		}

		public Perlin()
		{
			System.Random random = new System.Random();
			int i;
			for (i = 0; i < 256; i++)
			{
				p[i] = i;
				g1[i] = (float)(random.Next(512) - 256) / 256f;
				for (int j = 0; j < 2; j++)
				{
					g2[i, j] = (float)(random.Next(512) - 256) / 256f;
				}
				normalize2(ref g2[i, 0], ref g2[i, 1]);
				for (int j = 0; j < 3; j++)
				{
					g3[i, j] = (float)(random.Next(512) - 256) / 256f;
				}
				normalize3(ref g3[i, 0], ref g3[i, 1], ref g3[i, 2]);
			}
			while (--i != 0)
			{
				int num = p[i];
				int j;
				p[i] = p[j = random.Next(256)];
				p[j] = num;
			}
			for (i = 0; i < 258; i++)
			{
				p[256 + i] = p[i];
				g1[256 + i] = g1[i];
				for (int j = 0; j < 2; j++)
				{
					g2[256 + i, j] = g2[i, j];
				}
				for (int j = 0; j < 3; j++)
				{
					g3[256 + i, j] = g3[i, j];
				}
			}
		}
	}
	public class FractalNoise
	{
		private Perlin m_Noise;

		private float[] m_Exponent;

		private int m_IntOctaves;

		private float m_Octaves;

		private float m_Lacunarity;

		public FractalNoise(float inH, float inLacunarity, float inOctaves)
			: this(inH, inLacunarity, inOctaves, null)
		{
		}

		public FractalNoise(float inH, float inLacunarity, float inOctaves, Perlin noise)
		{
			m_Lacunarity = inLacunarity;
			m_Octaves = inOctaves;
			m_IntOctaves = (int)inOctaves;
			m_Exponent = new float[m_IntOctaves + 1];
			float num = 1f;
			for (int i = 0; i < m_IntOctaves + 1; i++)
			{
				m_Exponent[i] = (float)Math.Pow(m_Lacunarity, 0f - inH);
				num *= m_Lacunarity;
			}
			if (noise == null)
			{
				m_Noise = new Perlin();
			}
			else
			{
				m_Noise = noise;
			}
		}

		public float HybridMultifractal(float x, float y, float offset)
		{
			float num = (m_Noise.Noise(x, y) + offset) * m_Exponent[0];
			float num2 = num;
			x *= m_Lacunarity;
			y *= m_Lacunarity;
			int i;
			for (i = 1; i < m_IntOctaves; i++)
			{
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				float num3 = (m_Noise.Noise(x, y) + offset) * m_Exponent[i];
				num += num2 * num3;
				num2 *= num3;
				x *= m_Lacunarity;
				y *= m_Lacunarity;
			}
			float num4 = m_Octaves - (float)m_IntOctaves;
			return num + num4 * m_Noise.Noise(x, y) * m_Exponent[i];
		}

		public float RidgedMultifractal(float x, float y, float offset, float gain)
		{
			float num = Mathf.Abs(m_Noise.Noise(x, y));
			num = offset - num;
			num *= num;
			float num2 = num;
			float num3 = 1f;
			for (int i = 1; i < m_IntOctaves; i++)
			{
				x *= m_Lacunarity;
				y *= m_Lacunarity;
				num3 = num * gain;
				num3 = Mathf.Clamp01(num3);
				num = Mathf.Abs(m_Noise.Noise(x, y));
				num = offset - num;
				num *= num;
				num *= num3;
				num2 += num * m_Exponent[i];
			}
			return num2;
		}

		public float BrownianMotion(float x, float y)
		{
			float num = 0f;
			long num2;
			for (num2 = 0L; num2 < m_IntOctaves; num2++)
			{
				num = m_Noise.Noise(x, y) * m_Exponent[num2];
				x *= m_Lacunarity;
				y *= m_Lacunarity;
			}
			float num3 = m_Octaves - (float)m_IntOctaves;
			return num + num3 * m_Noise.Noise(x, y) * m_Exponent[num2];
		}
	}
	public class TweenMethods : MonoBehaviour
	{
		public delegate float TweenFunction(float start, float end, float value);

		public static float linear(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value);
		}

		public static float clerp(float start, float end, float value)
		{
			float num = 0f;
			float num2 = 360f;
			float num3 = Mathf.Abs((num2 - num) * 0.5f);
			float num4 = 0f;
			float num5 = 0f;
			if (end - start < 0f - num3)
			{
				num5 = (num2 - start + end) * value;
				return start + num5;
			}
			if (end - start > num3)
			{
				num5 = (0f - (num2 - end + start)) * value;
				return start + num5;
			}
			return start + (end - start) * value;
		}

		public static float spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * (float)Math.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
			return start + (end - start) * value;
		}

		public static float easeInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}

		public static float easeOutQuad(float start, float end, float value)
		{
			end -= start;
			return (0f - end) * value * (value - 2f) + start;
		}

		public static float easeInOutQuad(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value + start;
			}
			value -= 1f;
			return (0f - end) * 0.5f * (value * (value - 2f) - 1f) + start;
		}

		public static float easeInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}

		public static float easeOutCubic(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value + 1f) + start;
		}

		public static float easeInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value + start;
			}
			value -= 2f;
			return end * 0.5f * (value * value * value + 2f) + start;
		}

		public static float easeInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}

		public static float easeOutQuart(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return (0f - end) * (value * value * value * value - 1f) + start;
		}

		public static float easeInOutQuart(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value * value + start;
			}
			value -= 2f;
			return (0f - end) * 0.5f * (value * value * value * value - 2f) + start;
		}

		public static float easeInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}

		public static float easeOutQuint(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value * value * value + 1f) + start;
		}

		public static float easeInOutQuint(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value * value * value + start;
			}
			value -= 2f;
			return end * 0.5f * (value * value * value * value * value + 2f) + start;
		}

		public static float easeInSine(float start, float end, float value)
		{
			end -= start;
			return (0f - end) * Mathf.Cos(value * ((float)Math.PI / 2f)) + end + start;
		}

		public static float easeOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value * ((float)Math.PI / 2f)) + start;
		}

		public static float easeInOutSine(float start, float end, float value)
		{
			end -= start;
			return (0f - end) * 0.5f * (Mathf.Cos((float)Math.PI * value) - 1f) + start;
		}

		public static float easeInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2f, 10f * (value - 1f)) + start;
		}

		public static float easeOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (0f - Mathf.Pow(2f, -10f * value) + 1f) + start;
		}

		public static float easeInOutExpo(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
			}
			value -= 1f;
			return end * 0.5f * (0f - Mathf.Pow(2f, -10f * value) + 2f) + start;
		}

		public static float easeInCirc(float start, float end, float value)
		{
			end -= start;
			return (0f - end) * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}

		public static float easeOutCirc(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * Mathf.Sqrt(1f - value * value) + start;
		}

		public static float easeInOutCirc(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return (0f - end) * 0.5f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
			}
			value -= 2f;
			return end * 0.5f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
		}

		public static float easeInBounce(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			return end - easeOutBounce(0f, end, num - value) + start;
		}

		public static float easeOutBounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return end * (7.5625f * value * value) + start;
			}
			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return end * (7.5625f * value * value + 0.75f) + start;
			}
			if ((double)value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return end * (7.5625f * value * value + 0.9375f) + start;
			}
			value -= 21f / 22f;
			return end * (7.5625f * value * value + 63f / 64f) + start;
		}

		public static float easeInOutBounce(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			if (value < num * 0.5f)
			{
				return easeInBounce(0f, end, value * 2f) * 0.5f + start;
			}
			return easeOutBounce(0f, end, value * 2f - num) * 0.5f + end * 0.5f + start;
		}

		public static float easeInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1f;
			float num = 1.70158f;
			return end * value * value * ((num + 1f) * value - num) + start;
		}

		public static float easeOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value -= 1f;
			return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
		}

		public static float easeInOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return end * 0.5f * (value * value * ((num + 1f) * value - num)) + start;
			}
			value -= 2f;
			num *= 1.525f;
			return end * 0.5f * (value * value * ((num + 1f) * value + num) + 2f) + start;
		}

		public static float punch(float amplitude, float value)
		{
			float num = 9f;
			if (value == 0f)
			{
				return 0f;
			}
			if (value == 1f)
			{
				return 0f;
			}
			float num2 = 0.3f;
			num = num2 / ((float)Math.PI * 2f) * Mathf.Asin(0f);
			return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num) * ((float)Math.PI * 2f) / num2);
		}

		public static float easeInElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			if (num4 == 0f || num4 < Mathf.Abs(end))
			{
				num4 = end;
				num3 = num2 / 4f;
			}
			else
			{
				num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
			}
			return 0f - num4 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num3) * ((float)Math.PI * 2f) / num2) + start;
		}

		public static float easeOutElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			if (num4 == 0f || num4 < Mathf.Abs(end))
			{
				num4 = end;
				num3 = num2 * 0.25f;
			}
			else
			{
				num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
			}
			return num4 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num3) * ((float)Math.PI * 2f) / num2) + end + start;
		}

		public static float easeInOutElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num * 0.5f) == 2f)
			{
				return start + end;
			}
			if (num4 == 0f || num4 < Mathf.Abs(end))
			{
				num4 = end;
				num3 = num2 / 4f;
			}
			else
			{
				num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
			}
			if (value < 1f)
			{
				return -0.5f * (num4 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num3) * ((float)Math.PI * 2f) / num2)) + start;
			}
			return num4 * Mathf.Pow(2f, -10f * (value -= 1f)) * Mathf.Sin((value * num - num3) * ((float)Math.PI * 2f) / num2) * 0.5f + end + start;
		}
	}
}
namespace BeautifulTransitions.Scripts.DisplayItem
{
	[HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
	internal class DisplayItemHelper
	{
		public static void SyncActiveStateAnimated(GameObject gameObject)
		{
			gameObject.GetComponent<Animator>().SetBool("Active", gameObject.activeSelf);
		}

		public static void SetAttention(GameObject gameObject, bool attention)
		{
			gameObject.GetComponent<Animator>().SetBool("Attention", attention);
		}

		public static void SetActiveAnimated(MonoBehaviour caller, GameObject gameObject, bool value)
		{
			caller.StartCoroutine(SetActiveAnimatedCoroutine(gameObject, value));
		}

		public static IEnumerator SetActiveAnimatedCoroutine(GameObject gameObject, bool value)
		{
			Animator animator = gameObject.GetComponent<Animator>();
			if (value)
			{
				gameObject.SetActive(value: true);
				animator.Play("NotActive");
				animator.SetBool("Active", value: true);
				yield break;
			}
			animator.SetBool("Active", value: false);
			bool closedStateReached = false;
			while (!closedStateReached)
			{
				if (!animator.IsInTransition(0))
				{
					closedStateReached = animator.GetCurrentAnimatorStateInfo(0).IsName("NotActive");
				}
				yield return new WaitForEndOfFrame();
			}
			gameObject.SetActive(value: false);
		}
	}
}
namespace BeautifulTransitions._Demo.Transitions.Scripts
{
	public class ScreenCameraController : MonoBehaviour
	{
		public FadeCamera FadeCamera;

		public WipeCamera WipeCamera;

		public FadeScreen FadeScreen;

		public WipeScreen WipeScreen;

		public Texture2D OverlayTexture;

		public Texture2D[] WipeTextures;

		private Color _color = Color.white;

		private bool _showTexture = true;

		private int _effect;

		private float _softness;

		public void SetColorWhite()
		{
			SetColor(Color.white);
		}

		public void SetColorRed()
		{
			SetColor(Color.red);
		}

		public void SetColorBlue()
		{
			SetColor(Color.blue);
		}

		public void SetColorGreen()
		{
			SetColor(Color.green);
		}

		public void SetColorBlack()
		{
			SetColor(Color.black);
		}

		private void SetColor(Color color)
		{
			_color = color;
			FadeCamera.InConfig.Color = _color;
			FadeCamera.OutConfig.Color = _color;
			WipeCamera.InConfig.Color = _color;
			WipeCamera.OutConfig.Color = _color;
			FadeScreen.InConfig.Color = _color;
			FadeScreen.OutConfig.Color = _color;
			WipeScreen.InConfig.Color = _color;
			WipeScreen.OutConfig.Color = _color;
		}

		public void SetEffect(int effect)
		{
			_effect = effect;
		}

		public void SetSoftness(float softness)
		{
			_softness = softness;
			WipeCamera.InConfig.Softness = _softness;
			WipeCamera.OutConfig.Softness = _softness;
			WipeScreen.InConfig.Softness = _softness;
			WipeScreen.OutConfig.Softness = _softness;
		}

		public void SetShowTexture(bool showTexture)
		{
			_showTexture = showTexture;
			Texture2D texture = (_showTexture ? OverlayTexture : null);
			FadeCamera.InConfig.Texture = texture;
			FadeCamera.OutConfig.Texture = texture;
			WipeCamera.InConfig.Texture = texture;
			WipeCamera.OutConfig.Texture = texture;
			FadeScreen.InConfig.Texture = texture;
			FadeScreen.OutConfig.Texture = texture;
			WipeScreen.InConfig.Texture = texture;
			WipeScreen.OutConfig.Texture = texture;
		}

		public void SetWipeTexture()
		{
			if (_effect >= 1 && _effect - 1 <= WipeTextures.Length)
			{
				Texture2D maskTexture = WipeTextures[_effect - 1];
				WipeCamera.InConfig.MaskTexture = maskTexture;
				WipeCamera.OutConfig.MaskTexture = maskTexture;
				WipeScreen.InConfig.MaskTexture = maskTexture;
				WipeScreen.OutConfig.MaskTexture = maskTexture;
			}
		}

		public void DemoScreen()
		{
			SetColor(_color);
			SetShowTexture(_showTexture);
			if (_effect == 0)
			{
				StartCoroutine(DemoCameraInternal(FadeScreen));
			}
			else
			{
				StartCoroutine(DemoCameraInternal(WipeScreen));
			}
		}

		public void DemoCamera()
		{
			if (_effect == 0)
			{
				FadeCamera.enabled = true;
				WipeCamera.enabled = false;
				StartCoroutine(DemoCameraInternal(FadeCamera));
			}
			else
			{
				FadeCamera.enabled = false;
				WipeCamera.enabled = true;
				StartCoroutine(DemoCameraInternal(WipeCamera));
			}
		}

		public IEnumerator DemoCameraInternal(TransitionBase transitionBase)
		{
			SetColor(_color);
			SetShowTexture(_showTexture);
			SetWipeTexture();
			SetSoftness(_softness);
			float transitionOutTime = TransitionHelper.GetTransitionOutTime(new List<TransitionBase> { transitionBase });
			transitionBase.InitTransitionOut();
			transitionBase.TransitionOut();
			yield return new WaitForSeconds(transitionOutTime + 0.5f);
			transitionBase.TransitionIn();
		}

		public void ShowRatePage()
		{
			Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
		}
	}
	public class ScriptingDemo : MonoBehaviour
	{
		public GameObject TestGameObject;

		public GameObject TestGameObject2;

		public GameObject TestGameObject3;

		public Text Counter;

		public GameObject TestGameObject5;

		public Text Description;

		private void Start()
		{
			ShowTransitionedDescription("Basic linked transitions  with events");
			Vector3 value = new Vector3(10f, 0f, 0f);
			Vector3 zero = Vector3.zero;
			Move move = new Move(TestGameObject, value, zero, 1f, 3f, TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.easeInOutBack, null, TransitionStep.CoordinateSpaceType.Global, LogStart, LogUpdate, LogComplete);
			move.AddOnCompleteAction(LogComplete2, "Complete Parameter");
			move.ScaleToOriginal(Vector3.zero, 1f, 1f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, runAtStart: true).RotateFromCurrent(new Vector3(360f, 0f, 0f), 1f, 3f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, TransitionStep.CoordinateSpaceType.Local, runAtStart: true).RotateToOriginal(new Vector3(180f, 0f, 0f), 0f, 2f)
				.ScaleFromCurrent(Vector3.zero, 1f, 2f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, runAtStart: true, null, null, Step2LinkedTransitionsInOneCall);
			move.Start();
		}

		private void Step2LinkedTransitionsInOneCall(TransitionStep transitionStep)
		{
			ShowTransitionedDescription("Linked transitions in one call.");
			new Scale(TestGameObject2, Vector3.zero, Vector3.one * 5f, 0f, 3f, TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.easeInOutBack).ScaleFromCurrent(new Vector3(7.5f, 2.5f, 1f), 1f, 2f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.easeInOutBack).ScaleFromCurrent(new Vector3(10f, 10f, 1f), 1f, 2f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.easeInOutBack).ScaleFromCurrent(Vector3.zero, 1f, 2f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.easeInOutBack, null, runAtStart: false, null, null, Step3TransitionWithCallback)
				.GetChainRoot()
				.Start();
		}

		private void Step3TransitionWithCallback(TransitionStep transitionStep)
		{
			ShowTransitionedDescription("Trigger a transition component with callback.");
			TransitionHelper.TransitionIn(TestGameObject3);
		}

		public void Step4CustomTransitionStep()
		{
			ShowTransitionedDescription("Custom TransitionStep for your own transitions.");
			new Fade(Counter.gameObject, 0f, 1f, 0f, 4f).ScaleToOriginal(Vector3.zero, 0f, 3f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, runAtStart: true).ChainCustomTransitionStep(0f, 2f, TransitionStep.TransitionModeType.Specified, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, runAtStart: false, null, CustomTransitionStepUpdateCallback).ScaleFromCurrent(Vector3.zero, 0.5f, 2f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.easeInOutBack, null, runAtStart: false, null, null, Step5FadeAndScale)
				.GetChainRoot()
				.Start();
		}

		private void CustomTransitionStepUpdateCallback(TransitionStep transitionStep)
		{
			Counter.text = transitionStep.Progress.ToString();
		}

		private void Step5FadeAndScale(TransitionStep transitionStep)
		{
			ShowTransitionedDescription("Fade and Scale in one.");
			new Fade(TestGameObject5, 0f, 1f, 0f, 4f).ScaleToOriginal(Vector3.zero, 0f, 3f, TransitionStep.TimeUpdateMethodType.GameTime, TransitionHelper.TweenType.linear, null, runAtStart: true).GetChainRoot().Start();
		}

		private void ShowTransitionedDescription(string text)
		{
			Description.text = text;
			Fade fade = new Fade(Description.gameObject);
			fade.FadeFromCurrent(0f, 3f);
			fade.GetChainRoot().Start();
		}

		private void LogStart(TransitionStep transitionStep)
		{
			UnityEngine.Debug.Log("Start");
		}

		private void LogUpdate(TransitionStep transitionStep)
		{
			UnityEngine.Debug.Log("Update:" + transitionStep.Progress);
		}

		private void LogComplete(TransitionStep transitionStep)
		{
			UnityEngine.Debug.Log("Complete with user data:" + ((transitionStep == null) ? "<null>" : transitionStep.UserData.ToString()));
		}

		private void LogComplete2(TransitionStep transitionStep)
		{
			UnityEngine.Debug.Log("Complete (second callback)");
		}

		public void ShowRatePage()
		{
			Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
		}
	}
	public class TestController : MonoBehaviour
	{
		public GameObject TransitionFromButtons;

		public void TransitionIn()
		{
			TransitionHelper.TransitionIn(TransitionFromButtons);
		}

		public void TransitionOut()
		{
			TransitionHelper.TransitionOut(TransitionFromButtons);
		}

		public void ShowRatePage()
		{
			Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
		}
	}
	public class TransitionEvents : MonoBehaviour
	{
		public GameObject TransitionGameobject;

		public Text Status;

		public Text Progress;

		public Image ProgressFill;

		public void TransitionIn()
		{
			TransitionHelper.TransitionIn(TransitionGameobject);
		}

		public void TransitionOut()
		{
			TransitionHelper.TransitionOut(TransitionGameobject);
		}

		public void TransitionInStarted()
		{
			Status.text = "Transition In Started";
		}

		public void TransitionInCompleted()
		{
			Status.text = "Transition In Completed";
		}

		public void TransitionOutStarted()
		{
			Status.text = "Transition Out Started";
		}

		public void TransitionOutCompleted()
		{
			Status.text = "Transition Out Completed";
		}

		public void UpdateProgress(TransitionStep transitionStep)
		{
			Progress.text = (int)(transitionStep.Progress * 100f) + "%";
			ProgressFill.fillAmount = transitionStep.Progress;
		}

		public void ShowRatePage()
		{
			Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
		}
	}
}
namespace BeautifulTransitions._Demo.Shake.Scripts
{
	public class ShakeController : MonoBehaviour
	{
		public Text DurationText;

		public Slider DurationSlider;

		public Text DecayStartText;

		public Slider DecayStartSlider;

		public InputField XInput;

		public InputField YInput;

		public InputField ZInput;

		private void Start()
		{
			DurationSlider.value = ShakeCamera.Instance.Duration;
			DecayStartSlider.value = ShakeCamera.Instance.DecayStart;
			XInput.text = ShakeCamera.Instance.Range.x.ToString();
			YInput.text = ShakeCamera.Instance.Range.y.ToString();
			ZInput.text = ShakeCamera.Instance.Range.z.ToString();
		}

		private void Update()
		{
			DurationText.text = $"Duration ({DurationSlider.value})";
			DecayStartText.text = $"Decay Start ({DecayStartSlider.value})";
		}

		public void Shake()
		{
			ShakeCamera.Instance.Shake(DurationSlider.value, new Vector3(float.Parse(XInput.text), float.Parse(YInput.text), float.Parse(ZInput.text)), DecayStartSlider.value);
		}
	}
}
namespace BeautifulTransitions._Demo.DisplayItem.Scripts
{
	public class DisplayItemController : MonoBehaviour
	{
		public Button TestButton;

		public Button ShowButton;

		public Button HideButton;

		public Button EnableButton;

		public Button DisableButton;

		public Button AttentionButton;

		public void Start()
		{
			DisplayItemHelper.SetAttention(AttentionButton.gameObject, attention: true);
		}

		public void ShowClicked()
		{
			DisplayItemHelper.SetActiveAnimated(this, TestButton.gameObject, value: true);
			HideButton.interactable = true;
			ShowButton.interactable = false;
			SetEnableButtonStates();
		}

		public void HideClicked()
		{
			DisplayItemHelper.SetActiveAnimated(this, TestButton.gameObject, value: false);
			HideButton.interactable = false;
			ShowButton.interactable = true;
			SetEnableButtonStates();
		}

		public void EnableClicked()
		{
			TestButton.interactable = true;
			SetEnableButtonStates();
		}

		public void DisableClicked()
		{
			TestButton.interactable = false;
			SetEnableButtonStates();
		}

		private void SetEnableButtonStates()
		{
			EnableButton.interactable = !TestButton.interactable;
			DisableButton.interactable = TestButton.interactable;
		}

		public void ShowRatePage()
		{
			Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
		}
	}
}
