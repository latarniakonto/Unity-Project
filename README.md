## As a part of Introduction to Cloud Computing course at my university I'm preparing custom game deployment with dedicated game server hosted on Google Cloud Platform

<br/>

## Steps
    1. Creating the game in Unity
    2. Building server and client version of the game
    3. Creating Docker image using server build
    4. Pushing it to Docker registry or Google Cloud registry
    5. Installing Kubernetes and Agones in Google Cloud Platform
    
<br/>


There was problem with memory leak in default AgonesSdk.cs on my Unity version 2021.1.27f1
 
Memory leakead happend on <br/>
```uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json))```<br/>
inside ```async Task<AsyncResult> SendRequestAsync(string, string, string)``` function.
```csharp
// To prevent that an async method leaks after destroying this gameObject.
cancellationTokenSource.Token.ThrowIfCancellationRequested();

var req = new UnityWebRequest(sidecarAddress + api, method)
{
    uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
    downloadHandler = new DownloadHandlerBuffer()
};
req.SetRequestHeader("Content-Type", "application/json");

await new AgonesAsyncOperationWrapper(req.SendWebRequest());

var result = new AsyncResult();

result.ok = req.responseCode == (long) HttpStatusCode.OK;

if (result.ok)
{
    result.json = req.downloadHandler.text;
    Log($"Agones SendRequest ok: {api} {req.downloadHandler.text}");
}
else
{
    Log($"Agones SendRequest failed: {api} {req.error}");
}

return result;
```
To prevent the memory leaked from happening I added ``` using ``` statement as in: <br/>
<https://docs.unity3d.com/2021.2/Documentation/ScriptReference/Networking.UnityWebRequest.Dispose.html>

```csharp
// To prevent that an async method leaks after destroying this gameObject.
cancellationTokenSource.Token.ThrowIfCancellationRequested();
//MODIFIED !!!!!
using (var req = new UnityWebRequest(sidecarAddress + api, method)
{
    uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
    downloadHandler = new DownloadHandlerBuffer()
})
{
    req.SetRequestHeader("Content-Type", "application/json");

    await new AgonesAsyncOperationWrapper(req.SendWebRequest());

    var result = new AsyncResult();

    result.ok = req.responseCode == (long) HttpStatusCode.OK;

    if (result.ok)
    {
        result.json = req.downloadHandler.text;
        Log($"Agones SendRequest ok: {api} {req.downloadHandler.text}");
    }
    else
    {
        Log($"Agones SendRequest failed: {api} {req.error}");
    }

    return result;
}
```