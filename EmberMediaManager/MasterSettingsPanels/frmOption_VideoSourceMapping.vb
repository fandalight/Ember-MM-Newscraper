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

Public Class frmOption_VideoSourceMapping
    Implements Interfaces.IMasterSettingsPanel

#Region "Events"

    Public Event NeedsDBClean_Movie() Implements Interfaces.IMasterSettingsPanel.NeedsDBClean_Movie
    Public Event NeedsDBClean_TV() Implements Interfaces.IMasterSettingsPanel.NeedsDBClean_TV
    Public Event NeedsDBUpdate_Movie() Implements Interfaces.IMasterSettingsPanel.NeedsDBUpdate_Movie
    Public Event NeedsDBUpdate_TV() Implements Interfaces.IMasterSettingsPanel.NeedsDBUpdate_TV
    Public Event NeedsReload_Movie() Implements Interfaces.IMasterSettingsPanel.NeedsReload_Movie
    Public Event NeedsReload_MovieSet() Implements Interfaces.IMasterSettingsPanel.NeedsReload_MovieSet
    Public Event NeedsReload_TVEpisode() Implements Interfaces.IMasterSettingsPanel.NeedsReload_TVEpisode
    Public Event NeedsReload_TVShow() Implements Interfaces.IMasterSettingsPanel.NeedsReload_TVShow
    Public Event NeedsRestart() Implements Interfaces.IMasterSettingsPanel.NeedsRestart
    Public Event SettingsChanged() Implements Interfaces.IMasterSettingsPanel.SettingsChanged

#End Region 'Events 

#Region "Handles"

    Private Sub Handle_NeedsDBClean_Movie()
        RaiseEvent NeedsDBClean_Movie()
    End Sub

    Private Sub Handle_NeedsDBClean_TV()
        RaiseEvent NeedsDBClean_TV()
    End Sub

    Private Sub Handle_NeedsDBUpdate_Movie()
        RaiseEvent NeedsDBUpdate_Movie()
    End Sub

    Private Sub Handle_NeedsDBUpdate_TV()
        RaiseEvent NeedsDBUpdate_TV()
    End Sub

    Private Sub Handle_NeedsReload_Movie()
        RaiseEvent NeedsReload_Movie()
    End Sub

    Private Sub Handle_NeedsReload_MovieSet()
        RaiseEvent NeedsReload_MovieSet()
    End Sub

    Private Sub Handle_NeedsReload_TVEpisode()
        RaiseEvent NeedsReload_TVEpisode()
    End Sub

    Private Sub Handle_NeedsReload_TVShow()
        RaiseEvent NeedsReload_TVShow()
    End Sub

    Private Sub Handle_NeedsRestart()
        RaiseEvent NeedsRestart()
    End Sub

    Private Sub Handle_SettingsChanged()
        RaiseEvent SettingsChanged()
    End Sub

#End Region 'Handles

#Region "Constructors"

    Public Sub New()
        InitializeComponent()
        Setup()
    End Sub

#End Region 'Constructors 

#Region "Interface Methodes"

    Public Sub DoDispose() Implements Interfaces.IMasterSettingsPanel.DoDispose
        Dispose()
    End Sub

    Public Function InjectSettingsPanel() As Containers.SettingsPanel Implements Interfaces.IMasterSettingsPanel.InjectSettingsPanel
        Settings_Load()

        Return New Containers.SettingsPanel With {
            .Contains = Enums.SettingsPanelType.None,
            .ImageIndex = 12,
            .Order = 500,
            .Panel = pnlSettings,
            .SettingsPanelID = "Option_VideoSourceMapping",
            .Title = Master.eLang.GetString(784, "Video Source Mapping"),
            .Type = Enums.SettingsPanelType.Options
        }
    End Function

    Public Sub SaveSettings() Implements Interfaces.IMasterSettingsPanel.SaveSettings
        With Master.eSettings
            .GeneralVideoSourceByExtensionEnabled = chkByExtensionEnabled.Checked
            .GeneralVideoSourceByRegexEnabled = chkRegexEnabled.Checked
            Save_ByExtension()
            Save_ByRegex()
        End With
    End Sub

