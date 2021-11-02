namespace RioValleyChili.Client.Reporting.Reports
{
    partial class CustomerNotesSubReport
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.detail = new Telerik.Reporting.DetailSection();
            this.TableCustomerNotes = new Telerik.Reporting.Table();
            this.CustomerPanel = new Telerik.Reporting.Panel();
            this.TableCustomerNotes_Name = new Telerik.Reporting.TextBox();
            this.TableCustomerNotes_TableNotes = new Telerik.Reporting.Table();
            this.TableCustomerNotes_TableNotes_Type = new Telerik.Reporting.TextBox();
            this.TableCustomerNotes_TableNotes_Text = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.40004047751426697D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.TableCustomerNotes});
            this.detail.KeepTogether = false;
            this.detail.Name = "detail";
            // 
            // TableCustomerNotes
            // 
            this.TableCustomerNotes.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(7.4999585151672363D)));
            this.TableCustomerNotes.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.40000113844871521D)));
            this.TableCustomerNotes.Body.SetCellContent(0, 0, this.CustomerPanel);
            tableGroup4.Name = "tableGroup3";
            this.TableCustomerNotes.ColumnGroups.Add(tableGroup4);
            this.TableCustomerNotes.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.CustomerPanel});
            this.TableCustomerNotes.KeepTogether = false;
            this.TableCustomerNotes.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.TableCustomerNotes.Name = "TableCustomerNotes";
            tableGroup5.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup5.Name = "detailTableGroup1";
            this.TableCustomerNotes.RowGroups.Add(tableGroup5);
            this.TableCustomerNotes.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4999585151672363D), Telerik.Reporting.Drawing.Unit.Inch(0.40000113844871521D));
            // 
            // CustomerPanel
            // 
            this.CustomerPanel.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.TableCustomerNotes_Name,
            this.TableCustomerNotes_TableNotes});
            this.CustomerPanel.KeepTogether = false;
            this.CustomerPanel.Name = "CustomerPanel";
            this.CustomerPanel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4999585151672363D), Telerik.Reporting.Drawing.Unit.Inch(0.40000113844871521D));
            this.CustomerPanel.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.CustomerPanel.StyleName = "";
            // 
            // TableCustomerNotes_Name
            // 
            this.TableCustomerNotes_Name.CanGrow = false;
            this.TableCustomerNotes_Name.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.TableCustomerNotes_Name.Name = "TableCustomerNotes_Name";
            this.TableCustomerNotes_Name.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4999194145202637D), Telerik.Reporting.Drawing.Unit.Inch(0.20004017651081085D));
            this.TableCustomerNotes_Name.Style.Font.Bold = true;
            this.TableCustomerNotes_Name.Style.Font.Italic = true;
            this.TableCustomerNotes_Name.Value = "";
            // 
            // TableCustomerNotes_TableNotes
            // 
            this.TableCustomerNotes_TableNotes.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4268556833267212D)));
            this.TableCustomerNotes_TableNotes.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(5.8730630874633789D)));
            this.TableCustomerNotes_TableNotes.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19984243810176849D)));
            this.TableCustomerNotes_TableNotes.Body.SetCellContent(0, 0, this.TableCustomerNotes_TableNotes_Type);
            this.TableCustomerNotes_TableNotes.Body.SetCellContent(0, 1, this.TableCustomerNotes_TableNotes_Text);
            tableGroup1.Name = "tableGroup4";
            tableGroup2.Name = "tableGroup5";
            this.TableCustomerNotes_TableNotes.ColumnGroups.Add(tableGroup1);
            this.TableCustomerNotes_TableNotes.ColumnGroups.Add(tableGroup2);
            this.TableCustomerNotes_TableNotes.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.TableCustomerNotes_TableNotes_Type,
            this.TableCustomerNotes_TableNotes_Text});
            this.TableCustomerNotes_TableNotes.KeepTogether = false;
            this.TableCustomerNotes_TableNotes.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D), Telerik.Reporting.Drawing.Unit.Inch(0.20015843212604523D));
            this.TableCustomerNotes_TableNotes.Name = "TableCustomerNotes_TableNotes";
            tableGroup3.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup3.Name = "detailTableGroup2";
            this.TableCustomerNotes_TableNotes.RowGroups.Add(tableGroup3);
            this.TableCustomerNotes_TableNotes.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2999191284179688D), Telerik.Reporting.Drawing.Unit.Inch(0.19984243810176849D));
            // 
            // TableCustomerNotes_TableNotes_Type
            // 
            this.TableCustomerNotes_TableNotes_Type.KeepTogether = true;
            this.TableCustomerNotes_TableNotes_Type.Name = "TableCustomerNotes_TableNotes_Type";
            this.TableCustomerNotes_TableNotes_Type.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4268556833267212D), Telerik.Reporting.Drawing.Unit.Inch(0.19984243810176849D));
            this.TableCustomerNotes_TableNotes_Type.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.TableCustomerNotes_TableNotes_Type.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(10D);
            // 
            // TableCustomerNotes_TableNotes_Text
            // 
            this.TableCustomerNotes_TableNotes_Text.KeepTogether = true;
            this.TableCustomerNotes_TableNotes_Text.Name = "TableCustomerNotes_TableNotes_Text";
            this.TableCustomerNotes_TableNotes_Text.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.8730630874633789D), Telerik.Reporting.Drawing.Unit.Inch(0.19984243810176849D));
            this.TableCustomerNotes_TableNotes_Text.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.TableCustomerNotes_TableNotes_Text.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(10D);
            // 
            // CustomerNotesSubReport
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "CustomerNotesSubReport";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.5D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.Table TableCustomerNotes;
        private Telerik.Reporting.Panel CustomerPanel;
        private Telerik.Reporting.TextBox TableCustomerNotes_Name;
        private Telerik.Reporting.Table TableCustomerNotes_TableNotes;
        private Telerik.Reporting.TextBox TableCustomerNotes_TableNotes_Type;
        private Telerik.Reporting.TextBox TableCustomerNotes_TableNotes_Text;
    }
}