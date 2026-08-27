```powershell
winapp run .\ProjectManager.sln `
    -c Debug `
    --arch x64 `
    -p PublishSingleFile=false `
    -p PublishTrimmed=false `
    --output-appx-directory C:\Tool\appx `
    --no-launch `
    --verbose
```
