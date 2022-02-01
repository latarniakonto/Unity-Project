## As a part of Introduction to Cloud Computing course at my university I'm preparing custom game deployment with dedicated game server hosted on Google Cloud Platform


## Requirements
    Google Cloud Platform account
    Google Cloud SDK installed
    Docker installed
    Docker Compose installed

<br/>

## Steps
    1. Creating the game in Unity
    2. Building server and client version of the game
    3. Creating Docker image using server build
    4. Pushing it to Docker registry or Google Cloud registry
    5. Installing Kubernetes and Agones in Google Cloud Platform
    6. Run the game on the server
<br/>

## Creating the game
I am using Unity Engine to make this waiting room. You can use also Unreal Engine. For the full list of supported platforms by Agones SDK check the following link: https://agones.dev/site/docs/guides/client-sdks/

<img src="https://i.imgur.com/WHRaFlt.png"/>

## Game builds
You need to have a server version of yourn game and client version. Create builds
for both versions.

<p>
    <img src="https://i.imgur.com/9UQGIK6.png" width="400" height="300" />
    <img src="https://i.imgur.com/IEgegAG.png" width="400" height="300" />
</p>

## Docker image
I am using Docker Compose to create the Docker image. I have the following structure in my unity server build folder. <br/>
Run this command when you have everything ready: <br/>
`docker-compose up -d --build`
```
Server_Build_Folder_Structure
│   docker-compose.yml
|   fleet_configs.yaml
└───unity
│   │   Dockerfile
│   │   entrypoint.sh
│   │   sdk-server.linux.amd64
│   └───build
│       │   server.x86_64
│       │   ...
```
After everything is ready you should have new Docker image.<br/>
Find it's repository name using: <br/>
`docker images`

## Push the image to Google Container Registry
Tag the image in specific format: <br/>
`docker tag docker_image_repository_name eu.gcr.io/GCP_PROJECT_ID/docker_image_repository_name` <br/>
You have pick Google Container Registry location. I am using *eu.gcr.io* location. You can find more locations here:<br/>
https://cloud.google.com/container-registry/docs/overview <br/>

Push the image to your GCP project:<br/>
`docker push eu.gcr.io/GCP_PROJECT_ID/docker_image_repository_name`

## Set up your GCP project




There was problem with memory leak in default AgonesSdk.cs on my Unity version 2021.1.27f1 run on Ubuntu Focal
 
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