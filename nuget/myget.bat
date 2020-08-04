nuget pack Transformalize.Provider.Web.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.Web.Autofac.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Web.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Transform.Web.Autofac.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Validate.Web.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Validate.Web.Autofac.nuspec -OutputDirectory "c:\temp\modules"

REM nuget push "c:\temp\modules\Transformalize.Provider.Web.0.8.22-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.Web.Autofac.0.8.22-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Transform.Web.0.8.22-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Transform.Web.Autofac.0.8.22-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Validate.Web.0.8.22-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Validate.Web.Autofac.0.8.22-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json




