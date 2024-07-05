// TODO https://github.com/dotnet/maui/issues/884#issuecomment-1760299780

using System.Collections.Concurrent;
using Android.App;
using Android.Content;

namespace MauiHybridApp;

// Copied from: `Microsoft.Maui.Platform.ActivityResultCallbackRegistry` (https://github.com/dotnet/maui/blob/main/src/Core/src/Platform/Android/ActivityResultCallbackRegistry.cs)
public static class CustomActivityResultCallbackRegistry
{
    static readonly ConcurrentDictionary<int, Action<Result, Intent>> ActivityResultCallbacks =
        new();

    static int s_nextActivityResultCallbackKey = Random.Shared.Next();

    public static void InvokeCallback(int requestCode, Result resultCode, Intent data)
    {
        if (ActivityResultCallbacks.TryGetValue(requestCode, out Action<Result, Intent>? callback))
        {
            callback(resultCode, data);
        }
    }

    internal static int RegisterActivityResultCallback(Action<Result, Intent> callback)
    {
        int requestCode = s_nextActivityResultCallbackKey;

        while (!ActivityResultCallbacks.TryAdd(requestCode, callback))
        {
            s_nextActivityResultCallbackKey += 1;
            requestCode = s_nextActivityResultCallbackKey;
        }

        s_nextActivityResultCallbackKey += 1;

        return requestCode;
    }

    internal static void UnregisterActivityResultCallback(int requestCode)
    {
        ActivityResultCallbacks.TryRemove(requestCode, out Action<Result, Intent>? callback);
    }
}
