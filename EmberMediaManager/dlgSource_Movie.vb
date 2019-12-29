﻿' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports EmberAPI
Imports NLog
Imports System.IO
Imports System.Text.RegularExpressions

Public Class dlgSource_Movie

#Region "Fields"

    Shared _Logger As Logger = LogManager.GetCurrentClassLogger()

    Private _AutoName As Boolean = True
    Private _CurrentName As String = String.Empty
    Private _CurrentPath As String = String.Empty
    Private _ID As Long = -1
    Private _KnownSources As List(Of Database.DBSource) = New List(Of Database.DBSource)
    Private _OldName As String = String.Empty
    Private _OldPath As String = String.Empty
    Private _TempPath As String = String.Empty

#End Region 'Fields

#Region "Properties"

    Public Property Result As Database.DBSource = Nothing

#End Region 'Properties

#Region "Dialog"

    Public Sub New(ByVal knownSources As List(Of Database.DBSource))
        ' This call is required by the designer.
        InitializeComponent()
        FormsUtils.ResizeAndMoveDialog(Me, Me)
        _KnownSources = knownSources
    End Sub

    Private Sub Dialog_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Setup()
        If Not _ID = -1 Then
            Dim s As Database.DBSource = _KnownSources.FirstOrDefault(Function(y) y.ID = _ID)
            If s IsNot Nothing Then
                _AutoName = False
                If cbSourceLanguage.Items.Count > 0 Then
                    Dim tLanguage As languageProperty = APIXML.ScraperLanguages.Languages.FirstOrDefault(Function(l) l.Abbreviation = s.Language)
                    If tLanguage IsNot Nothing Then
                        cbSourceLanguage.Text = tLanguage.Description
                    Else
                        tLanguage = APIXML.ScraperLanguages.Languages.FirstOrDefault(Function(l) l.Abbreviation.StartsWith(s.Language))
                        If tLanguage IsNot Nothing Then
                            cbSourceLanguage.Text = tLanguage.Description
                        Else
                            cbSourceLanguage.Text = APIXML.ScraperLanguages.Languages.FirstOrDefault(Function(l) l.Abbreviation = "en-US").Description
                        End If
                    End If
                End If
                chkExclude.Checked = s.Exclude
                chkGetYear.Checked = s.GetYear
                chkScanRecursive.Checked = s.ScanRecursive
                chkIsSingle.Checked = s.IsSingle
                chkUseFolderName.Checked = s.UseFolderName
                txtSourceName.Text = s.Name
                txtSourcePath.Text = s.Path
            End If
        Else
            If cbSourceLanguage.Items.Count > 0 Then
                Dim tLanguage As languageProperty = APIXML.ScraperLanguages.Languages.FirstOrDefault(Function(l) l.Abbreviation = Master.eSettings.Movie.SourceSettings.DefaultLanguage)
                If tLanguage IsNot Nothing Then
                    cbSourceLanguage.Text = tLanguage.Description
                Else
                    tLanguage = APIXML.ScraperLanguages.Languages.FirstOrDefault(Function(l) l.Abbreviation.StartsWith(Master.eSettings.Movie.SourceSettings.DefaultLanguage))
                    If tLanguage IsNot Nothing Then
                        cbSourceLanguage.Text = tLanguage.Description
                    Else
                        cbSourceLanguage.Text = APIXML.ScraperLanguages.Languages.FirstOrDefault(Function(l) l.Abbreviation = "en-US").Description
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub Dialog_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Shown
        Activate()
        txtSourcePath.Focus()
    End Sub

    Private Sub DialogResult_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub DialogResult_OK_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOK.Click
        Dim strSourcePath As String = Regex.Replace(txtSourcePath.Text.Trim, "^(\\)+\\\\", "\\")
        Dim strLanguage As String = "en-US"
        If Not String.IsNullOrEmpty(cbSourceLanguage.Text) Then
            strLanguage = APIXML.ScraperLanguages.Languages.FirstOrDefault(Function(l) l.Description = cbSourceLanguage.Text).Abbreviation
        End If

        Result = New Database.DBSource With {
            .Exclude = chkExclude.Checked,
            .GetYear = chkGetYear.Checked,
            .ID = _ID,
            .IsSingle = chkIsSingle.Checked,
            .Language = strLanguage,
            .Name = txtSourceName.Text.Trim,
            .Path = strSourcePath,
            .ScanRecursive = chkScanRecursive.Checked,
            .UseFolderName = chkUseFolderName.Checked}

        DialogResult = DialogResult.OK
    End Sub

    Private Sub Setup()
        With Master.eLang
            Text = .GetString(198, "Movie Source")
            btnCancel.Text = .Cancel
            btnOK.Text = .OK
            chkExclude.Text = .GetString(164, "Exclude path from library updates")
            chkGetYear.Text = .GetString(585, "Get year from folder name")
            chkIsSingle.Text = .GetString(202, "Movies are in separate folders")
            chkScanRecursive.Text = .GetString(204, "Scan Recursively")
            chkUseFolderName.Text = .GetString(203, "Use Folder Name for Initial Listing")
            fbdBrowse.Description = .GetString(205, "Select the parent folder for your movie folders/files.")
            gbSourceOptions.Text = .GetString(201, "Source Options")
            lblSourceLanguage.Text = String.Concat(.GetString(1166, "Default Language"), ":")
            lblSourceName.Text = String.Concat(.GetString(199, "Source Name"), ":")
            lblSourcePath.Text = String.Concat(.GetString(200, "Source Path"), ":")
        End With

        cbSourceLanguage.Items.Clear()
        cbSourceLanguage.Items.AddRange((From lLang In APIXML.ScraperLanguages.Languages Select lLang.Description).ToArray)
    End Sub

    Public Overloads Function ShowDialog(ByVal id As Long) As DialogResult
        _ID = id
        btnBrowse.Enabled = False
        chkScanRecursive.Enabled = False
        chkIsSingle.Enabled = False
        txtSourcePath.Enabled = False
        Return ShowDialog()
    End Function

    Public Overloads Function ShowDialog(ByVal searchPath As String) As DialogResult
        _TempPath = searchPath
        Return ShowDialog()
    End Function

    Public Overloads Function ShowDialog(ByVal searchPath As String, ByVal folderPath As String) As DialogResult
        _TempPath = searchPath
        txtSourcePath.Text = folderPath
        Return ShowDialog()
    End Function

