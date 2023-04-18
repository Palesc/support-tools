Imports System.Data.SqlClient
Imports System.Linq.Expressions
Imports System.Security.Cryptography
Imports System.Web.Services.Description
Imports Microsoft.Ajax.Utilities

Public Class BinStorage
    Inherits System.Web.UI.Page

#Region "PageLoag"
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            PopulateAddCategoryCheckboxList()
            PopulateUpdateCategoryCheckboxList()
        End If
    End Sub
#End Region
#Region "Add Logic"
    Private Sub PopulateAddCategoryCheckboxList()
        CreateCheckBoxList(cblCategoriesAdd)
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("DBCS").ConnectionString)
            Using cmd As New SqlCommand("spBinStorageInsert", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@TblCategoryId", CreateCategoryTable(cblCategoriesAdd))
                cmd.Parameters.AddWithValue("@BinNameId", ddlBinLocationAdd.SelectedValue)
                cmd.Parameters.AddWithValue("@Descr", Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtDescr.Text))

                cmd.Parameters.Add("@Msg", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@RetVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                Dim ds As New DataSet
                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                    conn.Close()

                Catch ex As Exception
                    ShowAlert(ex.Message)
                Finally
                    If conn.State <> ConnectionState.Closed Then
                        conn.Close()
                    End If
                    cmd.Dispose()
                    conn.Dispose()
                End Try
            End Using
        End Using
        ClearAddPanel()
    End Sub

    Private Sub ClearAddPanel()
        For Each i As ListItem In cblCategoriesAdd.Items
            i.Selected = False
        Next
        ddlBinLocationAdd.SelectedIndex = 0
        txtDescr.Text = ""
    End Sub

#End Region
#Region "Update Logic"
    Private Sub PopulateUpdateCategoryCheckboxList()
        CreateCheckBoxList(cblCategoriesUpdate)
    End Sub
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        BindGridView()
    End Sub
    Private Sub BindGridView()
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("DBCS").ConnectionString)
            Using cmd As New SqlCommand("spBinStorageGridView", conn)
                cmd.CommandType = CommandType.StoredProcedure
                Dim dt As DataTable = CreateCategoryTable(cblCategoriesUpdate)
                cmd.Parameters.AddWithValue("@TblCategoryId", CreateCategoryTable(cblCategoriesUpdate))
                cmd.Parameters.AddWithValue("@BinNameId", ddlBinLocationUpdate.SelectedValue)
                cmd.Parameters.AddWithValue("@KeyWord", txtKeyWord.Text)

                Dim ds As New DataSet
                Try
                    conn.Open()
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(ds)
                        da.Dispose()
                    End Using
                    conn.Close()

                    gvUpdate.DataSource = ds
                    gvUpdate.DataBind()
                Catch ex As Exception
                    ShowAlert(ex.Message)
                Finally
                    If conn.State <> ConnectionState.Closed Then
                        conn.Close()
                    End If
                    cmd.Dispose()
                    conn.Dispose()
                End Try
            End Using
        End Using
    End Sub
#End Region
#Region "Radio Button List"
    Private Sub rblAddDeleteFind_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblAddDeleteFind.SelectedIndexChanged
        Select Case rblAddDeleteFind.SelectedValue
            Case "Add"
                pnlAdd.Visible = True
                pnlFind.Visible = False
            Case "Find"
                pnlFind.Visible = True
                pnlAdd.Visible = False
                BindGridView()
        End Select
    End Sub
