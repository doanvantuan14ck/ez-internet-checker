using System.Collections.Generic;
using UnityEngine;

namespace EasyAssets.InternetChecker.Runtime
{
    using Scripts;
    public class InternetCheckerSettings : ScriptableObject
    {
        public CaptivePortalMethod method = CaptivePortalMethod.ConnectivityCheckGStaticHTTPS;
        public List<CaptivePortalMethod> fallbackMethods = new();
        public string customUrl = "https://www.google.com/";
        public float timeoutSeconds = 5f;
        public float minIntervalSeconds = 5f; // cache duration
    }
}