#End Region 'Dialog

#Region "Methods"

    Private Sub Browse_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBrowse.Click
        With fbdBrowse
            If Not String.IsNullOrEmpty(txtSourcePath.Text) Then
                .SelectedPath = txtSourcePath.Text
            Else
                .SelectedPath = _TempPath
            End If
            If .ShowDialog = DialogResult.OK Then
                If Not String.IsNullOrEmpty(.SelectedPath) Then
                    txtSourcePath.Text = .SelectedPath
                End If
            End If
        End With
    End Sub

    Private Sub CheckConditions()
        Dim bIsValid_SourceName As Boolean = False
        Dim bIsValid_SourcePath As Boolean = True

        If String.IsNullOrEmpty(txtSourceName.Text.Trim) Then
            pbValidSourceName.Image = My.Resources.invalid
        Else
            'check duplicate source names 
            If _KnownSources.FirstOrDefault(Function(f) Not f.ID = _ID AndAlso f.Name = txtSourceName.Text) IsNot Nothing Then
                pbValidSourceName.Image = My.Resources.invalid
                bIsValid_SourceName = False
            Else
                pbValidSourceName.Image = My.Resources.valid
                bIsValid_SourceName = True
            End If
        End If

        If String.IsNullOrEmpty(txtSourcePath.Text) OrElse Not Directory.Exists(txtSourcePath.Text.Trim) Then
            bIsValid_SourcePath = False
            pbValidSourcePath.Image = My.Resources.invalid
        Else
            For Each tSource In _KnownSources.Where(Function(f) Not f.ID = _ID)
                'check if the path contains another source or is inside another source

                Dim strOtherSource As String = tSource.Path.ToLower
                Dim strCurrentSource As String = txtSourcePath.Text.Trim.ToLower
                'add a directory separator at the end of the path to distinguish between
                'D:\Movies
                'D:\Movies Shared
                '(needed for "LocalPath.ToLower.StartsWith(tLocalSource)"
                If strOtherSource.Contains(Path.DirectorySeparatorChar) Then
                    strOtherSource = If(strOtherSource.EndsWith(Path.DirectorySeparatorChar), strOtherSource, String.Concat(strOtherSource, Path.DirectorySeparatorChar)).Trim
                ElseIf strOtherSource.Contains(Path.AltDirectorySeparatorChar) Then
                    strOtherSource = If(strOtherSource.EndsWith(Path.AltDirectorySeparatorChar), strOtherSource, String.Concat(strOtherSource, Path.AltDirectorySeparatorChar)).Trim
                End If
                If strCurrentSource.Contains(Path.DirectorySeparatorChar) Then
                    strCurrentSource = If(strCurrentSource.EndsWith(Path.DirectorySeparatorChar), strCurrentSource, String.Concat(strCurrentSource, Path.DirectorySeparatorChar)).Trim
                ElseIf strCurrentSource.Contains(Path.AltDirectorySeparatorChar) Then
                    strCurrentSource = If(strCurrentSource.EndsWith(Path.AltDirectorySeparatorChar), strCurrentSource, String.Concat(strCurrentSource, Path.AltDirectorySeparatorChar)).Trim
                End If

                If strOtherSource.StartsWith(strCurrentSource) OrElse
                    strCurrentSource.Contains(strOtherSource) Then
                    bIsValid_SourcePath = False
                    pbValidSourcePath.Image = My.Resources.invalid
                    Exit For
                End If
            Next
        End If

        If bIsValid_SourcePath Then pbValidSourcePath.Image = My.Resources.valid

        If Not String.IsNullOrEmpty(txtSourcePath.Text) AndAlso Directory.Exists(txtSourcePath.Text.Trim) AndAlso
            Not String.IsNullOrEmpty(cbSourceLanguage.Text) AndAlso bIsValid_SourceName AndAlso bIsValid_SourcePath Then
            btnOK.Enabled = True
        End If
    End Sub

    Private Sub IsSingle_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkIsSingle.CheckedChanged
        chkUseFolderName.Enabled = chkIsSingle.Checked
        If Not chkIsSingle.Checked Then chkUseFolderName.Checked = False
    End Sub

    Private Sub SourceLanguage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbSourceLanguage.SelectedIndexChanged
        btnOK.Enabled = False
        tmrWait.Enabled = False
        tmrWait.Enabled = True
    End Sub

    Private Sub SourceName_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles txtSourceName.KeyPress
        _AutoName = False
    End Sub

    Private Sub SourceName_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSourceName.TextChanged
        btnOK.Enabled = False
        _CurrentName = txtSourceName.Text
        tmrWait.Enabled = False
        tmrWait.Enabled = True
    End Sub

    Private Sub SourcePath_Leave(sender As Object, e As EventArgs) Handles txtSourcePath.Leave
        Try
            Dim dInfo As DirectoryInfo = New DirectoryInfo(txtSourcePath.Text)
            If Not txtSourcePath.Text = dInfo.FullName Then
                txtSourcePath.Text = dInfo.FullName
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SourcePath_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSourcePath.TextChanged
        btnOK.Enabled = False
        _CurrentPath = txtSourcePath.Text
        tmrPathWait.Enabled = False
        tmrPathWait.Enabled = True
    End Sub

    Private Sub Timer_Name_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrName.Tick
        tmrWait.Enabled = False
        CheckConditions()
        tmrName.Enabled = False
    End Sub

    Private Sub Timer_Path_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrPath.Tick
        tmrPathWait.Enabled = False
        CheckConditions()
        tmrPath.Enabled = False
    End Sub

    Private Sub Timer_PathWait_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrPathWait.Tick
        If _OldPath = _CurrentPath Then
            tmrPath.Enabled = True
        Else
            If String.IsNullOrEmpty(txtSourceName.Text) OrElse _AutoName Then
                Try
                    If Not String.IsNullOrEmpty(txtSourcePath.Text) Then
                        Dim dInfo As DirectoryInfo = New DirectoryInfo(txtSourcePath.Text)
                        If dInfo IsNot Nothing AndAlso Not String.IsNullOrEmpty(dInfo.Name) Then
                            txtSourceName.Text = dInfo.Name
                            _AutoName = True
                        End If
                    End If
                Catch ex As Exception
                    txtSourceName.Text = String.Empty
                    _AutoName = True
                End Try
            End If
            _OldPath = _CurrentPath
        End If
    End Sub

    Private Sub Timer_Wait_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrWait.Tick
        If _OldName = _CurrentName Then
            tmrName.Enabled = True
        Else
            _OldName = _CurrentName
        End If
    End Sub

    Private Sub UseFolderName_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseFolderName.CheckedChanged
        If chkUseFolderName.Checked Then
            chkGetYear.Text = Master.eLang.GetString(585, "Get year from folder name")
        Else
            chkGetYear.Text = Master.eLang.GetString(584, "Get year from file name")
        End If
    End Sub

#End Region 'Methods

End Class