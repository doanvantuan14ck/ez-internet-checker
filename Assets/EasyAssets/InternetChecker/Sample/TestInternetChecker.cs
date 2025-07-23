using System.Collections;
using EasyAssets.InternetChecker.Scripts;
using EasyAssets.InternetChecker.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TestInternetChecker : MonoBehaviour
{
    [SerializeField] private Text textMesh;
    void Awake()
    {
        InternetCheckManager.Instance.OnInternetStatusChecked.AddListener(OnInternetStatusChecked);
    }

    public void TestInternet()
    {
        textMesh.text = "Checking Internet...";
        StartCoroutine(CheckInternetCoroutine());
    }

    private IEnumerator CheckInternetCoroutine()
    {
        yield return CoroutineTaskExtensions.WaitForTask(InternetCheckManager.Instance.IsInternetAvailable(),
            result =>
            {
                Debug.Log("Is connected: " + result);
                OnInternetStatusChecked(result);
            });
    }

    private void OnDisable()
    {
        InternetCheckManager.Instance.OnInternetStatusChecked.RemoveListener(OnInternetStatusChecked);
    }

    private void OnInternetStatusChecked(bool isInternetAvailable)
    {
        textMesh.text = isInternetAvailable ? "Internet Available" : "Internet Not Available";
        Debug.Log($"OnInternetStatusChecked - Internet available: {isInternetAvailable}");
    }
}