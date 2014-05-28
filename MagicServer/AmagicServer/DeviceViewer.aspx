
<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeviceViewer.aspx.cs" Inherits="AmagicServer.DeviceViewer"  %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="FeaturedContent">

</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    <h2>
        Device List </h2>
    <div class="accountInfo">
        <br />
        <asp:TextBox ID="tbCriteia" runat="server"></asp:TextBox>

        <br />

        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
        <br />
        <asp:GridView ID="dvDevice" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
            Width="972px" AutoGenerateColumns="False" >
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                
                <asp:BoundField DataField="PhoneSN" HeaderText="Phone SN." />

             
            
                  <asp:BoundField DataField="PaypalStatus" HeaderText="Server Status" />
                   <asp:BoundField DataField="RecordDate" HeaderText="Puchased Data" />
       
                     <asp:BoundField DataField="PaymentId" HeaderText="Payment Id" />
                        <asp:BoundField DataField="State" HeaderText="PayPal State" />
                         <asp:BoundField DataField="PayPayDate" HeaderText="PayPal Date" />
                  <asp:BoundField DataField="ProductType" HeaderText="Product" />
          
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
    </div>
    <p>
        &nbsp;</p>
</asp:Content>

