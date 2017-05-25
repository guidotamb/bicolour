using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class UnityAdsExample : MonoBehaviour
{
	public void ShowAd()
	{
		#if UNITY_ADS
		if (Advertisement.IsReady())
		{
			Advertisement.Show();
		}
		#endif
	}
}