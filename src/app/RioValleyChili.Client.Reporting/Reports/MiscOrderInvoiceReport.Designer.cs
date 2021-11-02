namespace RioValleyChili.Client.Reporting.Reports
{
    partial class MiscOrderInvoiceReport
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MiscOrderInvoiceReport));
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.InstanceReportSource instanceReportSource2 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.LabelProduct = new Telerik.Reporting.TextBox();
            this.LabelProductName = new Telerik.Reporting.TextBox();
            this.LabelPackaging = new Telerik.Reporting.TextBox();
            this.LabelNetWeight = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.PanelHeader = new Telerik.Reporting.Panel();
            this.LabelShipDate = new Telerik.Reporting.TextBox();
            this.LabelPO = new Telerik.Reporting.TextBox();
            this.PO = new Telerik.Reporting.TextBox();
            this.ReportTitle = new Telerik.Reporting.TextBox();
            this.LabelMoveNumber = new Telerik.Reporting.TextBox();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.InvoiceDate = new Telerik.Reporting.TextBox();
            this.MoveNum = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.ShipToSubReport = new Telerik.Reporting.SubReport();
            this.LabelMoveTo = new Telerik.Reporting.TextBox();
            this.SoldToSubReport = new Telerik.Reporting.SubReport();
            this.LabelMoveFrom = new Telerik.Reporting.TextBox();
            this.PaymentTerms = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.Broker = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.Freight = new Telerik.Reporting.TextBox();
            this.LabelFreight = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.Origin = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.ShipDate = new Telerik.Reporting.TextBox();
            this.ItemDetails = new Telerik.Reporting.Table();
            this.Items_Quantity = new Telerik.Reporting.TextBox();
            this.Items_Packaging = new Telerik.Reporting.TextBox();
            this.Items_Product = new Telerik.Reporting.TextBox();
            this.Items_ProductName = new Telerik.Reporting.TextBox();
            this.Items_Value = new Telerik.Reporting.TextBox();
            this.Items_Price = new Telerik.Reporting.TextBox();
            this.LabelItemDetails = new Telerik.Reporting.TextBox();
            this.TotalDue = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.pnlInvoiceNotes = new Telerik.Reporting.Panel();
            this.InvoiceNotes = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.MoveNumBottom = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.objectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // LabelProduct
            // 
            this.LabelProduct.CanGrow = false;
            this.LabelProduct.Name = "LabelProduct";
            this.LabelProduct.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.88916724920272827D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.LabelProduct.Style.Color = System.Drawing.Color.Black;
            this.LabelProduct.Style.Font.Bold = true;
            this.LabelProduct.Style.Font.Italic = false;
            this.LabelProduct.Style.Font.Name = "Tahoma";
            this.LabelProduct.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.LabelProduct.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProduct.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProduct.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProduct.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProduct.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.LabelProduct.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelProduct.StyleName = "";
            this.LabelProduct.TextWrap = false;
            this.LabelProduct.Value = "Product";
            // 
            // LabelProductName
            // 
            this.LabelProductName.CanGrow = false;
            this.LabelProductName.Name = "LabelProductName";
            this.LabelProductName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2009868621826172D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.LabelProductName.Style.Font.Bold = true;
            this.LabelProductName.Style.Font.Name = "Tahoma";
            this.LabelProductName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.LabelProductName.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProductName.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProductName.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProductName.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelProductName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelProductName.StyleName = "";
            this.LabelProductName.TextWrap = false;
            // 
            // LabelPackaging
            // 
            this.LabelPackaging.CanGrow = false;
            this.LabelPackaging.Name = "LabelPackaging";
            this.LabelPackaging.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.626672625541687D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.LabelPackaging.Style.Color = System.Drawing.Color.Black;
            this.LabelPackaging.Style.Font.Bold = true;
            this.LabelPackaging.Style.Font.Italic = false;
            this.LabelPackaging.Style.Font.Name = "Tahoma";
            this.LabelPackaging.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.LabelPackaging.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelPackaging.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelPackaging.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelPackaging.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelPackaging.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.LabelPackaging.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelPackaging.StyleName = "";
            this.LabelPackaging.TextWrap = false;
            this.LabelPackaging.Value = "Packaging";
            // 
            // LabelNetWeight
            // 
            this.LabelNetWeight.CanGrow = false;
            this.LabelNetWeight.Name = "LabelNetWeight";
            this.LabelNetWeight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.83714443445205688D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.LabelNetWeight.Style.Color = System.Drawing.Color.Black;
            this.LabelNetWeight.Style.Font.Bold = true;
            this.LabelNetWeight.Style.Font.Italic = false;
            this.LabelNetWeight.Style.Font.Name = "Tahoma";
            this.LabelNetWeight.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.LabelNetWeight.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelNetWeight.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelNetWeight.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelNetWeight.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.LabelNetWeight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.LabelNetWeight.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelNetWeight.StyleName = "";
            this.LabelNetWeight.TextWrap = false;
            this.LabelNetWeight.Value = "Qty";
            // 
            // textBox7
            // 
            this.textBox7.CanGrow = false;
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.92189085483551025D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.Style.Color = System.Drawing.Color.Black;
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Italic = false;
            this.textBox7.Style.Font.Name = "Tahoma";
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox7.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox7.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox7.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox7.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.StyleName = "";
            this.textBox7.TextWrap = false;
            this.textBox7.Value = "Price";
            // 
            // textBox8
            // 
            this.textBox8.CanGrow = false;
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0241378545761108D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox8.Style.Color = System.Drawing.Color.Black;
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.Font.Italic = false;
            this.textBox8.Style.Font.Name = "Tahoma";
            this.textBox8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox8.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.StyleName = "";
            this.textBox8.TextWrap = false;
            this.textBox8.Value = "Value";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.345833420753479D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.PanelHeader});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // PanelHeader
            // 
            this.PanelHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.LabelShipDate,
            this.LabelPO,
            this.PO,
            this.ReportTitle,
            this.LabelMoveNumber,
            this.pictureBox1,
            this.textBox4,
            this.textBox5,
            this.InvoiceDate,
            this.MoveNum});
            this.PanelHeader.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.PanelHeader.Name = "PanelHeader";
            this.PanelHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.5D), Telerik.Reporting.Drawing.Unit.Inch(1.3458335399627686D));
            this.PanelHeader.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.PanelHeader.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.PanelHeader.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Inch(0D);
            // 
            // LabelShipDate
            // 
            this.LabelShipDate.CanGrow = false;
            this.LabelShipDate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.LabelShipDate.Multiline = false;
            this.LabelShipDate.Name = "LabelShipDate";
            this.LabelShipDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4999997615814209D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.LabelShipDate.Style.Color = System.Drawing.Color.Navy;
            this.LabelShipDate.Style.Font.Bold = true;
            this.LabelShipDate.Style.Font.Italic = true;
            this.LabelShipDate.Style.Font.Name = "Tahoma";
            this.LabelShipDate.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(16D);
            this.LabelShipDate.Value = "Invoice Date";
            // 
            // LabelPO
            // 
            this.LabelPO.CanGrow = false;
            this.LabelPO.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.6023898720741272D));
            this.LabelPO.Multiline = false;
            this.LabelPO.Name = "LabelPO";
            this.LabelPO.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0021777153015137D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.LabelPO.Style.Color = System.Drawing.Color.Navy;
            this.LabelPO.Style.Font.Bold = true;
            this.LabelPO.Style.Font.Italic = true;
            this.LabelPO.Style.Font.Name = "Tahoma";
            this.LabelPO.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(16D);
            this.LabelPO.Value = "P O Number";
            // 
            // PO
            // 
            this.PO.CanGrow = false;
            this.PO.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.9023346900939941D), Telerik.Reporting.Drawing.Unit.Inch(0.6023898720741272D));
            this.PO.Multiline = false;
            this.PO.Name = "PO";
            this.PO.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.597665548324585D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.PO.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.PO.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.PO.Style.Color = System.Drawing.Color.Black;
            this.PO.Style.Font.Bold = false;
            this.PO.Style.Font.Italic = false;
            this.PO.Style.Font.Name = "Tahoma";
            this.PO.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(17D);
            this.PO.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.PO.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.PO.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.PO.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.PO.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.PO.Value = "";
            // 
            // ReportTitle
            // 
            this.ReportTitle.CanGrow = false;
            this.ReportTitle.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.95015734434127808D));
            this.ReportTitle.Multiline = true;
            this.ReportTitle.Name = "ReportTitle";
            this.ReportTitle.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.6000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.35197243094444275D));
            this.ReportTitle.Style.Color = System.Drawing.Color.Black;
            this.ReportTitle.Style.Font.Bold = true;
            this.ReportTitle.Style.Font.Italic = false;
            this.ReportTitle.Style.Font.Name = "Tahoma";
            this.ReportTitle.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(17D);
            this.ReportTitle.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.ReportTitle.Value = "";
            // 
            // LabelMoveNumber
            // 
            this.LabelMoveNumber.CanGrow = false;
            this.LabelMoveNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(7.8731114626862109E-05D));
            this.LabelMoveNumber.Multiline = false;
            this.LabelMoveNumber.Name = "LabelMoveNumber";
            this.LabelMoveNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0021774768829346D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.LabelMoveNumber.Style.Color = System.Drawing.Color.Navy;
            this.LabelMoveNumber.Style.Font.Bold = true;
            this.LabelMoveNumber.Style.Font.Italic = true;
            this.LabelMoveNumber.Style.Font.Name = "Tahoma";
            this.LabelMoveNumber.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(16D);
            this.LabelMoveNumber.Value = "Invoice Number";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.0023900270462036133D));
            this.pictureBox1.MimeType = "image/jpeg";
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.6582036018371582D), Telerik.Reporting.Drawing.Unit.Inch(0.89999991655349731D));
            this.pictureBox1.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pictureBox1.Value = ((object)(resources.GetObject("pictureBox1.Value")));
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.65828227996826172D), Telerik.Reporting.Drawing.Unit.Inch(0.252390056848526D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8332916498184204D), Telerik.Reporting.Drawing.Unit.Inch(0.64992135763168335D));
            this.textBox4.Style.Font.Bold = false;
            this.textBox4.Style.Font.Name = "Times New Roman";
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox4.Value = "P O Box 131\r\nRincon, NM  87940\r\nTele:  575-267-4636\r\nFax:  575-267-4633";
            // 
            // textBox5
            // 
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.65828227996826172D), Telerik.Reporting.Drawing.Unit.Inch(0.0023900270462036133D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8332916498184204D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Font.Name = "Times New Roman";
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox5.Value = "Rio Valley Chili, Inc.";
            // 
            // InvoiceDate
            // 
            this.InvoiceDate.CanGrow = false;
            this.InvoiceDate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.4000787734985352D), Telerik.Reporting.Drawing.Unit.Inch(0.30231109261512756D));
            this.InvoiceDate.Multiline = false;
            this.InvoiceDate.Name = "InvoiceDate";
            this.InvoiceDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.0999214649200439D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.InvoiceDate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.InvoiceDate.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.InvoiceDate.Style.Color = System.Drawing.Color.Black;
            this.InvoiceDate.Style.Font.Bold = false;
            this.InvoiceDate.Style.Font.Italic = false;
            this.InvoiceDate.Style.Font.Name = "Tahoma";
            this.InvoiceDate.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(17D);
            this.InvoiceDate.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.InvoiceDate.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.InvoiceDate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.InvoiceDate.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.InvoiceDate.Value = "";
            // 
            // MoveNum
            // 
            this.MoveNum.CanGrow = false;
            this.MoveNum.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.902256965637207D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.MoveNum.Multiline = false;
            this.MoveNum.Name = "MoveNum";
            this.MoveNum.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.5977427959442139D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.MoveNum.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.MoveNum.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.MoveNum.Style.Color = System.Drawing.Color.Black;
            this.MoveNum.Style.Font.Bold = false;
            this.MoveNum.Style.Font.Italic = false;
            this.MoveNum.Style.Font.Name = "Tahoma";
            this.MoveNum.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(17D);
            this.MoveNum.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.MoveNum.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.MoveNum.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.MoveNum.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.MoveNum.Value = "";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(3.8542850017547607D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.ShipToSubReport,
            this.LabelMoveTo,
            this.SoldToSubReport,
            this.LabelMoveFrom,
            this.PaymentTerms,
            this.textBox1,
            this.Broker,
            this.textBox2,
            this.Freight,
            this.LabelFreight,
            this.textBox3,
            this.Origin,
            this.textBox6,
            this.ShipDate,
            this.ItemDetails,
            this.LabelItemDetails,
            this.TotalDue,
            this.textBox12,
            this.pnlInvoiceNotes});
            this.detail.Name = "detail";
            // 
            // ShipToSubReport
            // 
            this.ShipToSubReport.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8916666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.65416646003723145D));
            this.ShipToSubReport.Name = "ShipToSubReport";
            instanceReportSource1.ReportDocument = null;
            this.ShipToSubReport.ReportSource = instanceReportSource1;
            this.ShipToSubReport.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6083333492279053D), Telerik.Reporting.Drawing.Unit.Inch(0.29988172650337219D));
            // 
            // LabelMoveTo
            // 
            this.LabelMoveTo.Anchoring = Telerik.Reporting.AnchoringStyles.Top;
            this.LabelMoveTo.CanGrow = false;
            this.LabelMoveTo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8916666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.45408767461776733D));
            this.LabelMoveTo.Name = "LabelMoveTo";
            this.LabelMoveTo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90000009536743164D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.LabelMoveTo.Style.Color = System.Drawing.Color.Navy;
            this.LabelMoveTo.Style.Font.Bold = true;
            this.LabelMoveTo.Style.Font.Italic = false;
            this.LabelMoveTo.Style.Font.Name = "Tahoma";
            this.LabelMoveTo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.LabelMoveTo.Value = "Ship To:";
            // 
            // SoldToSubReport
            // 
            this.SoldToSubReport.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.59999996423721313D), Telerik.Reporting.Drawing.Unit.Inch(0.65416646003723145D));
            this.SoldToSubReport.Name = "SoldToSubReport";
            instanceReportSource2.ReportDocument = null;
            this.SoldToSubReport.ReportSource = instanceReportSource2;
            this.SoldToSubReport.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.200000524520874D), Telerik.Reporting.Drawing.Unit.Inch(0.29988178610801697D));
            // 
            // LabelMoveFrom
            // 
            this.LabelMoveFrom.Anchoring = Telerik.Reporting.AnchoringStyles.Top;
            this.LabelMoveFrom.CanGrow = false;
            this.LabelMoveFrom.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.59999996423721313D), Telerik.Reporting.Drawing.Unit.Inch(0.45408767461776733D));
            this.LabelMoveFrom.Name = "LabelMoveFrom";
            this.LabelMoveFrom.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.LabelMoveFrom.Style.Color = System.Drawing.Color.Navy;
            this.LabelMoveFrom.Style.Font.Bold = true;
            this.LabelMoveFrom.Style.Font.Italic = false;
            this.LabelMoveFrom.Style.Font.Name = "Tahoma";
            this.LabelMoveFrom.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.LabelMoveFrom.Value = "Sold To:";
            // 
            // PaymentTerms
            // 
            this.PaymentTerms.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1.9541666507720947D));
            this.PaymentTerms.Multiline = false;
            this.PaymentTerms.Name = "PaymentTerms";
            this.PaymentTerms.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.5999212265014648D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.PaymentTerms.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.PaymentTerms.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.PaymentTerms.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.PaymentTerms.Value = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D), Telerik.Reporting.Drawing.Unit.Inch(1.9541668891906738D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89999991655349731D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox1.Style.Color = System.Drawing.Color.Navy;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Italic = true;
            this.textBox1.Style.Font.Name = "Tahoma";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "Pmt Terms:";
            // 
            // Broker
            // 
            this.Broker.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.8062100410461426D), Telerik.Reporting.Drawing.Unit.Inch(1.654166579246521D));
            this.Broker.Multiline = false;
            this.Broker.Name = "Broker";
            this.Broker.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6937488317489624D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.Broker.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Broker.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Broker.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Broker.Value = "";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(1.6541668176651D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.60613059997558594D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox2.Style.Color = System.Drawing.Color.Navy;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Italic = true;
            this.textBox2.Style.Font.Name = "Tahoma";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.Value = "Broker:";
            // 
            // Freight
            // 
            this.Freight.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.8939471244812012D), Telerik.Reporting.Drawing.Unit.Inch(1.9541668891906738D));
            this.Freight.Multiline = false;
            this.Freight.Name = "Freight";
            this.Freight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6060116291046143D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.Freight.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Freight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Freight.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Freight.Value = "";
            // 
            // LabelFreight
            // 
            this.LabelFreight.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(1.9541668891906738D));
            this.LabelFreight.Name = "LabelFreight";
            this.LabelFreight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.69386780261993408D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.LabelFreight.Style.Color = System.Drawing.Color.Navy;
            this.LabelFreight.Style.Font.Bold = true;
            this.LabelFreight.Style.Font.Italic = true;
            this.LabelFreight.Style.Font.Name = "Tahoma";
            this.LabelFreight.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.LabelFreight.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelFreight.Value = "Freight:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999956786632538D), Telerik.Reporting.Drawing.Unit.Inch(1.6541666984558106D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.55820357799530029D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox3.Style.Color = System.Drawing.Color.Navy;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Italic = true;
            this.textBox3.Style.Font.Name = "Tahoma";
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "Origin:";
            // 
            // Origin
            // 
            this.Origin.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.65828239917755127D), Telerik.Reporting.Drawing.Unit.Inch(1.6541666984558106D));
            this.Origin.Multiline = false;
            this.Origin.Name = "Origin";
            this.Origin.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.24171781539917D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.Origin.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Origin.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Origin.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Origin.Value = "";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9999210834503174D), Telerik.Reporting.Drawing.Unit.Inch(1.6541669368743897D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80000019073486328D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox6.Style.Color = System.Drawing.Color.Navy;
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Font.Italic = true;
            this.textBox6.Style.Font.Name = "Tahoma";
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.Value = "Ship Date:";
            // 
            // ShipDate
            // 
            this.ShipDate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(1.6541669368743897D));
            this.ShipDate.Multiline = false;
            this.ShipDate.Name = "ShipDate";
            this.ShipDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999216318130493D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.ShipDate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.ShipDate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.ShipDate.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.ShipDate.Value = "";
            // 
            // ItemDetails
            // 
            this.ItemDetails.Anchoring = ((Telerik.Reporting.AnchoringStyles)((Telerik.Reporting.AnchoringStyles.Left | Telerik.Reporting.AnchoringStyles.Right)));
            this.ItemDetails.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.889167308807373D)));
            this.ItemDetails.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.2009868621826172D)));
            this.ItemDetails.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6266727447509766D)));
            this.ItemDetails.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.83714455366134644D)));
            this.ItemDetails.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.92189085483551025D)));
            this.ItemDetails.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0241379737854004D)));
            this.ItemDetails.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.ItemDetails.Body.SetCellContent(0, 3, this.Items_Quantity);
            this.ItemDetails.Body.SetCellContent(0, 2, this.Items_Packaging);
            this.ItemDetails.Body.SetCellContent(0, 0, this.Items_Product);
            this.ItemDetails.Body.SetCellContent(0, 1, this.Items_ProductName);
            this.ItemDetails.Body.SetCellContent(0, 5, this.Items_Value);
            this.ItemDetails.Body.SetCellContent(0, 4, this.Items_Price);
            tableGroup1.Name = "group4";
            tableGroup1.ReportItem = this.LabelProduct;
            tableGroup2.Name = "group5";
            tableGroup2.ReportItem = this.LabelProductName;
            tableGroup3.Name = "group";
            tableGroup3.ReportItem = this.LabelPackaging;
            tableGroup4.Name = "tableGroup2";
            tableGroup4.ReportItem = this.LabelNetWeight;
            tableGroup5.Name = "group1";
            tableGroup5.ReportItem = this.textBox7;
            tableGroup6.Name = "group3";
            tableGroup6.ReportItem = this.textBox8;
            this.ItemDetails.ColumnGroups.Add(tableGroup1);
            this.ItemDetails.ColumnGroups.Add(tableGroup2);
            this.ItemDetails.ColumnGroups.Add(tableGroup3);
            this.ItemDetails.ColumnGroups.Add(tableGroup4);
            this.ItemDetails.ColumnGroups.Add(tableGroup5);
            this.ItemDetails.ColumnGroups.Add(tableGroup6);
            this.ItemDetails.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.Items_Quantity,
            this.Items_Packaging,
            this.Items_Product,
            this.Items_ProductName,
            this.Items_Value,
            this.Items_Price,
            this.LabelProduct,
            this.LabelProductName,
            this.LabelPackaging,
            this.LabelNetWeight,
            this.textBox7,
            this.textBox8});
            this.ItemDetails.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(2.5584075450897217D));
            this.ItemDetails.Name = "ItemDetails";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detailTableGroup";
            this.ItemDetails.RowGroups.Add(tableGroup7);
            this.ItemDetails.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.5D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.ItemDetails.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0D);
            // 
            // Items_Quantity
            // 
            this.Items_Quantity.CanGrow = false;
            this.Items_Quantity.Multiline = false;
            this.Items_Quantity.Name = "Items_Quantity";
            this.Items_Quantity.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.83714443445205688D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.Items_Quantity.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Items_Quantity.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Quantity.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Quantity.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Quantity.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Quantity.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Items_Quantity.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.Items_Quantity.TextWrap = false;
            this.Items_Quantity.Value = "";
            // 
            // Items_Packaging
            // 
            this.Items_Packaging.CanGrow = false;
            this.Items_Packaging.Multiline = false;
            this.Items_Packaging.Name = "Items_Packaging";
            this.Items_Packaging.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.626672625541687D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.Items_Packaging.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Items_Packaging.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Packaging.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Packaging.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Packaging.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Packaging.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Items_Packaging.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.Items_Packaging.StyleName = "";
            this.Items_Packaging.TextWrap = false;
            // 
            // Items_Product
            // 
            this.Items_Product.CanGrow = false;
            this.Items_Product.Multiline = false;
            this.Items_Product.Name = "Items_Product";
            this.Items_Product.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.88916724920272827D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.Items_Product.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Items_Product.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Product.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Product.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Product.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Product.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Items_Product.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.Items_Product.StyleName = "";
            this.Items_Product.TextWrap = false;
            // 
            // Items_ProductName
            // 
            this.Items_ProductName.CanGrow = false;
            this.Items_ProductName.Multiline = false;
            this.Items_ProductName.Name = "Items_ProductName";
            this.Items_ProductName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2009868621826172D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.Items_ProductName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Items_ProductName.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_ProductName.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_ProductName.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_ProductName.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_ProductName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Items_ProductName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.Items_ProductName.StyleName = "";
            this.Items_ProductName.TextWrap = false;
            // 
            // Items_Value
            // 
            this.Items_Value.Name = "Items_Value";
            this.Items_Value.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0241378545761108D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.Items_Value.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Items_Value.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Value.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Value.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Value.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Value.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Items_Value.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.Items_Value.StyleName = "";
            // 
            // Items_Price
            // 
            this.Items_Price.CanGrow = false;
            this.Items_Price.Multiline = false;
            this.Items_Price.Name = "Items_Price";
            this.Items_Price.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.92189085483551025D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.Items_Price.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.Items_Price.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Price.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Price.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Price.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Items_Price.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Items_Price.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.Items_Price.TextWrap = false;
            // 
            // LabelItemDetails
            // 
            this.LabelItemDetails.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(2.3583285808563232D));
            this.LabelItemDetails.Name = "LabelItemDetails";
            this.LabelItemDetails.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.LabelItemDetails.Style.Color = System.Drawing.Color.Black;
            this.LabelItemDetails.Style.Font.Bold = true;
            this.LabelItemDetails.Style.Font.Italic = false;
            this.LabelItemDetails.Style.Font.Name = "Tahoma";
            this.LabelItemDetails.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.LabelItemDetails.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelItemDetails.Value = "Item Details:";
            // 
            // TotalDue
            // 
            this.TotalDue.CanGrow = false;
            this.TotalDue.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.4759411811828613D), Telerik.Reporting.Drawing.Unit.Inch(3.1513822078704834D));
            this.TotalDue.Multiline = false;
            this.TotalDue.Name = "TotalDue";
            this.TotalDue.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0240181684494019D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.TotalDue.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.TotalDue.Style.Font.Bold = true;
            this.TotalDue.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalDue.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalDue.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalDue.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalDue.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.TotalDue.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.TotalDue.TextWrap = false;
            this.TotalDue.Value = "";
            // 
            // textBox12
            // 
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.7059669494628906D), Telerik.Reporting.Drawing.Unit.Inch(3.1541669368743896D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.76989567279815674D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox12.Style.Color = System.Drawing.Color.Black;
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.Font.Italic = false;
            this.textBox12.Style.Font.Name = "Tahoma";
            this.textBox12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox12.Value = "Total Due";
            // 
            // pnlInvoiceNotes
            // 
            this.pnlInvoiceNotes.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.InvoiceNotes,
            this.textBox15});
            this.pnlInvoiceNotes.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.1325885831611231E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.4541666507720947D));
            this.pnlInvoiceNotes.Name = "pnlInvoiceNotes";
            this.pnlInvoiceNotes.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4999594688415527D), Telerik.Reporting.Drawing.Unit.Inch(0.400118350982666D));
            // 
            // InvoiceNotes
            // 
            this.InvoiceNotes.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D), Telerik.Reporting.Drawing.Unit.Inch(0.20011837780475617D));
            this.InvoiceNotes.Name = "InvoiceNotes";
            this.InvoiceNotes.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.3000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.InvoiceNotes.Style.Color = System.Drawing.Color.Black;
            this.InvoiceNotes.Style.Font.Bold = false;
            this.InvoiceNotes.Style.Font.Italic = false;
            this.InvoiceNotes.Style.Font.Name = "Tahoma";
            this.InvoiceNotes.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.InvoiceNotes.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.InvoiceNotes.Value = "";
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox15.Style.Color = System.Drawing.Color.Black;
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Style.Font.Italic = false;
            this.textBox15.Style.Font.Name = "Tahoma";
            this.textBox15.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox15.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox15.Value = "Invoice Notes";
            // 
            // textBox9
            // 
            this.textBox9.CanGrow = false;
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.0744423030409962E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.14798800647258759D));
            this.textBox9.Multiline = true;
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.0999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.35197243094444275D));
            this.textBox9.Style.Color = System.Drawing.Color.Black;
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Font.Italic = true;
            this.textBox9.Style.Font.Name = "Tahoma";
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(16D);
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox9.Value = "Thank you. We appreciate your business!";
            // 
            // MoveNumBottom
            // 
            this.MoveNumBottom.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3698954582214355D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.MoveNumBottom.Name = "MoveNumBottom";
            this.MoveNumBottom.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1300640106201172D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.MoveNumBottom.Style.Color = System.Drawing.Color.Black;
            this.MoveNumBottom.Style.Font.Bold = false;
            this.MoveNumBottom.Style.Font.Italic = false;
            this.MoveNumBottom.Style.Font.Name = "Tahoma";
            this.MoveNumBottom.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.MoveNumBottom.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.MoveNumBottom.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.MoveNumBottom.Value = "";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20003938674926758D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.MoveNumBottom});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // objectDataSource
            // 
            this.objectDataSource.DataMember = "GetCustomerOrderInvoice";
            this.objectDataSource.DataSource = typeof(RioValleyChili.Client.Reporting.Services.InventoryShipmentOrderReportingService);
            this.objectDataSource.Name = "objectDataSource";
            this.objectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("orderKey", typeof(string), "= Parameters.orderKey.Value")});
            // 
            // reportFooterSection1
            // 
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.49999985098838806D);
            this.reportFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox9});
            this.reportFooterSection1.Name = "reportFooterSection1";
            this.reportFooterSection1.PrintAtBottom = true;
            // 
            // MiscOrderInvoiceReport
            // 
            this.DataSource = this.objectDataSource;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportFooterSection1});
            this.Name = "MiscOrderInvoiceReport";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowBlank = false;
            reportParameter1.Name = "orderKey";
            reportParameter1.Text = "orderKey";
            this.ReportParameters.Add(reportParameter1);
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

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Panel PanelHeader;
        private Telerik.Reporting.TextBox LabelShipDate;
        private Telerik.Reporting.TextBox LabelPO;
        private Telerik.Reporting.TextBox PO;
        private Telerik.Reporting.TextBox ReportTitle;
        private Telerik.Reporting.TextBox LabelMoveNumber;
        private Telerik.Reporting.PictureBox pictureBox1;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox InvoiceDate;
        private Telerik.Reporting.TextBox MoveNum;
        private Telerik.Reporting.SubReport ShipToSubReport;
        private Telerik.Reporting.TextBox LabelMoveTo;
        private Telerik.Reporting.SubReport SoldToSubReport;
        private Telerik.Reporting.TextBox LabelMoveFrom;
        private Telerik.Reporting.ObjectDataSource objectDataSource;
        private Telerik.Reporting.TextBox PaymentTerms;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox Broker;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox Freight;
        private Telerik.Reporting.TextBox LabelFreight;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox Origin;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox ShipDate;
        private Telerik.Reporting.Table ItemDetails;
        private Telerik.Reporting.TextBox Items_Quantity;
        private Telerik.Reporting.TextBox Items_Packaging;
        private Telerik.Reporting.TextBox Items_Product;
        private Telerik.Reporting.TextBox Items_ProductName;
        private Telerik.Reporting.TextBox Items_Value;
        private Telerik.Reporting.TextBox Items_Price;
        private Telerik.Reporting.TextBox LabelProduct;
        private Telerik.Reporting.TextBox LabelProductName;
        private Telerik.Reporting.TextBox LabelPackaging;
        private Telerik.Reporting.TextBox LabelNetWeight;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox LabelItemDetails;
        private Telerik.Reporting.TextBox TotalDue;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.Panel pnlInvoiceNotes;
        private Telerik.Reporting.TextBox InvoiceNotes;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox MoveNumBottom;
        private Telerik.Reporting.ReportFooterSection reportFooterSection1;
    }
}