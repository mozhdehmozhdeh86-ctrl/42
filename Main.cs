using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Paint
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        //تعریف متغیرهای موردد نیاز
        Bitmap bmp_image;
        Graphics g;
        Pen p = new Pen(Brushes.Black, 3);
        LinearGradientBrush linearBr;
        Boolean fill;
        public string action = "قلم";
        public Font font;
        public string text;
        Point start_pnt;
        int width, height, start_pnt_X, start_pnt_Y;

        private void Form1_Load(object sender, EventArgs e)
        {
            //صدازدن تابع فایل جدید به منظور ساخت کاغذ نقاشی
            New_file();
            p.Color = front_def_color.BackColor;
            pictureBox1.Cursor = new Cursor("cross.cur");
        }

        void New_file()
        {
            //
            bmp_image = null;
            bmp_image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp_image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp_image.Width, bmp_image.Height));
            pictureBox1.Image = bmp_image;
        }

        private void panel10_DoubleClick(object sender, EventArgs e)
        {
            Panel panel = (Panel)sender;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                panel.BackColor = colorDialog1.Color;
            p.Color = front_def_color.BackColor;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Color temp_c=back_def_color.BackColor;
            back_def_color.BackColor= front_def_color.BackColor;
            front_def_color.BackColor = temp_c;
            p.Color = front_def_color.BackColor;
        }

        private void panel10_Click(object sender, EventArgs e)
        {
            //انتخاب رنگ کلیم شده برای قلم در حال استفاده
            Panel panel=(Panel)sender;
            front_def_color.BackColor = panel.BackColor;
            p.Color = front_def_color.BackColor;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                switch (action)
                {
                    case "متن":
                        if (!string.IsNullOrEmpty(text))
                        {
                            pictureBox1.Image = bmp_image;
                            SizeF size = g.MeasureString(text, font);
                            Bitmap temp_bmp0 = new Bitmap(bmp_image);
                            Graphics temp_g0 = Graphics.FromImage(temp_bmp0);
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, (int)size.Width, (int)size.Height), Color.FromArgb(147, front_def_color.BackColor), Color.FromArgb(147, front_def_color.BackColor), LinearGradientMode.BackwardDiagonal);
                            temp_g0.DrawString(text, font, linearBr, e.X, e.Y);
                            pictureBox1.Image = temp_bmp0;
                        }
                        break;
                    case "قلم":
                        if (e.Button == MouseButtons.Left)
                        {
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, 4, 4), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                            g.FillEllipse(linearBr, (float)e.X - (p.Width / 2), (float)e.Y - (p.Width / 2), p.Width, p.Width);
                        }
                        pictureBox1.Image = bmp_image;
                        break;
                    case "خواندن رنگ":
                        if (e.Button == MouseButtons.Left)
                            front_def_color.BackColor = Color.FromArgb(bmp_image.GetPixel(e.X, e.Y).R, bmp_image.GetPixel(e.X, e.Y).G, bmp_image.GetPixel(e.X, e.Y).B);
                        p.Color = front_def_color.BackColor;
                        break;
                    case "شعاع دایره":
                        pictureBox1.Image = bmp_image;
                        Bitmap temp_bmp = new Bitmap(bmp_image);
                        Graphics temp_g = Graphics.FromImage(temp_bmp);
                        int radius = Math.Max(Math.Abs(start_pnt.X - e.X), Math.Abs(start_pnt.Y - e.Y));
                        if (fill)
                        {
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, bmp_image.Width, bmp_image.Height), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                            temp_g.FillEllipse(linearBr, (float)start_pnt.X - radius, (float)start_pnt.Y - radius, (float)radius * 2, (float)radius * 2);
                        }
                        else
                            temp_g.DrawEllipse(p, (float)start_pnt.X - radius, (float)start_pnt.Y - radius, (float)radius * 2, (float)radius * 2);
                        pictureBox1.Image = temp_bmp;
                        this.toolStripStatusLabel1.Text = radius.ToString();
                        break;
                    case "عرض مربع":
                        pictureBox1.Image = bmp_image;
                        Bitmap temp_bmp2 = new Bitmap(bmp_image);
                        Graphics temp_g2 = Graphics.FromImage(temp_bmp2);
                        temp_g2.DrawLine(p, start_pnt, new Point(e.X, start_pnt.Y));
                        start_pnt_X = Math.Min(e.X, start_pnt.X);
                        width = Math.Abs(e.X - start_pnt.X);
                        pictureBox1.Image = temp_bmp2;
                        this.toolStripStatusLabel1.Text = width.ToString() + "×" + height.ToString();
                        break;
                    case "طول مربع":
                        pictureBox1.Image = bmp_image;
                        Bitmap temp_bmp3 = new Bitmap(bmp_image);
                        Graphics temp_g3 = Graphics.FromImage(temp_bmp3);
                        height = Math.Abs(e.Y - start_pnt.Y);
                        start_pnt_Y = Math.Min(e.Y, start_pnt.Y);
                        if (fill)
                        {
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, bmp_image.Width, bmp_image.Height), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                            temp_g3.FillRectangle(linearBr, new Rectangle(start_pnt_X, start_pnt_Y, width, height));
                        }
                        else
                        temp_g3.DrawRectangle(p, new Rectangle(start_pnt_X, start_pnt_Y, width, height));
                        pictureBox1.Image = temp_bmp3;
                        this.toolStripStatusLabel1.Text = width.ToString() + "×" + height.ToString();
                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void panel14_Click(object sender, EventArgs e)
        {
            Panel panel = (Panel)sender;
            panel14.BorderStyle = panel11.BorderStyle = panel12.BorderStyle = panel13.BorderStyle = panel14.BorderStyle = panel15.BorderStyle = panel16.BorderStyle = BorderStyle.None;
            panel.BorderStyle = BorderStyle.FixedSingle;
            p.Width =int.Parse(panel.Tag.ToString());
        }

        private void فایلجدیدToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("از ایجاد فایل جدید اطمینان دارید؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                New_file();
        }

        private void بازکردنفایلToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile( openFileDialog1.FileName);
                bmp_image = new Bitmap(pictureBox1.Image);
                g = Graphics.FromImage(bmp_image);
            }
        }

        private void خروجToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ذخیرهکردنعکسToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPEG File (*.jpg)|*.jpg|Bitmap File (*.bmp)|*.bmp|PNG File(*.png)|*.png";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(saveFileDialog1.FileName))
                {
                    case ".jpg":
                        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Jpeg); break;
                    case ".bmp":
                        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Bmp); break;
                    case ".png":
                        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Png); break;
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Set_border(pictureBox2);
            action = "متن";
            Add_string addStr = new Add_string(font,text);
            addStr.Tag = this;
            addStr.ShowDialog();
        }
