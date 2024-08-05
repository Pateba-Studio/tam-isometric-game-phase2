using UnityEngine;
using System.Runtime.InteropServices;

namespace MarksAssets.LaunchURLWebGL {
	[DisallowMultipleComponent]
	public class LaunchURLWebGL:MonoBehaviour {
		private static LaunchURLWebGL m_Instance = null;

		public static LaunchURLWebGL instance { get => m_Instance; }

		private void Awake() {
			if (m_Instance != null && m_Instance != this) {
				Destroy(this.gameObject);
			} else {
				m_Instance = this;
			}

		}

		//don't forget that you need to call all methods below on a pointerdown event to guarantee that they will work properly.
		[DllImport("__Internal", EntryPoint="LaunchURLWebGL_launchURL")]
		public static extern void launchURL(string url = "", string windowName = "_blank", string windowFeatures = "");
		
		//since most of the time you won't care about the "windowsFeatures" parameter, and you can't call a method with more than 1 parameter directly in the inspector,
		//I made the three methods below for convenience. Open in the same tab/window, and open in a new tab/window, respectively.
		public void launchURLSelf(string url = "") {
			launchURL(url, "_self");
		}
		
		public void launchURLBlank(string url = "") {
			launchURL(url);
		}

		public void OpenURL(string url) {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                launchURL(url);
            else
                Application.OpenURL(url);
        }
		
	}
}