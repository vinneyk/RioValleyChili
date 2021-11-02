namespace RioValleyChili.Client.Reporting.Reports
{
    partial class PackingListBarcodeReport
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackingListBarcodeReport));
            Telerik.Reporting.Barcodes.Code39Encoder code39Encoder1 = new Telerik.Reporting.Barcodes.Code39Encoder();
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.InstanceReportSource instanceReportSource2 = new Telerik.Reporting.InstanceReportSource();
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
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup14 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Barcodes.Code39Encoder code39Encoder2 = new Telerik.Reporting.Barcodes.Code39Encoder();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.Item_QuantityHeader = new Telerik.Reporting.TextBox();
            this.Item_UnityPackSizeHeader = new Telerik.Reporting.TextBox();
            this.Item_ProductHeader = new Telerik.Reporting.TextBox();
            this.Item_LotNumHeader = new Telerik.Reporting.TextBox();
            this.Item_LoBacHeader = new Telerik.Reporting.TextBox();
            this.Item_TrtmtHeader = new Telerik.Reporting.TextBox();
            this.Item_CustCodeHeader = new Telerik.Reporting.TextBox();
            this.Item_CustLotHeader = new Telerik.Reporting.TextBox();
            this.Item_GrossWeightHeader = new Telerik.Reporting.TextBox();
            this.Item_NetWeightHeader = new Telerik.Reporting.TextBox();
            this.Item_CheckHeader = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.LabelTitle = new Telerik.Reporting.TextBox();
            this.MoveNumberPanel = new Telerik.Reporting.Panel();
            this.MoveNum = new Telerik.Reporting.TextBox();
            this.LabelMoveNumber = new Telerik.Reporting.TextBox();
            this.PO = new Telerik.Reporting.TextBox();
            this.LabelPO = new Telerik.Reporting.TextBox();
            this.Date = new Telerik.Reporting.TextBox();
            this.LabelDate = new Telerik.Reporting.TextBox();
            this.barcodePONumber = new Telerik.Reporting.Barcode();
            this.detail = new Telerik.Reporting.DetailSection();
            this.PanelLabels = new Telerik.Reporting.Panel();
            this.MoveTo = new Telerik.Reporting.SubReport();
            this.MoveFromOrSoldTo = new Telerik.Reporting.SubReport();
            this.panel1 = new Telerik.Reporting.Panel();
            this.TableItems = new Telerik.Reporting.Table();
            this.Item_Quantity = new Telerik.Reporting.TextBox();
            this.Item_UnityPackSize = new Telerik.Reporting.TextBox();
            this.Item_Product = new Telerik.Reporting.TextBox();
            this.Item_LotNum = new Telerik.Reporting.TextBox();
            this.Item_CustCode = new Telerik.Reporting.TextBox();
            this.Item_CustLot = new Telerik.Reporting.TextBox();
            this.Item_Gross = new Telerik.Reporting.TextBox();
            this.Item_NetWeight = new Telerik.Reporting.TextBox();
            this.Item_Treatment = new Telerik.Reporting.TextBox();
            this.Item_LoBac = new Telerik.Reporting.CheckBox();
            this.Item_Check = new Telerik.Reporting.CheckBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.barcodeLotNumber = new Telerik.Reporting.Barcode();
            this.barcodeCustomerProduct = new Telerik.Reporting.Barcode();
            this.TotalQuantity = new Telerik.Reporting.TextBox();
            this.PalletWeight = new Telerik.Reporting.TextBox();
            this.PalletWeightLabel = new Telerik.Reporting.TextBox();
            this.TotalsLabel = new Telerik.Reporting.TextBox();
            this.TotalGrossWeight = new Telerik.Reporting.TextBox();
            this.TotalNetWeight = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.Footer_MoveNum = new Telerik.Reporting.TextBox();
            this.Footer_PageNumber = new Telerik.Reporting.TextBox();
            this.Footer_DateTime = new Telerik.Reporting.TextBox();
            this.packingListDataSource = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Item_QuantityHeader
            // 
            this.Item_QuantityHeader.Name = "Item_QuantityHeader";
            this.Item_QuantityHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.31337907910346985D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_QuantityHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_QuantityHeader.Style.Color = System.Drawing.Color.White;
            this.Item_QuantityHeader.Style.Font.Bold = true;
            this.Item_QuantityHeader.Style.Font.Italic = true;
            this.Item_QuantityHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_QuantityHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_QuantityHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_QuantityHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Item_QuantityHeader.TextWrap = false;
            this.Item_QuantityHeader.Value = "Qty";
            // 
            // Item_UnityPackSizeHeader
            // 
            this.Item_UnityPackSizeHeader.Name = "Item_UnityPackSizeHeader";
            this.Item_UnityPackSizeHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98265516757965088D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_UnityPackSizeHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_UnityPackSizeHeader.Style.Color = System.Drawing.Color.White;
            this.Item_UnityPackSizeHeader.Style.Font.Bold = true;
            this.Item_UnityPackSizeHeader.Style.Font.Italic = true;
            this.Item_UnityPackSizeHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_UnityPackSizeHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_UnityPackSizeHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_UnityPackSizeHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_UnityPackSizeHeader.TextWrap = false;
            this.Item_UnityPackSizeHeader.Value = "Unit Pack Size";
            // 
            // Item_ProductHeader
            // 
            this.Item_ProductHeader.Name = "Item_ProductHeader";
            this.Item_ProductHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.329169511795044D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_ProductHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_ProductHeader.Style.Color = System.Drawing.Color.White;
            this.Item_ProductHeader.Style.Font.Bold = true;
            this.Item_ProductHeader.Style.Font.Italic = true;
            this.Item_ProductHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_ProductHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_ProductHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_ProductHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_ProductHeader.StyleName = "";
            this.Item_ProductHeader.TextWrap = false;
            this.Item_ProductHeader.Value = "Product";
            // 
            // Item_LotNumHeader
            // 
            this.Item_LotNumHeader.Name = "Item_LotNumHeader";
            this.Item_LotNumHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.81725025177001953D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_LotNumHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_LotNumHeader.Style.Color = System.Drawing.Color.White;
            this.Item_LotNumHeader.Style.Font.Bold = true;
            this.Item_LotNumHeader.Style.Font.Italic = true;
            this.Item_LotNumHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_LotNumHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LotNumHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LotNumHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_LotNumHeader.StyleName = "";
            this.Item_LotNumHeader.TextWrap = false;
            this.Item_LotNumHeader.Value = "Lot Num";
            // 
            // Item_LoBacHeader
            // 
            this.Item_LoBacHeader.Name = "Item_LoBacHeader";
            this.Item_LoBacHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.43347728252410889D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_LoBacHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_LoBacHeader.Style.Color = System.Drawing.Color.White;
            this.Item_LoBacHeader.Style.Font.Bold = true;
            this.Item_LoBacHeader.Style.Font.Italic = true;
            this.Item_LoBacHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_LoBacHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LoBacHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LoBacHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Item_LoBacHeader.StyleName = "";
            this.Item_LoBacHeader.TextWrap = false;
            this.Item_LoBacHeader.Value = "LoBac";
            // 
            // Item_TrtmtHeader
            // 
            this.Item_TrtmtHeader.Name = "Item_TrtmtHeader";
            this.Item_TrtmtHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.37643584609031677D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_TrtmtHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_TrtmtHeader.Style.Color = System.Drawing.Color.White;
            this.Item_TrtmtHeader.Style.Font.Bold = true;
            this.Item_TrtmtHeader.Style.Font.Italic = true;
            this.Item_TrtmtHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_TrtmtHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_TrtmtHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_TrtmtHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_TrtmtHeader.StyleName = "";
            this.Item_TrtmtHeader.TextWrap = false;
            this.Item_TrtmtHeader.Value = "Trtmt";
            // 
            // Item_CustCodeHeader
            // 
            this.Item_CustCodeHeader.Name = "Item_CustCodeHeader";
            this.Item_CustCodeHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.7600405216217041D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_CustCodeHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_CustCodeHeader.Style.Color = System.Drawing.Color.White;
            this.Item_CustCodeHeader.Style.Font.Bold = true;
            this.Item_CustCodeHeader.Style.Font.Italic = true;
            this.Item_CustCodeHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_CustCodeHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustCodeHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustCodeHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_CustCodeHeader.StyleName = "";
            this.Item_CustCodeHeader.TextWrap = false;
            this.Item_CustCodeHeader.Value = "Cust Code";
            // 
            // Item_CustLotHeader
            // 
            this.Item_CustLotHeader.Name = "Item_CustLotHeader";
            this.Item_CustLotHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80522626638412476D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_CustLotHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_CustLotHeader.Style.Color = System.Drawing.Color.White;
            this.Item_CustLotHeader.Style.Font.Bold = true;
            this.Item_CustLotHeader.Style.Font.Italic = true;
            this.Item_CustLotHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_CustLotHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustLotHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustLotHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_CustLotHeader.StyleName = "";
            this.Item_CustLotHeader.TextWrap = false;
            this.Item_CustLotHeader.Value = "Cust Lot";
            // 
            // Item_GrossWeightHeader
            // 
            this.Item_GrossWeightHeader.Name = "Item_GrossWeightHeader";
            this.Item_GrossWeightHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80100381374359131D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_GrossWeightHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_GrossWeightHeader.Style.Color = System.Drawing.Color.White;
            this.Item_GrossWeightHeader.Style.Font.Bold = true;
            this.Item_GrossWeightHeader.Style.Font.Italic = true;
            this.Item_GrossWeightHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_GrossWeightHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_GrossWeightHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_GrossWeightHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Item_GrossWeightHeader.StyleName = "";
            this.Item_GrossWeightHeader.TextWrap = false;
            this.Item_GrossWeightHeader.Value = "Gross Weight";
            // 
            // Item_NetWeightHeader
            // 
            this.Item_NetWeightHeader.Name = "Item_NetWeightHeader";
            this.Item_NetWeightHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66277575492858887D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_NetWeightHeader.Style.BackgroundColor = System.Drawing.Color.Black;
            this.Item_NetWeightHeader.Style.Color = System.Drawing.Color.White;
            this.Item_NetWeightHeader.Style.Font.Bold = true;
            this.Item_NetWeightHeader.Style.Font.Italic = true;
            this.Item_NetWeightHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_NetWeightHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_NetWeightHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_NetWeightHeader.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Item_NetWeightHeader.StyleName = "";
            this.Item_NetWeightHeader.TextWrap = false;
            this.Item_NetWeightHeader.Value = "Net Weight";
            // 
            // Item_CheckHeader
            // 
            this.Item_CheckHeader.Name = "Item_CheckHeader";
            this.Item_CheckHeader.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.21842926740646362D), Telerik.Reporting.Drawing.Unit.Inch(0.20833317935466766D));
            this.Item_CheckHeader.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.Item_CheckHeader.Style.Color = System.Drawing.Color.White;
            this.Item_CheckHeader.Style.Font.Bold = true;
            this.Item_CheckHeader.Style.Font.Italic = true;
            this.Item_CheckHeader.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_CheckHeader.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CheckHeader.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CheckHeader.StyleName = "";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.98000001907348633D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pictureBox1,
            this.textBox2,
            this.textBox1,
            this.LabelTitle,
            this.MoveNumberPanel,
            this.LabelMoveNumber,
            this.PO,
            this.LabelPO,
            this.Date,
            this.LabelDate,
            this.barcodePONumber});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.93986701965332E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.93986701965332E-05D));
            this.pictureBox1.MimeType = "image/jpeg";
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.6582036018371582D), Telerik.Reporting.Drawing.Unit.Inch(0.89999991655349731D));
            this.pictureBox1.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pictureBox1.Value = ((object)(resources.GetObject("pictureBox1.Value")));
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.66670608520507812D), Telerik.Reporting.Drawing.Unit.Inch(0.25003945827484131D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.3999998569488525D), Telerik.Reporting.Drawing.Unit.Inch(0.64992135763168335D));
            this.textBox2.Style.Font.Bold = false;
            this.textBox2.Style.Font.Name = "Times New Roman";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox2.Value = "P O Box 131\r\nRincon, NM  87940\r\nTele:  575-267-4636\r\nFax:  575-267-4633";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.66670608520507812D), Telerik.Reporting.Drawing.Unit.Inch(3.93986701965332E-05D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.3999998569488525D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Name = "Times New Roman";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox1.Value = "Rio Valley Chili, Inc.";
            // 
            // LabelTitle
            // 
            this.LabelTitle.CanGrow = false;
            this.LabelTitle.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.0667848587036133D), Telerik.Reporting.Drawing.Unit.Inch(0.59992152452468872D));
            this.LabelTitle.Multiline = true;
            this.LabelTitle.Name = "LabelTitle";
            this.LabelTitle.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5332150459289551D), Telerik.Reporting.Drawing.Unit.Inch(0.30003929138183594D));
            this.LabelTitle.Style.Color = System.Drawing.Color.Black;
            this.LabelTitle.Style.Font.Bold = true;
            this.LabelTitle.Style.Font.Italic = false;
            this.LabelTitle.Style.Font.Name = "Tahoma";
            this.LabelTitle.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(16D);
            this.LabelTitle.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.LabelTitle.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelTitle.Value = "Packing List";
            // 
            // MoveNumberPanel
            // 
            this.MoveNumberPanel.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.MoveNum});
            this.MoveNumberPanel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.1999607086181641D), Telerik.Reporting.Drawing.Unit.Inch(0.15007877349853516D));
            this.MoveNumberPanel.Name = "MoveNumberPanel";
            this.MoveNumberPanel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999998331069946D), Telerik.Reporting.Drawing.Unit.Inch(0.14996062219142914D));
            this.MoveNumberPanel.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // MoveNum
            // 
            this.MoveNum.CanGrow = false;
            this.MoveNum.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(7.8731114626862109E-05D));
            this.MoveNum.Multiline = false;
            this.MoveNum.Name = "MoveNum";
            this.MoveNum.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999604940414429D), Telerik.Reporting.Drawing.Unit.Inch(0.14988189935684204D));
            this.MoveNum.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.MoveNum.Style.Color = System.Drawing.Color.Black;
            this.MoveNum.Style.Font.Bold = false;
            this.MoveNum.Style.Font.Italic = false;
            this.MoveNum.Style.Font.Name = "Tahoma";
            this.MoveNum.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.MoveNum.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.MoveNum.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.MoveNum.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.MoveNum.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.MoveNum.Value = "";
            // 
            // LabelMoveNumber
            // 
            this.LabelMoveNumber.CanGrow = false;
            this.LabelMoveNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.15003931522369385D));
            this.LabelMoveNumber.Multiline = false;
            this.LabelMoveNumber.Name = "LabelMoveNumber";
            this.LabelMoveNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99988126754760742D), Telerik.Reporting.Drawing.Unit.Inch(0.15000000596046448D));
            this.LabelMoveNumber.Style.Color = System.Drawing.Color.Black;
            this.LabelMoveNumber.Style.Font.Bold = true;
            this.LabelMoveNumber.Style.Font.Italic = true;
            this.LabelMoveNumber.Style.Font.Name = "Tahoma";
            this.LabelMoveNumber.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.LabelMoveNumber.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(4D);
            this.LabelMoveNumber.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.LabelMoveNumber.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelMoveNumber.Value = "Move Num";
            // 
            // PO
            // 
            this.PO.CanGrow = false;
            this.PO.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.1999607086181641D), Telerik.Reporting.Drawing.Unit.Inch(0.30011805891990662D));
            this.PO.Multiline = false;
            this.PO.Name = "PO";
            this.PO.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.300040602684021D), Telerik.Reporting.Drawing.Unit.Inch(0.14996087551116943D));
            this.PO.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.PO.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.PO.Style.Color = System.Drawing.Color.Black;
            this.PO.Style.Font.Bold = false;
            this.PO.Style.Font.Italic = false;
            this.PO.Style.Font.Name = "Tahoma";
            this.PO.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.PO.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.PO.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.PO.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.PO.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.PO.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.PO.Value = "";
            // 
            // LabelPO
            // 
            this.LabelPO.CanGrow = false;
            this.LabelPO.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.7000007629394531D), Telerik.Reporting.Drawing.Unit.Inch(0.30007883906364441D));
            this.LabelPO.Multiline = false;
            this.LabelPO.Name = "LabelPO";
            this.LabelPO.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.49988105893135071D), Telerik.Reporting.Drawing.Unit.Inch(0.15000000596046448D));
            this.LabelPO.Style.Color = System.Drawing.Color.Black;
            this.LabelPO.Style.Font.Bold = true;
            this.LabelPO.Style.Font.Italic = true;
            this.LabelPO.Style.Font.Name = "Tahoma";
            this.LabelPO.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.LabelPO.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(4D);
            this.LabelPO.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.LabelPO.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelPO.Value = "P O";
            // 
            // Date
            // 
            this.Date.CanGrow = false;
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.2000012397766113D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.Date.Multiline = false;
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.15000000596046448D));
            this.Date.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Date.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.Date.Style.Color = System.Drawing.Color.Black;
            this.Date.Style.Font.Bold = false;
            this.Date.Style.Font.Italic = false;
            this.Date.Style.Font.Name = "Tahoma";
            this.Date.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.Date.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Date.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.Date.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Date.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Date.Value = "";
            // 
            // LabelDate
            // 
            this.LabelDate.CanGrow = false;
            this.LabelDate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.6000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.00011804368841694668D));
            this.LabelDate.Multiline = false;
            this.LabelDate.Name = "LabelDate";
            this.LabelDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59992200136184692D), Telerik.Reporting.Drawing.Unit.Inch(0.15000000596046448D));
            this.LabelDate.Style.Color = System.Drawing.Color.Black;
            this.LabelDate.Style.Font.Bold = true;
            this.LabelDate.Style.Font.Italic = true;
            this.LabelDate.Style.Font.Name = "Tahoma";
            this.LabelDate.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.LabelDate.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(4D);
            this.LabelDate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.LabelDate.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.LabelDate.Value = "Date";
            // 
            // barcodePONumber
            // 
            this.barcodePONumber.Checksum = false;
            code39Encoder1.ShowText = false;
            this.barcodePONumber.Encoder = code39Encoder1;
            this.barcodePONumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.45015764236450195D));
            this.barcodePONumber.Module = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.barcodePONumber.Name = "barcodePONumber";
            this.barcodePONumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.29988169670105D), Telerik.Reporting.Drawing.Unit.Inch(0.44980320334434509D));
            this.barcodePONumber.Stretch = true;
            this.barcodePONumber.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodePONumber.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodePONumber.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodePONumber.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Inch(0.02500000037252903D);
            this.barcodePONumber.Value = "2122029095";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(2.1999607086181641D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.PanelLabels,
            this.panel1});
            this.detail.Name = "detail";
            // 
            // PanelLabels
            // 
            this.PanelLabels.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.MoveTo,
            this.MoveFromOrSoldTo});
            this.PanelLabels.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.PanelLabels.Name = "PanelLabels";
            this.PanelLabels.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4999213218688965D), Telerik.Reporting.Drawing.Unit.Inch(0.50003939867019653D));
            // 
            // MoveTo
            // 
            this.MoveTo.Anchoring = Telerik.Reporting.AnchoringStyles.Right;
            this.MoveTo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9998817443847656D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.MoveTo.Name = "MoveTo";
            instanceReportSource1.ReportDocument = null;
            this.MoveTo.ReportSource = instanceReportSource1;
            this.MoveTo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            // 
            // MoveFromOrSoldTo
            // 
            this.MoveFromOrSoldTo.Anchoring = Telerik.Reporting.AnchoringStyles.Left;
            this.MoveFromOrSoldTo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.MoveFromOrSoldTo.Name = "MoveFromOrSoldTo";
            instanceReportSource2.ReportDocument = null;
            this.MoveFromOrSoldTo.ReportSource = instanceReportSource2;
            this.MoveFromOrSoldTo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.TableItems,
            this.TotalQuantity,
            this.PalletWeight,
            this.PalletWeightLabel,
            this.TotalsLabel,
            this.TotalGrossWeight,
            this.TotalNetWeight});
            this.panel1.KeepTogether = false;
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.60007888078689575D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4998817443847656D), Telerik.Reporting.Drawing.Unit.Inch(1.4998817443847656D));
            this.panel1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // TableItems
            // 
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.31337907910346985D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.98265498876571655D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.329169511795044D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.81725001335144043D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.43347722291946411D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.37643587589263916D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.76004070043563843D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.80522608757019043D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.80100393295288086D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.66277557611465454D)));
            this.TableItems.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.21842923760414124D)));
            this.TableItems.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20833343267440796D)));
            this.TableItems.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.50000005960464478D)));
            this.TableItems.Body.SetCellContent(0, 0, this.Item_Quantity);
            this.TableItems.Body.SetCellContent(0, 1, this.Item_UnityPackSize);
            this.TableItems.Body.SetCellContent(0, 2, this.Item_Product);
            this.TableItems.Body.SetCellContent(0, 3, this.Item_LotNum);
            this.TableItems.Body.SetCellContent(0, 6, this.Item_CustCode);
            this.TableItems.Body.SetCellContent(0, 7, this.Item_CustLot);
            this.TableItems.Body.SetCellContent(0, 8, this.Item_Gross);
            this.TableItems.Body.SetCellContent(0, 9, this.Item_NetWeight);
            this.TableItems.Body.SetCellContent(0, 5, this.Item_Treatment);
            this.TableItems.Body.SetCellContent(0, 4, this.Item_LoBac);
            this.TableItems.Body.SetCellContent(0, 10, this.Item_Check);
            this.TableItems.Body.SetCellContent(1, 0, this.textBox3);
            this.TableItems.Body.SetCellContent(1, 1, this.textBox4);
            this.TableItems.Body.SetCellContent(1, 6, this.textBox9);
            this.TableItems.Body.SetCellContent(1, 10, this.textBox13);
            this.TableItems.Body.SetCellContent(1, 2, this.barcodeLotNumber, 1, 4);
            this.TableItems.Body.SetCellContent(1, 7, this.barcodeCustomerProduct, 1, 3);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.Item_QuantityHeader;
            tableGroup2.Name = "tableGroup2";
            tableGroup2.ReportItem = this.Item_UnityPackSizeHeader;
            tableGroup3.Name = "group";
            tableGroup3.ReportItem = this.Item_ProductHeader;
            tableGroup4.Name = "group2";
            tableGroup4.ReportItem = this.Item_LotNumHeader;
            tableGroup5.Name = "group1";
            tableGroup5.ReportItem = this.Item_LoBacHeader;
            tableGroup6.Name = "group3";
            tableGroup6.ReportItem = this.Item_TrtmtHeader;
            tableGroup7.Name = "group4";
            tableGroup7.ReportItem = this.Item_CustCodeHeader;
            tableGroup8.Name = "group5";
            tableGroup8.ReportItem = this.Item_CustLotHeader;
            tableGroup9.Name = "group6";
            tableGroup9.ReportItem = this.Item_GrossWeightHeader;
            tableGroup10.Name = "group7";
            tableGroup10.ReportItem = this.Item_NetWeightHeader;
            tableGroup11.Name = "group9";
            tableGroup11.ReportItem = this.Item_CheckHeader;
            this.TableItems.ColumnGroups.Add(tableGroup1);
            this.TableItems.ColumnGroups.Add(tableGroup2);
            this.TableItems.ColumnGroups.Add(tableGroup3);
            this.TableItems.ColumnGroups.Add(tableGroup4);
            this.TableItems.ColumnGroups.Add(tableGroup5);
            this.TableItems.ColumnGroups.Add(tableGroup6);
            this.TableItems.ColumnGroups.Add(tableGroup7);
            this.TableItems.ColumnGroups.Add(tableGroup8);
            this.TableItems.ColumnGroups.Add(tableGroup9);
            this.TableItems.ColumnGroups.Add(tableGroup10);
            this.TableItems.ColumnGroups.Add(tableGroup11);
            this.TableItems.ColumnHeadersPrintOnEveryPage = true;
            this.TableItems.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.Item_Quantity,
            this.Item_UnityPackSize,
            this.Item_Product,
            this.Item_LotNum,
            this.Item_CustCode,
            this.Item_CustLot,
            this.Item_Gross,
            this.Item_NetWeight,
            this.Item_Treatment,
            this.Item_LoBac,
            this.Item_Check,
            this.textBox3,
            this.textBox4,
            this.textBox9,
            this.textBox13,
            this.barcodeLotNumber,
            this.barcodeCustomerProduct,
            this.Item_QuantityHeader,
            this.Item_UnityPackSizeHeader,
            this.Item_ProductHeader,
            this.Item_LotNumHeader,
            this.Item_LoBacHeader,
            this.Item_TrtmtHeader,
            this.Item_CustCodeHeader,
            this.Item_CustLotHeader,
            this.Item_GrossWeightHeader,
            this.Item_NetWeightHeader,
            this.Item_CheckHeader});
            this.TableItems.KeepTogether = false;
            this.TableItems.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.TableItems.Name = "TableItems";
            tableGroup13.Name = "group8";
            tableGroup14.Name = "group10";
            tableGroup12.ChildGroups.Add(tableGroup13);
            tableGroup12.ChildGroups.Add(tableGroup14);
            tableGroup12.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup12.Name = "detailTableGroup";
            this.TableItems.RowGroups.Add(tableGroup12);
            this.TableItems.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4998421669006348D), Telerik.Reporting.Drawing.Unit.Inch(0.91666662693023682D));
            this.TableItems.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            // 
            // Item_Quantity
            // 
            this.Item_Quantity.Name = "Item_Quantity";
            this.Item_Quantity.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.31337907910346985D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_Quantity.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_Quantity.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Quantity.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Quantity.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Item_Quantity.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            // 
            // Item_UnityPackSize
            // 
            this.Item_UnityPackSize.Name = "Item_UnityPackSize";
            this.Item_UnityPackSize.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98265516757965088D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_UnityPackSize.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_UnityPackSize.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_UnityPackSize.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_UnityPackSize.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_UnityPackSize.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            // 
            // Item_Product
            // 
            this.Item_Product.Name = "Item_Product";
            this.Item_Product.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.329169511795044D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_Product.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_Product.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Product.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Product.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_Product.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.Item_Product.StyleName = "";
            // 
            // Item_LotNum
            // 
            this.Item_LotNum.Name = "Item_LotNum";
            this.Item_LotNum.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.81725025177001953D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_LotNum.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_LotNum.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LotNum.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LotNum.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_LotNum.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.Item_LotNum.StyleName = "";
            // 
            // Item_CustCode
            // 
            this.Item_CustCode.Name = "Item_CustCode";
            this.Item_CustCode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.7600405216217041D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_CustCode.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_CustCode.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustCode.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustCode.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_CustCode.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.Item_CustCode.StyleName = "";
            this.Item_CustCode.Value = "";
            // 
            // Item_CustLot
            // 
            this.Item_CustLot.Name = "Item_CustLot";
            this.Item_CustLot.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80522626638412476D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_CustLot.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_CustLot.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustLot.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_CustLot.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_CustLot.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.Item_CustLot.StyleName = "";
            this.Item_CustLot.TextWrap = true;
            // 
            // Item_Gross
            // 
            this.Item_Gross.Name = "Item_Gross";
            this.Item_Gross.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80100381374359131D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_Gross.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_Gross.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Gross.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Gross.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Item_Gross.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.Item_Gross.StyleName = "";
            // 
            // Item_NetWeight
            // 
            this.Item_NetWeight.Name = "Item_NetWeight";
            this.Item_NetWeight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66277575492858887D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_NetWeight.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_NetWeight.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_NetWeight.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_NetWeight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.Item_NetWeight.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.Item_NetWeight.StyleName = "";
            // 
            // Item_Treatment
            // 
            this.Item_Treatment.Name = "Item_Treatment";
            this.Item_Treatment.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.37643584609031677D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_Treatment.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_Treatment.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Treatment.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Treatment.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Item_Treatment.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Item_Treatment.StyleName = "";
            this.Item_Treatment.Value = "";
            // 
            // Item_LoBac
            // 
            this.Item_LoBac.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Item_LoBac.FalseValue = "";
            this.Item_LoBac.Name = "Item_LoBac";
            this.Item_LoBac.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.43347728252410889D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_LoBac.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_LoBac.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LoBac.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.Item_LoBac.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.Item_LoBac.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_LoBac.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Item_LoBac.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Item_LoBac.StyleName = "";
            this.Item_LoBac.Text = "";
            this.Item_LoBac.TextWrap = false;
            this.Item_LoBac.TrueValue = "";
            this.Item_LoBac.Value = "";
            // 
            // Item_Check
            // 
            this.Item_Check.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Item_Check.FalseValue = "";
            this.Item_Check.Name = "Item_Check";
            this.Item_Check.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.21842926740646362D), Telerik.Reporting.Drawing.Unit.Inch(0.20833347737789154D));
            this.Item_Check.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Item_Check.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Check.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.Item_Check.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Item_Check.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Item_Check.StyleName = "";
            this.Item_Check.Text = "";
            this.Item_Check.TrueValue = "";
            this.Item_Check.Value = "=false";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.31337907910346985D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox3.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox3.StyleName = "";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98265498876571655D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox4.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox4.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox4.StyleName = "";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.76004070043563843D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox9.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox9.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox9.StyleName = "";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.21842923760414124D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox13.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox13.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox13.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox13.StyleName = "";
            // 
            // barcodeLotNumber
            // 
            this.barcodeLotNumber.Checksum = false;
            code39Encoder2.ShowText = false;
            this.barcodeLotNumber.Encoder = code39Encoder2;
            this.barcodeLotNumber.Module = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.barcodeLotNumber.Name = "barcodeLotNumber";
            this.barcodeLotNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9563326835632324D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.barcodeLotNumber.Stretch = true;
            this.barcodeLotNumber.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.barcodeLotNumber.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeLotNumber.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeLotNumber.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeLotNumber.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeLotNumber.StyleName = "";
            this.barcodeLotNumber.Value = "03 16 243 41";
            // 
            // barcodeCustomerProduct
            // 
            this.barcodeCustomerProduct.Checksum = false;
            this.barcodeCustomerProduct.Encoder = code39Encoder2;
            this.barcodeCustomerProduct.Module = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.barcodeCustomerProduct.Name = "barcodeCustomerProduct";
            this.barcodeCustomerProduct.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2690055370330811D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.barcodeCustomerProduct.Stretch = true;
            this.barcodeCustomerProduct.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.barcodeCustomerProduct.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeCustomerProduct.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeCustomerProduct.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeCustomerProduct.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.barcodeCustomerProduct.StyleName = "";
            this.barcodeCustomerProduct.Value = "140-711";
            // 
            // TotalQuantity
            // 
            this.TotalQuantity.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0033286411780864D), Telerik.Reporting.Drawing.Unit.Inch(1.199881911277771D));
            this.TotalQuantity.Name = "TotalQuantity";
            this.TotalQuantity.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.39663195610046387D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.TotalQuantity.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TotalQuantity.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.TotalQuantity.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalQuantity.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalQuantity.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.TotalQuantity.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.TotalQuantity.Value = "";
            // 
            // PalletWeight
            // 
            this.PalletWeight.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.8236861228942871D), Telerik.Reporting.Drawing.Unit.Inch(0.94996070861816406D));
            this.PalletWeight.Name = "PalletWeight";
            this.PalletWeight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80767041444778442D), Telerik.Reporting.Drawing.Unit.Inch(0.19992128014564514D));
            this.PalletWeight.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.PalletWeight.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.PalletWeight.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.PalletWeight.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.PalletWeight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.PalletWeight.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.PalletWeight.Value = "";
            // 
            // PalletWeightLabel
            // 
            this.PalletWeightLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.988213062286377D), Telerik.Reporting.Drawing.Unit.Inch(0.94996070861816406D));
            this.PalletWeightLabel.Name = "PalletWeightLabel";
            this.PalletWeightLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.83547192811965942D), Telerik.Reporting.Drawing.Unit.Inch(0.19992128014564514D));
            this.PalletWeightLabel.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.PalletWeightLabel.Style.Font.Bold = true;
            this.PalletWeightLabel.Style.Font.Italic = true;
            this.PalletWeightLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.PalletWeightLabel.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.PalletWeightLabel.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.PalletWeightLabel.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.PalletWeightLabel.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.PalletWeightLabel.Value = "Pallet Weight";
            // 
            // TotalsLabel
            // 
            this.TotalsLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.988213062286377D), Telerik.Reporting.Drawing.Unit.Inch(1.2499605417251587D));
            this.TotalsLabel.Name = "TotalsLabel";
            this.TotalsLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.83547192811965942D), Telerik.Reporting.Drawing.Unit.Inch(0.19992128014564514D));
            this.TotalsLabel.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.TotalsLabel.Style.Font.Bold = true;
            this.TotalsLabel.Style.Font.Italic = true;
            this.TotalsLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.TotalsLabel.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalsLabel.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalsLabel.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.TotalsLabel.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.TotalsLabel.Value = "Totals";
            // 
            // TotalGrossWeight
            // 
            this.TotalGrossWeight.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.8237643241882324D), Telerik.Reporting.Drawing.Unit.Inch(1.199881911277771D));
            this.TotalGrossWeight.Name = "TotalGrossWeight";
            this.TotalGrossWeight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80759203433990479D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.TotalGrossWeight.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TotalGrossWeight.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.TotalGrossWeight.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalGrossWeight.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalGrossWeight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.TotalGrossWeight.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.TotalGrossWeight.Value = "";
            // 
            // TotalNetWeight
            // 
            this.TotalNetWeight.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.6313567161560059D), Telerik.Reporting.Drawing.Unit.Inch(1.199881911277771D));
            this.TotalNetWeight.Name = "TotalNetWeight";
            this.TotalNetWeight.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.64540410041809082D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.TotalNetWeight.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TotalNetWeight.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.TotalNetWeight.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalNetWeight.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.TotalNetWeight.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.TotalNetWeight.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.TotalNetWeight.Value = "";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.2000783234834671D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.Footer_MoveNum,
            this.Footer_PageNumber,
            this.Footer_DateTime});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // Footer_MoveNum
            // 
            this.Footer_MoveNum.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0033680598717182875D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.Footer_MoveNum.Name = "Footer_MoveNum";
            this.Footer_MoveNum.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99663197994232178D), Telerik.Reporting.Drawing.Unit.Inch(0.20003890991210938D));
            this.Footer_MoveNum.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.Footer_MoveNum.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Footer_MoveNum.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Footer_MoveNum.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Footer_MoveNum.Value = "";
            // 
            // Footer_PageNumber
            // 
            this.Footer_PageNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.5033698081970215D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.Footer_PageNumber.Name = "Footer_PageNumber";
            this.Footer_PageNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99663197994232178D), Telerik.Reporting.Drawing.Unit.Inch(0.20003890991210938D));
            this.Footer_PageNumber.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.Footer_PageNumber.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Footer_PageNumber.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Footer_PageNumber.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Footer_PageNumber.Value = "=\'Page \' + PageNumber + \' of \' + PageCount";
            // 
            // Footer_DateTime
            // 
            this.Footer_DateTime.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.Footer_DateTime.Name = "Footer_DateTime";
            this.Footer_DateTime.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5000004768371582D), Telerik.Reporting.Drawing.Unit.Inch(0.20003890991210938D));
            this.Footer_DateTime.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.Footer_DateTime.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.Footer_DateTime.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Footer_DateTime.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.Footer_DateTime.Value = "=Now()";
            // 
            // packingListDataSource
            // 
            this.packingListDataSource.DataMember = "GetPackingList";
            this.packingListDataSource.DataSource = typeof(RioValleyChili.Client.Reporting.Services.InventoryShipmentOrderReportingService);
            this.packingListDataSource.Name = "packingListDataSource";
            this.packingListDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("orderKey", typeof(string), "= Parameters.OrderKey.Value")});
            // 
            // PackingListBarcodeReport
            // 
            this.DataSource = this.packingListDataSource;
            this.DocumentName = "Packing List Barcode Report";
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "PackingListBarcodeReport";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowBlank = false;
            reportParameter1.Name = "OrderKey";
            reportParameter1.Text = "Order Key";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.5000014305114746D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.SubReport MoveFromOrSoldTo;
        private Telerik.Reporting.SubReport MoveTo;
        private Telerik.Reporting.ObjectDataSource packingListDataSource;
        private Telerik.Reporting.PictureBox pictureBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox LabelTitle;
        private Telerik.Reporting.Panel MoveNumberPanel;
        private Telerik.Reporting.TextBox MoveNum;
        private Telerik.Reporting.TextBox LabelMoveNumber;
        private Telerik.Reporting.TextBox PO;
        private Telerik.Reporting.TextBox LabelPO;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.TextBox LabelDate;
        private Telerik.Reporting.Table TableItems;
        private Telerik.Reporting.TextBox Item_Quantity;
        private Telerik.Reporting.TextBox Item_UnityPackSize;
        private Telerik.Reporting.TextBox Item_Product;
        private Telerik.Reporting.TextBox Item_LotNum;
        private Telerik.Reporting.TextBox Item_CustCode;
        private Telerik.Reporting.TextBox Item_CustLot;
        private Telerik.Reporting.TextBox Item_Gross;
        private Telerik.Reporting.TextBox Item_NetWeight;
        private Telerik.Reporting.TextBox Item_Treatment;
        private Telerik.Reporting.CheckBox Item_LoBac;
        private Telerik.Reporting.TextBox Item_QuantityHeader;
        private Telerik.Reporting.TextBox Item_UnityPackSizeHeader;
        private Telerik.Reporting.TextBox Item_ProductHeader;
        private Telerik.Reporting.TextBox Item_LotNumHeader;
        private Telerik.Reporting.TextBox Item_LoBacHeader;
        private Telerik.Reporting.TextBox Item_TrtmtHeader;
        private Telerik.Reporting.TextBox Item_CustCodeHeader;
        private Telerik.Reporting.TextBox Item_CustLotHeader;
        private Telerik.Reporting.TextBox Item_GrossWeightHeader;
        private Telerik.Reporting.TextBox Item_NetWeightHeader;
        private Telerik.Reporting.TextBox Item_CheckHeader;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.CheckBox Item_Check;
        private Telerik.Reporting.TextBox TotalQuantity;
        private Telerik.Reporting.Panel PanelLabels;
        private Telerik.Reporting.TextBox PalletWeight;
        private Telerik.Reporting.TextBox PalletWeightLabel;
        private Telerik.Reporting.TextBox TotalsLabel;
        private Telerik.Reporting.TextBox TotalGrossWeight;
        private Telerik.Reporting.TextBox TotalNetWeight;
        private Telerik.Reporting.TextBox Footer_MoveNum;
        private Telerik.Reporting.TextBox Footer_PageNumber;
        private Telerik.Reporting.TextBox Footer_DateTime;
        private Telerik.Reporting.Barcode barcodePONumber;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.Barcode barcodeLotNumber;
        private Telerik.Reporting.Barcode barcodeCustomerProduct;
    }
}