//------------------------------------------- SetBorder
        void Set_border(PictureBox picB)
        {
            pictureBox2.BorderStyle = pictureBox3.BorderStyle = pictureBox4.BorderStyle = pictureBox5.BorderStyle = pictureBox6.BorderStyle = pictureBox7.BorderStyle = pictureBox8.BorderStyle = pictureBox9.BorderStyle = BorderStyle.None;
            picB.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            action = "قلم";
            Set_border(pictureBox3);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                switch (action)
                {
                    case "متن":
                        if (!string.IsNullOrEmpty(text))
                        {
                            pictureBox1.Image = bmp_image;
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, bmp_image.Width, bmp_image.Height), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                            g.DrawString(text, font, linearBr, e.X, e.Y);
                        }
                        break;
                    case "قلم":
                        if (e.Button == MouseButtons.Left)
                        {
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, 4, 4), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                            g.FillEllipse(linearBr, (float)e.X - (p.Width / 2), (float)e.Y - (p.Width / 2), p.Width, p.Width);
                        }
                        break;
                    case "نقطه شروع":
                        start_pnt = new Point(e.X, e.Y);
                        action = "خط";
                        break;
                    case "خط":
                        linearBr = new LinearGradientBrush(new Rectangle(0, 0, 4, 4), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                        g.DrawLine(p, start_pnt, new Point(e.X, e.Y));
                        start_pnt = new Point(e.X, e.Y);
                        break;
                    case "خواندن رنگ":
                        front_def_color.BackColor = Color.FromArgb(bmp_image.GetPixel(e.X, e.Y).R, bmp_image.GetPixel(e.X, e.Y).G, bmp_image.GetPixel(e.X, e.Y).B);
                        p.Color = front_def_color.BackColor;
                        break;
                    case "نقطه مرکزی دایره":
                        start_pnt = new Point(e.X, e.Y);
                        action = "شعاع دایره";
                        break;
                    case "شعاع دایره":
                        int radius = Math.Max(Math.Abs(start_pnt.X - e.X), Math.Abs(start_pnt.Y - e.Y));
                        if (fill)
                        {
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, bmp_image.Width, bmp_image.Height), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                            g.FillEllipse(linearBr, (float)start_pnt.X - radius, (float)start_pnt.Y - radius, (float)radius * 2, (float)radius * 2);
                        }
                        else
                            g.DrawEllipse(p, (float)start_pnt.X - radius, (float)start_pnt.Y - radius, (float)radius * 2, (float)radius * 2);
                        action = "نقطه مرکزی دایره";
                        break;
                    case "شروع مربع":
                        start_pnt = new Point(e.X, e.Y);
                        action = "عرض مربع";
                        break;
                    case "عرض مربع":
                        action = "طول مربع";
                        break;
                    case "طول مربع":
                        if (fill)
                        {
                            linearBr = new LinearGradientBrush(new Rectangle(0, 0, bmp_image.Width, bmp_image.Height), front_def_color.BackColor, front_def_color.BackColor, LinearGradientMode.BackwardDiagonal);
                            g.FillRectangle(linearBr, new Rectangle(start_pnt_X, start_pnt_Y, width, height));
                        }
                        else
                            g.DrawRectangle(p, new Rectangle(start_pnt_X, start_pnt_Y, width, height));
                        action = "شروع مربع";
                        break;

                }
                pictureBox1.Image = bmp_image;
                this.toolStripStatusLabel1.Text = "";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Set_border(pictureBox4);
            action = "نقطه شروع";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Set_border(pictureBox5);
            action = "خواندن رنگ";
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Set_border(pictureBox6);
            action = "نقطه مرکزی دایره";
            fill = false;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Set_border(pictureBox7);
            action = "شروع مربع";
            fill = false;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Set_border(pictureBox9);
            action = "نقطه مرکزی دایره";
            fill = true;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Set_border(pictureBox8);
            action = "شروع مربع";
            fill = true;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "درج متن";
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "قلم";
        }

        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "تشخیص رنگ";
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "رسم چند ضلعی";
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "رسم دایره توخالی";
        }

        private void pictureBox7_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "رسم چهارضلعی توخالی";
        }

        private void pictureBox9_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "رسم دایره توپر";
        }

        private void pictureBox8_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "رسم چهار ضلعی توپر";
        }

        private void panel16_MouseEnter(object sender, EventArgs e)
        {
            Panel p=(Panel)sender;
            toolStripStatusLabel1.Text = "قلم اندازه " + p.Tag.ToString();
        }

        private void pictureBox8_MouseLeave(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "جابجایی رنگ های زیر و زبر";
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("آیا قصد خروج از برنامه را دارید؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                e.Cancel = false;
            else e.Cancel = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}