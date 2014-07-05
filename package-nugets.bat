@echo off
set version=%1
set key=%2
shift

nuget.exe pack Nancy.Raygun.nuspec -Version %version%

nuget.exe push Nancy.Raygun.%version%.nupkg %key%
