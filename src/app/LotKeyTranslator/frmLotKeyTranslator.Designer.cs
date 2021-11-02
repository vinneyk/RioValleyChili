namespace LotKeyTranslator
{
    partial class frmLotKeyTranslator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lotDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.lotDateSequence = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lotTypeId = new System.Windows.Forms.NumericUpDown();
            this.lotNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtQuery = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.lotDateSequence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lotTypeId)).BeginInit();
            this.SuspendLayout();
            // 
            // lotDatePicker
            // 
            this.lotDatePicker.CustomFormat = "yyyy-MM-dd";
            this.lotDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.lotDatePicker.Location = new System.Drawing.Point(66, 9);
            this.lotDatePicker.Name = "lotDatePicker";
            this.lotDatePicker.Size = new System.Drawing.Size(200, 20);
            this.lotDatePicker.TabIndex = 0;
            this.lotDatePicker.ValueChanged += new System.EventHandler(this.lotDatePicker_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Lot Date";
            // 
            // lotDateSequence
            // 
            this.lotDateSequence.Location = new System.Drawing.Point(96, 35);
            this.lotDateSequence.Name = "lotDateSequence";
            this.lotDateSequence.Size = new System.Drawing.Size(170, 20);
            this.lotDateSequence.TabIndex = 2;
            this.lotDateSequence.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lotDateSequence.ValueChanged += new System.EventHandler(this.lotDateSequence_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lot Sequence";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Lot Type Id";
            // 
            // lotTypeId
            // 
            this.lotTypeId.Location = new System.Drawing.Point(96, 61);
            this.lotTypeId.Name = "lotTypeId";
            this.lotTypeId.Size = new System.Drawing.Size(170, 20);
            this.lotTypeId.TabIndex = 5;
            this.lotTypeId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lotTypeId.ValueChanged += new System.EventHandler(this.lotTypeId_ValueChanged);
            // 
            // lotNumber
            // 
            this.lotNumber.Location = new System.Drawing.Point(96, 87);
            this.lotNumber.Name = "lotNumber";
            this.lotNumber.Size = new System.Drawing.Size(170, 20);
            this.lotNumber.TabIndex = 6;
            this.lotNumber.TextChanged += new System.EventHandler(this.lotNumber_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Lot Number";
            // 
            // txtQuery
            // 
            this.txtQuery.Location = new System.Drawing.Point(12, 123);
            this.txtQuery.Multiline = true;
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(253, 194);
            this.txtQuery.TabIndex = 8;
            // 
            // frmLotKeyTranslator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 329);
            this.Controls.Add(this.txtQuery);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lotNumber);
            this.Controls.Add(this.lotTypeId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lotDateSequence);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lotDatePicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmLotKeyTranslator";
            this.Text = "LotKey Translator";
            this.Load += new System.EventHandler(this.frmLotKeyTranslator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lotDateSequence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lotTypeId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker lotDatePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown lotDateSequence;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown lotTypeId;
        private System.Windows.Forms.TextBox lotNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtQuery;
    }
}

