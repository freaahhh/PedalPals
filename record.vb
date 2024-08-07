﻿Imports System.Data.OleDb

Public Class Record
    Dim connection As New OleDbConnection(My.Settings.dataConnectionString)
    Private Sub RentalStatBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Me.Validate()
        Me.RentalBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.PedalPalsDBDataSet1)
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim rentID As Integer = Integer.Parse(txtBookID.Text)
        Dim status As String = ddStatus.SelectedItem.ToString()

        UpdateRentStatus(rentID, status)
    End Sub

    Private Sub Record_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load data into the DataGridView on form load
        LoadRentalData()

        connection.Open()
        Dim cmdAdd As New OleDbCommand("SELECT address FROM location", connection)
        Using readerAdd As OleDbDataReader = cmdAdd.ExecuteReader()
            ' Add "None" option
            ddLocation.Items.Add("None")
            While readerAdd.Read()
                ddLocation.Items.Add(readerAdd("address").ToString())
            End While
        End Using

        connection.Close()
    End Sub

    Private Sub UpdateRentStatus(rentID As Integer, status As String)
        ' Adjust the connection string to match your settings
        Using connection As New OleDbConnection(My.Settings.dataConnectionString)
            connection.Open()
            Dim query As String = "UPDATE Rental SET rent_status = ? WHERE rent_ID = ?"
            Using command As New OleDbCommand(query, connection)
                command.Parameters.AddWithValue("?", status)
                command.Parameters.AddWithValue("?", rentID)

                Dim rowsAffected As Integer = command.ExecuteNonQuery()
                If rowsAffected > 0 Then
                    MsgBox("Rent status updated successfully.", MsgBoxStyle.Information)
                    ' Refresh the DataGridView to reflect the update
                    LoadRentalData()
                Else
                    MsgBox("Failed to update rent status.", MsgBoxStyle.Exclamation)
                End If
            End Using
        End Using
    End Sub

    Private Sub LoadRentalData()
        ' Load data into the DataGridView
        Using connection As New OleDbConnection(My.Settings.dataConnectionString)
            connection.Open()
            Dim query As String = "SELECT * FROM Rental ORDER BY rent_id DESC;"
            Using command As New OleDbCommand(query, connection)
                Using adapter As New OleDbDataAdapter(command)
                    Dim rentalData As New DataTable()
                    adapter.Fill(rentalData)
                    RentalDataGridView.DataSource = rentalData
                End Using
            End Using
        End Using
    End Sub

    Private Sub RentalBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs) Handles RentalBindingNavigatorSaveItem.Click
        Me.Validate()
        Me.RentalBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.PedalPalsDBDataSet1)

    End Sub

    Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        ' Retrieve selected place
        Dim selectedPlace As String = ddLocation.SelectedItem.ToString()

        ' Check if "None" is selected
        If selectedPlace = "None" Then
            ' Retrieve all data
            LoadRentalData()
        Else
            ' Open the connection
            connection.Open()

            ' Get the location_id for the selected place
            Dim placeID As Integer
            Using cmdPlaceID As New OleDbCommand("SELECT location_id FROM location WHERE address = ?", connection)
                cmdPlaceID.Parameters.AddWithValue("?", selectedPlace)
                placeID = Convert.ToInt32(cmdPlaceID.ExecuteScalar())
            End Using

            ' Filter the Rental data based on the location_id
            Using cmdFilter As New OleDbCommand("SELECT * FROM Rental WHERE location_id = ? ORDER BY rent_id DESC;", connection)
                cmdFilter.Parameters.AddWithValue("?", placeID)

                Using adapter As New OleDbDataAdapter(cmdFilter)
                    Dim rentalData As New DataTable()
                    adapter.Fill(rentalData)
                    RentalDataGridView.DataSource = rentalData
                End Using
            End Using

            ' Close the connection
            connection.Close()
        End If
    End Sub


End Class
