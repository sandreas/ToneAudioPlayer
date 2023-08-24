#!/usr/bin/env bash

dotnet publish *.Android -f net7.0-android -c Release -p:PublishTrimmed=true -p:TrimmerRemoveSymbols=true -p:RunAOTCompilation=true -p:DebugType=None -p:DebugSymbols=false -p:GenerateDocumentationFile=false -o dist/android/