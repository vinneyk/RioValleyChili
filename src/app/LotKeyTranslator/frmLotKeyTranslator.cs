using System;
using System.Windows.Forms;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Utilities;

namespace LotKeyTranslator
{
    public partial class frmLotKeyTranslator : Form
    {
        public frmLotKeyTranslator()
        {
            InitializeComponent();
        }

        private void frmLotKeyTranslator_Load(object sender, EventArgs e)
        {
            BuildLotNumber();
        }

        private void lotNumber_TextChanged(object sender, EventArgs e)
        {
            ParseLotNumber();
        }

        private void lotDatePicker_ValueChanged(object sender, EventArgs e)
        {
            BuildLotNumber();
        }

        private void lotDateSequence_ValueChanged(object sender, EventArgs e)
        {
            BuildLotNumber();
        }

        private void lotTypeId_ValueChanged(object sender, EventArgs e)
        {
            BuildLotNumber();
        }

        private void ParseLotNumber()
        {
            int lotNum;
            if(int.TryParse(lotNumber.Text, out lotNum))
            {
                LotKey lotKey;
                if(LotNumberParser.ParseLotNumber(lotNum, out lotKey))
                {
                    lotDatePicker.Value = lotKey.LotKey_DateCreated;
                    lotDateSequence.Value = lotKey.LotKey_DateSequence;
                    lotTypeId.Value = lotKey.LotKey_LotTypeId;
                }
                SetQuery();
            }
        }

        private void BuildLotNumber()
        {
            lotNumber.Text = LotNumberParser.BuildLotNumber(new LotKeyImplementation
                {
                    LotKey_DateCreated = lotDatePicker.Value,
                    LotKey_DateSequence = (int) lotDateSequence.Value,
                    LotKey_LotTypeId = (int) lotTypeId.Value
                }).ToString();
            SetQuery();
        }

        private void SetQuery()
        {
            txtQuery.Text = string.Format("WHERE LotDateCreated = '{0}' and LotDateSequence = {1} and LotTypeId = {2}", lotDatePicker.Value.ToString("yyyy-MM-dd"), lotDateSequence.Value, lotTypeId.Value);
        }

        private class LotKeyImplementation : ILotKey
        {
            public DateTime LotKey_DateCreated { get; set; }
            public int LotKey_DateSequence { get; set; }
            public int LotKey_LotTypeId { get; set; }
        }
    }
}
