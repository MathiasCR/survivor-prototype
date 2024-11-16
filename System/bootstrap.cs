using UnityEngine;
using static UnityEngine.Object;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute() => DontDestroyOnLoad(Instantiate(Resources.Load("System")));
}