#End Region
#Region "GridView"
    Private Sub gvUpdate_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles gvUpdate.RowEditing
        gvUpdate.EditIndex = e.NewEditIndex

        BindGridView()

        For Each row As GridViewRow In gvUpdate.Rows
            Dim btnEdit As Button = CType(row.FindControl("btnEdit"), Button)
            If btnEdit IsNot Nothing Then
                If row.RowIndex <> e.NewEditIndex Then
                    btnEdit.Visible = False
                End If
            End If
        Next

        Dim gvDDL As DropDownList = gvUpdate.Rows(e.NewEditIndex).FindControl("ddlGvBinLocation")
        Dim BinNameID As Integer = CType(gvUpdate.Rows(e.NewEditIndex).FindControl("hdnBinNameID"), HiddenField).Value

        gvDDL.SelectedValue = BinNameID
    End Sub

    Protected Sub gvUpdate_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvUpdate.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow AndAlso gvUpdate.EditIndex = e.Row.RowIndex Then

            Dim gvCBL As CheckBoxList = CType(e.Row.FindControl("gvCBL"), CheckBoxList)

            CreateCheckBoxList(gvCBL)

            Dim categories As String = CType(e.Row.FindControl("hdnCategories"), HiddenField).Value

            For Each item As ListItem In gvCBL.Items
                Dim category As String = item.Text
                Dim categoriesArray = categories.Split(","c).Select(Function(x) x.Trim()).ToArray()
                If categoriesArray.Contains(category) Then
                    item.Selected = True
                End If
            Next
        End If
    End Sub

    Private Sub gvUpdate_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles gvUpdate.RowUpdating
        Dim rowIndex As Integer = e.RowIndex

        Dim Descr As String = CType(gvUpdate.Rows(rowIndex).FindControl("txtItemDescription"), TextBox).Text
        Dim binStorageID As Integer = CType(gvUpdate.Rows(e.RowIndex).FindControl("hdnBinStorageID"), HiddenField).Value
        Dim binNameID As Integer = CType(gvUpdate.Rows(e.RowIndex).FindControl("ddlGvBinLocation"), DropDownList).SelectedValue
        Dim gvCBL As CheckBoxList = gvUpdate.Rows(e.RowIndex).FindControl("gvCBL")

        UpdateBinStorage(binStorageID, binNameID, Descr, CreateCategoryTable(gvCBL))

        gvUpdate.EditIndex = -1
        BindGridView() ' call a method to bind the GridView data again
    End Sub

    Private Sub gvUpdate_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvUpdate.RowDeleting
        Dim rowIndex As Integer = e.RowIndex
        Dim binStorageID As Integer = CType(gvUpdate.Rows(e.RowIndex).FindControl("hdnBinStorageID"), HiddenField).Value

        DeleteBinStorage(binStorageID)

        BindGridView() ' call a method to bind the GridView data again
    End Sub

    Private Sub gvUpdate_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles gvUpdate.RowCancelingEdit
        gvUpdate.EditIndex = -1

        BindGridView()
    End Sub
#End Region
#Region "Supporting Subs and Functions"
    Private Function CreateCategoryTable(cbl As CheckBoxList) As DataTable
        Dim dt As New DataTable

        dt.Columns.Add("CategoryId", GetType(String))

        For Each i As ListItem In cbl.Items
            If i.Selected Then
                dt.Rows.Add(i.Value)
            End If
        Next

        Return dt
    End Function

    Private Sub CreateCheckBoxList(cbl As CheckBoxList)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("DBCS").ConnectionString)
            Using cmd As New SqlCommand("spBinStoragePopulateCategoryChecklist", conn)
                cmd.CommandType = CommandType.StoredProcedure

                Try
                    conn.Open()
                    Dim reader As SqlDataReader = cmd.ExecuteReader

                    cbl.DataSource = reader
                    cbl.DataTextField = "chkText"
                    cbl.DataValueField = "chkValue"
                    cbl.DataBind()
                    reader.Close()
                    reader.Dispose()
                    conn.Close()
                Catch ex As Exception
                    ShowAlert(ex.Message)
                Finally
                    If conn.State <> ConnectionState.Closed Then
                        conn.Close()
                    End If
                    cmd.Dispose()
                    conn.Dispose()
                End Try
            End Using
        End Using
    End Sub

    Private Sub ShowAlert(message As String)
        Dim script As String = "alert(""" + message + """);"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "DeleteBinStorageError", script, True)
    End Sub

    Private Sub UpdateBinStorage(binStorageId As Integer, binNameId As Integer, Descr As String, Categories As DataTable)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("DBCS").ConnectionString)
            Using cmd As New SqlCommand("spBinStorageUpdate", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@TblCategoryId", Categories)
                cmd.Parameters.AddWithValue("@BinStorageId", binStorageId)
                cmd.Parameters.AddWithValue("@BinNameId", binNameId)
                cmd.Parameters.AddWithValue("@Descr", Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Descr))

                cmd.Parameters.Add("@Msg", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@RetVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                Dim ds As New DataSet
                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                    conn.Close()
                Catch ex As Exception
                    ShowAlert(ex.Message)
                Finally
                    If conn.State <> ConnectionState.Closed Then
                        conn.Close()
                    End If
                    cmd.Dispose()
                    conn.Dispose()
                End Try
            End Using
        End Using
    End Sub

    Private Sub DeleteBinStorage(BinStorageId As Integer)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("DBCS").ConnectionString)
            Using cmd As New SqlCommand("spBinStorageDelete", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@BinStorageId", BinStorageId)

                cmd.Parameters.Add("@Msg", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@RetVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                    conn.Close()
                Catch ex As Exception
                    ShowAlert(ex.Message)
                Finally
                    If conn.State <> ConnectionState.Closed Then
                        conn.Close()
                    End If
                    cmd.Dispose()
                    conn.Dispose()
                End Try
            End Using
        End Using
    End Sub
#End Region

End Class