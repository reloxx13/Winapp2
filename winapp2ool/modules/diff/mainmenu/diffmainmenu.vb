﻿'    Copyright (C) 2018-2021 Hazel Ward
' 
'    This file is a part of Winapp2ool
' 
'    Winapp2ool is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    Winapp2ool is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with Winapp2ool.  If not, see <http://www.gnu.org/licenses/>.
Option Strict On
''' <summary> Displays the Diff main menu to the user and handles their input </summary>
''' Docs last updated: 2020-08-30
Module diffmainmenu

    ''' <summary> Prints the Diff main menu to the user </summary>
    ''' Docs last updated: 2020-07-23 | Code last updated: 2020-07-19
    Public Sub printDiffMainMenu()
        Console.WindowHeight = If(DiffModuleSettingsChanged, 34, 32)
        printMenuTop({"Observe the differences between two ini files"})
        print(1, "Run (default)", "Run the diff tool", enStrCond:=Not (DiffFile2.Name.Length = 0 AndAlso Not DownloadDiffFile), colorLine:=True)
        print(0, "Select Older/Local File:", leadingBlank:=True)
        print(1, "File Chooser", "Choose a new name or location for your older ini file")
        print(0, "Select Newer/Remote File:", leadingBlank:=True)
        print(5, GetNameFromDL(True), "diffing against the latest winapp2.ini version on GitHub", cond:=Not isOffline, enStrCond:=DownloadDiffFile, leadingBlank:=True)
        print(5, "Remote file trimming", "trimming the remote winapp2.ini before diffing", cond:=DownloadDiffFile = True, enStrCond:=TrimRemoteFile, trailingBlank:=True)
        print(1, "File Chooser", "Choose a new name or location for your newer ini file", Not DownloadDiffFile, isOffline, True)
        print(0, "Log Settings:")
        print(5, "Toggle Log Saving", "automatic saving of the Diff output", leadingBlank:=True, trailingBlank:=Not SaveDiffLog, enStrCond:=SaveDiffLog)
        print(1, "File Chooser (log)", "Change where Diff saves its log", SaveDiffLog, trailingBlank:=True)
        print(5, "Verbose Mode", "printing full entries in the diff output", enStrCond:=ShowFullEntries, trailingBlank:=True)
        print(0, $"Older file: {replDir(DiffFile1.Path)}")
        print(0, $"Newer file: {If(DiffFile2.Name.Length = 0 AndAlso Not DownloadDiffFile, "Not yet selected", If(DownloadDiffFile, GetNameFromDL(True), replDir(DiffFile2.Path)))}",
                                  closeMenu:=Not SaveDiffLog AndAlso Not DiffModuleSettingsChanged AndAlso MostRecentDiffLog.Length = 0)
        print(0, $"Log   file: {replDir(DiffFile3.Path)}", cond:=SaveDiffLog, closeMenu:=(Not DiffModuleSettingsChanged) AndAlso MostRecentDiffLog.Length = 0)
        print(2, "Diff", cond:=DiffModuleSettingsChanged, closeMenu:=MostRecentDiffLog.Length = 0)
        print(1, "Log Viewer", "Show the most recent Diff log", cond:=Not MostRecentDiffLog.Length = 0, closeMenu:=True, leadingBlank:=True)
    End Sub

    ''' <summary> Handles the user input from the Diff main menu </summary>
    ''' <param name="input"> The user's input </param>
    ''' Docs last updated: 2020-09-01 | Code last updated: 2020-09-01
    Public Sub handleDiffMainMenuUserInput(input As String)
        Select Case True
            Case input = "0"
                exitModule()
            Case input = "1" OrElse input.Length = 0
                If Not denyActionWithHeader(DiffFile2.Name.Length = 0 AndAlso Not DownloadDiffFile, "Please select a file against which to diff") Then initDiff()
            Case input = "2"
                changeFileParams(DiffFile1, DiffModuleSettingsChanged, NameOf(Diff), NameOf(DiffFile1), NameOf(DiffModuleSettingsChanged))
            Case input = "3" AndAlso Not isOffline
                If Not denySettingOffline() Then toggleSettingParam(DownloadDiffFile, "Downloading", DiffModuleSettingsChanged, NameOf(CCiniDebug), NameOf(DownloadDiffFile),
                                                                                                                                            NameOf(DiffModuleSettingsChanged))
                DiffFile2.Name = GetNameFromDL(DownloadDiffFile)
            Case input = "4" AndAlso DownloadDiffFile
                toggleSettingParam(TrimRemoteFile, "Trimming", DiffModuleSettingsChanged, NameOf(Trim), NameOf(TrimRemoteFile), NameOf(DiffModuleSettingsChanged))
            Case (input = "4" AndAlso Not (DownloadDiffFile OrElse isOffline)) OrElse (input = "3" AndAlso isOffline)
                changeFileParams(DiffFile2, DiffModuleSettingsChanged, NameOf(Trim), NameOf(DiffFile2), NameOf(DiffModuleSettingsChanged))
            Case (input = "5" AndAlso Not isOffline) OrElse (input = "4" AndAlso isOffline)
                toggleSettingParam(SaveDiffLog, "Log Saving", DiffModuleSettingsChanged, NameOf(Trim), NameOf(SaveDiffLog), NameOf(DiffModuleSettingsChanged))
            Case SaveDiffLog AndAlso ((input = "6" AndAlso Not isOffline) OrElse (input = "5" AndAlso isOffline))
                changeFileParams(DiffFile3, DiffModuleSettingsChanged, NameOf(Trim), NameOf(DiffFile3), NameOf(DiffModuleSettingsChanged))
            Case (input = "6" AndAlso Not SaveDiffLog) OrElse (input = "7" AndAlso SaveDiffLog)
                toggleSettingParam(ShowFullEntries, "Verbose Mode", DiffModuleSettingsChanged, NameOf(Trim), NameOf(ShowFullEntries), NameOf(DiffModuleSettingsChanged))
            Case DiffModuleSettingsChanged AndAlso ( 'Online Case below
                                        (Not isOffline AndAlso ((Not SaveDiffLog AndAlso input = "7") OrElse
                                        (SaveDiffLog AndAlso input = "8"))) OrElse
                                        (isOffline AndAlso ((input = "5") OrElse (input = "6" AndAlso SaveDiffLog)))) ' Offline case
                resetModuleSettings(NameOf(Diff), AddressOf initDefaultDiffSettings)
            Case Not MostRecentDiffLog.Length = 0 AndAlso ((input = "7" AndAlso Not DiffModuleSettingsChanged) OrElse (input = "8" AndAlso DiffModuleSettingsChanged))
                MostRecentDiffLog = getLogSliceFromGlobal("Beginning diff", "Diff complete")
                printSlice(MostRecentDiffLog)
            Case Else
                setHeaderText(invInpStr, True)
        End Select
    End Sub

End Module