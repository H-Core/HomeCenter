# Description
Hints for developers

Issues:
1. [ ] Debug with enabled MouseHook

### Debug with enabled MouseHook
*This solution is temporary and have delay* <br/>
Use this action in breakpoint(also disable continue code execution):
```
{HomeCenter.NET.Bootstrapper.StopMouseHook()}
```
![MouseHook](https://github.com/HavenDV/HomeCenter.NET/blob/master/DeveloperReadme/mouse_hook.png)

### Caliburn.Micro 4.0.104-alpha+

Use latest install command from https://www.myget.org/feed/caliburn-micro-builds/package/nuget/Caliburn.Micro  <br/>
Nuget Console to run command available here:  <br/>
**Tools -> NuGET Package Manager -> Package Manager Console**

```
Install-Package Caliburn.Micro -Version 4.0.104-alpha -Source https://www.myget.org/F/caliburn-micro-builds/api/v3/index.json
```

## YandexConverter - How to receive OAuthToken and FolderId
### OAuthToken
1. Go to https://cloud.yandex.ru/docs/iam/operations/iam-token/create
2. Select API
3. Go to url from second option

### FolderId
1. Go to https://console.cloud.yandex.ru/
2. Copy value from "cloud-$USERNAME$"