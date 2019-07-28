﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using operator_morena.Connection;
using operator_morena.Models;
using System.IO;

namespace operator_morena
{
    public partial class wfDashBoard : MaterialForm
    {
        double LatIncial = 19.043719;
        double LngIncial = -98.198911;

        #region FUNCIONES
        private int check_score()
        {
            if (chb1.Checked)
                return 1;
            if (chb2.Checked)
                return 2;
            if (chb3.Checked)
                return 3;
            if (chb4.Checked)
                return 4;
            if (chb5.Checked)
                return 5;
            return 0;
        }

        private byte[] ConvertImageToBinary(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        #endregion

        public wfDashBoard()
        {
            InitializeComponent();

            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Red900, //Color dialog 
                Primary.Red900, //Color control buttons
                Primary.Blue400,
                Accent.LightBlue200,
                TextShade.WHITE

            );
        }

        private void WfDashBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string image_name;
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Archivos de Imagen JPG|*.jpg|Todos los archivos|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                image_name = ofd.FileName;
                pbImagen.Image = Image.FromFile(image_name);
            }


        }

        private void wfDashBoard_Load(object sender, EventArgs e)
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(LatIncial, LngIncial);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 9;
            gMapControl1.AutoScroll = true;

            ConnectionDB db = new ConnectionDB();

            List<string> sections = db.Sections.Select(x => x.section).Distinct().ToList();

            cbSection.Items.Clear();
            cbSection.DataSource = sections;
        }

        #region BOTONES
        private void tsbtnNuevo_Click(object sender, EventArgs e)
        {
            tsbtnGuarda.Visible = true;
            tsbtnCancela.Visible = true;
            tsbtnNuevo.Visible = false;

            tbName.Enabled = true;
            tbAlias.Enabled = true;
            tbEmail.Enabled = true;
            tbPhone.Enabled = true;

            rtbComents.Enabled = true;

            chb1.Enabled = true;
            chb2.Enabled = true;
            chb3.Enabled = true;
            chb4.Enabled = true;
            chb5.Enabled = true;

            cbSection.Enabled = true;
            cbPopulation.Enabled = true;
            cbMunicipality.Enabled = true;
        }

        private void tsbtnCancela_Click(object sender, EventArgs e)
        {
            tsbtnGuarda.Visible = false;
            tsbtnCancela.Visible = false;
            tsbtnNuevo.Visible = true;

            tbName.Enabled = false;
            tbAlias.Enabled = false;
            tbEmail.Enabled = false;
            tbPhone.Enabled = false;

            rtbComents.Enabled = false;

            chb1.Enabled = false;
            chb2.Enabled = false;
            chb3.Enabled = false;
            chb4.Enabled = false;
            chb5.Enabled = false;


            cbSection.Enabled = false;
            cbPopulation.Enabled = false;
            cbMunicipality.Enabled = false;
        }

        private void tsbtnGuarda_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tbAlias.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tbEmail.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbEmail.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPhone.Focus();
                return;
            }
            if(chb1.Checked == false && chb2.Checked == false && chb3.Checked == false && chb4.Checked == false && chb5.Checked == false)
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(cbSection.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbSection.Focus();
                return;
            }
            if (string.IsNullOrEmpty(cbMunicipality.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbMunicipality.Focus();
                return;
            }
            if (string.IsNullOrEmpty(cbPopulation.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbPopulation.Focus();
                return;
            }

            ConnectionDB db = new ConnectionDB();

            Section sections = db.Sections.Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text && x.location_name == cbPopulation.Text)
                            .FirstOrDefault();

            Operator operators = new Operator();
            operators.name = tbName.Text;
            operators.alias = tbAlias.Text;
            operators.email = tbEmail.Text;
            operators.phone = tbPhone.Text;
            operators.observation = rtbComents.Text;
            operators.score = check_score();
            operators.id_sections = sections.id;
            operators.image = ConvertImageToBinary(pbImagen.Image);

            db.Operators.Add(operators);
            db.SaveChanges();

            MessageBox.Show("Registro realizado con éxito", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);

            List<Operator> operator_s = db.Operators.ToList();
            dgvOperator.DataSource = operator_s;
        }
        #endregion

        #region COMBO BOX
        private void cbSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbMunicipality.Items.Clear();
            cbPopulation.Items.Clear();
            
            ConnectionDB db = new ConnectionDB();

            List<string> municipality = db.Sections.Where(x => x.section == cbSection.Text).Select(x => x.town_name).Distinct().ToList();

            foreach (string item in municipality)
            {
                cbMunicipality.Items.Add(item);
            }

            cbMunicipality.Text = string.Empty;
            cbPopulation.Text = string.Empty;
        }

        private void cbMunicipality_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbPopulation.Items.Clear();
            ConnectionDB db = new ConnectionDB();

            List<string> population = db.Sections.Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text).
                Select(x => x.location_name).ToList();

            foreach (string item in population)
            {
                cbPopulation.Items.Add(item);
            }
        }
        #endregion

        #region CHECKBOX
        private void chb1_CheckedChanged(object sender, EventArgs e)
        {
            chb2.Checked = false;
            chb3.Checked = false;
            chb4.Checked = false;
            chb5.Checked = false;
        }

        private void chb2_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb3.Checked = false;
            chb4.Checked = false;
            chb5.Checked = false;
        }

        private void chb3_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb2.Checked = false;
            chb4.Checked = false;
            chb5.Checked = false;
        }

        private void chb4_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb2.Checked = false;
            chb3.Checked = false;
            chb5.Checked = false;
        }

        private void chb5_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb2.Checked = false;
            chb3.Checked = false;
            chb4.Checked = false;
        }
        #endregion
    }
}
