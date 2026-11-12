
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bai52
{
    public partial class Form1 : Form
    {
        // Lớp lưu thông tin dịch vụ
        private class Service
        {
            public string Name { get; set; }
            public decimal Price { get; set; }

            public Service(string name, decimal price)
            {
                Name = name;
                Price = price;
            }

            public override string ToString()
            {
                return Name + " - " + Price.ToString("N0") + " VNĐ";
            }
        }

        // Khai báo Control
        private ComboBox cboCategory;
        private ListBox lstAvailableServices;
        private ListBox lstSelectedServices;
        private ComboBox cboDiscount;

        private Button btnSelect;
        private Button btnRemove;
        private Button btnClearAll;

        private Label lblTotal;
        private Label lblPayment;

        private Dictionary<string, List<Service>> serviceData;

        public Form1()
        {
            CreateInterface();
            InitializeServiceData();
            LoadCategories();
            CalculateTotal();
        }

        // Tạo giao diện
        private void CreateInterface()
        {
            this.Text = "Bài 5.2 - Quản lý dịch vụ và tính tiền";
            this.Size = new Size(1050, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 250);
            this.Font = new Font("Segoe UI", 10);

            // Khung chính
            Panel mainPanel = new Panel();
            mainPanel.Size = new Size(960, 660);
            mainPanel.Location = new Point(35, 25);
            mainPanel.BackColor = Color.White;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(mainPanel);

            // Header
            Panel header = new Panel();
            header.Size = new Size(958, 100);
            header.Location = new Point(0, 0);
            header.BackColor = Color.FromArgb(35, 91, 165);
            mainPanel.Controls.Add(header);

            Label lblTitle = new Label();
            lblTitle.Text = "QUẢN LÝ DỊCH VỤ Y TẾ";
            lblTitle.Font = new Font("Segoe UI", 23, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(280, 15);
            header.Controls.Add(lblTitle);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Lựa chọn dịch vụ và thanh toán đơn hàng";
            lblSubtitle.Font = new Font("Segoe UI", 11);
            lblSubtitle.ForeColor = Color.White;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(330, 62);
            header.Controls.Add(lblSubtitle);

            // Chọn loại dịch vụ
            AddLabel(mainPanel, "LOẠI DỊCH VỤ", 35, 125);

            cboCategory = new ComboBox();
            cboCategory.Location = new Point(35, 155);
            cboCategory.Size = new Size(880, 35);
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.Font = new Font("Segoe UI", 11);
            cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;
            mainPanel.Controls.Add(cboCategory);

            // Danh sách dịch vụ có sẵn
            AddLabel(mainPanel, "DANH SÁCH DỊCH VỤ", 35, 210);

            lstAvailableServices = new ListBox();
            lstAvailableServices.Location = new Point(35, 240);
            lstAvailableServices.Size = new Size(365, 230);
            lstAvailableServices.Font = new Font("Segoe UI", 10);
            lstAvailableServices.BorderStyle = BorderStyle.FixedSingle;
            lstAvailableServices.DoubleClick += LstAvailableServices_DoubleClick;
            mainPanel.Controls.Add(lstAvailableServices);

            // Nút thêm
            btnSelect = CreateButton(">", 435, 275,
                Color.FromArgb(35, 91, 165));
            btnSelect.Click += BtnSelect_Click;
            mainPanel.Controls.Add(btnSelect);

            // Nút xóa một dịch vụ
            btnRemove = CreateButton("<", 435, 330,
                Color.FromArgb(230, 145, 45));
            btnRemove.Click += BtnRemove_Click;
            mainPanel.Controls.Add(btnRemove);

            // Nút xóa tất cả
            btnClearAll = CreateButton("<<", 435, 385,
                Color.FromArgb(210, 65, 65));
            btnClearAll.Click += BtnClearAll_Click;
            mainPanel.Controls.Add(btnClearAll);

            // Danh sách đã chọn
            AddLabel(mainPanel, "DỊCH VỤ ĐÃ CHỌN", 510, 210);

            lstSelectedServices = new ListBox();
            lstSelectedServices.Location = new Point(510, 240);
            lstSelectedServices.Size = new Size(405, 230);
            lstSelectedServices.Font = new Font("Segoe UI", 10);
            lstSelectedServices.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Controls.Add(lstSelectedServices);

            // Khu vực thanh toán
            Panel paymentPanel = new Panel();
            paymentPanel.Location = new Point(35, 490);
            paymentPanel.Size = new Size(880, 145);
            paymentPanel.BackColor = Color.FromArgb(245, 248, 253);
            paymentPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Controls.Add(paymentPanel);

            Label lblPaymentTitle = new Label();
            lblPaymentTitle.Text = "THÔNG TIN THANH TOÁN";
            lblPaymentTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPaymentTitle.ForeColor = Color.FromArgb(35, 91, 165);
            lblPaymentTitle.AutoSize = true;
            lblPaymentTitle.Location = new Point(20, 12);
            paymentPanel.Controls.Add(lblPaymentTitle);

            // Tổng tiền
            Label lblTotalTitle = new Label();
            lblTotalTitle.Text = "Tổng tiền chưa giảm:";
            lblTotalTitle.Location = new Point(20, 55);
            lblTotalTitle.AutoSize = true;
            paymentPanel.Controls.Add(lblTotalTitle);

            lblTotal = CreateValueLabel(210, 55);
            paymentPanel.Controls.Add(lblTotal);

            // Chiết khấu
            Label lblDiscountTitle = new Label();
            lblDiscountTitle.Text = "Tỷ lệ chiết khấu:";
            lblDiscountTitle.Location = new Point(20, 95);
            lblDiscountTitle.AutoSize = true;
            paymentPanel.Controls.Add(lblDiscountTitle);

            cboDiscount = new ComboBox();
            cboDiscount.Location = new Point(210, 90);
            cboDiscount.Size = new Size(120, 30);
            cboDiscount.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDiscount.Font = new Font("Segoe UI", 10);

            cboDiscount.Items.AddRange(new object[]
            {
                "0%", "5%", "10%", "15%", "20%", "30%"
            });

            cboDiscount.SelectedIndex = 0;
            cboDiscount.SelectedIndexChanged += CboDiscount_SelectedIndexChanged;
            paymentPanel.Controls.Add(cboDiscount);

            // Thành tiền
            Label lblPaymentTitle2 = new Label();
            lblPaymentTitle2.Text = "THÀNH TIỀN:";
            lblPaymentTitle2.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPaymentTitle2.ForeColor = Color.FromArgb(35, 91, 165);
            lblPaymentTitle2.Location = new Point(470, 55);
            lblPaymentTitle2.AutoSize = true;
            paymentPanel.Controls.Add(lblPaymentTitle2);

            lblPayment = new Label();
            lblPayment.Text = "0 VNĐ";
            lblPayment.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            lblPayment.ForeColor = Color.FromArgb(220, 65, 65);
            lblPayment.Location = new Point(600, 48);
            lblPayment.AutoSize = true;
            paymentPanel.Controls.Add(lblPayment);
        }

        // Khởi tạo dữ liệu dịch vụ
        private void InitializeServiceData()
        {
            serviceData = new Dictionary<string, List<Service>>();

            serviceData.Add("Khám bệnh", new List<Service>
            {
                new Service("Khám tổng quát", 200000),
                new Service("Khám chuyên khoa", 300000),
                new Service("Khám tim mạch", 250000),
                new Service("Khám da liễu", 180000),
                new Service("Khám tai mũi họng", 150000)
            });

            serviceData.Add("Xét nghiệm", new List<Service>
            {
                new Service("Xét nghiệm máu", 150000),
                new Service("Xét nghiệm nước tiểu", 100000),
                new Service("Xét nghiệm đường huyết", 120000),
                new Service("Xét nghiệm chức năng gan", 250000)
            });

            serviceData.Add("Chụp X-Quang", new List<Service>
            {
                new Service("Chụp X-Quang ngực", 200000),
                new Service("Chụp X-Quang xương", 250000),
                new Service("Chụp X-Quang cột sống", 300000)
            });

            serviceData.Add("Vắc-xin", new List<Service>
            {
                new Service("Vắc-xin cúm", 350000),
                new Service("Vắc-xin viêm gan B", 250000),
                new Service("Vắc-xin uốn ván", 200000),
                new Service("Vắc-xin HPV", 1800000)
            });
        }

        // Nạp danh mục dịch vụ
        private void LoadCategories()
        {
            cboCategory.Items.AddRange(new object[]
            {
                "Khám bệnh",
                "Xét nghiệm",
                "Chụp X-Quang",
                "Vắc-xin"
            });

            cboCategory.SelectedIndex = 0;
        }

        // Thay đổi loại dịch vụ
        private void CboCategory_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            lstAvailableServices.Items.Clear();

            if (cboCategory.SelectedItem == null)
                return;

            string category = cboCategory.SelectedItem.ToString();

            foreach (Service service in serviceData[category])
            {
                lstAvailableServices.Items.Add(service);
            }
        }

        // Nút thêm dịch vụ
        private void BtnSelect_Click(object sender, EventArgs e)
        {
            AddSelectedService();
        }

        // Double Click thêm dịch vụ
        private void LstAvailableServices_DoubleClick(
            object sender, EventArgs e)
        {
            AddSelectedService();
        }

        // Hàm thêm dịch vụ vào danh sách đã chọn
        private void AddSelectedService()
        {
            if (lstAvailableServices.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn dịch vụ cần thêm!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            Service service =
                (Service)lstAvailableServices.SelectedItem;

            bool exists = lstSelectedServices.Items
                .Cast<Service>()
                .Any(s => s.Name == service.Name);

            if (exists)
            {
                MessageBox.Show(
                    "Dịch vụ này đã được chọn!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            lstSelectedServices.Items.Add(service);

            CalculateTotal();
        }

        // Xóa một dịch vụ
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstSelectedServices.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn dịch vụ cần xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            lstSelectedServices.Items.Remove(
                lstSelectedServices.SelectedItem
            );

            CalculateTotal();
        }

        // Xóa toàn bộ dịch vụ
        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            lstSelectedServices.Items.Clear();
            CalculateTotal();
        }

        // Thay đổi mức chiết khấu
        private void CboDiscount_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            CalculateTotal();
        }

        // Tính tổng tiền và thành tiền
        private void CalculateTotal()
        {
            decimal total = 0;

            foreach (Service service in lstSelectedServices.Items)
            {
                total += service.Price;
            }

            decimal discount = 0;

            if (cboDiscount.SelectedItem != null)
            {
                discount = decimal.Parse(
                    cboDiscount.SelectedItem.ToString().Replace("%", "")
                );
            }

            decimal payment = total * (100 - discount) / 100;

            lblTotal.Text = total.ToString("N0") + " VNĐ";
            lblPayment.Text = payment.ToString("N0") + " VNĐ";
        }

        // Hàm tạo Label
        private void AddLabel(
            Control parent, string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(55, 65, 81);
            parent.Controls.Add(lbl);
        }

        // Hàm tạo Button
        private Button CreateButton(
            string text, int x, int y, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(55, 45);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;

            return btn;
        }

        // Hàm tạo Label hiển thị tiền
        private Label CreateValueLabel(int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = "0 VNĐ";
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(35, 91, 165);

            return lbl;
        }
    }
}