#End Region 'Interface Methodes

#Region "Methods"

    Public Sub Settings_Load()
        With Master.eSettings
            chkByExtensionEnabled.Checked = .GeneralVideoSourceByExtensionEnabled
            chkRegexEnabled.Checked = .GeneralVideoSourceByRegexEnabled

            DataGridView_Fill_ByExtension(.GeneralVideoSourceByExtension)
            DataGridView_Fill_ByRegex(.GeneralVideoSourceByRegex)
        End With
    End Sub

    Private Sub Setup()
        With Master.eLang
            btnByRegexDefaults.Text = .GetString(713, "Defaults")
            colByExtensionFileExtension.HeaderText = .GetString(775, "File Extension")
            colByExtensionVideoSource.HeaderText = .GetString(824, "Video Source")
            colByRegexRegex.HeaderText = .GetString(699, "Regex")
            colByRegexVideoSource.HeaderText = .GetString(824, "Video Source")
            gbByExtension.Text = .GetString(763, "Mapping by File Extension")
            gbByRegex.Text = .GetString(764, "Mapping by Regex")
        End With
    End Sub

    Private Sub Enable_ApplyButton() Handles _
        chkByExtensionEnabled.CheckedChanged,
        chkRegexEnabled.CheckedChanged,
        dgvByExtension.CellValueChanged,
        dgvByExtension.RowsAdded,
        dgvByExtension.RowsRemoved,
        dgvByRegex.CellValueChanged,
        dgvByRegex.RowsAdded,
        dgvByRegex.RowsRemoved

        Handle_SettingsChanged()
    End Sub

    Private Sub DataGridView_Fill_ByExtension(ByVal List As List(Of Settings.VideoSourceByExtension))
        dgvByExtension.Rows.Clear()
        For Each sett In List
            Dim i As Integer = dgvByExtension.Rows.Add(New Object() {
                                                       sett.Extension,
                                                       sett.VideoSource
                                                       })
        Next
        dgvByExtension.ClearSelection()
    End Sub

    Private Sub DataGridView_Fill_ByRegex(ByVal List As List(Of Settings.VideoSourceByRegex))
        dgvByRegex.Rows.Clear()
        For Each sett In List
            Dim i As Integer = dgvByRegex.Rows.Add(New Object() {
                                                   sett.Regexp,
                                                   sett.Videosource
                                                   })
        Next
        dgvByRegex.ClearSelection()
    End Sub

    Private Sub LoadDefaults_ByRegex(ByVal sender As Object, ByVal e As EventArgs) Handles btnByRegexDefaults.Click
        DataGridView_Fill_ByRegex(Master.eSettings.GetDefaultsForList_VideoSourceMappingsByRegex())
        Handle_SettingsChanged()
    End Sub

    Private Sub Save_ByExtension()
        Master.eSettings.GeneralVideoSourceByExtension.Clear()
        For Each r As DataGridViewRow In dgvByExtension.Rows
            If r.Cells(0).Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(r.Cells(0).Value.ToString) Then
                Master.eSettings.GeneralVideoSourceByExtension.Add(New Settings.VideoSourceByExtension With {
                                                                   .Extension = r.Cells(0).Value.ToString,
                                                                   .VideoSource = If(r.Cells(1).Value IsNot Nothing, r.Cells(1).Value.ToString, String.Empty)
                                                                   })
            End If
        Next
    End Sub

    Private Sub Save_ByRegex()
        Master.eSettings.GeneralVideoSourceByRegex.Clear()
        For Each r As DataGridViewRow In dgvByRegex.Rows
            If r.Cells(0).Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(r.Cells(0).Value.ToString) Then
                Master.eSettings.GeneralVideoSourceByRegex.Add(New Settings.VideoSourceByRegex With {
                                                               .Regexp = r.Cells(0).Value.ToString,
                                                               .Videosource = If(r.Cells(1).Value IsNot Nothing, r.Cells(1).Value.ToString, String.Empty)
                                                               })
            End If
        Next
    End Sub

#End Region 'Methods

End Class