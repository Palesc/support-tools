<%@ Page Title="BinStorage" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BinStorage.aspx.vb" Inherits="SupportTools.BinStorage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="Content/BinStorage.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:RadioButtonList ID="rblAddDeleteFind" runat="server" CssClass="d-flex justify-content-around mt-3 mb-2"
        RepeatDirection="Horizontal" AutoPostBack="true">
        <asp:ListItem Text="Add Item" Value="Add" Selected="true" style="margin-right: 2em;" />
        <asp:ListItem Text="Find Item" Value="Find" />
    </asp:RadioButtonList>
    <asp:Panel ID="pnlAdd" runat="server" Visible="true">
        <div class="text-center col-2 m-auto" style="font-size: 15px;">
            <div class="input-group mt-3">
                <asp:Label Text="Select A Storage Bin" runat="server" CssClass="input-group-text" />
                <asp:DropDownList ID="ddlBinLocationAdd" runat="server" CssClass="form-control form-select"
                    DataSourceID="dsBinNames" DataTextField="ddlText" DataValueField="ddlValue" />
            </div>
            <div>
                <asp:RequiredFieldValidator runat="server" ErrorMessage="Must Select A Storage Bin" ControlToValidate="ddlBinLocationAdd"
                    InitialValue="-999" ValidationGroup="Add" ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-floating mt-3">
                <asp:TextBox ID="txtDescr" runat="server" TextMode="MultiLine" Height="100px" CssClass="form-control" />
                <label for="txtDescr">Description</label>
            </div>
            <div>
                <asp:RequiredFieldValidator runat="server" ErrorMessage="Must Have A Description" ControlToValidate="txtDescr"
                    ValidationGroup="Add" ForeColor="Red" Display="Dynamic" />
            </div>
        </div>
        <asp:CheckBoxList ID="cblCategoriesAdd" runat="server" RepeatColumns="7" 
            CssClass="d-flex justify-content-center mt-3 checkbox-style" />
        <div class="text-center mt-4">
            <asp:Button ID="btnAdd" Text="Submit" runat="server" CssClass="btn btn-primary" ValidationGroup="Add" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlFind" CssClass="container-fluid col-10 m-auto" runat="server" Visible="false">
        <div class="row col-10 m-auto">
            <div class="col-8">
                <asp:CheckBoxList ID="cblCategoriesUpdate" runat="server" RepeatColumns="7"
                    CssClass="d-flex justify-content-center mt-3 checkbox-style" />
            </div>
            <div class="col d-flex flex-column justify-content-around">
                <div class="input-group">
                    <asp:Label Text="Select A Storage Bin" runat="server" CssClass="input-group-text" />
                    <asp:DropDownList ID="ddlBinLocationUpdate" runat="server" CssClass="form-control form-select"
                        DataSourceID="dsBinNames" DataTextField="ddlText" DataValueField="ddlValue" />
                </div>
                <div class="input-group">
                    <asp:Label runat="server" Text="Key-Word" CssClass="input-group-text" />
                    <asp:TextBox ID="txtKeyWord" runat="server" CssClass="form-control" />
                </div>
                <div class="text-center">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
        <div class="mt-4" style="max-height: 61em; overflow: auto; max-width: inherit;">
            <asp:GridView ID="gvUpdate" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered" EmptyDataText="No Items Match Your Search Criteria">
                <Columns>
                    <asp:TemplateField HeaderText="Tools" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center">
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="Edit" CssClass="btn btn-primary" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="Update" CssClass="btn btn-sm btn-primary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" CssClass="btn btn-sm btn-info" />
                            <asp:HiddenField ID="hdnBinStorageID" runat="server" Value='<%# Bind("BinStorageID") %>' />
                            <asp:HiddenField ID="hdnBinNameID" runat="server" Value='<%# Bind("BinNameID") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Bin Name">
                        <ItemTemplate>
                            <asp:Label ID="lblBinName" runat="server" Text='<%# Bind("BinName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlGvBinLocation" runat="server" CssClass="form-control form-select"
                                DataSourceID="dsBinNames" DataTextField="ddlText" DataValueField="ddlValue" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item Description">
                        <ItemTemplate>
                            <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval("ItemDescription") %>' />
                            <asp:HiddenField ID="hdnBinStorageID" runat="server" Value='<%# Bind("BinStorageID") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtItemDescription" runat="server" Text='<%# Bind("ItemDescription") %>' TextMode="MultiLine" Rows="3" CssClass="form-control" />
                            <div class="text-center">
                                <asp:RequiredFieldValidator ErrorMessage="Cannot be empty" ControlToValidate="txtItemDescription" runat="server"
                                    Display="Dynamic" ForeColor="Red" />
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Categories">
                        <ItemTemplate>
                            <asp:Label ID="lblCategories" runat="server" Text='<%# Bind("Categories") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBoxList ID="gvCBL" runat="server" RepeatColumns="7"
                                CssClass="d-flex justify-content-center mt-3 checkbox-style" />
                            <asp:HiddenField ID="hdnCategories" runat="server" Value='<%# Bind("Categories") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
<%--                                        <asp:CommandField ShowDeleteButton="true" ControlStyle-CssClass="btn btn-danger" />--%>
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
    <asp:SqlDataSource ID="dsBinNames" runat="server" ConnectionString="<%$ ConnectionStrings:DBCS %>"
        SelectCommandType="StoredProcedure" SelectCommand="spBinStorageBinNameDDL" />
</asp:Content>
