namespace EasyAssets.InternetChecker.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Events;
    using Runtime;

    public class InternetCheckManager : MonoBehaviour
    {
        #region Singleton

        private static InternetCheckManager _instance;
        public static InternetCheckManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InternetCheckManager>();
                }

                if (_instance == null)
                {
                    Debug.LogError("There is no instance of InternetCheckManager");
                }
                
                return _instance;
            }
        }

        #endregion

        public InternetCheckerSettings settings;
        public UnityEvent<bool> OnInternetStatusChecked;

        private float lastCheckTime = -999f;
        private bool lastResult = false;

        public async Task<bool> IsInternetAvailable(bool useCache = true)
        {
            if(settings == null)
            {
                Debug.LogError("There is no internet settings");
                return false;
            }
            if (useCache && Time.time - lastCheckTime < settings.minIntervalSeconds)
            {
                Debug.Log("Using cached internet check result.");
                return lastResult;
            }

            List<CaptivePortalMethod> methods = new(settings.fallbackMethods);
            methods.Insert(0, settings.method); // add primary method to top

            foreach (var method in methods)
            {
                string url = GetCheckUrl(method, settings.customUrl);
                using HttpClient client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(settings.timeoutSeconds)
                };

                try
                {
                    var response = await client.GetAsync(url);
                    bool ok = response.StatusCode == HttpStatusCode.NoContent || response.IsSuccessStatusCode;

                    if (ok)
                    {
                        lastResult = true;
                        lastCheckTime = Time.time;
                        OnInternetStatusChecked?.Invoke(true);
                        return true;
                    }
                }
                catch
                {
                    Debug.LogError($"Failed to connect to {url}");
                }
            }

            lastResult = false;
            lastCheckTime = Time.time;
            OnInternetStatusChecked?.Invoke(false);
            return false;
        }

        private string GetCheckUrl(CaptivePortalMethod method, string custom)
        {
            return method switch
            {
                CaptivePortalMethod.ConnectivityCheckGStatic => "http://connectivitycheck.gstatic.com/generate_204",
                CaptivePortalMethod.ConnectivityCheckGStaticHTTPS =>
                    "https://connectivitycheck.gstatic.com/generate_204",
                CaptivePortalMethod.Google204 => "http://clients3.google.com/generate_204",
                CaptivePortalMethod.Google204HTTPS => "https://clients3.google.com/generate_204",
                CaptivePortalMethod.GoogleBlank => "http://www.google.com/blank.html",
                CaptivePortalMethod.GoogleBlankHTTPS => "https://www.google.com/blank.html",
                CaptivePortalMethod.MicrosoftNCSI => "http://www.msftncsi.com/ncsi.txt",
                CaptivePortalMethod.MicrosoftNCSI_IPV6 => "http://ipv6.msftncsi.com/ncsi.txt",
                CaptivePortalMethod.Apple => "http://captive.apple.com",
                CaptivePortalMethod.Apple2 => "http://www.apple.com/library/test/success.html",
                CaptivePortalMethod.AppleHTTPS => "https://captive.apple.com/hotspot-detect.html",
                CaptivePortalMethod.Ubuntu => "http://connectivity-check.ubuntu.com",
                CaptivePortalMethod.UbuntuHTTPS => "https://connectivity-check.ubuntu.com",
                CaptivePortalMethod.MicrosoftConnectTest => "http://www.msftconnecttest.com/connecttest.txt",
                CaptivePortalMethod.MicrosoftConnectTest_IPV6 => "http://ipv6.msftconnecttest.com/connecttest.txt",
                CaptivePortalMethod.Custom => custom,
                _ => "http://clients3.google.com/generate_204",
            };
        }
    }
}