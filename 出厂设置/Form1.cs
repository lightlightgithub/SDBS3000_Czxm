using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 出厂设置
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Font font;
        private void Form1_Load(object sender, EventArgs e)
        {
            tabset.Enter += Tabset_Enter;
            dataGridView1.RowPostPaint += DataGridView1_RowPostPaint;
            font = new Font("微软雅黑", 12F);
        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // 获取行头列的矩形区域
            Rectangle rectangle = new Rectangle(
                e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dataGridView1.RowHeadersWidth - 4,
                e.RowBounds.Height);

            // 在行头列中绘制数字（从1开始）
            TextRenderer.DrawText(
                e.Graphics,
                (e.RowIndex + 1).ToString(),
                font,
                rectangle,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void Tabset_Enter(object sender, EventArgs e)
        {
            try
            {
                using (CodeFirstDbContext Entity = new CodeFirstDbContext())
                {
                    BindingSource binding = new BindingSource();
                    binding.DataSource = Entity.Dictionarys.ToList();
                    dataGridView1.DataSource = binding;
                    dataGridView1.Columns["code"].HeaderText = "功能码";
                    dataGridView1.Columns["name"].HeaderText = "名称";
                    dataGridView1.Columns["plcaddr"].HeaderText = "PLC地址";
                    dataGridView1.Columns["value"].HeaderText = "设置值";
                    dataGridView1.Columns["note"].HeaderText = "备注";
                    dataGridView1.Columns["active"].HeaderText = "激活";
                    dataGridView1.Columns["time"].HeaderText = "时间";
                }

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.DefaultCellStyle.Font = font;
                    col.Width = 130;
                }
                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.Columns["note"].Width = 350;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = font;
                dataGridView1.AllowUserToAddRows = true;
                // 允许用户编辑单元格
                dataGridView1.ReadOnly = false;

                // （可选）为新行提供一个不同的背景色
                //dataGridView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dataGridView_RowPostPaint);

                //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int hs = 0;
            try
            {
                using (CodeFirstDbContext Entity = new CodeFirstDbContext())
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        hs = row.Index + 1;
                        if (!row.IsNewRow) // 忽略新行
                        {
                            // 假设DataGridView有两列，分别是Id和Name
                            int id = Convert.ToInt32(row.Cells["id"].Value);
                            string name = row.Cells["name"].Value.ToString();
                            float value = Convert.ToSingle(row.Cells["value"].Value);
                            string note = row.Cells["note"].Value?.ToString();
                            string addr = row.Cells["plcaddr"].Value?.ToString();
                            bool active = (bool)row.Cells["active"].Value;
                            int code = Convert.ToInt32(row.Cells["code"].Value);
                            if (id != 0)
                            {
                                // 查找数据库中的对应实体
                                var entity = Entity.Dictionarys.Find(id);
                                // 更新实体的属性
                                entity.name = name;
                                entity.value = value;
                                entity.note = note;
                                entity.active = active;
                                entity.code = code;
                                entity.plcaddr = addr;
                                entity.time = DateTime.Now;
                            }
                            else
                            {
                                T_Dictionary dictionary = (T_Dictionary)row.DataBoundItem;
                                dictionary.time = DateTime.Now;
                                Entity.Dictionarys.Add(dictionary);
                            }
                        }

                    }

                    int i = Entity.SaveChanges();
                    Tabset_Enter(this, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("第" + hs + "行保存失败" + ex.ToString());
            }
        }
    }
}
