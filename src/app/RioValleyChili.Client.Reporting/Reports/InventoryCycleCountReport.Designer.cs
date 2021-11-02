namespace RioValleyChili.Client.Reporting.Reports
{
    partial class InventoryCycleCountReport
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.facilitiesDataSource = new Telerik.Reporting.ObjectDataSource();
            this.locationGroupNamesDataSource = new Telerik.Reporting.ObjectDataSource();
            this.txtLocation = new Telerik.Reporting.TextBox();
            this.tblInventory = new Telerik.Reporting.Table();
            this.txtLot = new Telerik.Reporting.TextBox();
            this.txtLotDate = new Telerik.Reporting.TextBox();
            this.txtProduct = new Telerik.Reporting.TextBox();
            this.txtPackaging = new Telerik.Reporting.TextBox();
            this.txtTrmt = new Telerik.Reporting.TextBox();
            this.txtQuantity = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.LabelTitle = new Telerik.Reporting.TextBox();
            this.txtHeaderTimestamp = new Telerik.Reporting.TextBox();
            this.txtFacility = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.txtGroupName = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.txtGrandTotal = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.txtFooterDate = new Telerik.Reporting.TextBox();
            this.txtPageCount = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.tblHeaderLocations = new Telerik.Reporting.Table();
            this.txtHeaderLocationWeight = new Telerik.Reporting.TextBox();
            this.txtHeaderLocation = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.txtHeaderGroupWeight = new Telerik.Reporting.TextBox();
            this.dataSource = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0297750234603882D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox2.Value = "Lot";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.854361891746521D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Value = "Lot Date";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.4560933113098145D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox8.StyleName = "";
            this.textBox8.Value = "Product";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6103278398513794D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox10.StyleName = "";
            this.textBox10.Value = "Packaging";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.43999671936035156D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox12.StyleName = "";
            this.textBox12.Value = "Trtmt";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4054452180862427D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox14.StyleName = "";
            this.textBox14.Value = "Quantity";
            // 
            // facilitiesDataSource
            // 
            this.facilitiesDataSource.DataMember = "GetFacilities";
            this.facilitiesDataSource.DataSource = typeof(RioValleyChili.Client.Reporting.Services.InventoryReportingService);
            this.facilitiesDataSource.Name = "facilitiesDataSource";
            // 
            // locationGroupNamesDataSource
            // 
            this.locationGroupNamesDataSource.DataMember = "GetLocationGroupsNames";
            this.locationGroupNamesDataSource.DataSource = typeof(RioValleyChili.Client.Reporting.Services.InventoryReportingService);
            this.locationGroupNamesDataSource.Name = "locationGroupNamesDataSource";
            this.locationGroupNamesDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("facilityKey", typeof(string), "= Parameters.FacilityKey.Value")});
            // 
            // txtLocation
            // 
            this.txtLocation.CanGrow = false;
            this.txtLocation.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.05000000074505806D));
            this.txtLocation.Multiline = true;
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.3999605178833008D), Telerik.Reporting.Drawing.Unit.Inch(0.1999211311340332D));
            this.txtLocation.Style.Color = System.Drawing.Color.Blue;
            this.txtLocation.Style.Font.Bold = true;
            this.txtLocation.Style.Font.Italic = false;
            this.txtLocation.Style.Font.Name = "Tahoma";
            this.txtLocation.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.txtLocation.Value = "A03";
            // 
            // tblInventory
            // 
            this.tblInventory.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0297749042510986D)));
            this.tblInventory.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.85436183214187622D)));
            this.tblInventory.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.4560933113098145D)));
            this.tblInventory.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6103278398513794D)));
            this.tblInventory.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.43999671936035156D)));
            this.tblInventory.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4054453372955322D)));
            this.tblInventory.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.tblInventory.Body.SetCellContent(0, 0, this.txtLot);
            this.tblInventory.Body.SetCellContent(0, 1, this.txtLotDate);
            this.tblInventory.Body.SetCellContent(0, 2, this.txtProduct);
            this.tblInventory.Body.SetCellContent(0, 3, this.txtPackaging);
            this.tblInventory.Body.SetCellContent(0, 4, this.txtTrmt);
            this.tblInventory.Body.SetCellContent(0, 5, this.txtQuantity);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox2;
            tableGroup2.Name = "tableGroup1";
            tableGroup2.ReportItem = this.textBox4;
            tableGroup3.Name = "group";
            tableGroup3.ReportItem = this.textBox8;
            tableGroup4.Name = "group1";
            tableGroup4.ReportItem = this.textBox10;
            tableGroup5.Name = "group2";
            tableGroup5.ReportItem = this.textBox12;
            tableGroup6.Name = "group3";
            tableGroup6.ReportItem = this.textBox14;
            this.tblInventory.ColumnGroups.Add(tableGroup1);
            this.tblInventory.ColumnGroups.Add(tableGroup2);
            this.tblInventory.ColumnGroups.Add(tableGroup3);
            this.tblInventory.ColumnGroups.Add(tableGroup4);
            this.tblInventory.ColumnGroups.Add(tableGroup5);
            this.tblInventory.ColumnGroups.Add(tableGroup6);
            this.tblInventory.ColumnHeadersPrintOnEveryPage = true;
            this.tblInventory.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtLot,
            this.txtLotDate,
            this.txtProduct,
            this.txtPackaging,
            this.txtTrmt,
            this.txtQuantity,
            this.textBox2,
            this.textBox4,
            this.textBox8,
            this.textBox10,
            this.textBox12,
            this.textBox14});
            this.tblInventory.KeepTogether = false;
            this.tblInventory.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.20003938674926758D), Telerik.Reporting.Drawing.Unit.Inch(0.29999995231628418D));
            this.tblInventory.Name = "tblInventory";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detailTableGroup";
            this.tblInventory.RowGroups.Add(tableGroup7);
            this.tblInventory.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.7960000038146973D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            // 
            // txtLot
            // 
            this.txtLot.Name = "txtLot";
            this.txtLot.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0297750234603882D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtLot.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtLot.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtLot.Style.Color = System.Drawing.Color.Red;
            this.txtLot.Value = "02 15 301 15";
            // 
            // txtLotDate
            // 
            this.txtLotDate.Format = "{0:MM/dd/yyyy}";
            this.txtLotDate.Name = "txtLotDate";
            this.txtLotDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.854361891746521D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtLotDate.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtLotDate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtLotDate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtLotDate.Value = "10/28/2015";
            // 
            // txtProduct
            // 
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.4560933113098145D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtProduct.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtProduct.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtProduct.StyleName = "";
            this.txtProduct.Value = "Chili Pepper Base";
            // 
            // txtPackaging
            // 
            this.txtPackaging.Name = "txtPackaging";
            this.txtPackaging.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6103278398513794D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtPackaging.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtPackaging.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtPackaging.StyleName = "";
            this.txtPackaging.Value = "900 lb Bin";
            // 
            // txtTrmt
            // 
            this.txtTrmt.Name = "txtTrmt";
            this.txtTrmt.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.43999671936035156D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtTrmt.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtTrmt.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtTrmt.StyleName = "";
            this.txtTrmt.Value = "ET";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Format = "{0:N0}";
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4054452180862427D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtQuantity.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtQuantity.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtQuantity.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtQuantity.StyleName = "";
            this.txtQuantity.Value = "5";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.51674550771713257D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.LabelTitle,
            this.txtHeaderTimestamp,
            this.txtFacility,
            this.textBox3,
            this.textBox5,
            this.txtGroupName});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // LabelTitle
            // 
            this.LabelTitle.CanGrow = false;
            this.LabelTitle.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.LabelTitle.Multiline = true;
            this.LabelTitle.Name = "LabelTitle";
            this.LabelTitle.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2999603748321533D), Telerik.Reporting.Drawing.Unit.Inch(0.51670616865158081D));
            this.LabelTitle.Style.Color = System.Drawing.Color.Black;
            this.LabelTitle.Style.Font.Bold = true;
            this.LabelTitle.Style.Font.Italic = false;
            this.LabelTitle.Style.Font.Name = "Tahoma";
            this.LabelTitle.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.LabelTitle.Value = "Rio Valley Chili, Inc.\r\nInventory Cycle Count";
            // 
            // txtHeaderTimestamp
            // 
            this.txtHeaderTimestamp.CanGrow = false;
            this.txtHeaderTimestamp.Format = "{0:d/M/yyyy H:mm:ss tt}";
            this.txtHeaderTimestamp.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.8000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.txtHeaderTimestamp.Multiline = true;
            this.txtHeaderTimestamp.Name = "txtHeaderTimestamp";
            this.txtHeaderTimestamp.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1999988555908203D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtHeaderTimestamp.Style.Color = System.Drawing.Color.Black;
            this.txtHeaderTimestamp.Style.Font.Bold = false;
            this.txtHeaderTimestamp.Style.Font.Italic = false;
            this.txtHeaderTimestamp.Style.Font.Name = "Tahoma";
            this.txtHeaderTimestamp.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.txtHeaderTimestamp.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtHeaderTimestamp.Value = "";
            // 
            // txtFacility
            // 
            this.txtFacility.CanGrow = false;
            this.txtFacility.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5000789165496826D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.txtFacility.Multiline = true;
            this.txtFacility.Name = "txtFacility";
            this.txtFacility.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2998428344726562D), Telerik.Reporting.Drawing.Unit.Inch(0.25833335518836975D));
            this.txtFacility.Style.Color = System.Drawing.Color.Black;
            this.txtFacility.Style.Font.Bold = true;
            this.txtFacility.Style.Font.Italic = false;
            this.txtFacility.Style.Font.Name = "Tahoma";
            this.txtFacility.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.txtFacility.Value = "";
            // 
            // textBox3
            // 
            this.textBox3.CanGrow = false;
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.3000786304473877D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999211311340332D), Telerik.Reporting.Drawing.Unit.Inch(0.25833335518836975D));
            this.textBox3.Style.Color = System.Drawing.Color.Black;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Italic = false;
            this.textBox3.Style.Font.Name = "Tahoma";
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox3.Value = "Facility:";
            // 
            // textBox5
            // 
            this.textBox5.CanGrow = false;
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.3000786304473877D), Telerik.Reporting.Drawing.Unit.Inch(0.25841212272644043D));
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999211311340332D), Telerik.Reporting.Drawing.Unit.Inch(0.25833335518836975D));
            this.textBox5.Style.Color = System.Drawing.Color.Black;
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Font.Italic = false;
            this.textBox5.Style.Font.Name = "Tahoma";
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox5.Value = "Group:";
            // 
            // txtGroupName
            // 
            this.txtGroupName.CanGrow = false;
            this.txtGroupName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(0.25841212272644043D));
            this.txtGroupName.Multiline = true;
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2999217510223389D), Telerik.Reporting.Drawing.Unit.Inch(0.25833335518836975D));
            this.txtGroupName.Style.Color = System.Drawing.Color.Black;
            this.txtGroupName.Style.Font.Bold = true;
            this.txtGroupName.Style.Font.Italic = false;
            this.txtGroupName.Style.Font.Name = "Tahoma";
            this.txtGroupName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.txtGroupName.Value = "";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.983254611492157D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblInventory,
            this.txtLocation,
            this.textBox9,
            this.txtGrandTotal});
            this.detail.KeepTogether = false;
            this.detail.Name = "detail";
            this.detail.PageBreak = Telerik.Reporting.PageBreak.Before;
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2212862968444824D), Telerik.Reporting.Drawing.Unit.Inch(0.78325456380844116D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3893368244171143D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox9.StyleName = "";
            this.textBox9.Value = "Grand Total";
            // 
            // txtGrandTotal
            // 
            this.txtGrandTotal.Format = "{0:N0}";
            this.txtGrandTotal.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.6107010841369629D), Telerik.Reporting.Drawing.Unit.Inch(0.78325456380844116D));
            this.txtGrandTotal.Name = "txtGrandTotal";
            this.txtGrandTotal.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3893368244171143D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtGrandTotal.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtGrandTotal.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtGrandTotal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtGrandTotal.StyleName = "";
            this.txtGrandTotal.Value = "";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtFooterDate,
            this.txtPageCount});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // txtFooterDate
            // 
            this.txtFooterDate.CanGrow = false;
            this.txtFooterDate.Format = "{0:dddd, MMMM d, yyyy}";
            this.txtFooterDate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.20007880032062531D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.txtFooterDate.Multiline = true;
            this.txtFooterDate.Name = "txtFooterDate";
            this.txtFooterDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.7266085147857666D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.txtFooterDate.Style.Color = System.Drawing.Color.Black;
            this.txtFooterDate.Style.Font.Bold = false;
            this.txtFooterDate.Style.Font.Italic = false;
            this.txtFooterDate.Style.Font.Name = "Tahoma";
            this.txtFooterDate.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.txtFooterDate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtFooterDate.Value = "";
            // 
            // txtPageCount
            // 
            this.txtPageCount.CanGrow = false;
            this.txtPageCount.Format = "";
            this.txtPageCount.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2212862968444824D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.txtPageCount.Multiline = true;
            this.txtPageCount.Name = "txtPageCount";
            this.txtPageCount.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.7787139415740967D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.txtPageCount.Style.Color = System.Drawing.Color.Black;
            this.txtPageCount.Style.Font.Bold = false;
            this.txtPageCount.Style.Font.Italic = false;
            this.txtPageCount.Style.Font.Name = "Tahoma";
            this.txtPageCount.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.txtPageCount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtPageCount.Value = "= \'Page \' +  PageNumber + \' of \' + PageCount";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.6832546591758728D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblHeaderLocations,
            this.textBox1,
            this.txtHeaderGroupWeight});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.PageBreak = Telerik.Reporting.PageBreak.After;
            // 
            // tblHeaderLocations
            // 
            this.tblHeaderLocations.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8499605655670166D)));
            this.tblHeaderLocations.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8499605655670166D)));
            this.tblHeaderLocations.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.tblHeaderLocations.Body.SetCellContent(0, 1, this.txtHeaderLocationWeight);
            this.tblHeaderLocations.Body.SetCellContent(0, 0, this.txtHeaderLocation);
            tableGroup8.Name = "tableGroup2";
            tableGroup9.Name = "tableGroup4";
            this.tblHeaderLocations.ColumnGroups.Add(tableGroup8);
            this.tblHeaderLocations.ColumnGroups.Add(tableGroup9);
            this.tblHeaderLocations.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtHeaderLocationWeight,
            this.txtHeaderLocation});
            this.tblHeaderLocations.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D), Telerik.Reporting.Drawing.Unit.Inch(0.38325461745262146D));
            this.tblHeaderLocations.Name = "tblHeaderLocations";
            tableGroup10.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup10.Name = "detailTableGroup1";
            this.tblHeaderLocations.RowGroups.Add(tableGroup10);
            this.tblHeaderLocations.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6999211311340332D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            // 
            // txtHeaderLocationWeight
            // 
            this.txtHeaderLocationWeight.Format = "{0:N0}";
            this.txtHeaderLocationWeight.Name = "txtHeaderLocationWeight";
            this.txtHeaderLocationWeight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8499605655670166D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtHeaderLocationWeight.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtHeaderLocationWeight.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtHeaderLocationWeight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtHeaderLocationWeight.StyleName = "";
            this.txtHeaderLocationWeight.Value = "";
            // 
            // txtHeaderLocation
            // 
            this.txtHeaderLocation.Name = "txtHeaderLocation";
            this.txtHeaderLocation.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8499605655670166D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtHeaderLocation.Style.Font.Bold = true;
            this.txtHeaderLocation.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtHeaderLocation.StyleName = "";
            this.txtHeaderLocation.Value = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.20003938674926758D), Telerik.Reporting.Drawing.Unit.Inch(0.18317587673664093D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8499211072921753D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox1.StyleName = "";
            this.textBox1.Value = "Group Total Pounds";
            // 
            // txtHeaderGroupWeight
            // 
            this.txtHeaderGroupWeight.Format = "{0:N0}";
            this.txtHeaderGroupWeight.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.050039529800415D), Telerik.Reporting.Drawing.Unit.Inch(0.183175727725029D));
            this.txtHeaderGroupWeight.Name = "txtHeaderGroupWeight";
            this.txtHeaderGroupWeight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8498817682266235D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.txtHeaderGroupWeight.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.txtHeaderGroupWeight.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtHeaderGroupWeight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtHeaderGroupWeight.StyleName = "";
            this.txtHeaderGroupWeight.Value = "";
            // 
            // dataSource
            // 
            this.dataSource.DataMember = "GetCycleCount";
            this.dataSource.DataSource = typeof(RioValleyChili.Client.Reporting.Services.InventoryReportingService);
            this.dataSource.Name = "dataSource";
            this.dataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("facilityKey", typeof(string), "= Parameters.FacilityKey.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("groupName", typeof(string), "= Parameters.GroupName.Value")});
            // 
            // InventoryCycleCountReport
            // 
            this.DataSource = this.dataSource;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "InventoryCycleCountReport";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.25D), Telerik.Reporting.Drawing.Unit.Inch(0.25D), Telerik.Reporting.Drawing.Unit.Inch(0.25D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowBlank = false;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.facilitiesDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Value";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Key";
            reportParameter1.Name = "FacilityKey";
            reportParameter1.Text = "Facility";
            reportParameter1.Visible = true;
            reportParameter2.AllowBlank = false;
            reportParameter2.AutoRefresh = true;
            reportParameter2.AvailableValues.DataSource = this.locationGroupNamesDataSource;
            reportParameter2.AvailableValues.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.Item", Telerik.Reporting.SortDirection.Asc));
            reportParameter2.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter2.Name = "GroupName";
            reportParameter2.Text = "Street";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox LabelTitle;
        private Telerik.Reporting.TextBox txtHeaderTimestamp;
        private Telerik.Reporting.ObjectDataSource dataSource;
        private Telerik.Reporting.TextBox txtLocation;
        private Telerik.Reporting.Table tblInventory;
        private Telerik.Reporting.TextBox txtLot;
        private Telerik.Reporting.TextBox txtLotDate;
        private Telerik.Reporting.TextBox txtProduct;
        private Telerik.Reporting.TextBox txtPackaging;
        private Telerik.Reporting.TextBox txtTrmt;
        private Telerik.Reporting.TextBox txtQuantity;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox txtFooterDate;
        private Telerik.Reporting.TextBox txtPageCount;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox txtFacility;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox txtGroupName;
        private Telerik.Reporting.TextBox txtGrandTotal;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.ObjectDataSource facilitiesDataSource;
        private Telerik.Reporting.ObjectDataSource locationGroupNamesDataSource;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.Table tblHeaderLocations;
        private Telerik.Reporting.TextBox txtHeaderLocationWeight;
        private Telerik.Reporting.TextBox txtHeaderLocation;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox txtHeaderGroupWeight;
    }
}