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
''' <summary> Contains the menu for CCiniDebug and handles the user input into that menu </summary>
Module ccdbmainmenu

    ''' <summary> Prints the CCiniDebug menu to the user </summary>
    ''' Docs last updated: 2020-07-18 | Code last updated: 2020-07-18
    Public Sub printCCDBMainMenu()
        printMenuTop({"Sort alphabetically the contents of ccleaner.ini and prune stale winapp2.ini settings"})
        print(1, "Run (default)", "Debug ccleaner.ini", trailingBlank:=True, enStrCond:=PruneStaleEntries Or SaveDebuggedFile Or SortFileForOutput, colorLine:=True)
        print(5, "Toggle Pruning", "removal of dead winapp2.ini settings", enStrCond:=PruneStaleEntries)
        print(5, "Toggle Saving", "automatic saving of changes made by CCiniDebug", enStrCond:=SaveDebuggedFile)
        print(5, "Toggle Sorting", "alphabetical sorting of ccleaner.ini", enStrCond:=SortFileForOutput, trailingBlank:=True)
        print(1, "File Chooser (ccleaner.ini)", "Choose a new ccleaner.ini name or location")
        print(1, "File Chooser (winapp2.ini)", "Choose a new winapp2.ini name or location", PruneStaleEntries, trailingBlank:=Not SaveDebuggedFile)
        print(1, "File Chooser (save)", "Change where CCiniDebug saves its changes", SaveDebuggedFile, trailingBlank:=True)
        print(0, $"Current ccleaner.ini:  {replDir(CCDebugFile2.Path)}")
        print(0, $"Current winapp2.ini:   {replDir(CCDebugFile1.Path)}", cond:=PruneStaleEntries)
        print(0, $"Current save location: {replDir(CCDebugFile3.Path)}", cond:=SaveDebuggedFile, closeMenu:=Not CCDBSettingsChanged)
        print(2, NameOf(CCiniDebug), cond:=CCDBSettingsChanged, closeMenu:=True)
    End Sub

    ''' <summary> Handles the user's input from the CCiniDebug main menu </summary>
    ''' <param name="input"> The user's input </param>
    ''' Docs last updated: 2020-07-18 | Code last updated: 2020-07-18
    Public Sub handleCCDBMainMenuUserInput(input As String)
        Select Case True
            Case input = "0"
                exitModule()
            Case (input = "1" OrElse input.Length = 0) AndAlso (PruneStaleEntries OrElse SaveDebuggedFile OrElse SortFileForOutput)
                initCCDebug()
            Case input = "2"
                toggleSettingParam(PruneStaleEntries, "Pruning", CCDBSettingsChanged, NameOf(CCiniDebug), NameOf(PruneStaleEntries), NameOf(CCDBSettingsChanged))
            Case input = "3"
                toggleSettingParam(SaveDebuggedFile, "Autosaving", CCDBSettingsChanged, NameOf(CCiniDebug), NameOf(SaveDebuggedFile), NameOf(CCDBSettingsChanged))
            Case input = "4"
                toggleSettingParam(SortFileForOutput, "Sorting", CCDBSettingsChanged, NameOf(CCiniDebug), NameOf(SortFileForOutput), NameOf(CCDBSettingsChanged))
            Case input = "5"
                changeFileParams(CCDebugFile2, CCDBSettingsChanged, NameOf(CCiniDebug), NameOf(CCDebugFile2), NameOf(CCDBSettingsChanged))
            Case input = "6" AndAlso PruneStaleEntries
                changeFileParams(CCDebugFile1, CCDBSettingsChanged, NameOf(CCiniDebug), NameOf(CCDebugFile1), NameOf(CCDBSettingsChanged))
            Case SaveDebuggedFile AndAlso ((input = "6" AndAlso Not PruneStaleEntries) OrElse (input = "7" AndAlso PruneStaleEntries))
                changeFileParams(CCDebugFile3, CCDBSettingsChanged, NameOf(CCiniDebug), NameOf(CCDebugFile3), NameOf(CCDBSettingsChanged))
            Case CCDBSettingsChanged AndAlso
                                        ((input = "6" AndAlso Not (PruneStaleEntries OrElse SaveDebuggedFile)) OrElse
                                        (input = "7" AndAlso (PruneStaleEntries Xor SaveDebuggedFile)) OrElse
                                        (input = "8" AndAlso PruneStaleEntries AndAlso SaveDebuggedFile))
                resetModuleSettings("CCiniDebug", AddressOf initDefaultCCDBSettings)
            Case Not (PruneStaleEntries OrElse SaveDebuggedFile OrElse SortFileForOutput)
                setHeaderText("Please enable at least one option", True)
            Case Else
                setHeaderText(invInpStr, True)
        End Select
    End Sub

End Module